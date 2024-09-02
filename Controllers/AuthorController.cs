using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly INotyfService _notyf;

        public AuthorController(IAuthorService authorService, INotyfService notyfService)
        {
            _authorService = authorService;
            _notyf = notyfService;
        }

        // GET: Authors
        public async Task<IActionResult> Index()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return View(authors);
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        [HttpPost]
        public async Task<IActionResult> Create(AuthorRequestModel authorRequest)
        {
            if (ModelState.IsValid)
            {
                var author = await _authorService.CreateAuthorAsync(authorRequest);
                _notyf.Success("Author Created Succesfully");
                return RedirectToAction(nameof(Index));
            }
            return View(authorRequest);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            var authorRequest = new AuthorRequestModel
            {
                Name = author.Name,
                Biography = author.Biography
            };
            return View(authorRequest);
        }

        // POST: Authors/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, AuthorRequestModel authorRequest)
        {
            if (ModelState.IsValid)
            {
                var updatedAuthor = await _authorService.UpdateAuthorAsync(id, authorRequest);
                _notyf.Success("Author Updated Succesfully");
                if (updatedAuthor == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(authorRequest);
        }

        // POST: Authors/Delete/5
        [HttpGet("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var success = await _authorService.DeleteAuthorAsync(id);
            _notyf.Success("Author Deleted Succesfully");
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
