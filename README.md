# Vehicle Management System (VMAPP)

A web-based Vehicle Management System built with ASP.NET Core (.NET 8) and MVC / Razor Views.  
The application helps manage vehicles, their service history, and related service station records.

---

## Features

### Vehicle Management

- Create, edit, and delete vehicle entries
- Store core vehicle information: VIN, brand, model, year, color, type
- List all vehicles with sortable DataTables

### Service Station Management

- Create, edit, and delete vehicle service stations
- Store contact info: name, city, address, email, phone, description
- Assign vehicles to service stations and remove them

### Service Records

- Add, edit, and delete per-vehicle service records
- Track service date, cost, and description per service station
- Modal-based UI with DataTables for a responsive service history table

### UI / UX

- Fixed header and footer with consistent navigation
- Global background image
- Content displayed in centered white cards for readability
- Bootstrap-based layout
- jQuery DataTables integration for tabular data with pagination, serching, and sorting

### Privacy & Terms

- Dedicated Privacy Policy page (`/Home/Privacy`)
- Dedicated Terms and Conditions page (`/Home/Terms`)
- Both rendered in readable white content boxes

---

## Architecture & Project Structure

The solution is split into clear layers with Dependency Injection (DI) and services.
Controllers do **not** work with `DbContext` directly; instead they depend on
services (interfaces) and map **DTOs -> ViewModels**.

### Projects

| Project | Type | Role |
|---|---|---|
| `VMAPP.Web` | ASP.NET Core MVC | Presentation layer - controllers, views, view models |
| `VMAPP.Services` | Class Library | Service layer - business logic, data orchestration, DTOs |
| `VMAPP.Data` | Class Library | Data layer - EF Core `DbContext`, entity models, seeding |
| `VMAPP.Common` | Class Library | Shared constants used across all layers |
| `VMAPP.SandBox` | Console App | Standalone tool for DB migrations and seed data testing |

---

### VMAPP.Common

Provides shared validation constants consumed by data model annotations and any other layer that needs them.

**`GlobalConstant`** (static class):

| Constant | Value / Purpose |
|---|---|
| `SystemName` | `"VMAPP"` |
| `AdministratorRoleName` | `"Administrator"` |
| `VINRegex` | Regex for a valid 17-character VIN (`A-H, J-N, P-R, Z, 0-9`) |
| `VehicleServiceName` | `150` - max length for service station name |
| `VehicleServiceDescription` | `1000` - max length for service station description |
| `ServiceRecordDescription` | `1500` - max length for service record description |
| `CarBrandLength` | `100` - max length for vehicle brand |
| `CarModelLength` | `100` - max length for vehicle model |

---

### VMAPP.Data

Data access layer. Only the service layer (`VMAPP.Services`) uses `ApplicationDbContext` directly.

#### `ApplicationDbContext` - EF Core context

| `DbSet` | Entity | Notes |
|---|---|---|
| `Vehicles` | `Vehicle` | Unique index on `VIN` |
| `ServiceRecords` | `ServiceRecord` | `Cost` as `decimal(18,2)`; cascade-deletes with `Vehicle`; restricted delete on `VehicleService` |
| `VehicleServices` | `VehicleService` | Service station entries |

**Fluent configuration highlights:**
- `Vehicle.VIN` - unique index
- `ServiceRecord.Cost` - `decimal(18,2)`, required
- `ServiceRecord -> Vehicle` - cascade delete
- `ServiceRecord -> VehicleService` - delete restricted (cannot delete a service station that has records)

#### Entity Models (`VMAPP.Data/Models`)

- **`Vehicle`** - `VehicleId`, `VIN`, `CarBrand`, `CarModel`, `CreatedOnYear`, `Color`, `VehicleType` (enum), `CreatedOn`, navigation: `ICollection<ServiceRecord>`
- **`VehicleService`** - `Id`, `Name`, `Description`, `CreatedOn`, `City`, `Address`, `Email`, `Phone`, navigation: `ICollection<ServiceRecord>`
- **`ServiceRecord`** - `ServiceRecordId`, `ServiceDate`, `Description`, `Cost`, `VehicleId`, `VehicleServiceId`, navigation: `Vehicle`, `VehicleService`
- **`VehicleType`** (enum) - `Sedan`, `SUV`, `Truck`, `Coupe`, `Convertible`, `Hatchback`, `Van`, `Wagon`, `Motorcycle`, `Electric`

#### Seeding Infrastructure (`VMAPP.Data/Seeding`)

