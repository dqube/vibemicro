using BuildingBlocks.Domain.Guards;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Domain.Validation;

/// <summary>
/// Base class for domain validators providing common functionality
/// </summary>
/// <typeparam name="T">The type to validate</typeparam>
public abstract class DomainValidatorBase<T> : IDomainValidator<T>
{
    /// <summary>
    /// Gets the logger instance
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Initializes a new instance of the DomainValidatorBase class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    protected DomainValidatorBase(ILogger logger)
    {
        Logger = Guard.NotNull(logger);
    }

    /// <summary>
    /// Validates the specified object
    /// </summary>
    /// <param name="obj">The object to validate</param>
    /// <returns>The validation result</returns>
    public ValidationResult Validate(T obj)
    {
        Guard.NotNull(obj);

        LogValidationStart(obj);

        try
        {
            var result = ValidateCore(obj);
            LogValidationComplete(obj, result);
            return result;
        }
        catch (Exception ex)
        {
            LogValidationError(obj, ex);
            throw;
        }
    }

    /// <summary>
    /// Validates the specified object asynchronously
    /// </summary>
    /// <param name="obj">The object to validate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The validation result</returns>
    public virtual async Task<ValidationResult> ValidateAsync(T obj, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(obj);

        LogValidationStart(obj);

        try
        {
            var result = await ValidateAsyncCore(obj, cancellationToken);
            LogValidationComplete(obj, result);
            return result;
        }
        catch (Exception ex)
        {
            LogValidationError(obj, ex);
            throw;
        }
    }

    /// <summary>
    /// Core validation implementation to be provided by derived classes
    /// </summary>
    /// <param name="obj">The object to validate</param>
    /// <returns>The validation result</returns>
    protected abstract ValidationResult ValidateCore(T obj);

    /// <summary>
    /// Core async validation implementation. Override in derived classes for async validation
    /// </summary>
    /// <param name="obj">The object to validate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The validation result</returns>
    protected virtual async Task<ValidationResult> ValidateAsyncCore(T obj, CancellationToken cancellationToken)
    {
        return await Task.FromResult(ValidateCore(obj));
    }

    /// <summary>
    /// Validates a property value and adds errors to the result if validation fails
    /// </summary>
    /// <param name="result">The validation result to add errors to</param>
    /// <param name="propertyName">The name of the property being validated</param>
    /// <param name="value">The value to validate</param>
    /// <param name="predicate">The validation predicate</param>
    /// <param name="errorMessage">The error message if validation fails</param>
    /// <param name="errorCode">The optional error code</param>
    protected void ValidateProperty<TValue>(
        ValidationResult result,
        string propertyName,
        TValue value,
        Func<TValue, bool> predicate,
        string errorMessage,
        string? errorCode = null)
    {
        Guard.NotNull(result);
        Guard.NotNullOrEmpty(propertyName);
        Guard.NotNull(predicate);
        Guard.NotNullOrEmpty(errorMessage);

        if (!predicate(value))
        {
            result.AddError(propertyName, errorMessage, errorCode);
        }
    }

    /// <summary>
    /// Validates a property value asynchronously and adds errors to the result if validation fails
    /// </summary>
    /// <param name="result">The validation result to add errors to</param>
    /// <param name="propertyName">The name of the property being validated</param>
    /// <param name="value">The value to validate</param>
    /// <param name="predicate">The async validation predicate</param>
    /// <param name="errorMessage">The error message if validation fails</param>
    /// <param name="errorCode">The optional error code</param>
    /// <param name="cancellationToken">The cancellation token</param>
    protected async Task ValidatePropertyAsync<TValue>(
        ValidationResult result,
        string propertyName,
        TValue value,
        Func<TValue, Task<bool>> predicate,
        string errorMessage,
        string? errorCode = null,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(result);
        Guard.NotNullOrEmpty(propertyName);
        Guard.NotNull(predicate);
        Guard.NotNullOrEmpty(errorMessage);

        if (!await predicate(value))
        {
            result.AddError(propertyName, errorMessage, errorCode);
        }
    }

    /// <summary>
    /// Validates a required property (not null or empty for strings)
    /// </summary>
    /// <param name="result">The validation result to add errors to</param>
    /// <param name="propertyName">The name of the property being validated</param>
    /// <param name="value">The value to validate</param>
    /// <param name="errorCode">The optional error code</param>
    protected void ValidateRequired(ValidationResult result, string propertyName, object? value, string? errorCode = null)
    {
        ValidateProperty(
            result,
            propertyName,
            value,
            v => v != null && (v is not string s || !string.IsNullOrWhiteSpace(s)),
            $"{propertyName} is required",
            errorCode);
    }

    /// <summary>
    /// Validates string length constraints
    /// </summary>
    /// <param name="result">The validation result to add errors to</param>
    /// <param name="propertyName">The name of the property being validated</param>
    /// <param name="value">The string value to validate</param>
    /// <param name="minLength">The minimum length (optional)</param>
    /// <param name="maxLength">The maximum length (optional)</param>
    /// <param name="errorCode">The optional error code</param>
    protected void ValidateStringLength(
        ValidationResult result,
        string propertyName,
        string? value,
        int? minLength = null,
        int? maxLength = null,
        string? errorCode = null)
    {
        if (value == null) return;

        if (minLength.HasValue)
        {
            ValidateProperty(
                result,
                propertyName,
                value,
                v => v.Length >= minLength.Value,
                $"{propertyName} must be at least {minLength} characters long",
                errorCode);
        }

        if (maxLength.HasValue)
        {
            ValidateProperty(
                result,
                propertyName,
                value,
                v => v.Length <= maxLength.Value,
                $"{propertyName} cannot exceed {maxLength} characters",
                errorCode);
        }
    }

    /// <summary>
    /// Logs the start of validation
    /// </summary>
    /// <param name="obj">The object being validated</param>
    protected virtual void LogValidationStart(T obj)
    {
        Logger.LogDebug("Starting validation for {ObjectType}: {@Object}", typeof(T).Name, obj);
    }

    /// <summary>
    /// Logs the completion of validation
    /// </summary>
    /// <param name="obj">The object that was validated</param>
    /// <param name="result">The validation result</param>
    protected virtual void LogValidationComplete(T obj, ValidationResult result)
    {
        if (result.IsValid)
        {
            Logger.LogDebug("Validation completed successfully for {ObjectType}", typeof(T).Name);
        }
        else
        {
            Logger.LogDebug("Validation failed for {ObjectType} with {ErrorCount} error(s): {Errors}",
                typeof(T).Name, result.Errors.Count, string.Join(", ", result.GetErrorMessages()));
        }
    }

    /// <summary>
    /// Logs a validation error
    /// </summary>
    /// <param name="obj">The object being validated</param>
    /// <param name="exception">The exception that occurred</param>
    protected virtual void LogValidationError(T obj, Exception exception)
    {
        Logger.LogError(exception, "Error during validation of {ObjectType}", typeof(T).Name);
    }
} 