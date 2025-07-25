using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Specifications;

/// <summary>
/// Base class for specifications
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    /// <summary>
    /// Converts the specification to an expression
    /// </summary>
    public abstract Expression<Func<T, bool>> ToExpression();

    /// <summary>
    /// Checks if the specified entity satisfies the specification
    /// </summary>
    public virtual bool IsSatisfiedBy(T entity)
    {
        var predicate = ToExpression().Compile();
        return predicate(entity);
    }

    /// <summary>
    /// Combines this specification with another using AND logic
    /// </summary>
    /// <param name="specification">The specification to combine with</param>
    /// <returns>A new combined specification</returns>
    public virtual ISpecification<T> And(ISpecification<T> specification)
    {
        return new AndSpecification<T>(this, specification);
    }

    /// <summary>
    /// Combines this specification with another using OR logic
    /// </summary>
    /// <param name="specification">The specification to combine with</param>
    /// <returns>A new combined specification</returns>
    public virtual ISpecification<T> Or(ISpecification<T> specification)
    {
        return new OrSpecification<T>(this, specification);
    }

    /// <summary>
    /// Negates this specification
    /// </summary>
    /// <returns>A new negated specification</returns>
    public virtual ISpecification<T> Not()
    {
        return new NotSpecification<T>(this);
    }

    /// <summary>
    /// Implicit conversion to expression
    /// </summary>
    public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
    {
        return specification.ToExpression();
    }

    /// <summary>
    /// AND operator overload
    /// </summary>
    public static Specification<T> operator &(Specification<T> left, Specification<T> right)
    {
        return new AndSpecification<T>(left, right);
    }

    /// <summary>
    /// OR operator overload
    /// </summary>
    public static Specification<T> operator |(Specification<T> left, Specification<T> right)
    {
        return new OrSpecification<T>(left, right);
    }

    /// <summary>
    /// NOT operator overload
    /// </summary>
    public static Specification<T> operator !(Specification<T> specification)
    {
        return new NotSpecification<T>(specification);
    }

    /// <summary>
    /// True specification (always satisfied)
    /// </summary>
    public static readonly Specification<T> True = new TrueSpecification<T>();

    /// <summary>
    /// False specification (never satisfied)
    /// </summary>
    public static readonly Specification<T> False = new FalseSpecification<T>();
}

/// <summary>
/// Specification that is always satisfied
/// </summary>
internal class TrueSpecification<T> : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return x => true;
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return true;
    }
}

/// <summary>
/// Specification that is never satisfied
/// </summary>
internal class FalseSpecification<T> : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return x => false;
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return false;
    }
} 