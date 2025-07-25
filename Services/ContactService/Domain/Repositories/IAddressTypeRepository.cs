using BuildingBlocks.Domain.Repository;
using ContactService.Domain.Aggregates.AddressType;
using ContactService.Domain.StronglyTypedIds;
using ContactService.Domain.ValueObjects;

namespace ContactService.Domain.Repositories;

public interface IAddressTypeRepository : IRepository<AddressType, AddressTypeId, int>
{
    Task<AddressType?> GetByNameAsync(AddressTypeName name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(AddressTypeName name, CancellationToken cancellationToken = default);
    Task<IEnumerable<AddressType>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<AddressType?> GetDefaultHomeTypeAsync(CancellationToken cancellationToken = default);
    Task<AddressType?> GetDefaultShippingTypeAsync(CancellationToken cancellationToken = default);
    Task<AddressType?> GetDefaultBillingTypeAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AddressType>> GetBusinessTypesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AddressType>> GetShippingTypesAsync(CancellationToken cancellationToken = default);
} 