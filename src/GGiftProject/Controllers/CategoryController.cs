using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GGiftProject.Services;
using GGiftProject.Models;
using GGiftProject.ViewModels;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GGiftProject.Controllers
{
    public class CategoryController : Controller
    {
        IDataService<Category> _categoryDataService;
        public CategoryController(IDataService<Category> categoryDataService)
        {
            _categoryDataService = categoryDataService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddCategory()
        {
            HamperCategoryListViewModel vm = new ViewModels.HamperCategoryListViewModel();
   
            IEnumerable<Category> categoryList = _categoryDataService.GetAll();
            vm.Categories = categoryList.Select(c => new HamperCategoryViewModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }).ToList();

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddCategory(HamperCategoryListViewModel vm, string save, string id)
        {

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(save))
                {
                    Category category;
                    if (vm.CategoryId == 0)
                    {
                        category = new Models.Category { CategoryName = vm.CategoryName };
                        _categoryDataService.Create(category);
                    }
                    else
                    {
                        category = _categoryDataService.GetSingle(c => c.CategoryId == vm.CategoryId);
                        category.CategoryName = vm.CategoryName;
                        _categoryDataService.Update(category);
                    }
                }
                else
                {
                    int categoryId = 0;
                    int.TryParse(id, out categoryId);
                    if (categoryId != 0)
                    {
                        Category category = _categoryDataService.GetSingle(c => c.CategoryId == categoryId);
                        if (category != null)
                        {
                            ModelState.Remove("CategoryId");
                            vm.CategoryId = category.CategoryId;
                            ModelState.Remove("CategoryName");
                            vm.CategoryName = category.CategoryName;
                        }
                    }
                }
            }

            IEnumerable<Category> categoryList = _categoryDataService.GetAll();
            vm.Categories = categoryList.Select(c => new HamperCategoryViewModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }).ToList();

            return View(vm);
        }
    }
}
