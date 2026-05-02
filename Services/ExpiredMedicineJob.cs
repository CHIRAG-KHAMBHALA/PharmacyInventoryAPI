using Microsoft.EntityFrameworkCore;
using PharmacyInventoryAPI.Data;

namespace PharmacyInventoryAPI.Services
{
    public class ExpiredMedicineJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ExpiredMedicineJob> _logger;

        public ExpiredMedicineJob(
            IServiceScopeFactory scopeFactory,
            ILogger<ExpiredMedicineJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckExpiredMedicines();

                // Har 24 ghante mein run hoga
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task CheckExpiredMedicines()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var expiredMedicines = await context.Medicines
                .Where(m => m.ExpiryDate < DateTime.UtcNow
                       && m.Quantity > 0)
                .ToListAsync();

            foreach (var medicine in expiredMedicines)
            {
                medicine.Quantity = 0;
                _logger.LogWarning(
                    "Expired medicine auto-zeroed: {Name}, ExpiryDate: {Date}",
                    medicine.Name, medicine.ExpiryDate);
            }

            if (expiredMedicines.Any())
            {
                await context.SaveChangesAsync();
                _logger.LogInformation(
                    "{Count} expired medicines quantity set to 0",
                    expiredMedicines.Count);
            }
        }
    }
}