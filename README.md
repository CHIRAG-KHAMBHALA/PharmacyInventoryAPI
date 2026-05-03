# Pharmacy Inventory Management System

A role-based RESTful API built with **ASP.NET Core / .NET 8** for managing 
medicine inventory in a pharmacy. Designed to solve real pharmacy problems — 
manual stock tracking, missed expiry dates, and no real-time alerts.

---

## Problem It Solves

| Problem | Solution |
|---|---|
| Manual stock tracking | Digital CRUD with search + filter |
| Expired medicines not caught | Auto-expiry tracking + background job |
| No low stock alerts | SignalR real-time + supplier email |
| Anyone can modify data | JWT role-based access control |
| No audit trail | Serilog structured logging |

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core / .NET 8 |
| ORM | Entity Framework Core (Code-First) |
| Database | SQL Server |
| Authentication | JWT Bearer Tokens |
| Real-time | SignalR |
| Email | SMTP (Gmail) |
| Logging | Serilog |
| Documentation | Swagger UI |
| Testing | xUnit + Moq |
| Background Jobs | IHostedService |

---

## Features

### Auth Module
- `POST /api/Auth/register` — Register as Pharmacist
- `POST /api/Auth/login` — Login + get JWT token
- `POST /api/Auth/register-admin` — Register Admin (Admin only)

### Medicine Module
- `GET /api/Medicine` — Get all with search, filter, pagination, sorting
- `GET /api/Medicine/{id}` — Get by ID
- `POST /api/Medicine` — Add medicine (Admin only)
- `PUT /api/Medicine/{id}` — Update medicine (Admin only)
- `DELETE /api/Medicine/{id}` — Delete medicine (Admin only)
- `PATCH /api/Medicine/{id}/stock` — Update stock (Admin + Pharmacist)
- `GET /api/Medicine/low-stock` — Get medicines with qty < 10
- `GET /api/Medicine/expired` — Get expired medicines
- `GET /api/Medicine/expiring-soon?days=30` — Expiring in N days

### Supplier Module
- `GET /api/Supplier` — Get all suppliers
- `GET /api/Supplier/{id}` — Get by ID
- `GET /api/Supplier/{id}/medicines` — Get medicines by supplier
- `POST /api/Supplier` — Add supplier (Admin only)
- `PUT /api/Supplier/{id}` — Update supplier (Admin only)
- `DELETE /api/Supplier/{id}` — Delete supplier (Admin only)

### Dashboard Module
- `GET /api/Dashboard/summary` — Total medicines, low stock count,
  expired count, expiring soon, total suppliers, inventory value (Admin only)
- `GET /api/Dashboard/stock-report` — Per-supplier stock report (Admin only)

---

## Role Permissions

| Feature / Endpoint             | Admin  |  Pharmacist |
| ------------------------------ | :---:  | :--------: |
| View medicines                 |   ✅   |      ✅     |
| Low stock / Expired / Expiring |   ✅   |      ✅     |
| Add / Edit / Delete medicine   |   ✅   |      ❌     |
| Update stock                   |   ✅   |      ✅     |
| Supplier management            |   ✅   |      ❌     |
| Dashboard                      |   ✅   |      ❌     |
| Register Admin                 |   ✅   |      ❌     |


---

## Low Stock Workflow

When stock drops below 10 units:
1. **Serilog** logs a warning
2. **SignalR** broadcasts real-time alert to all connected clients
3. **Email** is sent automatically to the medicine's supplier

---

## Background Job

`ExpiredMedicineJob` runs every 24 hours using `IHostedService`.
Automatically sets quantity to 0 for all expired medicines and logs each action.

---

## Database Schema