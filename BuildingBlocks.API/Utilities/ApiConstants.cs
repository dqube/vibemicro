namespace BuildingBlocks.API.Utilities;

/// <summary>
/// API constants for headers, content types, status codes, and common values
/// </summary>
public static class ApiConstants
{
    /// <summary>
    /// HTTP headers
    /// </summary>
    public static class Headers
    {
        public const string Accept = "Accept";
        public const string AcceptLanguage = "Accept-Language";
        public const string Authorization = "Authorization";
        public const string CacheControl = "Cache-Control";
        public const string ContentType = "Content-Type";
        public const string CorrelationId = "X-Correlation-ID";
        public const string RequestId = "X-Request-ID";
        public const string TraceId = "X-Trace-ID";
        public const string UserAgent = "User-Agent";
        public const string IfMatch = "If-Match";
        public const string IfNoneMatch = "If-None-Match";
        public const string IfModifiedSince = "If-Modified-Since";
        public const string LastModified = "Last-Modified";
        public const string ETag = "ETag";
        public const string Location = "Location";
        public const string XForwardedFor = "X-Forwarded-For";
        public const string XRealIp = "X-Real-IP";
        public const string ApiKey = "X-API-Key";
        public const string ApiVersion = "X-API-Version";
        public const string TotalCount = "X-Total-Count";
        public const string PageCount = "X-Page-Count";
        public const string CurrentPage = "X-Current-Page";
        public const string PageSize = "X-Page-Size";
        public const string RateLimit = "X-RateLimit-Limit";
        public const string RateLimitRemaining = "X-RateLimit-Remaining";
        public const string RateLimitReset = "X-RateLimit-Reset";
    }

    /// <summary>
    /// Content types
    /// </summary>
    public static class ContentTypes
    {
        public const string ApplicationJson = "application/json";
        public const string ApplicationXml = "application/xml";
        public const string ApplicationPdf = "application/pdf";
        public const string ApplicationOctetStream = "application/octet-stream";
        public const string TextPlain = "text/plain";
        public const string TextHtml = "text/html";
        public const string TextCsv = "text/csv";
        public const string TextXml = "text/xml";
        public const string MultipartFormData = "multipart/form-data";
        public const string ApplicationFormUrlEncoded = "application/x-www-form-urlencoded";
        public const string ImageJpeg = "image/jpeg";
        public const string ImagePng = "image/png";
        public const string ImageGif = "image/gif";
        public const string ApplicationJsonPatch = "application/json-patch+json";
        public const string ApplicationProblem = "application/problem+json";
        public const string ApplicationHal = "application/hal+json";
    }

    /// <summary>
    /// HTTP status codes
    /// </summary>
    public static class StatusCodes
    {
        // 2xx Success
        public const int Ok = 200;
        public const int Created = 201;
        public const int Accepted = 202;
        public const int NoContent = 204;
        public const int PartialContent = 206;

        // 3xx Redirection
        public const int NotModified = 304;
        public const int TemporaryRedirect = 307;
        public const int PermanentRedirect = 308;

        // 4xx Client Error
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int MethodNotAllowed = 405;
        public const int NotAcceptable = 406;
        public const int Conflict = 409;
        public const int Gone = 410;
        public const int PreconditionFailed = 412;
        public const int PayloadTooLarge = 413;
        public const int UnsupportedMediaType = 415;
        public const int UnprocessableEntity = 422;
        public const int TooManyRequests = 429;

        // 5xx Server Error
        public const int InternalServerError = 500;
        public const int NotImplemented = 501;
        public const int BadGateway = 502;
        public const int ServiceUnavailable = 503;
        public const int GatewayTimeout = 504;
    }

    /// <summary>
    /// API versioning
    /// </summary>
    public static class Versioning
    {
        public const string DefaultVersion = "1.0";
        public const string HeaderName = "X-API-Version";
        public const string QueryParameterName = "version";
        public const string UrlSegmentName = "v{version:apiVersion}";
        public const string MediaTypeParameterName = "version";
    }

    /// <summary>
    /// Pagination constants
    /// </summary>
    public static class Pagination
    {
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
        public const string PageNumberQueryParameter = "pageNumber";
        public const string PageSizeQueryParameter = "pageSize";
        public const string SortByQueryParameter = "sortBy";
        public const string SortDirectionQueryParameter = "sortDirection";
        public const string SearchQueryParameter = "search";
        public const string FilterQueryParameter = "filter";
    }

    /// <summary>
    /// Security constants
    /// </summary>
    public static class Security
    {
        public const string BearerScheme = "Bearer";
        public const string ApiKeyScheme = "ApiKey";
        public const string BasicScheme = "Basic";
        public const string JwtScheme = "JWT";
        public const string CookieScheme = "Cookies";
        
        // Security headers
        public const string XContentTypeOptions = "X-Content-Type-Options";
        public const string XFrameOptions = "X-Frame-Options";
        public const string XSSProtection = "X-XSS-Protection";
        public const string StrictTransportSecurity = "Strict-Transport-Security";
        public const string ContentSecurityPolicy = "Content-Security-Policy";
        public const string ReferrerPolicy = "Referrer-Policy";
        public const string PermissionsPolicy = "Permissions-Policy";
    }

