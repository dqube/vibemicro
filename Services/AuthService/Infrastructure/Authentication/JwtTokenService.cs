using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Authentication;

/// <summary>
/// Service for generating and validating JWT tokens
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT token for the user
    /// </summary>
    /// <param name="user">The user</param>
    /// <param name="rememberMe">Whether to create a long-lived token</param>
    /// <returns>The JWT token</returns>
    string GenerateToken(User user, bool rememberMe = false);

    /// <summary>
    /// Validates a JWT token
    /// </summary>
    /// <param name="token">The token to validate</param>
    /// <returns>The claims principal if valid, null otherwise</returns>
    ClaimsPrincipal? ValidateToken(string token);
}

/// <summary>
/// JWT token service implementation
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;
    private readonly int _rememberMeExpirationDays;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        _issuer = _configuration["JwtSettings:Issuer"] ?? "AuthService";
        _audience = _configuration["JwtSettings:Audience"] ?? "AuthService.Users";
        _expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 60);
        _rememberMeExpirationDays = _configuration.GetValue<int>("JwtSettings:RememberMeExpirationDays", 7);
    }

    public string GenerateToken(User user, bool rememberMe = false)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
            new(ClaimTypes.Name, user.Username.Value),
            new(ClaimTypes.Email, user.Email.Value),
            new("username", user.Username.Value),
            new("email", user.Email.Value),
            new("isActive", user.IsActive.ToString()),
            new("jti", Guid.NewGuid().ToString())
        };

        // Add role claims
        foreach (var roleId in user.RoleIds)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleId.Value.ToString()));
        }

        var expiration = rememberMe 
            ? DateTime.UtcNow.AddDays(_rememberMeExpirationDays)
            : DateTime.UtcNow.AddMinutes(_expirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
} 