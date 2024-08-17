using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]

    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(IBookService bookService, IAuthorService authorService, IWebHostEnvironment webHostEnvironment, ICategoryService catgoryService)
        {
            _bookService = bookService;
            _authorService = authorService;
            _webHostEnvironment = webHostEnvironment;
            _categoryService = catgoryService;
        }

        [AllowAnonymous]
        // GET: Books
        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooksAsync();
            return View(books);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Authors = await _authorService.GetAuthorSelectList();
            ViewBag.Categories = await _categoryService.GetCategorySelectList();
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookRequestModel bookRequest)
        {

            var book = await _bookService.CreateBookAsync(bookRequest);
            return RedirectToAction(nameof(Details), new { id = book.Id });

            return View(bookRequest);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewBag.Authors = await _authorService.GetAuthorSelectList();
            ViewBag.Categories = await _categoryService.GetCategorySelectList();
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            // Convert BookDto to BookRequestModel for editing
            var bookRequest = new BookRequestModel
            {
                Title = book.Title,
                Description = book.Description,
                ISBN = book.ISBN,
                Publisher = book.Publisher,
                PublicationDate = book.PublicationDate,
                Price = book.Price,
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId,
                //CoverImageFile = book.CoverImageUrl,
                Pages = book.Pages,
                Language = book.Language
            };
            return View(bookRequest);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BookRequestModel bookRequest)
        {
            if (ModelState.IsValid)
            {
                var book = await _bookService.UpdateBookAsync(id, bookRequest);
                if (book == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Details), new { id = book.Id });
            }
            return View(bookRequest);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var success = await _bookService.DeleteBookAsync(id);
            if (!success)
            {
                // Handle the case when the book is not found
                ViewData["ErrorMessage"] = "The book could not be found.";
                return View("Error");
            }

            return RedirectToAction("Index");
        }

    }
}