namespace BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// Base record for value objects that contain a single value
/// </summary>
/// <typeparam name="T">The type of the wrapped value</typeparam>
public abstract record SingleValueObject<T> : ValueObject
    where T : notnull
{
    /// <summary>
    /// Gets the wrapped value with validation
    /// </summary>
    public T Value { get; init; }
    
    /// <summary>
    /// Initializes a new instance of the SingleValueObject record
    /// </summary>
    /// <param name="value">The value to wrap</param>
    protected SingleValueObject(T value)
    {
        Value = ValidateValue(value);
    }

    /// <summary>
    /// Validates the input value
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <returns>The validated value</returns>
    protected virtual T ValidateValue(T value)
    {
        return value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets the atomic values that define the value object's equality
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the string representation of the value
    /// </summary>
    public override string ToString()
    {
        return Value.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Implicit conversion to the wrapped type
    /// </summary>
    public static implicit operator T(SingleValueObject<T> valueObject)
    {
        return valueObject.Value;
    }
} 