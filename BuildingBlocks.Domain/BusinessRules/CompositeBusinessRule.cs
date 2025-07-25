using System.Text;

namespace BuildingBlocks.Domain.BusinessRules;

/// <summary>
/// Business rule that combines multiple business rules
/// </summary>
public class CompositeBusinessRule : IBusinessRule
{
    private readonly List<IBusinessRule> _rules;
    private readonly CompositeRuleLogic _logic;

    /// <summary>
    /// Gets the message that describes the business rule
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// Gets the individual business rules
    /// </summary>
    public IReadOnlyList<IBusinessRule> Rules => _rules.AsReadOnly();

    /// <summary>
    /// Gets the logic used to combine the rules
    /// </summary>
    public CompositeRuleLogic Logic => _logic;

    /// <summary>
    /// Initializes a new instance of the CompositeBusinessRule class
    /// </summary>
    /// <param name="logic">The logic to use when combining rules</param>
    /// <param name="rules">The business rules to combine</param>
    public CompositeBusinessRule(CompositeRuleLogic logic, params IBusinessRule[] rules)
        : this(logic, (IEnumerable<IBusinessRule>)rules)
    {
    }

    /// <summary>
    /// Initializes a new instance of the CompositeBusinessRule class
    /// </summary>
    /// <param name="logic">The logic to use when combining rules</param>
    /// <param name="rules">The business rules to combine</param>
    public CompositeBusinessRule(CompositeRuleLogic logic, IEnumerable<IBusinessRule> rules)
    {
        _logic = logic;
        _rules = rules?.ToList() ?? throw new ArgumentNullException(nameof(rules));
        
        if (_rules.Count == 0)
            throw new ArgumentException("At least one business rule must be provided", nameof(rules));

        Message = GenerateMessage();
    }

    /// <summary>
    /// Checks if the business rule is satisfied
    /// </summary>
    /// <returns>True if the rule is satisfied, false otherwise</returns>
    public bool IsBroken()
    {
        return _logic switch
        {
            CompositeRuleLogic.And => _rules.Any(rule => rule.IsBroken()),
            CompositeRuleLogic.Or => _rules.All(rule => rule.IsBroken()),
            _ => throw new InvalidOperationException($"Unsupported composite rule logic: {_logic}")
        };
    }

    /// <summary>
    /// Gets the broken rules
    /// </summary>
    /// <returns>The rules that are broken</returns>
    public IEnumerable<IBusinessRule> GetBrokenRules()
    {
        return _rules.Where(rule => rule.IsBroken());
    }

    /// <summary>
    /// Adds a business rule to the composite
    /// </summary>
    /// <param name="rule">The business rule to add</param>
    public void AddRule(IBusinessRule rule)
    {
        if (rule == null)
            throw new ArgumentNullException(nameof(rule));

        _rules.Add(rule);
        Message = GenerateMessage();
    }

    /// <summary>
    /// Removes a business rule from the composite
    /// </summary>
    /// <param name="rule">The business rule to remove</param>
    /// <returns>True if the rule was removed, false otherwise</returns>
    public bool RemoveRule(IBusinessRule rule)
    {
        if (rule == null)
            return false;

        var removed = _rules.Remove(rule);
        if (removed)
        {
            Message = GenerateMessage();
        }
        return removed;
    }

    /// <summary>
    /// Creates a composite business rule with AND logic
    /// </summary>
    /// <param name="rules">The business rules to combine</param>
    /// <returns>A new composite business rule</returns>
    public static CompositeBusinessRule And(params IBusinessRule[] rules)
    {
        return new CompositeBusinessRule(CompositeRuleLogic.And, rules);
    }

    /// <summary>
    /// Creates a composite business rule with OR logic
    /// </summary>
    /// <param name="rules">The business rules to combine</param>
    /// <returns>A new composite business rule</returns>
    public static CompositeBusinessRule Or(params IBusinessRule[] rules)
    {
        return new CompositeBusinessRule(CompositeRuleLogic.Or, rules);
    }

    /// <summary>
    /// Generates the message for the composite rule
    /// </summary>
    private string GenerateMessage()
    {
        if (_rules.Count == 1)
            return _rules[0].Message;

        var sb = new StringBuilder();
        var connector = _logic == CompositeRuleLogic.And ? " AND " : " OR ";
        
        sb.Append("(");
        for (int i = 0; i < _rules.Count; i++)
        {
            if (i > 0)
                sb.Append(connector);
            sb.Append(_rules[i].Message);
        }
        sb.Append(")");

        return sb.ToString();
    }

    /// <summary>
    /// Returns the string representation of the composite business rule
    /// </summary>
    public override string ToString()
    {
        return $"CompositeBusinessRule[{_logic}]: {Message}";
    }
}

/// <summary>
/// Enumeration for composite business rule logic
/// </summary>
public enum CompositeRuleLogic
{
    /// <summary>
    /// All rules must be satisfied (none broken)
    /// </summary>
    And,

    /// <summary>
    /// At least one rule must be satisfied (not all broken)
    /// </summary>
    Or
} 