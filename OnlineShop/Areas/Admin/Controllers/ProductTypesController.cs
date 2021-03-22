using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductTypesController : Controller
    {
        private ApplicationDbContext _db;

        public ProductTypesController(ApplicationDbContext db)
        {
            _db = db;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_db.Products.ToList());
        }


        //Create get action method
        public IActionResult Create()
        {
            return View();
        }

        //Create post action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                _db.Products.Add(productTypes);
                await _db.SaveChangesAsync();
                TempData["save"] = "Product type has been saved.";
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }

        //Edit get action method
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = _db.Products.Find(id);
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
        }

        //Edit post action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductTypes productTypes)
        {

            if (ModelState.IsValid)
            {
                _db.Products.Update(productTypes);
                await _db.SaveChangesAsync();
                TempData["update"] = "Product type has been updated.";
                return RedirectToAction(nameof(Index));
            }

            return View(productTypes);
        }
        //Details get action method
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = _db.Products.Find(id);
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
        }

        //Details post action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Details(ProductTypes productTypes)
        {
            return RedirectToAction(nameof(Index));
        }
        //Delete get action method
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = _db.Products.Find(id);
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
        }

        //Delete post action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id,ProductTypes productTypes)
        {
            if (id==null)
            {
                return NotFound();
            }
            if (id!=productTypes.Id)
            {
                NotFound();
            }

            var productType = _db.Products.Find(id);
            if (productType == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _db.Products.Remove(productType);
                await _db.SaveChangesAsync();
                TempData["delete"] = "Product type has been deleted.";
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }
    }
}
