using BuildingBlocks.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace BuildingBlocks.Domain.Common;

/// <summary>
/// Value object representing a phone number
/// </summary>
public class PhoneNumber : SingleValueObject<string>
{
    private static readonly Regex PhoneRegex = new(
        @"^\+?[1-9]\d{1,14}$",
        RegexOptions.Compiled);

    private static readonly Regex DigitsOnlyRegex = new(
        @"[^\d+]",
        RegexOptions.Compiled);

    /// <summary>
    /// Initializes a new instance of the PhoneNumber class
    /// </summary>
    /// <param name="value">The phone number</param>
    public PhoneNumber(string value) : base(ValidateAndNormalize(value))
    {
    }

    /// <summary>
    /// Gets the country code if present (including the + prefix)
    /// </summary>
    public string? CountryCode
    {
        get
        {
            if (!Value.StartsWith("+"))
                return null;

            // Extract country code (1-3 digits after +)
            var match = Regex.Match(Value, @"^\+(\d{1,3})");
            return match.Success ? match.Value : null;
        }
    }

    /// <summary>
    /// Gets the phone number without the country code
    /// </summary>
    public string NationalNumber
    {
        get
        {
            var countryCode = CountryCode;
            return countryCode != null ? Value[countryCode.Length..] : Value;
        }
    }

    /// <summary>
    /// Checks if the phone number has a country code
    /// </summary>
    public bool HasCountryCode => Value.StartsWith("+");

    /// <summary>
    /// Gets the phone number formatted for display
    /// </summary>
    public string DisplayFormat
    {
        get
        {
            if (HasCountryCode)
            {
                var countryCode = CountryCode;
                var national = NationalNumber;
                
                // Format based on length and country code
                if (countryCode == "+1" && national.Length == 10)
                {
                    // US/Canada format: +1 (555) 123-4567
                    return $"{countryCode} ({national[..3]}) {national[3..6]}-{national[6..]}";
                }
                
                // Generic international format: +CC XXXXXXXXX
                return $"{countryCode} {national}";
            }
            
            // National format without country code
            if (Value.Length == 10)
            {
                // Assume US format: (555) 123-4567
                return $"({Value[..3]}) {Value[3..6]}-{Value[6..]}";
            }
            
            return Value;
        }
    }

    /// <summary>
    /// Checks if the phone number is from the specified country code
    /// </summary>
    /// <param name="countryCode">The country code to check (with or without +)</param>
    /// <returns>True if the phone number is from the specified country</returns>
    public bool IsFromCountry(string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            return false;

        var normalizedCode = countryCode.StartsWith("+") ? countryCode : $"+{countryCode}";
        return Value.StartsWith(normalizedCode);
    }

    /// <summary>
    /// Adds a country code to the phone number if it doesn't have one
    /// </summary>
    /// <param name="countryCode">The country code to add (with or without +)</param>
    /// <returns>A new PhoneNumber with the country code</returns>
    public PhoneNumber WithCountryCode(string countryCode)
    {
        if (HasCountryCode)
            return this;

        var normalizedCode = countryCode.StartsWith("+") ? countryCode : $"+{countryCode}";
        return new PhoneNumber($"{normalizedCode}{Value}");
    }

    /// <summary>
    /// Removes the country code from the phone number
    /// </summary>
    /// <returns>A new PhoneNumber without the country code</returns>
    public PhoneNumber WithoutCountryCode()
    {
        return HasCountryCode ? new PhoneNumber(NationalNumber) : this;
    }

    /// <summary>
    /// Checks if this is a valid phone number format
    /// </summary>
    /// <param name="phoneNumber">The phone number string to validate</param>
    /// <returns>True if the phone number format is valid</returns>
    public static bool IsValid(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        try
        {
            var normalized = NormalizePhoneNumber(phoneNumber);
            return PhoneRegex.IsMatch(normalized);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Tries to create a PhoneNumber instance from a string
    /// </summary>
    /// <param name="value">The phone number string</param>
    /// <param name="phoneNumber">The resulting PhoneNumber instance</param>
    /// <returns>True if the phone number was created successfully</returns>
    public static bool TryCreate(string value, out PhoneNumber? phoneNumber)
    {
        phoneNumber = null;
        
        if (!IsValid(value))
            return false;

        try
        {
            phoneNumber = new PhoneNumber(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a PhoneNumber instance if the string is valid, otherwise returns null
    /// </summary>
    /// <param name="value">The phone number string</param>
    /// <returns>A PhoneNumber instance or null if invalid</returns>
    public static PhoneNumber? CreateIfValid(string value)
    {
        return TryCreate(value, out var phoneNumber) ? phoneNumber : null;
    }

    /// <summary>
    /// Validates and normalizes the phone number
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate</param>
    /// <returns>The normalized phone number</returns>
    /// <exception cref="ArgumentException">Thrown when the phone number format is invalid</exception>
    private static string ValidateAndNormalize(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be null or empty", nameof(phoneNumber));

        var normalized = NormalizePhoneNumber(phoneNumber);

        if (!PhoneRegex.IsMatch(normalized))
            throw new ArgumentException($"Invalid phone number format: {phoneNumber}", nameof(phoneNumber));

        return normalized;
    }

    /// <summary>
    /// Normalizes a phone number by removing non-digit characters except +
    /// </summary>
    /// <param name="phoneNumber">The phone number to normalize</param>
    /// <returns>The normalized phone number</returns>
    private static string NormalizePhoneNumber(string phoneNumber)
    {
        var trimmed = phoneNumber.Trim();
        var hasPlus = trimmed.StartsWith("+");
        
        // Remove all non-digit characters except +
        var digitsOnly = DigitsOnlyRegex.Replace(trimmed, "");
        
        // Add back the + if it was present
        return hasPlus ? $"+{digitsOnly}" : digitsOnly;
    }

    /// <summary>
    /// Returns the string representation of the phone number
    /// </summary>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Returns the formatted string representation of the phone number
    /// </summary>
    /// <param name="formatted">Whether to return the formatted version</param>
    /// <returns>The phone number string</returns>
    public string ToString(bool formatted)
    {
        return formatted ? DisplayFormat : Value;
    }

    /// <summary>
    /// Implicit conversion from PhoneNumber to string
    /// </summary>
    public static implicit operator string(PhoneNumber phoneNumber)
    {
        return phoneNumber.Value;
    }

    /// <summary>
    /// Explicit conversion from string to PhoneNumber
    /// </summary>
    public static explicit operator PhoneNumber(string phoneNumber)
    {
        return new PhoneNumber(phoneNumber);
    }
} 