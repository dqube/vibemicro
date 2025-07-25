using System.Reflection;

namespace BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// Base class for creating domain enumerations
/// </summary>
public abstract class Enumeration : IComparable
{
    /// <summary>
    /// Gets the unique identifier for this enumeration value
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    /// Gets the name of this enumeration value
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the Enumeration class
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="name">The name</param>
    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Returns the string representation of the enumeration
    /// </summary>
    public override string ToString() => Name;

    /// <summary>
    /// Gets all enumeration values of the specified type
    /// </summary>
    /// <typeparam name="T">The enumeration type</typeparam>
    /// <returns>All enumeration values</returns>
    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        var fields = typeof(T).GetFields(BindingFlags.Public |
                                       BindingFlags.Static |
                                       BindingFlags.DeclaredOnly);

        return fields.Select(f => f.GetValue(null)).Cast<T>();
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current enumeration
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue || GetType() != obj.GetType())
            return false;

        return Id.Equals(otherValue.Id);
    }

    /// <summary>
    /// Returns the hash code for this enumeration
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    /// Gets the absolute difference between two enumeration values
    /// </summary>
    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        return Math.Abs(firstValue.Id - secondValue.Id);
    }

    /// <summary>
    /// Gets an enumeration value by its identifier
    /// </summary>
    /// <typeparam name="T">The enumeration type</typeparam>
    /// <param name="value">The identifier</param>
    /// <returns>The enumeration value</returns>
    public static T FromValue<T>(int value) where T : Enumeration
    {
        var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
        return matchingItem;
    }

    /// <summary>
    /// Gets an enumeration value by its name
    /// </summary>
    /// <typeparam name="T">The enumeration type</typeparam>
    /// <param name="displayName">The name</param>
    /// <returns>The enumeration value</returns>
    public static T FromDisplayName<T>(string displayName) where T : Enumeration
    {
        var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
        return matchingItem;
    }

    /// <summary>
    /// Parses an enumeration value using the specified predicate
    /// </summary>
    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
            throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

        return matchingItem;
    }

    /// <summary>
    /// Compares this enumeration to another object
    /// </summary>
    public int CompareTo(object? other)
    {
        if (other is null) return 1;
        return Id.CompareTo(((Enumeration)other).Id);
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(Enumeration? left, Enumeration? right)
    {
        return EqualityComparer<Enumeration>.Default.Equals(left, right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(Enumeration? left, Enumeration? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Less than operator
    /// </summary>
    public static bool operator <(Enumeration left, Enumeration right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Less than or equal operator
    /// </summary>
    public static bool operator <=(Enumeration left, Enumeration right)
    {
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Greater than operator
    /// </summary>
    public static bool operator >(Enumeration left, Enumeration right)
    {
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Greater than or equal operator
    /// </summary>
    public static bool operator >=(Enumeration left, Enumeration right)
    {
        return left.CompareTo(right) >= 0;
    }
} 