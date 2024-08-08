namespace OnlineBookStoreMVC.DTOs
{
    public class ShoppingCartItemDto
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; }  // Optional: Additional book details
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Quantity * Price;
    }
}
