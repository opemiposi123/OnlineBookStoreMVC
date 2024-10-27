namespace OnlineBookStoreMVC.DTOs
{
    public class ReportOrderItemDto
    {
        public string BookTitle { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
