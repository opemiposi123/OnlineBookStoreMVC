using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(Guid id);
        Task<CategoryDto> CreateCategoryAsync(CategoryRequestModel categoryRequest);
        Task<CategoryDto> UpdateCategoryAsync(Guid id, CategoryRequestModel categoryRequest);
        Task<bool> DeleteCategoryAsync(Guid id);
        Task<IEnumerable<SelectListItem>> GetCategorySelectList();
    }
}
