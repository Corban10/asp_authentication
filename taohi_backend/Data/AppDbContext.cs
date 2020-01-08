﻿using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using taohi_backend.Models;

namespace taohi_backend.Data
{
    public class AppDbContext : IdentityDbContext<User, UserRole, Guid>
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
            //UserManager<User> userManager,
            //RoleManager< UserRole > roleManager)
            //UserDataSeeder.SeedData(userManager, roleManager);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
