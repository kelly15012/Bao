namespace Bao.Models
{
    public class ReportView
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class ProductSalesViewModel
    {
        public string ProductName { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class SalesReportViewModel
    {
        public List<ReportView> ReportData { get; set; }
        public List<ProductSalesViewModel> ProductSalesData { get; set; }
        public decimal TotalSales { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
