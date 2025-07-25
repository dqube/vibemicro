using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.Caching;
using ContactService.Application.DTOs.ContactNumberType;
using ContactService.Application.Caching;
using ContactService.Domain.Repositories;
using ContactService.Domain.Specifications;
using Microsoft.Extensions.Logging;

namespace ContactService.Application.Queries.ContactNumberType.GetAllContactNumberTypes;

public sealed class GetAllContactNumberTypesQueryHandler : IQueryHandler<GetAllContactNumberTypesQuery, IEnumerable<ContactNumberTypeDto>>
{
    private readonly IContactNumberTypeRepository _contactNumberTypeRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetAllContactNumberTypesQueryHandler> _logger;

    public GetAllContactNumberTypesQueryHandler(
        IContactNumberTypeRepository contactNumberTypeRepository,
        ICacheService cacheService,
        ILogger<GetAllContactNumberTypesQueryHandler> logger)
    {
        _contactNumberTypeRepository = contactNumberTypeRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<IEnumerable<ContactNumberTypeDto>> HandleAsync(GetAllContactNumberTypesQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = ContactCacheKeys.ContactNumberTypesList();
        
        // Try cache first (only for active types)
        if (!query.IncludeInactive)
        {
            var cachedTypes = await _cacheService.GetAsync<IEnumerable<ContactNumberTypeDto>>(cacheKey, cancellationToken);
            if (cachedTypes != null)
            {
                _logger.LogDebug("Contact number types found in cache");
                return cachedTypes;
            }
        }

        // Get from repository
        var specification = new ContactNumberTypeSpecifications.AllContactNumberTypesSpecification();
        var contactNumberTypes = await _contactNumberTypeRepository.GetAsync(specification, cancellationToken);

        // Manual mapping to DTOs
        var contactNumberTypeDtos = contactNumberTypes.Select(contactNumberType => new ContactNumberTypeDto
        {
            Id = contactNumberType.Id.Value,
            Name = contactNumberType.Name.Value,
            Description = contactNumberType.Description,
            CreatedAt = contactNumberType.CreatedAt,
            CreatedBy = contactNumberType.CreatedBy,
            UpdatedAt = contactNumberType.UpdatedAt,
            UpdatedBy = contactNumberType.UpdatedBy
        }).ToList();

        // Cache the result (only for active types)
        if (!query.IncludeInactive)
        {
            await _cacheService.SetAsync(cacheKey, contactNumberTypeDtos, TimeSpan.FromHours(1), cancellationToken);
        }

        _logger.LogDebug("Retrieved {Count} contact number types from database", contactNumberTypeDtos.Count);
        return contactNumberTypeDtos;
    }
} 