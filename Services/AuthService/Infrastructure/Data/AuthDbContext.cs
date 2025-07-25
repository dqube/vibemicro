using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data.Configurations;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Data.Interceptors;
using BuildingBlocks.Application.Outbox;
using BuildingBlocks.Application.Inbox;

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

    /// <summary>
    /// Outbox messages table
    /// </summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <summary>
    /// Inbox messages table
    /// </summary>
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Add interceptors
        optionsBuilder.AddInterceptors(
            new AuditInterceptor(),
            new DomainEventInterceptor(),
            new SoftDeleteInterceptor()
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations for domain entities
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new RegistrationTokenConfiguration());

        // Configure outbox messages
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages", "auth");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MessageType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Content).HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(e => e.Destination).HasMaxLength(200);
            entity.Property(e => e.CorrelationId).HasMaxLength(100);
            entity.Property(e => e.Error).HasMaxLength(2000);
            
            // Convert Dictionary to JSON
            entity.Property(e => e.Headers)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new());
            
            entity.Property(e => e.Metadata)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new());

            // Indexes
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.Status, e.CreatedAt });
        });

        // Configure inbox messages
        modelBuilder.Entity<InboxMessage>(entity =>
        {
            entity.ToTable("InboxMessages", "auth");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MessageType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Content).HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(e => e.Source).HasMaxLength(200);
            entity.Property(e => e.CorrelationId).HasMaxLength(100);
            entity.Property(e => e.MessageGroup).HasMaxLength(100);
            entity.Property(e => e.Error).HasMaxLength(2000);
            
            // Convert Dictionary to JSON
            entity.Property(e => e.Headers)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new());
            
            entity.Property(e => e.Metadata)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new());

            // Indexes
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ReceivedAt);
            entity.HasIndex(e => new { e.Status, e.ReceivedAt });
            entity.HasIndex(e => new { e.MessageGroup, e.SequenceNumber });
        });

        // Set schema
        modelBuilder.HasDefaultSchema("auth");
    }
} 