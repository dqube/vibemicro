using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data.Configurations;
using BuildingBlocks.Infrastructure.Data.Context;

namespace AuthService.Infrastructure.Data;

/// <summary>
/// Database context for AuthService
/// </summary>
public class AuthDbContext : DbContextBase
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Users table
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Roles table
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>
    /// Registration tokens table
    /// </summary>
    public DbSet<RegistrationToken> RegistrationTokens => Set<RegistrationToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new RegistrationTokenConfiguration());

        // Set schema
        modelBuilder.HasDefaultSchema("auth");
    }
} 