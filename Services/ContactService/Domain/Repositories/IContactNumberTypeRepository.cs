using BuildingBlocks.Domain.Repository;
using ContactService.Domain.Aggregates.ContactNumberType;
using ContactService.Domain.StronglyTypedIds;
using ContactService.Domain.ValueObjects;

namespace ContactService.Domain.Repositories;

public interface IContactNumberTypeRepository : IRepository<ContactNumberType, ContactNumberTypeId, int>
{
    Task<ContactNumberType?> GetByNameAsync(ContactNumberTypeName name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(ContactNumberTypeName name, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContactNumberType>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<ContactNumberType?> GetDefaultMobileTypeAsync(CancellationToken cancellationToken = default);
    Task<ContactNumberType?> GetDefaultHomeTypeAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ContactNumberType>> GetWorkRelatedTypesAsync(CancellationToken cancellationToken = default);
} 