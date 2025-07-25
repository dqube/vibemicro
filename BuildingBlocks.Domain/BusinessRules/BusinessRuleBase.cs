namespace BuildingBlocks.Domain.BusinessRules;

/// <summary>
/// Base class for business rules
/// </summary>
public abstract class BusinessRuleBase : IBusinessRule
{
    /// <summary>
    /// Gets the message that describes the business rule
    /// </summary>
    public abstract string Message { get; }

    /// <summary>
    /// Checks if the business rule is satisfied
    /// </summary>
    /// <returns>True if the rule is satisfied, false otherwise</returns>
    public abstract bool IsBroken();

    /// <summary>
    /// Returns the string representation of the business rule
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name}: {Message}";
    }
} 