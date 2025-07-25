using BuildingBlocks.Application.Outbox;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Services;

/// <summary>
/// AuthService implementation of IOutboxService using EntityFramework
/// </summary>
public class AuthOutboxService : IOutboxService
{
    private readonly AuthDbContext _context;
    private readonly ILogger<AuthOutboxService> _logger;

    public AuthOutboxService(AuthDbContext context, ILogger<AuthOutboxService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddMessageAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        _context.OutboxMessages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogDebug("Added outbox message {MessageId} of type {MessageType}", 
            message.Id, message.MessageType);
    }

    public async Task<IEnumerable<OutboxMessage>> GetPendingMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await _context.OutboxMessages
            .Where(m => m.Status == OutboxMessageStatus.Pending)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _context.OutboxMessages.FindAsync(new object[] { messageId }, cancellationToken);
        if (message != null)
        {
            message.MarkAsProcessed();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default)
    {
        var message = await _context.OutboxMessages.FindAsync(new object[] { messageId }, cancellationToken);
        if (message != null)
        {
            message.MarkAsFailed(error);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> RetryFailedMessagesAsync(int maxRetryCount = 3, CancellationToken cancellationToken = default)
    {
        var failedMessages = await _context.OutboxMessages
            .Where(m => m.Status == OutboxMessageStatus.Failed && m.RetryCount < maxRetryCount)
            .ToListAsync(cancellationToken);

        foreach (var message in failedMessages)
        {
            message.ResetForRetry();
        }

        if (failedMessages.Any())
        {
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Reset {Count} failed outbox messages for retry", failedMessages.Count);
        }

        return failedMessages.Count;
    }

    public async Task<int> CleanupOldMessagesAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var oldMessages = await _context.OutboxMessages
            .Where(m => m.Status == OutboxMessageStatus.Processed && m.ProcessedAt < olderThan)
            .ToListAsync(cancellationToken);

        if (oldMessages.Any())
        {
            _context.OutboxMessages.RemoveRange(oldMessages);
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Cleaned up {Count} old outbox messages", oldMessages.Count);
        }

        return oldMessages.Count;
    }
} 