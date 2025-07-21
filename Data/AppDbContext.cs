using Microsoft.EntityFrameworkCore;
using SimpleApiProject.Models;

namespace SimpleApiProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
        }

        // Entity sets (tables)
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship between User and Role
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Configure many-to-many relationship between User and Claim
            modelBuilder.Entity<UserClaim>()
                .HasKey(uc => new { uc.UserId, uc.ClaimId });

            modelBuilder.Entity<UserClaim>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserClaims)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserClaim>()
                .HasOne(uc => uc.Claim)
                .WithMany(c => c.UserClaims)
                .HasForeignKey(uc => uc.ClaimId);
        }

    }

}

