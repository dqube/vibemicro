using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace BuildingBlocks.Domain.Guards;

/// <summary>
/// Provides guard clauses for defensive programming
/// </summary>
public static class Guard
{
    /// <summary>
    /// Guards against null values
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-null value</returns>
    /// <exception cref="ArgumentNullException">Thrown when value is null</exception>
    public static T NotNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : class
    {
        if (value is null)
            throw new ArgumentNullException(paramName);
        return value;
    }

    /// <summary>
    /// Guards against null or empty strings
    /// </summary>
    /// <param name="value">The string value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-null, non-empty string</returns>
    /// <exception cref="ArgumentException">Thrown when value is null or empty</exception>
    public static string NotNullOrEmpty([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
        return value;
    }

    /// <summary>
    /// Guards against null, empty, or whitespace strings
    /// </summary>
    /// <param name="value">The string value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-null, non-empty, non-whitespace string</returns>
    /// <exception cref="ArgumentException">Thrown when value is null, empty, or whitespace</exception>
    public static string NotNullOrWhiteSpace([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be null, empty, or whitespace.", paramName);
        return value;
    }

    /// <summary>
    /// Guards against default values for value types
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-default value</returns>
    /// <exception cref="ArgumentException">Thrown when value is default</exception>
    public static T NotDefault<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : struct
    {
        if (value.Equals(default(T)))
            throw new ArgumentException($"{paramName} cannot be the default value.", paramName);
        return value;
    }

    /// <summary>
    /// Guards against negative numbers
    /// </summary>
    /// <param name="value">The numeric value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-negative value</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative</exception>
    public static int NotNegative(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} cannot be negative.");
        return value;
    }

    /// <summary>
    /// Guards against negative numbers
    /// </summary>
    /// <param name="value">The numeric value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-negative value</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative</exception>
    public static long NotNegative(long value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} cannot be negative.");
        return value;
    }

    /// <summary>
    /// Guards against negative numbers
    /// </summary>
    /// <param name="value">The numeric value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-negative value</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative</exception>
    public static decimal NotNegative(decimal value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} cannot be negative.");
        return value;
    }

    /// <summary>
    /// Guards against zero or negative numbers
    /// </summary>
    /// <param name="value">The numeric value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The positive value</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is zero or negative</exception>
    public static int Positive(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must be positive.");
        return value;
    }

    /// <summary>
    /// Guards against zero or negative numbers
    /// </summary>
    /// <param name="value">The numeric value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The positive value</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is zero or negative</exception>
    public static long Positive(long value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must be positive.");
        return value;
    }

    /// <summary>
    /// Guards against zero or negative numbers
    /// </summary>
    /// <param name="value">The numeric value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The positive value</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is zero or negative</exception>
    public static decimal Positive(decimal value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must be positive.");
        return value;
    }

    /// <summary>
    /// Guards against values outside the specified range
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value to check</param>
    /// <param name="min">The minimum allowed value</param>
    /// <param name="max">The maximum allowed value</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The value within range</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is outside the specified range</exception>
    public static T InRange<T>(T value, T min, T max, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must be between {min} and {max}.");
        return value;
    }

    /// <summary>
    /// Guards against null or empty collections
    /// </summary>
    /// <typeparam name="T">The collection type</typeparam>
    /// <param name="collection">The collection to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-null, non-empty collection</returns>
    /// <exception cref="ArgumentNullException">Thrown when collection is null</exception>
    /// <exception cref="ArgumentException">Thrown when collection is empty</exception>
    public static T NotNullOrEmpty<T>([NotNull] T? collection, [CallerArgumentExpression(nameof(collection))] string? paramName = null)
        where T : class, IEnumerable
    {
        NotNull(collection, paramName);
        if (!collection.Cast<object>().Any())
            throw new ArgumentException($"{paramName} cannot be empty.", paramName);
        return collection;
    }

    /// <summary>
    /// Guards against invalid email format
    /// </summary>
    /// <param name="email">The email to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The valid email</returns>
    /// <exception cref="ArgumentException">Thrown when email format is invalid</exception>
    public static string ValidEmail([NotNull] string? email, [CallerArgumentExpression(nameof(email))] string? paramName = null)
    {
        NotNullOrWhiteSpace(email, paramName);
        if (!email.Contains('@') || !email.Contains('.'))
            throw new ArgumentException($"{paramName} must be a valid email address.", paramName);
        return email;
    }

    /// <summary>
    /// Guards against strings that exceed maximum length
    /// </summary>
    /// <param name="value">The string value to check</param>
    /// <param name="maxLength">The maximum allowed length</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The string within length limit</returns>
    /// <exception cref="ArgumentException">Thrown when string exceeds maximum length</exception>
    public static string MaxLength([NotNull] string? value, int maxLength, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        NotNull(value, paramName);
        if (value.Length > maxLength)
            throw new ArgumentException($"{paramName} cannot exceed {maxLength} characters.", paramName);
        return value;
    }

    /// <summary>
    /// Guards against strings shorter than minimum length
    /// </summary>
    /// <param name="value">The string value to check</param>
    /// <param name="minLength">The minimum required length</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The string meeting minimum length</returns>
    /// <exception cref="ArgumentException">Thrown when string is shorter than minimum length</exception>
    public static string MinLength([NotNull] string? value, int minLength, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        NotNull(value, paramName);
        if (value.Length < minLength)
            throw new ArgumentException($"{paramName} must be at least {minLength} characters.", paramName);
        return value;
    }
} 