using BuildingBlocks.Application.CQRS.Commands;
using ContactService.Application.DTOs.AddressType;

namespace ContactService.Application.Commands.AddressType.CreateAddressType;

public sealed record CreateAddressTypeCommand : CommandBase<AddressTypeDto>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
} 