
using Microsoft.AspNetCore.Identity;

namespace BugBeading.Data
{
    public static class IdentitySeeder
    {
        public static async Task InitializeAsync(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            const string adminRole = "Admin";

            var email = configuration["AdminSeed:Email"];
            var password = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "AdminSeed:Email and AdminSeed:Password must be configured.");
            }

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                var roleResult = await roleManager.CreateAsync(
                    new IdentityRole(adminRole));

                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            var adminUser = await userManager.FindByEmailAsync(email);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var userResult = await userManager.CreateAsync(
                    adminUser, password);

                if (!userResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", userResult.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            {
                var assignmentResult = await userManager.AddToRoleAsync(
                    adminUser, adminRole);

                if (!assignmentResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", assignmentResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
