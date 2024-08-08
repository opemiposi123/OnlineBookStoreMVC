using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Models;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(string id);
        Task<UserDto> CreateUserAsync(UserRequestModel userRequest);
        Task<UserDto> UpdateUserAsync(Guid id, UserRequestModel userRequest);
        Task<bool> DeleteUserAsync(Guid id);
        Task<Status> Login(LoginModel login);
        Task LogoutAsync();
        Task<Status> ChangePasswordAsync(ChangePasswordModel model, string username);
    }
}
