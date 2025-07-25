using BuildingBlocks.Domain.ValueObjects;

namespace BuildingBlocks.Domain.Common;

/// <summary>
/// Value object representing a physical address
/// </summary>
public record Address : ValueObject
{
    /// <summary>
    /// Gets the street address
    /// </summary>
    public string Street { get; private set; }

    /// <summary>
    /// Gets the city
    /// </summary>
    public string City { get; private set; }

    /// <summary>
    /// Gets the state or province
    /// </summary>
    public string State { get; private set; }

    /// <summary>
    /// Gets the postal or ZIP code
    /// </summary>
    public string PostalCode { get; private set; }

    /// <summary>
    /// Gets the country
    /// </summary>
    public string Country { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Address class
    /// </summary>
    /// <param name="street">The street address</param>
    /// <param name="city">The city</param>
    /// <param name="state">The state or province</param>
    /// <param name="postalCode">The postal or ZIP code</param>
    /// <param name="country">The country</param>
    public Address(string street, string city, string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be null or empty", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be null or empty", nameof(city));
        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State cannot be null or empty", nameof(state));
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be null or empty", nameof(postalCode));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be null or empty", nameof(country));

        Street = street.Trim();
        City = city.Trim();
        State = state.Trim();
        PostalCode = postalCode.Trim();
        Country = country.Trim();
    }

    /// <summary>
    /// Gets the atomic values that define the value object's equality
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street.ToLowerInvariant();
        yield return City.ToLowerInvariant();
        yield return State.ToLowerInvariant();
        yield return PostalCode.ToLowerInvariant();
        yield return Country.ToLowerInvariant();
    }

    /// <summary>
    /// Gets the full address as a formatted string
    /// </summary>
    public string FullAddress => $"{Street}, {City}, {State} {PostalCode}, {Country}";

    /// <summary>
    /// Checks if this address is in the specified country
    /// </summary>
    /// <param name="country">The country to check</param>
    /// <returns>True if the address is in the specified country</returns>
    public bool IsInCountry(string country)
    {
        return Country.Equals(country, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if this address is in the specified state
    /// </summary>
    /// <param name="state">The state to check</param>
    /// <returns>True if the address is in the specified state</returns>
    public bool IsInState(string state)
    {
        return State.Equals(state, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if this address is in the specified city
    /// </summary>
    /// <param name="city">The city to check</param>
    /// <returns>True if the address is in the specified city</returns>
    public bool IsInCity(string city)
    {
        return City.Equals(city, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Creates a new address with updated street
    /// </summary>
    /// <param name="newStreet">The new street</param>
    /// <returns>A new Address instance with updated street</returns>
    public Address WithStreet(string newStreet)
    {
        return new Address(newStreet, City, State, PostalCode, Country);
    }

    /// <summary>
    /// Creates a new address with updated city
    /// </summary>
    /// <param name="newCity">The new city</param>
    /// <returns>A new Address instance with updated city</returns>
    public Address WithCity(string newCity)
    {
        return new Address(Street, newCity, State, PostalCode, Country);
    }

    /// <summary>
    /// Creates a new address with updated state
    /// </summary>
    /// <param name="newState">The new state</param>
    /// <returns>A new Address instance with updated state</returns>
    public Address WithState(string newState)
    {
        return new Address(Street, City, newState, PostalCode, Country);
    }

    /// <summary>
    /// Creates a new address with updated postal code
    /// </summary>
    /// <param name="newPostalCode">The new postal code</param>
    /// <returns>A new Address instance with updated postal code</returns>
    public Address WithPostalCode(string newPostalCode)
    {
        return new Address(Street, City, State, newPostalCode, Country);
    }

    /// <summary>
    /// Creates a new address with updated country
    /// </summary>
    /// <param name="newCountry">The new country</param>
    /// <returns>A new Address instance with updated country</returns>
    public Address WithCountry(string newCountry)
    {
        return new Address(Street, City, State, PostalCode, newCountry);
    }

    /// <summary>
    /// Returns the string representation of the address
    /// </summary>
    public override string ToString()
    {
        return FullAddress;
    }

    /// <summary>
    /// Returns the string representation of the address with custom formatting
    /// </summary>
    /// <param name="includeCountry">Whether to include the country</param>
    /// <returns>The formatted address string</returns>
    public string ToString(bool includeCountry)
    {
        if (includeCountry)
            return FullAddress;
        
        return $"{Street}, {City}, {State} {PostalCode}";
    }
} 