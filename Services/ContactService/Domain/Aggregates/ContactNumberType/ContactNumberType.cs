using BuildingBlocks.Domain.Entities;
using ContactService.Domain.StronglyTypedIds;
using ContactService.Domain.ValueObjects;
using ContactService.Domain.DomainEvents;

namespace ContactService.Domain.Aggregates.ContactNumberType;

public class ContactNumberType : AggregateRoot<ContactNumberTypeId, int>, IAuditableEntity
{
    // Private constructor for EF Core
    private ContactNumberType() { }
    
    // Factory method for creation
    public static ContactNumberType Create(ContactNumberTypeName name, string? description = null)
    {
        var contactNumberType = new ContactNumberType
        {
            Id = ContactNumberTypeId.New(),
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
        
        // Raise domain event
        contactNumberType.AddDomainEvent(new ContactNumberTypeCreatedDomainEvent(contactNumberType.Id, name));
        
        return contactNumberType;
    }
    
    // Properties
    public ContactNumberTypeName Name { get; private set; } = null!;
    public string? Description { get; private set; }
    
    // Audit properties (from IAuditableEntity)
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Domain methods
    public void UpdateName(ContactNumberTypeName newName)
    {
        if (Name.Value == newName.Value) return;
        
        var oldName = Name;
        Name = newName;
        
        AddDomainEvent(new ContactNumberTypeNameChangedDomainEvent(Id, oldName, newName));
    }
    
    public void UpdateDescription(string? newDescription)
    {
        if (Description == newDescription) return;
        
        Description = newDescription;
        AddDomainEvent(new ContactNumberTypeUpdatedDomainEvent(Id, Name));
    }
    
    public void Update(ContactNumberTypeName name, string? description = null)
    {
        UpdateName(name);
        UpdateDescription(description);
    }
    
    // Business methods
    public bool IsNamedAs(ContactNumberTypeName name) => Name.Value.Equals(name.Value, StringComparison.OrdinalIgnoreCase);
    public bool IsDefaultType() => IsNamedAs(ContactNumberTypeName.Mobile) || IsNamedAs(ContactNumberTypeName.Home);
    public bool IsWorkRelated() => IsNamedAs(ContactNumberTypeName.Work) || IsNamedAs(ContactNumberTypeName.Fax);
    public bool IsEmergencyType() => IsNamedAs(ContactNumberTypeName.Emergency);
} 