using BuildingBlocks.Domain.Guards;

namespace BuildingBlocks.Domain.Validation;

/// <summary>
/// Represents a validation error
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Gets the name of the property that failed validation
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the error message
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// Gets the error code
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the attempted value that caused the validation error
    /// </summary>
    public object? AttemptedValue { get; }

    /// <summary>
    /// Initializes a new instance of the ValidationError class
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation</param>
    /// <param name="errorMessage">The error message</param>
    public ValidationError(string propertyName, string errorMessage)
        : this(propertyName, errorMessage, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ValidationError class
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation</param>
    /// <param name="errorMessage">The error message</param>
    /// <param name="errorCode">The error code</param>
    public ValidationError(string propertyName, string errorMessage, string? errorCode)
        : this(propertyName, errorMessage, errorCode, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ValidationError class
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation</param>
    /// <param name="errorMessage">The error message</param>
    /// <param name="errorCode">The error code</param>
    /// <param name="attemptedValue">The attempted value that caused the validation error</param>
    public ValidationError(string propertyName, string errorMessage, string? errorCode, object? attemptedValue)
    {
        PropertyName = Guard.NotNullOrEmpty(propertyName);
        ErrorMessage = Guard.NotNullOrEmpty(errorMessage);
        ErrorCode = errorCode;
        AttemptedValue = attemptedValue;
    }

    /// <summary>
    /// Returns a string representation of the validation error
    /// </summary>
    /// <returns>A string representation of the validation error</returns>
    public override string ToString()
    {
        var message = $"{PropertyName}: {ErrorMessage}";
        
        if (!string.IsNullOrEmpty(ErrorCode))
        {
            message += $" (Code: {ErrorCode})";
        }
        
        if (AttemptedValue != null)
        {
            message += $" (Attempted Value: {AttemptedValue})";
        }
        
        return message;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current validation error
    /// </summary>
    /// <param name="obj">The object to compare with the current validation error</param>
    /// <returns>True if the specified object is equal to the current validation error; otherwise, false</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not ValidationError other)
            return false;

        return PropertyName == other.PropertyName &&
               ErrorMessage == other.ErrorMessage &&
               ErrorCode == other.ErrorCode;
    }

    /// <summary>
    /// Returns the hash code for this validation error
    /// </summary>
    /// <returns>A hash code for the current validation error</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(PropertyName, ErrorMessage, ErrorCode);
    }
} 