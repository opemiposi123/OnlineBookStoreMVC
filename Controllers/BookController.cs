using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Controllers
{
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var success = await _bookService.DeleteBookAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}





//using Microsoft.AspNetCore.Mvc;
//using OnlineBookStoreMVC.Implementation.Interface;
//using OnlineBookStoreMVC.Models.RequestModels;

//namespace OnlineBookStoreMVC.Controllers
//{
//    public class BookController : Controller
//    {
//        private readonly IBookService _bookService;

//        public BookController(IBookService bookService)
//        {
//            _bookService = bookService;
//        }

//        // GET: Books
//        public async Task<IActionResult> Index()
//        {
//            var books = await _bookService.GetAllBooksAsync();
//            return View(books);
//        }

//        // GET: Books/Details/{id}
//        public async Task<IActionResult> Details(Guid id)
//        {
//            var book = await _bookService.GetBookByIdAsync(id);

//            if (book == null)
//            {
//                return NotFound();
//            }

//            return View(book);
//        }

//        // GET: Books/Create
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // POST: Books/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(BookRequestModel bookRequest)
//        {
//            if (ModelState.IsValid)
//            {
//                await _bookService.CreateBookAsync(bookRequest);
//                return RedirectToAction(nameof(Index));
//            }
//            return View(bookRequest);
//        }

//        // GET: Books/Edit/{id}
//        public async Task<IActionResult> Edit(Guid id)
//        {
//            var book = await _bookService.GetBookByIdAsync(id);

//            if (book == null)
//            {
//                return NotFound();
//            }

//            var bookRequest = new BookRequestModel
//            {
//                Title = book.Title,
//                Description = book.Description,
//                ISBN = book.ISBN,
//                Publisher = book.Publisher,
//                PublicationDate = book.PublicationDate,
//                Price = book.Price,
//                AuthorId = book.AuthorId,
//                CategoryId = book.CategoryId,
//                CoverImageUrl = book.CoverImageUrl,
//                Pages = book.Pages,
//                Language = book.Language
//            };

//            return View(bookRequest);
//        }

//        // POST: Books/Edit/{id}
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(Guid id, BookRequestModel bookRequest)
//        {
//            if (ModelState.IsValid)
//            {
//                var updatedBook = await _bookService.UpdateBookAsync(id, bookRequest);
//                if (updatedBook == null)
//                {
//                    return NotFound();
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            return View(bookRequest);
//        }

//        // GET: Books/Delete/{id}
//        public async Task<IActionResult> Delete(Guid id)
//        {
//            var book = await _bookService.GetBookByIdAsync(id);

//            if (book == null)
//            {
//                return NotFound();
//            }

//            return View(book);
//        }

//        // POST: Books/Delete/{id}
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(Guid id)
//        {
//            var result = await _bookService.DeleteBookAsync(id);
//            if (!result)
//            {
//                return NotFound();
//            }

//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
