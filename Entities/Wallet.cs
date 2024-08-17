namespace OnlineBookStoreMVC.Entities
{
    public class Wallet : BaseEntity
    {
        public Guid? UserId { get; set; }
        public User User { get; set; }
        public double Amount { get; set; }
    }
}

