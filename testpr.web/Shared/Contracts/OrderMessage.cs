namespace testpr.web.Shared.Contracts;

/// <summary>
/// Order message contract for queue communication
/// </summary>
public class OrderMessage
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
