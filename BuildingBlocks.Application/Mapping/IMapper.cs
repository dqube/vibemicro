namespace BuildingBlocks.Application.Mapping;

/// <summary>
/// Interface for object mapping
/// </summary>
public interface IMapper
{
    /// <summary>
    /// Maps an object to another type
    /// </summary>
    /// <typeparam name="TDestination">The destination type</typeparam>
    /// <param name="source">The source object</param>
    /// <returns>The mapped object</returns>
    TDestination Map<TDestination>(object source);

    /// <summary>
    /// Maps an object to an existing destination object
    /// </summary>
    /// <typeparam name="TSource">The source type</typeparam>
    /// <typeparam name="TDestination">The destination type</typeparam>
    /// <param name="source">The source object</param>
    /// <param name="destination">The destination object</param>
    /// <returns>The mapped destination object</returns>
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

    /// <summary>
    /// Maps a collection of objects
    /// </summary>
    /// <typeparam name="TDestination">The destination type</typeparam>
    /// <param name="sources">The source objects</param>
    /// <returns>The mapped objects</returns>
    IEnumerable<TDestination> Map<TDestination>(IEnumerable<object> sources);

    /// <summary>
    /// Maps a collection of objects with specific source type
    /// </summary>
    /// <typeparam name="TSource">The source type</typeparam>
    /// <typeparam name="TDestination">The destination type</typeparam>
    /// <param name="sources">The source objects</param>
    /// <returns>The mapped objects</returns>
    IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> sources);
}

/// <summary>
/// Interface for mapping profiles
/// </summary>
public interface IMappingProfile
{
    /// <summary>
    /// Configures the mapping
    /// </summary>
    /// <param name="configuration">The mapping configuration</param>
    void Configure(IMappingConfiguration configuration);
}

/// <summary>
/// Interface for mapping configuration
/// </summary>
public interface IMappingConfiguration
{
    /// <summary>
    /// Creates a mapping from source to destination
    /// </summary>
    /// <typeparam name="TSource">The source type</typeparam>
    /// <typeparam name="TDestination">The destination type</typeparam>
    /// <returns>The mapping expression</returns>
    IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>();
}

/// <summary>
/// Interface for mapping expressions
/// </summary>
/// <typeparam name="TSource">The source type</typeparam>
/// <typeparam name="TDestination">The destination type</typeparam>
public interface IMappingExpression<TSource, TDestination>
{
    /// <summary>
    /// Configures a member mapping
    /// </summary>
    /// <typeparam name="TMember">The member type</typeparam>
    /// <param name="destinationMember">The destination member expression</param>
    /// <param name="sourceMember">The source member expression</param>
    /// <returns>The mapping expression</returns>
    IMappingExpression<TSource, TDestination> ForMember<TMember>(
        System.Linq.Expressions.Expression<Func<TDestination, TMember>> destinationMember,
        System.Linq.Expressions.Expression<Func<TSource, TMember>> sourceMember);

    /// <summary>
    /// Ignores a destination member
    /// </summary>
    /// <typeparam name="TMember">The member type</typeparam>
    /// <param name="destinationMember">The destination member expression</param>
    /// <returns>The mapping expression</returns>
    IMappingExpression<TSource, TDestination> Ignore<TMember>(
        System.Linq.Expressions.Expression<Func<TDestination, TMember>> destinationMember);
} 