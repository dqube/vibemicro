using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;

namespace AuthService.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for User
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        // Primary key
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(id => id.Value, value => new UserId(value))
            .HasColumnName("UserId");

        // Username (value object)
        builder.Property(u => u.Username)
            .HasConversion(username => username.Value, value => new Username(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(u => u.Username).IsUnique();

        // Email (value object)
        builder.Property(u => u.Email)
            .HasConversion(email => email.Value, value => new Email(value))
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();

        // Password hash (value object)
        builder.OwnsOne(u => u.PasswordHash, pwdBuilder =>
        {
            pwdBuilder.Property(p => p.Hash)
                .HasColumnName("PasswordHash")
                .IsRequired();

            pwdBuilder.Property(p => p.Salt)
                .HasColumnName("PasswordSalt")
                .IsRequired();
        });

        // Simple properties
        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.FailedLoginAttempts)
            .HasDefaultValue(0);

        builder.Property(u => u.LockoutEnd)
            .IsRequired(false);

        // Role IDs collection
        builder.Property(u => u.RoleIds)
            .HasConversion(
                roleIds => string.Join(',', roleIds.Select(r => r.Value)),
                value => value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => new RoleId(int.Parse(id)))
                    .ToList())
            .HasColumnName("RoleIds")
            .HasMaxLength(200);

        // Audit properties
        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.CreatedBy)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new UserId(value.Value) : null)
            .IsRequired(false);

        builder.Property(u => u.UpdatedAt)
            .IsRequired(false);

        builder.Property(u => u.UpdatedBy)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new UserId(value.Value) : null)
            .IsRequired(false);

        // Ignore domain events (handled by interceptor)
        builder.Ignore(u => u.DomainEvents);
    }
} 