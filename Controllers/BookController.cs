
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
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotyfService _notyf;

        public BookController(IBookService bookService,
                              IWebHostEnvironment webHostEnvironment,
                              ICategoryService categoryService,
                              INotyfService notyfService)
        {
            _bookService = bookService;
            _webHostEnvironment = webHostEnvironment;
            _categoryService = categoryService;
            _notyf = notyfService;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooksAsync();
            return View(books);
        }
         public async Task<IActionResult> GetBooksMissingCoverImage()
        {
            var books = await _bookService.GetBooksMissingCoverImageAsync();
            return View(books);
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCoverImage(Guid bookId, IFormFile coverImageFile)
        {
            if (coverImageFile == null || coverImageFile.Length == 0)
            {
                _notyf.Error("Please select a valid image file.");
                return RedirectToAction(nameof(GetBooksMissingCoverImage));
            }

            var result = await _bookService.AddCoverImageAsync(bookId, coverImageFile);
            if (result)
            {
                _notyf.Success("Cover image updated successfully.");
            }
            else
            {
                _notyf.Error("Failed to update cover image. Please try again.");
            }

            return RedirectToAction(nameof(GetBooksMissingCoverImage));
        }


        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryService.GetCategorySelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookRequestModel bookRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetCategorySelectList();
                return View(bookRequest);
            }

            var book = await _bookService.CreateBookAsync(bookRequest);
            if (book != null)
            {
                _notyf.Success("Book Created Successfully");
                return RedirectToAction(nameof(Index));
            }

            _notyf.Error("An error occurred while creating the book. Please try again.");
            ViewBag.Categories = await _categoryService.GetCategorySelectList();
            return View(bookRequest);
        }


        public async Task<IActionResult> Edit(Guid id)
        {
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
                Price = book.Price,
                Author = book.Author,
                CategoryId = book.CategoryId,
                Pages = book.Pages,
                Language = book.Language,
                TotalQuantity = book.TotalQuantity
            };

            return View(bookRequest);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BookRequestModel bookRequest)
        {
            await _bookService.UpdateBookAsync(id, bookRequest);
            _notyf.Success("Book Updated Successfully");
            return RedirectToAction(nameof(Index));

            return View(bookRequest);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            await _bookService.DeleteBookAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DownloadExcelTemplate()
        {
            var fileResult = await _bookService.DownloadExcelTemplateAsync();
            return fileResult;
        }

        public async Task<IActionResult> UploadExcelTemplate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcelTemplate(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                _notyf.Error("Please upload a valid Excel file.");
                return RedirectToAction("Index");
            }

            var fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension != ".xlsx")
            {
                _notyf.Error("Please upload a valid Excel file (.xlsx).");
                return RedirectToAction("Index");
            }

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                // Process the Excel file and upload books
                await _bookService.UploadBooksFromExcelAsync(stream);

                _notyf.Success("Books have been successfully uploaded.");
            }
            catch (Exception ex)
            {
                _notyf.Error($"An error occurred while processing the file: {ex.Message}");
            }

            return RedirectToAction("Index");
        }
    }
}

