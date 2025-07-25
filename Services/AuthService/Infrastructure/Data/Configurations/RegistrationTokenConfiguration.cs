using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuthService.Domain.Entities;
using AuthService.Domain.StronglyTypedIds;
using AuthService.Domain.ValueObjects;

namespace AuthService.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for RegistrationToken
/// </summary>
public class RegistrationTokenConfiguration : IEntityTypeConfiguration<RegistrationToken>
{
    public void Configure(EntityTypeBuilder<RegistrationToken> builder)
    {
        builder.ToTable("RegistrationTokens");

        // Primary key
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, value => new TokenId(value))
            .HasColumnName("TokenId");

        // User reference
        builder.Property(t => t.UserId)
            .HasConversion(id => id.Value, value => new UserId(value))
            .IsRequired();

        builder.HasIndex(t => t.UserId);

        // Token type (value object)
        builder.Property(t => t.TokenType)
            .HasConversion(type => type.Value, value => new TokenType(value))
            .HasMaxLength(20)
            .IsRequired();

        // Properties
        builder.Property(t => t.Expiration)
            .IsRequired();

        builder.Property(t => t.IsUsed)
            .HasDefaultValue(false);

        // Audit properties
        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.CreatedBy)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new UserId(value.Value) : null)
            .IsRequired(false);

        // Indexes for performance
        builder.HasIndex(t => new { t.UserId, t.TokenType });
        builder.HasIndex(t => t.Expiration);
        builder.HasIndex(t => t.IsUsed);

        // Ignore domain events (handled by interceptor)
        builder.Ignore(t => t.DomainEvents);
    }
} 