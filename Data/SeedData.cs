using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sjop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sjop.Data
{
    // Inspiration: https://docs.microsoft.com/en-us/aspnet/core/security/authorization/secure-data?view=aspnetcore-3.0
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Read default admin email and password from config
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var adminEmail = config["Admin:Email"];
                var adminPassword = config["Admin:Password"];

                // Check 
                var adminUserId = await EnsureAdminUser(serviceProvider, adminEmail, adminPassword);
                await EnsureRole(serviceProvider, adminUserId, "Admin");

                SeedDB(context);
            }
        }

        public static void SeedDB(ApplicationDbContext context)
        {
            // Seed data here
        }

        private static async Task<string> EnsureAdminUser(IServiceProvider serviceProvider,
                                            string adminUserEmail, string adminUserPassword)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(adminUserEmail);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = adminUserEmail,
                    Email = adminUserEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, adminUserPassword);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string userId, string role)
        {
            IdentityResult identityResult = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                identityResult = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            identityResult = await userManager.AddToRoleAsync(user, role);

            return identityResult;
        }

    }
}