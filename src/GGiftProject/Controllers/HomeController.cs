using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GGiftProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Contact Us";

            return View();
        }

        public IActionResult PrivacyPolicy()
        {
            ViewData["Message"] = "Privacy Policy";

            return View();
        }

        public IActionResult TermAndConditions()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
