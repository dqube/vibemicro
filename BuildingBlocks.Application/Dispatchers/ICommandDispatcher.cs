using BuildingBlocks.Application.CQRS.Commands;

namespace BuildingBlocks.Application.Dispatchers;

/// <summary>
/// Interface for dispatching commands
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Dispatches a command without return value
    /// </summary>
    /// <typeparam name="TCommand">The command type</typeparam>
    /// <param name="command">The command to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    /// <summary>
    /// Dispatches a command with return value
    /// </summary>
    /// <typeparam name="TCommand">The command type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="command">The command to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The command result</returns>
    Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>;
} 