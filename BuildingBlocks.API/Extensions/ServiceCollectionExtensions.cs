using BuildingBlocks.API.Configuration.Options;
using BuildingBlocks.API.Middleware.ErrorHandling;
using BuildingBlocks.API.Middleware.Logging;
using BuildingBlocks.Domain.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Threading.RateLimiting;

namespace BuildingBlocks.API.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to configure API services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the API layer services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Add core API services
        services.AddApiCore(configuration);
        
        // Add authentication
        services.AddApiAuthentication(configuration);
        
        // Add authorization
        services.AddApiAuthorization(configuration);
        
        // Add versioning
        services.AddApiVersioning(configuration);
        
        // Add OpenAPI/Swagger
        services.AddOpenApi(configuration);
        
        // Add CORS
        services.AddApiCors(configuration);
        
        // Add rate limiting
        services.AddApiRateLimiting(configuration);
        
        // Add health checks
        services.AddApiHealthChecks(configuration);
        
        // Add validation
        services.AddApiValidation();
        
        // Add middleware services
        services.AddMiddlewareServices();

        return services;
    }

    /// <summary>
    /// Adds core API services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApiCore(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure API options
        services.Configure<ApiOptions>(configuration.GetSection("Api"));
        
        // Add controllers with JSON options
        services.AddControllers(options =>
        {
            // Add custom filters
            options.Filters.Add<GlobalActionFilter>();
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.WriteIndented = false;
            options.JsonSerializerOptions.AddStronglyTypedIdConverters();
        });

        // Add API explorer
        services.AddEndpointsApiExplorer();

        // Add custom problem details
        services.AddProblemDetailsMiddleware(options =>
        {
            options.IncludeExceptionDetails = false; // Set in environment-specific config
            options.MapStandardExceptions();
        });

        // Configure JSON for strongly typed IDs
        services.ConfigureJsonOptionsForStronglyTypedIds(options =>
        {
            options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.WriteIndented = false;
        });

        return services;
    }

    /// <summary>
    /// Adds API authentication services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptions = configuration.GetSection("Authentication").Get<AuthenticationOptions>() ?? new();
        services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));

        var authBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

        // Add JWT Bearer authentication
        if (authOptions.Jwt.Enabled)
        {
            authBuilder.AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authOptions.Jwt.Issuer,
                    ValidAudience = authOptions.Jwt.Audience,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(authOptions.Jwt.SecretKey))
                };
            });
        }

        // Add API Key authentication
        if (authOptions.ApiKey.Enabled)
        {
            authBuilder.AddScheme<ApiKeyAuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
                "ApiKey", options =>
                {
                    options.ApiKeyHeaderName = authOptions.ApiKey.HeaderName;
                });
        }

        return services;
    }

    /// <summary>
    /// Adds API authorization services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApiAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization(options =>
        {
            // Add default policy
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Add custom policies
            options.AddPolicy("RequireAdminRole", policy =>
                policy.RequireRole("Admin"));

            options.AddPolicy("RequireManagerRole", policy =>
                policy.RequireRole("Manager", "Admin"));
        });

        return services;
    }

    /// <summary>
    /// Adds API versioning services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApiVersioning(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-API-Version"),
                new QueryStringApiVersionReader("version"));
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    /// <summary>
    /// Adds OpenAPI/Swagger services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddOpenApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BuildingBlocks API",
                Version = "v1",
                Description = "A comprehensive API built with BuildingBlocks",
                Contact = new OpenApiContact
                {
                    Name = "API Support",
                    Email = "support@example.com"
                }
            });

            // Add JWT authentication to Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Include XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    /// <summary>
    /// Adds CORS services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = configuration.GetSection("Cors").Get<CorsOptions>() ?? new();
        services.Configure<CorsOptions>(configuration.GetSection("Cors"));

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(corsOptions.AllowedOrigins.ToArray())
                    .WithMethods(corsOptions.AllowedMethods.ToArray())
                    .WithHeaders(corsOptions.AllowedHeaders.ToArray());

                if (corsOptions.AllowCredentials)
                {
                    builder.AllowCredentials();
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Adds rate limiting services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var rateLimitOptions = configuration.GetSection("RateLimit").Get<RateLimitingOptions>() ?? new();
        services.Configure<RateLimitingOptions>(configuration.GetSection("RateLimit"));

        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("Default", limiterOptions =>
            {
                limiterOptions.PermitLimit = rateLimitOptions.PermitLimit;
                limiterOptions.Window = rateLimitOptions.Window;
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = rateLimitOptions.QueueLimit;
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        return services;
    }

    /// <summary>
    /// Adds health check services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
            .AddDbContextCheck<BuildingBlocks.Infrastructure.Data.Context.ApplicationDbContext>();

        return services;
    }

    /// <summary>
    /// Adds validation services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApiValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        return services;
    }

    /// <summary>
    /// Adds middleware services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddMiddlewareServices(this IServiceCollection services)
    {
        services.AddSingleton<RequestLoggingOptions>();
        services.AddTransient<GlobalExceptionMiddleware>();
        services.AddTransient<RequestLoggingMiddleware>();
        services.AddTransient<CorrelationIdMiddleware>();

        // Configure correlation ID options
        services.Configure<CorrelationIdOptions>(options =>
        {
            options.RequestHeaderName = "X-Correlation-ID";
            options.ResponseHeaderName = "X-Correlation-ID";
            options.IncludeInResponse = true;
            options.ValidateCorrelationId = true;
            options.CorrelationIdGenerator = CorrelationIdGenerator.Guid;
        });

        return services;
    }
}

/// <summary>
/// Placeholder global action filter
/// </summary>
public class GlobalActionFilter : IActionFilter
{
    /// <summary>
    /// Called before the action executes
    /// </summary>
    /// <param name="context">The action executing context</param>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Implement global action logic here
    }

    /// <summary>
    /// Called after the action executes
    /// </summary>
    /// <param name="context">The action executed context</param>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Implement global action logic here
    }
}

/// <summary>
/// Placeholder API key authentication scheme options
/// </summary>
public class ApiKeyAuthenticationSchemeOptions : Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions
{
    /// <summary>
    /// Gets or sets the API key header name
    /// </summary>
    public string ApiKeyHeaderName { get; set; } = "X-API-Key";
}

/// <summary>
/// Placeholder API key authentication handler
/// </summary>
public class ApiKeyAuthenticationHandler : Microsoft.AspNetCore.Authentication.AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
{
    /// <summary>
    /// Initializes a new instance of the ApiKeyAuthenticationHandler class
    /// </summary>
    /// <param name="options">The options</param>
    /// <param name="logger">The logger factory</param>
    /// <param name="encoder">The URL encoder</param>
    /// <param name="clock">The system clock</param>
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        System.Text.Encodings.Web.UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    /// <summary>
    /// Handles the authentication
    /// </summary>
    /// <returns>The authentication result</returns>
    protected override Task<Microsoft.AspNetCore.Authentication.AuthenticateResult> HandleAuthenticateAsync()
    {
        // Implement API key authentication logic here
        return Task.FromResult(Microsoft.AspNetCore.Authentication.AuthenticateResult.NoResult());
    }
} 