using Microsoft.Extensions.DependencyInjection;
using ContactService.Domain.Specifications;

namespace ContactService.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContactDomain(this IServiceCollection services)
    {
        // Register domain specifications
        services.AddTransient<ContactNumberTypeSpecifications.AllContactNumberTypesSpecification>();
        services.AddTransient<ContactNumberTypeSpecifications.DefaultContactNumberTypesSpecification>();
        services.AddTransient<ContactNumberTypeSpecifications.WorkRelatedContactNumberTypesSpecification>();
        services.AddTransient<ContactNumberTypeSpecifications.EmergencyContactNumberTypesSpecification>();
        
        services.AddTransient<AddressTypeSpecifications.AllAddressTypesSpecification>();
        services.AddTransient<AddressTypeSpecifications.ResidentialAddressTypesSpecification>();
        services.AddTransient<AddressTypeSpecifications.BusinessAddressTypesSpecification>();
        services.AddTransient<AddressTypeSpecifications.ShippingAddressTypesSpecification>();
        services.AddTransient<AddressTypeSpecifications.BillingAddressTypesSpecification>();
        
        return services;
    }
} 