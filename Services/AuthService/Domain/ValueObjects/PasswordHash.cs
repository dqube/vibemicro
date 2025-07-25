using BuildingBlocks.Domain.ValueObjects;

namespace AuthService.Domain.ValueObjects;

/// <summary>
/// Password hash value object containing both hash and salt
/// </summary>
public sealed record PasswordHash : ValueObject
{
    /// <summary>
    /// Gets the password hash bytes
    /// </summary>
    public byte[] Hash { get; }

    /// <summary>
    /// Gets the password salt bytes
    /// </summary>
    public byte[] Salt { get; }

    /// <summary>
    /// Initializes a new instance of the PasswordHash class
    /// </summary>
    /// <param name="hash">The password hash bytes</param>
    /// <param name="salt">The password salt bytes</param>
    public PasswordHash(byte[] hash, byte[] salt)
    {
        Hash = hash ?? throw new ArgumentNullException(nameof(hash));
        Salt = salt ?? throw new ArgumentNullException(nameof(salt));

        if (hash.Length == 0)
            throw new ArgumentException("Password hash cannot be empty.", nameof(hash));

        if (salt.Length == 0)
            throw new ArgumentException("Password salt cannot be empty.", nameof(salt));
    }

    /// <summary>
    /// Creates a new password hash from raw bytes
    /// </summary>
    /// <param name="hash">The password hash bytes</param>
    /// <param name="salt">The password salt bytes</param>
    /// <returns>A new PasswordHash instance</returns>
    public static PasswordHash From(byte[] hash, byte[] salt) => new(hash, salt);

    /// <summary>
    /// Creates a new password hash from base64 encoded strings
    /// </summary>
    /// <param name="hashBase64">The base64 encoded hash</param>
    /// <param name="saltBase64">The base64 encoded salt</param>
    /// <returns>A new PasswordHash instance</returns>
    public static PasswordHash FromBase64(string hashBase64, string saltBase64)
    {
        if (string.IsNullOrWhiteSpace(hashBase64))
            throw new ArgumentException("Hash base64 string cannot be null or empty.", nameof(hashBase64));

        if (string.IsNullOrWhiteSpace(saltBase64))
            throw new ArgumentException("Salt base64 string cannot be null or empty.", nameof(saltBase64));

        try
        {
            var hash = Convert.FromBase64String(hashBase64);
            var salt = Convert.FromBase64String(saltBase64);
            return new PasswordHash(hash, salt);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Invalid base64 format for password hash or salt.", ex);
        }
    }

    /// <summary>
    /// Gets the hash as a base64 encoded string
    /// </summary>
    /// <returns>Base64 encoded hash</returns>
    public string GetHashBase64() => Convert.ToBase64String(Hash);

    /// <summary>
    /// Gets the salt as a base64 encoded string
    /// </summary>
    /// <returns>Base64 encoded salt</returns>
    public string GetSaltBase64() => Convert.ToBase64String(Salt);

    /// <summary>
    /// Verifies if a plain text password matches this hash
    /// </summary>
    /// <param name="password">The plain text password to verify</param>
    /// <returns>True if the password matches</returns>
    public bool VerifyPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        // This is a simplified example - in production, use proper password hashing
        // like BCrypt, Argon2, or PBKDF2
        using var hmac = new System.Security.Cryptography.HMACSHA512(Salt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(Hash);
    }

    /// <summary>
    /// Creates a password hash from a plain text password
    /// </summary>
    /// <param name="password">The plain text password</param>
    /// <returns>A new PasswordHash instance</returns>
    public static PasswordHash CreateHash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));

        // Generate a random salt
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

        return new PasswordHash(hash, salt);
    }

    /// <summary>
    /// Gets the equality components for value object comparison
    /// </summary>
    /// <returns>Enumerable of equality components</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hash;
        yield return Salt;
    }
} 