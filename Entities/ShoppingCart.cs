namespace OnlineBookStoreMVC.Entities
{
    public class ShoppingCart : BaseEntity
    {
        public string UserId { get; set; }
        public User? User { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    }
}
