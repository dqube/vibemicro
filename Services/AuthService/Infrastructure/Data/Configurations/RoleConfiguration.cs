using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;

namespace AuthService.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Role
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        // Primary key
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasConversion(id => id.Value, value => new RoleId(value))
            .HasColumnName("RoleId")
            .ValueGeneratedNever(); // Roles have predefined IDs

        // Properties
        builder.Property(r => r.Name)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(r => r.Name).IsUnique();

        builder.Property(r => r.Description)
            .HasMaxLength(255)
            .IsRequired(false);

        // Audit properties
        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.CreatedBy)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new UserId(value.Value) : null)
            .IsRequired(false);

        builder.Property(r => r.UpdatedAt)
            .IsRequired(false);

        builder.Property(r => r.UpdatedBy)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new UserId(value.Value) : null)
            .IsRequired(false);

        // Seed data
        builder.HasData(
            new
            {
                Id = new RoleId(1),
                Name = "Cashier",
                Description = "Can process sales and returns",
                CreatedAt = DateTime.UtcNow
            },
            new
            {
                Id = new RoleId(2),
                Name = "Supervisor",
                Description = "Can override transactions and manage registers",
                CreatedAt = DateTime.UtcNow
            },
            new
            {
                Id = new RoleId(3),
                Name = "Manager",
                Description = "Full store operations access",
                CreatedAt = DateTime.UtcNow
            },
            new
            {
                Id = new RoleId(4),
                Name = "Admin",
                Description = "System administration access",
                CreatedAt = DateTime.UtcNow
            },
            new
            {
                Id = new RoleId(5),
                Name = "Inventory",
                Description = "Inventory management access",
                CreatedAt = DateTime.UtcNow
            },
            new
            {
                Id = new RoleId(6),
                Name = "Reporting",
                Description = "Reporting and analytics access",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
} 