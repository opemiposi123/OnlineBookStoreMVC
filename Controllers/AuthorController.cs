using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Biography")] AuthorRequestModel authorRequest)
        {
            if (ModelState.IsValid)
            {
                var author = await _authorService.CreateAuthorAsync(authorRequest);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Biography")] AuthorRequestModel authorRequest)
        {
            if (ModelState.IsValid)
            {
                var updatedAuthor = await _authorService.UpdateAuthorAsync(id, authorRequest);
                if (updatedAuthor == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(authorRequest);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var success = await _authorService.DeleteAuthorAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
