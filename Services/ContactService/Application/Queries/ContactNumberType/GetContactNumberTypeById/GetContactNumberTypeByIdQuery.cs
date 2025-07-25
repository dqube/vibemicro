using BuildingBlocks.Application.CQRS.Queries;
using ContactService.Application.DTOs.ContactNumberType;
using ContactService.Domain.StronglyTypedIds;

namespace ContactService.Application.Queries.ContactNumberType.GetContactNumberTypeById;

public sealed record GetContactNumberTypeByIdQuery : QueryBase<ContactNumberTypeDto?>
{
    public ContactNumberTypeId Id { get; init; }
    
    public GetContactNumberTypeByIdQuery(ContactNumberTypeId id)
    {
        Id = id;
    }
} 