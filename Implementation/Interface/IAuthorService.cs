using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync();
        Task<AuthorDto> GetAuthorByIdAsync(Guid id);
        Task<AuthorDto> CreateAuthorAsync(AuthorRequestModel authorRequest);
        Task<AuthorDto> UpdateAuthorAsync(Guid id, AuthorRequestModel authorRequest);
        Task<bool> DeleteAuthorAsync(Guid id);
        Task<IEnumerable<SelectListItem>> GetAuthorSelectList();
    }
}
