namespace OnlineBookStoreMVC.Entities
{
    public class Payment : BaseEntity
    {
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public string Status { get; set; }
    }
}
