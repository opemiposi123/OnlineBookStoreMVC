using AspNetCoreHero.ToastNotification.Abstractions;
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
        private readonly INotyfService _notyf;

        public BookController(IBookService bookService,
                              IAuthorService authorService, 
                              IWebHostEnvironment webHostEnvironment,
                              ICategoryService catgoryService, 
                              INotyfService notyfService)
        {
            _bookService = bookService;
            _authorService = authorService;
            _webHostEnvironment = webHostEnvironment;
            _categoryService = catgoryService;
            _notyf = notyfService;
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
            _notyf.Success("Book Created Succesfully");
            return RedirectToAction(nameof(Index), new { id = book.Id });

            return View(bookRequest);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            ViewBag.Authors = await _authorService.GetAuthorSelectList();
            ViewBag.Categories = await _categoryService.GetCategorySelectList();
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

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
                Pages = book.Pages,
                Language = book.Language,
                CoverImageUrl = book.CoverImageUrl
            };

            return View(bookRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, BookRequestModel bookRequest)
        {
            var existingBook = await _bookService.GetBookByIdAsync(id);
            if (existingBook == null)
            {
                return NotFound();
            }

            existingBook.Title = bookRequest.Title;
            existingBook.Description = bookRequest.Description;
            existingBook.ISBN = bookRequest.ISBN;
            existingBook.Publisher = bookRequest.Publisher;
            existingBook.PublicationDate = bookRequest.PublicationDate;
            existingBook.Price = bookRequest.Price;
            existingBook.AuthorId = bookRequest.AuthorId;
            existingBook.CategoryId = bookRequest.CategoryId;
            existingBook.Pages = bookRequest.Pages;
            existingBook.Language = bookRequest.Language;

            // Handle Cover Image
            if (bookRequest.CoverImageFile != null && bookRequest.CoverImageFile.Length > 0)
            {
                existingBook.CoverImageUrl = await _bookService.SaveFileAsync(bookRequest.CoverImageFile);
            }

            await _bookService.UpdateBookAsync(id, bookRequest);
            _notyf.Success("Book Updated Succesfully");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("DeleteBook")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var success = await _bookService.DeleteBookAsync(id);
            if (!success)
            {
                _notyf.Success("Coud'nt Delete Book mate");
            }
            _notyf.Success("Book Deleted Successfully mate");
            return RedirectToAction("Index");
        }


    }
}