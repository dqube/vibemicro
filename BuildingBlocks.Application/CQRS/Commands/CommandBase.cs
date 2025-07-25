namespace BuildingBlocks.Application.CQRS.Commands;

/// <summary>
/// Base class for commands that don't return a result
/// </summary>
public abstract class CommandBase : ICommand
{
    /// <summary>
    /// Gets the unique identifier for this command
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets when the command was created
    /// </summary>
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the correlation identifier for tracking related operations
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets the user identifier who initiated the command
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Returns the string representation of the command
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name} [Id: {Id}, CreatedAt: {CreatedAt:yyyy-MM-dd HH:mm:ss}]";
    }
}

/// <summary>
/// Base class for commands that return a result
/// </summary>
/// <typeparam name="TResult">The type of result returned by the command</typeparam>
public abstract class CommandBase<TResult> : ICommand<TResult>
{
    /// <summary>
    /// Gets the unique identifier for this command
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets when the command was created
    /// </summary>
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the correlation identifier for tracking related operations
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets the user identifier who initiated the command
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Returns the string representation of the command
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name} [Id: {Id}, CreatedAt: {CreatedAt:yyyy-MM-dd HH:mm:ss}]";
    }
} 