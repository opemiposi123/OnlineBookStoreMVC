namespace OnlineBookStoreMVC.DTOs
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<ShoppingCartItemDto> ShoppingCartItems { get; set; } = new List<ShoppingCartItemDto>();
        public decimal TotalPrice => ShoppingCartItems.Sum(item => item.TotalPrice);
    }
}
