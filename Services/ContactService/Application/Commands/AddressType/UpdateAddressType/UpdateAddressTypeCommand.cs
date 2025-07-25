using BuildingBlocks.Application.CQRS.Commands;
using ContactService.Application.DTOs.AddressType;
using ContactService.Domain.StronglyTypedIds;

namespace ContactService.Application.Commands.AddressType.UpdateAddressType;

public sealed record UpdateAddressTypeCommand : CommandBase<AddressTypeDto>
{
    public AddressTypeId Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
} 