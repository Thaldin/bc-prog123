using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Abshire_Ed.Controllers
{
    public class AbshireController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}