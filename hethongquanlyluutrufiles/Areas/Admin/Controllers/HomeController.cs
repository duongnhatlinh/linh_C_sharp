using AspNetCoreHero.ToastNotification.Abstractions;
using hethongquanlyluutrufiles.Extension;
using hethongquanlyluutrufiles.Models;
using hethongquanlyluutrufiles.ModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hethongquanlyluutrufiles.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly hethongquanlyluutrufileContext _context;
        public INotyfService _notyfService { get; }
        public HomeController(hethongquanlyluutrufileContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult TrangChuAdmin()
        {
            return View();
        }

        // đổi mật khẩu cho admin
        public IActionResult ChangePassword()
        {
            var AccountID = HttpContext.Session.GetString("UserId");
            if (AccountID == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var AccountID = HttpContext.Session.GetString("UserId");
                    var acc = _context.Users.Find(Convert.ToInt32(AccountID));
                    if (acc == null) return RedirectToAction("Login", "Accounts");
                    var pass = (model.PasswordCurrent.Trim() + acc.Salt.Trim()).ToMD5();
                    {
                        string passnew = (model.PasswordNew.Trim() + acc.Salt.Trim()).ToMD5();
                        acc.Password = passnew;
                        _context.Update(acc);
                        _context.SaveChanges();
                        _notyfService.Success("Đổi mật khẩu thành công");
                        return RedirectToAction("ChuyenTrang", "Accounts", new {area = ""});
                    }
                }
            }
            catch
            {
                _notyfService.Success("Thay đổi mật khẩu không thành công");
                return RedirectToAction("ChuyenTrang", "Accounts", new { area = "" });
            }
            _notyfService.Success("Thay đổi mật khẩu không thành công");
            return RedirectToAction("ChuyenTrang", "Accounts", new { area = "" });
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
            return RedirectToAction("TrangChuAdmin", "Home");
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
                            .OrderBy(x => x.DateShared);
            PagedList<SharedFile> models = new PagedList<SharedFile>(sharefile, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

    }
}
