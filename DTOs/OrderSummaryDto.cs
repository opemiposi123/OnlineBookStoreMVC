namespace OnlineBookStoreMVC.DTOs
{
    public class OrderSummaryDto
    {
        public Guid Id { get; set; } 
        public ShoppingCartDto ShoppingCart { get; set; }
        public AddressDto Address { get; set; }
        public string UserId { get; set; }
        public string OrderId { get; set; } 
        public decimal OrderTotal => ShoppingCart?.TotalPrice ?? 0m;
    }

}
