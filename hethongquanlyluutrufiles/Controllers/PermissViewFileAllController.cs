using AspNetCoreHero.ToastNotification.Abstractions;
using hethongquanlyluutrufiles.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace hethongquanlyluutrufiles.Controllers
{
    public class PermissViewFileAllController : Controller
    {
        const int idadmin = 24101; 
        private readonly hethongquanlyluutrufileContext _context;
        public INotyfService _notyfService { get; }

        public PermissViewFileAllController(hethongquanlyluutrufileContext context, INotyfService notyfService)
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
                .OrderBy(x => x.FileId);
            PagedList<File> models = new PagedList<File>(lsFiles, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }
        public async Task<IActionResult> PermissTotalUsers(int? fileid)
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
        public async Task<IActionResult> PermissTotalUsers(int fileid, SharedFile s)
        {
            if (ModelState.IsValid)
            {
                var userID = HttpContext.Session.GetString("UserId");


                List<User> lsuser = new List<User>();
                lsuser = _context.Users.Where(x => x.UserId != Convert.ToInt32(userID) && x.UserId != idadmin).ToList();

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
                _notyfService.Success("Cấp quyền xem cho toàn bộ thành công");
                return RedirectToAction("TrangChuUser", "Home", new { area = "" });
            }
            return View(fileid);
        }
    }
}
