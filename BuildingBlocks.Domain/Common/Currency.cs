using BuildingBlocks.Domain.Guards;
using BuildingBlocks.Domain.ValueObjects;

namespace BuildingBlocks.Domain.Common;

/// <summary>
/// Value object representing a currency code (ISO 4217)
/// </summary>
public sealed record Currency : SingleValueObject<string>
{
    /// <summary>
    /// Gets common currency instances
    /// </summary>
    public static Currency USD => new("USD");
    public static Currency EUR => new("EUR");
    public static Currency GBP => new("GBP");
    public static Currency JPY => new("JPY");
    public static Currency CAD => new("CAD");
    public static Currency AUD => new("AUD");
    public static Currency CHF => new("CHF");
    public static Currency CNY => new("CNY");
    public static Currency SEK => new("SEK");
    public static Currency NZD => new("NZD");

    /// <summary>
    /// Gets the currency symbol if known
    /// </summary>
    public string Symbol => GetCurrencySymbol(Value);

    /// <summary>
    /// Gets the currency name if known
    /// </summary>
    public string Name => GetCurrencyName(Value);

    /// <summary>
    /// Gets the number of decimal places typically used for this currency
    /// </summary>
    public int DecimalPlaces => GetDecimalPlaces(Value);

    /// <summary>
    /// Initializes a new instance of the Currency class
    /// </summary>
    /// <param name="value">The currency code (ISO 4217)</param>
    public Currency(string value) : base(ValidateAndNormalize(value))
    {
    }

    /// <summary>
    /// Checks if this currency supports fractional units
    /// </summary>
    /// <returns>True if the currency supports fractional units</returns>
    public bool SupportsFractionalUnits()
    {
        return DecimalPlaces > 0;
    }

    /// <summary>
    /// Formats an amount in this currency
    /// </summary>
    /// <param name="amount">The amount to format</param>
    /// <param name="includeSymbol">Whether to include the currency symbol</param>
    /// <returns>The formatted amount</returns>
    public string FormatAmount(decimal amount, bool includeSymbol = true)
    {
        var formatString = DecimalPlaces > 0 ? $"F{DecimalPlaces}" : "F0";
        var formattedAmount = amount.ToString(formatString);
        
        return includeSymbol ? $"{Symbol}{formattedAmount}" : formattedAmount;
    }

    /// <summary>
    /// Gets all supported currencies
    /// </summary>
    /// <returns>A collection of supported currencies</returns>
    public static IEnumerable<Currency> GetSupportedCurrencies()
    {
        return new[]
        {
            USD, EUR, GBP, JPY, CAD, AUD, CHF, CNY, SEK, NZD
        };
    }

    /// <summary>
    /// Tries to create a currency from a string
    /// </summary>
    /// <param name="currencyCode">The currency code</param>
    /// <param name="currency">The created currency if successful</param>
    /// <returns>True if the currency was created successfully</returns>
    public static bool TryCreate(string currencyCode, out Currency? currency)
    {
        try
        {
            currency = new Currency(currencyCode);
            return true;
        }
        catch
        {
            currency = null;
            return false;
        }
    }

    /// <summary>
    /// Validates and normalizes the currency code
    /// </summary>
    /// <param name="value">The currency code to validate</param>
    /// <returns>The validated and normalized currency code</returns>
    private static string ValidateAndNormalize(string value)
    {
        Guard.NotNullOrWhiteSpace(value);
        
        var normalized = value.Trim().ToUpperInvariant();
        
        if (normalized.Length != 3)
        {
            throw new ArgumentException($"Currency code must be exactly 3 characters: {value}", nameof(value));
        }
        
        if (!IsValidCurrencyCode(normalized))
        {
            throw new ArgumentException($"Invalid currency code: {value}", nameof(value));
        }
        
        return normalized;
    }

    /// <summary>
    /// Checks if a currency code is valid (basic validation)
    /// </summary>
    /// <param name="code">The currency code to check</param>
    /// <returns>True if the code is valid</returns>
    private static bool IsValidCurrencyCode(string code)
    {
        // Basic validation - all letters, 3 characters
        return code.Length == 3 && code.All(char.IsLetter);
    }

    /// <summary>
    /// Gets the currency symbol for a currency code
    /// </summary>
    /// <param name="currencyCode">The currency code</param>
    /// <returns>The currency symbol</returns>
    private static string GetCurrencySymbol(string currencyCode)
    {
        return currencyCode switch
        {
            "USD" => "$",
            "EUR" => "€",
            "GBP" => "£",
            "JPY" => "¥",
            "CAD" => "C$",
            "AUD" => "A$",
            "CHF" => "CHF",
            "CNY" => "¥",
            "SEK" => "kr",
            "NZD" => "NZ$",
            _ => currencyCode
        };
    }

    /// <summary>
    /// Gets the currency name for a currency code
    /// </summary>
    /// <param name="currencyCode">The currency code</param>
    /// <returns>The currency name</returns>
    private static string GetCurrencyName(string currencyCode)
    {
        return currencyCode switch
        {
            "USD" => "US Dollar",
            "EUR" => "Euro",
            "GBP" => "British Pound",
            "JPY" => "Japanese Yen",
            "CAD" => "Canadian Dollar",
            "AUD" => "Australian Dollar",
            "CHF" => "Swiss Franc",
            "CNY" => "Chinese Yuan",
            "SEK" => "Swedish Krona",
            "NZD" => "New Zealand Dollar",
            _ => currencyCode
        };
    }

    /// <summary>
    /// Gets the number of decimal places for a currency
    /// </summary>
    /// <param name="currencyCode">The currency code</param>
    /// <returns>The number of decimal places</returns>
    private static int GetDecimalPlaces(string currencyCode)
    {
        return currencyCode switch
        {
            "JPY" => 0,  // Japanese Yen doesn't use decimal places
            "KRW" => 0,  // Korean Won doesn't use decimal places
            _ => 2       // Most currencies use 2 decimal places
        };
    }

    /// <summary>
    /// Returns the string representation of the currency
    /// </summary>
    public override string ToString() => Value;
} 