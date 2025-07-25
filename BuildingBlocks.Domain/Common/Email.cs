using BuildingBlocks.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace BuildingBlocks.Domain.Common;

/// <summary>
/// Value object representing an email address
/// </summary>
public class Email : SingleValueObject<string>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Initializes a new instance of the Email class
    /// </summary>
    /// <param name="value">The email address</param>
    public Email(string value) : base(ValidateAndNormalize(value))
    {
    }

    /// <summary>
    /// Gets the local part of the email address (before the @)
    /// </summary>
    public string LocalPart => Value.Split('@')[0];

    /// <summary>
    /// Gets the domain part of the email address (after the @)
    /// </summary>
    public string Domain => Value.Split('@')[1];

    /// <summary>
    /// Checks if the email address is from the specified domain
    /// </summary>
    /// <param name="domain">The domain to check</param>
    /// <returns>True if the email is from the specified domain</returns>
    public bool IsFromDomain(string domain)
    {
        if (string.IsNullOrWhiteSpace(domain))
            return false;

        return Domain.Equals(domain.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the email address matches the specified pattern
    /// </summary>
    /// <param name="pattern">The regex pattern to match</param>
    /// <returns>True if the email matches the pattern</returns>
    public bool MatchesPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return false;

        try
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(Value);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if the email address ends with the specified domain suffix
    /// </summary>
    /// <param name="suffix">The domain suffix to check</param>
    /// <returns>True if the domain ends with the specified suffix</returns>
    public bool DomainEndsWith(string suffix)
    {
        if (string.IsNullOrWhiteSpace(suffix))
            return false;

        return Domain.EndsWith(suffix.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the top-level domain of the email address
    /// </summary>
    public string TopLevelDomain
    {
        get
        {
            var parts = Domain.Split('.');
            return parts.Length > 0 ? parts[^1] : string.Empty;
        }
    }

    /// <summary>
    /// Checks if this is a valid email address format
    /// </summary>
    /// <param name="email">The email string to validate</param>
    /// <returns>True if the email format is valid</returns>
    public static bool IsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var normalized = email.Trim().ToLowerInvariant();
            return EmailRegex.IsMatch(normalized);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Tries to create an Email instance from a string
    /// </summary>
    /// <param name="value">The email string</param>
    /// <param name="email">The resulting Email instance</param>
    /// <returns>True if the email was created successfully</returns>
    public static bool TryCreate(string value, out Email? email)
    {
        email = null;
        
        if (!IsValid(value))
            return false;

        try
        {
            email = new Email(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates an Email instance if the string is valid, otherwise returns null
    /// </summary>
    /// <param name="value">The email string</param>
    /// <returns>An Email instance or null if invalid</returns>
    public static Email? CreateIfValid(string value)
    {
        return TryCreate(value, out var email) ? email : null;
    }

    /// <summary>
    /// Validates and normalizes the email address
    /// </summary>
    /// <param name="email">The email to validate</param>
    /// <returns>The normalized email address</returns>
    /// <exception cref="ArgumentException">Thrown when the email format is invalid</exception>
    private static string ValidateAndNormalize(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email address cannot be null or empty", nameof(email));

        var normalized = email.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
            throw new ArgumentException($"Invalid email address format: {email}", nameof(email));

        return normalized;
    }

    /// <summary>
    /// Implicit conversion from Email to string
    /// </summary>
    public static implicit operator string(Email email)
    {
        return email.Value;
    }

    /// <summary>
    /// Explicit conversion from string to Email
    /// </summary>
    public static explicit operator Email(string email)
    {
        return new Email(email);
    }
} 