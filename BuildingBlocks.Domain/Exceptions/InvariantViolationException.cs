namespace BuildingBlocks.Domain.Exceptions;

/// <summary>
/// Exception thrown when a domain invariant is violated
/// </summary>
public class InvariantViolationException : DomainException
{

    /// <summary>
    /// Initializes a new instance of the InvariantViolationException class
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="errorCode">The error code</param>
    public InvariantViolationException(string message, string errorCode)
        : base(message, errorCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the InvariantViolationException class
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="errorCode">The error code</param>
    /// <param name="innerException">The inner exception</param>
    public InvariantViolationException(string message, string errorCode, Exception innerException)
        : base(message, errorCode, innerException)
    {
    }

    /// <summary>
    /// Returns a string representation of the exception
    /// </summary>
    public override string ToString()
    {
        return $"Invariant violation: {Message} (Code: {ErrorCode})";
    }
} 