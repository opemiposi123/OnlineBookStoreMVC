using OnlineBookStoreMVC.Entities;

namespace OnlineBookStoreMVC.Models.RequestModels
{
    public class BookRequestModel : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public string Publisher { get; set; }
        public DateTime PublicationDate { get; set; }
        public decimal Price { get; set; }
        public Guid AuthorId { get; set; }
        public Guid CategoryId { get; set; }
        public IFormFile CoverImageFile { get; set; }
        public string CoverImageUrl { get; set; }
        public int Pages { get; set; }
        public string Language { get; set; }
        public int TotalQuantity { get; set; }
    }
}
