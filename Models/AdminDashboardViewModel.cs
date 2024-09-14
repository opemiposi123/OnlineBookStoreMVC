namespace OnlineBookStoreMVC.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalOrders { get; set; }
        public List<BookInventoryDto> BookInventories { get; set; }
    }

    public class BookInventoryDto
    {
        public string Title { get; set; }
        public int TotalQuantity { get; set; }
        public int RemainingQuantity { get; set; }
    }
}
