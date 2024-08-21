namespace OnlineBookStoreMVC.Entities
{
    public class ForgotPasswordCode : BaseEntity
    {
        public string UserId { get; set; }
        public string Code { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool IsUsed { get; set; }
    }
}
