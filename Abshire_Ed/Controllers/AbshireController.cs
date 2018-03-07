using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Abshire_Ed.Models;
using Abshire_Ed.DAL;
using Microsoft.AspNetCore.Http;
using System;

namespace Abshire_Ed.Controllers
{
    public class AbshireController : Controller
    {
        const string personIdKey = "personId";
        const string productIdKey = "productId";
        const string firstNameKey = "fName";

        private readonly IConfiguration _configuration;

        public AbshireController(IConfiguration config)
        {
            _configuration = config;
        }

        public IActionResult Index()
        {
            InitView();

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

        public IActionResult AddProduct()
        {
            InitView();

            if (ViewBag.ShowLoginForm)
            {
                ViewBag.LoginMessage = "User not logged in.";
                return View("Index");
            }

            return View();
        }

        public IActionResult InsertProduct(ProductModel product)
        {
            InitView();

            var productDal = new ProductDAL(_configuration);
            var pId = productDal.InsertProduct(product);
            HttpContext.Session.SetString(productIdKey, pId.ToString());
            return View("Product", product);
        }

        public IActionResult Product(ProductModel product)
        {
            InitView();      

            return View(product);
        }

        public IActionResult EditProduct()
        {
            InitView();

            var id = HttpContext.Session.GetString(productIdKey);
            var productDal = new ProductDAL(_configuration);
            var product = productDal.GetProduct(Convert.ToInt32(id));

            return View(product);
        }

        public IActionResult UpdateProduct(ProductModel product)
        {
            InitView();

            var productDal = new ProductDAL(_configuration);
            var id = HttpContext.Session.GetString(productIdKey);

            product.ProductId = Convert.ToInt32(id);
            productDal.UpdateProduct(product);

            return View("Product", product);
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