using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories.Select(c => new CategoryDto 
            { 
                Id = c.Id, 
                Name = c.Name 
            });
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            return new CategoryDto 
            { 
                Id = category.Id, 
                Name = category.Name 
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryRequestModel categoryRequest)
        {
            var category = new Category { Name = categoryRequest.Name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryDto 
            { 
                Id = category.Id, 
                Name = category.Name 
            };
        }

        public async Task<CategoryDto> UpdateCategoryAsync(Guid id, CategoryRequestModel categoryRequest)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            category.Name = categoryRequest.Name;
            await _context.SaveChangesAsync();

            return new CategoryDto 
            { 
                Id = category.Id, 
                Name = category.Name 
            };
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<SelectListItem>> GetCategorySelectList()
        {
            var categories = await GetAllCategoriesAsync();
            var categoryList = categories.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            });

            return new SelectList(categoryList, "Value", "Text");
        }
    }

}
