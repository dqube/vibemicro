namespace BuildingBlocks.Domain.Specifications;

/// <summary>
/// Evaluator for applying specifications to queryable collections
/// </summary>
public static class SpecificationEvaluator
{
    /// <summary>
    /// Applies a specification to a queryable collection
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="specification">The specification to apply</param>
    /// <returns>The filtered queryable collection</returns>
    public static IQueryable<T> ApplySpecification<T>(this IQueryable<T> query, ISpecification<T> specification)
    {
        if (specification == null)
            return query;

        return query.Where(specification.ToExpression());
    }

    /// <summary>
    /// Applies multiple specifications to a queryable collection using AND logic
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="specifications">The specifications to apply</param>
    /// <returns>The filtered queryable collection</returns>
    public static IQueryable<T> ApplySpecifications<T>(this IQueryable<T> query, params ISpecification<T>[] specifications)
    {
        if (specifications == null || specifications.Length == 0)
            return query;

        var combinedSpecification = specifications.Aggregate((spec1, spec2) => spec1.And(spec2));
        return query.ApplySpecification(combinedSpecification);
    }

    /// <summary>
    /// Applies multiple specifications to a queryable collection using AND logic
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="specifications">The specifications to apply</param>
    /// <returns>The filtered queryable collection</returns>
    public static IQueryable<T> ApplySpecifications<T>(this IQueryable<T> query, IEnumerable<ISpecification<T>> specifications)
    {
        if (specifications == null)
            return query;

        var specList = specifications.ToList();
        if (specList.Count == 0)
            return query;

        var combinedSpecification = specList.Aggregate((spec1, spec2) => spec1.And(spec2));
        return query.ApplySpecification(combinedSpecification);
    }

    /// <summary>
    /// Checks if any item in the queryable collection satisfies the specification
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="specification">The specification to check</param>
    /// <returns>True if any item satisfies the specification</returns>
    public static bool Any<T>(this IQueryable<T> query, ISpecification<T> specification)
    {
        if (specification == null)
            return query.Any();

        return query.Any(specification.ToExpression());
    }

    /// <summary>
    /// Checks if any item in the queryable collection satisfies the specification asynchronously
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="specification">The specification to check</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if any item satisfies the specification</returns>
    public static async Task<bool> AnyAsync<T>(this IQueryable<T> query, ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
            return await Task.FromResult(query.Any());

        return await Task.FromResult(query.Any(specification.ToExpression()));
    }

    /// <summary>
    /// Counts items in the queryable collection that satisfy the specification
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="specification">The specification to check</param>
    /// <returns>The number of items that satisfy the specification</returns>
    public static int Count<T>(this IQueryable<T> query, ISpecification<T> specification)
    {
        if (specification == null)
            return query.Count();

        return query.Count(specification.ToExpression());
    }

    /// <summary>
    /// Counts items in the queryable collection that satisfy the specification asynchronously
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="specification">The specification to check</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of items that satisfy the specification</returns>
    public static async Task<int> CountAsync<T>(this IQueryable<T> query, ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
            return await Task.FromResult(query.Count());

        return await Task.FromResult(query.Count(specification.ToExpression()));
    }

    /// <summary>
    /// Gets the first item that satisfies the specification
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="specification">The specification to check</param>
    /// <returns>The first item that satisfies the specification</returns>
    public static T? FirstOrDefault<T>(this IQueryable<T> query, ISpecification<T> specification)
    {
        if (specification == null)
            return query.FirstOrDefault();

        return query.FirstOrDefault(specification.ToExpression());
    }

    /// <summary>
    /// Gets the first item that satisfies the specification asynchronously
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="specification">The specification to check</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The first item that satisfies the specification</returns>
    public static async Task<T?> FirstOrDefaultAsync<T>(this IQueryable<T> query, ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        if (specification == null)
            return await Task.FromResult(query.FirstOrDefault());

        return await Task.FromResult(query.FirstOrDefault(specification.ToExpression()));
    }
} 