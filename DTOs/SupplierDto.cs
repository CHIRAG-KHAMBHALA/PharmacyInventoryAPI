using System.ComponentModel.DataAnnotations;

namespace PharmacyInventoryAPI.DTOs
{
    public class SupplierDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;
    }
}