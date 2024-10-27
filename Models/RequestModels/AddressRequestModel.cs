using OnlineBookStoreMVC.Entities;

namespace OnlineBookStoreMVC.Models.RequestModels
{
    public class AddressRequestModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? AddittionalPhoneNumber { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string DeliveryAddress { get; set; }
        public bool IsDefault { get; set; }
    }
}
