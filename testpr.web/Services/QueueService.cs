using Azure.Storage.Queues;
using System.Text.Json;
using testpr.web.Shared.Contracts;

namespace testpr.web.Services;

public interface IQueueService
{
    Task SendOrderMessageAsync(OrderMessage message);
}

public class QueueService : IQueueService
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<QueueService> _logger;
    private const string QueueName = "orders";

    public QueueService(QueueServiceClient queueServiceClient, ILogger<QueueService> logger)
    {
        _logger = logger;
        _queueClient = queueServiceClient.GetQueueClient(QueueName);
    }

    /// <summary>
    /// Sends an order message to Azure Storage Queue
    /// </summary>
    public async Task SendOrderMessageAsync(OrderMessage message)
    {
        try
        {
            // Ensure queue exists
            await _queueClient.CreateIfNotExistsAsync();

            // Serialize message to JSON
            var messageJson = JsonSerializer.Serialize(message);

            // Send message
            await _queueClient.SendMessageAsync(messageJson);

            _logger.LogInformation($"Order message {message.OrderId} sent to queue");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending order message: {ex.Message}");
            throw;
        }
    }
}
