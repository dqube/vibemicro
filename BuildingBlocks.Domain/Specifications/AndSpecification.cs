using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Specifications;

/// <summary>
/// Specification that combines two specifications with AND logic
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
internal class AndSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    /// <summary>
    /// Initializes a new instance of the AndSpecification class
    /// </summary>
    /// <param name="left">The left specification</param>
    /// <param name="right">The right specification</param>
    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
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
            Expression.AndAlso(left!, right!), parameter);
    }

    /// <summary>
    /// Checks if the specified entity satisfies the specification
    /// </summary>
    public override bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);
    }
}

/// <summary>
/// Expression visitor for replacing parameter expressions
/// </summary>
internal class ReplaceExpressionVisitor : ExpressionVisitor
{
    private readonly Expression _oldValue;
    private readonly Expression _newValue;

    public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override Expression? Visit(Expression? node)
    {
        if (node == _oldValue)
            return _newValue;
        return base.Visit(node);
    }
} 