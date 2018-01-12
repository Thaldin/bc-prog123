using Microsoft.AspNetCore.Mvc;

namespace Abshire_Ed.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Page2()
        {
            return View();
        }
    }
}
