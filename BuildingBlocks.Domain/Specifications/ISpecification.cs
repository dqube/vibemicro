using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Specifications;

/// <summary>
/// Interface for specifications
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Converts the specification to an expression
    /// </summary>
    /// <returns>The expression representing the specification</returns>
    Expression<Func<T, bool>> ToExpression();

    /// <summary>
    /// Checks if the specified entity satisfies the specification
    /// </summary>
    /// <param name="entity">The entity to check</param>
    /// <returns>True if the entity satisfies the specification</returns>
    bool IsSatisfiedBy(T entity);

    /// <summary>
    /// Combines this specification with another using AND logic
    /// </summary>
    /// <param name="specification">The specification to combine with</param>
    /// <returns>A new combined specification</returns>
    ISpecification<T> And(ISpecification<T> specification);

    /// <summary>
    /// Combines this specification with another using OR logic
    /// </summary>
    /// <param name="specification">The specification to combine with</param>
    /// <returns>A new combined specification</returns>
    ISpecification<T> Or(ISpecification<T> specification);

    /// <summary>
    /// Negates this specification
    /// </summary>
    /// <returns>A new negated specification</returns>
    ISpecification<T> Not();
} 