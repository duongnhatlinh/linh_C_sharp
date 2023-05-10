using AspNetCoreHero.ToastNotification.Abstractions;
using hethongquanlyluutrufiles.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hethongquanlyluutrufiles.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminPermissViewFileAllController : Controller
    {
        private readonly hethongquanlyluutrufileContext _context;
        public INotyfService _notyfService { get; }

        public AdminPermissViewFileAllController(hethongquanlyluutrufileContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
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

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermissViewFile(int fileid, SharedFile s)
        {
            if (ModelState.IsValid)
            {
                var userID = HttpContext.Session.GetString("UserId");
               

                List<User> lsuser = new List<User>();
                lsuser = _context.Users.Where(x => x.UserId != Convert.ToInt32(userID)).ToList();

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
                _notyfService.Success("Chia sẻ file thành công");
                return RedirectToAction("TrangChuAdmin", "Home", new { area = "Admin" });
            }
            return View(fileid);
        }
    }
}
