using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShop.Areas.Admin.Models;
using OnlineShop.Data;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _db;
        private UserManager<IdentityUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles;
            return View();
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        //Post Create Action Method
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            IdentityRole role =new IdentityRole();
            role.Name = name;
            var isExist = await _roleManager.RoleExistsAsync(role.Name);
            if (isExist)
            {
                ViewBag.message = "This role is already exist.";
                ViewBag.name = name;
                return View();
            }
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                TempData["Save"] = "Role has been save successfully ";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }


        public async Task<IActionResult> Edit(string id)
        {
            var role =await _roleManager.FindByIdAsync(id);
            if (role==null)
            {
                return NotFound();
            }

            ViewBag.id = role.Id;
            ViewBag.name = role.Name;
            return View();
        }
        //Post Edit Action Method
        [HttpPost]
        public async Task<IActionResult> Edit( string id,string name)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            role.Name = name;
            var isExist = await _roleManager.RoleExistsAsync(role.Name);
            if (isExist)
            {
                ViewBag.message = "This role is already exist.";
                ViewBag.name = name;
                return View();
            }
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                TempData["Save"] = "Role has been update successfully ";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            ViewBag.id = role.Id;
            ViewBag.name = role.Name;
            return View();
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var result =await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {

                TempData["delete"] = "Role has been delete successfully ";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
       
        public async Task<IActionResult> Assign()
        {
            ViewData["userId"]=new SelectList(_db.ApplicationUsers.Where(c=>c.LockoutEnd<DateTime.Now || c.LockoutEnd==null).ToList(),"Id","UserName");
            ViewData["roleId"]=new SelectList(_roleManager.Roles.ToList(),"Name","Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Assign(RoleUserVm roleUser)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == roleUser.UserId);
            var isCheckRoleAssign =await _userManager.IsInRoleAsync(user, roleUser.RoleId);
            if (isCheckRoleAssign)
            {
                ViewBag.Message = "This user already assign this role";
                ViewData["userId"] = new SelectList(_db.ApplicationUsers.Where(c => c.LockoutEnd < DateTime.Now || c.LockoutEnd == null).ToList(), "Id", "UserName");
                ViewData["roleId"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
                return View();
            }
            var role = await _userManager.AddToRoleAsync(user, roleUser.RoleId);
            if (role.Succeeded)
            {
                TempData["save"] = "User role assigned";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public ActionResult AssignUserRole()
        {
            var result = from ur in _db.UserRoles
                join r in _db.Roles on ur.RoleId equals r.Id
                join a in _db.ApplicationUsers on ur.UserId equals a.Id
                select new UserRoleMapping()
                {
                    UserId = ur.UserId,
                    RoleId = ur.RoleId,
                    UserName = a.UserName,
                    RoleName = r.Name
                };
            ViewBag.UserRoles = result;
            return View();
        }
    }
}
