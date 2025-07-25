namespace BuildingBlocks.Application.Idempotency;

/// <summary>
/// Interface for idempotency service that ensures operations are performed only once
/// </summary>
public interface IIdempotencyService
{
    /// <summary>
    /// Checks if an operation with the given key has already been executed
    /// </summary>
    /// <param name="key">The idempotency key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the operation has already been executed</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the result of a previously executed operation
    /// </summary>
    /// <typeparam name="T">The result type</typeparam>
    /// <param name="key">The idempotency key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The cached result if found, default otherwise</returns>
    Task<T?> GetResultAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores the result of an operation for future idempotency checks
    /// </summary>
    /// <typeparam name="T">The result type</typeparam>
    /// <param name="key">The idempotency key</param>
    /// <param name="result">The operation result</param>
    /// <param name="expiry">Optional expiry time for the cached result</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task StoreResultAsync<T>(string key, T result, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation only if it hasn't been executed before (idempotent execution)
    /// </summary>
    /// <typeparam name="T">The result type</typeparam>
    /// <param name="key">The idempotency key</param>
    /// <param name="operation">The operation to execute</param>
    /// <param name="expiry">Optional expiry time for the cached result</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The operation result (either from cache or new execution)</returns>
    Task<T> ExecuteAsync<T>(string key, Func<Task<T>> operation, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation only if it hasn't been executed before (idempotent execution without return value)
    /// </summary>
    /// <param name="key">The idempotency key</param>
    /// <param name="operation">The operation to execute</param>
    /// <param name="expiry">Optional expiry time for the cached result</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task ExecuteAsync(string key, Func<Task> operation, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an idempotency record
    /// </summary>
    /// <param name="key">The idempotency key</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up expired idempotency records
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Number of records cleaned up</returns>
    Task<int> CleanupExpiredAsync(CancellationToken cancellationToken = default);
} 