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

            if (!ViewBag.ShowLoginForm)
                return RedirectToAction("ShowProducts");

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
                return RedirectToAction("Index");
            }

            return View("Index");
        }

        public IActionResult AddProduct()
        {
            InitView();

            return View();
        }

        public IActionResult InsertProduct(ProductModel product)
        {
            InitView();

            var productDal = new ProductDAL(_configuration);
            var pId = productDal.InsertProduct(product);
            return View("Product", product);
        }

        public IActionResult Product(ProductModel product)
        {
            InitView();

            return View(product);
        }

        public IActionResult EditProduct(string PID)
        {
            InitView();

            var productDal = new ProductDAL(_configuration);
            var product = productDal.GetProduct(Convert.ToInt32(PID));

            return View(product);
        }

        public IActionResult UpdateProduct(ProductModel product)
        {
            InitView();

            var productDal = new ProductDAL(_configuration);
            productDal.UpdateProduct(product);

            return View("Product", product);
        }

        public IActionResult DeleteProduct(string PID)
        {
            InitView();
            ProductModel product = null;

            try
            {
                var productDal = new ProductDAL(_configuration);
                product = productDal.GetProduct(Convert.ToInt32(PID));
                productDal.DeleteProduct(PID);
                ViewBag.ErrorMessage = string.Empty;
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Unable to delete product from database";
            }


            return View("DeleteProduct", product);
        }

        public IActionResult ProductList()
        {
            InitView();

            if (ViewBag.ShowLoginForm)
            {
                ViewBag.LoginMessage = "User not logged in.";
                return View("Index");
            }

            var productDal = new ProductDAL(_configuration);
            var productList = productDal.GetAllProducts();
            return View(productList);
        }

        public IActionResult ShowProducts()
        {
            InitView();

            var productDal = new ProductDAL(_configuration);
            var productList = productDal.GetAllProducts();
            return View(productList);
        }

        public IActionResult BuyProduct(string PID)
        {
            InitView();

            if (ViewBag.ShowLoginForm)
            {
                ViewBag.LoginMessage = "User not logged in.";
                return View("Index");
            }

            var productDal = new ProductDAL(_configuration);
            var product = productDal.GetProduct(Convert.ToInt32(PID));
            productDal.UpdateInventory(product, 1);

            var personDal = new PersonDAL(_configuration);
            var person = personDal.GetPerson(HttpContext.Session.GetString(personIdKey));

            var transactionDal = new TransactionDAL(_configuration);
            var txn = new SaleTransactionModel()
            {
                PersonId = person.PersonId,
                ProductId = product.ProductId,
                TransactionTime = DateTime.Now,
                Quantity = 1
            };

            var tranId = transactionDal.InsertTransaction(txn);

            var purchase = new PurchaseModel()
            {
                Person = person,
                Product = product,
                SalesId = tranId
            };

            return View(purchase);
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