using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Enums;
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
        Task<Status> AssignRoleToUserAsync(string userId, Role role);
        Task<Status> ForgotPasswordAsync(string email);
        Task<Status> VerifyResetCodeAsync(string email, string code);
        Task<Status> ChangePasswordAsync(string email, string newPassword, string confirmPassword);
        List<User> GetAllUser();
    }
}
