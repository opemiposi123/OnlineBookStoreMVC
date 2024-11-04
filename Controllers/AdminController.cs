using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Enums;
using OnlineBookStoreMVC.Implementation.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace OnlineBookStoreMVC.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        public IActionResult Index()
        {
            return View();
        }

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Admin/AssignRole
        [HttpGet]
        public async Task<IActionResult> AssignRole(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.UserId = user.Id;
            ViewBag.Username = user.Username;
            ViewBag.Roles = Enum.GetValues(typeof(Role))
                               .Cast<Role>()
                               .Select(r => new SelectListItem
                               {
                                   Value = r.ToString(),
                                   Text = r.ToString()
                               }).ToList();

            return View();
        }

        // POST: Admin/AssignRole
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, Role role)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var result = await _userService.AssignRoleToUserAsync(userId, role);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                ViewBag.Roles = Enum.GetValues(typeof(Role))
                                   .Cast<Role>()
                                   .Select(r => new SelectListItem
                                   {
                                       Value = r.ToString(),
                                       Text = r.ToString()
                                   }).ToList();

                ViewBag.Username = (await _userService.GetUserByIdAsync(userId)).Username;
                return View();
            }

            return RedirectToAction("Index", "User");
        }


    }
}
