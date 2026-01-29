using Microsoft.AspNetCore.Identity;
using StockMaster.Models.Identity;
using System.Security.Claims;

namespace StockMaster.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed Roles
            string[] roleNames = { "Admin", "Manager", "Operatore", "Viewer" };
            
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed Admin User
            var adminEmail = "admin@stockmaster.local";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    NomeCompleto = "Amministratore Sistema",
                    EmailConfirmed = true,
                    IsAttivo = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed Claims per Admin (permissions)
            var adminClaims = await userManager.GetClaimsAsync(adminUser);
            if (!adminClaims.Any(c => c.Type == "Permission"))
            {
                await userManager.AddClaimAsync(adminUser, new Claim("Permission", "admin"));
                await userManager.AddClaimAsync(adminUser, new Claim("Permission", "stock.manage"));
                await userManager.AddClaimAsync(adminUser, new Claim("Permission", "anagrafiche.edit"));
            }
        }
    }
}