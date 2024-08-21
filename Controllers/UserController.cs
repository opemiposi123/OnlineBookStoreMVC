using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;
using OnlineBookStoreMVC.Models;
using Microsoft.AspNetCore.Identity;
using OnlineBookStoreMVC.Entities;
using Microsoft.AspNetCore.Authorization;

[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;

    public UserController(IUserService userService, IEmailService emailService)
    {
        _userService = userService;
        _emailService = emailService;
    }

    // Display all users
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsersAsync();
        return View(users);
    }

    // Display details of a single user
    [HttpGet("{id}")]
    public async Task<IActionResult> Details(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null) return NotFound();

        return View(user);
    }

    // Display a form for creating a new user
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

                return RedirectToAction("Index", "Store");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message); 
            }
        }
        return View(userRequest);
    }

    // Display a form for editing an existing user
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

    // Handle form submission to update an existing user
    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UserRequestModel userRequest)
    {
        if (ModelState.IsValid)
        {
            await _userService.UpdateUserAsync(id, userRequest);
            return RedirectToAction(nameof(Index));
        }

        return View(userRequest);
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return RedirectToAction(nameof(Index));
    }
    // Display the login form
    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View(); // This will render a view named "Login"
    }

    // Handle login form submission
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginModel login)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.Login(login);

            if (result.Success)
                return RedirectToAction("Index", "Store"); // Redirect to the user list on successful login

            ModelState.AddModelError(string.Empty, result.Message);
        }

        return View(login); // Re-render the login form with validation errors
    }

    // Handle logout
    [HttpPost("Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _userService.LogoutAsync();
        return RedirectToAction("Login", "User"); // Redirect to the login page after logout
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
            // Redirect to VerifyResetCode with the email as a query parameter
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
            // Return the view with the current model state if the model is invalid
            return View(model);
        }

        var result = await _userService.VerifyResetCodeAsync(model.Email, model.Code);

        if (result.Success)
        {
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

        var result = await _userService.ChangePasswordAsync(model.Email, model.NewPassword,model.ConfirmNewPassword);

        if (result.Success)
        {
            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", result.Message);
        return View(model);
    }
}
