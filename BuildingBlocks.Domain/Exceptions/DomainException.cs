namespace BuildingBlocks.Domain.Exceptions;

/// <summary>
/// Base class for all domain exceptions
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Gets the error code associated with this exception
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Gets additional details about the exception
    /// </summary>
    public object? Details { get; }

    /// <summary>
    /// Initializes a new instance of the DomainException class
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="errorCode">The error code</param>
    protected DomainException(string message, string errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the DomainException class
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="errorCode">The error code</param>
    /// <param name="details">Additional details about the exception</param>
    protected DomainException(string message, string errorCode, object? details)
        : base(message)
    {
        ErrorCode = errorCode;
        Details = details;
    }

    /// <summary>
    /// Initializes a new instance of the DomainException class
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="errorCode">The error code</param>
    /// <param name="innerException">The inner exception</param>
    protected DomainException(string message, string errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the DomainException class
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="errorCode">The error code</param>
    /// <param name="details">Additional details about the exception</param>
    /// <param name="innerException">The inner exception</param>
    protected DomainException(string message, string errorCode, object? details, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Details = details;
    }

    /// <summary>
    /// Returns a string representation of the exception
    /// </summary>
    public override string ToString()
    {
        var result = $"[{ErrorCode}] {Message}";
        if (Details != null)
        {
            result += $" | Details: {Details}";
        }
        if (InnerException != null)
        {
            result += $" | Inner: {InnerException.Message}";
        }
        return result;
    }
} 