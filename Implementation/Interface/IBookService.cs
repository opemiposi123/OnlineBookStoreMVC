using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto> GetBookByIdAsync(Guid id);
        Task<BookDto> CreateBookAsync(BookRequestModel bookRequest);
        Task<BookDto> UpdateBookAsync(Guid id, BookRequestModel bookRequest);
        Task DeleteBookAsync(Guid id);
        Task<IEnumerable<BookDto>> GetBooksMissingCoverImageAsync();
        Task<string> SaveFileAsync(IFormFile file);
        List<Book> GetAllBooks();
        Task<FileResult> DownloadExcelTemplateAsync();
        Task UploadBooksFromExcelAsync(Stream excelStream);
        Task<bool> AddCoverImageAsync(Guid bookId, IFormFile coverImageFile);
    }
}
