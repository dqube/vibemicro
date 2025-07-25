using BuildingBlocks.Domain.Guards;
using BuildingBlocks.Domain.ValueObjects;

namespace ContactService.Domain.ValueObjects;

public sealed record ContactNumberTypeName : SingleValueObject<string>
{
    public ContactNumberTypeName(string value) : base(value)
    {
    }
    
    protected override string ValidateValue(string value)
    {
        Guard.Against.NullOrWhiteSpace(value, nameof(value));
        Guard.Against.LengthOutOfRange(value, nameof(value), 1, 50);
        
        // Ensure name contains only letters, spaces, and common punctuation
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z\s\-\.&/]+$"))
        {
            throw new ArgumentException("Contact number type name can only contain letters, spaces, hyphens, dots, ampersands, and forward slashes", nameof(value));
        }
        
        return value.Trim();
    }
    
    public static implicit operator string(ContactNumberTypeName name) => name.Value;
    public static explicit operator ContactNumberTypeName(string value) => new(value);
    
    // Predefined common types
    public static ContactNumberTypeName Mobile => new("Mobile");
    public static ContactNumberTypeName Home => new("Home");
    public static ContactNumberTypeName Work => new("Work");
    public static ContactNumberTypeName Emergency => new("Emergency");
    public static ContactNumberTypeName Fax => new("Fax");
    public static ContactNumberTypeName Other => new("Other");
} 