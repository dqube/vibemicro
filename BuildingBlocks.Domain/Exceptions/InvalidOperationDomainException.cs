namespace BuildingBlocks.Domain.Exceptions;

/// <summary>
/// Exception thrown when an invalid operation is attempted in the domain
/// </summary>
public class InvalidOperationDomainException : DomainException
{
    /// <summary>
    /// Gets the operation that was attempted
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Gets the reason why the operation is invalid
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// Initializes a new instance of the InvalidOperationDomainException class
    /// </summary>
    /// <param name="operation">The operation that was attempted</param>
    /// <param name="reason">The reason why the operation is invalid</param>
    public InvalidOperationDomainException(string operation, string reason)
        : base($"Invalid operation '{operation}': {reason}", 
               "INVALID_DOMAIN_OPERATION", 
               new { Operation = operation, Reason = reason })
    {
        Operation = operation;
        Reason = reason;
    }

    /// <summary>
    /// Initializes a new instance of the InvalidOperationDomainException class
    /// </summary>
    /// <param name="operation">The operation that was attempted</param>
    /// <param name="reason">The reason why the operation is invalid</param>
    /// <param name="innerException">The inner exception</param>
    public InvalidOperationDomainException(string operation, string reason, Exception innerException)
        : base($"Invalid operation '{operation}': {reason}", 
               "INVALID_DOMAIN_OPERATION", 
               new { Operation = operation, Reason = reason }, 
               innerException)
    {
        Operation = operation;
        Reason = reason;
    }

    /// <summary>
    /// Creates an InvalidOperationDomainException with a formatted message
    /// </summary>
    /// <param name="operation">The operation that was attempted</param>
    /// <param name="reasonFormat">The format string for the reason</param>
    /// <param name="args">The arguments for the format string</param>
    /// <returns>A new InvalidOperationDomainException</returns>
    public static InvalidOperationDomainException Create(string operation, string reasonFormat, params object[] args)
    {
        var reason = string.Format(reasonFormat, args);
        return new InvalidOperationDomainException(operation, reason);
    }

    /// <summary>
    /// Creates an InvalidOperationDomainException for when a required condition is not met
    /// </summary>
    /// <param name="operation">The operation that was attempted</param>
    /// <param name="condition">The condition that was not met</param>
    /// <returns>A new InvalidOperationDomainException</returns>
    public static InvalidOperationDomainException ConditionNotMet(string operation, string condition)
    {
        return new InvalidOperationDomainException(operation, $"Required condition not met: {condition}");
    }

    /// <summary>
    /// Creates an InvalidOperationDomainException for when an entity is in an invalid state
    /// </summary>
    /// <param name="operation">The operation that was attempted</param>
    /// <param name="entityType">The type of the entity</param>
    /// <param name="entityId">The identifier of the entity</param>
    /// <param name="currentState">The current state of the entity</param>
    /// <returns>A new InvalidOperationDomainException</returns>
    public static InvalidOperationDomainException InvalidState(string operation, Type entityType, object entityId, string currentState)
    {
        var reason = $"Entity '{entityType.Name}' with id '{entityId}' is in invalid state '{currentState}' for this operation";
        return new InvalidOperationDomainException(operation, reason);
    }
} 