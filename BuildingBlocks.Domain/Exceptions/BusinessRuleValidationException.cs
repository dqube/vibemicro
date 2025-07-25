using BuildingBlocks.Domain.BusinessRules;

namespace BuildingBlocks.Domain.Exceptions;

/// <summary>
/// Exception thrown when a business rule validation fails
/// </summary>
public class BusinessRuleValidationException : DomainException
{
    /// <summary>
    /// Gets the business rule that was violated
    /// </summary>
    public IBusinessRule BrokenRule { get; }

    /// <summary>
    /// Initializes a new instance of the BusinessRuleValidationException class
    /// </summary>
    /// <param name="brokenRule">The business rule that was violated</param>
    public BusinessRuleValidationException(IBusinessRule brokenRule)
        : base(brokenRule.Message, "BUSINESS_RULE_VIOLATION", brokenRule)
    {
        BrokenRule = brokenRule;
    }

    /// <summary>
    /// Initializes a new instance of the BusinessRuleValidationException class
    /// </summary>
    /// <param name="brokenRule">The business rule that was violated</param>
    /// <param name="innerException">The inner exception</param>
    public BusinessRuleValidationException(IBusinessRule brokenRule, Exception innerException)
        : base(brokenRule.Message, "BUSINESS_RULE_VIOLATION", brokenRule, innerException)
    {
        BrokenRule = brokenRule;
    }

    /// <summary>
    /// Returns a string representation of the exception
    /// </summary>
    public override string ToString()
    {
        return $"Business rule violation: {BrokenRule.Message} (Rule: {BrokenRule.GetType().Name})";
    }
} 