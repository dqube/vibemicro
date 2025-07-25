using BuildingBlocks.Application.CQRS.Commands;
using ContactService.Application.DTOs.ContactNumberType;

namespace ContactService.Application.Commands.ContactNumberType.CreateContactNumberType;

public sealed record CreateContactNumberTypeCommand : CommandBase<ContactNumberTypeDto>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
} 