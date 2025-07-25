using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.Caching;
using ContactService.Application.Caching;
using ContactService.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace ContactService.Application.Events.DomainEventHandlers;

public sealed class ContactNumberTypeDomainEventHandlers :
    IDomainEventHandler<ContactNumberTypeDomainEvents.ContactNumberTypeCreatedDomainEvent>,
    IDomainEventHandler<ContactNumberTypeDomainEvents.ContactNumberTypeNameChangedDomainEvent>,
    IDomainEventHandler<ContactNumberTypeDomainEvents.ContactNumberTypeUpdatedDomainEvent>,
    IDomainEventHandler<ContactNumberTypeDomainEvents.ContactNumberTypeDeletedDomainEvent>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<ContactNumberTypeDomainEventHandlers> _logger;

    public ContactNumberTypeDomainEventHandlers(
        ICacheService cacheService,
        ILogger<ContactNumberTypeDomainEventHandlers> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task HandleAsync(ContactNumberTypeDomainEvents.ContactNumberTypeCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ContactNumberTypeCreatedDomainEvent for: {Id}", domainEvent.ContactNumberTypeId);

        // Invalidate list caches when a new type is created
        await InvalidateListCaches(cancellationToken);
    }

    public async Task HandleAsync(ContactNumberTypeDomainEvents.ContactNumberTypeNameChangedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ContactNumberTypeNameChangedDomainEvent for: {Id}", domainEvent.ContactNumberTypeId);

        // Invalidate specific type cache and lists
        await InvalidateTypeCaches(domainEvent.ContactNumberTypeId, domainEvent.OldName.Value, cancellationToken);
        await InvalidateListCaches(cancellationToken);
    }

    public async Task HandleAsync(ContactNumberTypeDomainEvents.ContactNumberTypeUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ContactNumberTypeUpdatedDomainEvent for: {Id}", domainEvent.ContactNumberTypeId);

        // Invalidate specific type cache and lists
        await InvalidateTypeCaches(domainEvent.ContactNumberTypeId, domainEvent.Name.Value, cancellationToken);
        await InvalidateListCaches(cancellationToken);
    }

    public async Task HandleAsync(ContactNumberTypeDomainEvents.ContactNumberTypeDeletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ContactNumberTypeDeletedDomainEvent for: {Id}", domainEvent.ContactNumberTypeId);

        // Invalidate specific type cache and lists
        await InvalidateTypeCaches(domainEvent.ContactNumberTypeId, domainEvent.Name.Value, cancellationToken);
        await InvalidateListCaches(cancellationToken);
    }

    private async Task InvalidateTypeCaches(Domain.StronglyTypedIds.ContactNumberTypeId typeId, string typeName, CancellationToken cancellationToken)
    {
        var keys = new[]
        {
            ContactCacheKeys.ContactNumberTypeById(typeId),
            ContactCacheKeys.ContactNumberTypeByName(typeName)
        };

        foreach (var key in keys)
        {
            await _cacheService.RemoveAsync(key, cancellationToken);
        }

        _logger.LogDebug("Invalidated contact number type caches for: {Id}", typeId);
    }

    private async Task InvalidateListCaches(CancellationToken cancellationToken)
    {
        var listKeys = new[]
        {
            ContactCacheKeys.ContactNumberTypesList(),
            ContactCacheKeys.DefaultContactNumberTypes(),
            ContactCacheKeys.WorkRelatedContactNumberTypes()
        };

        foreach (var key in listKeys)
        {
            await _cacheService.RemoveAsync(key, cancellationToken);
        }

        _logger.LogDebug("Invalidated contact number type list caches");
    }
} 