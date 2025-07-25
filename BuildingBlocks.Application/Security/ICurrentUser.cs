namespace BuildingBlocks.Application.Security;

/// <summary>
/// Interface for accessing current user information
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets the current user identifier
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the current user name
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Gets the current user email
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets the current user roles
    /// </summary>
    IEnumerable<string> Roles { get; }

    /// <summary>
    /// Gets the current user claims
    /// </summary>
    IEnumerable<UserClaim> Claims { get; }

    /// <summary>
    /// Gets the tenant identifier for the current user
    /// </summary>
    string? TenantId { get; }

    /// <summary>
    /// Gets whether the user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Checks if the current user has a specific role
    /// </summary>
    /// <param name="role">The role to check</param>
    /// <returns>True if the user has the role</returns>
    bool IsInRole(string role);

    /// <summary>
    /// Checks if the current user has any of the specified roles
    /// </summary>
    /// <param name="roles">The roles to check</param>
    /// <returns>True if the user has any of the roles</returns>
    bool IsInAnyRole(params string[] roles);

    /// <summary>
    /// Checks if the current user has all of the specified roles
    /// </summary>
    /// <param name="roles">The roles to check</param>
    /// <returns>True if the user has all of the roles</returns>
    bool IsInAllRoles(params string[] roles);

    /// <summary>
    /// Gets a claim value by type
    /// </summary>
    /// <param name="claimType">The claim type</param>
    /// <returns>The claim value if found</returns>
    string? GetClaimValue(string claimType);

    /// <summary>
    /// Gets all claim values by type
    /// </summary>
    /// <param name="claimType">The claim type</param>
    /// <returns>The claim values</returns>
    IEnumerable<string> GetClaimValues(string claimType);

    /// <summary>
    /// Checks if the current user has a specific claim
    /// </summary>
    /// <param name="claimType">The claim type</param>
    /// <param name="claimValue">The claim value</param>
    /// <returns>True if the user has the claim</returns>
    bool HasClaim(string claimType, string? claimValue = null);
}

/// <summary>
/// Represents a user claim
/// </summary>
public class UserClaim
{
    /// <summary>
    /// Gets or sets the claim type
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the claim value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the UserClaim class
    /// </summary>
    public UserClaim()
    {
    }

    /// <summary>
    /// Initializes a new instance of the UserClaim class
    /// </summary>
    /// <param name="type">The claim type</param>
    /// <param name="value">The claim value</param>
    public UserClaim(string type, string value)
    {
        Type = type;
        Value = value;
    }

    /// <summary>
    /// Returns the string representation of the claim
    /// </summary>
    public override string ToString()
    {
        return $"{Type}: {Value}";
    }
} 