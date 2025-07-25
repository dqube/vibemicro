namespace BuildingBlocks.Infrastructure.Messaging.Configuration;

/// <summary>
/// Configuration options for message bus
/// </summary>
public class MessageBusConfiguration
{
    /// <summary>
    /// Gets or sets the message bus provider type
    /// </summary>
    public MessageBusProvider Provider { get; set; } = MessageBusProvider.InMemory;

    /// <summary>
    /// Gets or sets the connection string for the message bus
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the default message timeout
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the maximum retry attempts
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the retry delay
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets whether to enable dead letter queues
    /// </summary>
    public bool EnableDeadLetterQueue { get; set; } = true;

    /// <summary>
    /// Gets or sets the dead letter queue name
    /// </summary>
    public string DeadLetterQueueName { get; set; } = "dead-letter";

    /// <summary>
    /// Gets or sets the default exchange name (for RabbitMQ)
    /// </summary>
    public string? DefaultExchange { get; set; }

    /// <summary>
    /// Gets or sets the default routing key (for RabbitMQ)
    /// </summary>
    public string? DefaultRoutingKey { get; set; }

    /// <summary>
    /// Gets or sets additional provider-specific properties
    /// </summary>
    public Dictionary<string, string> Properties { get; set; } = new();

    /// <summary>
    /// Gets or sets whether to enable message compression
    /// </summary>
    public bool EnableCompression { get; set; } = false;

    /// <summary>
    /// Gets or sets the compression threshold in bytes
    /// </summary>
    public int CompressionThreshold { get; set; } = 1024;

    /// <summary>
    /// Gets or sets the serializer type to use
    /// </summary>
    public MessageSerializerType SerializerType { get; set; } = MessageSerializerType.Json;
}

/// <summary>
/// Supported message bus providers
/// </summary>
public enum MessageBusProvider
{
    /// <summary>
    /// In-memory message bus for testing and local development
    /// </summary>
    InMemory = 0,

    /// <summary>
    /// Azure Service Bus
    /// </summary>
    ServiceBus = 1,

    /// <summary>
    /// RabbitMQ message broker
    /// </summary>
    RabbitMQ = 2,

    /// <summary>
    /// Apache Kafka
    /// </summary>
    Kafka = 3,

    /// <summary>
    /// Redis Streams
    /// </summary>
    Redis = 4
}

/// <summary>
/// Supported message serializer types
/// </summary>
public enum MessageSerializerType
{
    /// <summary>
    /// JSON serialization
    /// </summary>
    Json = 0,

    /// <summary>
    /// Binary serialization
    /// </summary>
    Binary = 1,

    /// <summary>
    /// MessagePack serialization
    /// </summary>
    MessagePack = 2,

    /// <summary>
    /// Protocol Buffers serialization
    /// </summary>
    ProtocolBuffers = 3
} 