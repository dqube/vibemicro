using BuildingBlocks.Application.Inbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Application.Services;

/// <summary>
/// Background service for processing inbox messages
/// </summary>
public class InboxBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<InboxBackgroundService> _logger;
    private readonly InboxBackgroundServiceOptions _options;

    /// <summary>
    /// Initializes a new instance of the InboxBackgroundService class
    /// </summary>
    public InboxBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<InboxBackgroundService> logger,
        IOptions<InboxBackgroundServiceOptions> options)
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
        _logger.LogInformation("Inbox background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var inboxProcessor = scope.ServiceProvider.GetRequiredService<IInboxProcessor>();

                var processedCount = await inboxProcessor.ProcessPendingMessagesAsync(stoppingToken);

                if (processedCount > 0)
                {
                    _logger.LogDebug("Processed {ProcessedCount} inbox messages", processedCount);
                }

                // Process ordered messages if enabled
                if (_options.EnableOrderedProcessing && _options.OrderedMessageGroups?.Any() == true)
                {
                    foreach (var messageGroup in _options.OrderedMessageGroups)
                    {
                        if (stoppingToken.IsCancellationRequested)
                            break;

                        var orderedProcessedCount = await inboxProcessor.ProcessOrderedMessagesAsync(messageGroup, stoppingToken);

                        if (orderedProcessedCount > 0)
                        {
                            _logger.LogDebug("Processed {ProcessedCount} ordered messages for group {MessageGroup}",
                                orderedProcessedCount, messageGroup);
                        }
                    }
                }

                // Cleanup old messages if enabled
                if (_options.EnableCleanup)
                {
                    var inboxService = scope.ServiceProvider.GetRequiredService<IInboxService>();
                    var cleanupThreshold = DateTime.UtcNow.Subtract(_options.CleanupThreshold);
                    var cleanedCount = await inboxService.CleanupOldMessagesAsync(cleanupThreshold, stoppingToken);

                    if (cleanedCount > 0)
                    {
                        _logger.LogDebug("Cleaned up {CleanedCount} old inbox messages", cleanedCount);
                    }
                }

                // Retry failed messages if enabled
                if (_options.EnableRetry)
                {
                    var inboxService = scope.ServiceProvider.GetRequiredService<IInboxService>();
                    var retriedCount = await inboxService.RetryFailedMessagesAsync(_options.MaxRetryAttempts, stoppingToken);

                    if (retriedCount > 0)
                    {
                        _logger.LogDebug("Retried {RetriedCount} failed inbox messages", retriedCount);
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error processing inbox messages: {Error}", ex.Message);
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

        _logger.LogInformation("Inbox background service stopped");
    }
}

/// <summary>
/// Configuration options for inbox background service
/// </summary>
public class InboxBackgroundServiceOptions
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

    /// <summary>
    /// Gets or sets whether to enable ordered message processing
    /// </summary>
    public bool EnableOrderedProcessing { get; set; } = false;

    /// <summary>
    /// Gets or sets the message groups for ordered processing
    /// </summary>
    public List<string> OrderedMessageGroups { get; set; } = new();
} 