    /// <summary>
    /// OpenAPI/Swagger constants
    /// </summary>
    public static class OpenApi
    {
        public const string DefaultTitle = "BuildingBlocks API";
        public const string DefaultDescription = "A comprehensive API built with BuildingBlocks";
        public const string DefaultContactName = "API Support";
        public const string DefaultContactEmail = "support@example.com";
        public const string DefaultLicenseName = "MIT";
        public const string DefaultLicenseUrl = "https://opensource.org/licenses/MIT";
        public const string DefaultTermsOfService = "https://example.com/terms";
        
        // Security scheme names
        public const string BearerSecurityScheme = "Bearer";
        public const string ApiKeySecurityScheme = "ApiKey";
        public const string OAuthSecurityScheme = "OAuth2";
        
        // Common tags
        public const string AuthenticationTag = "Authentication";
        public const string UsersTag = "Users";
        public const string HealthTag = "Health";
        public const string MetricsTag = "Metrics";
    }

    /// <summary>
    /// Cache control values
    /// </summary>
    public static class CacheControl
    {
        public const string NoCache = "no-cache";
        public const string NoStore = "no-store";
        public const string MustRevalidate = "must-revalidate";
        public const string Public = "public";
        public const string Private = "private";
        public const string MaxAge = "max-age";
        public const string SMaxAge = "s-maxage";
        public const string NoTransform = "no-transform";
        public const string ProxyRevalidate = "proxy-revalidate";
    }

    /// <summary>
    /// Minimal API endpoint names and patterns
    /// </summary>
    public static class Endpoints
    {
        // Route patterns
        public const string ApiV1Prefix = "/api/v1";
        public const string ApiV2Prefix = "/api/v2";
        public const string HealthPattern = "/health";
        public const string MetricsPattern = "/metrics";
        public const string IdRoutePattern = "/{id}";
        public const string SearchPattern = "/search";
        
        // Common endpoint names
        public const string GetById = "GetById";
        public const string GetAll = "GetAll";
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string Search = "Search";
        public const string Health = "Health";
        public const string Metrics = "Metrics";
        
        // Query parameter names
        public const string Id = "id";
        public const string Ids = "ids";
        public const string Query = "query";
        public const string Filter = "filter";
        public const string Include = "include";
        public const string Expand = "expand";
        public const string Fields = "fields";
    }

    /// <summary>
    /// Problem Details constants
    /// </summary>
    public static class ProblemDetails
    {
        // RFC 7807 standard types
        public const string AboutBlank = "about:blank";
        public const string BadRequestType = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        public const string UnauthorizedType = "https://tools.ietf.org/html/rfc7235#section-3.1";
        public const string ForbiddenType = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
        public const string NotFoundType = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
        public const string MethodNotAllowedType = "https://tools.ietf.org/html/rfc7231#section-6.5.5";
        public const string ConflictType = "https://tools.ietf.org/html/rfc7231#section-6.5.8";
        public const string UnprocessableEntityType = "https://tools.ietf.org/html/rfc4918#section-11.2";
        public const string TooManyRequestsType = "https://tools.ietf.org/html/rfc6585#section-4";
        public const string InternalServerErrorType = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
        
        // Custom extension properties
        public const string CorrelationIdProperty = "correlationId";
        public const string TraceIdProperty = "traceId";
        public const string TimestampProperty = "timestamp";
        public const string ErrorsProperty = "errors";
        public const string ExceptionTypeProperty = "exceptionType";
        public const string StackTraceProperty = "stackTrace";
        public const string InnerExceptionProperty = "innerException";
    }

    /// <summary>
    /// JSON property names for consistent API responses
    /// </summary>
    public static class JsonProperties
    {
        public const string Success = "success";
        public const string Data = "data";
        public const string Message = "message";
        public const string Timestamp = "timestamp";
        public const string TraceId = "traceId";
        public const string Errors = "errors";
        public const string Pagination = "pagination";
        public const string TotalCount = "totalCount";
        public const string PageNumber = "pageNumber";
        public const string PageSize = "pageSize";
        public const string TotalPages = "totalPages";
        public const string HasPrevious = "hasPrevious";
        public const string HasNext = "hasNext";
        public const string CurrentPage = "currentPage";
    }

    /// <summary>
    /// Common response messages
    /// </summary>
    public static class Messages
    {
        public const string Success = "Success";
        public const string Created = "Created successfully";
        public const string Updated = "Updated successfully";
        public const string Deleted = "Deleted successfully";
        public const string NotFound = "Resource not found";
        public const string BadRequest = "Invalid request";
        public const string Unauthorized = "Unauthorized access";
        public const string Forbidden = "Access forbidden";
        public const string Conflict = "Resource conflict";
        public const string ValidationFailed = "Validation failed";
        public const string InternalError = "Internal server error";
        public const string ServiceUnavailable = "Service temporarily unavailable";
        public const string TooManyRequests = "Too many requests";
    }
} 