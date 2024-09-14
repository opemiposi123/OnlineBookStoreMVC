using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
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

        // GET: Book
        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooksAsync();
            return View(books);
        }

        // GET: Book/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Json(book);
        }

        // GET: Book/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Authors = await _authorService.GetAuthorSelectList();
            ViewBag.Categories = await _categoryService.GetCategorySelectList();
            return View();
        }

        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookRequestModel bookRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Authors = await _authorService.GetAuthorSelectList();
                ViewBag.Categories = await _categoryService.GetCategorySelectList();
                return View(bookRequest);
            }

            var book = await _bookService.CreateBookAsync(bookRequest);
            _notyf.Success("Book Created Successfully");
            return RedirectToAction(nameof(Index));
        }

        //// GET: Books/Create
        //public async Task<IActionResult> Create()
        //{
        //    ViewBag.Authors = await _authorService.GetAuthorSelectList();
        //    ViewBag.Categories = await _categoryService.GetCategorySelectList();
        //    return View();
        //}

        //// POST: Books/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(BookRequestModel bookRequest)
        //{

        //    var book = await _bookService.CreateBookAsync(bookRequest);
        //    _notyf.Success("Book Created Succesfully");
        //    return RedirectToAction(nameof(Index), new { id = book.Id });

        //    return View(bookRequest);
        //}

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
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
                CoverImageUrl = book.CoverImageUrl,
                Pages = book.Pages,
                Language = book.Language,
                TotalQuantity = book.TotalQuantity
            };

            return View(bookRequest);
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BookRequestModel bookRequest)
        {
            if (ModelState.IsValid)
            {
                await _bookService.UpdateBookAsync(id, bookRequest);
                _notyf.Success("Book Updated Succesfully");
                return RedirectToAction(nameof(Index));
            }

            return View(bookRequest);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            await _bookService.DeleteBookAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
