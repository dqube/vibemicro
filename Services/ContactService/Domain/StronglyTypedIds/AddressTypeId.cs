using BuildingBlocks.Domain.StronglyTypedIds;

namespace ContactService.Domain.StronglyTypedIds;

public readonly struct AddressTypeId : IStronglyTypedId<int>
{
    public int Value { get; }
    
    public AddressTypeId(int value) => Value = value;
    
    public static AddressTypeId New() => new(0); // For new entities, will be set by database
    public static AddressTypeId Empty => new(0);
    
    // Implicit conversion to int
    public static implicit operator int(AddressTypeId id) => id.Value;
    
    // Explicit conversion from int
    public static explicit operator AddressTypeId(int value) => new(value);
    
    // Equality and comparison
    public bool Equals(AddressTypeId other) => Value.Equals(other.Value);
    public override bool Equals(object? obj) => obj is AddressTypeId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
    
    // Operators
    public static bool operator ==(AddressTypeId left, AddressTypeId right) => left.Equals(right);
    public static bool operator !=(AddressTypeId left, AddressTypeId right) => !left.Equals(right);
} 