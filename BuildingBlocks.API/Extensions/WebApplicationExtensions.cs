using BuildingBlocks.API.Configuration.Options;
using BuildingBlocks.API.Middleware.ErrorHandling;
using BuildingBlocks.API.Middleware.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.API.Extensions;

/// <summary>
/// Extension methods for WebApplication to configure the request pipeline
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures the API pipeline
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The web application</returns>
    public static WebApplication UseApi(this WebApplication app, IConfiguration? configuration = null)
    {
        configuration ??= app.Configuration;
        
        // Use exception handling
        app.UseExceptionHandling();
        
        // Use request logging
        app.UseRequestLogging();
        
        // Use HTTPS redirection
        app.UseHttpsRedirection();
        
        // Use CORS
        app.UseApiCors();
        
        // Use authentication and authorization
        app.UseAuthentication();
        app.UseAuthorization();
        
        // Use rate limiting
        app.UseApiRateLimiting();
        
        // Use OpenAPI/Swagger
        app.UseOpenApi();
        
        // Use health checks
        app.UseApiHealthChecks();

        return app;
    }

    /// <summary>
    /// Uses exception handling middleware
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
    public static WebApplication UseExceptionHandling(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();
        }

        return app;
    }

    /// <summary>
    /// Uses request logging middleware
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
    public static WebApplication UseRequestLogging(this WebApplication app)
    {
        var apiOptions = app.Services.GetService<IOptions<ApiOptions>>()?.Value ?? new ApiOptions();
        
        if (apiOptions.EnableRequestLogging)
        {
            app.UseMiddleware<RequestLoggingMiddleware>();
        }

        return app;
    }

    /// <summary>
    /// Uses CORS middleware
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
    public static WebApplication UseApiCors(this WebApplication app)
    {
        app.UseCors();
        return app;
    }

    /// <summary>
    /// Uses rate limiting middleware
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
    public static WebApplication UseApiRateLimiting(this WebApplication app)
    {
        var rateLimitOptions = app.Services.GetService<IOptions<RateLimitingOptions>>()?.Value ?? new RateLimitingOptions();
        
        if (rateLimitOptions.Enabled)
        {
            app.UseRateLimiter();
        }

        return app;
    }

    /// <summary>
    /// Uses OpenAPI/Swagger middleware
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
    public static WebApplication UseOpenApi(this WebApplication app)
    {
        var apiOptions = app.Services.GetService<IOptions<ApiOptions>>()?.Value ?? new ApiOptions();
        
        if (app.Environment.IsDevelopment() || apiOptions.EnableSwaggerInProduction)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{apiOptions.Title} v{apiOptions.Version}");
                options.RoutePrefix = "swagger";
                options.DocumentTitle = apiOptions.Title;
                options.DisplayRequestDuration();
                options.EnableTryItOutByDefault();
                options.EnableFilter();
                options.EnableDeepLinking();
            });

            // Use Scalar (alternative to Swagger UI)
            app.MapScalarApiReference(options =>
            {
                options.WithTitle(apiOptions.Title)
                       .WithTheme(ScalarTheme.BluePlanet)
                       .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }

        return app;
    }

    /// <summary>
    /// Uses health checks middleware
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
    public static WebApplication UseApiHealthChecks(this WebApplication app)
    {
        var healthOptions = app.Services.GetService<IOptions<HealthCheckOptions>>()?.Value ?? new HealthCheckOptions();
        
        if (healthOptions.Enabled)
        {
            app.MapHealthChecks(healthOptions.EndpointPath, new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    
                    var response = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            data = entry.Value.Data
                        }),
                        totalDuration = report.TotalDuration
                    };

                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                    }));
                }
            });

            // Map health checks UI if configured
            if (!string.IsNullOrEmpty(healthOptions.UIPath))
            {
                app.MapHealthChecksUI(options =>
                {
                    options.UIPath = healthOptions.UIPath;
                });
            }
        }

        return app;
    }

    /// <summary>
    /// Maps API endpoints
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="configureEndpoints">Action to configure endpoints</param>
    /// <returns>The web application</returns>
    public static WebApplication MapApiEndpoints(this WebApplication app, Action<IEndpointRouteBuilder> configureEndpoints)
    {
        configureEndpoints(app);
        return app;
    }

    /// <summary>
    /// Maps versioned API endpoints
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="version">The API version</param>
    /// <param name="configureEndpoints">Action to configure endpoints</param>
    /// <returns>The web application</returns>
    public static WebApplication MapVersionedApiEndpoints(
        this WebApplication app, 
        string version, 
        Action<RouteGroupBuilder> configureEndpoints)
    {
        var versionedGroup = app.MapGroup($"/api/v{version}")
            .WithTags($"API v{version}")
            .WithOpenApi();

        configureEndpoints(versionedGroup);

        return app;
    }
}

/// <summary>
/// Scalar theme enumeration
/// </summary>
public enum ScalarTheme
{
    /// <summary>
    /// Blue planet theme
    /// </summary>
    BluePlanet,
    
    /// <summary>
    /// Default theme
    /// </summary>
    Default
}

/// <summary>
/// Scalar target enumeration
/// </summary>
public enum ScalarTarget
{
    /// <summary>
    /// C# target
    /// </summary>
    CSharp
}

/// <summary>
/// Scalar client enumeration
/// </summary>
public enum ScalarClient
{
    /// <summary>
    /// HTTP client
    /// </summary>
    HttpClient
}

/// <summary>
/// Placeholder Scalar API reference options
/// </summary>
public class ScalarApiReferenceOptions
{
    /// <summary>
    /// Sets the title
    /// </summary>
    /// <param name="title">The title</param>
    /// <returns>The options</returns>
    public ScalarApiReferenceOptions WithTitle(string title) => this;

    /// <summary>
    /// Sets the theme
    /// </summary>
    /// <param name="theme">The theme</param>
    /// <returns>The options</returns>
    public ScalarApiReferenceOptions WithTheme(ScalarTheme theme) => this;

    /// <summary>
    /// Sets the default HTTP client
    /// </summary>
    /// <param name="target">The target</param>
    /// <param name="client">The client</param>
    /// <returns>The options</returns>
    public ScalarApiReferenceOptions WithDefaultHttpClient(ScalarTarget target, ScalarClient client) => this;
}

/// <summary>
/// Extension methods for Scalar integration
/// </summary>
public static class ScalarExtensions
{
    /// <summary>
    /// Maps Scalar API reference
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="configureOptions">Action to configure options</param>
    /// <returns>The web application</returns>
    public static WebApplication MapScalarApiReference(this WebApplication app, Action<ScalarApiReferenceOptions> configureOptions)
    {
        var options = new ScalarApiReferenceOptions();
        configureOptions(options);
        
        // Placeholder implementation - would integrate with actual Scalar package
        app.MapGet("/scalar", () => "Scalar API Reference would be here");
        
        return app;
    }
} 