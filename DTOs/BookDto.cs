namespace OnlineBookStoreMVC.DTOs
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public string Publisher { get; set; }
        public decimal Price { get; set; }
        public string Author { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CoverImageUrl { get; set; }
        public int Pages { get; set; }
        public string Language { get; set; }
        public int TotalQuantity { get; set; }
    }
}
