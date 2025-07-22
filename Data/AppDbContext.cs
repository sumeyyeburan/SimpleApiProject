using Microsoft.EntityFrameworkCore;
using SimpleApiProject.Models;

namespace SimpleApiProject.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // DbSets represent tables in the database
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Claim> Claims { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserClaim> UserClaims { get; set; }

    // Configure relationships and keys using Fluent API
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite key for UserRole join table (many-to-many User-Role)
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        // Define relationship: UserRole has one User, User has many UserRoles
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        // Define relationship: UserRole has one Role, Role has many UserRoles
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        // Composite key for UserClaim join table (many-to-many User-Claim)
        modelBuilder.Entity<UserClaim>()
            .HasKey(uc => new { uc.UserId, uc.ClaimId });

        // Define relationship: UserClaim has one User, User has many UserClaims
        modelBuilder.Entity<UserClaim>()
            .HasOne(uc => uc.User)
            .WithMany(u => u.UserClaims)
            .HasForeignKey(uc => uc.UserId);

        // Define relationship: UserClaim has one Claim, Claim has many UserClaims
        modelBuilder.Entity<UserClaim>()
            .HasOne(uc => uc.Claim)
            .WithMany(c => c.UserClaims)
            .HasForeignKey(uc => uc.ClaimId);
    }
}
