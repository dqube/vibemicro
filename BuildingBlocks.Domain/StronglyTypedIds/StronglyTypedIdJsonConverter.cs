using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Domain.StronglyTypedIds;

/// <summary>
/// JSON converter for strongly-typed identifiers
/// </summary>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
/// <typeparam name="TValue">The underlying value type</typeparam>
public class StronglyTypedIdJsonConverter<TId, TValue> : JsonConverter<TId>
    where TId : struct, IStronglyTypedId<TValue>
    where TValue : notnull
{
    /// <summary>
    /// Reads and converts the JSON to a strongly-typed identifier
    /// </summary>
    /// <param name="reader">The reader</param>
    /// <param name="typeToConvert">The type to convert</param>
    /// <param name="options">The serializer options</param>
    /// <returns>The strongly-typed identifier</returns>
    public override TId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException($"Cannot convert null to {typeToConvert.Name}");
        }

        var value = JsonSerializer.Deserialize<TValue>(ref reader, options);
        if (value == null)
        {
            throw new JsonException($"Cannot convert null value to {typeToConvert.Name}");
        }

        return CreateInstance(value);
    }

    /// <summary>
    /// Writes the strongly-typed identifier as JSON
    /// </summary>
    /// <param name="writer">The writer</param>
    /// <param name="value">The value to write</param>
    /// <param name="options">The serializer options</param>
    public override void Write(Utf8JsonWriter writer, TId value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    /// <summary>
    /// Creates an instance of the strongly-typed identifier
    /// </summary>
    /// <param name="value">The underlying value</param>
    /// <returns>The strongly-typed identifier instance</returns>
    private static TId CreateInstance(TValue value)
    {
        try
        {
            // Try to use Activator.CreateInstance first (fastest)
            return (TId)Activator.CreateInstance(typeof(TId), value)!;
        }
        catch
        {
            // Fallback to reflection-based construction
            var constructor = typeof(TId).GetConstructor(new[] { typeof(TValue) });
            if (constructor == null)
            {
                throw new InvalidOperationException($"No constructor found for {typeof(TId).Name} that accepts {typeof(TValue).Name}");
            }

            return (TId)constructor.Invoke(new object[] { value });
        }
    }
}

/// <summary>
/// Non-generic JSON converter for strongly-typed identifiers
/// </summary>
public class StronglyTypedIdJsonConverter : JsonConverter<object>
{
    private readonly Type _idType;
    private readonly Type _valueType;
    private readonly JsonConverter _innerConverter;

    /// <summary>
    /// Initializes a new instance of the StronglyTypedIdJsonConverter class
    /// </summary>
    /// <param name="idType">The strongly-typed identifier type</param>
    /// <param name="valueType">The underlying value type</param>
    public StronglyTypedIdJsonConverter(Type idType, Type valueType)
    {
        _idType = idType;
        _valueType = valueType;
        
        var converterType = typeof(StronglyTypedIdJsonConverter<,>).MakeGenericType(idType, valueType);
        _innerConverter = (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    /// <summary>
    /// Determines whether the specified type can be converted
    /// </summary>
    /// <param name="typeToConvert">The type to convert</param>
    /// <returns>True if the type can be converted</returns>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == _idType;
    }

    /// <summary>
    /// Reads and converts the JSON to a strongly-typed identifier
    /// </summary>
    /// <param name="reader">The reader</param>
    /// <param name="typeToConvert">The type to convert</param>
    /// <param name="options">The serializer options</param>
    /// <returns>The strongly-typed identifier</returns>
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var readMethod = _innerConverter.GetType().GetMethod("Read");
        if (readMethod == null)
        {
            throw new InvalidOperationException($"Read method not found on converter for {typeToConvert.Name}");
        }

        var result = readMethod.Invoke(_innerConverter, new object[] { reader, typeToConvert, options });
        return result ?? throw new JsonException($"Failed to deserialize {typeToConvert.Name}");
    }

    /// <summary>
    /// Writes the strongly-typed identifier as JSON
    /// </summary>
    /// <param name="writer">The writer</param>
    /// <param name="value">The value to write</param>
    /// <param name="options">The serializer options</param>
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        var writeMethod = _innerConverter.GetType().GetMethod("Write");
        if (writeMethod == null)
        {
            throw new InvalidOperationException($"Write method not found on converter for {value.GetType().Name}");
        }

        writeMethod.Invoke(_innerConverter, new object[] { writer, value, options });
    }
} 