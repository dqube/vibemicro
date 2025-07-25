using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Services;

/// <summary>
/// Base class for application services
/// </summary>
public abstract class ApplicationServiceBase : IApplicationService
{
    /// <summary>
    /// Gets the logger
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Initializes a new instance of the ApplicationServiceBase class
    /// </summary>
    /// <param name="logger">The logger</param>
    protected ApplicationServiceBase(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Logs the start of an operation
    /// </summary>
    /// <param name="operationName">The operation name</param>
    /// <param name="parameters">Optional parameters</param>
    protected virtual void LogOperationStart(string operationName, object? parameters = null)
    {
        if (parameters != null)
        {
            Logger.LogInformation("Starting operation {OperationName} with parameters: {@Parameters}", 
                operationName, parameters);
        }
        else
        {
            Logger.LogInformation("Starting operation {OperationName}", operationName);
        }
    }

    /// <summary>
    /// Logs the completion of an operation
    /// </summary>
    /// <param name="operationName">The operation name</param>
    /// <param name="result">Optional result</param>
    protected virtual void LogOperationComplete(string operationName, object? result = null)
    {
        if (result != null)
        {
            Logger.LogInformation("Completed operation {OperationName} with result: {@Result}", 
                operationName, result);
        }
        else
        {
            Logger.LogInformation("Completed operation {OperationName}", operationName);
        }
    }

    /// <summary>
    /// Logs an operation error
    /// </summary>
    /// <param name="operationName">The operation name</param>
    /// <param name="exception">The exception</param>
    protected virtual void LogOperationError(string operationName, Exception exception)
    {
        Logger.LogError(exception, "Error in operation {OperationName}", operationName);
    }
}

/// <summary>
/// Base class for application services with context
/// </summary>
/// <typeparam name="TContext">The context type</typeparam>
public abstract class ApplicationServiceBase<TContext> : ApplicationServiceBase, IApplicationService<TContext>
    where TContext : class
{
    /// <summary>
    /// Gets the service context
    /// </summary>
    protected TContext? Context { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ApplicationServiceBase class
    /// </summary>
    /// <param name="logger">The logger</param>
    protected ApplicationServiceBase(ILogger logger) : base(logger)
    {
    }

    /// <summary>
    /// Sets the service context
    /// </summary>
    /// <param name="context">The service context</param>
    public virtual void SetContext(TContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Ensures that the context is set
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when context is not set</exception>
    protected virtual void EnsureContext()
    {
        if (Context == null)
        {
            throw new InvalidOperationException($"Context of type {typeof(TContext).Name} is not set");
        }
    }
} 