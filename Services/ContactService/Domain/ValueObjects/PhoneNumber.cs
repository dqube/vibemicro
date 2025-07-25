using BuildingBlocks.Domain.Guards;
using BuildingBlocks.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace ContactService.Domain.ValueObjects;

public sealed record PhoneNumber : SingleValueObject<string>
{
    private static readonly Regex PhoneRegex = new(@"^\+?[\d\s\-\(\)\.]{7,20}$", RegexOptions.Compiled);
    
    public PhoneNumber(string value) : base(value)
    {
    }
    
    protected override string ValidateValue(string value)
    {
        Guard.Against.NullOrWhiteSpace(value, nameof(value));
        
        // Clean the phone number (remove spaces, dashes, parentheses, dots)
        var cleanedValue = Regex.Replace(value, @"[\s\-\(\)\.]", "");
        
        // Validate format
        Guard.Against.InvalidFormat(value, nameof(value), PhoneRegex, "Phone number must be 7-20 digits with optional formatting");
        
        // Ensure it has enough digits
        var digitsOnly = Regex.Replace(cleanedValue, @"[^\d]", "");
        if (digitsOnly.Length < 7 || digitsOnly.Length > 15)
        {
            throw new ArgumentException("Phone number must contain 7-15 digits", nameof(value));
        }
        
        return value.Trim();
    }
    
    // Convenience methods
    public string GetDigitsOnly() => Regex.Replace(Value, @"[^\d]", "");
    public bool IsInternational() => Value.StartsWith('+');
    public string GetCountryCode()
    {
        if (!IsInternational()) return string.Empty;
        
        var digits = GetDigitsOnly();
        // Simple heuristic for common country codes
        if (digits.StartsWith("1")) return "1"; // US/Canada
        if (digits.StartsWith("44")) return "44"; // UK
        if (digits.StartsWith("49")) return "49"; // Germany
        if (digits.StartsWith("33")) return "33"; // France
        
        return string.Empty; // Unknown or needs more sophisticated parsing
    }
    
    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
    public static explicit operator PhoneNumber(string value) => new(value);
    
    // Common formats
    public static PhoneNumber FromUS(string areaCode, string exchange, string number)
    {
        Guard.Against.NullOrWhiteSpace(areaCode, nameof(areaCode));
        Guard.Against.NullOrWhiteSpace(exchange, nameof(exchange));
        Guard.Against.NullOrWhiteSpace(number, nameof(number));
        
        return new PhoneNumber($"({areaCode}) {exchange}-{number}");
    }
    
    public static PhoneNumber FromInternational(string countryCode, string nationalNumber)
    {
        Guard.Against.NullOrWhiteSpace(countryCode, nameof(countryCode));
        Guard.Against.NullOrWhiteSpace(nationalNumber, nameof(nationalNumber));
        
        var cleanCountryCode = countryCode.TrimStart('+');
        return new PhoneNumber($"+{cleanCountryCode} {nationalNumber}");
    }
} 