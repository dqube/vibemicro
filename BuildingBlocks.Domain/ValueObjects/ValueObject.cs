namespace BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// Base record for value objects in the domain - provides immutability and structural equality
/// </summary>
public abstract record ValueObject
{
    /// <summary>
    /// Gets the atomic values that define the value object's equality (optional override for custom behavior)
    /// </summary>
    /// <returns>The atomic values</returns>
    protected virtual IEnumerable<object?> GetEqualityComponents()
    {
        // Records provide structural equality by default, so this is mainly for custom scenarios
        yield break;
    }

    /// <summary>
    /// Validates the value object state - override in derived records for validation
    /// </summary>
    protected virtual void Validate()
    {
        // Override in derived records to add validation logic
    }
} 