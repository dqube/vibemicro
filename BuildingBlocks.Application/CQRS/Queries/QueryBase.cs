namespace BuildingBlocks.Application.CQRS.Queries;

/// <summary>
/// Base class for queries
/// </summary>
/// <typeparam name="TResult">The type of result returned by the query</typeparam>
public abstract class QueryBase<TResult> : IQuery<TResult>
{
    /// <summary>
    /// Gets the unique identifier for this query
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets when the query was created
    /// </summary>
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the correlation identifier for tracking related operations
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets the user identifier who initiated the query
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Returns the string representation of the query
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name} [Id: {Id}, CreatedAt: {CreatedAt:yyyy-MM-dd HH:mm:ss}]";
    }
} 