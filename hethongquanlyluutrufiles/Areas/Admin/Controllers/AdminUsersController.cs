using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using hethongquanlyluutrufiles.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using PagedList.Core;
using Microsoft.AspNetCore.Http;

namespace hethongquanlyluutrufiles.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly hethongquanlyluutrufileContext _context;

        public INotyfService _notyfService { get; }

        public AdminUsersController(hethongquanlyluutrufileContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminUsers
        public IActionResult Index(int page = 1, int DepID = 0)
        {
            var pageNumber = page;
            var pageSize = 10;
            var userID = HttpContext.Session.GetString("UserId");


            List<User> listuser = new List<User>();

            if (DepID != 0)
            {
                listuser = _context.Users
                    .AsNoTracking()
                    .Include(x => x.Department)
                    .Where(x => x.DepartmentId == DepID && x.UserId != Convert.ToInt32(userID))
                    .OrderByDescending(x => x.UserId).ToList();
            }
            else
            {
                listuser = _context.Users
                    .Include(x => x.Department)
                    .Where(x => x.UserId != Convert.ToInt32(userID))
                    .AsNoTracking()
                    .OrderByDescending(x => x.UserId).ToList();
            }


            PagedList<User> models = new PagedList<User>(listuser.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentDepartment = DepID;
            ViewBag.CurrentPage = pageNumber;
            ViewData["Department"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");

            return View(models);
        }

        public IActionResult Filtter(int DepID = 0)
        {
            var url = $"/Admin/AdminUsers?DepID={DepID}";
            if (DepID == 0)
            {
                url = $"/Admin/AdminUsers";
            }
            return Json(new { status = "success", redirectUrl = url });
        }

        [HttpPost]
        public IActionResult UpdateSwitch(int id, bool value)
        {
            var user = _context.Users.Find(id);
            user.AbleShared = value;
          
            _context.Update(user);
            _context.SaveChanges();

            // xử lý sự kiện tại đây
            return Json(new { success = true }); // trả về JSON nếu muốn
        }
        // GET: Admin/AdminUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

    }
}