| Class | Role |
|---|---|
| `ISeeder` | Interface - `Task SeedAsync(ApplicationDbContext)` |
| `ApplicationDbContextSeeder` | Orchestrator - runs all seeders in order |
| `VehicleSeeding` | Seeds sample `Vehicle` rows (skips if any exist) |
| `VehicleServiceSeeding` | Seeds sample `VehicleService` rows (skips if any exist) |
| `ServiceRecordSeeding` | Seeds sample `ServiceRecord` rows (skips if any exist) |

The seeder is idempotent - it checks for existing data before inserting.

---

### VMAPP.Services

This layer owns all `ApplicationDbContext` usage. Controllers never see `DbContext`.

#### DTOs (`VMAPP.Services/DTOs`)

- **`VehicleServiceDto`** - Service station info: `Id`, `Name`, `City`, `Address`, `Email`, `Phone`, `Description`, `CreatedOn`
- **`VehicleDto`** - Vehicle info: `Id`, `VIN`, `CarBrand`, `CarModel`, `CreatedOnYear`, `Color`, `VehicleType` (`VehicleType` enum), `CreatedOn`
- **`ServiceRecordDto`** - Service record info: `Id`, `VehicleId`, `VehicleServiceId`, `ServiceDate`, `Cost`, `Description`
- **`ServiceWithVehiclesDto`** - Full service station info plus its assigned vehicles: `Id`, `Name`, `City`, `Address`, `Email`, `Phone`, `Description`, `CreatedOn`, `List<VehicleDto> Vehicles`
- **`VehicleDetailsDto`** - Detailed vehicle view: `VehicleDto Vehicle`, `List<ServiceRecordDto> ServiceRecords`
- **`VehicleWithServiceRecordsDto`** - Vehicle with `VehicleType` as string plus its service records: `Id`, `VIN`, `CarBrand`, `CarModel`, `CreatedOnYear`, `Color`, `VehicleType` (string), `List<ServiceRecordDto> ServiceRecords`

#### Service Interfaces (`VMAPP.Services/Interfaces`)

**`IVSManagementService`** - Manages `VehicleService` (service stations) and vehicle assignments:

| Method | Description |
|---|---|
| `GetAllAsync()` | Returns all service stations as `IReadOnlyList<VehicleServiceDto>` |
| `GetByIdAsync(int id)` | Returns a single service station or `null` |
| `CreateAsync(VehicleServiceDto dto)` | Creates a new service station, returns new `Id` |
| `UpdateAsync(VehicleServiceDto dto)` | Updates an existing service station |
| `DeleteAsync(int id)` | Deletes a service station |
| `ExistsAsync(int id)` | Returns `true` if the service station exists |
| `GetVehiclesByServiceIdAsync(int serviceId)` | Returns vehicles assigned to a service station |
| `GetVehiclesServiceDetailsByIdAsync(int id)` | Returns full `ServiceWithVehiclesDto` by ID |
| `GetVehicleByVinAsync(string vin)` | Looks up a vehicle by VIN |
| `AddVehicleToServiceAsync(int serviceId, int vehicleId)` | Assigns a vehicle to a service station |
| `RemoveVehicleFromServiceAsync(int serviceId, int vehicleId)` | Removes a vehicle assignment, returns `(Success, Message)` |

**`IVSCarsService`** - Manages vehicles and their service records within a service station context:

| Method | Description |
|---|---|
| `GetServiceWithVehiclesByNameAsync(string serviceName)` | Returns a `ServiceWithVehiclesDto` by name |
| `AddVehicleToServiceAsync(int serviceId, VehicleDto vehicle)` | Creates a new vehicle and assigns it |
| `GetVehicleDetailsAsync(int vehicleId)` | Returns `VehicleDetailsDto` |
| `UpdateVehicleAsync(VehicleDto vehicle)` | Updates vehicle data |
| `DeleteVehicleAsync(int vehicleId)` | Deletes a vehicle |
| `GetVehicleWithServiceRecordsAsync(int vehicleId, int serviceId)` | Returns `VehicleWithServiceRecordsDto` filtered by service |
| `GetServiceRecordByIdAsync(int id)` | Returns a single service record or `null` |
| `AddServiceRecordAsync(ServiceRecordDto dto)` | Creates a new service record, returns new `Id` |
| `UpdateServiceRecordAsync(ServiceRecordDto dto)` | Updates an existing service record |
| `DeleteServiceRecordAsync(int id)` | Deletes a service record |

**`IVSService`** - Standalone vehicle CRUD (not scoped to a service station):

