using OnlineBookStoreMVC.DTOs;
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
        Task<string> SaveFileAsync(IFormFile file);
    }
}
