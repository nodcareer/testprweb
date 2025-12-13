# Components Documentation

## 1. HomeController

**Location:** `Controllers/HomeController.cs`

**Purpose:** Handles file management operations for Azure Blob Storage

**Dependencies:**
- `BlobStorageService` - File operations
- `ILogger<HomeController>` - Logging

**Actions:**

### Index() - GET
Returns a view displaying all files in blob container.
```csharp
public async Task<IActionResult> Index()
```
- **Returns:** View with `List<string>` of file names
- **Container:** "uploads"

### UploadFile(IFormFile file) - POST
Uploads a new file to blob storage.
```csharp
public async Task<IActionResult> UploadFile(IFormFile file)
```
- **Parameters:** `file` (IFormFile from form upload)
- **Validation:** File required, size > 0
- **Response:** Redirects to Index with success message
- **Errors:** Shows error message on form

### DeleteFile(string fileName) - POST
Deletes a file from blob container.
```csharp
public async Task<IActionResult> DeleteFile(string fileName)
```
- **Parameters:** `fileName` (blob name to delete)
- **Response:** Redirects to Index with success/error message

---

## 2. OrderController

**Location:** `Controllers/OrderController.cs`

**Purpose:** Manages order creation and viewing with database and queue integration

**Dependencies:**
- `ApplicationDbContext` - Database operations
- `IQueueService` - Message publishing
- `ILogger<OrderController>` - Logging

**Actions:**

### Index() - GET
Displays all orders sorted by creation date (newest first).
```csharp
public IActionResult Index()
```
- **Returns:** View with `List<Order>`
- **Sorting:** By CreatedAt descending
- **Status Badges:** Color-coded (Pending/Processing/Completed)

### Create() - GET
Displays order creation form.
```csharp
public IActionResult Create()
```
- **Returns:** Empty Order form view

### Create(Order order) - POST
Creates new order and publishes to queue.
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Order order)
```
- **Parameters:** Order model from form
- **Validation:** ModelState check
- **Process:**
  1. Save order to database with "Pending" status
  2. Publish OrderMessage to queue
  3. Redirect to Index with success message
- **Error Handling:** Logs errors, shows validation messages

### Details(int id) - GET
Displays detailed order information.
```csharp
public IActionResult Details(int id)
```
- **Parameters:** Order ID from route
- **Returns:** Order details view
- **Error:** NotFound if order doesn't exist

---

## 3. BlobStorageService

**Location:** `Services/BlobStorageService.cs`

**Purpose:** Manages Azure Blob Storage operations

**Key Methods:**

### UploadFileAsync()
```csharp
public async Task UploadFileAsync(IFormFile file, string containerName)
```
- **Parameters:**
  - `file` - IFormFile from upload
  - `containerName` - Target container (e.g., "uploads")
- **Process:**
  1. Create container if not exists
  2. Get blob client for filename
  3. Upload file stream with overwrite
  4. Log success
- **Throws:** Exception on failure

### ListBlobsAsync()
```csharp
public async Task<List<string>> ListBlobsAsync(string containerName)
```
- **Parameters:** Target container name
- **Returns:** List of blob names
- **Behavior:** Returns empty list if container doesn't exist
- **Throws:** Exception on error

### DeleteBlobAsync()
```csharp
public async Task DeleteBlobAsync(string blobName, string containerName)
```
- **Parameters:** Blob name and container
- **Process:**
  1. Get container and blob clients
  2. Delete blob
  3. Log deletion
- **Throws:** Exception on failure

---

## 4. QueueService

**Location:** `Services/QueueService.cs`

**Purpose:** Publishes order messages to Azure Storage Queue

**Dependencies:**
- `QueueServiceClient` - Azure SDK
- `ILogger<QueueService>` - Logging

**Configuration:**
- **Queue Name:** "orders"
- **Message Format:** JSON serialized OrderMessage

**Key Methods:**

### SendOrderMessageAsync()
```csharp
public async Task SendOrderMessageAsync(OrderMessage message)
```
- **Parameters:** OrderMessage object
- **Process:**
  1. Ensure queue exists
  2. Serialize message to JSON
  3. Send message to queue
  4. Log message ID
- **Throws:** Exception on failure

---

## 5. ApplicationDbContext

**Location:** `Data/ApplicationDbContext.cs`

**Purpose:** Entity Framework Core context for database operations

**DbSets:**
- `DbSet<Order> Orders` - Order records

**Configuration:**

### Order Entity Mapping
```csharp
modelBuilder.Entity<Order>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
    entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
    entity.Property(e => e.Total).HasPrecision(18, 2);
    entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
    // ... timestamps
});
```

---

## 6. Order Domain Model

**Location:** `Domain/Order/Order.cs`

**Purpose:** Entity model representing an order

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| Id | int | Primary key (auto-increment) |
| CustomerName | string | Customer full name |
| ProductName | string | Product name/description |
| Total | decimal | Order total amount (2 decimal places) |
| Status | string | Order status (Pending/Processing/Completed/Failed) |
| CreatedAt | DateTime | Creation timestamp (UTC) |
| UpdatedAt | DateTime? | Last update timestamp (UTC, nullable) |

**Status Lifecycle:**
```
Pending → Processing → Completed
       ↓
      Failed (error scenario)
