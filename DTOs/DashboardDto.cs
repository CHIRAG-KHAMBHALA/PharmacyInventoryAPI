namespace PharmacyInventoryAPI.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalMedicines { get; set; }
        public int LowStockCount { get; set; }
        public int ExpiredCount { get; set; }
        public int ExpiringSoonCount { get; set; }
        public int TotalSuppliers { get; set; }
        public decimal TotalInventoryValue { get; set; }
    }
  
}