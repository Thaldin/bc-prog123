using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Abshire_Ed.Models;
using Abshire_Ed.DAL;
using Microsoft.AspNetCore.Http;

namespace Abshire_Ed.Controllers
{
    public class AbshireController : Controller
    {
        const string personIdKey = "personId";
        const string firstNameKey = "fName";

        private readonly IConfiguration _configuration;

        public AbshireController(IConfiguration config)
        {
            _configuration = config;
        }

        public IActionResult Index()
        {
            ViewBag.ShowLoginForm = string.IsNullOrEmpty(HttpContext.Session.GetString(personIdKey));

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(firstNameKey)))
            {
                ViewBag.UserFirstName = HttpContext.Session.GetString(firstNameKey);
            }

            return View();
        }

        public IActionResult Login(LoginCredentials creds)
        {
            var personDal = new PersonDAL(_configuration);
            var compact = personDal.CheckUser(creds.UserName, creds.Password);

            if (compact == null)
            {
                ViewBag.LoginMessage = "Invalid Credentials, please try again.";
                ViewBag.ShowLoginForm = true;
            }
            else
            {
                HttpContext.Session.SetString(personIdKey, compact.PersonId.ToString());
                HttpContext.Session.SetString(firstNameKey, compact.FirstName);
                ViewBag.UserFirstName = compact.FirstName;
                ViewBag.ShowLoginForm = false;
                ViewBag.LoginMessage = string.Empty;
            }

            return View("Index");
        }
    }
}