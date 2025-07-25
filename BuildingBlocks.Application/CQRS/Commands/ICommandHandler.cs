namespace BuildingBlocks.Application.CQRS.Commands;

/// <summary>
/// Interface for handling commands that don't return a result
/// </summary>
/// <typeparam name="TCommand">The type of command to handle</typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Handles the specified command
    /// </summary>
    /// <param name="command">The command to handle</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for handling commands that return a result
/// </summary>
/// <typeparam name="TCommand">The type of command to handle</typeparam>
/// <typeparam name="TResult">The type of result returned by the command</typeparam>
public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Handles the specified command
    /// </summary>
    /// <param name="command">The command to handle</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation with the result</returns>
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
} 