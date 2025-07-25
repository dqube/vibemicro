using BuildingBlocks.Domain.Guards;

namespace BuildingBlocks.Domain.Validation;

/// <summary>
/// Represents the result of a validation operation
/// </summary>
public class ValidationResult
{
    private readonly List<ValidationError> _errors;

    /// <summary>
    /// Gets a value indicating whether the validation was successful
    /// </summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>
    /// Gets the validation errors
    /// </summary>
    public IReadOnlyList<ValidationError> Errors => _errors.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the ValidationResult class
    /// </summary>
    public ValidationResult()
    {
        _errors = new List<ValidationError>();
    }

    /// <summary>
    /// Initializes a new instance of the ValidationResult class with errors
    /// </summary>
    /// <param name="errors">The validation errors</param>
    public ValidationResult(IEnumerable<ValidationError> errors)
    {
        _errors = Guard.NotNull(errors).ToList();
    }

    /// <summary>
    /// Adds a validation error
    /// </summary>
    /// <param name="error">The validation error to add</param>
    public void AddError(ValidationError error)
    {
        Guard.NotNull(error);
        _errors.Add(error);
    }

    /// <summary>
    /// Adds a validation error with property name and message
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation</param>
    /// <param name="errorMessage">The error message</param>
    public void AddError(string propertyName, string errorMessage)
    {
        AddError(new ValidationError(propertyName, errorMessage));
    }

    /// <summary>
    /// Adds a validation error with property name, message, and error code
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation</param>
    /// <param name="errorMessage">The error message</param>
    /// <param name="errorCode">The error code</param>
    public void AddError(string propertyName, string errorMessage, string errorCode)
    {
        AddError(new ValidationError(propertyName, errorMessage, errorCode));
    }

    /// <summary>
    /// Adds multiple validation errors
    /// </summary>
    /// <param name="errors">The validation errors to add</param>
    public void AddErrors(IEnumerable<ValidationError> errors)
    {
        Guard.NotNull(errors);
        _errors.AddRange(errors);
    }

    /// <summary>
    /// Merges another validation result into this one
    /// </summary>
    /// <param name="other">The other validation result to merge</param>
    public void Merge(ValidationResult other)
    {
        Guard.NotNull(other);
        _errors.AddRange(other._errors);
    }

    /// <summary>
    /// Gets errors for a specific property
    /// </summary>
    /// <param name="propertyName">The property name</param>
    /// <returns>The errors for the specified property</returns>
    public IEnumerable<ValidationError> GetErrorsForProperty(string propertyName)
    {
        Guard.NotNullOrEmpty(propertyName);
        return _errors.Where(e => string.Equals(e.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets all error messages
    /// </summary>
    /// <returns>All error messages</returns>
    public IEnumerable<string> GetErrorMessages()
    {
        return _errors.Select(e => e.ErrorMessage);
    }

    /// <summary>
    /// Gets error messages for a specific property
    /// </summary>
    /// <param name="propertyName">The property name</param>
    /// <returns>Error messages for the specified property</returns>
    public IEnumerable<string> GetErrorMessagesForProperty(string propertyName)
    {
        return GetErrorsForProperty(propertyName).Select(e => e.ErrorMessage);
    }

    /// <summary>
    /// Converts the validation result to a string representation
    /// </summary>
    /// <returns>A string representation of the validation result</returns>
    public override string ToString()
    {
        if (IsValid)
        {
            return "Validation successful";
        }

        var errorMessages = string.Join(Environment.NewLine, GetErrorMessages());
        return $"Validation failed with {_errors.Count} error(s):{Environment.NewLine}{errorMessages}";
    }

    /// <summary>
    /// Creates a successful validation result
    /// </summary>
    /// <returns>A successful validation result</returns>
    public static ValidationResult Success()
    {
        return new ValidationResult();
    }

    /// <summary>
    /// Creates a failed validation result with a single error
    /// </summary>
    /// <param name="propertyName">The property name</param>
    /// <param name="errorMessage">The error message</param>
    /// <returns>A failed validation result</returns>
    public static ValidationResult Failure(string propertyName, string errorMessage)
    {
        var result = new ValidationResult();
        result.AddError(propertyName, errorMessage);
        return result;
    }

    /// <summary>
    /// Creates a failed validation result with multiple errors
    /// </summary>
    /// <param name="errors">The validation errors</param>
    /// <returns>A failed validation result</returns>
    public static ValidationResult Failure(IEnumerable<ValidationError> errors)
    {
        return new ValidationResult(errors);
    }
} 