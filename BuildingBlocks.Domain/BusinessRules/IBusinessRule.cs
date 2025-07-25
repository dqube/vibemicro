namespace BuildingBlocks.Domain.BusinessRules;

/// <summary>
/// Interface for business rules in the domain
/// </summary>
public interface IBusinessRule
{
    /// <summary>
    /// Gets the message that describes the business rule
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Checks if the business rule is satisfied
    /// </summary>
    /// <returns>True if the rule is satisfied, false otherwise</returns>
    bool IsBroken();
} 