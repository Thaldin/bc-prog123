using Microsoft.AspNetCore.Mvc;
using Abshire_Ed.Models;

namespace Abshire_Ed.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Page2(PersonModel person)
        {
            return View(person);
        }
    }
}
