namespace Bao.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
        public DateTime SaleDate { get; set; }
    }
}
