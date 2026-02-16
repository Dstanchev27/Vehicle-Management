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
- Content displayed in centered white “cards” for readability
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
  Application/service layer – business logic and data access orchestration
- `VMAPP.Data`  
  Data access layer – EF Core `DbContext` and entity models

### VMAPP.Data

- `ApplicationDbContext` – EF Core context:
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
  (in code: `VehicleServiceManagementService`) – implements `IVSManagementService`  
  Responsibilities:
  - CRUD for `VehicleService` (`VehicleServices` table)
  - All EF work happens here (no controllers touch `DbContext`)
  - Maps EF entities ↔ `VehicleServiceDto`

- `VSCarsService`  
  (in code: `VehicleServiceCarsService`) – implements `IVSCarsService`  
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
