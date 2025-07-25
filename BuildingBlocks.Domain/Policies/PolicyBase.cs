using BuildingBlocks.Domain.Guards;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Domain.Policies;

/// <summary>
/// Base class for synchronous policies providing common functionality
/// </summary>
/// <typeparam name="TInput">The input type</typeparam>
/// <typeparam name="TOutput">The output type</typeparam>
public abstract class PolicyBase<TInput, TOutput> : IPolicy<TInput, TOutput>
{
    /// <summary>
    /// Gets the logger instance
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the policy name
    /// </summary>
    public virtual string PolicyName => GetType().Name;

    /// <summary>
    /// Initializes a new instance of the PolicyBase class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    protected PolicyBase(ILogger logger)
    {
        Logger = Guard.NotNull(logger);
    }

    /// <summary>
    /// Applies the policy to the input
    /// </summary>
    /// <param name="input">The input to process</param>
    /// <returns>The processed output</returns>
    public TOutput Apply(TInput input)
    {
        LogPolicyStart(input);
        
        try
        {
            ValidateInput(input);
            var result = ApplyCore(input);
            LogPolicyComplete(input, result);
            return result;
        }
        catch (Exception ex)
        {
            LogPolicyError(input, ex);
            throw;
        }
    }

    /// <summary>
    /// Core policy implementation to be provided by derived classes
    /// </summary>
    /// <param name="input">The input to process</param>
    /// <returns>The processed output</returns>
    protected abstract TOutput ApplyCore(TInput input);

    /// <summary>
    /// Validates the input before applying the policy
    /// Override in derived classes for custom validation
    /// </summary>
    /// <param name="input">The input to validate</param>
    protected virtual void ValidateInput(TInput input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));
    }

    /// <summary>
    /// Logs the start of policy application
    /// </summary>
    /// <param name="input">The input being processed</param>
    protected virtual void LogPolicyStart(TInput input)
    {
        Logger.LogInformation("Starting policy {PolicyName} with input: {@Input}", PolicyName, input);
    }

    /// <summary>
    /// Logs the completion of policy application
    /// </summary>
    /// <param name="input">The input that was processed</param>
    /// <param name="output">The output that was produced</param>
    protected virtual void LogPolicyComplete(TInput input, TOutput output)
    {
        Logger.LogInformation("Completed policy {PolicyName} with output: {@Output}", PolicyName, output);
    }

    /// <summary>
    /// Logs an error during policy application
    /// </summary>
    /// <param name="input">The input that was being processed</param>
    /// <param name="exception">The exception that occurred</param>
    protected virtual void LogPolicyError(TInput input, Exception exception)
    {
        Logger.LogError(exception, "Error in policy {PolicyName} with input: {@Input}", PolicyName, input);
    }
}

/// <summary>
/// Base class for asynchronous policies providing common functionality
/// </summary>
/// <typeparam name="TInput">The input type</typeparam>
/// <typeparam name="TOutput">The output type</typeparam>
public abstract class AsyncPolicyBase<TInput, TOutput> : IAsyncPolicy<TInput, TOutput>
{
    /// <summary>
    /// Gets the logger instance
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the policy name
    /// </summary>
    public virtual string PolicyName => GetType().Name;

    /// <summary>
    /// Initializes a new instance of the AsyncPolicyBase class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    protected AsyncPolicyBase(ILogger logger)
    {
        Logger = Guard.NotNull(logger);
    }

    /// <summary>
    /// Applies the policy to the input asynchronously
    /// </summary>
    /// <param name="input">The input to process</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The processed output</returns>
    public async Task<TOutput> ApplyAsync(TInput input, CancellationToken cancellationToken = default)
    {
        LogPolicyStart(input);
        
        try
        {
            await ValidateInputAsync(input, cancellationToken);
            var result = await ApplyAsyncCore(input, cancellationToken);
            LogPolicyComplete(input, result);
            return result;
        }
        catch (Exception ex)
        {
            LogPolicyError(input, ex);
            throw;
        }
    }

    /// <summary>
    /// Core async policy implementation to be provided by derived classes
    /// </summary>
    /// <param name="input">The input to process</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The processed output</returns>
    protected abstract Task<TOutput> ApplyAsyncCore(TInput input, CancellationToken cancellationToken);

    /// <summary>
    /// Validates the input before applying the policy asynchronously
    /// Override in derived classes for custom validation
    /// </summary>
    /// <param name="input">The input to validate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    protected virtual Task ValidateInputAsync(TInput input, CancellationToken cancellationToken)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Logs the start of policy application
    /// </summary>
    /// <param name="input">The input being processed</param>
    protected virtual void LogPolicyStart(TInput input)
    {
        Logger.LogInformation("Starting async policy {PolicyName} with input: {@Input}", PolicyName, input);
    }

    /// <summary>
    /// Logs the completion of policy application
    /// </summary>
    /// <param name="input">The input that was processed</param>
    /// <param name="output">The output that was produced</param>
    protected virtual void LogPolicyComplete(TInput input, TOutput output)
    {
        Logger.LogInformation("Completed async policy {PolicyName} with output: {@Output}", PolicyName, output);
    }

    /// <summary>
    /// Logs an error during policy application
    /// </summary>
    /// <param name="input">The input that was being processed</param>
    /// <param name="exception">The exception that occurred</param>
    protected virtual void LogPolicyError(TInput input, Exception exception)
    {
        Logger.LogError(exception, "Error in async policy {PolicyName} with input: {@Input}", PolicyName, input);
    }
}

/// <summary>
/// Base class for validation policies
/// </summary>
/// <typeparam name="TInput">The input type to validate</typeparam>
public abstract class ValidationPolicyBase<TInput> : PolicyBase<TInput, bool>, IValidationPolicy<TInput>
{
    /// <summary>
    /// Gets the validation error message when the policy fails
    /// </summary>
    public abstract string ErrorMessage { get; }

    /// <summary>
    /// Gets the validation error code when the policy fails
    /// </summary>
    public abstract string ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the ValidationPolicyBase class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    protected ValidationPolicyBase(ILogger logger) : base(logger)
    {
    }
} 