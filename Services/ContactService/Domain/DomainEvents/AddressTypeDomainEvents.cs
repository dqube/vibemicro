using BuildingBlocks.Domain.DomainEvents;
using ContactService.Domain.StronglyTypedIds;
using ContactService.Domain.ValueObjects;

namespace ContactService.Domain.DomainEvents;

public static class AddressTypeDomainEvents
{
    public sealed record AddressTypeCreatedDomainEvent(
        AddressTypeId AddressTypeId,
        AddressTypeName Name
    ) : DomainEventBase(nameof(AddressTypeCreatedDomainEvent));
    
    public sealed record AddressTypeNameChangedDomainEvent(
        AddressTypeId AddressTypeId,
        AddressTypeName OldName,
        AddressTypeName NewName
    ) : DomainEventBase(nameof(AddressTypeNameChangedDomainEvent));
    
    public sealed record AddressTypeUpdatedDomainEvent(
        AddressTypeId AddressTypeId,
        AddressTypeName Name
    ) : DomainEventBase(nameof(AddressTypeUpdatedDomainEvent));
    
    public sealed record AddressTypeDeletedDomainEvent(
        AddressTypeId AddressTypeId,
        AddressTypeName Name
    ) : DomainEventBase(nameof(AddressTypeDeletedDomainEvent));
} 