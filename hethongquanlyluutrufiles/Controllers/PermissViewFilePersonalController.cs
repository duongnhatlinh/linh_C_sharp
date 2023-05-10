using AspNetCoreHero.ToastNotification.Abstractions;
using hethongquanlyluutrufiles.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace hethongquanlyluutrufiles.Controllers
{
    public class PermissViewFilePersonalController : Controller
    {
        private readonly hethongquanlyluutrufileContext _context;
        public INotyfService _notyfService { get; }
        public PermissViewFilePersonalController(hethongquanlyluutrufileContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult SelectUser(int? page)
        {
            var userID = HttpContext.Session.GetString("UserId");
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var lsUsers = _context.Users
                .AsNoTracking()
                .Include(u => u.Department)
                .Include(u => u.Role)
                .Where(m => m.UserId != Convert.ToInt32(userID) && m.UserId != 24101)
                .OrderBy(x => x.UserId);
            PagedList<User> models = new PagedList<User>(lsUsers, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }
        public IActionResult SelectFile(int? page, int userid)
        {
            var userID = HttpContext.Session.GetString("UserId");

            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

            var lsFiles = _context.Files
                .AsNoTracking()
                .Where(x => x.UserId == Convert.ToInt32(userID) && x.IsActive == true)
                .Include(x => x.TypeFile)
                .OrderBy(x => x.FileId);
            PagedList<File> models = new PagedList<File>(lsFiles, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            TempData["userid"] = userid;
            return View(models);
        }
        public async Task<IActionResult> PermissOneOfUsers(int? fileid)
        {
            if (fileid == null)
            {
                return NotFound();
            }

            int userid = Convert.ToInt32(TempData["userid"]);
            TempData.Remove("userid");
            if (userid == null)
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

            // người nhận
            var userreceive = await _context.Users
                .Include(m => m.Role)
                .Include(m => m.Department)
                .FirstOrDefaultAsync(m => m.UserId == userid);

            ViewBag.userreceived = userreceive;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermissOneOfUsers([Bind("SharedFileId,FileId,SharedWithUserId,DateShared,Message")] SharedFile sf, int fileid, int userid)
        {
            if (ModelState.IsValid)
            {
                sf.FileId = fileid;
                sf.SharedWithUserId = userid;
                sf.DateShared = DateTime.Now;
                sf.Notify = true;
                _context.Add(sf);
                await _context.SaveChangesAsync();
                _notyfService.Success("Cấp quyền người dùng thành công");
                return RedirectToAction("TrangChuAdmin", "Home", new { area = "Admin" });
            }
            return View(sf);
        }
    }
}