| Method | Description |
|---|---|
| `GetAllAsync()` | Returns all vehicles as `IReadOnlyList<VehicleDto>` |
| `GetByIdAsync(int id)` | Returns a single vehicle or `null` |
| `CreateAsync(VehicleDto dto)` | Creates a new vehicle, returns new `Id` |
| `UpdateAsync(VehicleDto dto)` | Updates a vehicle |
| `DeleteAsync(int id)` | Deletes a vehicle |

#### Service Implementations

| Implementation | Interface | Key responsibilities |
|---|---|---|
| `VSManagementService` | `IVSManagementService` | CRUD for service stations; vehicle assignment via `ServiceRecord` link; maps EF entities <-> `VehicleServiceDto` |
| `VSCarsService` | `IVSCarsService` | Vehicle creation/edit/delete within a service context; load service records per vehicle/service combination |
| `VSService` | `IVSService` | Standalone vehicle CRUD; maps `Vehicle` <-> `VehicleDto` |

> **Important:** Only the service layer touches `ApplicationDbContext`.
> Controllers only see interfaces + DTOs and map them to ViewModels.

---

### VMAPP.Web

ASP.NET Core MVC project with controllers, view models, and views.

#### Dependency Injection (`Program.cs`)

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Application services
builder.Services.AddScoped<IVSManagementService, VSManagementService>();
builder.Services.AddScoped<IVSCarsService, VSCarsService>();
builder.Services.AddScoped<IVSService, VSService>();
```

Also configures `RequestLocalizationOptions` to use `en-US` for consistent decimal
handling (for costs like `10.05`).

#### Controllers

- **`VehicleServicesController`**
  - Injects `IVSManagementService` + `IVSCarsService`
  - Handles service station listing, create/edit/delete, vehicle assignment/removal, and service record management
  - Key DTO -> ViewModel mappings:
    - `VehicleServiceDto` <-> `VehicleServiceViewModel`, `AddServiceViewModel`, `EditViewModel`, `DeleteViewModel`
    - `ServiceWithVehiclesDto` -> `VehicleServiceDetailsViewModel`
    - `ServiceRecordDto` <-> `AddRecordViewModel`, `ServiceRecordFormViewModel`
  - No direct `DbContext` usage.

- **`VehicleController`**
  - Injects `IVSService`
  - Handles standalone vehicle listing and CRUD
  - Key DTO -> ViewModel mappings:
    - `VehicleDto` <-> `VehicleIndexViewModel`, `AddVehicleViewModel`, `EditVehicleViewModel`
  - No direct `DbContext` usage.

- **`HomeController`**
  - Home / Privacy / Terms / Error pages.

#### Extensions (`VMAPP.Web/Extensions`)

- **`ApplicationsBuilderExtensions.SeedDatabase()`**  
  An `IApplicationBuilder` extension method that runs `ApplicationDbContextSeeder` using a scoped `ApplicationDbContext`.  
  Call it from `Program.cs` after `app.Build()` to automatically seed the database on startup:

  ```csharp
  app.SeedDatabase();
  ```

#### View Models

| Folder | View Models |
|---|---|
| `Models/VehicleServiceModels/` | `VehicleServiceViewModel`, `AddServiceViewModel`, `EditViewModel`, `DeleteViewModel`, `VehicleServiceDetailsViewModel`, `AddVehicleToServiceRequest` |
| `Models/VehicleViewModels/` | `VehicleIndexViewModel`, `AddVehicleViewModel`, `EditVehicleViewModel` |
| `Models/VehicleServiceCars/` | `ServiceVehicleViewModel` |
| `Models/ServiceRecordModels/` | `AddRecordViewModel`, `ServiceRecordFormViewModel` |


#### Views (`VMAPP.Web/Views`)

##### Home/

| View | Route | Description |
|---|---|---|
| `Index.cshtml` | `GET /` | Landing page. Displays the VMAPP welcome heading. Entry point for the application. |
| `Privacy.cshtml` | `GET /Home/Privacy` | Privacy Policy page. Structured into sections: Overview, Information We Collect, Purpose of Processing, Security Measures, and Contact. Effective date is rendered dynamically from `DateTime.UtcNow`. |

##### Shared/

| View | Description |
|---|---|
| `_Layout.cshtml` | Master layout applied to every page. Renders a **fixed header** with the VMAPP logo and navbar links (Home, Privacy, Vehicle Services, Vehicle), a **fixed footer** with copyright and a Privacy link, and the `@RenderBody()` content area. Includes Bootstrap, jQuery, and jQuery DataTables CSS/JS bundles. |
| `Error.cshtml` | Error page rendered by `HomeController.Error()`. Shows an error heading and, when a `RequestId` is available, displays it for tracing. Includes guidance on enabling the Development environment for detailed exception info. Bound to `ErrorViewModel`. |
| `_ValidationScriptsPartial.cshtml` | Partial view that injects jQuery unobtrusive validation scripts. Included in form views that require client-side validation. |

##### VehicleServices/

| View | Route | Model | Description |
|---|---|---|---|
| `Index.cshtml` | `GET /VehicleServices /Index` | `IEnumerable <VehicleServiceViewModel>` | Lists all service stations in a jQuery DataTables table (Name, City, Address, Created On, Email as `mailto:` link, Phone as `tel:` link). Provides a **Create New Service** button. Each row has **Details**, **Edit**, and **Delete** buttons. Delete opens a Bootstrap confirmation modal before submitting. |
| `AddService.cshtml` | `GET/POST /VehicleServices /AddService` | `AddServiceViewModel` | Form to create a new service station. Two-column layout with fields: Name, City, Address, Phone, Email, Description (textarea). Includes server-side and client-side validation. |
| `EditService.cshtml` | `GET/POST /VehicleServices /EditService /{id}` | `EditViewModel` | Form to update an existing service station. Same fields as AddService, pre-populated from the database. Hidden `Id` field ensures the correct record is updated. Cancel returns to the Index. |
| `DeleteService.cshtml` | `GET/POST /VehicleServices /DeleteService /{id}` | `DeleteViewModel` | Delete confirmation form. Displays all service station fields as read-only (disabled inputs). A JavaScript `confirm()` prompt provides a second confirmation before the POST is submitted. Cancel returns to the Index. |
| `Details.cshtml` | `GET /VehicleServices /Details /{id}` | `VehicleServiceDetailsViewModel` | Service station detail page. Shows a read-only **Service Information** card, then a DataTables table of **Assigned Vehicles** (VIN, Brand, Model, Year, Color, Type). Each row has a **Service** button (navigates to `ServiceVehicle`) and a **Remove** button. Two Bootstrap modals: (1) **Add Vehicle** - searches the global vehicle list by VIN via AJAX, previews the found vehicle, and assigns it to the service; (2) **Remove Vehicle** - confirms removal and also deletes all service records for that vehicle in this service. |
| `ServiceVehicle.cshtml` | `GET /VehicleServices /ServiceVehicle?vehicleId=&serviceId=` | `ServiceVehicleViewModel` | Per-vehicle service record management page scoped to a specific service station. Shows a read-only **Vehicle Information** card, then a DataTables table of **Service Records** (Description, Date, Cost). All record operations are AJAX-driven through four Bootstrap modals: (1) **Add Record** - Description, Service Date, Cost with client-side validation; (2) **Details** - read-only view of a record; (3) **Edit Record** - pre-filled editable fields; (4) **Delete Confirmation** - shows the record description before confirming. Hidden fields carry `vehicleId` and `serviceId` context. JS logic lives in `serviceVehicle.js`. |

##### Vehicle/

| View | Route | Model | Description |
|---|---|---|---|
| `Index.cshtml` | `GET /Vehicle /Index` | `IEnumerable <VehicleIndexViewModel>` | Lists all vehicles in a jQuery DataTables table (VIN, Brand, Model, Year, Added On). Provides an **Add New Vehicle** button. Each row has **Edit** and **Delete** buttons. Delete opens a Bootstrap confirmation modal that displays the vehicle's brand, model, and VIN. JS logic lives in `vehicles.js`. |
| `AddVehicle.cshtml` | `GET/POST /Vehicle /AddVehicle` | `AddVehicleViewModel` | Form to create a new vehicle. Fields: VIN (17 characters, forced uppercase), Brand, Model, Year (1886-2100), Color, and Vehicle Type (dropdown populated from the `VehicleType` enum). All fields are required with server-side and client-side validation. |
| `EditVehicle.cshtml` | `GET/POST /Vehicle /EditVehicle /{id}` | `EditVehicleViewModel` | Form to update an existing vehicle. Identical fields to AddVehicle, pre-populated from the database. Hidden `Id` field identifies the record. Includes a Back to List link and Cancel button. |

---

### VMAPP.SandBox

A standalone **.NET 8 console application** designed to **run database migrations and seed data without starting the web application**.

#### Purpose

- Apply EF Core migrations to SQL Server independently of `VMAPP.Web`
- Execute the full seeding pipeline (`ApplicationDbContextSeeder`) to populate the database with sample data
- Useful for:
  - Initial database setup without running the web server
  - CI/CD pipeline scripts that need to prepare the DB before deploying the web app
  - Quickly resetting and re-seeding the database during development or testing
  - Verifying that `ApplicationDbContext` and migrations are correct in isolation

#### How It Works

1. Builds a generic `IHost` via `Host.CreateDefaultBuilder(args)`
2. Reads the `DefaultConnection` connection string from `appsettings.json` (same format as `VMAPP.Web`)
3. Registers `ApplicationDbContext` with the EF Core SQL Server provider
4. Creates a DI scope and resolves `ApplicationDbContext`
5. Runs `ApplicationDbContextSeeder.SeedAsync()`, which chains:
   - `VehicleSeeding` -> seeds `Vehicles`
   - `VehicleServiceSeeding` -> seeds `VehicleServices`
   - `ServiceRecordSeeding` -> seeds `ServiceRecords`

Each individual seeder is **idempotent**: it skips insertion if rows already exist in the target table.

#### Running the SandBox

> **Before running**, ensure the database exists and all migrations have been applied (see [Step 4](#step-4--create-the-database) below).

```
dotnet run --project VMAPP.SandBox
```

or simply set `VMAPP.SandBox` as the startup project in Visual Studio and press **F5**.

#### Applying Migrations via SandBox

The `VMAPP.SandBox` project references `VMAPP.Data`, so EF Core tooling can target it for migration commands:

```
dotnet ef database update --project VMAPP.Data --startup-project VMAPP.SandBox
```

or in Visual Studio Package Manager Console (with `VMAPP.Data` as the default project):

```
Update-Database
```

---

## Tech Stack

### Backend

- ASP.NET Core (.NET 8)
- MVC / Razor Views
- Entity Framework Core (SQL Server provider)
- Layered architecture:
  - `VMAPP.Common` - shared constants
  - `VMAPP.Data` - EF entities & `ApplicationDbContext` + seeding
  - `VMAPP.Services` - services, interfaces & DTOs (business logic)
  - `VMAPP.Web` - controllers, view models & views (UI)
  - `VMAPP.SandBox` - standalone DB migration & seeding tool

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

This project connects to SQL Server using the following connection string
(configured in `appsettings.json` in both `VMAPP.Web` and `VMAPP.SandBox`):

```json
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

