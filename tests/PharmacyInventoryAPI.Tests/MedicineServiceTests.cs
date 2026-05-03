using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PharmacyInventoryAPI.Data;
using PharmacyInventoryAPI.DTOs;
using PharmacyInventoryAPI.Hubs;
using PharmacyInventoryAPI.Models;
using PharmacyInventoryAPI.Services;

namespace PharmacyInventoryAPI.Tests
{
    public class MedicineServiceTests
    {
        private AppDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private MedicineService GetService(AppDbContext db)
        {
            var hub = new Mock<IHubContext<StockHub>>();
            var logger = new Mock<ILogger<MedicineService>>();
            var emailLogger = new Mock<ILogger<EmailService>>();
            var config = new Mock<IConfiguration>();

            var emailService = new EmailService(
                config.Object, emailLogger.Object);

            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            hub.Setup(h => h.Clients).Returns(mockClients.Object);
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);

            return new MedicineService(
                db, hub.Object, logger.Object, emailService);
        }

        [Fact]
        public async Task Add_ShouldAddMedicine_Successfully()
        {
            var db = GetInMemoryDb();
            var service = GetService(db);

            var dto = new MedicineDto
            {
                Name = "Paracetamol",
                Category = "Painkiller",
                Quantity = 100,
                Price = 9.99m,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                SupplierId = 1
            };

            var result = await service.Add(dto);

            Assert.NotNull(result);
            Assert.Equal("Paracetamol", result.Name);
        }

        [Fact]
        public async Task GetLowStock_ShouldReturn_OnlyLowStockMedicines()
        {
            var db = GetInMemoryDb();
            var service = GetService(db);

            await service.Add(new MedicineDto
            {
                Name = "HighStock",
                Category = "A",
                Quantity = 100,
                Price = 5m,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                SupplierId = 1
            });

            await service.Add(new MedicineDto
            {
                Name = "LowStock",
                Category = "B",
                Quantity = 5,
                Price = 3m,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                SupplierId = 1
            });

            var result = await service.GetLowStock();

            Assert.Single(result);
            Assert.Equal("LowStock", result[0].Name);
        }

        [Fact]
        public async Task Delete_ShouldReturn_FalseIfNotFound()
        {
            var db = GetInMemoryDb();
            var service = GetService(db);

            var result = await service.Delete(999);

            Assert.False(result);
        }

        [Fact]
        public async Task Add_ShouldReturn_CorrectCategory()
        {
            var db = GetInMemoryDb();
            var service = GetService(db);

            var dto = new MedicineDto
            {
                Name = "Ibuprofen",
                Category = "Painkiller",
                Quantity = 50,
                Price = 15m,
                ExpiryDate = DateTime.UtcNow.AddYears(2),
                SupplierId = 1
            };

            var result = await service.Add(dto);

            Assert.Equal("Painkiller", result.Category);
        }

        [Fact]
        public async Task GetExpired_ShouldReturn_OnlyExpiredMedicines()
        {
            var db = GetInMemoryDb();
            var service = GetService(db);

            await service.Add(new MedicineDto
            {
                Name = "NotExpired",
                Category = "A",
                Quantity = 50,
                Price = 5m,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                SupplierId = 1
            });

            await service.Add(new MedicineDto
            {
                Name = "Expired",
                Category = "B",
                Quantity = 10,
                Price = 3m,
                ExpiryDate = DateTime.UtcNow.AddDays(-1),
                SupplierId = 1
            });

            var result = await service.GetExpired();

            Assert.Single(result);
            Assert.Equal("Expired", result[0].Name);
        }
    }
}