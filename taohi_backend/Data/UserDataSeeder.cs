using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using taohi_backend.Models;

namespace taohi_backend.Data
{
    public static class UserDataSeeder
    {
        public static void SeedData(UserManager<User> userManager, RoleManager<UserRole> roleManager)
        {
            SeedRoles(roleManager).Wait();
            SeedUsers(userManager).Wait();
        }
        public static async Task AddRolesAndClaims(UserManager<User> userManager, User user)
        {
            await userManager.AddToRoleAsync(user, "User");
            await userManager.AddToRoleAsync(user, "Moderator");
            await userManager.AddToRoleAsync(user, "Admin");
            var claim = new Claim("ContentType", user.ContentType.ToString());
            await userManager.AddClaimAsync(user, claim);
        }
        public static async Task SeedUsers(UserManager<User> userManager)
        {
            if (await userManager.FindByNameAsync("corban10") == null)
            {
                var user = new User();
                user.UserName = "corban";
                user.Email = "corbanhirawani@gmail.com";
                user.ContentType = ContentType.Heitiki;

                IdentityResult result = await userManager.CreateAsync(user, "password");
                if (result.Succeeded)
                    await AddRolesAndClaims(userManager, user);
            }
            if (await userManager.FindByNameAsync("maraea") == null)
            {
                var user = new User();
                user.UserName = "maraea";
                user.Email = "maraea@heitikicreatives.com";
                user.ContentType = ContentType.Heitiki;

                IdentityResult result = await userManager.CreateAsync(user, "password");
                if (result.Succeeded)
                    await AddRolesAndClaims(userManager, user);
            }
            if (await userManager.FindByNameAsync("peter") == null)
            {
                var user = new User();
                user.UserName = "peter";
                user.Email = "petertanerapalmer@gmail.com";
                user.ContentType = ContentType.Heitiki;

                IdentityResult result = await userManager.CreateAsync(user, "password");
                if (result.Succeeded)
                    await AddRolesAndClaims(userManager, user);
            }
        }

        public static async Task SeedRoles(RoleManager<UserRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("User"))
            {
                var role = new UserRole();
                role.Name = "User";
                var roleResult = await roleManager.CreateAsync(role);
                Console.WriteLine($"Creating role: {role.Name}...{roleResult}");
            }

            if (!await roleManager.RoleExistsAsync("Moderator"))
            {
                var role = new UserRole();
                role.Name = "Moderator";
                var roleResult = await roleManager.CreateAsync(role);
                Console.WriteLine($"Creating role: {role.Name}...{roleResult}");
            }

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var role = new UserRole();
                role.Name = "Admin";
                var roleResult = await roleManager.CreateAsync(role);
                Console.WriteLine($"Creating role: {role.Name}...{roleResult}");
            }
        }
    }
}
