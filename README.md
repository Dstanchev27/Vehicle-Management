# Vehicle Management System (VMAPP)

A web-based Vehicle Management System built with ASP.NET Core (.NET 8) and MVC / Razor Views.  
The application manages vehicles, service history, insurance policies, annual vehicle reviews, and related company records.  
It includes role-based access control, ASP.NET Core Identity integration, transactional email via SendGrid, structured logging with Serilog, and a dedicated NUnit test suite.

---

## ⚠️ Important Notes for Testers / Reviewers

### 🚀 The Application Self-Initialises on First Run

**You do not need to create or migrate the database manually.**  
On startup, `Program.cs` calls `app.InitializeDatabaseAsync()`, which:

1. Automatically runs all pending EF Core migrations (`MigrateAsync()`)
2. Automatically seeds all sample data (roles, users, vehicles, stations, companies, policies, claims, reports)

Simply set `VMAPP.Web` as the startup project and press **F5** — the database will be created and seeded on the first run, provided the SQL Server instance is reachable (see [Database Setup](#database-setup-testing-environment)).

> If the database server is not reachable at startup, Serilog (which writes to the DB) will log a fatal error and the application will exit. Make sure the SQL Server instance is running **before** launching the app.

---

### 🔑 Pre-Seeded Test Accounts (No Email Confirmation Required)

To avoid any dependency on email / SendGrid during testing, four ready-to-use accounts are seeded automatically by `TestUsersSeeding`. All have `EmailConfirmed = true` and can be used immediately:

| Email | Password | Role | Scope |
|---|---|---|---|
| `testadmin@vmapp.com` | `TestAdmin123!` | `ProgramAdministrator` | Full access to everything |
| `testinsurance@vmapp.com` | `TestInsurance123!` | `InsuranceCompany` | Scoped to first seeded insurance company |
| `testservice@vmapp.com` | `TestService123!` | `VehicleService` | Scoped to first seeded service station |
| `testannual@vmapp.com` | `TestAnnual123!` | `AnnualReviewCompany` | Scoped to first seeded annual review company |

> **The three non-admin test accounts (`testinsurance`, `testservice`, `testannual`) do NOT require 2FA and can log in directly.**

---

### 🔐 Administrator 2FA Requirement

The `ProgramAdministrator` role enforces two-factor authentication via `EnforceAdmin2FAMiddleware`.  
**The test admin account (`testadmin@vmapp.com`) will be redirected to the 2FA setup page on first login.**

To complete admin login you will need an authenticator app (e.g. Google Authenticator, Microsoft Authenticator) installed on your phone. This is by design — administrators are required to have 2FA enabled for security.

**If you want to test admin features without 2FA**, you can:
- Use `testinsurance@vmapp.com` — full insurance management
- Use `testservice@vmapp.com` — service station and service records
- Use `testannual@vmapp.com` — annual review management

---

### 📧 New User Registration & Email (SendGrid)

When a **new user registers** through the standard registration page, an email confirmation link is sent via SendGrid.  
**The SendGrid API key is not included in the repository for security reasons.**

To enable email sending, add your SendGrid credentials to `appsettings.json` in `VMAPP.Web`:

```json
"SendGridApiKey": {
  "ApiKey": "YOUR_SENDGRID_API_KEY",
  "SenderEmail": "your-verified-sender@example.com",
  "SenderName": "VMAPP"
}
```

> If the key is not configured, email sending will silently fail but the application will still run normally.  
> **For testing, use the pre-seeded accounts above — they do not require email confirmation.**

---

## Features

### Vehicle Management

- Create, edit, and delete vehicle entries
- Store core vehicle information: VIN, brand, model, year, color, type
- List all vehicles with sortable DataTables

### Service Station Management

- Create, edit, and delete vehicle service stations — `ProgramAdministrator` only
- Store contact info: name, city, address, email, phone, description
- Assign vehicles to service stations and remove them — `ProgramAdministrator` only
- `VehicleService` users are automatically redirected to their own station's detail page
- `VehicleService` users can view their station details and navigate to individual vehicles

### Service Records

- Add per-vehicle service records — `ProgramAdministrator` and `VehicleService` users
- Edit and delete per-vehicle service records — `ProgramAdministrator` only
- Track service date, cost, and description per service station
- Modal-based UI with DataTables for a responsive service history table

### Insurance Management

- Create, edit, and delete insurance companies — `ProgramAdministrator` only
- Assign vehicles to insurance companies via insurance policies — `ProgramAdministrator` only
- Delete insurance policies and their associated claims — `ProgramAdministrator` only
- Track policy number, start date, and end date per policy
- Add insurance claims linked to a policy (claim date, description, amount) — `ProgramAdministrator` and `InsuranceCompany` users
- Delete insurance claims — `ProgramAdministrator` only
- `InsuranceCompany` users are automatically redirected to their own company's detail page
- `InsuranceCompany` users can view their company details, policies, and claims, and add new claims

### Annual Review Management

- Create, edit, and delete annual review companies — `ProgramAdministrator` only
- Assign vehicles to annual review companies via annual reports — `ProgramAdministrator` and `AnnualReviewCompany` users
- Delete annual reports — `ProgramAdministrator` only
- Track report number, inspection date, expiry date, pass/fail status, and notes per report
- `AnnualReviewCompany` users are automatically redirected to their own company's detail page
- `AnnualReviewCompany` users can view their company details and assigned vehicles, and add new reports

### User Management & Administration

- Administration area restricted to `ProgramAdministrator` role
- List, view, create, edit, soft-delete users
- Assign roles (`ProgramAdministrator`, `VehicleService`, `InsuranceCompany`, `AnnualReviewCompany`) to users
- Assign users to a specific service station, insurance company, or annual review company

### Authentication & Authorization

- ASP.NET Core Identity with custom `ApplicationUser` and `ApplicationRole`
- Email confirmation and password reset via SendGrid
- Two-factor authentication (2FA) support; admins are required to have 2FA enabled (`EnforceAdmin2FAMiddleware`)
- Four application roles: `ProgramAdministrator`, `VehicleService`, `InsuranceCompany`, `AnnualReviewCompany`
- Cookie-based authentication with consistent security policy

### Role-Based Access Control

Permissions are enforced at both the controller action level (`[Authorize(Roles = ...)]`) and in the views (UI elements hidden for unauthorised roles).

#### `ProgramAdministrator`

Full access to everything in the application.

| Area | Allowed Actions |
|---|---|
| Service Stations | Create, edit, delete, view details, assign/remove vehicles |
| Service Records | Add, edit, delete |
| Insurance Companies | Create, edit, delete, view details |
| Insurance Policies | Add, delete |
| Insurance Claims | Add, delete |
| Annual Review Companies | Create, edit, delete, view details |
| Annual Reports | Add, delete |
| Vehicles | Full CRUD |
| User Administration | Full CRUD, role assignment, service/company assignment |

#### `VehicleService`

Read-only access to service station data plus the ability to add service records.

| Area | Allowed Actions |
|---|---|
| Service Stations | View own station's detail page only (auto-redirected on login) |
| Service Records | Add new records only — edit and delete are restricted to `ProgramAdministrator` |
| Vehicle assignment / removal | Not permitted |
| Insurance, Annual Review, Vehicles, Administration | Not accessible |

#### `InsuranceCompany`

Read-only access to insurance data plus the ability to add claims.

| Area | Allowed Actions |
|---|---|
| Insurance Companies | View own company's detail page only (auto-redirected on login) |
| Insurance Policies | View policy details only |
| Insurance Claims | Add new claims only — delete is restricted to `ProgramAdministrator` |
| Policy creation / deletion | Not permitted |
| Service Stations, Annual Review, Vehicles, Administration | Not accessible |

#### `AnnualReviewCompany`

Read-only access to annual review data plus the ability to add reports.

| Area | Allowed Actions |
|---|---|
| Annual Review Companies | View own company's detail page only (auto-redirected on login) |
| Annual Reports | Add new reports only — delete is restricted to `ProgramAdministrator` |
| Company creation / deletion | Not permitted |
| Service Stations, Insurance, Vehicles, Administration | Not accessible |

### UI / UX

- Fixed header and footer with consistent navigation
- Global background image
- Content displayed in centered white cards for readability
- Bootstrap-based layout
- jQuery DataTables integration for tabular data with pagination, searching, and sorting

### Logging & Error Handling

- Structured logging with Serilog (request logging, enriched with user identity)
- Bootstrap logger configured in `Program.cs` before host build — captures startup errors even before the full logger is configured
- `GlobalExceptionMiddleware` catches any unhandled exception, logs it with Serilog, and redirects to the 500 error page (in Development the default exception page is shown instead)
- `UseStatusCodePagesWithReExecute` intercepts HTTP error responses and re-executes through `HomeController.Error(int? statusCode)`
- Four dedicated, branded error pages served by `HomeController`:
  - **400 Bad Request** - yellow error code, asks the user to check their input
  - **403 Access Denied** - orange error code, informs the user they lack permission
  - **404 Page Not Found** - grey error code, explains the page does not exist
  - **500 Server Error** - red error code, notifies the user of an unexpected server-side failure
- Every error page is rendered inside the main `_Layout.cshtml` and includes a **Back to Home** button

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
| `VMAPP.Web` | ASP.NET Core MVC | Presentation layer - controllers, views, view models, Identity pages |
| `VMAPP.Services` | Class Library | Service layer - business logic, data orchestration, DTOs |
| `VMAPP.Data` | Class Library | Data layer - EF Core `DbContext`, entity models, seeding |
| `VMAPP.Common` | Class Library | Shared constants used across all layers |
| `VMAPP.SandBox` | Console App | Standalone tool for DB migrations and seed data testing |
| `VMAPP.Test` | NUnit Test Project | Unit tests for all service layer implementations |

---

### VMAPP.Common

Provides shared validation constants consumed by data model annotations and any other layer that needs them.

**`GlobalConstant`** (static class):

| Constant | Value / Purpose |
|---|---|
| `SystemName` | `"VMAPP"` |
| `AdministratorRoleName` | `"ProgramAdministrator"` |
| `InsuranceCompanyRoleName` | `"InsuranceCompany"` |
| `VehicleServiceRoleName` | `"VehicleService"` |
| `AnnualReviewCompanyRoleName` | `"AnnualReviewCompany"` |
| `VINRegex` | Regex for a valid 17-character VIN (`A-H, J-N, P-R, Z, 0-9`) |
| `VehicleServiceName` | `150` - max length for service station name |
| `VehicleServiceDescription` | `1000` - max length for service station description |
| `ServiceRecordDescription` | `1500` - max length for service record description |
| `CarBrandLength` | `100` - max length for vehicle brand |
| `CarModelLength` | `100` - max length for vehicle model |
| `CityMaxLength` / `CityMinLength` | `100` / `3` - city field constraints |
| `AddressMaxLength` / `AddressMinLength` | `1024` / `3` - address field constraints |
| `InsuranceCompanyName` | `150` - max length for insurance company name |
| `InsuranceCompanyDescription` | `1000` - max length for insurance company description |
| `InsurancePolicyNumber` | `50` - max length for policy number |
| `InsuranceClaimDescription` | `1500` - max length for claim description |
| `AnnualReviewCompanyName` | `150` - max length for annual review company name |
| `AnnualReviewCompanyDescription` | `1000` - max length for annual review company description |
| `AnnualReportNumber` | `50` - max length for report number |
| `AnnualReportNotes` | `1500` - max length for report notes |

---

### VMAPP.Data

Data access layer. Only the service layer (`VMAPP.Services`) uses `ApplicationDbContext` directly.

#### `ApplicationDbContext` - EF Core context

| `DbSet` | Entity | Notes |
|---|---|---|
| `Vehicles` | `Vehicle` | Unique index on `VIN` |
| `ServiceRecords` | `ServiceRecord` | `Cost` as `decimal(18,2)`; cascade-deletes with `Vehicle`; restricted delete on `VehicleService` |
| `VehicleServices` | `VehicleService` | Service station entries |
| `InsuranceCompanies` | `InsuranceCompany` | Insurance company entries; implements `IAuditInfo`, `IDeletableEntity` |
| `InsurancePolicies` | `InsurancePolicy` | Policies linking a `Vehicle` to an `InsuranceCompany` |
| `InsuranceClaims` | `InsuranceClaim` | Claims filed against an `InsurancePolicy`; `Amount` as `decimal(18,2)` |
| `AnnualReviewCompanies` | `AnnualReviewCompany` | Annual review company entries; implements `IAuditInfo`, `IDeletableEntity` |
| `AnnualReports` | `AnnualReport` | Inspection reports linking a `Vehicle` to an `AnnualReviewCompany`; implements `IAuditInfo`, `IDeletableEntity` |
| `Users` | `ApplicationUser` | Extended Identity user |

**Fluent configuration highlights:**
- `Vehicle.VIN` - unique index
- `ServiceRecord.Cost` - `decimal(18,2)`, required
- `ServiceRecord -> Vehicle` - cascade delete
- `ServiceRecord -> VehicleService` - delete restricted (cannot delete a service station that has records)
- `InsuranceClaim.Amount` - `decimal(18,2)`, required

#### Entity Models (`VMAPP.Data/Models`)

- **`Vehicle`** - `VehicleId`, `VIN`, `CarBrand`, `CarModel`, `CreatedOnYear`, `Color`, `VehicleType` (enum), `CreatedOn`, navigation: `ICollection<ServiceRecord>`, `ICollection<InsurancePolicy>`, `ICollection<AnnualReport>`
- **`VehicleService`** - `Id`, `Name`, `Description`, `CreatedOn`, `City`, `Address`, `Email`, `Phone`, navigation: `ICollection<ServiceRecord>`, `ICollection<ApplicationUser>`
- **`ServiceRecord`** - `ServiceRecordId`, `ServiceDate`, `Description`, `Cost`, `VehicleId`, `VehicleServiceId`, `CreatedById`, `CreatedOn`, navigation: `Vehicle`, `VehicleService`
- **`InsuranceCompany`** - `Id`, `Name`, `Description`, `City`, `Address`, `Email`, `Phone`, `CreatedOn`, `CreatedById`, `ModifiedById`, `IsDeleted`, navigation: `ICollection<InsurancePolicy>`, `ICollection<ApplicationUser>`
- **`InsurancePolicy`** - `Id`, `VehicleId`, `InsuranceCompanyId`, `PolicyNumber`, `StartDate`, `EndDate`, `CreatedById`, `IsDeleted`, navigation: `Vehicle`, `InsuranceCompany`, `ICollection<InsuranceClaim>`
- **`InsuranceClaim`** - `Id`, `InsurancePolicyId`, `ClaimDate`, `Description`, `Amount`, `CreatedOn`, `IsDeleted`, navigation: `InsurancePolicy`
- **`AnnualReviewCompany`** - `Id`, `Name`, `Description`, `City`, `Address`, `Email`, `Phone`, `CreatedOn`, `CreatedById`, `ModifiedById`, `IsDeleted`, navigation: `ICollection<AnnualReport>`, `ICollection<ApplicationUser>`
- **`AnnualReport`** - `Id`, `VehicleId`, `AnnualReviewCompanyId`, `ReportNumber`, `InspectionDate`, `ExpiryDate`, `Passed`, `Notes`, `CreatedById`, `CreatedOn`, `IsDeleted`, navigation: `Vehicle`, `AnnualReviewCompany`
- **`ApplicationUser`** - extends `IdentityUser`; adds `City`, `Address`, `CreatedOn`, `IsDeleted`, `VehicleServiceId`, `InsuranceCompanyId`, `AnnualReviewCompanyId`; implements `IAuditInfo`, `IDeletableEntity`
- **`ApplicationRole`** - extends `IdentityRole`
- **`VehicleType`** (enum) - `Sedan`, `SUV`, `Truck`, `Coupe`, `Convertible`, `Hatchback`, `Van`, `Wagon`, `Motorcycle`, `Electric`
- **`IAuditInfo`** (base interface) - `CreatedOn`, `CreatedById`, `ModifiedOn`, `ModifiedById`
- **`IDeletableEntity`** (base interface) - `IsDeleted`, `DeletedOn`

#### Seeding Infrastructure (`VMAPP.Data/Seeding`)

| Class | Role |
|---|---|
| `ISeeder` | Interface - `Task SeedAsync(ApplicationDbContext)` |
| `ApplicationDbContextSeeder` | Orchestrator - runs all seeders in order |
| `RoleSeeding` | Seeds application roles (`ProgramAdministrator`, `VehicleService`, `InsuranceCompany`, `AnnualReviewCompany`) |
| `AdminUserSeeding` | Seeds the default admin user account |
| `VehicleSeeding` | Seeds sample `Vehicle` rows |
| `VehicleServiceSeeding` | Seeds sample `VehicleService` rows |
| `InsuranceCompanySeeding` | Seeds sample `InsuranceCompany` rows |
| `AnnualReviewCompanySeeding` | Seeds sample `AnnualReviewCompany` rows |
| `UserSeeding` | Seeds sample application users and assigns them to services/companies |
| `TestUsersSeeding` | Seeds 4 ready-to-use test accounts with confirmed emails and no 2FA (see [Test Accounts](#-pre-seeded-test-accounts-no-email-confirmation-required)) |
| `ServiceRecordSeeding` | Seeds sample `ServiceRecord` rows |
| `InsurancePolicySeeding` | Seeds sample `InsurancePolicy` rows |
| `InsuranceClaimSeeding` | Seeds sample `InsuranceClaim` rows |
| `AnnualReportSeeding` | Seeds sample `AnnualReport` rows |

The seeder is idempotent - each individual seeder checks for existing data before inserting.

---

### VMAPP.Services

This layer owns all `ApplicationDbContext` usage. Controllers never see `DbContext`.

#### DTOs (`VMAPP.Services/DTOs`)

**Vehicle / Service Station DTOs (`VehicleServiceDTOs/`, `VehicleDTOs/`):**

- **`VehicleServiceDto`** - Service station info: `Id`, `Name`, `City`, `Address`, `Email`, `Phone`, `Description`, `CreatedOn`
- **`VehicleDto`** - Vehicle info: `Id`, `VIN`, `CarBrand`, `CarModel`, `CreatedOnYear`, `Color`, `VehicleType` (`VehicleType` enum), `CreatedOn`
- **`ServiceRecordDto`** - Service record info: `Id`, `VehicleId`, `VehicleServiceId`, `ServiceDate`, `Cost`, `Description`, `CreatedById`
- **`ServiceWithVehiclesDto`** - Full service station info plus its assigned vehicles: `Id`, `Name`, `City`, `Address`, `Email`, `Phone`, `Description`, `CreatedOn`, `List<VehicleDto> Vehicles`
- **`VehicleDetailsDto`** - Detailed vehicle view: `VehicleDto Vehicle`, `List<ServiceRecordDto> ServiceRecords`
- **`VehicleWithServiceRecordsDto`** - Vehicle with `VehicleType` as string plus its service records: `Id`, `VIN`, `CarBrand`, `CarModel`, `CreatedOnYear`, `Color`, `VehicleType` (string), `List<ServiceRecordDto> ServiceRecords`

**Insurance DTOs (`InsuranceDTOs/`):**

- **`InsuranceCompanyDto`** - Insurance company info: `Id`, `Name`, `City`, `Address`, `Email`, `Phone`, `Description`, `CreatedOn`, `CreatedById`, `ModifiedById`
- **`InsuranceCompanyWithVehiclesDto`** - Company info plus its insured vehicles (via policies): `Id`, `Name`, `City`, `Address`, `Email`, `Phone`, `Description`, `CreatedOn`, `List<VehicleDto> Vehicles`, `List<InsurancePolicyDetailsDto> Policies`
- **`InsurancePolicyFormDto`** - Policy creation/edit data: `Id`, `VehicleId`, `InsuranceCompanyId`, `PolicyNumber`, `StartDate`, `EndDate`, `CreatedById`
- **`InsurancePolicyDetailsDto`** - Full policy view: `Id`, `PolicyNumber`, `StartDate`, `EndDate`, `VehicleVIN`, `InsuranceCompanyName`, `List<InsuranceClaimFormDto> Claims`
- **`InsuranceClaimFormDto`** - Claim data: `Id`, `InsurancePolicyId`, `ClaimDate`, `Description`, `Amount`

**Annual Review DTOs (`AnnualReviewDTOs/`):**

- **`AnnualReviewCompanyDto`** - Annual review company info: `Id`, `Name`, `City`, `Address`, `Email`, `Phone`, `Description`, `CreatedOn`, `CreatedById`, `ModifiedById`
- **`AnnualReviewCompanyWithVehiclesDto`** - Company info plus vehicles with active reports: `Id`, `Name`, `City`, `Address`, `Email`, `Phone`, `Description`, `CreatedOn`, `List<VehicleWithReportIdDto> Vehicles`
- **`VehicleWithReportIdDto`** - Vehicle info with the associated report ID and report number: `Id`, `ReportId`, `VIN`, `CarBrand`, `CarModel`, `CreatedOnYear`, `Color`, `VehicleType`, `ReportNumber`
- **`AnnualReportFormDto`** - Report creation data: `VehicleId`, `AnnualReviewCompanyId`, `ReportNumber`, `InspectionDate`, `ExpiryDate`, `Passed`, `Notes`, `CreatedById`
- **`AnnualReportDetailsDto`** - Full report view: `Id`, `ReportNumber`, `InspectionDate`, `ExpiryDate`, `Passed`, `Notes`, `VehicleId`, `VehicleVIN`, `VehicleBrand`, `VehicleModel`, `AnnualReviewCompanyId`, `AnnualReviewCompanyName`

#### Service Interfaces (`VMAPP.Services/Interfaces`)

**`IVSManagementService`** - Manages `VehicleService` (service stations) and vehicle assignments:

| Method | Description |
|---|---|
| `GetAllAsync()` | Returns all service stations as `IReadOnlyList<VehicleServiceDto>` |
| `GetByIdAsync(int id)` | Returns a single service station or `null` |
| `CreateAsync(VehicleServiceDto dto)` | Creates a new service station, returns new `Id` |
| `UpdateAsync(VehicleServiceDto dto)` | Updates an existing service station, returns `bool` |
| `DeleteAsync(int id)` | Deletes a service station, returns `bool` |
| `ExistsAsync(int id)` | Returns `true` if the service station exists |
| `GetVehiclesByServiceIdAsync(int serviceId)` | Returns vehicles assigned to a service station |
| `GetVehiclesServiceDetailsByIdAsync(int id)` | Returns full `ServiceWithVehiclesDto` by ID |
| `GetVehicleByVinAsync(string vin)` | Looks up a vehicle by VIN |
| `AddVehicleToServiceAsync(int serviceId, int vehicleId, string userId)` | Assigns a vehicle to a service station, returns `bool` |
| `RemoveVehicleFromServiceAsync(int serviceId, int vehicleId)` | Removes a vehicle assignment, returns `(Success, Message)` |
| `AssignUserAsync(string userId, int? serviceId)` | Assigns or clears a user's service station association, returns `bool` |

**`IVSCarsService`** - Manages vehicles and their service records within a service station context:

| Method | Description |
|---|---|
| `GetServiceWithVehiclesByNameAsync(string serviceName)` | Returns a `ServiceWithVehiclesDto` by name |
| `AddVehicleToServiceAsync(int serviceId, VehicleDto vehicle)` | Creates a new vehicle and assigns it |
| `GetVehicleDetailsAsync(int vehicleId)` | Returns `VehicleDetailsDto` |
| `UpdateVehicleAsync(VehicleDto vehicle)` | Updates vehicle data, returns `bool` |
| `DeleteVehicleAsync(int vehicleId)` | Deletes a vehicle, returns `bool` |
| `GetVehicleWithServiceRecordsAsync(int vehicleId, int serviceId)` | Returns `VehicleWithServiceRecordsDto` filtered by service |
| `GetServiceRecordByIdAsync(int id)` | Returns a single service record or `null` |
| `AddServiceRecordAsync(ServiceRecordDto dto)` | Creates a new service record, returns new `Id` |
| `UpdateServiceRecordAsync(ServiceRecordDto dto)` | Updates an existing service record, returns `bool` |
| `DeleteServiceRecordAsync(int id)` | Deletes a service record, returns `bool` |

**`IVSService`** - Standalone vehicle CRUD (not scoped to a service station):

| Method | Description |
|---|---|
| `GetAllAsync()` | Returns all vehicles as `IReadOnlyList<VehicleDto>` |
| `GetByIdAsync(int id)` | Returns a single vehicle or `null` |
| `CreateAsync(VehicleDto dto)` | Creates a new vehicle, returns new `Id` |
| `UpdateAsync(VehicleDto dto)` | Updates a vehicle, returns `bool` |
| `DeleteAsync(int id)` | Deletes a vehicle, returns `bool` |

**`IVSInsuranceService`** - Manages insurance companies, policies, and claims:

| Method | Description |
|---|---|
| `GetAllAsync()` | Returns all insurance companies as `IReadOnlyList<InsuranceCompanyDto>` |
| `GetByIdAsync(int id)` | Returns a single insurance company or `null` |
| `CreateAsync(InsuranceCompanyDto dto)` | Creates a new insurance company, returns new `Id` |
| `UpdateAsync(InsuranceCompanyDto dto)` | Updates an existing insurance company, returns `bool` |
| `DeleteAsync(int id)` | Deletes an insurance company, returns `bool` |
| `GetCompanyWithVehiclesAsync(int id)` | Returns full `InsuranceCompanyWithVehiclesDto` by ID |
| `GetPolicyDetailsAsync(int id)` | Returns `InsurancePolicyDetailsDto` including claims |
| `GetVehicleByVinAsync(string vin)` | Looks up a vehicle by VIN |
| `AddPolicyAsync(InsurancePolicyFormDto dto)` | Creates a new insurance policy, returns new `Id` |
| `DeletePolicyAsync(int id)` | Deletes an insurance policy, returns `bool` |
| `GetClaimByIdAsync(int id)` | Returns a single insurance claim or `null` |
| `AddClaimAsync(InsuranceClaimFormDto dto)` | Creates a new insurance claim, returns new `Id` |
| `DeleteClaimAsync(int id)` | Deletes an insurance claim, returns `bool` |
| `AssignUserAsync(string userId, int? companyId)` | Assigns or clears a user's insurance company association, returns `bool` |
| `GetCompanyIdByPolicyIdAsync(int policyId)` | Returns the `InsuranceCompanyId` for a given policy, or `null` |

**`IVSAnnualReviewService`** - Manages annual review companies and inspection reports:

| Method | Description |
|---|---|
| `GetAllAsync()` | Returns all annual review companies as `IReadOnlyList<AnnualReviewCompanyDto>` |
| `GetByIdAsync(int id)` | Returns a single annual review company or `null` |
| `CreateAsync(AnnualReviewCompanyDto dto)` | Creates a new annual review company, returns new `Id` |
| `UpdateAsync(AnnualReviewCompanyDto dto)` | Updates an existing annual review company, returns `bool` |
| `DeleteAsync(int id)` | Deletes an annual review company, returns `bool` |
| `GetCompanyWithVehiclesAsync(int id)` | Returns full `AnnualReviewCompanyWithVehiclesDto` by ID |
| `GetReportDetailsAsync(int id)` | Returns `AnnualReportDetailsDto` for a given report |
| `GetVehicleByVinAsync(string vin)` | Looks up a vehicle by VIN |
| `AddReportAsync(AnnualReportFormDto dto)` | Creates a new annual report, returns new `Id` |
| `DeleteReportAsync(int id)` | Deletes an annual report, returns `bool` |
| `AssignUserAsync(string userId, int? companyId)` | Assigns or clears a user's annual review company association, returns `bool` |
| `GetCompanyIdByReportIdAsync(int reportId)` | Returns the `AnnualReviewCompanyId` for a given report, or `null` |

**`ICustomEmailSender`** / **`IEmailSender`** - Transactional email abstractions backed by `SendGridEmailSender` and `IdentityEmailSenderAdapter`.

#### Service Implementations

| Implementation | Interface | Key responsibilities |
|---|---|---|
| `VSManagementService` | `IVSManagementService` | CRUD for service stations; vehicle assignment via `ServiceRecord` link; user assignment; maps EF entities <-> `VehicleServiceDto` |
| `VSCarsService` | `IVSCarsService` | Vehicle creation/edit/delete within a service context; load service records per vehicle/service combination |
| `VSService` | `IVSService` | Standalone vehicle CRUD; maps `Vehicle` <-> `VehicleDto` |
| `VSInsurance` | `IVSInsuranceService` | CRUD for insurance companies, policies, and claims; user assignment; vehicle lookup by VIN |
| `VSAnnualReview` | `IVSAnnualReviewService` | CRUD for annual review companies and inspection reports; user assignment; vehicle lookup by VIN |
| `SendGridEmailSender` | `ICustomEmailSender` | Sends transactional emails via the SendGrid API |
| `IdentityEmailSenderAdapter` | `IEmailSender` (ASP.NET Core Identity) | Adapts `ICustomEmailSender` for use with Identity email confirmation and password reset flows |

> **Important:** Only the service layer touches `ApplicationDbContext`.
> Controllers only see interfaces + DTOs and map them to ViewModels.

---

### VMAPP.Web

ASP.NET Core MVC project with controllers, Identity Razor Pages, administration area, view models, and views.

#### Startup & Database Initialisation

`Program.cs` uses a bootstrap Serilog logger before host build (to capture pre-startup errors), then calls:

```csharp
await app.InitializeDatabaseAsync();
```

`InitializeDatabaseAsync` (in `ServiceCollectionExtensions`) runs on every startup:

```csharp
await db.Database.MigrateAsync();                    // applies any pending migrations
await new ApplicationDbContextSeeder().SeedAsync(db); // seeds data if not already present
```

This means **the database is created and seeded automatically on first run** — no manual migration or seeding step is required.

#### Dependency Injection

All DI registrations are organized into extension methods in `ServiceCollectionExtensions`:

```csharp
services
    .AddDatabase(configuration)        // EF Core + SQL Server
    .AddIdentity()                     // ASP.NET Core Identity
    .AddControllersAndViews()          // MVC + global authorization filter
    .AddCookieConsent()
    .AddApplicationServices(config)    // scoped services + SendGrid + localization
    .AddSecurity()
    .AddRazorPages();
```

Application services registered:

```csharp
services.AddScoped<IVSManagementService, VSManagementService>();
services.AddScoped<IVSCarsService, VSCarsService>();
services.AddScoped<IVSService, VSService>();
services.AddScoped<IVSInsuranceService, VSInsurance>();
services.AddScoped<IVSAnnualReviewService, VSAnnualReview>();
services.AddTransient<ICustomEmailSender, SendGridEmailSender>();
services.AddTransient<IEmailSender, IdentityEmailSenderAdapter>();
```

`RequestLocalizationOptions` is configured to use `en-US` for consistent decimal handling (e.g., costs like `10.05`).

#### Controllers

- **`VehicleServicesController`**
  - Requires `ProgramAdministrator` or `VehicleService` role
  - Injects `IVSManagementService` + `IVSCarsService`
  - `ProgramAdministrator`: full access — create/edit/delete stations, assign/remove vehicles, add/edit/delete service records
  - `VehicleService`: restricted access — view own station and its vehicles, add service records only
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

- **`InsuranceController`**
  - Requires `ProgramAdministrator` or `InsuranceCompany` role
  - Injects `IVSInsuranceService` + `UserManager<ApplicationUser>`
  - `ProgramAdministrator`: full access — create/edit/delete companies, add/delete policies, add/delete claims
  - `InsuranceCompany`: restricted access — view own company details and policies, add claims only
  - `InsuranceCompany` role users are automatically scoped to their own company
  - Key DTO -> ViewModel mappings:
    - `InsuranceCompanyDto` <-> `InsuranceCompanyViewModel`, `AddInsuranceCompanyViewModel`, `EditInsuranceCompanyViewModel`
    - `InsuranceCompanyWithVehiclesDto` -> `InsuranceCompanyDetailsViewModel`
    - `InsurancePolicyFormDto` <-> `InsurancePolicyFormViewModel`
    - `InsurancePolicyDetailsDto` -> `InsurancePolicyDetailsViewModel`
    - `InsuranceClaimFormDto` <-> `InsuranceClaimFormViewModel`

- **`AnnualReviewController`**
  - Requires `ProgramAdministrator` or `AnnualReviewCompany` role
  - Injects `IVSAnnualReviewService` + `UserManager<ApplicationUser>`
  - `ProgramAdministrator`: full access — create/edit/delete companies, add/delete reports
  - `AnnualReviewCompany`: restricted access — view own company details and vehicles, add reports only
  - `AnnualReviewCompany` role users are automatically scoped to their own company
  - Key DTO -> ViewModel mappings:
    - `AnnualReviewCompanyDto` <-> `AnnualReviewCompanyViewModel`, `AddAnnualReviewCompanyViewModel`, `EditAnnualReviewCompanyViewModel`
    - `AnnualReviewCompanyWithVehiclesDto` -> `AnnualReviewCompanyDetailsViewModel`
    - `AnnualReportFormDto` <-> `AnnualReportFormViewModel`
    - `AnnualReportDetailsDto` -> `AnnualReportDetailsViewModel`
  - No direct `DbContext` usage.

- **`HomeController`**
  - Home / Privacy / Terms / Error pages.

- **`Administration/UsersController`** (Administration Area, `ProgramAdministrator` only)
  - Injects `UserManager<ApplicationUser>`, `RoleManager<ApplicationRole>`, `IVSManagementService`, `IVSInsuranceService`
  - Lists, views, creates, edits, and soft-deletes users
  - Assigns roles and service station / insurance company / annual review company associations to users

#### Middleware & Extensions (`VMAPP.Web/Extensions`)

| Class | Role |
|---|---|
| `ServiceCollectionExtensions` | Fluent extension methods for all DI registrations; includes `InitializeDatabaseAsync` which auto-migrates and seeds on startup |
| `ApplicationsBuilderExtensions` | `InsertEndpoints()` - configures route mapping; `AddErrorHandler()` - error page middleware; `InsertUserInLog()` - enriches Serilog with the current user; `AddSerilog()` - configures Serilog |
| `SerilogExtensions` | Configures Serilog from `appsettings.json` |
| `IdentityOptionsProvider` | Centralizes Identity password, lockout, and sign-in policy configuration |
| `GlobalExceptionMiddleware` | Catches unhandled exceptions and returns a friendly error response |
| `EnforceAdmin2FAMiddleware` | Redirects `ProgramAdministrator` users to the 2FA setup page if 2FA is not enabled |

#### View Models

| Folder | View Models |
|---|---|
| `Models/VehicleServiceModels/` | `VehicleServiceViewModel`, `AddServiceViewModel`, `EditViewModel`, `DeleteViewModel`, `VehicleServiceDetailsViewModel`, `AddVehicleToServiceRequest` |
| `Models/VehicleViewModels/` | `VehicleIndexViewModel`, `AddVehicleViewModel`, `EditVehicleViewModel` |
| `Models/VehicleServiceCars/` | `ServiceVehicleViewModel` |
| `Models/ServiceRecordModels/` | `AddRecordViewModel`, `ServiceRecordFormViewModel` |
| `Models/InsuranceModels/` | `InsuranceCompanyViewModel`, `AddInsuranceCompanyViewModel`, `EditInsuranceCompanyViewModel`, `InsuranceCompanyDetailsViewModel`, `InsurancePolicyFormViewModel`, `InsurancePolicyDetailsViewModel`, `InsuranceClaimFormViewModel` |
| `Models/AnnualReviewModels/` | `AnnualReviewCompanyViewModel`, `AddAnnualReviewCompanyViewModel`, `EditAnnualReviewCompanyViewModel`, `AnnualReviewCompanyDetailsViewModel`, `AnnualReportDetailsViewModel`, `AnnualReportFormViewModel`, `VehicleWithReportRowViewModel` |
| `Areas/Administration/Models/` | `UserListViewModel`, `UserDetailsViewModel`, `CreateUserViewModel`, `EditUserViewModel` |

#### Views (`VMAPP.Web/Views`)

##### Home/

| View | Route | Description |
|---|---|---|
| `Index.cshtml` | `GET /` | Landing page. Displays the VMAPP welcome heading. Entry point for the application. |
| `Privacy.cshtml` | `GET /Home/Privacy` | Privacy Policy page. Structured into sections: Overview, Information We Collect, Purpose of Processing, Security Measures, and Contact. Effective date is rendered dynamically from `DateTime.UtcNow`. |

##### Shared/

| View | Description |
|---|---|
| `_Layout.cshtml` | Master layout applied to every page. Renders a **fixed header** with the VMAPP logo and navbar links, a **fixed footer** with copyright and a Privacy link, and the `@RenderBody()` content area. Includes Bootstrap, jQuery, and jQuery DataTables CSS/JS bundles. |
| `Error.cshtml` | Fallback error view used by the Identity area. Shows an error heading and, when a `RequestId` is available, displays it for tracing. Bound to `ErrorViewModel`. |
| `_ValidationScriptsPartial.cshtml` | Partial view that injects jQuery unobtrusive validation scripts. Included in form views that require client-side validation. |

##### Home/ - Custom Error Pages

All custom error pages are routed through `HomeController.Error(int? statusCode)`, which selects the appropriate view based on the HTTP status code. They are triggered in two ways:
- **Status code interception** - `UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}")` in `AddErrorHandler()` intercepts 4xx/5xx responses before they reach the client.
- **Unhandled exceptions** - `GlobalExceptionMiddleware` catches any uncaught exception, logs it with Serilog, and redirects to `/Home/Error?statusCode=500` (in Development the built-in developer exception page is shown instead).

| View | Status Code | Title | Error Code Colour | Description |
|---|---|---|---|---|
| `Error400.cshtml` | `400` | Bad Request | Yellow (`#f0ad4e`) | Shown when the server cannot understand the request. Prompts the user to check their input and try again. |
| `Error403.cshtml` | `403` | Access Denied | Orange (`#fd7e14`) | Shown when the user lacks permission to access a resource. Suggests contacting the administrator. |
| `Error404.cshtml` | `404` | Page Not Found | Grey (`#6c757d`) | Shown when a requested route or resource does not exist. Prompts the user to check the URL. |
| `Error500.cshtml` | All others | Something Went Wrong | Red (`#dc3545`) | Shown for unexpected server-side failures. Informs the user the team has been notified. |

Each page renders a large coloured status-code number, a short title, a plain-language description, and a **Back to Home** button, all inside the standard white card layout.

##### VehicleServices/

| View | Route | Model | Description |
|---|---|---|---|
| `Index.cshtml` | `GET /VehicleServices/Index` | `IEnumerable<VehicleServiceViewModel>` | Lists all service stations in a jQuery DataTables table. Provides a **Create New Service** button. Each row has **Details**, **Edit**, and **Delete** buttons. |
| `AddService.cshtml` | `GET/POST /VehicleServices/AddService` | `AddServiceViewModel` | Form to create a new service station. Fields: Name, City, Address, Phone, Email, Description. Includes server-side and client-side validation. |
| `EditService.cshtml` | `GET/POST /VehicleServices/EditService/{id}` | `EditViewModel` | Form to update an existing service station, pre-populated from the database. |
| `DeleteService.cshtml` | `GET/POST /VehicleServices/DeleteService/{id}` | `DeleteViewModel` | Delete confirmation form displaying all fields as read-only. |
| `Details.cshtml` | `GET /VehicleServices/Details/{id}` | `VehicleServiceDetailsViewModel` | Service station detail page with assigned vehicles table. **Add Vehicle** button and **Remove** column visible to `ProgramAdministrator` only. |
| `ServiceVehicle.cshtml` | `GET /VehicleServices/ServiceVehicle?vehicleId=&serviceId=` | `ServiceVehicleViewModel` | Per-vehicle service record management page. **Add Service Record** available to all authorised roles. **Edit** and **Delete** record buttons visible to `ProgramAdministrator` only. JS logic lives in `serviceVehicle.js`. |

##### Vehicle/

| View | Route | Model | Description |
|---|---|---|---|
| `Index.cshtml` | `GET /Vehicle/Index` | `IEnumerable<VehicleIndexViewModel>` | Lists all vehicles in a jQuery DataTables table. Provides an **Add New Vehicle** button. Each row has **Edit** and **Delete** buttons. JS logic lives in `vehicles.js`. |
| `AddVehicle.cshtml` | `GET/POST /Vehicle/AddVehicle` | `AddVehicleViewModel` | Form to create a new vehicle. Fields: VIN (17 characters, forced uppercase), Brand, Model, Year (1886–2100), Color, Vehicle Type (enum dropdown). |
| `EditVehicle.cshtml` | `GET/POST /Vehicle/EditVehicle/{id}` | `EditVehicleViewModel` | Form to update an existing vehicle, pre-populated from the database. |

##### Insurance/

| View | Route | Model | Description |
|---|---|---|---|
| `Index.cshtml` | `GET /Insurance/Index` | `IEnumerable<InsuranceCompanyViewModel>` | Lists all insurance companies. `ProgramAdministrator` sees all; `InsuranceCompany` users are redirected to their own details page. |
| `AddInsuranceCompany.cshtml` | `GET/POST /Insurance/AddInsuranceCompany` | `AddInsuranceCompanyViewModel` | Form to create a new insurance company. Restricted to `ProgramAdministrator`. |
| `EditInsuranceCompany.cshtml` | `GET/POST /Insurance/EditInsuranceCompany/{id}` | `EditInsuranceCompanyViewModel` | Form to update an existing insurance company. |
| `Details.cshtml` | `GET /Insurance/Details/{id}` | `InsuranceCompanyDetailsViewModel` | Company detail page with a table of associated vehicles and policies. **Add Policy** button and **Delete** policy column visible to `ProgramAdministrator` only. |
| `PolicyDetails.cshtml` | `GET /Insurance/PolicyDetails/{id}` | `InsurancePolicyDetailsViewModel` | Full policy view with vehicle and company info plus a table of claims. **Add Claim** available to all authorised roles. **Delete** claim column visible to `ProgramAdministrator` only. |

##### AnnualReview/

| View | Route | Model | Description |
|---|---|---|---|
| `Index.cshtml` | `GET /AnnualReview/Index` | `IEnumerable<AnnualReviewCompanyViewModel>` | Lists all annual review companies. `ProgramAdministrator` sees all; `AnnualReviewCompany` users are redirected to their own details page. |
| `AddAnnualReviewCompany.cshtml` | `GET/POST /AnnualReview/AddAnnualReviewCompany` | `AddAnnualReviewCompanyViewModel` | Form to create a new annual review company. Restricted to `ProgramAdministrator`. |
| `EditAnnualReviewCompany.cshtml` | `GET/POST /AnnualReview/EditAnnualReviewCompany/{id}` | `EditAnnualReviewCompanyViewModel` | Form to update an existing annual review company. |
| `Details.cshtml` | `GET /AnnualReview/Details/{id}` | `AnnualReviewCompanyDetailsViewModel` | Company detail page with a table of associated vehicles and their report numbers. **Add Report** available to all authorised roles. **Delete** company button visible to `ProgramAdministrator` only. |
| `ReportDetails.cshtml` | `GET /AnnualReview/ReportDetails/{id}` | `AnnualReportDetailsViewModel` | Full report view with vehicle info, inspection date, expiry date, pass/fail status, and notes. **Delete** report button visible to `ProgramAdministrator` only. |

##### Areas/Administration/

| View | Route | Description |
|---|---|---|
| `Users/Index.cshtml` | `GET /Administration/Users` | Lists all non-deleted users with their roles. |
| `Users/Details.cshtml` | `GET /Administration/Users/Details/{id}` | Full profile view for a single user. |
| `Users/Create.cshtml` | `GET/POST /Administration/Users/Create` | Form to create a new user with a role and optional service/company assignment. |
| `Users/Edit.cshtml` | `GET/POST /Administration/Users/Edit/{id}` | Form to update user details, role, and service/company assignment. |

##### Areas/Identity/

Standard ASP.NET Core Identity Razor Pages for user self-service:

- Login, Register, Logout, Forgot Password, Reset Password, Email Confirmation
- Account management: Change Password, Enable 2FA, Authenticator App setup, Recovery Codes, External Logins, Personal Data, Delete Account

---

### VMAPP.SandBox

A standalone **.NET 8 console application** designed to **run database migrations and seed data without starting the web application**.

#### Purpose

- Apply EF Core migrations to SQL Server independently of `VMAPP.Web`
- Execute the full seeding pipeline (`ApplicationDbContextSeeder`) to populate the database with complete sample data
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
5. Runs `ApplicationDbContextSeeder.SeedAsync()`, which chains all seeders in order:
   - `RoleSeeding` -> seeds roles
   - `AdminUserSeeding` -> seeds the admin account
   - `VehicleSeeding` -> seeds `Vehicles`
   - `VehicleServiceSeeding` -> seeds `VehicleServices`
   - `InsuranceCompanySeeding` -> seeds `InsuranceCompanies`
   - `AnnualReviewCompanySeeding` -> seeds `AnnualReviewCompanies`
   - `UserSeeding` -> seeds users and assignments
   - `TestUsersSeeding` -> seeds 4 ready-to-use test accounts
   - `ServiceRecordSeeding` -> seeds `ServiceRecords`
   - `InsurancePolicySeeding` -> seeds `InsurancePolicies`
   - `InsuranceClaimSeeding` -> seeds `InsuranceClaims`
   - `AnnualReportSeeding` -> seeds `AnnualReports`

Each individual seeder is **idempotent**: it skips insertion if rows already exist in the target table.

#### Running the SandBox

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

### VMAPP.Test

A **.NET 8 NUnit test project** that covers all service layer implementations using an **EF Core InMemory database** - no SQL Server instance is required to run the tests.

#### Test Stack

| Package | Version | Role |
|---|---|---|
| `NUnit` | 4.1.0 | Test framework |
| `NUnit3TestAdapter` | 4.5.0 | Visual Studio / `dotnet test` integration |
| `Microsoft.NET.Test.Sdk` | 17.9.0 | Test runner infrastructure |
| `Microsoft.EntityFrameworkCore.InMemory` | 8.0.24 | In-memory database provider for isolated tests |
| `NUnit.Analyzers` | 4.1.0 | Static analysis for NUnit assertions |

#### Test Classes

Each test class uses `[SetUp]` to create a fresh `InMemoryDatabase` (keyed by `Guid.NewGuid()`) and `[TearDown]` to dispose it, ensuring full test isolation.

**`VSManagementServiceTests`** - Tests for `VSManagementService` (`IVSManagementService`):

| Test | Description |
|---|---|
| `GetAllAsync_WhenEmpty_ReturnsEmptyList` | Empty DB returns empty list |
| `GetAllAsync_ReturnsServicesOrderedByNameThenCreatedOn` | Result is sorted alphabetically by name |
| `GetByIdAsync_WhenExists_ReturnsDto` | Correct DTO returned for a valid ID |
| `GetByIdAsync_WhenNotFound_ReturnsNull` | `null` returned for a missing ID |
| `CreateAsync_AddsServiceAndReturnsId` | Entity is persisted and a positive ID is returned |
| `UpdateAsync_WhenExists_UpdatesFieldsAndReturnsTrue` | Fields are updated and `true` is returned |
| `UpdateAsync_WhenNotFound_ReturnsFalse` | `false` returned for a missing ID |
| `DeleteAsync_WhenExists_DeletesAndReturnsTrue` | Entity is removed and `true` is returned |
| `DeleteAsync_WhenNotFound_ReturnsFalse` | `false` returned for a missing ID |
| `ExistsAsync_WhenExists_ReturnsTrue` | `true` for an existing entity |
| `ExistsAsync_WhenNotFound_ReturnsFalse` | `false` for a missing entity |
| `GetVehiclesByServiceIdAsync_ReturnsVehiclesForService` | Only vehicles for the given service are returned |
| `GetVehiclesServiceDetailsByIdAsync_WhenNotFound_ReturnsNull` | `null` returned for a missing ID |
| `GetVehiclesServiceDetailsByIdAsync_WhenExists_ReturnsDetailsWithVehicles` | Full details DTO with vehicle list returned |
| `GetVehicleByVinAsync_WhenExists_ReturnsVehicleDto` | Vehicle DTO returned for a valid VIN |
| `GetVehicleByVinAsync_WhenNotFound_ReturnsNull` | `null` for an unknown VIN |
| `AddVehicleToServiceAsync_WhenNotAlreadyAssigned_AddsRecordAndReturnsTrue` | Assignment record created, `true` returned |
| `AddVehicleToServiceAsync_WhenAlreadyAssigned_ReturnsFalse` | Duplicate assignment blocked, `false` returned |
| `RemoveVehicleFromServiceAsync_WhenNotAssigned_ReturnsFalseWithMessage` | Failure result with message for non-existent assignment |
| `RemoveVehicleFromServiceAsync_WhenHasPaidRecords_ReturnsFalseWithMessage` | Removal blocked when paid service records exist |
| `RemoveVehicleFromServiceAsync_WhenNoPaidRecords_RemovesAndReturnsSuccess` | Assignment and free records removed, success returned |
| `AssignUserAsync_WhenUserNotFound_ReturnsFalse` | `false` for a missing user |
| `AssignUserAsync_WhenServiceNotFound_ReturnsFalse` | `false` for a missing service |
| `AssignUserAsync_WhenUserAndServiceExist_SetsServiceIdAndReturnsTrue` | `VehicleServiceId` updated on the user, `true` returned |
| `AssignUserAsync_WhenServiceIdIsNull_ClearsAssignmentAndReturnsTrue` | `VehicleServiceId` cleared when `null` is passed |
| `CreateAsync_WhenCreatedOnIsProvided_PersistsProvidedDate` | Explicit `CreatedOn` value is stored |

**`VSCarsServiceTests`** - Tests for `VSCarsService` (`IVSCarsService`):

| Test | Description |
|---|---|
| `GetServiceWithVehiclesByNameAsync_WhenNameIsNullOrWhiteSpace_ReturnsNull` | Whitespace name returns `null` |
| `GetServiceWithVehiclesByNameAsync_WhenServiceNotFound_ReturnsNull` | Unknown name returns `null` |
| `GetServiceWithVehiclesByNameAsync_WhenServiceExists_ReturnsDtoWithName` | Correct DTO returned for a valid name |
| `AddVehicleToServiceAsync_AddsVehicleAndReturnsNewId` | Vehicle created and a positive ID is returned |
| `GetVehicleDetailsAsync_WhenVehicleNotFound_ReturnsNull` | `null` for a missing vehicle |
| `GetVehicleDetailsAsync_WhenVehicleExists_ReturnsDetailsWithServiceRecords` | Full vehicle details with service records returned |
| `UpdateVehicleAsync_WhenVehicleExists_UpdatesAndReturnsTrue` | Fields updated, `true` returned |
| `UpdateVehicleAsync_WhenVehicleNotFound_ReturnsFalse` | `false` for a missing vehicle |
| `DeleteVehicleAsync_WhenVehicleExists_DeletesAndReturnsTrue` | Vehicle removed, `true` returned |
| `DeleteVehicleAsync_WhenVehicleNotFound_ReturnsFalse` | `false` for a missing vehicle |
| `GetVehicleWithServiceRecordsAsync_ReturnsVehicleWithFilteredRecords` | Only records for the specified service are returned |
| `GetVehicleWithServiceRecordsAsync_WhenVehicleNotFound_ReturnsNull` | `null` for a missing vehicle |
| `GetServiceRecordByIdAsync_WhenExists_ReturnsDto` | Correct DTO returned for a valid ID |
| `GetServiceRecordByIdAsync_WhenNotFound_ReturnsNull` | `null` for a missing ID |
| `AddServiceRecordAsync_AddsRecordAndReturnsId` | Record created, positive ID returned |
| `UpdateServiceRecordAsync_WhenExists_UpdatesAndReturnsTrue` | Record updated, `true` returned |
| `UpdateServiceRecordAsync_WhenNotFound_ReturnsFalse` | `false` for a missing ID |
| `DeleteServiceRecordAsync_WhenExists_DeletesAndReturnsTrue` | Record removed, `true` returned |
| `DeleteServiceRecordAsync_WhenNotFound_ReturnsFalse` | `false` for a missing ID |

**`VSServiceTests`** - Tests for `VSService` (`IVSService`):

| Test | Description |
|---|---|
| `GetAllAsync_WhenEmpty_ReturnsEmptyList` | Empty DB returns empty list |
| `GetAllAsync_ReturnsVehiclesOrderedByBrandThenModel` | Result is sorted alphabetically by brand |
| `GetByIdAsync_WhenVehicleExists_ReturnsVehicleDto` | Correct DTO returned for a valid ID |
| `GetByIdAsync_WhenVehicleNotFound_ReturnsNull` | `null` for a missing ID |
| `CreateAsync_AddsVehicleAndReturnsNewId` | Vehicle created, positive ID returned |
| `UpdateAsync_WhenVehicleExists_UpdatesFieldsAndReturnsTrue` | Fields updated, `true` returned |
| `UpdateAsync_WhenVehicleNotFound_ReturnsFalse` | `false` for a missing ID |
| `DeleteAsync_WhenVehicleExists_DeletesAndReturnsTrue` | Vehicle removed, `true` returned |
| `DeleteAsync_WhenVehicleNotFound_ReturnsFalse` | `false` for a missing ID |

**`VSInsuranceServiceTests`** - Tests for `VSInsurance` (`IVSInsuranceService`):

| Test | Description |
|---|---|
| `GetAllAsync_WhenEmpty_ReturnsEmptyList` | Empty DB returns empty list |
| `GetAllAsync_ReturnsCompaniesOrderedByName` | Result is sorted alphabetically by company name |
| `GetByIdAsync_WhenExists_ReturnsDto` | Correct DTO returned for a valid ID |
| `GetByIdAsync_WhenNotFound_ReturnsNull` | `null` for a missing ID |
| `CreateAsync_WithCreatedById_CreatesCompanyAndReturnsId` | Company created with explicit creator, positive ID returned |
| `CreateAsync_WhenNoCreatedByIdAndNoUsers_ThrowsInvalidOperationException` | Exception thrown when no `CreatedById` and no users exist |
| `CreateAsync_WhenNoCreatedByIdButUserExists_UsesFirstUser` | Falls back to the first user in the DB as creator |
| `CreateAsync_WhenCreatedOnIsProvided_PersistsProvidedDate` | Explicit `CreatedOn` is persisted |
| `UpdateAsync_WhenExists_UpdatesFieldsAndReturnsTrue` | Fields updated, `true` returned |
| `UpdateAsync_WhenNotFound_ReturnsFalse` | `false` for a missing ID |
| `UpdateAsync_WhenModifiedByIdIsProvided_SetsModifiedById` | `ModifiedById` is persisted on the entity |
| `DeleteAsync_WhenExists_DeletesAndReturnsTrue` | Company removed, `true` returned |
| `DeleteAsync_WhenNotFound_ReturnsFalse` | `false` for a missing ID |
| `GetVehicleByVinAsync_WhenExists_ReturnsVehicleDto` | Vehicle DTO returned for a valid VIN |
| `GetVehicleByVinAsync_WhenNotFound_ReturnsNull` | `null` for an unknown VIN |
| `AddPolicyAsync_WithCreatedById_AddsPolicyAndReturnsId` | Policy created, positive ID returned |
| `AddPolicyAsync_WhenNoCreatedByIdButUserExists_UsesFirstUser` | Falls back to first user as creator |
| `DeletePolicyAsync_WhenExists_DeletesAndReturnsTrue` | Policy removed, `true` returned |
| `DeletePolicyAsync_WhenNotFound_ReturnsFalse` | `false` for a missing ID |
| `GetClaimByIdAsync_WhenExists_ReturnsDto` | Correct claim DTO returned |
| `GetClaimByIdAsync_WhenNotFound_ReturnsNull` | `null` for a missing ID |
| `AddClaimAsync_AddsClaimAndReturnsId` | Claim created, positive ID returned |
| `DeleteClaimAsync_WhenExists_DeletesAndReturnsTrue` | Claim removed, `true` returned |
| `DeleteClaimAsync_WhenNotFound_ReturnsFalse` | `false` for a missing ID |
| `GetCompanyWithVehiclesAsync_WhenNotFound_ReturnsNull` | `null` for a missing company |
| `GetCompanyWithVehiclesAsync_WhenExists_ReturnsCompanyWithVehicles` | Full DTO with vehicle list returned |
| `GetPolicyDetailsAsync_WhenNotFound_ReturnsNull` | `null` for a missing policy |
| `GetPolicyDetailsAsync_WhenExists_ReturnsPolicyWithVehicleAndCompanyInfo` | Full policy details with VIN and company name |
| `AssignUserAsync_WhenUserNotFound_ReturnsFalse` | `false` for a missing user |
| `AssignUserAsync_WhenCompanyNotFound_ReturnsFalse` | `false` for a missing company |
| `AssignUserAsync_WhenUserAndCompanyExist_SetsCompanyIdAndReturnsTrue` | `InsuranceCompanyId` updated on the user, `true` returned |
| `AssignUserAsync_WhenCompanyIdIsNull_ClearsAssignmentAndReturnsTrue` | `InsuranceCompanyId` cleared when `null` is passed |
| `GetCompanyIdByPolicyIdAsync_WhenPolicyNotFound_ReturnsNull` | `null` for a missing policy |
| `GetCompanyIdByPolicyIdAsync_WhenPolicyFound_ReturnsCompanyId` | Correct company ID returned |

#### Running the Tests

From the solution root:

```
dotnet test VMAPP.Test
```

or in Visual Studio use **Test Explorer** -> **Run All Tests**.

No database setup is required - all tests use the EF Core InMemory provider.

---

## Tech Stack

### Backend

- ASP.NET Core (.NET 8)
- MVC / Razor Views + Identity Razor Pages
- Entity Framework Core (SQL Server provider + InMemory for tests)
- ASP.NET Core Identity (custom `ApplicationUser` / `ApplicationRole`)
- SendGrid (transactional email)
- Serilog (structured logging)
- Layered architecture:
  - `VMAPP.Common` - shared constants
  - `VMAPP.Data` - EF entities & `ApplicationDbContext` + seeding
  - `VMAPP.Services` - services, interfaces & DTOs (business logic)
  - `VMAPP.Web` - controllers, view models & views (UI)
  - `VMAPP.SandBox` - standalone DB migration & seeding tool
  - `VMAPP.Test` - NUnit unit tests for the service layer

### Frontend

- Bootstrap
- jQuery
- jQuery DataTables
- Custom CSS (`site.css`, `homeStyle.css`, etc.)

### Testing

- NUnit 4
- EF Core InMemory provider (no external DB needed)

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (Express or Developer Edition)
- SQL Server Management Studio (SSMS)
- Visual Studio 2022 / VS Code
- (Optional) A SendGrid account and API key for email features
- (Optional) An authenticator app (Google Authenticator / Microsoft Authenticator) for admin 2FA

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

> **The database is created automatically on first run** via `MigrateAsync()` in `InitializeDatabaseAsync`. You only need to ensure the SQL Server instance is running and reachable.

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

### Step 4 - Run the Application

Set `VMAPP.Web` as the startup project and press **F5**.

On first launch the application will automatically:
1. Connect to SQL Server
2. Create the `VMAPP` database (via `MigrateAsync`)
3. Apply all EF Core migrations
4. Seed all roles, sample data, and test user accounts

> You can then log in immediately using any of the [pre-seeded test accounts](#-pre-seeded-test-accounts-no-email-confirmation-required).

#### Alternative: Manual Migration via VMAPP.SandBox

If you prefer to prepare the database before running the web app:

```
dotnet ef database update --project VMAPP.Data --startup-project VMAPP.SandBox
dotnet run --project VMAPP.SandBox
```

or in Visual Studio Package Manager Console (with `VMAPP.Data` as default project):

```
Update-Database
```

---

## How the Application Connects

All service registrations are in `ServiceCollectionExtensions.cs`. The database is configured via:

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

The application reads the connection string from `appsettings.json` and
connects to SQL Server using Entity Framework Core **inside the service layer**
(`VSManagementService`, `VSCarsService`, `VSService`, `VSInsurance`, `VSAnnualReview`), not directly in controllers.

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
- The database is **created and seeded automatically on startup** — no manual migration or seeding step is required for a first run.
- `VMAPP.SandBox` provides a web-free way to apply migrations and seed data - useful for testing the data layer in isolation.
- `VMAPP.Test` runs entirely against an EF Core InMemory provider - no SQL Server required.
- `ProgramAdministrator` users are required to have 2FA enabled before accessing the application (`EnforceAdmin2FAMiddleware`). Use the non-admin test accounts to explore the app without an authenticator app.
- The SendGrid API key is **not included** in the repository for security reasons. Email features require adding the key to `appsettings.json`. The pre-seeded test accounts do not require email confirmation.
- This configuration and the `sa` account/password are intended **only** for local development and testing. Do **not** use the `sa` account or weak passwords in production environments.