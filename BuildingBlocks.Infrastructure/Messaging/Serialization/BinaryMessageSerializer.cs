using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Messaging.Serialization;

/// <summary>
/// Binary implementation of message serializer using System.Text.Json as the underlying serializer
/// Note: This uses JSON serialization with compression for safety and cross-platform compatibility
/// </summary>
public class BinaryMessageSerializer : IMessageSerializer
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Gets the content type for binary serialization
    /// </summary>
    public string ContentType => "application/octet-stream";

    /// <summary>
    /// Initializes a new instance of the BinaryMessageSerializer class
    /// </summary>
    public BinaryMessageSerializer()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Serializes an object to a byte array
    /// </summary>
    /// <param name="obj">The object to serialize</param>
    /// <returns>The serialized byte array</returns>
    public byte[] Serialize(object obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        // Use JSON serialization for safety and cross-platform compatibility
        var json = JsonSerializer.SerializeToUtf8Bytes(obj, _options);
        
        // Optionally compress the data
        return CompressData(json);
    }

    /// <summary>
    /// Deserializes a byte array to an object of the specified type
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="data">The serialized data</param>
    /// <returns>The deserialized object</returns>
    public T Deserialize<T>(byte[] data)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("Data cannot be null or empty", nameof(data));

        // Decompress the data
        var decompressedData = DecompressData(data);
        
        return JsonSerializer.Deserialize<T>(decompressedData, _options)!;
    }

    /// <summary>
    /// Deserializes a byte array to an object of the specified type
    /// </summary>
    /// <param name="data">The serialized data</param>
    /// <param name="type">The type to deserialize to</param>
    /// <returns>The deserialized object</returns>
    public object Deserialize(byte[] data, Type type)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("Data cannot be null or empty", nameof(data));

        if (type == null)
            throw new ArgumentNullException(nameof(type));

        // Decompress the data
        var decompressedData = DecompressData(data);
        
        return JsonSerializer.Deserialize(decompressedData, type, _options)!;
    }

    /// <summary>
    /// Compresses data using GZip compression
    /// </summary>
    /// <param name="data">The data to compress</param>
    /// <returns>The compressed data</returns>
    private static byte[] CompressData(byte[] data)
    {
        using var output = new MemoryStream();
        using var gzip = new System.IO.Compression.GZipStream(output, System.IO.Compression.CompressionMode.Compress);
        gzip.Write(data, 0, data.Length);
        gzip.Close();
        return output.ToArray();
    }

    /// <summary>
    /// Decompresses data using GZip decompression
    /// </summary>
    /// <param name="compressedData">The compressed data</param>
    /// <returns>The decompressed data</returns>
    private static byte[] DecompressData(byte[] compressedData)
    {
        using var input = new MemoryStream(compressedData);
        using var gzip = new System.IO.Compression.GZipStream(input, System.IO.Compression.CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return output.ToArray();
    }
} 