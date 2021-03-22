using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.Utility;
using X.PagedList;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index(int? page)
        {
            return View(_db.Productses.Include(c => c.ProductTypes).Include(c => c.SpecialTag).ToList().ToPagedList(page??1,9));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Get Details Action Method
        public IActionResult Details(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var product = _db.Productses.Include(c => c.ProductTypes).FirstOrDefault(c => c.Id == id);
            if (product==null)
            {
                NotFound();
            }
            return View(product);
        }

        //Post Details Action Method
        [HttpPost]
        [ActionName("Details")]
        public IActionResult ProductDetails(int? id)
        {
            List<Products> productList=new List<Products>();
            if (id == null)
            {
                return NotFound();
            }

            var product = _db.Productses.Include(c => c.ProductTypes).FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                NotFound();
            }

            productList=HttpContext.Session.Get<List<Products>>("productList");
            if (productList==null)
            {
                productList=new List<Products>();
            }
            productList.Add(product);
            HttpContext.Session.Set("productList", productList);
            return View(product);
        }
        //Get Remove Action Method
        [ActionName("Remove")]
        public IActionResult RemoveToCart(int? id)
        {
            List<Products> productList = HttpContext.Session.Get<List<Products>>("productList");
            if (productList != null)
            {
                var product = productList.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    productList.Remove(product);
                    HttpContext.Session.Set("productList", productList);
                }
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult Remove(int? id)
        {
            List<Products> productList = HttpContext.Session.Get<List<Products>>("productList");
            if (productList!=null)
            {
                var product = productList.FirstOrDefault(c => c.Id == id);
                if (product!=null)
                {
                    productList.Remove(product);
                    HttpContext.Session.Set("productList", productList);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        //Get product Cart Action Method
        public IActionResult Cart()
        {
            List<Products> productList = HttpContext.Session.Get<List<Products>>("productList");
            if (productList==null)
            {
                productList= new List<Products>();
            }

            return View(productList);
        }
    }
}
