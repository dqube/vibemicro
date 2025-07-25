namespace BuildingBlocks.Domain.StronglyTypedIds;

/// <summary>
/// Interface for strongly-typed identifiers
/// </summary>
/// <typeparam name="T">The underlying type of the identifier</typeparam>
public interface IStronglyTypedId<out T> : IEquatable<IStronglyTypedId<T>>
    where T : notnull
{
    /// <summary>
    /// Gets the underlying value of the identifier
    /// </summary>
    T Value { get; }
} 