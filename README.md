# Vehicle Management System (VMAPP)

A web-based Vehicle Management System built with ASP.NET Core (.NET 8)
and Razor Pages / MVC.  
The application helps manage vehicles, their service history,
insurance/inspection data, and related records.

---

## Features

### Vehicle Management

- Create, edit, and delete vehicle entries
- Store core vehicle information: VIN, brand, model, year, color, type

### Service Management

- List available vehicle services
- Filter vehicles by selected service
- Assign vehicles to services

### Service Records

- Add, edit, and delete per-vehicle service records
- Track service date, cost, and description
- Modal-based UI with DataTables for a responsive service history table

### UI / UX

- Fixed header and footer with consistent navigation
- Global background image
- Content displayed in centered white cards for readability
- Bootstrap-based layout
- jQuery DataTables integration for tabular data

### Privacy & Terms

- Dedicated Privacy Policy page (`/Home/Privacy`)
- Dedicated Terms and Conditions page (`/Home/Terms`)
- Both rendered in readable white content boxes

---

## Architecture & Project Structure

The solution is split into clear layers with Dependency Injection (DI) and services.
Controllers do **not** work with `DbContext` directly; instead they depend on
services (interfaces) and map **DTOs → ViewModels**.

### Projects

- `VMAPP.Web`  
  ASP.NET Core MVC / Razor app (presentation layer)
- `VMAPP.Services`  
  Application/service layer  business logic and data access orchestration
- `VMAPP.Data`  
  Data access layer  EF Core `DbContext` and entity models

### VMAPP.Data

- `ApplicationDbContext`  EF Core context:
  - `DbSet<Vehicle>`
  - `DbSet<ServiceRecord>`
  - `DbSet<VehicleService>`
  - `DbSet<VehicleVehicleService>`
- Fluent configuration for relationships and decimal precision (e.g. `ServiceRecord.Cost` as `decimal(18,2)`)

---

### VMAPP.Services

This layer owns all `ApplicationDbContext` usage. Controllers never see `DbContext`.

#### DTOs (`VMAPP.Services/DTOs`)

- `VehicleServiceDto`  
  Basic info for a service:
  - `Id`, `Name`, `City`, `Address`, `Email`, `Phone`, `Description`, `CreatedOn`
- `VehicleDto`  
  Vehicle info:
  - `Id`, `VIN`, `CarBrand`, `CarModel`, `CreatedOnYear`, `Color`, `VehicleType` (as `int`)
- `ServiceRecordDto`  
  Service record info:
  - `Id`, `VehicleId`, `ServiceDate`, `Cost`, `Description`
- `ServiceWithVehiclesDto`  
  A service plus its vehicles:
  - `Id`, `Name`, `List<VehicleDto> Vehicles`
- `VehicleDetailsDto`  
  Detailed view for one vehicle:
  - `VehicleDto Vehicle`
  - `List<ServiceRecordDto> ServiceRecords`

#### Service Interfaces (`VMAPP.Services/Interfaces`)

- `IVSManagementService`  
  (in code: `IVehicleServiceManagementService`)  
  Manages `VehicleService` entities (service stations):
  - `Task<IReadOnlyList<VehicleServiceDto>> GetAllAsync()`
  - `Task<VehicleServiceDto?> GetByIdAsync(int id)`
  - `Task<int> CreateAsync(VehicleServiceDto dto)`
  - `Task<bool> UpdateAsync(VehicleServiceDto dto)`
  - `Task<bool> DeleteAsync(int id)`

- `IVSCarsService`  
  (in code: `IVehicleServiceCarsService`)  
  Manages vehicles and their service records for a given service:
  - `Task<ServiceWithVehiclesDto?> GetServiceWithVehiclesByNameAsync(string serviceName)`
  - `Task<int> AddVehicleToServiceAsync(int serviceId, VehicleDto vehicle)`
  - `Task<VehicleDetailsDto?> GetVehicleDetailsAsync(int vehicleId)`
  - `Task<bool> UpdateVehicleAsync(VehicleDto vehicle)`
  - `Task<int> AddServiceRecordAsync(ServiceRecordDto record)`
  - `Task<bool> UpdateServiceRecordAsync(ServiceRecordDto record)`
  - `Task<bool> DeleteServiceRecordAsync(int recordId)`
  - `Task<bool> DeleteVehicleAsync(int vehicleId)`

#### Service Implementations

- `VSManagementService`  
  (in code: `VehicleServiceManagementService`)  implements `IVSManagementService`  
  Responsibilities:
  - CRUD for `VehicleService` (`VehicleServices` table)
  - All EF work happens here (no controllers touch `DbContext`)
  - Maps EF entities ↔ `VehicleServiceDto`

