using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Specifications;

/// <summary>
/// Specification that negates another specification
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
internal class NotSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _specification;

    /// <summary>
    /// Initializes a new instance of the NotSpecification class
    /// </summary>
    /// <param name="specification">The specification to negate</param>
    public NotSpecification(ISpecification<T> specification)
    {
        _specification = specification ?? throw new ArgumentNullException(nameof(specification));
    }

    /// <summary>
    /// Converts the specification to an expression
    /// </summary>
    public override Expression<Func<T, bool>> ToExpression()
    {
        var expression = _specification.ToExpression();
        var parameter = expression.Parameters[0];
        var body = Expression.Not(expression.Body);

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    /// <summary>
    /// Checks if the specified entity satisfies the specification
    /// </summary>
    public override bool IsSatisfiedBy(T entity)
    {
        return !_specification.IsSatisfiedBy(entity);
    }
} 