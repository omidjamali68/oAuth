using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Auth.Infrastructure.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string userRoleName = "User";

            string adminRoleName = "Admin";
            string adminEmail = "admin@example.com";
            string adminPassword = "Admin@123";
            string adminUserName = "09177870290";

            if (!await roleManager.RoleExistsAsync(userRoleName))
            {
                await roleManager.CreateAsync(new IdentityRole(userRoleName));
            }

            if (!await roleManager.RoleExistsAsync(adminRoleName))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRoleName));
            }

            var existingUser = await userManager.FindByNameAsync(adminUserName);
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminUserName,
                    NormalizedUserName = adminUserName,
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpper(),
                    PhoneNumberConfirmed = true,
                    EmailConfirmed = true,
                    PhoneNumber = adminUserName,
                    FullName = "Admin",
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    CreatedAt = DateTime.Now
                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, adminRoleName);
                }
                else
                {
                    throw new Exception("خطا در ساخت کاربر پیش‌فرض: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

}
