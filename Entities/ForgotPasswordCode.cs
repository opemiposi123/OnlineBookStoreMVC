namespace OnlineBookStoreMVC.Entities
{
    public class ForgotPasswordCode : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool IsUsed { get; set; }
    }
}
