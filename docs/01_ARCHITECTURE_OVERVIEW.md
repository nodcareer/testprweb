# Architecture Overview

## Solution Architecture

This solution implements a distributed order management system with cloud-native services. It demonstrates integration between ASP.NET Core web application, Azure cloud services, and serverless computing.

```
┌─────────────────────┐
│   Web Browser       │
└──────────┬──────────┘
           │ HTTP/HTTPS
           ▼
┌─────────────────────────────────────┐
│   ASP.NET Core Web Application      │
│  - File Management (Blob Storage)   │
│  - Order Management (SQL Database)  │
└──────┬──────────────────────┬───────┘
       │                      │
       │ REST API Calls       │ Queue Messages
       │                      │
       ▼                      ▼
┌──────────────────┐  ┌──────────────────────┐
│  Azure Blob      │  │ Azure Storage Queue  │
│  Storage         │  │ (orders queue)       │
│                  │  └──────┬───────────────┘
│ - Containers     │         │
│ - File Upload    │         │ Queue Trigger
│ - File List      │         │
└──────────────────┘         ▼
                    ┌──────────────────────┐
                    │ Azure Function       │
                    │ OrderProcessor       │
                    └─────────┬────────────┘
                              │
                              │ SQL Operations
                              ▼
                    ┌──────────────────────┐
                    │ Azure SQL Database   │
                    │                      │
                    │ - Orders Table       │
                    │ - Blob References    │
                    └──────────────────────┘
```

## Core Components

### 1. Web Application (ASP.NET Core 9.0)
**Responsibilities:**
- User interface for file upload/management
- Order creation and viewing
- Database access via Entity Framework Core
- Queue message publishing

**Key Features:**
- Razor views with Bootstrap styling
- MVC pattern with controllers and services
- Dependency injection for loose coupling
- Entity Framework Core ORM

### 2. Azure Blob Storage
**Responsibilities:**
- File storage and retrieval
- Container management
- Access control and security

**Use Cases:**
- Document uploads (PDFs, images, etc.)
- Order attachments
- Backup storage

### 3. Azure SQL Database
**Responsibilities:**
- Persistent data storage
- Order records and metadata
- Transaction support

**Schema:**
- Orders table with customer, product, and status information

### 4. Azure Storage Queue
**Responsibilities:**
- Asynchronous message delivery
- Decoupling web app from order processing
- Message persistence and retry logic

**Message Contract:**
- OrderMessage: Contains order details for processing

### 5. Azure Function (OrderProcessor)
**Responsibilities:**
- Consume messages from queue
- Process orders asynchronously
- Update order status in database
- Error handling and logging

**Workflow:**
- Triggered by queue message
- Updates order status (Pending → Processing → Completed)
- Stores results in SQL Database

## Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Web Framework | ASP.NET Core | 9.0 |
| Language | C# | Latest |
| ORM | Entity Framework Core | 9.0.0 |
| Blob Storage SDK | Azure.Storage.Blobs | 12.20.0 |
| Queue SDK | Azure.Storage.Queues | 12.18.0 |
| SQL Server Driver | Microsoft.EntityFrameworkCore.SqlServer | 9.0.0 |
| Function Runtime | Azure Functions Worker | 1.21.0 |
| Frontend | Bootstrap 5 | Latest |

## Data Flow

### File Management Flow
1. User uploads file via web UI
2. Web app validates and saves to Blob Storage
3. File appears in file listing
4. User can delete files from Blob Storage

### Order Management Flow
1. User fills order creation form
2. Web app validates and saves Order to SQL Database (Status: Pending)
3. Web app publishes OrderMessage to Queue
4. Azure Function consumes message from queue
5. Function updates Order status to Processing
6. Function performs processing logic
7. Function updates Order status to Completed
8. User views order status on web UI

## Security Considerations

- Connection strings stored in appsettings.json (use Azure Key Vault in production)
- Managed authentication for Azure services
- HTTPS enforced for all endpoints
- CORS policies for API access (if needed)
- Role-based access control (optional)

## Scalability

- **Horizontal Scaling:** Multiple web app instances behind load balancer
- **Queue Buffering:** Storage queue handles traffic spikes
- **Function Auto-scaling:** Functions scale based on queue depth
- **Database Scaling:** Azure SQL elastic pools or serverless tier
- **Blob Storage:** Automatically scales to handle large files

## Error Handling & Resilience

- Exception logging and monitoring
- Queue dead-letter queue for failed messages
- Database transaction rollback on errors
- Retry logic in Azure Functions
- Health checks on critical services

## Cost Optimization

- Storage Queue: Pay per operation (cost-effective)
- Azure Functions: Consumption plan (pay per execution)
- SQL Database: Serverless tier for variable workloads
- Blob Storage: Hot/Cool/Archive tiers based on access patterns

## Monitoring & Diagnostics

- Application Insights integration (optional)
- Azure Storage Analytics
- Function execution logs
- Database query performance insights
- Error and exception tracking
