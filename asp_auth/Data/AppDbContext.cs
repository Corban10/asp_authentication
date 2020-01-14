using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using asp_auth.Models;

namespace asp_auth.Data
{
    public class AppDbContext : IdentityDbContext<User, UserRole, Guid>
    {
        public DbSet<Message> Messages { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User
            builder.Entity<User>()
                .HasIndex(e => e.DisplayName)
                .IsUnique();
            builder.Entity<User>()
                .Property(x => x.UserType)
                .HasConversion(
                    e => e.ToString(),
                    e => (UserType)Enum.Parse(typeof(UserType), e));

            // pms
            builder.Entity<Message>()
                .HasOne(e => e.Sender)
                .WithMany()
                .HasForeignKey(e => e.SenderId);
            builder.Entity<Message>()
                .HasOne(e => e.Receiver)
                .WithMany()
                .HasForeignKey(e => e.ReceiverId);
        }
    }
}
