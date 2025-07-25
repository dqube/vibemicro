namespace BuildingBlocks.Infrastructure.Messaging.Serialization;

/// <summary>
/// Interface for message serialization and deserialization
/// </summary>
public interface IMessageSerializer
{
    /// <summary>
    /// Serializes an object to a byte array
    /// </summary>
    /// <param name="obj">The object to serialize</param>
    /// <returns>The serialized byte array</returns>
    byte[] Serialize(object obj);

    /// <summary>
    /// Deserializes a byte array to an object of the specified type
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="data">The serialized data</param>
    /// <returns>The deserialized object</returns>
    T Deserialize<T>(byte[] data);

    /// <summary>
    /// Deserializes a byte array to an object of the specified type
    /// </summary>
    /// <param name="data">The serialized data</param>
    /// <param name="type">The type to deserialize to</param>
    /// <returns>The deserialized object</returns>
    object Deserialize(byte[] data, Type type);

    /// <summary>
    /// Gets the content type for this serializer
    /// </summary>
    string ContentType { get; }
} 