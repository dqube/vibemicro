using BuildingBlocks.Domain.Guards;

namespace BuildingBlocks.Domain.Invariants;

/// <summary>
/// Base class for domain invariants providing common functionality
/// </summary>
public abstract class InvariantBase : IInvariant
{
    /// <summary>
    /// Gets the descriptive message about the invariant
    /// </summary>
    public abstract string Message { get; }

    /// <summary>
    /// Gets the unique code for the invariant
    /// </summary>
    public abstract string Code { get; }

    /// <summary>
    /// Checks if the invariant is violated
    /// </summary>
    /// <returns>True if the invariant is violated, false otherwise</returns>
    public abstract bool IsViolated();

    /// <summary>
    /// Returns the string representation of the invariant
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name}: {Message} (Code: {Code})";
    }

    /// <summary>
    /// Checks an invariant and throws an exception if it's violated
    /// </summary>
    /// <param name="invariant">The invariant to check</param>
    /// <exception cref="InvariantViolationException">Thrown when the invariant is violated</exception>
    public static void Check(IInvariant invariant)
    {
        Guard.NotNull(invariant);
        
        if (invariant.IsViolated())
        {
            throw new InvariantViolationException(invariant);
        }
    }

    /// <summary>
    /// Checks multiple invariants and throws an exception if any are violated
    /// </summary>
    /// <param name="invariants">The invariants to check</param>
    /// <exception cref="InvariantViolationException">Thrown when any invariant is violated</exception>
    public static void CheckAll(params IInvariant[] invariants)
    {
        Guard.NotNull(invariants);
        
        foreach (var invariant in invariants)
        {
            Check(invariant);
        }
    }

    /// <summary>
    /// Checks multiple invariants and throws an exception if any are violated
    /// </summary>
    /// <param name="invariants">The invariants to check</param>
    /// <exception cref="InvariantViolationException">Thrown when any invariant is violated</exception>
    public static void CheckAll(IEnumerable<IInvariant> invariants)
    {
        Guard.NotNull(invariants);
        
        foreach (var invariant in invariants)
        {
            Check(invariant);
        }
    }
}

/// <summary>
/// Base class for async domain invariants providing common functionality
/// </summary>
public abstract class AsyncInvariantBase : IAsyncInvariant
{
    /// <summary>
    /// Gets the descriptive message about the invariant
    /// </summary>
    public abstract string Message { get; }

    /// <summary>
    /// Gets the unique code for the invariant
    /// </summary>
    public abstract string Code { get; }

    /// <summary>
    /// Checks if the invariant is violated asynchronously
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the invariant is violated, false otherwise</returns>
    public abstract Task<bool> IsViolatedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the string representation of the invariant
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name}: {Message} (Code: {Code})";
    }

    /// <summary>
    /// Checks an async invariant and throws an exception if it's violated
    /// </summary>
    /// <param name="invariant">The invariant to check</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <exception cref="InvariantViolationException">Thrown when the invariant is violated</exception>
    public static async Task CheckAsync(IAsyncInvariant invariant, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(invariant);
        
        if (await invariant.IsViolatedAsync(cancellationToken))
        {
            throw new InvariantViolationException(invariant.Message, invariant.Code);
        }
    }

    /// <summary>
    /// Checks multiple async invariants and throws an exception if any are violated
    /// </summary>
    /// <param name="invariants">The invariants to check</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <exception cref="InvariantViolationException">Thrown when any invariant is violated</exception>
    public static async Task CheckAllAsync(IEnumerable<IAsyncInvariant> invariants, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(invariants);
        
        foreach (var invariant in invariants)
        {
            await CheckAsync(invariant, cancellationToken);
        }
    }
} 