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

        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration config)
        {
            _configuration = config;    
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Page2(PersonModel person)
        {
            var personDal = new PersonDAL(_configuration);
            int personId = personDal.InsertPerson(person);
            HttpContext.Session.SetString(personIdKey, personId.ToString());
            return View(person);
        }

        public IActionResult EditPerson()
        {
            var id = HttpContext.Session.GetString(personIdKey);
            var personDal = new PersonDAL(_configuration);
            var person = personDal.GetPerson(id);
            return View(person);
        }

        public IActionResult UpdatePerson(PersonModel person)
        {
            var personDal = new PersonDAL(_configuration);
            var id = HttpContext.Session.GetString(personIdKey);

            person.PersonId = Convert.ToInt32(id);
            personDal.UpdatePerson(person);

            return View("Page2", person);
        }
    }
}
