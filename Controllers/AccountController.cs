using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockMaster.Models.Identity;
using StockMaster.ViewModels.Account;

namespace StockMaster.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Consenti login con username O email
            var user = await _userManager.FindByNameAsync(model.UsernameOrEmail) 
                ?? await _userManager.FindByEmailAsync(model.UsernameOrEmail);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Username o password non validi.");
                return View(model);
            }

            // Verifica se utente è attivo
            if (!user.IsAttivo)
            {
                ModelState.AddModelError(string.Empty, "Utente disabilitato. Contatta l'amministratore.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!, 
                model.Password, 
                model.RememberMe, 
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Aggiorna ultimo accesso
                user.UltimoAccesso = DateTime.Now;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("User {Username} logged in.", user.UserName);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                
                //return RedirectToAction("Index", "Home");
                return RedirectToAction("Index", "Magazzino");
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {Username} account locked out.", model.UsernameOrEmail);
                ModelState.AddModelError(string.Empty, "Account bloccato per troppi tentativi. Riprova tra 15 minuti.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Username o password non validi.");
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: /Account/Register (solo per Admin)
        [Authorize(Roles = "Admin")]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                NomeCompleto = model.NomeCompleto,
                Reparto = model.Reparto,
                EmailConfirmed = true,
                IsAttivo = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("New user {Username} created by admin.", user.UserName);
                TempData["SuccessMessage"] = $"✅ Utente '{user.UserName}' creato con successo";
                return RedirectToAction("Index", "Users", new { area = "Admin" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}