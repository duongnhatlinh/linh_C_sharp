using AspNetCoreHero.ToastNotification.Abstractions;
using hethongquanlyluutrufiles.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PagedList.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hethongquanlyluutrufiles.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminPermissViewFileGroupController : Controller
    {
        private readonly hethongquanlyluutrufileContext _context;
        public INotyfService _notyfService { get; }

        public AdminPermissViewFileGroupController(hethongquanlyluutrufileContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult SelectGroupUsers()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectGroupUsers(int check)
        {
            var userID = HttpContext.Session.GetString("UserId");

            List<User> lsuser = new List<User>();
            switch (check)
            {
                case 1: // phòng nhân sự
                    lsuser = _context.Users.Where(x => x.DepartmentId == 1 && x.UserId != Convert.ToInt32(userID)).ToList();
                    TempData["department"] = "Phòng Nhân Sự";
                    break;
                case 2: // phòng kế toán
                    lsuser = _context.Users.Where(x => x.DepartmentId == 2 && x.UserId != Convert.ToInt32(userID)).ToList();
                    TempData["department"] = "Phòng Kế Toán";
                    break;
                case 3: // phòng kinh doanh
                    lsuser = _context.Users.Where(x => x.DepartmentId == 3 && x.UserId != Convert.ToInt32(userID)).ToList();
                    TempData["department"] = "Phòng Kinh Doanh";
                    break;
                case 4: // phòng kỹ thuật
                    lsuser = _context.Users.Where(x => x.DepartmentId == 4 && x.UserId != Convert.ToInt32(userID)).ToList();
                    TempData["department"] = "Phòng Kỹ Thuật";
                    break;
                case 5: // phòng sản xuất
                    lsuser = _context.Users.Where(x => x.DepartmentId == 5 && x.UserId != Convert.ToInt32(userID)).ToList();
                    TempData["department"] = "Phòng Sản Xuất";
                    break;
            }
            TempData["listuser"] = JsonConvert.SerializeObject(lsuser);

            return RedirectToAction("SelectFile");
        }
        public IActionResult SelectFile(int? page)
        {
            var userID = HttpContext.Session.GetString("UserId");

            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var lsFiles = _context.Files
                .AsNoTracking()
                .Include(x => x.TypeFile)
                .Where(x => x.UserId == Convert.ToInt32(userID) && x.IsActive == true)
                .OrderBy(x => x.DateUploaded);
            PagedList<File> models = new PagedList<File>(lsFiles, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        public async Task<IActionResult> PermissViewFile(int? fileid)
        {

            if (fileid == null)
            {
                return NotFound();
            }

            var file = await _context.Files
                .Include(f => f.TypeFile)
                .FirstOrDefaultAsync(m => m.FileId == fileid);

            if (file == null)
            {
                return NotFound();
            }

            ViewBag.file = file;
            // người gửi
            var userID = HttpContext.Session.GetString("UserId");
            var usersend = await _context.Users
                .Include(m => m.Role)
                .Include(m => m.Department)
                .FirstOrDefaultAsync(m => m.UserId == Convert.ToInt32(userID));

            if (usersend == null)
            {
                return NotFound();
            }
            ViewBag.usersend = usersend;

            ViewBag.department = TempData["department"];
            TempData.Remove("department");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermissViewFile(int fileid, SharedFile s)
        {
            if (ModelState.IsValid)
            {
                var lsuser = JsonConvert.DeserializeObject<List<User>>(TempData["listuser"].ToString());

                foreach (var lu in lsuser)
                {
                    SharedFile sf = new SharedFile();
                    sf.FileId = fileid;
                    sf.SharedWithUserId = lu.UserId;
                    sf.Message = s.Message;
                    sf.DateShared = DateTime.Now;
                    sf.Notify = true;
                    _context.SharedFiles.Add(sf);
                }
                await _context.SaveChangesAsync();
                TempData.Remove("listuser");

                _notyfService.Success("Cấp quyền nhóm người dùng thành công");
                return RedirectToAction("TrangChuAdmin", "Home", new { area = "Admin" });
            }
            return View(fileid);
        }
    }
}
