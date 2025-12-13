using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace testpr.functions;

public class OrderProcessorFunction
{
    private readonly OrderDbContext _dbContext;
    private readonly ILogger<OrderProcessorFunction> _logger;

    public OrderProcessorFunction(OrderDbContext dbContext, ILogger<OrderProcessorFunction> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [Function("OrderProcessor")]
    public async Task Run(
        [QueueTrigger("orders", Connection = "AzureWebJobsStorage")] string queueMessage,
        FunctionContext context)
    {
        _logger.LogInformation($"Processing order message: {queueMessage}");

        try
        {
            // Deserialize the message
            var orderMessage = JsonSerializer.Deserialize<OrderMessage>(queueMessage);
            
            if (orderMessage == null)
            {
                _logger.LogError("Failed to deserialize order message");
                return;
            }

            // Find the order in database
            var order = _dbContext.Orders.FirstOrDefault(o => o.Id == orderMessage.OrderId);
            
            if (order == null)
            {
                _logger.LogError($"Order {orderMessage.OrderId} not found");
                return;
            }

            // Process the order (simulate processing)
            _logger.LogInformation($"Processing order {order.Id} for {order.CustomerName}");
            
            // Update status to Processing
            order.Status = "Processing";
            order.UpdatedAt = DateTime.UtcNow;
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();

            // Simulate some processing work
            await Task.Delay(2000);

            // Mark as completed
            order.Status = "Completed";
            order.UpdatedAt = DateTime.UtcNow;
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Order {order.Id} processed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing order: {ex.Message}");
            throw;
        }
    }
}

public class OrderMessage
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}
