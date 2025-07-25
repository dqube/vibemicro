namespace BuildingBlocks.Domain.Invariants;

/// <summary>
/// Interface for domain invariants that represent constraints that must always be satisfied
/// </summary>
public interface IInvariant
{
    /// <summary>
    /// Gets the descriptive message about the invariant
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Gets the unique code for the invariant
    /// </summary>
    string Code { get; }

    /// <summary>
    /// Checks if the invariant is violated
    /// </summary>
    /// <returns>True if the invariant is violated, false otherwise</returns>
    bool IsViolated();
}

/// <summary>
/// Interface for async invariants that may require external validation
/// </summary>
public interface IAsyncInvariant
{
    /// <summary>
    /// Gets the descriptive message about the invariant
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Gets the unique code for the invariant
    /// </summary>
    string Code { get; }

    /// <summary>
    /// Checks if the invariant is violated asynchronously
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the invariant is violated, false otherwise</returns>
    Task<bool> IsViolatedAsync(CancellationToken cancellationToken = default);
} 