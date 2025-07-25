using BuildingBlocks.Application.CQRS.Queries;
using ContactService.Application.DTOs.ContactNumberType;

namespace ContactService.Application.Queries.ContactNumberType.GetAllContactNumberTypes;

public sealed record GetAllContactNumberTypesQuery : QueryBase<IEnumerable<ContactNumberTypeDto>>
{
    public bool IncludeInactive { get; init; } = false;
} 