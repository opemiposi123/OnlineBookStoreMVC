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
        public IFormFile CoverImageFile { get; set; } // For uploading a new image
        public string CoverImageUrl { get; set; } // To hold the existing image URL
        public int Pages { get; set; }
        public string Language { get; set; }
    }
}
