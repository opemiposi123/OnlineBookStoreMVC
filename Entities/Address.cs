namespace OnlineBookStoreMVC.Entities
{
    public class Address : BaseEntity
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? AddittionalPhoneNumber { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string DeliveryAddress { get; set; } 
        public bool IsDefault { get; set; }
        public User User { get; set; }
        public ICollection<Order> Orders { get; set; }
    }

}
