using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookStoreMVC.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string ISBN { get; set; }
    public string Publisher { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
    public string Author { get; set; }
    public Guid CategoryId { get; set; }
    public string? CoverImageUrl { get; set; }
    public int Pages { get; set; }
    public string Language { get; set; }
    public int TotalQuantity { get; set; }
    public Category Category { get; set; }
}
