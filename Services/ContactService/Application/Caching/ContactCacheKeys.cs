using ContactService.Domain.StronglyTypedIds;

namespace ContactService.Application.Caching;

public static class ContactCacheKeys
{
    private const string PREFIX = "contact";
    private const string CONTACT_NUMBER_TYPE_PREFIX = $"{PREFIX}:contact-number-type";
    private const string ADDRESS_TYPE_PREFIX = $"{PREFIX}:address-type";

    // Contact Number Type cache keys
    public static string ContactNumberTypeById(ContactNumberTypeId id) => $"{CONTACT_NUMBER_TYPE_PREFIX}:id:{id.Value}";
    public static string ContactNumberTypeByName(string name) => $"{CONTACT_NUMBER_TYPE_PREFIX}:name:{name.ToLowerInvariant()}";
    public static string ContactNumberTypesList() => $"{CONTACT_NUMBER_TYPE_PREFIX}:list:all";
    public static string DefaultContactNumberTypes() => $"{CONTACT_NUMBER_TYPE_PREFIX}:list:default";
    public static string WorkRelatedContactNumberTypes() => $"{CONTACT_NUMBER_TYPE_PREFIX}:list:work-related";

    // Address Type cache keys
    public static string AddressTypeById(AddressTypeId id) => $"{ADDRESS_TYPE_PREFIX}:id:{id.Value}";
    public static string AddressTypeByName(string name) => $"{ADDRESS_TYPE_PREFIX}:name:{name.ToLowerInvariant()}";
    public static string AddressTypesList() => $"{ADDRESS_TYPE_PREFIX}:list:all";
    public static string ResidentialAddressTypes() => $"{ADDRESS_TYPE_PREFIX}:list:residential";
    public static string BusinessAddressTypes() => $"{ADDRESS_TYPE_PREFIX}:list:business";
    public static string ShippingAddressTypes() => $"{ADDRESS_TYPE_PREFIX}:list:shipping";
    public static string BillingAddressTypes() => $"{ADDRESS_TYPE_PREFIX}:list:billing";
} 