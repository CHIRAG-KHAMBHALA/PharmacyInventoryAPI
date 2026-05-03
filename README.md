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
## 🗄️ Database Schema

### Supplier

* **Id** (PK)
* **Name**
* **ContactEmail**

---

### Medicine

* **Id** (PK)
* **Name**
* **Category**
* **Quantity**
* **Price**
* **ExpiryDate**
* **CreatedAt**
* **SupplierId** (FK → Supplier.Id)

---

### User

* **Id** (PK)
* **Username**
* **PasswordHash**
* **Role**

---

### 🔗 Relationships

* One **Supplier** → Many **Medicines**
* Each **Medicine** belongs to one **Supplier**

---

### 🔐 Notes

* No direct FK between **User** and other tables
* Role-based access handled using **JWT Claims**

---

## How to Run

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB works)
- Visual Studio 2022

### Steps

1. Clone the repo
```bash
git clone https://github.com/chiragsk0106/PharmacyInventoryAPI.git
cd PharmacyInventoryAPI
```

2. Update `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PharmacyDB;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "YourSecretKeyMinimum32Characters",
    "Issuer": "PharmacyAPI",
    "Audience": "PharmacyUsers"
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "FromEmail": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

3. Apply migrations
```bash
dotnet ef database update
```

4. Run the project
```bash
dotnet run
```

5. Open Swagger UI
https://localhost:7191/swagger

6. Open SignalR test client
https://localhost:7191/signalr-test.html

---

## Running Tests

```bash
dotnet test PharmacyInventoryAPI.Tests
```

5 unit tests — all passing:
- Add medicine successfully
- Get low stock medicines only
- Delete returns false if not found
- Correct category saved
- Get expired medicines only

---
## 📁 Project Structure

```
PharmacyInventoryAPI/
│
├── Controllers/
│   ├── AuthController.cs
│   ├── MedicineController.cs
│   ├── SupplierController.cs
│   └── DashboardController.cs
│
├── Services/
│   ├── AuthService.cs
│   ├── MedicineService.cs
│   ├── SupplierService.cs
│   ├── DashboardService.cs
│   ├── EmailService.cs
│   └── ExpiredMedicineJob.cs
│
├── Models/
│   ├── Medicine.cs
│   ├── Supplier.cs
│   └── User.cs
│
├── DTOs/
│   ├── AuthDto.cs
│   ├── MedicineDto.cs
│   ├── SupplierDto.cs
│   ├── PaginationDto.cs
│   └── DashboardDto.cs
│
├── Data/
│   └── AppDbContext.cs
│
├── Hubs/
│   └── StockHub.cs
│
├── Migrations/
│
├── wwwroot/
│   └── signalr-test.html
│
├── logs/
│
├── Program.cs
│
└── PharmacyInventoryAPI.Tests/
    └── MedicineServiceTests.cs
```


---

## Key Design Decisions

- **Role hardcoded on server** — clients cannot self-assign Admin role
- **AsNoTracking** on all read-only queries for performance
- **DTO validation** with DataAnnotations on all input models
- **Global exception handling** middleware for consistent error responses
- **Dependency Injection** throughout — testable and maintainable
- **Supplier-Medicine FK** — tracks medicine source for reorder workflow

---

## Author

**Chirag Khambhala**   
chiragsk0106@gmail.com