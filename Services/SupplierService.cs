using Microsoft.EntityFrameworkCore;
using PharmacyInventoryAPI.Data;
using PharmacyInventoryAPI.DTOs;
using PharmacyInventoryAPI.Models;

namespace PharmacyInventoryAPI.Services
{
    public class SupplierService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(AppDbContext context,
            ILogger<SupplierService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Supplier>> GetAll()
            => await _context.Suppliers
                .AsNoTracking()
                .ToListAsync();

        public async Task<Supplier?> GetById(int id)
            => await _context.Suppliers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task<List<Medicine>> GetMedicinesBySupplier(int id)
            => await _context.Medicines
                .AsNoTracking()
                .Where(m => m.SupplierId == id)
                .ToListAsync();

        public async Task<Supplier> Add(SupplierDto dto)
        {
            var supplier = new Supplier
            {
                Name = dto.Name,
                ContactEmail = dto.ContactEmail
            };
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Supplier added: {Name}", supplier.Name);
            return supplier;
        }

        public async Task<bool> Update(int id, SupplierDto dto)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return false;

            supplier.Name = dto.Name;
            supplier.ContactEmail = dto.ContactEmail;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return false;

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Supplier deleted: {Id}", id);
            return true;
        }
    }
}