using AspNetCoreHero.ToastNotification.Abstractions;
using hethongquanlyluutrufiles.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace hethongquanlyluutrufiles.Controllers
{
    public class HomeController : Controller
    {
        private readonly hethongquanlyluutrufileContext _context;
        public INotyfService _notyfService { get; }
        public HomeController(hethongquanlyluutrufileContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult TrangChuUser()
        {
            var userID = HttpContext.Session.GetString("UserId");
            List<SharedFile> sharefile = new List<SharedFile>();

            int countNotify = _context.SharedFiles.Count(x => x.SharedWithUserId == Convert.ToInt32(userID) && x.Notify == true && x.File.IsActive == true);
            return View(countNotify);
        }

        // cấp quyền chia sẻ file
        public IActionResult AuthorShareFile(int index)
        {
            var userID = HttpContext.Session.GetString("UserId");
            var user = _context.Users.Find(Convert.ToInt32(userID));
            if (user.AbleShared == false)
            {
                return View();
            }

            if (index == 1)
            {
                return RedirectToAction("SelectFile", "PermissViewFileAll", new { area = "" });
                // chia sẻ toàn bộ người dùng
            } else if(index == 2)
            {
                return RedirectToAction("SelectGroupUsers", "PermissViewFileGroup", new { area = "" });
                // chia sẻ nhóm người dùng
            }
            else
            {
                return RedirectToAction("SelectUser", "PermissViewFilePersonal", new { area = "" });
                // chia sẻ một người dùng
            }
        }

        // Nhật ký chia sẻ file
        public IActionResult LogShareFile(int? page)
        {
            var userID = HttpContext.Session.GetString("UserId");
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

            var sharefile = _context.SharedFiles
                            .AsNoTracking()
                            .Include(x => x.SharedWithUser)
                            .Include(x => x.File)
                            .Where(x => x.File.UserId == Convert.ToInt32(userID))
                            .OrderByDescending(x => x.DateShared);
            PagedList<SharedFile> models = new PagedList<SharedFile>(sharefile, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // Lịch sử xóa file
        public IActionResult HistoryDeleteFile(int? page)
        {
            var userID = HttpContext.Session.GetString("UserId");

            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var lsFiles = _context.Files
                .AsNoTracking()
                .Include(x => x.TypeFile)
                .Where(x => x.UserId == Convert.ToInt32(userID) && x.IsActive == false)
                .OrderBy(x => x.DateUploaded);
            PagedList<File> models = new PagedList<File>(lsFiles, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }


        [HttpPost]
        public async Task<IActionResult> HistoryDeleteFile(int fileid)
        {
            var f = await _context.Files.FindAsync(fileid);
            if (f == null)
            {
                return NotFound();
            }
            f.IsActive = true;

            _context.Files.Update(f);
            await _context.SaveChangesAsync();
            _notyfService.Success("Khôi phục file thành công");
            return RedirectToAction("TrangChuUser", "Home");
        }
    }
}
