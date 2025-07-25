using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.Caching;
using ContactService.Application.DTOs.ContactNumberType;
using ContactService.Application.Caching;
using ContactService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ContactService.Application.Queries.ContactNumberType.GetContactNumberTypeById;

public sealed class GetContactNumberTypeByIdQueryHandler : IQueryHandler<GetContactNumberTypeByIdQuery, ContactNumberTypeDto?>
{
    private readonly IContactNumberTypeRepository _contactNumberTypeRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetContactNumberTypeByIdQueryHandler> _logger;

    public GetContactNumberTypeByIdQueryHandler(
        IContactNumberTypeRepository contactNumberTypeRepository,
        ICacheService cacheService,
        ILogger<GetContactNumberTypeByIdQueryHandler> logger)
    {
        _contactNumberTypeRepository = contactNumberTypeRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ContactNumberTypeDto?> HandleAsync(GetContactNumberTypeByIdQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = ContactCacheKeys.ContactNumberTypeById(query.Id);
        
        // Try cache first
        var cachedContactNumberType = await _cacheService.GetAsync<ContactNumberTypeDto>(cacheKey, cancellationToken);
        if (cachedContactNumberType != null)
        {
            _logger.LogDebug("Contact number type found in cache: {Id}", query.Id);
            return cachedContactNumberType;
        }

        // Get from repository
        var contactNumberType = await _contactNumberTypeRepository.GetByIdAsync(query.Id, cancellationToken);
        if (contactNumberType == null)
        {
            _logger.LogWarning("Contact number type not found: {Id}", query.Id);
            return null;
        }

        // Manual mapping to DTO
        var contactNumberTypeDto = new ContactNumberTypeDto
        {
            Id = contactNumberType.Id.Value,
            Name = contactNumberType.Name.Value,
            Description = contactNumberType.Description,
            CreatedAt = contactNumberType.CreatedAt,
            CreatedBy = contactNumberType.CreatedBy,
            UpdatedAt = contactNumberType.UpdatedAt,
            UpdatedBy = contactNumberType.UpdatedBy
        };

        // Cache the result
        await _cacheService.SetAsync(cacheKey, contactNumberTypeDto, TimeSpan.FromMinutes(30), cancellationToken);

        _logger.LogDebug("Contact number type retrieved from database: {Id}", query.Id);
        return contactNumberTypeDto;
    }
} 