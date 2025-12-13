# Azure SQL Database Setup and Order Workflow

## Prerequisites
- Azure Storage Account (for blobs and queues)
- Azure SQL Database
- .NET 9.0 SDK

## Step 1: Update Connection Strings
Update `appsettings.json` with your Azure resources:

```json
{
  "ConnectionStrings": {
    "AzureBlobStorage": "DefaultEndpointsProtocol=https;AccountName=<your-storage>;AccountKey=<your-key>;EndpointSuffix=core.windows.net",
    "AzureStorageQueue": "DefaultEndpointsProtocol=https;AccountName=<your-storage>;AccountKey=<your-key>;EndpointSuffix=core.windows.net",
    "AzureSqlDatabase": "Server=tcp:<server>.database.windows.net,1433;Initial Catalog=<database>;Persist Security Info=False;User ID=<user>;Password=<password>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

## Step 2: Create Database Migrations
Run these commands from the web project directory:

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

This will create the Orders table in your Azure SQL Database.

## Step 3: Build and Run Web Application
```powershell
dotnet build
dotnet run
```

The web app will be available at `https://localhost:7xxx`

## Step 4: Create and Process Orders
1. Navigate to `/order` in the browser
2. Click "Create New Order"
3. Fill in: Customer Name, Product Name, Total Amount
4. Submit the form
5. The order is saved to the database and a message is sent to the Azure Storage Queue

## Step 5: Deploy Azure Function
The Azure Function `OrderProcessorFunction` processes messages from the queue:

1. Reads the order message from the "orders" queue
2. Updates the order status to "Processing"
3. Simulates processing work
4. Updates the order status to "Completed"
5. Stores changes back to the Azure SQL Database

To deploy the function:
```powershell
cd testpr.functions
func azure functionapp publish <your-function-app-name>
```

## Project Structure

```
testpr.web/
├── Controllers/
│   ├── HomeController.cs (File management)
│   └── OrderController.cs (Order creation and viewing)
├── Views/
│   ├── Home/
│   │   └── Index.cshtml (File upload/listing)
│   └── Order/
│       ├── Index.cshtml (Order listing)
│       ├── Create.cshtml (Create order form)
│       └── Details.cshtml (Order details)
├── Domain/
│   └── Order/
│       └── Order.cs (Order entity model)
├── Data/
│   └── ApplicationDbContext.cs (EF Core DbContext)
├── Services/
│   ├── BlobStorageService.cs (Blob storage operations)
│   └── QueueService.cs (Queue messaging)
├── Shared/
│   └── Contracts/
│       └── OrderMessage.cs (Queue message contract)
└── appsettings.json (Configuration)

testpr.functions/
├── OrderProcessorFunction.cs (Queue trigger function)
├── Program.cs (Function startup)
└── local.settings.json (Local development settings)
```

## Workflow Summary

1. **User creates an order** via `/order/create`
2. **Web app saves order** to Azure SQL Database with "Pending" status
3. **Web app sends message** to Azure Storage Queue with order details
4. **Azure Function** is triggered by the queue message
5. **Function processes the order**:
   - Updates status to "Processing"
   - Performs processing logic
   - Updates status to "Completed"
6. **User can view order status** at `/order/` or `/order/details/[id]`

## Monitoring

- Check database: Query `Orders` table in your Azure SQL Database
- Check queue: View messages in the "orders" queue in your Storage Account
- Check function logs: View function execution logs in Azure Portal
