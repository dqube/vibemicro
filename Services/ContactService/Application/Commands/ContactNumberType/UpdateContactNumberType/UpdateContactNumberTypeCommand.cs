using BuildingBlocks.Application.CQRS.Commands;
using ContactService.Application.DTOs.ContactNumberType;
using ContactService.Domain.StronglyTypedIds;

namespace ContactService.Application.Commands.ContactNumberType.UpdateContactNumberType;

public sealed record UpdateContactNumberTypeCommand : CommandBase<ContactNumberTypeDto>
{
    public ContactNumberTypeId Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
} 