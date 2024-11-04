using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IAddressService
    {
        Task<AddressDto> AddAddressAsync(AddressRequestModel model, string userId);
        Task<AddressDto> GetAddressByUserIdAsync(string userId);
        Task<IEnumerable<AddressDto>> GetAllAddressesByUserIdAsync(string userId);
        Task<AddressDto> GetAddressByIdAsync(Guid addressId);
        Task<AddressDto> UpdateAddressAsync(Guid addressId, AddressRequestModel model);
        Task SetDefaultAddressAsync(string userId, Guid selectedAddressId);
    }
}
