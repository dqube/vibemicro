using BuildingBlocks.Domain.Guards;
using BuildingBlocks.Domain.ValueObjects;

namespace ContactService.Domain.ValueObjects;

public sealed record Address : ValueObject
{
    public string Line1 { get; init; } = string.Empty;
    public string? Line2 { get; init; }
    public string City { get; init; } = string.Empty;
    public string? State { get; init; }
    public string PostalCode { get; init; } = string.Empty;
    public string CountryCode { get; init; } = string.Empty;
    
    public Address(
        string line1, 
        string city, 
        string postalCode, 
        string countryCode,
        string? line2 = null,
        string? state = null)
    {
        Line1 = Guard.Against.NullOrWhiteSpace(line1, nameof(line1));
        Line2 = string.IsNullOrWhiteSpace(line2) ? null : line2.Trim();
        City = Guard.Against.NullOrWhiteSpace(city, nameof(city));
        State = string.IsNullOrWhiteSpace(state) ? null : state.Trim();
        PostalCode = Guard.Against.NullOrWhiteSpace(postalCode, nameof(postalCode));
        CountryCode = Guard.Against.NullOrWhiteSpace(countryCode, nameof(countryCode));
        
        Validate();
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Line1;
        yield return Line2;
        yield return City;
        yield return State;
        yield return PostalCode;
        yield return CountryCode;
    }
    
    protected override void Validate()
    {
        // Validate Line1 length
        if (Line1.Length > 100)
            throw new ArgumentException("Address line 1 cannot exceed 100 characters", nameof(Line1));
        
        // Validate Line2 length if provided
        if (Line2?.Length > 100)
            throw new ArgumentException("Address line 2 cannot exceed 100 characters", nameof(Line2));
        
        // Validate City length
        if (City.Length > 50)
            throw new ArgumentException("City cannot exceed 50 characters", nameof(City));
        
        // Validate State length if provided
        if (State?.Length > 50)
            throw new ArgumentException("State cannot exceed 50 characters", nameof(State));
        
        // Validate PostalCode length
        if (PostalCode.Length > 20)
            throw new ArgumentException("Postal code cannot exceed 20 characters", nameof(PostalCode));
        
        // Validate CountryCode format (ISO 3166-1 alpha-2)
        if (CountryCode.Length != 2)
            throw new ArgumentException("Country code must be exactly 2 characters (ISO 3166-1 alpha-2)", nameof(CountryCode));
        
        // Validate postal code format based on country (basic validation)
        ValidatePostalCodeFormat();
    }
    
    private void ValidatePostalCodeFormat()
    {
        switch (CountryCode.ToUpperInvariant())
        {
            case "US":
                if (!System.Text.RegularExpressions.Regex.IsMatch(PostalCode, @"^\d{5}(-\d{4})?$"))
                    throw new ArgumentException("US postal code must be in format 12345 or 12345-6789", nameof(PostalCode));
                break;
            case "CA":
                if (!System.Text.RegularExpressions.Regex.IsMatch(PostalCode, @"^[A-Za-z]\d[A-Za-z] ?\d[A-Za-z]\d$"))
                    throw new ArgumentException("Canadian postal code must be in format A1A 1A1", nameof(PostalCode));
                break;
            case "GB":
                if (!System.Text.RegularExpressions.Regex.IsMatch(PostalCode, @"^[A-Za-z]{1,2}\d[A-Za-z\d]? ?\d[A-Za-z]{2}$"))
                    throw new ArgumentException("UK postal code must be in valid format (e.g., SW1A 1AA)", nameof(PostalCode));
                break;
            // Add more country validations as needed
        }
    }
    
    // Convenience methods
    public string GetFullAddress()
    {
        var parts = new List<string> { Line1 };
        
        if (!string.IsNullOrWhiteSpace(Line2))
            parts.Add(Line2);
        
        parts.Add(City);
        
        if (!string.IsNullOrWhiteSpace(State))
            parts.Add(State);
        
        parts.Add(PostalCode);
        parts.Add(CountryCode);
        
        return string.Join(", ", parts);
    }
    
    public string GetShortAddress()
    {
        var cityState = string.IsNullOrWhiteSpace(State) ? City : $"{City}, {State}";
        return $"{Line1}, {cityState} {PostalCode}";
    }
    
    public bool IsInCountry(string countryCode)
    {
        return string.Equals(CountryCode, countryCode, StringComparison.OrdinalIgnoreCase);
    }
    
    public bool IsComplete()
    {
        return !string.IsNullOrWhiteSpace(Line1) &&
               !string.IsNullOrWhiteSpace(City) &&
               !string.IsNullOrWhiteSpace(PostalCode) &&
               !string.IsNullOrWhiteSpace(CountryCode);
    }
    
    // Factory methods for common countries
    public static Address CreateUSAddress(string line1, string city, string state, string zipCode, string? line2 = null)
    {
        return new Address(line1, city, zipCode, "US", line2, state);
    }
    
    public static Address CreateCanadianAddress(string line1, string city, string province, string postalCode, string? line2 = null)
    {
        return new Address(line1, city, postalCode, "CA", line2, province);
    }
    
    public static Address CreateUKAddress(string line1, string city, string postcode, string? line2 = null, string? county = null)
    {
        return new Address(line1, city, postcode, "GB", line2, county);
    }
} 