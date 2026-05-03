using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PharmacyInventoryAPI.Data;
using PharmacyInventoryAPI.DTOs;
using PharmacyInventoryAPI.Hubs;
using PharmacyInventoryAPI.Models;

namespace PharmacyInventoryAPI.Services
{
    public class MedicineService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<StockHub> _hubContext;
        private readonly ILogger<MedicineService> _logger;
        private readonly EmailService _emailService;

        public MedicineService(
            AppDbContext context,
            IHubContext<StockHub> hubContext,
            ILogger<MedicineService> logger,
            EmailService emailService)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<PagedResult<Medicine>> GetAll(
    string? category,
    string? search,
    int page = 1,
    int pageSize = 10,
    string? sortBy = null)
        {
            var query = _context.Medicines.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(m => m.Category == category);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(m => m.Name.Contains(search));

            // Sorting
            query = sortBy switch
            {
                "name" => query.OrderBy(m => m.Name),
                "price" => query.OrderBy(m => m.Price),
                "quantity" => query.OrderBy(m => m.Quantity),
                "expiryDate" => query.OrderBy(m => m.ExpiryDate),
                _ => query.OrderBy(m => m.Id)
            };

            var totalCount = await query.CountAsync();
            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Medicine>
            {
                Data = data,
                Pagination = new PaginationDto
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                }
            };
        }

        public async Task<Medicine?> GetById(int id)
            => await _context.Medicines.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<Medicine> Add(MedicineDto dto)
        {
            var medicine = new Medicine
            {
                Name = dto.Name,
                Category = dto.Category,
                Quantity = dto.Quantity,
                Price = dto.Price,
                ExpiryDate = dto.ExpiryDate,
                SupplierId = dto.SupplierId
            };
            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Medicine added: {Name}", medicine.Name);
            return medicine;
        }

        public async Task<bool> Update(int id, MedicineDto dto)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
            {
                _logger.LogWarning("Update failed - Medicine {Id} not found", id);
                return false;
            }

            medicine.Name = dto.Name;
            medicine.Category = dto.Category;
            medicine.Price = dto.Price;
            medicine.ExpiryDate = dto.ExpiryDate;
            medicine.Quantity = dto.Quantity;
            medicine.SupplierId = dto.SupplierId;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Medicine updated: {Id}", id);
            return true;
        }

        public async Task<bool> UpdateStock(int id, int quantity)
        {
            var medicine = await _context.Medicines
                .Include(m => m.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicine == null)
            {
                _logger.LogWarning(
                    "Stock update failed - Medicine {Id} not found", id);
                return false;
            }

            medicine.Quantity = quantity;
            await _context.SaveChangesAsync();

            if (quantity < 10)
            {
                _logger.LogWarning(
                    "Low stock alert - {Name} has {Qty} units",
                    medicine.Name, quantity);

                await _hubContext.Clients.All.SendAsync("LowStockAlert",
                    $"LOW STOCK: {medicine.Name} has only {quantity} units left!");

                if (medicine.Supplier != null)
                {
                    await _emailService.SendLowStockAlert(
                        medicine.Supplier.ContactEmail,
                        medicine.Name,
                        quantity);
                }
            }

            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
            {
                _logger.LogWarning("Delete failed - Medicine {Id} not found", id);
                return false;
            }

            _context.Medicines.Remove(medicine);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Medicine deleted: {Id}", id);
            return true;
        }

        public async Task<List<Medicine>> GetLowStock()
            => await _context.Medicines.AsNoTracking()
                .Where(m => m.Quantity < 10)
                .ToListAsync();

        public async Task<List<Medicine>> GetExpired()
            => await _context.Medicines.AsNoTracking()
                .Where(m => m.ExpiryDate < DateTime.UtcNow)
                .ToListAsync();

        public async Task<List<Medicine>> GetExpiringSoon(int days = 30)
    => await _context.Medicines.AsNoTracking()
        .Where(m => m.ExpiryDate >= DateTime.UtcNow
               && m.ExpiryDate <= DateTime.UtcNow.AddDays(days))
        .OrderBy(m => m.ExpiryDate)
        .ToListAsync();
    }
}