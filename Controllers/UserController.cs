using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;
using OnlineBookStoreMVC.Models;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using OnlineBookStoreMVC.Entities;

[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly UserManager<User> _userManager;
    private readonly INotyfService _notyf;

    public UserController(IUserService userService, IEmailService emailService, INotyfService notyf, UserManager<User> userManager)
    {
        _userService = userService;
        _emailService = emailService;
        _notyf = notyf;
        _userManager = userManager;
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var paginatedUsers = await _userService.GetPaginatedUsersAsync(page, pageSize);
        return View(paginatedUsers);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> Details(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null) return NotFound();

        return View(user);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(UserRequestModel userRequest, UserRequestModel profile)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _userService.CreateUserAsync(userRequest);
                var emailResponse = await _emailService.SendMessageToUserAsync(profile);

                _notyf.Success("User has been successfully created.");
                return RedirectToAction("Index", "Store");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
        return View(userRequest);
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null) return NotFound();

        return View(new UserRequestModel
        {
            Username = user.Username,
            Email = user.Email
        });
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UserRequestModel userRequest)
    {
        if (ModelState.IsValid)
        {
            await _userService.UpdateUserAsync(id, userRequest);
            _notyf.Success("User details have been successfully updated.");
            return RedirectToAction(nameof(Index));
        }

        return View(userRequest);
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        _notyf.Success("User has been successfully deleted.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginModel login)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.Login(login);

            if (result.Success)
            {
                var user = await _userManager.FindByNameAsync(login.Username);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin") || roles.Contains("SuperAdmin"))
                    {
                        _notyf.Success("Login successful.");
                        return RedirectToAction("Index", "DashboardCount");
                    }
                    else if (roles.Contains("User"))
                    {
                        _notyf.Success("Login successful.");
                        return RedirectToAction("Index", "Store");
                    }
                }
            }
            else
            {
                _notyf.Error("Invalid username or password. Please try again.");
            }
        }

        return View(login);
    }

    [HttpPost("Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _userService.LogoutAsync();
        _notyf.Success("Logout successful.");
        return RedirectToAction("Login", "User");
    }

    [HttpGet("ForgotPassword")]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromForm] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email is required.");
        }

        var response = await _userService.ForgotPasswordAsync(email);

        if (response.Success)
        {
            _notyf.Success("Reset code has been sent to your email.");
            return RedirectToAction("VerifyResetCode", new { email = email });
        }

        return BadRequest(response.Message);
    }

    [HttpGet("VerifyResetCode")]
    public IActionResult VerifyResetCode(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email is required.");
        }

        var model = new VerifyResetCodeModel
        {
            Email = email
        };
        return View(model);
    }

    [HttpPost("VerifyResetCode")]
    public async Task<IActionResult> VerifyResetCode(VerifyResetCodeModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _userService.VerifyResetCodeAsync(model.Email, model.Code);

        if (result.Success)
        {
            _notyf.Success("Reset code verified successfully.");
            // Redirect to the Reset Password page
            return RedirectToAction("ChangePassword", new { email = model.Email });
        }

        // If code verification fails, display an error
        ModelState.AddModelError("", "Invalid or expired code.");
        return View(model);
    }

    [HttpGet("ChangePassword")]
    public IActionResult ChangePassword(string email)
    {
        var model = new ChangePasswordModel
        {
            Email = email
        };
        return View(model);
    }

    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _userService.ChangePasswordAsync(model.Email, model.NewPassword, model.ConfirmNewPassword);

        if (result.Success)
        {
            _notyf.Success("Password changed successfully.");
            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", result.Message);
        return View(model);
    }
}
