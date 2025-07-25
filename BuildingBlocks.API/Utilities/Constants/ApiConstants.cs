namespace BuildingBlocks.API.Utilities.Constants;

/// <summary>
/// Contains commonly used API constants
/// </summary>
public static class ApiConstants
{
    /// <summary>
    /// HTTP header names
    /// </summary>
    public static class Headers
    {
        /// <summary>
        /// Correlation ID header name
        /// </summary>
        public const string CorrelationId = "X-Correlation-ID";

        /// <summary>
        /// Request ID header name
        /// </summary>
        public const string RequestId = "X-Request-ID";

        /// <summary>
        /// API version header name
        /// </summary>
        public const string ApiVersion = "X-API-Version";

        /// <summary>
        /// Rate limit remaining header name
        /// </summary>
        public const string RateLimitRemaining = "X-RateLimit-Remaining";

        /// <summary>
        /// Rate limit limit header name
        /// </summary>
        public const string RateLimitLimit = "X-RateLimit-Limit";

        /// <summary>
        /// Rate limit reset header name
        /// </summary>
        public const string RateLimitReset = "X-RateLimit-Reset";

        /// <summary>
        /// Total count header name for pagination
        /// </summary>
        public const string TotalCount = "X-Total-Count";

        /// <summary>
        /// Content range header name
        /// </summary>
        public const string ContentRange = "Content-Range";

        /// <summary>
        /// Accept ranges header name
        /// </summary>
        public const string AcceptRanges = "Accept-Ranges";

        /// <summary>
        /// User agent header name
        /// </summary>
        public const string UserAgent = "User-Agent";

        /// <summary>
        /// Authorization header name
        /// </summary>
        public const string Authorization = "Authorization";

        /// <summary>
        /// Content type header name
        /// </summary>
        public const string ContentType = "Content-Type";

        /// <summary>
        /// Accept header name
        /// </summary>
        public const string Accept = "Accept";

        /// <summary>
        /// If-Match header name
        /// </summary>
        public const string IfMatch = "If-Match";

        /// <summary>
        /// If-None-Match header name
        /// </summary>
        public const string IfNoneMatch = "If-None-Match";

        /// <summary>
        /// ETag header name
        /// </summary>
        public const string ETag = "ETag";

        /// <summary>
        /// Cache control header name
        /// </summary>
        public const string CacheControl = "Cache-Control";

        /// <summary>
        /// Last modified header name
        /// </summary>
        public const string LastModified = "Last-Modified";
    }

    /// <summary>
    /// Content type values
    /// </summary>
    public static class ContentTypes
    {
        /// <summary>
        /// JSON content type
        /// </summary>
        public const string Json = "application/json";

        /// <summary>
        /// XML content type
        /// </summary>
        public const string Xml = "application/xml";

        /// <summary>
        /// Plain text content type
        /// </summary>
        public const string Text = "text/plain";

        /// <summary>
        /// HTML content type
        /// </summary>
        public const string Html = "text/html";

        /// <summary>
        /// Form URL encoded content type
        /// </summary>
        public const string FormUrlEncoded = "application/x-www-form-urlencoded";

        /// <summary>
        /// Multipart form data content type
        /// </summary>
        public const string MultipartFormData = "multipart/form-data";

        /// <summary>
        /// Octet stream content type
        /// </summary>
        public const string OctetStream = "application/octet-stream";

        /// <summary>
        /// PDF content type
        /// </summary>
        public const string Pdf = "application/pdf";

        /// <summary>
        /// CSV content type
        /// </summary>
        public const string Csv = "text/csv";

        /// <summary>
        /// Problem details content type
        /// </summary>
        public const string ProblemJson = "application/problem+json";

        /// <summary>
        /// HAL JSON content type
        /// </summary>
        public const string HalJson = "application/hal+json";

        /// <summary>
        /// JSON API content type
        /// </summary>
        public const string JsonApi = "application/vnd.api+json";
    }

    /// <summary>
    /// HTTP status code groups
    /// </summary>
    public static class StatusCodes
    {
        /// <summary>
        /// Success status codes (2xx)
        /// </summary>
        public static class Success
        {
            /// <summary>
            /// 200 OK
            /// </summary>
            public const int Ok = 200;

