using BuildingBlocks.Domain.DomainEvents;
using ContactService.Domain.StronglyTypedIds;
using ContactService.Domain.ValueObjects;

namespace ContactService.Domain.DomainEvents;

public static class ContactNumberTypeDomainEvents
{
    public sealed record ContactNumberTypeCreatedDomainEvent(
        ContactNumberTypeId ContactNumberTypeId,
        ContactNumberTypeName Name
    ) : DomainEventBase(nameof(ContactNumberTypeCreatedDomainEvent));
    
    public sealed record ContactNumberTypeNameChangedDomainEvent(
        ContactNumberTypeId ContactNumberTypeId,
        ContactNumberTypeName OldName,
        ContactNumberTypeName NewName
    ) : DomainEventBase(nameof(ContactNumberTypeNameChangedDomainEvent));
    
    public sealed record ContactNumberTypeUpdatedDomainEvent(
        ContactNumberTypeId ContactNumberTypeId,
        ContactNumberTypeName Name
    ) : DomainEventBase(nameof(ContactNumberTypeUpdatedDomainEvent));
    
    public sealed record ContactNumberTypeDeletedDomainEvent(
        ContactNumberTypeId ContactNumberTypeId,
        ContactNumberTypeName Name
    ) : DomainEventBase(nameof(ContactNumberTypeDeletedDomainEvent));
} 