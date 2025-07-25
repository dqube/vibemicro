using BuildingBlocks.Application.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Application.Services;

/// <summary>
/// Background service for processing outbox messages
/// </summary>
public class OutboxBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OutboxBackgroundService> _logger;
    private readonly OutboxBackgroundServiceOptions _options;

    /// <summary>
    /// Initializes a new instance of the OutboxBackgroundService class
    /// </summary>
    public OutboxBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<OutboxBackgroundService> logger,
        IOptions<OutboxBackgroundServiceOptions> options)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Executes the background service
    /// </summary>
    /// <param name="stoppingToken">The stopping token</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var outboxProcessor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();

                var processedCount = await outboxProcessor.ProcessPendingMessagesAsync(stoppingToken);

                if (processedCount > 0)
                {
                    _logger.LogDebug("Processed {ProcessedCount} outbox messages", processedCount);
                }

                // Cleanup old messages if enabled
                if (_options.EnableCleanup)
                {
                    var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
                    var cleanupThreshold = DateTime.UtcNow.Subtract(_options.CleanupThreshold);
                    var cleanedCount = await outboxService.CleanupOldMessagesAsync(cleanupThreshold, stoppingToken);

                    if (cleanedCount > 0)
                    {
                        _logger.LogDebug("Cleaned up {CleanedCount} old outbox messages", cleanedCount);
                    }
                }

                // Retry failed messages if enabled
                if (_options.EnableRetry)
                {
                    var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
                    var retriedCount = await outboxService.RetryFailedMessagesAsync(_options.MaxRetryAttempts, stoppingToken);

                    if (retriedCount > 0)
                    {
                        _logger.LogDebug("Retried {RetriedCount} failed outbox messages", retriedCount);
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error processing outbox messages: {Error}", ex.Message);
            }

            try
            {
                await Task.Delay(_options.ProcessingInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("Outbox background service stopped");
    }
}

/// <summary>
/// Configuration options for outbox background service
/// </summary>
public class OutboxBackgroundServiceOptions
{
    /// <summary>
    /// Gets or sets the processing interval
    /// </summary>
    public TimeSpan ProcessingInterval { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets whether to enable automatic cleanup of old messages
    /// </summary>
    public bool EnableCleanup { get; set; } = true;

    /// <summary>
    /// Gets or sets the threshold for cleaning up old messages
    /// </summary>
    public TimeSpan CleanupThreshold { get; set; } = TimeSpan.FromDays(7);

    /// <summary>
    /// Gets or sets whether to enable retry of failed messages
    /// </summary>
    public bool EnableRetry { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;
} 