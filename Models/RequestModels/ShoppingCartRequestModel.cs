namespace OnlineBookStoreMVC.Models.RequestModels
{
    public class ShoppingCartRequestModel
    {
        public Guid UserId { get; set; }
        public List<ShoppingCartItemRequestModel> ShoppingCartItems { get; set; }
    }
}
