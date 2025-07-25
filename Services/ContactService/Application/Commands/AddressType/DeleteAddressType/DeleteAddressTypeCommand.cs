using BuildingBlocks.Application.CQRS.Commands;
using ContactService.Domain.StronglyTypedIds;

namespace ContactService.Application.Commands.AddressType.DeleteAddressType;

public sealed record DeleteAddressTypeCommand : CommandBase
{
    public AddressTypeId Id { get; init; }
    
    public DeleteAddressTypeCommand(AddressTypeId id)
    {
        Id = id;
    }
} 