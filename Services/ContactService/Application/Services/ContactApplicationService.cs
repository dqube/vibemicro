using BuildingBlocks.Application.Services;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.Application.Security;
using ContactService.Application.Commands.ContactNumberType.CreateContactNumberType;
using ContactService.Application.Commands.ContactNumberType.UpdateContactNumberType;
using ContactService.Application.Commands.ContactNumberType.DeleteContactNumberType;
using ContactService.Application.Commands.AddressType.CreateAddressType;
using ContactService.Application.Commands.AddressType.UpdateAddressType;
using ContactService.Application.Commands.AddressType.DeleteAddressType;
using ContactService.Application.Queries.ContactNumberType.GetContactNumberTypeById;
using ContactService.Application.Queries.ContactNumberType.GetAllContactNumberTypes;
using ContactService.Application.DTOs.ContactNumberType;
using ContactService.Application.DTOs.AddressType;
using ContactService.Application.DTOs.Common;
using ContactService.Application.Mapping;
using ContactService.Domain.StronglyTypedIds;
using ContactService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ContactService.Application.Services;

public sealed class ContactApplicationService : ApplicationServiceBase, IContactApplicationService
{
    private readonly IMediator _mediator;

    public ContactApplicationService(
        IMediator mediator,
        ILogger<ContactApplicationService> logger,
        ICurrentUser currentUser)
        : base(mediator, logger, currentUser)
    {
        _mediator = mediator;
    }

    // Contact Number Type operations
    public async Task<ContactNumberTypeDto> CreateContactNumberTypeAsync(string name, string? description = null, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Creating contact number type: {Name}", name);

        var command = new CreateContactNumberTypeCommand
        {
            Name = name,
            Description = description
        };

        return await _mediator.SendAsync<CreateContactNumberTypeCommand, ContactNumberTypeDto>(command, cancellationToken);
    }

    public async Task<ContactNumberTypeDto> UpdateContactNumberTypeAsync(ContactNumberTypeId id, string name, string? description = null, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Updating contact number type: {Id}", id);

        var command = new UpdateContactNumberTypeCommand
        {
            Id = id,
            Name = name,
            Description = description
        };

        return await _mediator.SendAsync<UpdateContactNumberTypeCommand, ContactNumberTypeDto>(command, cancellationToken);
    }

    public async Task DeleteContactNumberTypeAsync(ContactNumberTypeId id, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Deleting contact number type: {Id}", id);

        var command = new DeleteContactNumberTypeCommand(id);
        await _mediator.SendAsync(command, cancellationToken);
    }

    public async Task<ContactNumberTypeDto?> GetContactNumberTypeByIdAsync(ContactNumberTypeId id, CancellationToken cancellationToken = default)
    {
        var query = new GetContactNumberTypeByIdQuery(id);
        return await _mediator.QueryAsync<GetContactNumberTypeByIdQuery, ContactNumberTypeDto?>(query, cancellationToken);
    }

    public async Task<ContactNumberTypeDto?> GetContactNumberTypeByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        // This would need a specific query - for now, get all and filter
        var allTypes = await GetAllContactNumberTypesAsync(false, cancellationToken);
        return allTypes.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<ContactNumberTypeDto>> GetAllContactNumberTypesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = new GetAllContactNumberTypesQuery { IncludeInactive = includeInactive };
        return await _mediator.QueryAsync<GetAllContactNumberTypesQuery, IEnumerable<ContactNumberTypeDto>>(query, cancellationToken);
    }

    public async Task<IEnumerable<ContactNumberTypeDto>> GetDefaultContactNumberTypesAsync(CancellationToken cancellationToken = default)
    {
        var allTypes = await GetAllContactNumberTypesAsync(false, cancellationToken);
        return allTypes.Where(t => t.Name == "Mobile" || t.Name == "Home");
    }

    public async Task<IEnumerable<ContactNumberTypeDto>> GetWorkRelatedContactNumberTypesAsync(CancellationToken cancellationToken = default)
    {
        var allTypes = await GetAllContactNumberTypesAsync(false, cancellationToken);
        return allTypes.Where(t => t.Name == "Work" || t.Name == "Fax");
    }

    // Address Type operations
    public async Task<AddressTypeDto> CreateAddressTypeAsync(string name, string? description = null, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Creating address type: {Name}", name);

        var command = new CreateAddressTypeCommand
        {
            Name = name,
            Description = description
        };

        return await _mediator.SendAsync<CreateAddressTypeCommand, AddressTypeDto>(command, cancellationToken);
    }

    public async Task<AddressTypeDto> UpdateAddressTypeAsync(AddressTypeId id, string name, string? description = null, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Updating address type: {Id}", id);

        var command = new UpdateAddressTypeCommand
        {
            Id = id,
            Name = name,
            Description = description
        };

        return await _mediator.SendAsync<UpdateAddressTypeCommand, AddressTypeDto>(command, cancellationToken);
    }

    public async Task DeleteAddressTypeAsync(AddressTypeId id, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Deleting address type: {Id}", id);

        var command = new DeleteAddressTypeCommand(id);
        await _mediator.SendAsync(command, cancellationToken);
    }

    public async Task<AddressTypeDto?> GetAddressTypeByIdAsync(AddressTypeId id, CancellationToken cancellationToken = default)
    {
        // This would need specific queries - placeholder implementation
        return null;
    }

    public async Task<AddressTypeDto?> GetAddressTypeByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var allTypes = await GetAllAddressTypesAsync(false, cancellationToken);
        return allTypes.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<AddressTypeDto>> GetAllAddressTypesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        // This would need a specific query - placeholder implementation
        return Enumerable.Empty<AddressTypeDto>();
    }

    public async Task<IEnumerable<AddressTypeDto>> GetResidentialAddressTypesAsync(CancellationToken cancellationToken = default)
    {
        var allTypes = await GetAllAddressTypesAsync(false, cancellationToken);
        return allTypes.Where(t => t.Name == "Home");
    }

    public async Task<IEnumerable<AddressTypeDto>> GetBusinessAddressTypesAsync(CancellationToken cancellationToken = default)
    {
        var allTypes = await GetAllAddressTypesAsync(false, cancellationToken);
        return allTypes.Where(t => t.Name == "Work" || t.Name == "Headquarters" || t.Name == "Warehouse");
    }

    public async Task<IEnumerable<AddressTypeDto>> GetShippingAddressTypesAsync(CancellationToken cancellationToken = default)
    {
        var allTypes = await GetAllAddressTypesAsync(false, cancellationToken);
        return allTypes.Where(t => t.Name == "Shipping" || t.Name == "Home");
    }

    public async Task<IEnumerable<AddressTypeDto>> GetBillingAddressTypesAsync(CancellationToken cancellationToken = default)
    {
        var allTypes = await GetAllAddressTypesAsync(false, cancellationToken);
        return allTypes.Where(t => t.Name == "Billing" || t.Name == "Work");
    }

    // Utility operations for shared value objects
    public async Task<PhoneNumberDto> ValidatePhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var phone = new PhoneNumber(phoneNumber);
        return phone.ToDto();
    }

    public async Task<AddressDto> ValidateAddressAsync(string line1, string city, string postalCode, string countryCode, string? line2 = null, string? state = null, CancellationToken cancellationToken = default)
    {
        var address = new Address(line1, city, postalCode, countryCode, line2, state);
        return address.ToDto();
    }
} 