using System;
using System.Collections.Generic;
using System.Linq;
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
            var claims = new List<Claim>
            {
                new Claim("UserType", user.UserType.ToString()),
                new Claim("IsActive", user.IsActive.ToString())
            };
            await userManager.AddClaimsAsync(user, claims);

            var oldClaim = (await userManager.GetClaimsAsync(user)).Where(claim => claim.Type == "IsActive").First();
            var newClaim = new Claim("IsActive", (!user.IsActive).ToString());
            await userManager.ReplaceClaimAsync(user, oldClaim, newClaim);
        }
        public static async Task SeedUsers(UserManager<User> userManager)
        {
            const string corbansEmail = "corbanhirawani@gmail.com";
            const string maraeasEmail = "maraea@heitikicreatives.com";
            const string petersEmail = "petertanerapalmer@gmail.com";
            if (await userManager.FindByEmailAsync(corbansEmail) == null)
            {
                var user = new User();
                user.FirstName = "corban";
                user.LastName = "hirawani";
                user.Age = 26;
                user.IsActive = true;
                user.UserType = UserType.Heitiki;
                user.Email = corbansEmail;
                user.UserName = corbansEmail;

                IdentityResult result = await userManager.CreateAsync(user, "password");
                if (result.Succeeded)
                    await AddRolesAndClaims(userManager, user);
            }
            if (await userManager.FindByEmailAsync(maraeasEmail) == null)
            {
                var user = new User();
                user.FirstName = "maraea";
                user.LastName = "davies";
                user.Age = 21;
                user.IsActive = true;
                user.UserType = UserType.Heitiki;
                user.Email = maraeasEmail;
                user.UserName = maraeasEmail;

                IdentityResult result = await userManager.CreateAsync(user, "password");
                if (result.Succeeded)
                    await AddRolesAndClaims(userManager, user);
            }
            if (await userManager.FindByEmailAsync(petersEmail) == null)
            {
                var user = new User();
                user.FirstName = "peter";
                user.LastName = "palmer";
                user.Age = 100;
                user.IsActive = true;
                user.UserType = UserType.Heitiki;
                user.Email = petersEmail;
                user.UserName = petersEmail;

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
