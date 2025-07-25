using BuildingBlocks.Domain.Entities;
using ContactService.Domain.StronglyTypedIds;
using ContactService.Domain.ValueObjects;
using ContactService.Domain.DomainEvents;

namespace ContactService.Domain.Aggregates.AddressType;

public class AddressType : AggregateRoot<AddressTypeId, int>, IAuditableEntity
{
    // Private constructor for EF Core
    private AddressType() { }
    
    // Factory method for creation
    public static AddressType Create(AddressTypeName name, string? description = null)
    {
        var addressType = new AddressType
        {
            Id = AddressTypeId.New(),
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
        
        // Raise domain event
        addressType.AddDomainEvent(new AddressTypeCreatedDomainEvent(addressType.Id, name));
        
        return addressType;
    }
    
    // Properties
    public AddressTypeName Name { get; private set; } = null!;
    public string? Description { get; private set; }
    
    // Audit properties (from IAuditableEntity)
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Domain methods
    public void UpdateName(AddressTypeName newName)
    {
        if (Name.Value == newName.Value) return;
        
        var oldName = Name;
        Name = newName;
        
        AddDomainEvent(new AddressTypeNameChangedDomainEvent(Id, oldName, newName));
    }
    
    public void UpdateDescription(string? newDescription)
    {
        if (Description == newDescription) return;
        
        Description = newDescription;
        AddDomainEvent(new AddressTypeUpdatedDomainEvent(Id, Name));
    }
    
    public void Update(AddressTypeName name, string? description = null)
    {
        UpdateName(name);
        UpdateDescription(description);
    }
    
    // Business methods
    public bool IsNamedAs(AddressTypeName name) => Name.Value.Equals(name.Value, StringComparison.OrdinalIgnoreCase);
    public bool IsResidentialType() => IsNamedAs(AddressTypeName.Home);
    public bool IsBusinessType() => IsNamedAs(AddressTypeName.Work) || IsNamedAs(AddressTypeName.Headquarters) || IsNamedAs(AddressTypeName.Warehouse);
    public bool IsShippingType() => IsNamedAs(AddressTypeName.Shipping) || IsNamedAs(AddressTypeName.Home);
    public bool IsBillingType() => IsNamedAs(AddressTypeName.Billing) || IsNamedAs(AddressTypeName.Work);
} 