using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Sagas;

/// <summary>
/// Base class for sagas providing common functionality
/// </summary>
public abstract class SagaBase : ISaga
{
    /// <summary>
    /// Gets the unique identifier of the saga
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the correlation identifier for tracking related events
    /// </summary>
    public string CorrelationId { get; private set; }

    /// <summary>
    /// Gets the current state of the saga
    /// </summary>
    public SagaState State { get; private set; }

    /// <summary>
    /// Gets the saga metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; private set; }

    /// <summary>
    /// Gets the date and time when the saga was created
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the saga was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the error message if the saga failed
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// Gets the logger instance
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Initializes a new instance of the SagaBase class
    /// </summary>
    /// <param name="correlationId">The correlation identifier</param>
    /// <param name="logger">The logger instance</param>
    protected SagaBase(string correlationId, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(correlationId))
            throw new ArgumentException("Correlation ID cannot be null or empty", nameof(correlationId));

        Id = Guid.NewGuid();
        CorrelationId = correlationId;
        State = SagaState.Running;
        Metadata = new Dictionary<string, object>();
        CreatedAt = DateTime.UtcNow;
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Checks if the saga can handle the specified event
    /// </summary>
    /// <param name="eventItem">The event to check</param>
    /// <returns>True if the saga can handle the event</returns>
    public abstract bool CanHandle(IEvent eventItem);

    /// <summary>
    /// Handles an event and updates the saga state
    /// </summary>
    /// <param name="eventItem">The event to handle</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A collection of commands or events to publish</returns>
    public async Task<IEnumerable<object>> HandleAsync(IEvent eventItem, CancellationToken cancellationToken = default)
    {
        if (eventItem == null)
            throw new ArgumentNullException(nameof(eventItem));

        if (State != SagaState.Running && State != SagaState.Waiting)
        {
            Logger.LogWarning("Saga {SagaId} cannot handle event {EventType} in state {State}",
                Id, eventItem.GetType().Name, State);
            return Enumerable.Empty<object>();
        }

        if (!CanHandle(eventItem))
        {
            Logger.LogDebug("Saga {SagaId} cannot handle event {EventType}",
                Id, eventItem.GetType().Name);
            return Enumerable.Empty<object>();
        }

        Logger.LogDebug("Saga {SagaId} handling event {EventType}: {@Event}",
            Id, eventItem.GetType().Name, eventItem);

        try
        {
            var result = await HandleEventAsync(eventItem, cancellationToken);
            UpdatedAt = DateTime.UtcNow;

            Logger.LogDebug("Saga {SagaId} successfully handled event {EventType}, State: {State}",
                Id, eventItem.GetType().Name, State);

            return result ?? Enumerable.Empty<object>();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling event {EventType} in saga {SagaId}: {Error}",
                eventItem.GetType().Name, Id, ex.Message);

            Fail(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Core event handling implementation to be provided by derived classes
    /// </summary>
    /// <param name="eventItem">The event to handle</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A collection of commands or events to publish</returns>
    protected abstract Task<IEnumerable<object>?> HandleEventAsync(IEvent eventItem, CancellationToken cancellationToken);

    /// <summary>
    /// Marks the saga as completed
    /// </summary>
    public virtual void Complete()
    {
        if (State == SagaState.Running || State == SagaState.Waiting)
        {
            State = SagaState.Completed;
            UpdatedAt = DateTime.UtcNow;
            Logger.LogInformation("Saga {SagaId} completed successfully", Id);
        }
    }

    /// <summary>
    /// Marks the saga as failed
    /// </summary>
    /// <param name="error">The error that caused the failure</param>
    public virtual void Fail(string error)
    {
        if (State == SagaState.Running || State == SagaState.Waiting)
        {
            State = SagaState.Failed;
            Error = error;
            UpdatedAt = DateTime.UtcNow;
            Logger.LogError("Saga {SagaId} failed: {Error}", Id, error);
        }
    }

    /// <summary>
    /// Marks the saga as cancelled
    /// </summary>
    public virtual void Cancel()
    {
        if (State == SagaState.Running || State == SagaState.Waiting)
        {
            State = SagaState.Cancelled;
            UpdatedAt = DateTime.UtcNow;
            Logger.LogInformation("Saga {SagaId} cancelled", Id);
        }
    }

    /// <summary>
    /// Marks the saga as waiting for external input
    /// </summary>
    protected void SetWaiting()
    {
        if (State == SagaState.Running)
        {
            State = SagaState.Waiting;
            UpdatedAt = DateTime.UtcNow;
            Logger.LogDebug("Saga {SagaId} is now waiting", Id);
        }
    }

    /// <summary>
    /// Resumes the saga from waiting state
    /// </summary>
    protected void Resume()
    {
        if (State == SagaState.Waiting)
        {
            State = SagaState.Running;
            UpdatedAt = DateTime.UtcNow;
            Logger.LogDebug("Saga {SagaId} resumed", Id);
        }
    }

    /// <summary>
    /// Adds or updates metadata
    /// </summary>
    /// <param name="key">The metadata key</param>
    /// <param name="value">The metadata value</param>
    protected void SetMetadata(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        Metadata[key] = value;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets metadata value
    /// </summary>
    /// <typeparam name="T">The metadata value type</typeparam>
    /// <param name="key">The metadata key</param>
    /// <returns>The metadata value or default if not found</returns>
    protected T? GetMetadata<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return default;

        return Metadata.TryGetValue(key, out var value) && value is T typedValue
            ? typedValue
            : default;
    }
} 