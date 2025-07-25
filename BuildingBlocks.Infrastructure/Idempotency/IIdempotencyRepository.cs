namespace BuildingBlocks.Infrastructure.Idempotency;

/// <summary>
/// Repository interface for idempotency operations
/// </summary>
public interface IIdempotencyRepository
{
    /// <summary>
    /// Gets an idempotency record by key
    /// </summary>
    /// <param name="key">The idempotency key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The idempotency record if found</returns>
    Task<IdempotencyEntity?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new idempotency record
    /// </summary>
    /// <param name="entity">The idempotency entity to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task CreateAsync(IdempotencyEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing idempotency record
    /// </summary>
    /// <param name="entity">The idempotency entity to update</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task UpdateAsync(IdempotencyEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an idempotency record by key
    /// </summary>
    /// <param name="key">The idempotency key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes expired idempotency records
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of records deleted</returns>
    Task<int> DeleteExpiredAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an idempotency record exists for the given key
    /// </summary>
    /// <param name="key">The idempotency key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the record exists</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets idempotency records by status
    /// </summary>
    /// <param name="status">The status to filter by</param>
    /// <param name="maxResults">Maximum number of results to return</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A collection of idempotency records</returns>
    Task<IEnumerable<IdempotencyEntity>> GetByStatusAsync(IdempotencyStatus status, int maxResults = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets idempotency records created within a date range
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A collection of idempotency records</returns>
    Task<IEnumerable<IdempotencyEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
} 