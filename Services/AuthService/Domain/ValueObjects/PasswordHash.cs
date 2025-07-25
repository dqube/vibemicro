using BuildingBlocks.Domain.ValueObjects;

namespace AuthService.Domain.ValueObjects;

/// <summary>
/// Value object representing a password hash with salt
/// </summary>
internal sealed record PasswordHash(byte[] Hash, byte[] Salt) : ValueObject
{
    /// <summary>
    /// Gets the password hash with validation
    /// </summary>
    public byte[] Hash { get; init; } = ValidateHash(Hash);

    /// <summary>
    /// Gets the password salt with validation
    /// </summary>
    public byte[] Salt { get; init; } = ValidateSalt(Salt);

    /// <summary>
    /// Validates the password hash
    /// </summary>
    /// <param name="hash">The password hash</param>
    /// <returns>The validated hash</returns>
    private static byte[] ValidateHash(byte[] hash)
    {
        if (hash == null)
            throw new ArgumentNullException(nameof(hash));

        if (hash.Length == 0)
            throw new ArgumentException("Password hash cannot be empty", nameof(hash));

        return hash;
    }

    /// <summary>
    /// Validates the password salt
    /// </summary>
    /// <param name="salt">The password salt</param>
    /// <returns>The validated salt</returns>
    private static byte[] ValidateSalt(byte[] salt)
    {
        if (salt == null)
            throw new ArgumentNullException(nameof(salt));

        if (salt.Length == 0)
            throw new ArgumentException("Password salt cannot be empty", nameof(salt));

        return salt;
    }

    /// <summary>
    /// Creates a new password hash from base64 encoded strings
    /// </summary>
    /// <param name="hashBase64">The base64 encoded hash</param>
    /// <param name="saltBase64">The base64 encoded salt</param>
    /// <returns>A new PasswordHash instance</returns>
    public static PasswordHash FromBase64(string hashBase64, string saltBase64)
    {
        if (string.IsNullOrWhiteSpace(hashBase64))
            throw new ArgumentException("Hash base64 string cannot be null or empty", nameof(hashBase64));

        if (string.IsNullOrWhiteSpace(saltBase64))
            throw new ArgumentException("Salt base64 string cannot be null or empty", nameof(saltBase64));

        try
        {
            var hash = Convert.FromBase64String(hashBase64);
            var salt = Convert.FromBase64String(saltBase64);
            return new PasswordHash(hash, salt);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Invalid base64 format", ex);
        }
    }

    /// <summary>
    /// Gets the hash as a base64 encoded string
    /// </summary>
    /// <returns>The base64 encoded hash</returns>
    public string GetHashAsBase64() => Convert.ToBase64String(Hash);

    /// <summary>
    /// Gets the salt as a base64 encoded string
    /// </summary>
    /// <returns>The base64 encoded salt</returns>
    public string GetSaltAsBase64() => Convert.ToBase64String(Salt);

    /// <summary>
    /// Verifies if the provided password matches this hash
    /// </summary>
    /// <param name="password">The password to verify</param>
    /// <param name="hashProvider">The hash provider to use for verification</param>
    /// <returns>True if the password matches</returns>
    public bool VerifyPassword(string password, IPasswordHashProvider hashProvider)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        if (hashProvider == null)
            throw new ArgumentNullException(nameof(hashProvider));

        return hashProvider.VerifyPassword(password, Hash, Salt);
    }

    /// <summary>
    /// Gets the atomic values that define the value object's equality
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Hash;
        yield return Salt;
    }
}

/// <summary>
/// Interface for password hashing providers
/// </summary>
internal interface IPasswordHashProvider
{
    /// <summary>
    /// Creates a password hash with salt
    /// </summary>
    /// <param name="password">The password to hash</param>
    /// <returns>The password hash</returns>
    PasswordHash CreateHash(string password);

    /// <summary>
    /// Verifies a password against a hash and salt
    /// </summary>
    /// <param name="password">The password to verify</param>
    /// <param name="hash">The stored hash</param>
    /// <param name="salt">The stored salt</param>
    /// <returns>True if the password is valid</returns>
    bool VerifyPassword(string password, byte[] hash, byte[] salt);
} 