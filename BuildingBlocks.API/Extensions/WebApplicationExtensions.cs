using BuildingBlocks.API.Middleware.ErrorHandling;
using BuildingBlocks.API.Middleware.Logging;

namespace BuildingBlocks.API.Extensions;

/// <summary>
/// Extension methods for IApplicationBuilder to configure the API request pipeline
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures the API request pipeline
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <param name="environment">The host environment</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseApi(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        // Add security headers
        app.UseSecurityHeaders();

        // Add problem details middleware (should be early in pipeline)
        app.UseProblemDetailsMiddleware();

        // Add HTTPS redirection
        app.UseHttpsRedirection();

        // Add correlation ID middleware
        app.UseCorrelationId();

        // Add request logging
        app.UseRequestLogging();

        // Add routing
        app.UseRouting();

        // Add CORS
        app.UseCors();

        // Add rate limiting
        app.UseRateLimiter();

        // Add authentication
        app.UseAuthentication();

        // Add authorization  
        app.UseAuthorization();

        // Add endpoints
        app.UseEndpoints(endpoints =>
        {
            // Map health checks
            endpoints.MapHealthChecks("/health");
            
            // Map other endpoints will be added by consuming applications
            // using the extension methods from CrudEndpoints and QueryEndpoints
        });

        return app;
    }

    /// <summary>
    /// Configures development-specific middleware
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseDevelopmentApi(this IApplicationBuilder app)
    {
        // Add Swagger in development
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "BuildingBlocks API v1");
            options.RoutePrefix = "swagger";
        });

        // Add Scalar API documentation
        app.UseScalar(options =>
        {
            options.RoutePrefix = "scalar";
            options.OpenApiRoutePattern = "/swagger/{documentName}/swagger.json";
        });

        return app;
    }

    /// <summary>
    /// Adds security headers middleware
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            // Add security headers
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

            await next();
        });
    }

    /// <summary>
    /// Adds correlation ID middleware
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }

    /// <summary>
    /// Adds request logging middleware
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
} 