            /// <summary>
            /// 201 Created
            /// </summary>
            public const int Created = 201;

            /// <summary>
            /// 202 Accepted
            /// </summary>
            public const int Accepted = 202;

            /// <summary>
            /// 204 No Content
            /// </summary>
            public const int NoContent = 204;

            /// <summary>
            /// 206 Partial Content
            /// </summary>
            public const int PartialContent = 206;
        }

        /// <summary>
        /// Client error status codes (4xx)
        /// </summary>
        public static class ClientError
        {
            /// <summary>
            /// 400 Bad Request
            /// </summary>
            public const int BadRequest = 400;

            /// <summary>
            /// 401 Unauthorized
            /// </summary>
            public const int Unauthorized = 401;

            /// <summary>
            /// 403 Forbidden
            /// </summary>
            public const int Forbidden = 403;

            /// <summary>
            /// 404 Not Found
            /// </summary>
            public const int NotFound = 404;

            /// <summary>
            /// 405 Method Not Allowed
            /// </summary>
            public const int MethodNotAllowed = 405;

            /// <summary>
            /// 406 Not Acceptable
            /// </summary>
            public const int NotAcceptable = 406;

            /// <summary>
            /// 408 Request Timeout
            /// </summary>
            public const int RequestTimeout = 408;

            /// <summary>
            /// 409 Conflict
            /// </summary>
            public const int Conflict = 409;

            /// <summary>
            /// 410 Gone
            /// </summary>
            public const int Gone = 410;

            /// <summary>
            /// 412 Precondition Failed
            /// </summary>
            public const int PreconditionFailed = 412;

            /// <summary>
            /// 413 Payload Too Large
            /// </summary>
            public const int PayloadTooLarge = 413;

            /// <summary>
            /// 415 Unsupported Media Type
            /// </summary>
            public const int UnsupportedMediaType = 415;

            /// <summary>
            /// 422 Unprocessable Entity
            /// </summary>
            public const int UnprocessableEntity = 422;

            /// <summary>
            /// 429 Too Many Requests
            /// </summary>
            public const int TooManyRequests = 429;
        }

        /// <summary>
        /// Server error status codes (5xx)
        /// </summary>
        public static class ServerError
        {
            /// <summary>
            /// 500 Internal Server Error
            /// </summary>
            public const int InternalServerError = 500;

            /// <summary>
            /// 501 Not Implemented
            /// </summary>
            public const int NotImplemented = 501;

            /// <summary>
            /// 502 Bad Gateway
            /// </summary>
            public const int BadGateway = 502;

            /// <summary>
            /// 503 Service Unavailable
            /// </summary>
            public const int ServiceUnavailable = 503;

            /// <summary>
            /// 504 Gateway Timeout
            /// </summary>
            public const int GatewayTimeout = 504;
        }
    }

    /// <summary>
    /// API versioning constants
    /// </summary>
    public static class Versioning
    {
        /// <summary>
        /// Default API version
        /// </summary>
        public const string DefaultVersion = "1.0";

        /// <summary>
        /// API version parameter name
        /// </summary>
        public const string VersionParameterName = "version";

        /// <summary>
        /// API version header name
        /// </summary>
        public const string VersionHeaderName = "X-API-Version";

        /// <summary>
        /// API version query parameter name
        /// </summary>
        public const string VersionQueryParameterName = "api-version";

        /// <summary>
        /// API version media type parameter name
        /// </summary>
        public const string VersionMediaTypeParameterName = "v";
    }

    /// <summary>
    /// Pagination constants
    /// </summary>
    public static class Pagination
    {
        /// <summary>
        /// Default page size
        /// </summary>
        public const int DefaultPageSize = 10;

        /// <summary>
        /// Maximum page size
        /// </summary>
        public const int MaxPageSize = 100;

        /// <summary>
        /// Default page number
        /// </summary>
        public const int DefaultPageNumber = 1;

        /// <summary>
        /// Page number parameter name
        /// </summary>
        public const string PageNumberParameterName = "pageNumber";

        /// <summary>
        /// Page size parameter name
        /// </summary>
        public const string PageSizeParameterName = "pageSize";

        /// <summary>
        /// Sort by parameter name
        /// </summary>
        public const string SortByParameterName = "sortBy";

