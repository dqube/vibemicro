using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Domain.StronglyTypedIds;

/// <summary>
/// JSON converter factory for strongly-typed identifiers
/// </summary>
public class StronglyTypedIdJsonConverterFactory : JsonConverterFactory
{
    /// <summary>
    /// Determines whether the specified type can be converted by this factory
    /// </summary>
    /// <param name="typeToConvert">The type to convert</param>
    /// <returns>True if the type can be converted</returns>
    public override bool CanConvert(Type typeToConvert)
    {
        // Check if the type implements IStronglyTypedId<T>
        return IsStronglyTypedId(typeToConvert);
    }

    /// <summary>
    /// Creates a converter for the specified type
    /// </summary>
    /// <param name="typeToConvert">The type to convert</param>
    /// <param name="options">The serializer options</param>
    /// <returns>A JSON converter for the specified type</returns>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (!IsStronglyTypedId(typeToConvert))
        {
            return null;
        }

        var valueType = GetStronglyTypedIdValueType(typeToConvert);
        if (valueType == null)
        {
            return null;
        }

        // Create the generic converter type
        var converterType = typeof(StronglyTypedIdJsonConverter<,>).MakeGenericType(typeToConvert, valueType);
        
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }

    /// <summary>
    /// Checks if a type is a strongly-typed identifier
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <returns>True if the type is a strongly-typed identifier</returns>
    private static bool IsStronglyTypedId(Type type)
    {
        if (!type.IsValueType)
        {
            return false;
        }

        // Check if the type implements IStronglyTypedId<T>
        var interfaces = type.GetInterfaces();
        return interfaces.Any(i => 
            i.IsGenericType && 
            i.GetGenericTypeDefinition() == typeof(IStronglyTypedId<>));
    }

    /// <summary>
    /// Gets the value type of a strongly-typed identifier
    /// </summary>
    /// <param name="stronglyTypedIdType">The strongly-typed identifier type</param>
    /// <returns>The value type, or null if not found</returns>
    private static Type? GetStronglyTypedIdValueType(Type stronglyTypedIdType)
    {
        var interfaces = stronglyTypedIdType.GetInterfaces();
        
        var stronglyTypedIdInterface = interfaces.FirstOrDefault(i => 
            i.IsGenericType && 
            i.GetGenericTypeDefinition() == typeof(IStronglyTypedId<>));

        return stronglyTypedIdInterface?.GetGenericArguments().FirstOrDefault();
    }
}

/// <summary>
/// Attribute to mark strongly-typed identifiers for automatic JSON conversion
/// </summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public class StronglyTypedIdAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the underlying value type
    /// </summary>
    public Type? ValueType { get; set; }

    /// <summary>
    /// Initializes a new instance of the StronglyTypedIdAttribute class
    /// </summary>
    public StronglyTypedIdAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the StronglyTypedIdAttribute class
    /// </summary>
    /// <param name="valueType">The underlying value type</param>
    public StronglyTypedIdAttribute(Type valueType)
    {
        ValueType = valueType;
    }
} 