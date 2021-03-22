using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private ApplicationDbContext _db;
        private IHostingEnvironment _he;

        public ProductController(ApplicationDbContext db,IHostingEnvironment he)
        {
            _db = db;
            _he = he;
        }
        public IActionResult Index()
        {
            return View(_db.Productses.Include(m=>m.ProductTypes).Include(m=>m.SpecialTag).ToList());
        }

        //Post Index Action Method
        [HttpPost]
        public IActionResult Index(decimal? minAmount, decimal? maxAmount)
        {
            var products = _db.Productses.Include(c => c.ProductTypes).Include(c => c.SpecialTag)
                .Where(c => c.Price >= minAmount && c.Price <= maxAmount).ToList();
            if (minAmount==null || maxAmount==null)
            {
                products = _db.Productses.Include(c => c.ProductTypes).Include(c => c.SpecialTag).ToList();
            }
            return View(products);
        }

        [Authorize]
        //Get Create Method
        public IActionResult Create()
        {
            ViewData["productTypeId"]= new SelectList(_db.Products.ToList(),"Id", "ProductType");
            ViewData["tagId"]= new SelectList(_db.SpecialTags.ToList(),"Id", "TagName");
            return View();
        }

        //Post Create Method 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Products products,IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var searchProduct = _db.Productses.FirstOrDefault(c => c.Name == products.Name); 
                if (searchProduct != null)
                {
                    ViewBag.message = "This product is already exist.";
                    ViewData["productTypeId"] = new SelectList(_db.Products.ToList(), "Id", "ProductType");
                    ViewData["tagId"] = new SelectList(_db.SpecialTags.ToList(), "Id", "TagName");

                    return View(products);
                }
                if (imageFile !=null)
                {
                    var name = Path.Combine(_he.WebRootPath + "/Images", Path.GetFileName(imageFile.FileName));
                    await imageFile.CopyToAsync(new FileStream(name, FileMode.Create));
                    products.Image = "Images/"+imageFile.FileName;
                }

                if (imageFile==null)
                {
                    products.Image = "Images/images.png";
                }
                _db.Productses.Add(products);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }

            return View(products);
        }

        //Get Edit Action Method
        public ActionResult Edit(int? id)
        {
            ViewData["productTypeId"] = new SelectList(_db.Products.ToList(), "Id", "ProductType");
            ViewData["tagId"] = new SelectList(_db.SpecialTags.ToList(), "Id", "TagName");
            if (id==null)
            {
                return NotFound();
            }

            var product = _db.Productses.Include(c => c.ProductTypes).Include(c => c.SpecialTag).FirstOrDefault(c=>c.Id==id);
            if (product==null)
            {
                return NotFound();
            }
            return View(product);
        }
        //Edit post action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Products products)
        {

            if (ModelState.IsValid)
            {
                _db.Update(products);
                await _db.SaveChangesAsync();
                TempData["update"] = "Product type has been updated.";
                return RedirectToAction(nameof(Index));
            }

            return View(products);
        }

        //Get Detail Action Methods
        public ActionResult Details(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var product = _db.Productses.Include(c => c.ProductTypes).Include(c => c.SpecialTag)
                .FirstOrDefault(c => c.Id == id);
            if (product==null)
            {
                return NotFound();
            }
            return View(product);
        }
        //Get Delete Action Method
        public ActionResult Delete(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var product = _db.Productses.Include(c => c.ProductTypes).Include(c => c.SpecialTag).FirstOrDefault(c => c.Id == id);
            if (product==null)
            {
                return NotFound();
            }
            return View(product);
        }
        //Post Delete Action Method
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var product = _db.Productses.FirstOrDefault(c => c.Id == id);
            if (product==null)
            {
                return NotFound();
            }

            _db.Productses.Remove(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
