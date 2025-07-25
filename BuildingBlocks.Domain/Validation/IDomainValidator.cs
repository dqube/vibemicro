namespace BuildingBlocks.Domain.Validation;

/// <summary>
/// Interface for domain validators
/// </summary>
/// <typeparam name="T">The type to validate</typeparam>
public interface IDomainValidator<in T>
{
    /// <summary>
    /// Validates the specified object
    /// </summary>
    /// <param name="obj">The object to validate</param>
    /// <returns>The validation result</returns>
    ValidationResult Validate(T obj);

    /// <summary>
    /// Validates the specified object asynchronously
    /// </summary>
    /// <param name="obj">The object to validate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The validation result</returns>
    Task<ValidationResult> ValidateAsync(T obj, CancellationToken cancellationToken = default);
} 