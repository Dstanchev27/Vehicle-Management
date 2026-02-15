# Vehicle Management System (VMAPP)

A web-based Vehicle Management System built with ASP.NET Core (.NET 8) and Razor Pages / MVC.  
The application helps manage vehicles, their service history, insurance/inspection data, and related records.

## Features

- **Vehicle Management**
  - Create, edit, and delete vehicle entries
  - Store core vehicle information: VIN, brand, model, year, color, type

- **Service Management**
  - List available vehicle services
  - Filter vehicles by selected service
  - Assign vehicles to services

- **Service Records**
  - Add, edit, and delete per-vehicle service records
  - Track service date, cost, and description
  - Modal-based UI with DataTables for a responsive service history table

- **UI / UX**
  - Fixed header and footer with consistent navigation
  - Global background image
  - Content displayed in centered white “cards” for readability
  - Bootstrap-based layout
  - jQuery DataTables integration for tabular data

- **Privacy & Terms**
  - Dedicated Privacy Policy page (`/Home/Privacy`)
  - Dedicated Terms and Conditions page (`/Home/Terms`)
  - Both rendered in readable white content boxes

## Tech Stack

- **Backend**
  - ASP.NET Core (.NET 8)
  - MVC / Razor Views
  - Entity Framework Core
  - SQL Server (via `ApplicationDbContext`)

- **Frontend**
  - Bootstrap
  - jQuery
  - jQuery DataTables
  - Custom CSS (`site.css`, `homeStyle.css`, etc.)

## Project Structure

- `VMAPP.Web/`
  - `Controllers/`
    - `HomeController.cs` – Home, Privacy, error pages
    - `VehicleServiceCarsController.cs` – Vehicles and service records
    - `Management`-related controllers (if present)
  - `Views/`
    - `Home/`
      - `Index.cshtml` – Home page
      - `Privacy.cshtml` – Privacy Policy
      - `Terms.cshtml` – Terms & Conditions
    - `VehicleServiceCars/`
      - Views for listing, adding, editing vehicles and their service records
    - `Shared/`
      - `_Layout.cshtml` – global layout (header, footer, background)
  - `wwwroot/`
    - `css/`
      - `site.css` – global styling and imports
      - `homeStyle.css` – privacy/terms page styling
      - other feature-specific CSS
    - `js/`
      - `vehicleServiceRecords.js` – DataTables + modal logic for service records
      - `site.js` – global scripts
    - `img/` – background and logo images

- `VMAPP.Data/`
  - `ApplicationDbContext.cs` – EF Core context
  - Entity models: `Vehicle`, `VehicleService`, `ServiceRecord`, etc.

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (local or remote)
- Visual Studio 2022 or VS Code (or any editor with .NET support)

### Setup

1. **Clone the repository**
