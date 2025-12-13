# Project Structure

## Directory Layout

```
contweb/
├── docs/
│   ├── 01_ARCHITECTURE_OVERVIEW.md      # System architecture and design
│   ├── 02_PROJECT_STRUCTURE.md          # Directory layout (this file)
│   ├── 03_COMPONENTS.md                 # Detailed component documentation
│   ├── 04_DATA_FLOW.md                  # Workflow and data flow diagrams
│   ├── 05_SETUP_GUIDE.md                # Setup and configuration
│   └── 06_DEPLOYMENT.md                 # Deployment instructions
│
├── testpr.web/                          # ASP.NET Core Web Application
│   ├── Controllers/                     # MVC Controllers
│   │   ├── HomeController.cs            # File management controller
│   │   └── OrderController.cs           # Order management controller
│   │
│   ├── Views/                           # Razor view templates
│   │   ├── Home/
│   │   │   └── Index.cshtml             # File upload and list UI
│   │   ├── Order/
│   │   │   ├── Index.cshtml             # Order list view
│   │   │   ├── Create.cshtml            # Order creation form
│   │   │   └── Details.cshtml           # Order details view
│   │   └── Shared/
│   │       ├── _Layout.cshtml           # Master layout
│   │       ├── _Layout.cshtml.css       # Layout styles
│   │       └── Error.cshtml             # Error page
│   │
│   ├── Controllers/                     # MVC Controllers
│   │   ├── HomeController.cs            # Home page and files
│   │   └── OrderController.cs           # Order management
│   │
│   ├── Domain/                          # Domain models
│   │   └── Order/
│   │       └── Order.cs                 # Order entity model
│   │
│   ├── Data/                            # Data access layer
│   │   └── ApplicationDbContext.cs      # Entity Framework DbContext
│   │
│   ├── Models/                          # View models
│   │   └── ErrorViewModel.cs            # Error display model
│   │
│   ├── Services/                        # Business logic services
│   │   ├── BlobStorageService.cs        # Blob storage operations
│   │   ├── QueueService.cs              # Queue messaging service
│   │   └── IBlobStorageService.cs       # (interface removed - refactor)
│   │
│   ├── Shared/                          # Shared contracts
│   │   └── Contracts/
│   │       └── OrderMessage.cs          # Queue message contract
│   │
│   ├── Properties/
│   │   └── launchSettings.json          # Launch configuration
│   │
│   ├── wwwroot/                         # Static files
│   │   ├── css/
│   │   │   └── site.css                 # Custom styles
│   │   ├── js/
│   │   │   └── site.js                  # Custom scripts
│   │   └── lib/                         # Third-party libraries
│   │       ├── bootstrap/               # Bootstrap CSS framework
│   │       ├── jquery/                  # jQuery library
│   │       └── jquery-validation/       # Form validation
│   │
│   ├── bin/                             # Build output (gitignore)
│   │   ├── Debug/
│   │   └── Release/
│   │
│   ├── obj/                             # Build artifacts (gitignore)
│   │
│   ├── appsettings.json                 # Configuration (development)
│   ├── appsettings.Development.json     # Development overrides
│   ├── Dockerfile                       # Docker build configuration
│   ├── Program.cs                       # Application startup
│   └── testpr.web.csproj                # Project file with NuGet references
│
├── testpr.functions/                    # Azure Functions Project
│   ├── OrderProcessorFunction.cs        # Queue trigger function
│   ├── Program.cs                       # Function runtime startup
│   ├── local.settings.json              # Local development settings
│   └── testpr.functions.csproj          # Project file
│
├── contweb.sln                          # Visual Studio solution file
├── readme.txt                           # Project readme
└── SETUP_GUIDE.md                       # Quick start guide

```

## Directory Responsibilities

### `/docs` - Documentation
Contains all technical documentation including architecture, setup, and deployment guides.

### `/testpr.web` - Web Application
Main ASP.NET Core application serving the user interface and REST API.

**Key subdirectories:**
- `Controllers/` - HTTP request handlers
- `Views/` - Razor templates for HTML rendering
- `Services/` - Business logic and external service integration
- `Domain/` - Entity models (Order, etc.)
- `Data/` - Database context and migrations
- `Shared/` - Shared contracts between web app and functions

### `/testpr.functions` - Azure Functions
Serverless components for background processing and event handling.

**Key files:**
- `OrderProcessorFunction.cs` - Processes orders from queue
- `Program.cs` - Dependency injection and runtime configuration

## File Type Organization

### Controllers
- `*Controller.cs` - Inherits from `Controller` base class
- Handles HTTP requests and returns views/responses
- Uses dependency injection for services

### Views
- `*.cshtml` - Razor syntax for server-side HTML rendering
- Located in `Views/[ControllerName]/` directories
- Share common layout in `Views/Shared/`

### Domain Models
- Located in `Domain/[EntityName]/`
- Represent database entities
- Include validation and business logic

### Services
- `I*Service.cs` - Interface definitions
- `*Service.cs` - Implementation classes
- Handle external integrations (Blob, Queue, Database)

### Configuration
- `appsettings.json` - Environment-agnostic settings
- `appsettings.Development.json` - Development overrides
- `local.settings.json` - Azure Function local settings

## Naming Conventions

| Type | Naming Pattern | Example |
|------|---|---|
| Classes | PascalCase | `OrderController`, `BlobStorageService` |
| Methods | PascalCase | `CreateOrderAsync()`, `ListBlobsAsync()` |
| Properties | PascalCase | `CustomerName`, `TotalAmount` |
| Private fields | _camelCase | `_logger`, `_dbContext` |
| Parameters | camelCase | `customerId`, `orderMessage` |
| Constants | UPPER_SNAKE_CASE | `QUEUE_NAME = "orders"` |

## Dependency Graph

```
HomeController
  └── BlobStorageService
      └── BlobServiceClient

OrderController
  ├── ApplicationDbContext
  │   └── Order entity
  └── QueueService
      └── QueueServiceClient

Program.cs
  ├── BlobServiceClient
  ├── QueueServiceClient
  ├── ApplicationDbContext
  └── ILogger
```

## Build Output

- **Debug:** `bin/Debug/net9.0/`
- **Release:** `bin/Release/net9.0/`

Generated files:
- `testpr.web.dll` - Main assembly
- `appsettings.json` - Configuration (copied)
- `testpr.web.staticwebassets.runtime.json` - Static asset manifest

## Configuration Files

| File | Purpose | Scope |
|------|---------|-------|
| `appsettings.json` | Base configuration | All environments |
| `appsettings.Development.json` | Dev overrides | Development only |
| `launchSettings.json` | VS launch profiles | Development |
| `testpr.web.csproj` | Project metadata | Build system |
| `Dockerfile` | Container image | Production deployment |

## Git Ignore Patterns

Typically excluded from version control:
- `/bin/` - Build output
- `/obj/` - Build artifacts
- `.vs/` - Visual Studio cache
- `.user` - User-specific settings
- `appsettings.*.local.json` - Local secrets
- `local.settings.json` - Function local settings
