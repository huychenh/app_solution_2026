using Microsoft.AspNetCore.Identity;

namespace ShopOnline.IdentityServer.Models
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Role (Admin, User)
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            // Seed Admin
            string adminEmail = "admin@local.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new AppUser
                {
                    UserName = adminEmail.Substring(0, adminEmail.IndexOf("@")),
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(admin, "Password@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed User
            string userEmail = "user@local.com";
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var user = new AppUser
                {
                    UserName = userEmail.Substring(0, userEmail.IndexOf("@")),
                    Email = userEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Password@123");
                await userManager.AddToRoleAsync(user, "User");
            }
        }
    }
}
