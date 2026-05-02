using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyInventoryAPI.Models
{
    public class Medicine
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relationship
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
    }
}