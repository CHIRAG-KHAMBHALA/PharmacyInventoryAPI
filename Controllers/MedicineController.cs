using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyInventoryAPI.DTOs;
using PharmacyInventoryAPI.Services;

namespace PharmacyInventoryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicineController : ControllerBase
    {
        private readonly MedicineService _medicineService;
        private readonly ILogger<MedicineController> _logger;

        public MedicineController(MedicineService medicineService,
            ILogger<MedicineController> logger)
        {
            _medicineService = medicineService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
        [FromQuery] string? category,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null)
        {
            var result = await _medicineService
                .GetAll(category, search, page, pageSize, sortBy);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var medicine = await _medicineService.GetById(id);
            if (medicine == null)
            {
                _logger.LogWarning("Medicine {Id} not found", id);
                return NotFound(new { message = $"Medicine with id {id} not found" });
            }
            return Ok(medicine);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(MedicineDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var medicine = await _medicineService.Add(dto);
            return CreatedAtAction(nameof(GetById),
                new { id = medicine.Id }, medicine);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, MedicineDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _medicineService.Update(id, dto);
            if (!result)
                return NotFound(new { message = $"Medicine with id {id} not found" });

            return NoContent();
        }

        [HttpPatch("{id}/stock")]
        [Authorize(Roles = "Admin,Pharmacist")]  // ← dono kar sakte hain
        public async Task<IActionResult> UpdateStock(int id, UpdateStockDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _medicineService.UpdateStock(id, dto.Quantity);
            if (!result)
                return NotFound(new { message = $"Medicine with id {id} not found" });

            return Ok(new { message = "Stock updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _medicineService.Delete(id);
            if (!result)
                return NotFound(new { message = $"Medicine with id {id} not found" });

            return NoContent();
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock()
        {
            var medicines = await _medicineService.GetLowStock();
            return Ok(medicines);
        }

        [HttpGet("expired")]
        public async Task<IActionResult> GetExpired()
        {
            var medicines = await _medicineService.GetExpired();
            return Ok(medicines);
        }

        [HttpGet("expiring-soon")]
        public async Task<IActionResult> GetExpiringSoon(
        [FromQuery] int days = 30)
        {
            var medicines = await _medicineService.GetExpiringSoon(days);
            return Ok(medicines);
        }
    }
}