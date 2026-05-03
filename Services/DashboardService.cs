using Microsoft.EntityFrameworkCore;
using PharmacyInventoryAPI.Data;
using PharmacyInventoryAPI.DTOs;

namespace PharmacyInventoryAPI.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummary()
        {
            var now = DateTime.UtcNow;
            var soonThreshold = now.AddDays(30);

            var medicines = await _context.Medicines
                .AsNoTracking()
                .ToListAsync();

            var totalSuppliers = await _context.Suppliers
                .AsNoTracking()
                .CountAsync();

            return new DashboardSummaryDto
            {
                TotalMedicines = medicines.Count,
                LowStockCount = medicines
                    .Count(m => m.Quantity < 10),
                ExpiredCount = medicines
                    .Count(m => m.ExpiryDate < now),
                ExpiringSoonCount = medicines
                    .Count(m => m.ExpiryDate >= now
                           && m.ExpiryDate <= soonThreshold),
                TotalSuppliers = totalSuppliers,
                TotalInventoryValue = medicines
                    .Sum(m => m.Price * m.Quantity)
            };
        }
        public async Task<object> GetStockReport()
        {
            var result = await _context.Database
                .SqlQueryRaw<StockReportDto>(@"
            SELECT 
                s.Name AS SupplierName,
                COUNT(m.Id) AS TotalMedicines,
                SUM(m.Quantity) AS TotalStock,
                SUM(CASE WHEN m.Quantity < 10 THEN 1 ELSE 0 END) 
                    AS LowStockCount,
                SUM(m.Price * m.Quantity) AS InventoryValue
            FROM Suppliers s
            LEFT JOIN Medicines m ON m.SupplierId = s.Id
            GROUP BY s.Name
            ORDER BY InventoryValue DESC
        ").ToListAsync();

            return result;
        }
    }
}