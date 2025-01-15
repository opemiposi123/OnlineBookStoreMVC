using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryService _categoryService;

        public BookService(ApplicationDbContext context, ICategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }

        public List<Book> GetAllBooks()
        {
            return _context.Books.ToList();
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _context.Books
                .Where(b => b.CoverImageUrl != null)
                .Include(b => b.Category)
                .ToListAsync();

            return books.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                ISBN = b.ISBN,
                Publisher = b.Publisher,
                Price = b.Price,
                Author = b.Author,
                CategoryId = b.CategoryId,
                CategoryName = b.Category.Name,
                CoverImageUrl = b.CoverImageUrl,
                Pages = b.Pages,
                Language = b.Language,
                TotalQuantity = b.TotalQuantity
            });
        }

        public async Task<PaginatedDto<BookDto>> GetPaginatedBooksAsync(int page, int pageSize)
        {
            var totalBooks = await _context.Books.CountAsync(b => b.CoverImageUrl != null);

            var books = await _context.Books
                .Where(b => b.CoverImageUrl != null)
                .Include(b => b.Category)
                .OrderBy(b => b.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var bookDtos = books.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                ISBN = b.ISBN,
                Publisher = b.Publisher,
                Price = b.Price,
                Author = b.Author,
                CategoryId = b.CategoryId,
                CategoryName = b.Category.Name,
                CoverImageUrl = b.CoverImageUrl,
                Pages = b.Pages,
                Language = b.Language,
                TotalQuantity = b.TotalQuantity
            }).ToList();

            return new PaginatedDto<BookDto>
            {
                Items = bookDtos,
                TotalCount = totalBooks,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<BookDto>> GetBooksMissingCoverImageAsync()
        {
            var books = await _context.Books
                .Where(b => b.CoverImageUrl == null)
                .Include(b => b.Category)
                .ToListAsync();

            return books.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                ISBN = b.ISBN,
                Publisher = b.Publisher,
                Price = b.Price,
                Author = b.Author,
                CategoryId = b.CategoryId,
                CategoryName = b.Category.Name,
                CoverImageUrl = b.CoverImageUrl,
                Pages = b.Pages,
                Language = b.Language,
                TotalQuantity = b.TotalQuantity,
            });
        }

        public async Task<BookDto> GetBookByIdAsync(Guid id)
        {
            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return null;

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                ISBN = book.ISBN,
                Publisher = book.Publisher,
                Price = book.Price,
                Author = book.Author,
                CategoryName = book.Category.Name,
                CoverImageUrl = book.CoverImageUrl,
                Pages = book.Pages,
                Language = book.Language,
                TotalQuantity = book.TotalQuantity
            };
        }

        public async Task<BookDto> CreateBookAsync(BookRequestModel bookRequest)
        {
            if (bookRequest.TotalQuantity <= 0)
            {
                throw new InvalidOperationException("The TotalQuantity must be greater than zero.");
            }

            string coverImageUrl = null;
            if (bookRequest.CoverImageFile != null && bookRequest.CoverImageFile.Length > 0)
            {
                coverImageUrl = await SaveFileAsync(bookRequest.CoverImageFile);
            }

            var book = new Book
            {
                Title = bookRequest.Title,
                Description = bookRequest.Description,
                ISBN = bookRequest.ISBN,
                Publisher = bookRequest.Publisher,
                Price = bookRequest.Price,
                Author = bookRequest.Author,
                CategoryId = bookRequest.CategoryId,
                CoverImageUrl = coverImageUrl,
                Pages = bookRequest.Pages,
                Language = bookRequest.Language,
                TotalQuantity = bookRequest.TotalQuantity
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                ISBN = book.ISBN,
                Author = book.Author,
                Publisher = book.Publisher,
                Price = book.Price,
                CoverImageUrl = book.CoverImageUrl,
                Pages = book.Pages,
                Language = book.Language,
                TotalQuantity = book.TotalQuantity
            };
        }

        public async Task<BookDto> UpdateBookAsync(Guid id, BookRequestModel bookRequest)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            book.Title = bookRequest.Title;
            book.Description = bookRequest.Description;
            book.ISBN = bookRequest.ISBN;
            book.Publisher = bookRequest.Publisher;
            book.Price = bookRequest.Price;
            book.Author = bookRequest.Author;
            book.CategoryId = bookRequest.CategoryId;
            book.Pages = bookRequest.Pages;
            book.Language = bookRequest.Language;

            if (bookRequest.CoverImageFile != null && bookRequest.CoverImageFile.Length > 0)
            {
                book.CoverImageUrl = await SaveFileAsync(bookRequest.CoverImageFile);
            }

            book.TotalQuantity = bookRequest.TotalQuantity;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                ISBN = book.ISBN,
                Publisher = book.Publisher,
                Author = book.Author,
                Price = book.Price,
                CoverImageUrl = book.CoverImageUrl,
                Pages = book.Pages,
                Language = book.Language,
                TotalQuantity = book.TotalQuantity
            };
        }

        public async Task DeleteBookAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var uploads = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }
            var filePath = Path.Combine(uploads, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/images/{file.FileName}";
        }

        public async Task<FileResult> DownloadExcelTemplateAsync()
        {
            var categories = await _categoryService.GetCategorySelectList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Book Template");

            // Set header row
            worksheet.Cells[1, 1].Value = "Title";
            worksheet.Cells[1, 2].Value = "Description";
            worksheet.Cells[1, 3].Value = "ISBN";
            worksheet.Cells[1, 4].Value = "Publisher";
            worksheet.Cells[1, 5].Value = "Price"; // Adjusted column numbers after removing Publication Date
            worksheet.Cells[1, 6].Value = "Author";
            worksheet.Cells[1, 7].Value = "Category Name";
            worksheet.Cells[1, 8].Value = "Pages"; // Adjusted column numbers
            worksheet.Cells[1, 9].Value = "Language";
            worksheet.Cells[1, 10].Value = "Total Quantity";

            var categoryRangeName = "CategoryList";

            // Fill Category list and create named range
            for (int i = 0; i < categories.Count(); i++)
            {
                worksheet.Cells[i + 2, 16].Value = categories.ElementAt(i).Text;
            }
            worksheet.Names.Add(categoryRangeName, worksheet.Cells[2, 16, categories.Count() + 1, 16]);

            // Set data validation for the Category dropdown (Start from row 2)
            var categoryValidation = worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 7, 100, 7].Address);
            categoryValidation.ShowErrorMessage = true;
            categoryValidation.ErrorTitle = "Invalid selection";
            categoryValidation.Error = "Please select a valid category from the list.";
            categoryValidation.Formula.ExcelFormula = $"={categoryRangeName}";

            // Hide the columns where categories are listed
            worksheet.Column(16).Hidden = true;

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "BookTemplate.xlsx"
            };
        }

        public async Task UploadBooksFromExcelAsync(Stream excelStream)
        {
            try
            {
                using var package = new ExcelPackage(excelStream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    throw new ArgumentException("The Excel file is empty.");
                }

                var bookList = new List<BookRequestModel>();
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    var title = worksheet.Cells[row, 1]?.Value?.ToString()?.Trim();
                    var description = worksheet.Cells[row, 2]?.Value?.ToString()?.Trim();
                    var isbn = worksheet.Cells[row, 3]?.Value?.ToString()?.Trim();
                    var publisher = worksheet.Cells[row, 4]?.Value?.ToString()?.Trim();
                    var price = decimal.TryParse(worksheet.Cells[row, 5]?.Value?.ToString(), out var priceValue) ? priceValue : 0;
                    var author = worksheet.Cells[row, 6]?.Value?.ToString()?.Trim();
                    var categoryName = worksheet.Cells[row, 7]?.Value?.ToString()?.Trim();
                    var pages = int.TryParse(worksheet.Cells[row, 8]?.Value?.ToString(), out var pageCount) ? pageCount : 0;
                    var language = worksheet.Cells[row, 9]?.Value?.ToString()?.Trim();
                    var totalQuantity = int.TryParse(worksheet.Cells[row, 10]?.Value?.ToString(), out var quantity) ? quantity : 0;

                    // Validate required fields
                    if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(isbn))
                    {
                        // Skip invalid rows and log a message
                        continue;
                    }

                    var category = await _categoryService.GetCategoryByNameAsync(categoryName);

                    if (category == null)
                    {
                        // Skip if category is not found and log a message
                        continue;
                    }

                    var bookRequest = new BookRequestModel
                    {
                        Title = title,
                        Description = description,
                        ISBN = isbn,
                        Publisher = publisher,
                        Price = price,
                        Author = author,
                        CategoryId = category.Id,
                        Pages = pages,
                        Language = language,
                        TotalQuantity = totalQuantity
                    };

                    bookList.Add(bookRequest);
                }

                // Create books
                foreach (var bookRequest in bookList)
                {
                    await CreateBookAsync(bookRequest);
                }
            }
            catch (ArgumentException ex)
            {
                throw new ApplicationException("There was an issue with the Excel file format. Please ensure it is correctly structured.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while processing the Excel file. Please try again later.", ex);
            }
        }

        public async Task<bool> AddCoverImageAsync(Guid bookId, IFormFile coverImageFile)
        {
            try
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                {
                    return false; 
                }

                if (coverImageFile != null && coverImageFile.Length > 0)
                {
                    var coverImageUrl = await SaveFileAsync(coverImageFile);
                    book.CoverImageUrl = coverImageUrl;

                    _context.Books.Update(book);
                    await _context.SaveChangesAsync();
                    return true; 
                }

                return false; 
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}