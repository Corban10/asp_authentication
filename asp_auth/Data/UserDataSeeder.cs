using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using asp_auth.Models;

namespace asp_auth.Data
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
            await userManager.AddToRoleAsync(user, user.UserType.ToString());
            var claims = new List<Claim>
            {
                new Claim("IsActive", user.IsActive.ToString()), // for blacklisting tokens
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                new Claim(ClaimTypes.DateOfBirth, user.DateOfBirth.ToString()),
            };
            await userManager.AddClaimsAsync(user, claims);
        }
        public static async Task SeedUsers(UserManager<User> userManager)
        {
            const string userOneEmail = "userone@email.com";
            const string userTwoEmail = "usertwo@email.com";
            if (await userManager.FindByEmailAsync(userOneEmail) == null)
            {
                var user = new User();
                user.DisplayName = "User one";
                user.FirstName = "user";
                user.LastName = "one";
                user.DateOfBirth = new DateTime(1993, 6, 21);
                user.IsActive = true;
                user.UserType = UserType.Admin;
                user.Email = userOneEmail;
                user.UserName = userOneEmail;

                var result = await userManager.CreateAsync(user, "password");
                if (result.Succeeded)
                    await AddRolesAndClaims(userManager, user);
            }
            if (await userManager.FindByEmailAsync(userTwoEmail) == null)
            {
                var user = new User();
                user.DisplayName = "User two";
                user.FirstName = "user";
                user.LastName = "two";
                user.DateOfBirth = new DateTime(1993, 6, 21);
                user.IsActive = true;
                user.UserType = UserType.Admin;
                user.Email = userTwoEmail;
                user.UserName = userTwoEmail;

                var result = await userManager.CreateAsync(user, "password");
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
                await roleManager.CreateAsync(role);
            }
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var role = new UserRole();
                role.Name = "Admin";
                await roleManager.CreateAsync(role);
            }
        }
    }
}
