using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Application.Behaviors;

/// <summary>
/// Pipeline behavior for validating requests
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    /// <summary>
    /// Handles the request with validation
    /// </summary>
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Validate using data annotations
        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();
        
        if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
        {
            var errors = validationResults
                .Where(x => !string.IsNullOrEmpty(x.ErrorMessage))
                .Select(x => x.ErrorMessage!)
                .ToList();

            throw new ValidationException($"Validation failed for {typeof(TRequest).Name}: {string.Join(", ", errors)}");
        }

        // If request implements IValidatable, call custom validation
        if (request is IValidatable validatable)
        {
            var validationResult = await validatable.ValidateAsync(cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors);
                throw new ValidationException($"Custom validation failed for {typeof(TRequest).Name}: {errors}");
            }
        }

        return await next();
    }
}

/// <summary>
/// Interface for objects that can perform custom validation
/// </summary>
public interface IValidatable
{
    /// <summary>
    /// Validates the object
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The validation result</returns>
    Task<ValidationResult> ValidateAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents the result of a validation operation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets whether the validation was successful
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Gets the validation errors
    /// </summary>
    public IEnumerable<string> Errors { get; init; } = Enumerable.Empty<string>();

    /// <summary>
    /// Creates a successful validation result
    /// </summary>
    public static ValidationResult Success() => new() { IsValid = true };

    /// <summary>
    /// Creates a failed validation result
    /// </summary>
    /// <param name="errors">The validation errors</param>
    public static ValidationResult Failure(params string[] errors) => new() 
    { 
        IsValid = false, 
        Errors = errors ?? Enumerable.Empty<string>() 
    };

    /// <summary>
    /// Creates a failed validation result
    /// </summary>
    /// <param name="errors">The validation errors</param>
    public static ValidationResult Failure(IEnumerable<string> errors) => new() 
    { 
        IsValid = false, 
        Errors = errors ?? Enumerable.Empty<string>() 
    };
} 