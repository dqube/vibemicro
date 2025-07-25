using System.Text;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Messaging.Serialization;

/// <summary>
/// JSON implementation of message serializer
/// </summary>
public class JsonMessageSerializer : IMessageSerializer
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Gets the content type for JSON serialization
    /// </summary>
    public string ContentType => "application/json";

    /// <summary>
    /// Initializes a new instance of the JsonMessageSerializer class
    /// </summary>
    /// <param name="options">Optional JSON serializer options</param>
    public JsonMessageSerializer(JsonSerializerOptions? options = null)
    {
        _options = options ?? new JsonSerializerOptions
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

        var json = JsonSerializer.Serialize(obj, _options);
        return Encoding.UTF8.GetBytes(json);
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

        var json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<T>(json, _options)!;
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

        var json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize(json, type, _options)!;
    }
} 