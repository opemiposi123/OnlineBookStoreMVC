using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly INotyfService _notyf;

        public CategoryController(ICategoryService categoryService,INotyfService notyf)
        {
            _categoryService = categoryService;
            _notyf = notyf;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        public async Task<IActionResult> Create(CategoryRequestModel categoryRequest)
        {
            if (ModelState.IsValid)
            {
                var category = await _categoryService.CreateCategoryAsync(categoryRequest);
                _notyf.Success("Category Created Succesfully");
                return RedirectToAction(nameof(Index));
            }
            return View(categoryRequest);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var categoryRequest = new CategoryRequestModel
            {
                Name = category.Name
            };
            return View(categoryRequest);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id,CategoryRequestModel categoryRequest)
        {
            if (ModelState.IsValid)
            {
                var updatedCategory = await _categoryService.UpdateCategoryAsync(id, categoryRequest);
                _notyf.Success("Category Updated Succesfully");
                if (updatedCategory == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(categoryRequest);
        }

        [HttpGet("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            _notyf.Success("Category Deleted Succesfully");
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
