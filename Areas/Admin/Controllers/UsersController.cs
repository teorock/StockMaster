using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockMaster.Models.Identity;
using StockMaster.ViewModels.Admin;
using System.Security.Claims;

namespace StockMaster.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "CanManageUsers")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            var userViewModels = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                
                userViewModels.Add(new UserListViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    NomeCompleto = user.NomeCompleto,
                    Reparto = user.Reparto,
                    IsAttivo = user.IsAttivo,
                    UltimoAccesso = user.UltimoAccesso,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Utente non trovato";
                return RedirectToAction(nameof(Index));
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                NomeCompleto = user.NomeCompleto,
                Reparto = user.Reparto,
                IsAttivo = user.IsAttivo,
                CurrentRoles = userRoles.ToList(),
                AllRoles = allRoles.Select(r => r.Name!).ToList()
            };

            return View(viewModel);
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllRoles = (await _roleManager.Roles.ToListAsync()).Select(r => r.Name!).ToList();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Utente non trovato";
                return RedirectToAction(nameof(Index));
            }

            user.Email = model.Email;
            user.NomeCompleto = model.NomeCompleto;
            user.Reparto = model.Reparto;
            user.IsAttivo = model.IsAttivo;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                model.AllRoles = (await _roleManager.Roles.ToListAsync()).Select(r => r.Name!).ToList();
                return View(model);
            }

            // Aggiorna ruoli
            var currentRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.SelectedRoles ?? new List<string>();

            // Rimuovi ruoli non più selezionati
            var rolesToRemove = currentRoles.Except(selectedRoles).ToList();
            if (rolesToRemove.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            }

            // Aggiungi nuovi ruoli
            var rolesToAdd = selectedRoles.Except(currentRoles).ToList();
            if (rolesToAdd.Any())
            {
                await _userManager.AddToRolesAsync(user, rolesToAdd);
            }

            _logger.LogInformation("User {UserId} updated by {AdminUser}", user.Id, User.Identity!.Name);
            TempData["SuccessMessage"] = $"✅ Utente '{user.UserName}' aggiornato";
            
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Users/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Utente non trovato";
                return RedirectToAction(nameof(Index));
            }

            // Non permettere di disabilitare se stesso
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["ErrorMessage"] = "❌ Non puoi disabilitare il tuo stesso account";
                return RedirectToAction(nameof(Index));
            }

            user.IsAttivo = !user.IsAttivo;
            await _userManager.UpdateAsync(user);

            var status = user.IsAttivo ? "abilitato" : "disabilitato";
            TempData["SuccessMessage"] = $"✅ Utente '{user.UserName}' {status}";
            
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Users/ResetPassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                TempData["ErrorMessage"] = "Password deve essere almeno 6 caratteri";
                return RedirectToAction(nameof(Edit), new { id });
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Utente non trovato";
                return RedirectToAction(nameof(Index));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation("Password reset for user {UserId} by {AdminUser}", user.Id, User.Identity!.Name);
                TempData["SuccessMessage"] = $"✅ Password reimpostata per '{user.UserName}'";
            }
            else
            {
                TempData["ErrorMessage"] = "❌ Errore reset password: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Edit), new { id });
        }

        // POST: Admin/Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Utente non trovato";
                return RedirectToAction(nameof(Index));
            }

            // Non permettere di eliminare se stesso
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["ErrorMessage"] = "❌ Non puoi eliminare il tuo stesso account";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogWarning("User {UserId} deleted by {AdminUser}", user.Id, User.Identity!.Name);
                TempData["SuccessMessage"] = $"✅ Utente '{user.UserName}' eliminato";
            }
            else
            {
                TempData["ErrorMessage"] = "❌ Errore eliminazione: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}