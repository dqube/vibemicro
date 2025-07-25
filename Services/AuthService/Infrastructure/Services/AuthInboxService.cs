using BuildingBlocks.Application.Inbox;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Services;

/// <summary>
/// AuthService implementation of IInboxService using EntityFramework
/// </summary>
public class AuthInboxService : IInboxService
{
    private readonly AuthDbContext _context;
    private readonly ILogger<AuthInboxService> _logger;

    public AuthInboxService(AuthDbContext context, ILogger<AuthInboxService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddMessageAsync(InboxMessage message, CancellationToken cancellationToken = default)
    {
        _context.InboxMessages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogDebug("Added inbox message {MessageId} of type {MessageType}", 
            message.Id, message.MessageType);
    }

    public async Task<IEnumerable<InboxMessage>> GetPendingMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await _context.InboxMessages
            .Where(m => m.Status == InboxMessageStatus.Pending)
            .OrderBy(m => m.ReceivedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> MessageExistsAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return await _context.InboxMessages.AnyAsync(m => m.Id == messageId, cancellationToken);
    }

    public async Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _context.InboxMessages.FindAsync(new object[] { messageId }, cancellationToken);
        if (message != null)
        {
            message.MarkAsProcessed();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default)
    {
        var message = await _context.InboxMessages.FindAsync(new object[] { messageId }, cancellationToken);
        if (message != null)
        {
            message.MarkAsFailed(error);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> RetryFailedMessagesAsync(int maxRetryCount = 3, CancellationToken cancellationToken = default)
    {
        var failedMessages = await _context.InboxMessages
            .Where(m => m.Status == InboxMessageStatus.Failed && m.RetryCount < maxRetryCount)
            .ToListAsync(cancellationToken);

        foreach (var message in failedMessages)
        {
            message.ResetForRetry();
        }

        if (failedMessages.Any())
        {
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Reset {Count} failed inbox messages for retry", failedMessages.Count);
        }

        return failedMessages.Count;
    }

    public async Task<int> CleanupOldMessagesAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var oldMessages = await _context.InboxMessages
            .Where(m => m.Status == InboxMessageStatus.Processed && m.ProcessedAt < olderThan)
            .ToListAsync(cancellationToken);

        if (oldMessages.Any())
        {
            _context.InboxMessages.RemoveRange(oldMessages);
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Cleaned up {Count} old inbox messages", oldMessages.Count);
        }

        return oldMessages.Count;
    }
} 