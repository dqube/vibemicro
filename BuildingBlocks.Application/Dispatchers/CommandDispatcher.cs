using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Mediator;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Dispatchers;

/// <summary>
/// Default implementation of command dispatcher
/// </summary>
public class CommandDispatcher : ICommandDispatcher
{
    private readonly IMediator _mediator;
    private readonly ILogger<CommandDispatcher> _logger;

    /// <summary>
    /// Initializes a new instance of the CommandDispatcher class
    /// </summary>
    public CommandDispatcher(IMediator mediator, ILogger<CommandDispatcher> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Dispatches a command without return value
    /// </summary>
    /// <typeparam name="TCommand">The command type</typeparam>
    /// <param name="command">The command to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        _logger.LogDebug("Dispatching command {CommandType}: {@Command}", typeof(TCommand).Name, command);

        try
        {
            await _mediator.SendAsync(command, cancellationToken);
            _logger.LogDebug("Successfully dispatched command {CommandType}", typeof(TCommand).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch command {CommandType}: {Error}", typeof(TCommand).Name, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Dispatches a command with return value
    /// </summary>
    /// <typeparam name="TCommand">The command type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="command">The command to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The command result</returns>
    public async Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        _logger.LogDebug("Dispatching command {CommandType}: {@Command}", typeof(TCommand).Name, command);

        try
        {
            var result = await _mediator.SendAsync<TCommand, TResult>(command, cancellationToken);
            _logger.LogDebug("Successfully dispatched command {CommandType} with result: {@Result}", typeof(TCommand).Name, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch command {CommandType}: {Error}", typeof(TCommand).Name, ex.Message);
            throw;
        }
    }
} 