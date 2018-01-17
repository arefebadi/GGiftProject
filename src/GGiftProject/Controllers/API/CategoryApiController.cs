using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GGiftProject.Models;
using GGiftProject.Services;
using GGiftProject.ViewModels;
using System.Net;


namespace GGiftProject.Controllers.API
{



    public class CategoryApiController : Controller
    {
        private IDataService<Category> _categoryDataService;
        private IDataService<Hamper> _hamperDataService;
        public CategoryApiController(IDataService<Category> service, IDataService<Hamper> HamperService)
        {
            _categoryDataService = service;
            _hamperDataService = HamperService;
        }

        [HttpGet("api/categories")]
        public JsonResult GetCategories()
        {
            try
            {
                IEnumerable<Category> categories = _categoryDataService.GetAll();
                return Json(categories);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = ex.Message });
            }
        }

        [HttpGet("api/allhampers")]
        public JsonResult GetHampers()
        {
            try
            {
                IEnumerable<Hamper> products = _hamperDataService.Query(p => !p.Discontinue);
                return Json(products);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = ex.Message });
            }
        }

        //web method - to get Hampers by category name
        [HttpGet("api/hampers/cat")]
        public JsonResult GetHampersByCategory(string catName)
        {
            try
            {
                Category cat = _categoryDataService.GetSingle(c => c.CategoryName == catName);
                if (cat != null)
                {
                    IEnumerable<Hamper> products = _hamperDataService.Query(p => p.CategoryId == cat.CategoryId && !p.Discontinue);
                    return Json(products);
                }
                else
                {
                    return Json(new { message = "cannot find this category" });
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = ex.Message });
            }
        }
    }
}
