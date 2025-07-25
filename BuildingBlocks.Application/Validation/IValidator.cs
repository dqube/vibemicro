namespace BuildingBlocks.Application.Validation;

/// <summary>
/// Interface for validators
/// </summary>
/// <typeparam name="T">The type to validate</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates the specified object
    /// </summary>
    /// <param name="instance">The object to validate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The validation result</returns>
    Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
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
    public IEnumerable<ValidationError> Errors { get; init; } = Enumerable.Empty<ValidationError>();

    /// <summary>
    /// Creates a successful validation result
    /// </summary>
    public static ValidationResult Success() => new() { IsValid = true };

    /// <summary>
    /// Creates a failed validation result
    /// </summary>
    /// <param name="errors">The validation errors</param>
    public static ValidationResult Failure(params ValidationError[] errors) => new() 
    { 
        IsValid = false, 
        Errors = errors ?? Enumerable.Empty<ValidationError>() 
    };

    /// <summary>
    /// Creates a failed validation result
    /// </summary>
    /// <param name="errors">The validation errors</param>
    public static ValidationResult Failure(IEnumerable<ValidationError> errors) => new() 
    { 
        IsValid = false, 
        Errors = errors ?? Enumerable.Empty<ValidationError>() 
    };
}

/// <summary>
/// Represents a validation error
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Gets the property name that failed validation
    /// </summary>
    public string PropertyName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the error message
    /// </summary>
    public string ErrorMessage { get; init; } = string.Empty;

    /// <summary>
    /// Gets the attempted value
    /// </summary>
    public object? AttemptedValue { get; init; }

    /// <summary>
    /// Gets the error code
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Initializes a new instance of the ValidationError class
    /// </summary>
    /// <param name="propertyName">The property name</param>
    /// <param name="errorMessage">The error message</param>
    /// <param name="attemptedValue">The attempted value</param>
    /// <param name="errorCode">The error code</param>
    public ValidationError(string propertyName, string errorMessage, object? attemptedValue = null, string? errorCode = null)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
        AttemptedValue = attemptedValue;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Returns the string representation of the validation error
    /// </summary>
    public override string ToString()
    {
        return $"{PropertyName}: {ErrorMessage}";
    }
} 