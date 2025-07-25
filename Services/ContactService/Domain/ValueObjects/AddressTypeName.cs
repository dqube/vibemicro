using BuildingBlocks.Domain.Guards;
using BuildingBlocks.Domain.ValueObjects;

namespace ContactService.Domain.ValueObjects;

public sealed record AddressTypeName : SingleValueObject<string>
{
    public AddressTypeName(string value) : base(value)
    {
    }
    
    protected override string ValidateValue(string value)
    {
        Guard.Against.NullOrWhiteSpace(value, nameof(value));
        Guard.Against.LengthOutOfRange(value, nameof(value), 1, 50);
        
        // Ensure name contains only letters, spaces, and common punctuation
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z\s\-\.&/]+$"))
        {
            throw new ArgumentException("Address type name can only contain letters, spaces, hyphens, dots, ampersands, and forward slashes", nameof(value));
        }
        
        return value.Trim();
    }
    
    public static implicit operator string(AddressTypeName name) => name.Value;
    public static explicit operator AddressTypeName(string value) => new(value);
    
    // Predefined common types
    public static AddressTypeName Home => new("Home");
    public static AddressTypeName Work => new("Work");
    public static AddressTypeName Billing => new("Billing");
    public static AddressTypeName Shipping => new("Shipping");
    public static AddressTypeName Warehouse => new("Warehouse");
    public static AddressTypeName Headquarters => new("Headquarters");
} 