using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBookStoreMVC.Implementation.Interface;

namespace BookStore.Controllers
{
    [AllowAnonymous]
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookService _bookService;

        public StoreController(ApplicationDbContext context,IBookService bookService)
        {
            _context = context;
            _bookService = bookService;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var books = await _bookService.GetAllBooksAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                         b.AuthorName.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            return View(books);
        }


            public async Task<IActionResult> Details(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }
    }
}
