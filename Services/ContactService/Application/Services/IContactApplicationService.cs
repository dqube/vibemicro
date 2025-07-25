using BuildingBlocks.Application.Services;
using ContactService.Application.DTOs.ContactNumberType;
using ContactService.Application.DTOs.AddressType;
using ContactService.Application.DTOs.Common;
using ContactService.Domain.StronglyTypedIds;

namespace ContactService.Application.Services;

public interface IContactApplicationService : IApplicationService
{
    // Contact Number Type operations
    Task<ContactNumberTypeDto> CreateContactNumberTypeAsync(string name, string? description = null, CancellationToken cancellationToken = default);
    Task<ContactNumberTypeDto> UpdateContactNumberTypeAsync(ContactNumberTypeId id, string name, string? description = null, CancellationToken cancellationToken = default);
    Task DeleteContactNumberTypeAsync(ContactNumberTypeId id, CancellationToken cancellationToken = default);
    Task<ContactNumberTypeDto?> GetContactNumberTypeByIdAsync(ContactNumberTypeId id, CancellationToken cancellationToken = default);
    Task<ContactNumberTypeDto?> GetContactNumberTypeByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContactNumberTypeDto>> GetAllContactNumberTypesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContactNumberTypeDto>> GetDefaultContactNumberTypesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ContactNumberTypeDto>> GetWorkRelatedContactNumberTypesAsync(CancellationToken cancellationToken = default);

    // Address Type operations
    Task<AddressTypeDto> CreateAddressTypeAsync(string name, string? description = null, CancellationToken cancellationToken = default);
    Task<AddressTypeDto> UpdateAddressTypeAsync(AddressTypeId id, string name, string? description = null, CancellationToken cancellationToken = default);
    Task DeleteAddressTypeAsync(AddressTypeId id, CancellationToken cancellationToken = default);
    Task<AddressTypeDto?> GetAddressTypeByIdAsync(AddressTypeId id, CancellationToken cancellationToken = default);
    Task<AddressTypeDto?> GetAddressTypeByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<AddressTypeDto>> GetAllAddressTypesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<AddressTypeDto>> GetResidentialAddressTypesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AddressTypeDto>> GetBusinessAddressTypesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AddressTypeDto>> GetShippingAddressTypesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AddressTypeDto>> GetBillingAddressTypesAsync(CancellationToken cancellationToken = default);

    // Utility operations for shared value objects
    Task<PhoneNumberDto> ValidatePhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<AddressDto> ValidateAddressAsync(string line1, string city, string postalCode, string countryCode, string? line2 = null, string? state = null, CancellationToken cancellationToken = default);
} 