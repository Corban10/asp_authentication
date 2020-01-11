using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using taohi_backend.Models;

namespace taohi_backend.Data
{
    public class AppDbContext : IdentityDbContext<User, UserRole, Guid>
    {
        public DbSet<Video> Videos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // User
            builder.Entity<User>()
                .HasIndex(e => e.DisplayName)
                .IsUnique();
            builder.Entity<User>()
                .Property(x => x.UserType)
                .HasConversion(
                    e => e.ToString(),
                    e => (UserType)Enum.Parse(typeof(UserType), e));

            // Video
            builder.Entity<Video>()
                .Property(e => e.ContentType)
                .HasConversion(
                    e => e.ToString(),
                    e => (ContentType)Enum.Parse(typeof(ContentType), e));

            base.OnModelCreating(builder);
        }
    }
}
