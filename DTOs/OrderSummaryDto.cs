namespace OnlineBookStoreMVC.DTOs
{
    public class OrderSummaryDto
    {
        public ShoppingCartDto ShoppingCart { get; set; } // The shopping cart containing the items being purchased
        public AddressDto Address { get; set; } // The delivery address
        public string UserId { get; set; }

        // Optional: You can add additional properties like OrderTotal, Tax, ShippingCost, etc.
        public decimal OrderTotal => ShoppingCart?.TotalPrice ?? 0m;
    }

}
