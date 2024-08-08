namespace OnlineBookStoreMVC.Entities
{
    public class ShoppingCart : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    }
}
