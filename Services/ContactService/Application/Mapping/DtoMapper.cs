using ContactService.Application.DTOs.ContactNumberType;
using ContactService.Application.DTOs.AddressType;
using ContactService.Application.DTOs.Common;
using ContactService.Domain.Aggregates.ContactNumberType;
using ContactService.Domain.Aggregates.AddressType;
using ContactService.Domain.ValueObjects;

namespace ContactService.Application.Mapping;

/// <summary>
/// Simple manual mapping helper for converting domain entities to DTOs
/// </summary>
public static class DtoMapper
{
    // ContactNumberType mappings
    public static ContactNumberTypeDto ToDto(this ContactNumberType contactNumberType)
    {
        return new ContactNumberTypeDto
        {
            Id = contactNumberType.Id.Value,
            Name = contactNumberType.Name.Value,
            Description = contactNumberType.Description,
            CreatedAt = contactNumberType.CreatedAt,
            CreatedBy = contactNumberType.CreatedBy,
            UpdatedAt = contactNumberType.UpdatedAt,
            UpdatedBy = contactNumberType.UpdatedBy
        };
    }

    public static IEnumerable<ContactNumberTypeDto> ToDto(this IEnumerable<ContactNumberType> contactNumberTypes)
    {
        return contactNumberTypes.Select(ToDto);
    }

    // AddressType mappings
    public static AddressTypeDto ToDto(this AddressType addressType)
    {
        return new AddressTypeDto
        {
            Id = addressType.Id.Value,
            Name = addressType.Name.Value,
            Description = addressType.Description,
            CreatedAt = addressType.CreatedAt,
            CreatedBy = addressType.CreatedBy,
            UpdatedAt = addressType.UpdatedAt,
            UpdatedBy = addressType.UpdatedBy
        };
    }

    public static IEnumerable<AddressTypeDto> ToDto(this IEnumerable<AddressType> addressTypes)
    {
        return addressTypes.Select(ToDto);
    }

    // Value object mappings
    public static PhoneNumberDto ToDto(this PhoneNumber phoneNumber)
    {
        return new PhoneNumberDto
        {
            Value = phoneNumber.Value,
            DigitsOnly = phoneNumber.GetDigitsOnly(),
            IsInternational = phoneNumber.IsInternational(),
            CountryCode = phoneNumber.GetCountryCode()
        };
    }

    public static AddressDto ToDto(this Address address)
    {
        return new AddressDto
        {
            Line1 = address.Line1,
            Line2 = address.Line2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            CountryCode = address.CountryCode,
            FullAddress = address.GetFullAddress(),
            ShortAddress = address.GetShortAddress(),
            IsComplete = address.IsComplete()
        };
    }
} 