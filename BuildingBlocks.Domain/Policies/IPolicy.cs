namespace BuildingBlocks.Domain.Policies;

/// <summary>
/// Interface for synchronous policies that apply business logic
/// </summary>
/// <typeparam name="TInput">The input type</typeparam>
/// <typeparam name="TOutput">The output type</typeparam>
public interface IPolicy<in TInput, out TOutput>
{
    /// <summary>
    /// Applies the policy to the input
    /// </summary>
    /// <param name="input">The input to process</param>
    /// <returns>The processed output</returns>
    TOutput Apply(TInput input);
}

/// <summary>
/// Interface for asynchronous policies that apply business logic
/// </summary>
/// <typeparam name="TInput">The input type</typeparam>
/// <typeparam name="TOutput">The output type</typeparam>
public interface IAsyncPolicy<in TInput, TOutput>
{
    /// <summary>
    /// Applies the policy to the input asynchronously
    /// </summary>
    /// <param name="input">The input to process</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The processed output</returns>
    Task<TOutput> ApplyAsync(TInput input, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for validation policies
/// </summary>
/// <typeparam name="TInput">The input type to validate</typeparam>
public interface IValidationPolicy<in TInput> : IPolicy<TInput, bool>
{
    /// <summary>
    /// Gets the validation error message when the policy fails
    /// </summary>
    string ErrorMessage { get; }

    /// <summary>
    /// Gets the validation error code when the policy fails
    /// </summary>
    string ErrorCode { get; }
}

/// <summary>
/// Interface for transformation policies
/// </summary>
/// <typeparam name="TInput">The input type</typeparam>
/// <typeparam name="TOutput">The output type</typeparam>
public interface ITransformationPolicy<in TInput, out TOutput> : IPolicy<TInput, TOutput>
{
    /// <summary>
    /// Gets the policy name for logging and debugging
    /// </summary>
    string PolicyName { get; }
}

/// <summary>
/// Interface for calculation policies
/// </summary>
/// <typeparam name="TInput">The input type</typeparam>
/// <typeparam name="TResult">The calculation result type</typeparam>
public interface ICalculationPolicy<in TInput, out TResult> : IPolicy<TInput, TResult>
{
    /// <summary>
    /// Gets the calculation method used by this policy
    /// </summary>
    string CalculationMethod { get; }
} 