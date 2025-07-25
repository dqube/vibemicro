using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Specifications;

/// <summary>
/// Specification that combines two specifications with OR logic
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
internal class OrSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    /// <summary>
    /// Initializes a new instance of the OrSpecification class
    /// </summary>
    /// <param name="left">The left specification</param>
    /// <param name="right">The right specification</param>
    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <summary>
    /// Converts the specification to an expression
    /// </summary>
    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var parameter = Expression.Parameter(typeof(T), "x");
        var leftVisitor = new ReplaceExpressionVisitor(leftExpression.Parameters[0], parameter);
        var rightVisitor = new ReplaceExpressionVisitor(rightExpression.Parameters[0], parameter);

        var left = leftVisitor.Visit(leftExpression.Body);
        var right = rightVisitor.Visit(rightExpression.Body);

        return Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(left!, right!), parameter);
    }

    /// <summary>
    /// Checks if the specified entity satisfies the specification
    /// </summary>
    public override bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) || _right.IsSatisfiedBy(entity);
    }
} 