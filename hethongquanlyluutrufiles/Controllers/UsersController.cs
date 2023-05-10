using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using hethongquanlyluutrufiles.Models;
using Microsoft.AspNetCore.Http;
using AspNetCoreHero.ToastNotification.Abstractions;
using hethongquanlyluutrufiles.ModelViews;
using Microsoft.AspNetCore.Authorization;

namespace hethongquanlyluutrufiles.Controllers
{
    public class UsersController : Controller
    {
        private readonly hethongquanlyluutrufileContext _context;

        public INotyfService _notyfService { get; }

        public UsersController(hethongquanlyluutrufileContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details()
        {
            var userID = HttpContext.Session.GetString("UserId");
            var user = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == Convert.ToInt32(userID));

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["department"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            ViewData["typerole"] = new SelectList(_context.Roles, "RoleId", "TypeRole");
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                var userID = HttpContext.Session.GetString("UserId");
                var user = _context.Users.AsNoTracking().SingleOrDefault(x => x.UserId == Convert.ToInt32(userID));
                if (user == null)
                {
                    return NotFound();
                }

                user.Birthday = userModel.Birthday;
                user.Sex = userModel.Sex;
                user.Address = userModel.Address;
                user.Cmnd = userModel.Cmnd;
                user.RoleId = userModel.RoleId;
                user.DepartmentId = userModel.DepartmentId;
                user.Typework = userModel.Typework;
                user.Company = userModel.Company;
                user.Workplace = userModel.Workplace;

                _context.Update(user);
                await _context.SaveChangesAsync();
                _notyfService.Success("Tạo thông tin người dùng thành công");
                return RedirectToAction("TrangChuUser", "Home");
            }
            ViewData["department"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            ViewData["typerole"] = new SelectList(_context.Roles, "RoleId", "TypeRole");
            return View(userModel);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit()
        {
            var userID = HttpContext.Session.GetString("UserId");

            var user = await _context.Users.FindAsync(Convert.ToInt32(userID));
            if (user == null)
            {
                return NotFound();
            }
            ViewData["department"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            ViewData["typerole"] = new SelectList(_context.Roles, "RoleId", "TypeRole");
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int UserId, [Bind("UserId,Username,Email,Password,Salt,IsAdminOrUser,Birthday,Sex,Address,Phone,Cmnd,RoleId,DepartmentId,Typework,Company,Workplace,IsActive,AbleShared")] User user)
        {
            if (UserId != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    _notyfService.Success("Thay đổi thông tin người dùng thành công");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("TrangChuUser", "Home");
            }
            ViewData["department"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            ViewData["typerole"] = new SelectList(_context.Roles, "RoleId", "TypeRole");
            return View();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
