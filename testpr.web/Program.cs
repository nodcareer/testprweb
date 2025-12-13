using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using testpr.web.Data;
using testpr.web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Azure Blob Storage
var blobConnectionString = builder.Configuration.GetConnectionString("AzureBlobStorage");
builder.Services.AddSingleton(x => new BlobServiceClient(blobConnectionString));
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

// Register Azure Storage Queues
var queueConnectionString = builder.Configuration.GetConnectionString("AzureStorageQueue");
builder.Services.AddSingleton(x => new QueueServiceClient(queueConnectionString));
builder.Services.AddScoped<IQueueService, QueueService>();

// Register Entity Framework and SQL Database
var sqlConnectionString = builder.Configuration.GetConnectionString("AzureSqlDatabase");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(sqlConnectionString));

var app = builder.Build();

// Apply database migrations on startup
/*using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
