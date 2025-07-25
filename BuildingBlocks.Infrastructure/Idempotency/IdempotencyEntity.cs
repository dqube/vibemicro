namespace BuildingBlocks.Infrastructure.Idempotency;

/// <summary>
/// Entity for storing idempotency records in the database
/// </summary>
public class IdempotencyEntity
{
    /// <summary>
    /// Gets or sets the idempotency key
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the serialized request data
    /// </summary>
    public string? RequestData { get; set; }

    /// <summary>
    /// Gets or sets the request hash for quick comparison
    /// </summary>
    public string? RequestHash { get; set; }

    /// <summary>
    /// Gets or sets the serialized response data
    /// </summary>
    public string? ResponseData { get; set; }

    /// <summary>
    /// Gets or sets the response type name
    /// </summary>
    public string? ResponseType { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the expiration timestamp
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the completion timestamp
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the status of the idempotency record
    /// </summary>
    public IdempotencyStatus Status { get; set; } = IdempotencyStatus.InProgress;

    /// <summary>
    /// Gets or sets the error message if the operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the user or service that initiated the operation
    /// </summary>
    public string? InitiatedBy { get; set; }

    /// <summary>
    /// Gets or sets additional metadata
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Checks if this record has expired
    /// </summary>
    /// <returns>True if the record has expired</returns>
    public bool IsExpired()
    {
        return ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if this record is completed (either successful or failed)
    /// </summary>
    /// <returns>True if the record is completed</returns>
    public bool IsCompleted()
    {
        return Status == IdempotencyStatus.Completed || Status == IdempotencyStatus.Failed;
    }

    /// <summary>
    /// Marks the record as completed with response data
    /// </summary>
    /// <param name="responseData">The serialized response data</param>
    /// <param name="responseType">The response type name</param>
    public void MarkCompleted(string? responseData, string? responseType)
    {
        Status = IdempotencyStatus.Completed;
        ResponseData = responseData;
        ResponseType = responseType;
        CompletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the record as failed with error information
    /// </summary>
    /// <param name="errorMessage">The error message</param>
    public void MarkFailed(string errorMessage)
    {
        Status = IdempotencyStatus.Failed;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Status of an idempotency record
/// </summary>
public enum IdempotencyStatus
{
    /// <summary>
    /// Operation is in progress
    /// </summary>
    InProgress = 0,

    /// <summary>
    /// Operation completed successfully
    /// </summary>
    Completed = 1,

    /// <summary>
    /// Operation failed
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Operation was cancelled
    /// </summary>
    Cancelled = 3
} 