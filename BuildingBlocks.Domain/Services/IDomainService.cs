namespace BuildingBlocks.Domain.Services;

/// <summary>
/// Marker interface for domain services
/// Domain services contain business logic that doesn't naturally fit within an entity or value object
/// </summary>
public interface IDomainService
{
    // Marker interface - no methods required
    // Used for DI registration and architectural identification
} 