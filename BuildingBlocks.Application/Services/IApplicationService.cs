namespace BuildingBlocks.Application.Services;

/// <summary>
/// Marker interface for application services
/// </summary>
public interface IApplicationService
{
}

/// <summary>
/// Base interface for application services with context
/// </summary>
public interface IApplicationService<in TContext> : IApplicationService
    where TContext : class
{
    /// <summary>
    /// Sets the service context
    /// </summary>
    /// <param name="context">The service context</param>
    void SetContext(TContext context);
} 