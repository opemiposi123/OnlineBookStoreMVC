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
        Task<List<AddressDto>> GetUniqueAddressesByUserIdAsync(string userId);
    }
}
