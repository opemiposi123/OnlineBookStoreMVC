using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookStoreMVC.Entities
{
    public class ShoppingCartItem : BaseEntity
    {
        public Guid ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
    }
}
