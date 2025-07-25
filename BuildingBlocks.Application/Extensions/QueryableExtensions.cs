using BuildingBlocks.Application.CQRS.Queries;
using System.Linq.Expressions;

namespace BuildingBlocks.Application.Extensions;

/// <summary>
/// Extension methods for IQueryable
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies paging to a queryable
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="queryable">The queryable</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>The paged queryable</returns>
    public static IQueryable<T> Page<T>(this IQueryable<T> queryable, int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));
        
        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        var skip = (pageNumber - 1) * pageSize;
        return queryable.Skip(skip).Take(pageSize);
    }

    /// <summary>
    /// Applies sorting to a queryable
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="queryable">The queryable</param>
    /// <param name="sortBy">The property to sort by</param>
    /// <param name="sortDirection">The sort direction</param>
    /// <returns>The sorted queryable</returns>
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string? sortBy, SortDirection sortDirection = SortDirection.Ascending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return queryable;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, sortBy);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = sortDirection == SortDirection.Ascending ? "OrderBy" : "OrderByDescending";
        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type);

        return (IQueryable<T>)method.Invoke(null, new object[] { queryable, lambda })!;
    }

    /// <summary>
    /// Applies conditional filtering to a queryable
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="queryable">The queryable</param>
    /// <param name="condition">The condition to check</param>
    /// <param name="predicate">The predicate to apply if condition is true</param>
    /// <returns>The filtered queryable</returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> queryable, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? queryable.Where(predicate) : queryable;
    }

    /// <summary>
    /// Creates a paged result from a queryable
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="queryable">The queryable</param>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The paged result</returns>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> queryable, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = queryable.Count();
        var items = await Task.FromResult(queryable.Page(pageNumber, pageSize).ToList());

        return PagedResult<T>.Create(items, totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Applies multiple sorting criteria to a queryable
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="queryable">The queryable</param>
    /// <param name="sortCriteria">The sort criteria</param>
    /// <returns>The sorted queryable</returns>
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, IEnumerable<SortCriteria> sortCriteria)
    {
        var criteriaList = sortCriteria?.ToList() ?? new List<SortCriteria>();
        
        if (!criteriaList.Any())
            return queryable;

        IOrderedQueryable<T>? orderedQueryable = null;

        foreach (var criteria in criteriaList)
        {
            if (string.IsNullOrWhiteSpace(criteria.PropertyName))
                continue;

            var parameter = Expression.Parameter(typeof(T), "x");
            
            try
            {
                var property = Expression.Property(parameter, criteria.PropertyName);
                var lambda = Expression.Lambda(property, parameter);

                if (orderedQueryable == null)
                {
                    // First sort
                    var methodName = criteria.Direction == SortDirection.Ascending ? "OrderBy" : "OrderByDescending";
                    var method = typeof(Queryable).GetMethods()
                        .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(T), property.Type);

                    orderedQueryable = (IOrderedQueryable<T>)method.Invoke(null, new object[] { queryable, lambda })!;
                }
                else
                {
                    // Subsequent sorts
                    var methodName = criteria.Direction == SortDirection.Ascending ? "ThenBy" : "ThenByDescending";
                    var method = typeof(Queryable).GetMethods()
                        .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(T), property.Type);

                    orderedQueryable = (IOrderedQueryable<T>)method.Invoke(null, new object[] { orderedQueryable, lambda })!;
                }
            }
            catch
            {
                // Ignore invalid property names
                continue;
            }
        }

        return orderedQueryable ?? queryable;
    }
}

/// <summary>
/// Represents sorting criteria
/// </summary>
public class SortCriteria
{
    /// <summary>
    /// Gets or sets the property name to sort by
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sort direction
    /// </summary>
    public SortDirection Direction { get; set; } = SortDirection.Ascending;

    /// <summary>
    /// Initializes a new instance of the SortCriteria class
    /// </summary>
    public SortCriteria()
    {
    }

    /// <summary>
    /// Initializes a new instance of the SortCriteria class
    /// </summary>
    /// <param name="propertyName">The property name</param>
    /// <param name="direction">The sort direction</param>
    public SortCriteria(string propertyName, SortDirection direction = SortDirection.Ascending)
    {
        PropertyName = propertyName;
        Direction = direction;
    }

    /// <summary>
    /// Creates ascending sort criteria
    /// </summary>
    /// <param name="propertyName">The property name</param>
    /// <returns>The sort criteria</returns>
    public static SortCriteria Ascending(string propertyName) => new(propertyName, SortDirection.Ascending);

    /// <summary>
    /// Creates descending sort criteria
    /// </summary>
    /// <param name="propertyName">The property name</param>
    /// <returns>The sort criteria</returns>
    public static SortCriteria Descending(string propertyName) => new(propertyName, SortDirection.Descending);
} 