using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using taohi_backend.Models;

namespace taohi_backend.Data
{
    public class AppDbContext : IdentityDbContext<User, UserRole, Guid>
    {
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Text> Texts { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Comment> Comments { get; set; }
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

            // profile
            builder.Entity<User>()
                .HasOne(e => e.UserProfile)
                .WithOne(e => e.User);


            // relationships
            builder.Entity<Relationship>().HasKey(x => new { x.LeftUserId, x.RightUserId });
            builder.Entity<Relationship>()
                .HasOne(e => e.LeftUser)
                .WithMany()
                .HasForeignKey(e => e.LeftUserId);
            builder.Entity<Relationship>()
                .HasOne(e => e.RightUser)
                .WithMany()
                .HasForeignKey(e => e.RightUserId);

            // Video
            builder.Entity<Video>()
                .HasOne(n => n.User)
                .WithMany(a => a.Videos)
                .HasForeignKey(n => n.UserId);
            builder.Entity<Video>()
                .Property(e => e.ContentType)
                .HasConversion(
                    e => e.ToString(),
                    e => (ContentType)Enum.Parse(typeof(ContentType), e));
            // Image
            builder.Entity<Image>()
                .HasOne(n => n.User)
                .WithMany(a => a.Images)
                .HasForeignKey(n => n.UserId);
            builder.Entity<Image>()
                .Property(e => e.ContentType)
                .HasConversion(
                    e => e.ToString(),
                    e => (ContentType)Enum.Parse(typeof(ContentType), e));
            // Text
            builder.Entity<Text>()
                .HasOne(n => n.User)
                .WithMany(a => a.Texts)
                .HasForeignKey(n => n.UserId);
            builder.Entity<Text>()
                .Property(e => e.ContentType)
                .HasConversion(
                    e => e.ToString(),
                    e => (ContentType)Enum.Parse(typeof(ContentType), e));

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
