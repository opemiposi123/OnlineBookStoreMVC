using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;
using OnlineBookStoreMVC.Models;
using Microsoft.AspNetCore.Identity;
using OnlineBookStoreMVC.Entities;

[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;
    //private readonly INotyfService notyfService;
    private readonly IEmailService _emailService;

    public UserController(IUserService userService, IEmailService emailService)
    {
        _userService = userService;
        _emailService = emailService;
    }

    // Display all users
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
    [ValidateAntiForgeryToken]
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

   
}
