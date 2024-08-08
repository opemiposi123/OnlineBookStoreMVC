namespace OnlineBookStoreMVC.DTOs
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public string Publisher { get; set; }
        public DateTime PublicationDate { get; set; }
        public decimal Price { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ReviewDto> Reviews { get; set; }
        public string CoverImageUrl { get; set; }
        public int Pages { get; set; }
        public string Language { get; set; }
    }
}
