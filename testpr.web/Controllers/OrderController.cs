using Microsoft.AspNetCore.Mvc;
using testpr.web.Data;
using testpr.web.Domain.Order;
using testpr.web.Services;
using testpr.web.Shared.Contracts;

namespace testpr.web.Controllers;

public class OrderController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IQueueService _queueService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        ApplicationDbContext dbContext,
        IQueueService queueService,
        ILogger<OrderController> logger)
    {
        _dbContext = dbContext;
        _queueService = queueService;
        _logger = logger;
    }

    // GET: Order
    public IActionResult Index()
    {
        var orders = _dbContext.Orders.OrderByDescending(o => o.CreatedAt).ToList();
        return View(orders);
    }

    // GET: Order/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Order/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Order order)
    {
        if (!ModelState.IsValid)
        {
            return View(order);
        }

        try
        {
            // Add order to database
            order.CreatedAt = DateTime.UtcNow;
            order.Status = "Pending";
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Order {order.Id} created for customer {order.CustomerName}");

            // Send message to queue for processing
            var orderMessage = new OrderMessage
            {
                OrderId = order.Id,
                CustomerName = order.CustomerName,
                ProductName = order.ProductName,
                Total = order.Total,
                Status = order.Status,
                CreatedAt = order.CreatedAt
            };

            await _queueService.SendOrderMessageAsync(orderMessage);

            TempData["SuccessMessage"] = $"Order {order.Id} created successfully and sent for processing!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating order: {ex.Message}");
            ModelState.AddModelError("", "An error occurred while creating the order. Please try again.");
            return View(order);
        }
    }

    // GET: Order/Details/5
    public IActionResult Details(int id)
    {
        var order = _dbContext.Orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }
}
