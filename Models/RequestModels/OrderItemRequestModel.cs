namespace OnlineBookStoreMVC.Models.RequestModels
{
    public class OrderItemRequestModel
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
