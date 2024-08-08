namespace OnlineBookStoreMVC.Models.RequestModels
{
    public class OrderRequestModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public List<OrderItemRequestModel> OrderItems { get; set; } = new List<OrderItemRequestModel>();
    }
}
