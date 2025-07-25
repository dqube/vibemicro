using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Domain.Services;

/// <summary>
/// Base class for domain services providing common functionality
/// </summary>
public abstract class DomainServiceBase : IDomainService
{
    /// <summary>
    /// Gets the logger instance
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Initializes a new instance of the DomainServiceBase class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    protected DomainServiceBase(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Logs the start of a domain service operation
    /// </summary>
    /// <param name="operationName">The name of the operation</param>
    /// <param name="parameters">Optional parameters to log</param>
    protected void LogOperationStart(string operationName, object? parameters = null)
    {
        if (parameters != null)
        {
            Logger.LogInformation("Starting domain service operation: {OperationName} with parameters: {@Parameters}", 
                operationName, parameters);
        }
        else
        {
            Logger.LogInformation("Starting domain service operation: {OperationName}", operationName);
        }
    }

    /// <summary>
    /// Logs the completion of a domain service operation
    /// </summary>
    /// <param name="operationName">The name of the operation</param>
    /// <param name="result">Optional result to log</param>
    protected void LogOperationComplete(string operationName, object? result = null)
    {
        if (result != null)
        {
            Logger.LogInformation("Completed domain service operation: {OperationName} with result: {@Result}", 
                operationName, result);
        }
        else
        {
            Logger.LogInformation("Completed domain service operation: {OperationName}", operationName);
        }
    }

    /// <summary>
    /// Logs an error in a domain service operation
    /// </summary>
    /// <param name="operationName">The name of the operation</param>
    /// <param name="exception">The exception that occurred</param>
    protected void LogOperationError(string operationName, Exception exception)
    {
        Logger.LogError(exception, "Error in domain service operation: {OperationName}", operationName);
    }
} 