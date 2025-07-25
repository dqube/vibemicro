using System.Text.Json;
using BuildingBlocks.Domain.StronglyTypedIds;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Domain.Extensions;

/// <summary>
/// Extension methods for JSON configuration with strongly-typed identifiers
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Adds strongly-typed identifier JSON converters to JsonSerializerOptions
    /// </summary>
    /// <param name="options">The JSON serializer options</param>
    /// <returns>The updated JSON serializer options</returns>
    public static JsonSerializerOptions AddStronglyTypedIdConverters(this JsonSerializerOptions options)
    {
        options.Converters.Add(new StronglyTypedIdJsonConverterFactory());
        return options;
    }

    /// <summary>
    /// Configures JSON options for ASP.NET Core with strongly-typed identifier support
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Optional additional configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection ConfigureJsonOptionsForStronglyTypedIds(
        this IServiceCollection services,
        Action<JsonSerializerOptions>? configureOptions = null)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.AddStronglyTypedIdConverters();
            configureOptions?.Invoke(options.SerializerOptions);
        });

        services.Configure<JsonSerializerOptions>(options =>
        {
            options.AddStronglyTypedIdConverters();
            configureOptions?.Invoke(options);
        });

        return services;
    }

    /// <summary>
    /// Creates a JsonSerializerOptions instance configured for strongly-typed identifiers
    /// </summary>
    /// <param name="configureOptions">Optional additional configuration</param>
    /// <returns>Configured JsonSerializerOptions</returns>
    public static JsonSerializerOptions CreateOptionsForStronglyTypedIds(
        Action<JsonSerializerOptions>? configureOptions = null)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        options.AddStronglyTypedIdConverters();
        configureOptions?.Invoke(options);

        return options;
    }

    /// <summary>
    /// Adds a specific strongly-typed identifier converter
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type</typeparam>
    /// <typeparam name="TValue">The underlying value type</typeparam>
    /// <param name="options">The JSON serializer options</param>
    /// <returns>The updated JSON serializer options</returns>
    public static JsonSerializerOptions AddStronglyTypedIdConverter<TId, TValue>(this JsonSerializerOptions options)
        where TId : struct, IStronglyTypedId<TValue>
        where TValue : notnull
    {
        options.Converters.Add(new StronglyTypedIdJsonConverter<TId, TValue>());
        return options;
    }

    /// <summary>
    /// Serializes an object to JSON with strongly-typed identifier support
    /// </summary>
    /// <typeparam name="T">The type to serialize</typeparam>
    /// <param name="value">The value to serialize</param>
    /// <param name="options">Optional JSON serializer options</param>
    /// <returns>The JSON string</returns>
    public static string SerializeWithStronglyTypedIds<T>(T value, JsonSerializerOptions? options = null)
    {
        options ??= CreateOptionsForStronglyTypedIds();
        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Deserializes JSON to an object with strongly-typed identifier support
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="json">The JSON string</param>
    /// <param name="options">Optional JSON serializer options</param>
    /// <returns>The deserialized object</returns>
    public static T? DeserializeWithStronglyTypedIds<T>(string json, JsonSerializerOptions? options = null)
    {
        options ??= CreateOptionsForStronglyTypedIds();
        return JsonSerializer.Deserialize<T>(json, options);
    }

    /// <summary>
    /// Deserializes JSON to an object with strongly-typed identifier support
    /// </summary>
    /// <param name="json">The JSON string</param>
    /// <param name="returnType">The type to deserialize to</param>
    /// <param name="options">Optional JSON serializer options</param>
    /// <returns>The deserialized object</returns>
    public static object? DeserializeWithStronglyTypedIds(string json, Type returnType, JsonSerializerOptions? options = null)
    {
        options ??= CreateOptionsForStronglyTypedIds();
        return JsonSerializer.Deserialize(json, returnType, options);
    }
} 