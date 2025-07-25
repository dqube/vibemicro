namespace BuildingBlocks.Infrastructure.Security.Encryption;

/// <summary>
/// Interface for encryption and decryption operations
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts the specified plain text
    /// </summary>
    /// <param name="plainText">The plain text to encrypt</param>
    /// <returns>The encrypted text</returns>
    string Encrypt(string plainText);

    /// <summary>
    /// Decrypts the specified encrypted text
    /// </summary>
    /// <param name="encryptedText">The encrypted text to decrypt</param>
    /// <returns>The decrypted plain text</returns>
    string Decrypt(string encryptedText);

    /// <summary>
    /// Encrypts the specified byte array
    /// </summary>
    /// <param name="data">The data to encrypt</param>
    /// <returns>The encrypted data</returns>
    byte[] Encrypt(byte[] data);

    /// <summary>
    /// Decrypts the specified encrypted byte array
    /// </summary>
    /// <param name="encryptedData">The encrypted data to decrypt</param>
    /// <returns>The decrypted data</returns>
    byte[] Decrypt(byte[] encryptedData);

    /// <summary>
    /// Generates a new encryption key
    /// </summary>
    /// <returns>The generated encryption key</returns>
    string GenerateKey();

    /// <summary>
    /// Validates if the provided key is valid for this encryption service
    /// </summary>
    /// <param name="key">The key to validate</param>
    /// <returns>True if the key is valid</returns>
    bool ValidateKey(string key);
} 