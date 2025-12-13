using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Add DbContext with connection string from environment
        var sqlConnectionString = Environment.GetEnvironmentVariable("SqlConnectionString") 
            ?? "Server=.;Database=testprdb;Integrated Security=true;TrustServerCertificate=true;";
        
        services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlServer(sqlConnectionString));

        services.AddLogging();
    })
    .Build();

host.Run();

// Simplified DbContext for the function
public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
    
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Order>().HasKey(e => e.Id);
    }
}

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
