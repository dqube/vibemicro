using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Infrastructure.Serialization.Json;

/// <summary>
/// JSON serialization service implementation
/// </summary>
public class JsonSerializationService : IJsonSerializationService
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initializes a new instance of the JsonSerializationService class
    /// </summary>
    /// <param name="options">Optional JSON serializer options</param>
    public JsonSerializationService(JsonSerializerOptions? options = null)
    {
        _options = options ?? CreateDefaultOptions();
    }

    /// <summary>
    /// Serializes an object to a JSON string
    /// </summary>
    /// <typeparam name="T">The type of the object</typeparam>
    /// <param name="obj">The object to serialize</param>
    /// <returns>The JSON string</returns>
    public string Serialize<T>(T obj)
    {
        if (obj == null)
            return "null";

        return JsonSerializer.Serialize(obj, _options);
    }

    /// <summary>
    /// Serializes an object to a JSON byte array
    /// </summary>
    /// <typeparam name="T">The type of the object</typeparam>
    /// <param name="obj">The object to serialize</param>
    /// <returns>The JSON byte array</returns>
    public byte[] SerializeToBytes<T>(T obj)
    {
        if (obj == null)
            return "null"u8.ToArray();

        return JsonSerializer.SerializeToUtf8Bytes(obj, _options);
    }

    /// <summary>
    /// Deserializes a JSON string to an object
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="json">The JSON string</param>
    /// <returns>The deserialized object</returns>
    public T? Deserialize<T>(string json)
    {
        if (string.IsNullOrEmpty(json) || json == "null")
            return default;

        return JsonSerializer.Deserialize<T>(json, _options);
    }

    /// <summary>
    /// Deserializes a JSON byte array to an object
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="jsonBytes">The JSON byte array</param>
    /// <returns>The deserialized object</returns>
    public T? Deserialize<T>(byte[] jsonBytes)
    {
        if (jsonBytes == null || jsonBytes.Length == 0)
            return default;

        return JsonSerializer.Deserialize<T>(jsonBytes, _options);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type
    /// </summary>
    /// <param name="json">The JSON string</param>
    /// <param name="type">The type to deserialize to</param>
    /// <returns>The deserialized object</returns>
    public object? Deserialize(string json, Type type)
    {
        if (string.IsNullOrEmpty(json) || json == "null")
            return null;

        return JsonSerializer.Deserialize(json, type, _options);
    }

    /// <summary>
    /// Deserializes a JSON byte array to an object of the specified type
    /// </summary>
    /// <param name="jsonBytes">The JSON byte array</param>
    /// <param name="type">The type to deserialize to</param>
    /// <returns>The deserialized object</returns>
    public object? Deserialize(byte[] jsonBytes, Type type)
    {
        if (jsonBytes == null || jsonBytes.Length == 0)
            return null;

        return JsonSerializer.Deserialize(jsonBytes, type, _options);
    }

    /// <summary>
    /// Creates default JSON serializer options
    /// </summary>
    /// <returns>The default JSON serializer options</returns>
    private static JsonSerializerOptions CreateDefaultOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
    }
}

/// <summary>
/// Interface for JSON serialization service
/// </summary>
public interface IJsonSerializationService
{
    /// <summary>
    /// Serializes an object to a JSON string
    /// </summary>
    /// <typeparam name="T">The type of the object</typeparam>
    /// <param name="obj">The object to serialize</param>
    /// <returns>The JSON string</returns>
    string Serialize<T>(T obj);

    /// <summary>
    /// Serializes an object to a JSON byte array
    /// </summary>
    /// <typeparam name="T">The type of the object</typeparam>
    /// <param name="obj">The object to serialize</param>
    /// <returns>The JSON byte array</returns>
    byte[] SerializeToBytes<T>(T obj);

    /// <summary>
    /// Deserializes a JSON string to an object
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="json">The JSON string</param>
    /// <returns>The deserialized object</returns>
    T? Deserialize<T>(string json);

    /// <summary>
    /// Deserializes a JSON byte array to an object
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="jsonBytes">The JSON byte array</param>
    /// <returns>The deserialized object</returns>
    T? Deserialize<T>(byte[] jsonBytes);

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type
    /// </summary>
    /// <param name="json">The JSON string</param>
    /// <param name="type">The type to deserialize to</param>
    /// <returns>The deserialized object</returns>
    object? Deserialize(string json, Type type);

    /// <summary>
    /// Deserializes a JSON byte array to an object of the specified type
    /// </summary>
    /// <param name="jsonBytes">The JSON byte array</param>
    /// <param name="type">The type to deserialize to</param>
    /// <returns>The deserialized object</returns>
    object? Deserialize(byte[] jsonBytes, Type type);
} 