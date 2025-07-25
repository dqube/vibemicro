using BuildingBlocks.Domain.Guards;
using System.Text;

namespace BuildingBlocks.Domain.Invariants;

/// <summary>
/// Represents a composite invariant that combines multiple invariants with logical operations
/// </summary>
public sealed class CompositeInvariant : InvariantBase
{
    private readonly List<IInvariant> _invariants;
    private readonly InvariantLogic _logic;

    /// <summary>
    /// Gets the descriptive message about the invariant
    /// </summary>
    public override string Message { get; }

    /// <summary>
    /// Gets the unique code for the invariant
    /// </summary>
    public override string Code { get; }

    /// <summary>
    /// Gets the individual invariants
    /// </summary>
    public IReadOnlyList<IInvariant> Invariants => _invariants.AsReadOnly();

    /// <summary>
    /// Gets the logic used to combine the invariants
    /// </summary>
    public InvariantLogic Logic => _logic;

    /// <summary>
    /// Initializes a new instance of the CompositeInvariant class
    /// </summary>
    /// <param name="logic">The logic to use when combining invariants</param>
    /// <param name="invariants">The invariants to combine</param>
    public CompositeInvariant(InvariantLogic logic, params IInvariant[] invariants)
        : this(logic, (IEnumerable<IInvariant>)invariants)
    {
    }

    /// <summary>
    /// Initializes a new instance of the CompositeInvariant class
    /// </summary>
    /// <param name="logic">The logic to use when combining invariants</param>
    /// <param name="invariants">The invariants to combine</param>
    public CompositeInvariant(InvariantLogic logic, IEnumerable<IInvariant> invariants)
    {
        _logic = logic;
        _invariants = Guard.NotNullOrEmpty(invariants?.ToList(), nameof(invariants));

        if (_invariants.Count == 0)
            throw new ArgumentException("At least one invariant must be provided", nameof(invariants));

        Message = GenerateMessage();
        Code = GenerateCode();
    }

    /// <summary>
    /// Checks if the invariant is violated
    /// </summary>
    /// <returns>True if the invariant is violated, false otherwise</returns>
    public override bool IsViolated()
    {
        return _logic switch
        {
            InvariantLogic.And => _invariants.Any(i => i.IsViolated()),
            InvariantLogic.Or => _invariants.All(i => i.IsViolated()),
            InvariantLogic.Nand => !_invariants.Any(i => i.IsViolated()),
            InvariantLogic.Nor => !_invariants.All(i => i.IsViolated()),
            _ => throw new InvalidOperationException($"Unknown invariant logic: {_logic}")
        };
    }

    /// <summary>
    /// Gets the violated invariants
    /// </summary>
    /// <returns>The invariants that are currently violated</returns>
    public IEnumerable<IInvariant> GetViolatedInvariants()
    {
        return _invariants.Where(i => i.IsViolated());
    }

    /// <summary>
    /// Gets the satisfied invariants
    /// </summary>
    /// <returns>The invariants that are currently satisfied</returns>
    public IEnumerable<IInvariant> GetSatisfiedInvariants()
    {
        return _invariants.Where(i => !i.IsViolated());
    }

    private string GenerateMessage()
    {
        var sb = new StringBuilder();
        sb.Append($"Composite invariant ({_logic}): ");
        
        for (int i = 0; i < _invariants.Count; i++)
        {
            if (i > 0)
            {
                sb.Append(_logic == InvariantLogic.And || _logic == InvariantLogic.Nand ? " AND " : " OR ");
            }
            sb.Append(_invariants[i].Message);
        }

        return sb.ToString();
    }

    private string GenerateCode()
    {
        var codes = _invariants.Select(i => i.Code).OrderBy(c => c);
        return $"COMPOSITE_{_logic}_{string.Join("_", codes)}";
    }

    /// <summary>
    /// Creates a composite invariant using AND logic
    /// </summary>
    /// <param name="invariants">The invariants to combine</param>
    /// <returns>A composite invariant using AND logic</returns>
    public static CompositeInvariant And(params IInvariant[] invariants)
    {
        return new CompositeInvariant(InvariantLogic.And, invariants);
    }

    /// <summary>
    /// Creates a composite invariant using OR logic
    /// </summary>
    /// <param name="invariants">The invariants to combine</param>
    /// <returns>A composite invariant using OR logic</returns>
    public static CompositeInvariant Or(params IInvariant[] invariants)
    {
        return new CompositeInvariant(InvariantLogic.Or, invariants);
    }

    /// <summary>
    /// Creates a composite invariant using NAND logic
    /// </summary>
    /// <param name="invariants">The invariants to combine</param>
    /// <returns>A composite invariant using NAND logic</returns>
    public static CompositeInvariant Nand(params IInvariant[] invariants)
    {
        return new CompositeInvariant(InvariantLogic.Nand, invariants);
    }

    /// <summary>
    /// Creates a composite invariant using NOR logic
    /// </summary>
    /// <param name="invariants">The invariants to combine</param>
    /// <returns>A composite invariant using NOR logic</returns>
    public static CompositeInvariant Nor(params IInvariant[] invariants)
    {
        return new CompositeInvariant(InvariantLogic.Nor, invariants);
    }
}

/// <summary>
/// Defines the logical operations for combining invariants
/// </summary>
public enum InvariantLogic
{
    /// <summary>
    /// All invariants must be satisfied (none violated)
    /// </summary>
    And,

    /// <summary>
    /// At least one invariant must be satisfied (not all violated)
    /// </summary>
    Or,

    /// <summary>
    /// Not all invariants must be satisfied (at least one violated)
    /// </summary>
    Nand,

    /// <summary>
    /// No invariants must be satisfied (all violated)
    /// </summary>
    Nor
} 