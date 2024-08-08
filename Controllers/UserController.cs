using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;
using OnlineBookStoreMVC.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // Display all users
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsersAsync();
        return View(users); // This will render a view named "Index" with the list of users
    }

    // Display details of a single user
    [HttpGet("{id}")]
    public async Task<IActionResult> Details(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null) return NotFound(); // Or you can return a view like "NotFound"

        return View(user); // This will render a view named "Details" with user details
    }

    // Display a form for creating a new user
    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View(); // This will render a view named "Create"
    }

    // Handle form submission to create a new user
    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserRequestModel userRequest)
    {
        if (ModelState.IsValid)
        {
            await _userService.CreateUserAsync(userRequest);
            return RedirectToAction(nameof(Index)); // Redirect to the list of users
        }

        return View(userRequest); // If validation fails, re-render the form with validation errors
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

    // Handle the deletion of a user
    [HttpGet("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
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

    // Display the change password form
    [HttpGet("ChangePassword")]
    public IActionResult ChangePassword() 
    {
        return View(); // This will render a view named "ChangePassword"
    }

    // Handle change password form submission
    [HttpPost("ChangePassword")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model, [FromQuery] string username)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.ChangePasswordAsync(model, username);

            if (result.Success)
                return RedirectToAction(nameof(Index)); // Redirect to the user list on successful password change

            ModelState.AddModelError(string.Empty, result.Message);
        }

        return View(model); // Re-render the change password form with validation errors
    }
}