```

---

## 7. OrderMessage Contract

**Location:** `Shared/Contracts/OrderMessage.cs`

**Purpose:** Serializable contract for queue messages

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| OrderId | int | Reference to Order.Id |
| CustomerName | string | Customer name (from Order) |
| ProductName | string | Product name (from Order) |
| Total | decimal | Total amount (from Order) |
| Status | string | Order status at time of message |
| CreatedAt | DateTime | Message creation timestamp |

**Usage:** Serialized to JSON for queue transport

---

## 8. OrderProcessorFunction

**Location:** `testpr.functions/OrderProcessorFunction.cs`

**Purpose:** Azure Function that processes orders from queue

**Trigger:** Queue message in "orders" queue

**Dependencies:**
- `OrderDbContext` - Database access
- `ILogger<OrderProcessorFunction>` - Logging

**Process Flow:**
1. **Trigger:** New message arrives in queue
2. **Deserialize:** Parse OrderMessage from JSON
3. **Lookup:** Find Order in database by OrderId
4. **Update:** Set status to "Processing", save
5. **Process:** Simulate processing work (await Task.Delay)
6. **Complete:** Set status to "Completed", save
7. **Log:** Log success or errors

**Error Handling:**
- Logs deserialization failures
- Handles missing orders gracefully
- Throws exception on unrecoverable errors

---

## 9. Views (Razor Components)

### Order/Index.cshtml
Displays table of all orders with status badges and action buttons.

**Features:**
- Responsive table design
- Status color-coding (Warning/Info/Success/Danger)
- Success message alerts
- "Create New Order" button

**Model:** `List<Order>`

### Order/Create.cshtml
Form for creating new orders.

**Fields:**
- Customer Name (text, required)
- Product Name (text, required)
- Total Amount (number, required, min=0)

**Features:**
- Bootstrap form validation
- Error display for validation failures
- Cancel button
- CSRF token protection

**Model:** `Order` (single)

### Order/Details.cshtml
Detailed view of a single order.

**Display:**
- Order ID
- Customer Name
- Product Name
- Total Amount
- Status badge
- Created At timestamp
- Updated At timestamp

**Navigation:** Back to Orders link

**Model:** `Order` (single)

### Home/Index.cshtml
File upload and listing interface.

**Features:**
- File input with drag-drop support
- File list with delete buttons
- Confirmation dialog for deletions
- Success/error alerts
- Empty state message

**Model:** `List<string>` (file names)

---

## 10. Program.cs (Startup Configuration)

**Location:** `testpr.web/Program.cs`

**Key Registrations:**

1. **Controllers & Views**
   ```csharp
   builder.Services.AddControllersWithViews();
   ```

2. **Blob Storage**
   ```csharp
   builder.Services.AddSingleton(x => new BlobServiceClient(connectionString));
   builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
   ```

3. **Queue Service**
   ```csharp
   builder.Services.AddSingleton(x => new QueueServiceClient(connectionString));
   builder.Services.AddScoped<IQueueService, QueueService>();
   ```

4. **Database Context**
   ```csharp
   builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(sqlConnectionString));
   ```

5. **Database Migrations**
   ```csharp
   using (var scope = app.Services.CreateScope())
   {
       var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
       dbContext.Database.Migrate();
   }
   ```

---

## Component Interaction Diagram

```
┌──────────────────┐
│ HomeController   │
└────────┬─────────┘
         │ uses
         ▼
┌──────────────────────────┐
│ BlobStorageService       │
└────────┬─────────────────┘
         │ calls
         ▼
┌──────────────────────────┐
│ BlobServiceClient (SDK)  │
└────────┬─────────────────┘
         │ REST API
         ▼
┌──────────────────────────┐
│ Azure Blob Storage       │
└──────────────────────────┘

┌──────────────────┐
│ OrderController  │
└────────┬─────────┘
         │ uses
         ├─────────────────────────┬──────────────────────┐
         ▼                         ▼                      ▼
┌─────────────────┐      ┌──────────────────┐   ┌────────────────┐
│ AppDbContext    │      │ QueueService     │   │ ILogger        │
└────────┬────────┘      └────────┬─────────┘   └────────────────┘
         │                        │
         │                        │ uses
         │                        ▼
         │               ┌──────────────────────┐
         │               │ QueueServiceClient   │
         │               └────────┬─────────────┘
         │                        │ REST API
         ▼                        ▼
┌──────────────────┐      ┌──────────────────────┐
│ Azure SQL DB     │      │ Azure Storage Queue  │
└──────────────────┘      └────────┬─────────────┘
                                   │
                                   │ Trigger
                                   ▼
                          ┌──────────────────────┐
                          │ OrderProcessorFunc   │
                          └────────┬─────────────┘
                                   │ uses
                                   ▼
                          ┌──────────────────────┐
                          │ AppDbContext (Func)  │
                          └────────┬─────────────┘
                                   │
                                   ▼
                          ┌──────────────────────┐
                          │ Azure SQL DB         │
                          └──────────────────────┘
```
