namespace OnlineBookStoreMVC.DTOs
{
    public class MonthlyReportDto
    {
        public string UserFullName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ReportOrderItemDto> Items { get; set; }
    }
}
