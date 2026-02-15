# Vehicle Management System (VMAPP)

A web-based Vehicle Management System built with ASP.NET Core (.NET 8)
and Razor Pages / MVC.\
The application helps manage vehicles, their service history,
insurance/inspection data, and related records.

------------------------------------------------------------------------

## Features

### Vehicle Management

-   Create, edit, and delete vehicle entries
-   Store core vehicle information: VIN, brand, model, year, color, type

### Service Management

-   List available vehicle services
-   Filter vehicles by selected service
-   Assign vehicles to services

### Service Records

-   Add, edit, and delete per-vehicle service records
-   Track service date, cost, and description
-   Modal-based UI with DataTables for a responsive service history
    table

### UI / UX

-   Fixed header and footer with consistent navigation
-   Global background image
-   Content displayed in centered white "cards" for readability
-   Bootstrap-based layout
-   jQuery DataTables integration for tabular data

### Privacy & Terms

-   Dedicated Privacy Policy page (`/Home/Privacy`)
-   Dedicated Terms and Conditions page (`/Home/Terms`)
-   Both rendered in readable white content boxes

------------------------------------------------------------------------

## Tech Stack

### Backend

-   ASP.NET Core (.NET 8)
-   MVC / Razor Views
-   Entity Framework Core
-   SQL Server (via `ApplicationDbContext`)

### Frontend

-   Bootstrap
-   jQuery
-   jQuery DataTables
-   Custom CSS (`site.css`, `homeStyle.css`, etc.)

------------------------------------------------------------------------

# Getting Started

## Prerequisites

-   .NET 8 SDK
-   SQL Server (Express or Developer Edition)
-   SQL Server Management Studio (SSMS)
-   Visual Studio 2022 / VS Code

------------------------------------------------------------------------

# Database Setup (Testing Environment)

This project connects to SQL Server using the following credentials:

``` json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\Server1;Database=VMAPP;User=sa;Password=pb;Encrypt=False;TrustServerCertificate=True;"
}
```

## Database Credentials

-   Server: localhost`\Server1`{=tex}
-   Database: VMAPP
-   Username: sa
-   Password: pb
-   Authentication Mode: SQL Server Authentication

⚠️ These credentials are for local development/testing only.

------------------------------------------------------------------------

## Step 1 -- Install SQL Server

Install SQL Server Express or Developer Edition and SQL Server
Management Studio (SSMS).

Ensure your SQL instance name is:

Server1

------------------------------------------------------------------------

## Step 2 -- Enable SQL Server Authentication Mode

1.  Open SSMS\
2.  Connect to localhost`\Server1`{=tex}\
3.  Right-click the server → Properties\
4.  Open Security tab\
5.  Select: SQL Server and Windows Authentication mode\
6.  Restart SQL Server service

------------------------------------------------------------------------

## Step 3 -- Enable and Configure `sa` Login

1.  In SSMS → Security → Logins\
2.  Right-click `sa` → Properties\
3.  Set password to: pb\
4.  In Status tab ensure Login is Enabled

------------------------------------------------------------------------

## Step 4 -- Create the Database

### Option A -- Manual Creation

1.  Right-click Databases\
2.  New Database\
3.  Name it: VMAPP

### Option B -- Using Entity Framework (Recommended)

Run:

dotnet ef database update

or in Visual Studio Package Manager Console:

Update-Database

This will create the database and all tables automatically.

------------------------------------------------------------------------

# How the Application Connects

In Program.cs:

``` csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

The application reads the connection string from appsettings.json and
connects to SQL Server using Entity Framework Core.

------------------------------------------------------------------------

# Run the Application

dotnet run

or press F5 in Visual Studio.

------------------------------------------------------------------------

# Important

This configuration is intended only for local development and testing.
Do NOT use sa account or simple passwords in production environments.
