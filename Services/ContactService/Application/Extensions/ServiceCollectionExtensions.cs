using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using BuildingBlocks.Application.Extensions;
using ContactService.Application.Services;
using ContactService.Application.Events.DomainEventHandlers;
using ContactService.Application.Commands.ContactNumberType.CreateContactNumberType;
using ContactService.Application.Commands.ContactNumberType.UpdateContactNumberType;
using ContactService.Application.Commands.ContactNumberType.DeleteContactNumberType;
using ContactService.Application.Commands.AddressType.CreateAddressType;
using ContactService.Application.Commands.AddressType.UpdateAddressType;
using ContactService.Application.Commands.AddressType.DeleteAddressType;
using ContactService.Application.Queries.ContactNumberType.GetContactNumberTypeById;
using ContactService.Application.Queries.ContactNumberType.GetAllContactNumberTypes;
using ContactService.Domain.DomainEvents;
using BuildingBlocks.Application.CQRS.Events;
using FluentValidation;

namespace ContactService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContactApplication(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        // Add BuildingBlocks Application
        var allAssemblies = new[] { typeof(ServiceCollectionExtensions).Assembly }.Concat(assemblies).ToArray();
        services.AddApplication(allAssemblies);

        // Register application services
        services.AddScoped<IContactApplicationService, ContactApplicationService>();

        // Register validators
        services.AddValidatorsFromAssemblyContaining<CreateContactNumberTypeCommandValidator>();

        // Register event handlers
        services.AddScoped<ContactNumberTypeDomainEventHandlers>();

        // Register domain event handler mappings for ContactNumberType
        services.AddScoped<IDomainEventHandler<ContactNumberTypeDomainEvents.ContactNumberTypeCreatedDomainEvent>>(
            provider => provider.GetRequiredService<ContactNumberTypeDomainEventHandlers>());
        services.AddScoped<IDomainEventHandler<ContactNumberTypeDomainEvents.ContactNumberTypeNameChangedDomainEvent>>(
            provider => provider.GetRequiredService<ContactNumberTypeDomainEventHandlers>());
        services.AddScoped<IDomainEventHandler<ContactNumberTypeDomainEvents.ContactNumberTypeUpdatedDomainEvent>>(
            provider => provider.GetRequiredService<ContactNumberTypeDomainEventHandlers>());
        services.AddScoped<IDomainEventHandler<ContactNumberTypeDomainEvents.ContactNumberTypeDeletedDomainEvent>>(
            provider => provider.GetRequiredService<ContactNumberTypeDomainEventHandlers>());

        // Register command handlers
        services.AddScoped<CreateContactNumberTypeCommandHandler>();
        services.AddScoped<UpdateContactNumberTypeCommandHandler>();
        services.AddScoped<CreateAddressTypeCommandHandler>();
        services.AddScoped<UpdateAddressTypeCommandHandler>();

        // Register query handlers
        services.AddScoped<GetContactNumberTypeByIdQueryHandler>();
        services.AddScoped<GetAllContactNumberTypesQueryHandler>();

        return services;
    }
} 