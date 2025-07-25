namespace BuildingBlocks.API.Configuration.Options;

/// <summary>
/// API configuration options
/// </summary>
public class ApiOptions
{
    /// <summary>
    /// Gets or sets the API title
    /// </summary>
    public string Title { get; set; } = "BuildingBlocks API";

    /// <summary>
    /// Gets or sets the API version
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Gets or sets the API description
    /// </summary>
    public string Description { get; set; } = "A comprehensive API built with BuildingBlocks";

    /// <summary>
    /// Gets or sets whether to enable detailed errors
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to enable request logging
    /// </summary>
    public bool EnableRequestLogging { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable Swagger in production
    /// </summary>
    public bool EnableSwaggerInProduction { get; set; } = false;

    /// <summary>
    /// Gets or sets the request timeout
    /// </summary>
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the maximum request body size
    /// </summary>
    public long MaxRequestBodySize { get; set; } = 1024 * 1024 * 10; // 10MB
}

/// <summary>
/// Authentication configuration options
/// </summary>
public class AuthenticationOptions
{
    /// <summary>
    /// Gets or sets the JWT options
    /// </summary>
    public JwtOptions Jwt { get; set; } = new();

    /// <summary>
    /// Gets or sets the API key options
    /// </summary>
    public ApiKeyOptions ApiKey { get; set; } = new();

    /// <summary>
    /// Gets or sets the OAuth options
    /// </summary>
    public OAuthOptions OAuth { get; set; } = new();
}

/// <summary>
/// JWT authentication options
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Gets or sets whether JWT authentication is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the JWT secret key
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the JWT issuer
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the JWT audience
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the JWT expiration time
    /// </summary>
    public TimeSpan Expiration { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets or sets whether to validate issuer
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to validate audience
    /// </summary>
    public bool ValidateAudience { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to validate lifetime
    /// </summary>
    public bool ValidateLifetime { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to validate issuer signing key
    /// </summary>
    public bool ValidateIssuerSigningKey { get; set; } = true;
}

/// <summary>
/// API key authentication options
/// </summary>
public class ApiKeyOptions
{
    /// <summary>
    /// Gets or sets whether API key authentication is enabled
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the API key header name
    /// </summary>
    public string HeaderName { get; set; } = "X-API-Key";

    /// <summary>
    /// Gets or sets the valid API keys
    /// </summary>
    public List<string> ValidApiKeys { get; set; } = new();
}

/// <summary>
/// OAuth authentication options
/// </summary>
public class OAuthOptions
{
    /// <summary>
    /// Gets or sets whether OAuth authentication is enabled
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the OAuth authority
    /// </summary>
    public string Authority { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the OAuth client ID
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the OAuth client secret
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the OAuth scopes
    /// </summary>
    public List<string> Scopes { get; set; } = new();
}

/// <summary>
/// CORS configuration options
/// </summary>
public class CorsOptions
{
    /// <summary>
    /// Gets or sets the allowed origins
    /// </summary>
    public List<string> AllowedOrigins { get; set; } = new() { "*" };

    /// <summary>
    /// Gets or sets the allowed methods
    /// </summary>
    public List<string> AllowedMethods { get; set; } = new() { "GET", "POST", "PUT", "DELETE", "OPTIONS" };

    /// <summary>
    /// Gets or sets the allowed headers
    /// </summary>
    public List<string> AllowedHeaders { get; set; } = new() { "*" };

    /// <summary>
    /// Gets or sets whether credentials are allowed
    /// </summary>
    public bool AllowCredentials { get; set; } = false;

    /// <summary>
    /// Gets or sets the preflight max age
    /// </summary>
    public TimeSpan PreflightMaxAge { get; set; } = TimeSpan.FromDays(1);
}

/// <summary>
/// Rate limiting configuration options
/// </summary>
public class RateLimitingOptions
{
    /// <summary>
    /// Gets or sets whether rate limiting is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the permit limit
    /// </summary>
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// Gets or sets the time window
    /// </summary>
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets or sets the queue limit
    /// </summary>
    public int QueueLimit { get; set; } = 10;

    /// <summary>
    /// Gets or sets the replenishment period
    /// </summary>
    public TimeSpan ReplenishmentPeriod { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the tokens per period
    /// </summary>
    public int TokensPerPeriod { get; set; } = 10;
}

/// <summary>
/// Health check configuration options
/// </summary>
public class HealthCheckOptions
{
    /// <summary>
    /// Gets or sets whether health checks are enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the health check endpoint path
    /// </summary>
    public string EndpointPath { get; set; } = "/health";

    /// <summary>
    /// Gets or sets the health check UI path
    /// </summary>
    public string UIPath { get; set; } = "/health-ui";

    /// <summary>
    /// Gets or sets whether to show detailed health information
    /// </summary>
    public bool ShowDetails { get; set; } = false;

    /// <summary>
    /// Gets or sets the health check timeout
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
} 