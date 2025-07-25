using BuildingBlocks.Application.CQRS.Events;

namespace BuildingBlocks.Application.Sagas;

/// <summary>
/// Interface for saga that manages long-running business processes
/// </summary>
public interface ISaga
{
    /// <summary>
    /// Gets the unique identifier of the saga
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the correlation identifier for tracking related events
    /// </summary>
    string CorrelationId { get; }

    /// <summary>
    /// Gets the current state of the saga
    /// </summary>
    SagaState State { get; }

    /// <summary>
    /// Gets the saga metadata
    /// </summary>
    Dictionary<string, object> Metadata { get; }

    /// <summary>
    /// Gets the date and time when the saga was created
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the date and time when the saga was last updated
    /// </summary>
    DateTime? UpdatedAt { get; }

    /// <summary>
    /// Checks if the saga can handle the specified event
    /// </summary>
    /// <param name="eventItem">The event to check</param>
    /// <returns>True if the saga can handle the event</returns>
    bool CanHandle(IEvent eventItem);

    /// <summary>
    /// Handles an event and updates the saga state
    /// </summary>
    /// <param name="eventItem">The event to handle</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A collection of commands or events to publish</returns>
    Task<IEnumerable<object>> HandleAsync(IEvent eventItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the saga as completed
    /// </summary>
    void Complete();

    /// <summary>
    /// Marks the saga as failed
    /// </summary>
    /// <param name="error">The error that caused the failure</param>
    void Fail(string error);

    /// <summary>
    /// Marks the saga as cancelled
    /// </summary>
    void Cancel();
}

/// <summary>
/// Represents the state of a saga
/// </summary>
public enum SagaState
{
    /// <summary>
    /// Saga is running
    /// </summary>
    Running = 0,

    /// <summary>
    /// Saga has completed successfully
    /// </summary>
    Completed = 1,

    /// <summary>
    /// Saga has failed
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Saga has been cancelled
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Saga is waiting for external input
    /// </summary>
    Waiting = 4
} 