using Microsoft.AspNetCore.Identity;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Enums;
using OnlineBookStoreMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace OnlineBookStoreMVC.Implementation.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context, ILogger<UserService> logger,IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            _logger.LogInformation("Getting all users.");
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? Role.User.ToString();

                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = Enum.Parse<Role>(role),
                    FullName = user.FullName,
                    Gender = user.Gender.ToString(),
                    PhoneNumber = user.PhoneNumber
                });
            }

            _logger.LogInformation("Successfully retrieved all users.");
            return userDtos;
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            _logger.LogInformation($"Getting user by ID: {id}");
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found.");
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? Role.User.ToString();

            _logger.LogInformation($"Successfully retrieved user with ID: {id}");
            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Role = Enum.Parse<Role>(role),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                Gender = user.Gender.ToString()
            };
        }

        public async Task<UserDto> CreateUserAsync(UserRequestModel userRequest)
        {
            _logger.LogInformation($"Creating user: {userRequest.Username}");
            var user = new User
            {
                UserName = userRequest.Username,
                Email = userRequest.Email,
                PhoneNumber = userRequest.PhoneNumber,
                FullName = userRequest.FullName,
                Gender = userRequest.Gender,
                Role = Role.User
            };

            var result = await _userManager.CreateAsync(user, userRequest.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("User creation failed.");
                throw new Exception("User creation failed.");
            }

            _logger.LogInformation($"User created successfully: {user.UserName}");

            await _userManager.AddToRoleAsync(user, Role.User.ToString());
            await _context.SaveChangesAsync();

            //// Send welcome email
            //var emailResponse = await _emailService.SendNotificationToUserAsync(userRequest);
            //if (!emailResponse.Success)
            //{
            //    _logger.LogWarning("Failed to send welcome email.");
            //    // Optionally, handle the failure case or notify admin
            //}

            return new UserDto
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = userRequest.Gender,
                FullName = userRequest.FullName,
                Role = Role.User
            };
        }

        public async Task<UserDto> UpdateUserAsync(Guid id, UserRequestModel userRequest)
        {
            _logger.LogInformation($"Updating user with ID: {id}");
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found.");
                return null;
            }

            user.UserName = userRequest.Username;
            user.Email = userRequest.Email;
            user.FullName = userRequest.FullName;
            user.PhoneNumber = userRequest.PhoneNumber;
            user.Gender = userRequest.Gender;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError("User update failed.");
                throw new Exception("User update failed.");
            }

            _logger.LogInformation($"User with ID {id} updated successfully.");
            return new UserDto
            {
                Username = user.UserName,
                Email = user.Email,
                Gender = userRequest.Gender,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
            };
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            _logger.LogInformation($"Deleting user with ID: {id}");
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found.");
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User with ID {id} deleted successfully.");
            }
            else
            {
                _logger.LogError($"Failed to delete user with ID {id}.");
            }
            return result.Succeeded;
        }
        public async Task<Status> Login(LoginModel login)
        {
            _logger.LogInformation($"Attempting to log in user: {login.Username}");
            var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, login.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User {login.Username} logged in successfully.");
            }
            else
            {
                _logger.LogWarning($"Failed to log in user: {login.Username}");
            }

            return new Status
            {
                Success = result.Succeeded,
                Message = result.Succeeded ? "Login successful" : "Login failed"
            };
        }

        public async Task LogoutAsync()
        {
            _logger.LogInformation("User logging out.");
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out successfully.");
        }

        public async Task<Status> AssignRoleToUserAsync(string userId, Role role)
        {
            _logger.LogInformation($"Assigning role {role} to user with ID: {userId}");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found.");
                return new Status
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    _logger.LogError($"Failed to remove user with ID {userId} from current roles.");
                    return new Status
                    {
                        Success = false,
                        Message = "Failed to remove user from current roles"
                    };
                }
            }

            var result = await _userManager.AddToRoleAsync(user, role.ToString());
            if (result.Succeeded)
            {
                _logger.LogInformation($"Role {role} assigned successfully to user with ID: {userId}");
            }
            else
            {
                _logger.LogError($"Failed to assign role {role} to user with ID: {userId}");
            }

            return new Status
            {
                Success = result.Succeeded,
                Message = result.Succeeded ? "Role assigned successfully" : "Failed to assign role"
            };
        }

       


    }
}
