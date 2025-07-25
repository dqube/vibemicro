using System.Runtime.CompilerServices;

namespace BuildingBlocks.Domain.Guards;

/// <summary>
/// Extension methods for guard clauses providing fluent syntax
/// </summary>
public static class GuardExtensions
{
    /// <summary>
    /// Guards against null values using fluent syntax
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-null value</returns>
    public static T GuardNotNull<T>(this T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : class
    {
        return Guard.NotNull(value, paramName);
    }

    /// <summary>
    /// Guards against null or empty strings using fluent syntax
    /// </summary>
    /// <param name="value">The string value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-null, non-empty string</returns>
    public static string GuardNotNullOrEmpty(this string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        return Guard.NotNullOrEmpty(value, paramName);
    }

    /// <summary>
    /// Guards against null, empty, or whitespace strings using fluent syntax
    /// </summary>
    /// <param name="value">The string value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-null, non-empty, non-whitespace string</returns>
    public static string GuardNotNullOrWhiteSpace(this string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        return Guard.NotNullOrWhiteSpace(value, paramName);
    }

    /// <summary>
    /// Guards against default values for value types using fluent syntax
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-default value</returns>
    public static T GuardNotDefault<T>(this T value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : struct
    {
        return Guard.NotDefault(value, paramName);
    }

    /// <summary>
    /// Guards against negative numbers using fluent syntax
    /// </summary>
    /// <param name="value">The numeric value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-negative value</returns>
    public static int GuardNotNegative(this int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        return Guard.NotNegative(value, paramName);
    }

    /// <summary>
    /// Guards against negative numbers using fluent syntax
    /// </summary>
    /// <param name="value">The numeric value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-negative value</returns>
    public static long GuardNotNegative(this long value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        return Guard.NotNegative(value, paramName);
    }

    /// <summary>
    /// Guards against negative numbers using fluent syntax
    /// </summary>
    /// <param name="value">The numeric value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The non-negative value</returns>
    public static decimal GuardNotNegative(this decimal value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        return Guard.NotNegative(value, paramName);
    }

    /// <summary>
    /// Guards against zero or negative numbers using fluent syntax
    /// </summary>
    /// <param name="value">The numeric value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The positive value</returns>
    public static int GuardPositive(this int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        return Guard.Positive(value, paramName);
    }

    /// <summary>
    /// Guards against zero or negative numbers using fluent syntax
    /// </summary>
    /// <param name="value">The numeric value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The positive value</returns>
    public static long GuardPositive(this long value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        return Guard.Positive(value, paramName);
    }

    /// <summary>
    /// Guards against zero or negative numbers using fluent syntax
    /// </summary>
    /// <param name="value">The numeric value to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The positive value</returns>
    public static decimal GuardPositive(this decimal value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        return Guard.Positive(value, paramName);
    }

    /// <summary>
    /// Guards against values outside the specified range using fluent syntax
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value to check</param>
    /// <param name="min">The minimum allowed value</param>
    /// <param name="max">The maximum allowed value</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The value within range</returns>
    public static T GuardInRange<T>(this T value, T min, T max, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        return Guard.InRange(value, min, max, paramName);
    }

    /// <summary>
    /// Guards against invalid email format using fluent syntax
    /// </summary>
    /// <param name="email">The email to check</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The valid email</returns>
    public static string GuardValidEmail(this string? email, [CallerArgumentExpression(nameof(email))] string? paramName = null)
    {
        return Guard.ValidEmail(email, paramName);
    }

    /// <summary>
    /// Guards against strings that exceed maximum length using fluent syntax
    /// </summary>
    /// <param name="value">The string value to check</param>
    /// <param name="maxLength">The maximum allowed length</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The string within length limit</returns>
    public static string GuardMaxLength(this string? value, int maxLength, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        return Guard.MaxLength(value, maxLength, paramName);
    }

    /// <summary>
    /// Guards against strings shorter than minimum length using fluent syntax
    /// </summary>
    /// <param name="value">The string value to check</param>
    /// <param name="minLength">The minimum required length</param>
    /// <param name="paramName">The parameter name</param>
    /// <returns>The string meeting minimum length</returns>
    public static string GuardMinLength(this string? value, int minLength, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        return Guard.MinLength(value, minLength, paramName);
    }
} 