using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookStoreMVC.Models.RequestModels
{
    public class ShoppingCartItemRequestModel
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; } = 0;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