        /// <summary>
        /// Sort order parameter name
        /// </summary>
        public const string SortOrderParameterName = "sortOrder";

        /// <summary>
        /// Search parameter name
        /// </summary>
        public const string SearchParameterName = "search";

        /// <summary>
        /// Filter parameter name
        /// </summary>
        public const string FilterParameterName = "filter";
    }

    /// <summary>
    /// Error type constants
    /// </summary>
    public static class ErrorTypes
    {
        /// <summary>
        /// Validation error type
        /// </summary>
        public const string Validation = "validation";

        /// <summary>
        /// Business rule error type
        /// </summary>
        public const string BusinessRule = "business_rule";

        /// <summary>
        /// Authorization error type
        /// </summary>
        public const string Authorization = "authorization";

        /// <summary>
        /// Authentication error type
        /// </summary>
        public const string Authentication = "authentication";

        /// <summary>
        /// Not found error type
        /// </summary>
        public const string NotFound = "not_found";

        /// <summary>
        /// Conflict error type
        /// </summary>
        public const string Conflict = "conflict";

        /// <summary>
        /// System error type
        /// </summary>
        public const string System = "system";

        /// <summary>
        /// External service error type
        /// </summary>
        public const string ExternalService = "external_service";
    }

    /// <summary>
    /// Cache control constants
    /// </summary>
    public static class CacheControl
    {
        /// <summary>
        /// No cache directive
        /// </summary>
        public const string NoCache = "no-cache";

        /// <summary>
        /// No store directive
        /// </summary>
        public const string NoStore = "no-store";

        /// <summary>
        /// Public directive
        /// </summary>
        public const string Public = "public";

        /// <summary>
        /// Private directive
        /// </summary>
        public const string Private = "private";

        /// <summary>
        /// Max age directive format
        /// </summary>
        public const string MaxAgeFormat = "max-age={0}";

        /// <summary>
        /// Must revalidate directive
        /// </summary>
        public const string MustRevalidate = "must-revalidate";

        /// <summary>
        /// Proxy revalidate directive
        /// </summary>
        public const string ProxyRevalidate = "proxy-revalidate";
    }

    /// <summary>
    /// Security header constants
    /// </summary>
    public static class Security
    {
        /// <summary>
        /// X-Content-Type-Options header
        /// </summary>
        public const string XContentTypeOptions = "X-Content-Type-Options";

        /// <summary>
        /// X-Frame-Options header
        /// </summary>
        public const string XFrameOptions = "X-Frame-Options";

        /// <summary>
        /// X-XSS-Protection header
        /// </summary>
        public const string XXssProtection = "X-XSS-Protection";

        /// <summary>
        /// Strict-Transport-Security header
        /// </summary>
        public const string StrictTransportSecurity = "Strict-Transport-Security";

        /// <summary>
        /// Content-Security-Policy header
        /// </summary>
        public const string ContentSecurityPolicy = "Content-Security-Policy";

        /// <summary>
        /// Referrer-Policy header
        /// </summary>
        public const string ReferrerPolicy = "Referrer-Policy";

        /// <summary>
        /// Feature-Policy header
        /// </summary>
        public const string FeaturePolicy = "Feature-Policy";

        /// <summary>
        /// Permissions-Policy header
        /// </summary>
        public const string PermissionsPolicy = "Permissions-Policy";
    }

    /// <summary>
    /// OpenAPI/Swagger constants
    /// </summary>
    public static class OpenApi
    {
        /// <summary>
        /// Default OpenAPI version
        /// </summary>
        public const string DefaultVersion = "3.0.1";

        /// <summary>
        /// Default swagger endpoint
        /// </summary>
        public const string DefaultEndpoint = "/swagger/v1/swagger.json";

        /// <summary>
        /// Default swagger UI endpoint
        /// </summary>
        public const string DefaultUiEndpoint = "/swagger";

        /// <summary>
        /// Bearer authentication scheme name
        /// </summary>
        public const string BearerSchemeName = "Bearer";

        /// <summary>
        /// API key authentication scheme name
        /// </summary>
        public const string ApiKeySchemeName = "ApiKey";

        /// <summary>
        /// OAuth2 authentication scheme name
        /// </summary>
        public const string OAuth2SchemeName = "OAuth2";
    }
} 