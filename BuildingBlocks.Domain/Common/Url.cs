using BuildingBlocks.Domain.Guards;
using BuildingBlocks.Domain.ValueObjects;

namespace BuildingBlocks.Domain.Common;

/// <summary>
/// Value object representing a URL
/// </summary>
public sealed record Url : SingleValueObject<string>
{
    /// <summary>
    /// Gets the URI representation of the URL
    /// </summary>
    public Uri Uri { get; }

    /// <summary>
    /// Gets the scheme (protocol) of the URL
    /// </summary>
    public string Scheme => Uri.Scheme;

    /// <summary>
    /// Gets the host of the URL
    /// </summary>
    public string Host => Uri.Host;

    /// <summary>
    /// Gets the port of the URL
    /// </summary>
    public int Port => Uri.Port;

    /// <summary>
    /// Gets the path of the URL
    /// </summary>
    public string Path => Uri.AbsolutePath;

    /// <summary>
    /// Gets the query string of the URL
    /// </summary>
    public string Query => Uri.Query;

    /// <summary>
    /// Gets the fragment (hash) of the URL
    /// </summary>
    public string Fragment => Uri.Fragment;

    /// <summary>
    /// Initializes a new instance of the Url class
    /// </summary>
    /// <param name="value">The URL string</param>
    public Url(string value) : base(ValidateAndNormalize(value))
    {
        Uri = new Uri(Value);
    }

    /// <summary>
    /// Initializes a new instance of the Url class from a Uri
    /// </summary>
    /// <param name="uri">The URI instance</param>
    public Url(Uri uri) : this(Guard.NotNull(uri).ToString())
    {
    }

    /// <summary>
    /// Checks if the URL uses HTTPS
    /// </summary>
    /// <returns>True if the URL uses HTTPS, false otherwise</returns>
    public bool IsSecure()
    {
        return string.Equals(Scheme, "https", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the URL is from the specified domain
    /// </summary>
    /// <param name="domain">The domain to check</param>
    /// <returns>True if the URL is from the specified domain</returns>
    public bool IsFromDomain(string domain)
    {
        Guard.NotNullOrWhiteSpace(domain);
        return string.Equals(Host, domain.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the URL matches the specified pattern
    /// </summary>
    /// <param name="pattern">The pattern to match (supports wildcards)</param>
    /// <returns>True if the URL matches the pattern</returns>
    public bool MatchesPattern(string pattern)
    {
        Guard.NotNullOrWhiteSpace(pattern);
        
        // Simple wildcard matching - can be enhanced with regex if needed
        pattern = pattern.Replace("*", ".*");
        return System.Text.RegularExpressions.Regex.IsMatch(Value, pattern, 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Combines this URL with a relative path
    /// </summary>
    /// <param name="relativePath">The relative path to combine</param>
    /// <returns>A new URL with the combined path</returns>
    public Url Combine(string relativePath)
    {
        Guard.NotNullOrWhiteSpace(relativePath);
        var combinedUri = new Uri(Uri, relativePath);
        return new Url(combinedUri);
    }

    /// <summary>
    /// Adds query parameters to the URL
    /// </summary>
    /// <param name="parameters">The query parameters to add</param>
    /// <returns>A new URL with the added query parameters</returns>
    public Url AddQueryParameters(Dictionary<string, string> parameters)
    {
        Guard.NotNull(parameters);
        
        if (parameters.Count == 0)
            return this;

        var uriBuilder = new UriBuilder(Uri);
        var queryParams = new List<string>();
        
        // Add existing query parameters
        if (!string.IsNullOrEmpty(uriBuilder.Query))
        {
            queryParams.Add(uriBuilder.Query.TrimStart('?'));
        }
        
        // Add new parameters
        foreach (var param in parameters)
        {
            var encodedKey = System.Web.HttpUtility.UrlEncode(param.Key);
            var encodedValue = System.Web.HttpUtility.UrlEncode(param.Value);
            queryParams.Add($"{encodedKey}={encodedValue}");
        }
        
        uriBuilder.Query = string.Join("&", queryParams);
        return new Url(uriBuilder.Uri);
    }

    /// <summary>
    /// Validates and normalizes the URL value
    /// </summary>
    /// <param name="value">The URL string to validate</param>
    /// <returns>The validated and normalized URL string</returns>
    private static string ValidateAndNormalize(string value)
    {
        Guard.NotNullOrWhiteSpace(value);
        
        var trimmedValue = value.Trim();
        
        if (!Uri.TryCreate(trimmedValue, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException($"Invalid URL format: {value}", nameof(value));
        }
        
        // Ensure it's a web URL (HTTP or HTTPS)
        if (uri.Scheme != "http" && uri.Scheme != "https")
        {
            throw new ArgumentException($"URL must use HTTP or HTTPS scheme: {value}", nameof(value));
        }
        
        return uri.ToString();
    }

    /// <summary>
    /// Implicit conversion from string to Url
    /// </summary>
    /// <param name="url">The URL object</param>
    public static implicit operator Uri(Url url) => url.Uri;

    /// <summary>
    /// Returns the string representation of the URL
    /// </summary>
    public override string ToString() => Value;
} 