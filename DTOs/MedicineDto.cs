using System.ComponentModel.DataAnnotations;

namespace PharmacyInventoryAPI.DTOs
{
    public class MedicineDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [Range(0, 10000)]
        public int Quantity { get; set; }

        [Range(0.01, 99999.99)]
        public decimal Price { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public int SupplierId { get; set; }
    }

    public class UpdateStockDto
    {
        [Range(0, 10000)]
        public int Quantity { get; set; }
    }
}