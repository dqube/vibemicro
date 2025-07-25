using BuildingBlocks.Application.CQRS.Commands;
using ContactService.Domain.StronglyTypedIds;

namespace ContactService.Application.Commands.ContactNumberType.DeleteContactNumberType;

public sealed record DeleteContactNumberTypeCommand : CommandBase
{
    public ContactNumberTypeId Id { get; init; }
    
    public DeleteContactNumberTypeCommand(ContactNumberTypeId id)
    {
        Id = id;
    }
} 