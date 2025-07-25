using BuildingBlocks.Domain.StronglyTypedIds;

namespace ContactService.Domain.StronglyTypedIds;

public readonly struct ContactNumberTypeId : IStronglyTypedId<int>
{
    public int Value { get; }
    
    public ContactNumberTypeId(int value) => Value = value;
    
    public static ContactNumberTypeId New() => new(0); // For new entities, will be set by database
    public static ContactNumberTypeId Empty => new(0);
    
    // Implicit conversion to int
    public static implicit operator int(ContactNumberTypeId id) => id.Value;
    
    // Explicit conversion from int
    public static explicit operator ContactNumberTypeId(int value) => new(value);
    
    // Equality and comparison
    public bool Equals(ContactNumberTypeId other) => Value.Equals(other.Value);
    public override bool Equals(object? obj) => obj is ContactNumberTypeId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
    
    // Operators
    public static bool operator ==(ContactNumberTypeId left, ContactNumberTypeId right) => left.Equals(right);
    public static bool operator !=(ContactNumberTypeId left, ContactNumberTypeId right) => !left.Equals(right);
} 