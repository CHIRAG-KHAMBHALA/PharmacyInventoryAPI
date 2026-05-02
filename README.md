# Pharmacy Inventory Management API

ASP.NET Core (.NET 8) REST API for managing pharmacy inventory with JWT auth, role-based access, EF Core, and SignalR low-stock alerts.

## Features
- JWT authentication with roles (Admin/Pharmacist)
- Medicine CRUD with category filtering and search
- Expiry tracking and low-stock alerts
- SignalR hub for real-time notifications
- EF Core code-first with SQL Server
- Serilog logging
- Swagger UI

## Tech Stack
- .NET 8, ASP.NET Core Web API
- Entity Framework Core, SQL Server
- SignalR
- Serilog

## Setup
1. Restore packages
2. Update configuration values (see `appsettings.json`)
3. Run migrations if needed
4. Start the API

## Configuration
Set these values using environment variables or user-secrets:
- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Email:SmtpHost`
- `Email:SmtpPort`
- `Email:FromEmail`
- `Email:Password`

## Running
- Swagger UI: `https://localhost:{PORT}/swagger`
- SignalR Hub: `/hubs/stock`

## Notes
- Do not commit secrets or `logs/` to source control.
