using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using taohi_backend.Models;

namespace taohi_backend.Data
{
    public static class UserDataSeeder
    {
        public static void SeedData(UserManager<User> userManager, RoleManager<UserRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }
        public static void SeedUsers(UserManager<User> userManager)
        {
            if (userManager.FindByNameAsync("corban10").Result == null)
            {
                User user = new User();
                user.UserName = "corban";
                user.Email = "corbanhirawani@gmail.com";

                IdentityResult result = userManager.CreateAsync(user, "password").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "User").Wait();
                    userManager.AddToRoleAsync(user, "Moderator").Wait();
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
            if (userManager.FindByNameAsync("maraea").Result == null)
            {
                User user = new User();
                user.UserName = "maraea";
                user.Email = "maraea@heitikicreatives.com";

                IdentityResult result = userManager.CreateAsync(user, "password").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "User").Wait();
                    userManager.AddToRoleAsync(user, "Moderator").Wait();
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
            if (userManager.FindByNameAsync("peter").Result == null)
            {
                User user = new User();
                user.UserName = "peter";
                user.Email = "petertanerapalmer@gmail.com";

                IdentityResult result = userManager.CreateAsync(user, "password").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "User").Wait();
                    userManager.AddToRoleAsync(user, "Moderator").Wait();
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }

        public static void SeedRoles(RoleManager<UserRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("User").Result)
            {
                UserRole role = new UserRole();
                role.Name = "User";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                Console.WriteLine($"Creating role: {role.Name}...{roleResult}");
            }

            if (!roleManager.RoleExistsAsync("Moderator").Result)
            {
                UserRole role = new UserRole();
                role.Name = "Moderator";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                Console.WriteLine($"Creating role: {role.Name}...{roleResult}");
            }

            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                UserRole role = new UserRole();
                role.Name = "Admin";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                Console.WriteLine($"Creating role: {role.Name}...{roleResult}");
            }
        }
    }
}
