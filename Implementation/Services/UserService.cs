using Microsoft.AspNetCore.Identity;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;
using OnlineBookStoreMVC.Models;
using OnlineBookStoreMVC.Entities;
namespace OnlineBookStoreMVC.Implementation.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ApplicationDbContext _context;

    public UserService(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = _userManager.Users.Select(user => new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            FullName = user.FullName,
            Gender = user.Gender.ToString(),
            PhoneNumber = user.PhoneNumber
        }).ToList();

        return await Task.FromResult(users);
    }

    public async Task<UserDto> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        //var checkUser = _context.Users.Where(x => x.Id == id);
        var user2 = new User();

        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FullName = user.FullName,
            Gender = user.Gender.ToString(),

        };
    }

    public async Task<UserDto> CreateUserAsync(UserRequestModel userRequest)
    {
        var user = new User
        {
            UserName = userRequest.Username,
            Email = userRequest.Email,
            PhoneNumber = userRequest.PhoneNumber,
            FullName = userRequest.FullName,
            Gender = userRequest.Gender,
        };

        var result = await _userManager.CreateAsync(user, userRequest.Password);

        if (!result.Succeeded)
        {
            throw new Exception("User creation failed.");
        }

        return new UserDto
        {
            Username = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Gender = userRequest.Gender,
            FullName = userRequest.FullName
        };
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UserRequestModel userRequest)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null) return null;

        user.UserName = userRequest.Username;
        user.Email = userRequest.Email;
        user.FullName = userRequest.FullName;
        user.PhoneNumber = userRequest.PhoneNumber;
        user.Gender = userRequest.Gender;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception("User update failed.");
        }

        return new UserDto
        {
            Username = user.UserName,
            Email = user.Email,
            Gender = userRequest.Gender,
            FullName = userRequest.FullName,
            PhoneNumber = userRequest.PhoneNumber,
        };
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);

        return result.Succeeded;
    }

    public async Task<Status> Login(LoginModel login)
    {
        var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, login.RememberMe, lockoutOnFailure: false);

        return new Status
        {
            Success = result.Succeeded,
            Message = result.Succeeded ? "Login successful" : "Login failed"
        };
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<Status> ChangePasswordAsync(ChangePasswordModel model, string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return new Status
            {
                Success = false,
                Message = "User not found"
            };
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        return new Status
        {
            Success = result.Succeeded,
            Message = result.Succeeded ? "Password changed successfully" : "Password change failed"
        };
    }
}
