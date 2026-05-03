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
            var result = await _context.Suppliers
                .AsNoTracking()
                .Select(s => new
                {
                    SupplierName = s.Name,
                    TotalMedicines = s.Medicines.Count,
                    TotalStock = s.Medicines.Sum(m => m.Quantity),
                    LowStockCount = s.Medicines
                        .Count(m => m.Quantity < 10),
                    InventoryValue = s.Medicines
                        .Sum(m => m.Price * m.Quantity)
                })
                .OrderByDescending(s => s.InventoryValue)
                .ToListAsync();

            return result;
        }
    }
}