> These credentials are for **local development/testing only**.

---

### Step 1 - Install SQL Server

Install SQL Server Express or Developer Edition and SQL Server
Management Studio (SSMS).

Ensure your SQL instance name is:

`Server1`

---

### Step 2 - Enable SQL Server Authentication Mode

1. Open SSMS  
2. Connect to `localhost\Server1`  
3. Right-click the server -> **Properties**  
4. Open **Security** tab  
5. Select: **SQL Server and Windows Authentication mode**  
6. Restart SQL Server service

---

### Step 3 - Enable and Configure `sa` Login

1. In SSMS -> **Security -> Logins**  
2. Right-click `sa` -> **Properties**  
3. Set password to: `pb`  
4. In **Status** tab ensure **Login is Enabled**

---

### Step 4 - Create the Database

#### Option A - Manual Creation

1. Right-click **Databases**  
2. **New Database...**  
3. Name it: `VMAPP`

#### Option B - Using Entity Framework (Recommended)

**Via `VMAPP.Web`** - from the `VMAPP.Web` project directory:

```
dotnet ef database update
```

or in Visual Studio Package Manager Console:

```
Update-Database
```

**Via `VMAPP.SandBox`** - without running the web app:

```
dotnet ef database update --project VMAPP.Data --startup-project VMAPP.SandBox
```

All options create the database and all tables automatically.

---

### Step 5 - Seed the Database (Optional)

To populate the database with sample vehicles, service stations, and service records,
run the SandBox project:

```
dotnet run --project VMAPP.SandBox
```

The web application can also seed on startup if `app.SeedDatabase()` is called in `Program.cs`
(via the `ApplicationsBuilderExtensions` extension).

---

## How the Application Connects

In `Program.cs`:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

The application reads the connection string from `appsettings.json` and
connects to SQL Server using Entity Framework Core **inside the service layer**
(`VSManagementService`, `VSCarsService`, `VSService`), not directly in controllers.

---

## Run the Application

From the solution root or `VMAPP.Web` project:

```
dotnet run --project VMAPP.Web
```

or simply press **F5** in Visual Studio (with `VMAPP.Web` as the startup project).

---

## Important

- Controllers use **DI + services + DTOs**, not `ApplicationDbContext` directly.
- All data access is isolated in the `VMAPP.Services` layer.
- `VMAPP.SandBox` provides a web-free way to apply migrations and seed data - useful for testing the data layer in isolation.
- This configuration and the `sa` account/password are intended **only** for
  local development and testing.  
  Do **not** use the `sa` account or weak passwords in production environments.