- `VSCarsService`  
  (in code: `VehicleServiceCarsService`)  implements `IVSCarsService`  
  Responsibilities:
  - Load a service with all its vehicles (`ServiceWithVehiclesDto`)
  - Add/update/delete vehicles (`VehicleDto`)
  - Load full vehicle details with records (`VehicleDetailsDto`)
  - Add/update/delete service records (`ServiceRecordDto`)
  - All EF work encapsulated here

> **Important:** Only the service layer touches `ApplicationDbContext`.  
> Controllers only see interfaces + DTOs and map them to ViewModels.

---

### VMAPP.Web

ASP.NET Core MVC project with controllers, view models, and views.

#### Dependency Injection (`Program.cs`)

```
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Application services
builder.Services.AddScoped<IVSManagementService, VSManagementService>();
builder.Services.AddScoped<IVSCarsService, VSCarsService>();
```

Also configures `RequestLocalizationOptions` to use `en-US` for consistent decimal
handling (for costs like `10.05`).

#### Controllers

- `ManagementController`
  - Injects `IVSManagementService`
  - Uses `VehicleServiceDto` for data, maps to:
    - `EditViewModel`
    - `AddServiceViewModel`
  - No direct `DbContext` usage.

- `VehicleServiceCarsController`
  - Injects `IVSCarsService`
  - Key mappings:
    - `ServiceWithVehiclesDto` → `ServiceIndexViewModel`
    - `VehicleDto` ↔ `AddVehicleViewModel` / `EditVehicleViewModel`
    - `VehicleDetailsDto` → `EditVehicleViewModel` + `ServiceRecords`
    - `ServiceRecordDto` ↔ `AddRecordViewModel`
  - No direct `DbContext` usage.

- `HomeController`
  - Home / Privacy / Terms / Error pages.

#### View Models (examples)

- `VMAPP.Web/Models/VehicleServiceModels/EditViewModel`  
  For editing `VehicleService` in Management.
- `VMAPP.Web/Models/VehicleServiceCars/*`  
  For:
  - Listing services and vehicles
  - Editing a vehicle and its service history
- `VMAPP.Web/Models/ServiceRecordModels/AddRecordViewModel`  
  For adding/editing service records.

---

## Tech Stack

### Backend

- ASP.NET Core (.NET 8)
- MVC / Razor Views
- Entity Framework Core
- Layered architecture:
  - `VMAPP.Data`  EF entities & `ApplicationDbContext`
  - `VMAPP.Services`  services + DTOs (business logic)
  - `VMAPP.Web`  controllers + view models + views (UI)

### Frontend

- Bootstrap
- jQuery
- jQuery DataTables
- Custom CSS (`site.css`, `homeStyle.css`, etc.)

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (Express or Developer Edition)
- SQL Server Management Studio (SSMS)
- Visual Studio 2022 / VS Code

---

## Database Setup (Testing Environment)

This project connects to SQL Server using the following connection string:

```
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\Server1;Database=VMAPP;User=sa;Password=pb;Encrypt=False;TrustServerCertificate=True;"
}
```

### Database Credentials

- Server: `localhost\Server1`
- Database: `VMAPP`
- Username: `sa`
- Password: `pb`
- Authentication Mode: SQL Server Authentication

⚠️ These credentials are for **local development/testing only**.

---

### Step 1 – Install SQL Server

Install SQL Server Express or Developer Edition and SQL Server
Management Studio (SSMS).

Ensure your SQL instance name is:

`Server1`

---

### Step 2 – Enable SQL Server Authentication Mode

1. Open SSMS  
2. Connect to `localhost\Server1`  
3. Right-click the server → **Properties**  
4. Open **Security** tab  
5. Select: **SQL Server and Windows Authentication mode**  
6. Restart SQL Server service

---

### Step 3 – Enable and Configure `sa` Login

1. In SSMS → **Security → Logins**  
2. Right-click `sa` → **Properties**  
3. Set password to: `pb`  
4. In **Status** tab ensure **Login is Enabled**

---

### Step 4 – Create the Database

#### Option A – Manual Creation

1. Right-click **Databases**  
2. **New Database…**  
3. Name it: `VMAPP`

#### Option B – Using Entity Framework (Recommended)

From the `VMAPP.Web` project directory run:

```
dotnet ef database update
```

or in Visual Studio Package Manager Console:

```
Update-Database

```

This will create the database and all tables automatically.

---

## How the Application Connects

In `Program.cs`:

```
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

The application reads the connection string from `appsettings.json` and
connects to SQL Server using Entity Framework Core **inside the service layer**
(`VSManagementService`, `VSCarsService`), not directly in controllers.

---

## Run the Application

From the solution root or `VMAPP.Web` project:

```
dotnet run --project VMAPP.Web
```

or simply press **F5** in Visual Studio.

---

## Important

- Controllers use **DI + services + DTOs**, not `ApplicationDbContext` directly.
- All data access is isolated in the `VMAPP.Services` layer.
- This configuration and the `sa` account/password are intended **only** for
  local development and testing.  
  Do **not** use the `sa` account or weak passwords in production environments.
