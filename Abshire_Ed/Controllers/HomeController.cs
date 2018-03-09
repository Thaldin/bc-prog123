using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Abshire_Ed.Models;
using Abshire_Ed.DAL;


namespace Abshire_Ed.Controllers
{
    public class HomeController : Controller
    {
        const string personIdKey = "personId";
        const string firstNameKey = "fName";

        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration config)
        {
            _configuration = config;    
        }

        public IActionResult Index()
        {
            InitView();
            return View();
        }

        public IActionResult Page2(PersonModel person)
        {
            var personDal = new PersonDAL(_configuration);
            int personId = personDal.InsertPerson(person);
            HttpContext.Session.SetString(personIdKey, personId.ToString());
            HttpContext.Session.SetString(firstNameKey, person.FirstName);
            InitView();

            return View(person);
        }

        public IActionResult EditPerson()
        {
            InitView();

            var id = HttpContext.Session.GetString(personIdKey);
            var personDal = new PersonDAL(_configuration);
            var person = personDal.GetPerson(id);
            return View(person);
        }

        public IActionResult UpdatePerson(PersonModel person)
        {
            InitView();

            var personDal = new PersonDAL(_configuration);
            var id = HttpContext.Session.GetString(personIdKey);

            person.PersonId = Convert.ToInt32(id);
            personDal.UpdatePerson(person);

            HttpContext.Session.SetString(firstNameKey, person.FirstName);

            return View("Page2", person);
        }

        public IActionResult DeletePerson()
        {
            InitView();

            var id = HttpContext.Session.GetString(personIdKey);
            PersonModel person = null;
            try
            {
                var personDal = new PersonDAL(_configuration);
                person = personDal.GetPerson(id);
                personDal.DeletePerson(id);
                ViewBag.ErrorMessage = string.Empty;
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Error Deleting Person from Database";
            }

            return View(person);
        }

        private void InitView()
        {
            ViewBag.ShowLoginForm = string.IsNullOrEmpty(HttpContext.Session.GetString(personIdKey));

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(firstNameKey)))
            {
                ViewBag.UserFirstName = HttpContext.Session.GetString(firstNameKey);
            }
        }

    }
}
