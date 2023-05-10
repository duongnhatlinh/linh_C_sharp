using hethongquanlyluutrufiles.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace hethongquanlyluutrufiles.Controllers
{
    public class SearchController : Controller
    {
        private readonly hethongquanlyluutrufileContext _context;

        public SearchController(hethongquanlyluutrufileContext context)
        {
            _context = context;
        }


        // Tìm kiếm file cá nhân
        [HttpPost]
        public IActionResult FindFilesPersonal(string keyword)
        {
            var userID = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                List<File> ls1 = new List<File>();
                ls1 = _context.Files.AsNoTracking()
                                    .Include(a => a.TypeFile)
                                    .Where(x => x.UserId == Convert.ToInt32(userID) && x.IsActive == true)
                                    .OrderByDescending(x => x.DateUploaded)
                                    .Take(10)
                                    .ToList();
                return PartialView("FindFilesPersonalPartial", ls1);
            }
            else
            {

                List<File> ls2 = new List<File>();
                ls2 = _context.Files.AsNoTracking()
                                    .Include(a => a.TypeFile)
                                    .Where(x => x.UserId == Convert.ToInt32(userID) && x.IsActive == true && x.FileName.Contains(keyword))
                                    .OrderByDescending(x => x.DateUploaded)
                                    .Take(10)
                                    .ToList();
                if (ls2 == null)
                {
                    return PartialView("FindFilesPersonalPartial", null);
                }
                else
                {
                    return PartialView("FindFilesPersonalPartial", ls2);
                }
            } 

        }

        // Tìm kiếm file chia sẻ cho toàn bộ người dùng
        [HttpPost]
        public IActionResult FindFilesSharedAll(string keyword)
        {
            var userID = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                List<File> ls1 = new List<File>();
                ls1 = _context.Files.AsNoTracking()
                                    .Include(a => a.TypeFile)
                                    .Where(x => x.UserId == Convert.ToInt32(userID) && x.IsActive == true)
                                    .OrderByDescending(x => x.DateUploaded)
                                    .Take(10)
                                    .ToList();
                return PartialView("AllFindFilesSharePartial", ls1);
            }
            else
            {

                List<File> ls2 = new List<File>();
                ls2 = _context.Files.AsNoTracking()
                                    .Include(a => a.TypeFile)
                                    .Where(x => x.UserId == Convert.ToInt32(userID) && x.IsActive == true && x.FileName.Contains(keyword))
                                    .OrderByDescending(x => x.DateUploaded)
                                    .Take(10)
                                    .ToList();
                if (ls2 == null)
                {
                    return PartialView("AllFindFilesSharePartial", null);
                }
                else
                {
                    return PartialView("AllFindFilesSharePartial", ls2);
                }
            }

        }

        // Tìm kiếm file chia sẻ cho nhóm người dùng

        [HttpPost]
        public IActionResult FindFilesSharedGroup(string keyword)
        {
            var userID = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                List<File> ls1 = new List<File>();
                ls1 = _context.Files.AsNoTracking()
                                    .Include(a => a.TypeFile)
                                    .Where(x => x.UserId == Convert.ToInt32(userID) && x.IsActive == true)
                                    .OrderByDescending(x => x.DateUploaded)
                                    .Take(10)
                                    .ToList();
                return PartialView("GroupFindFilesSharePartial", ls1);
            }
            else
            {

                List<File> ls2 = new List<File>();
                ls2 = _context.Files.AsNoTracking()
                                    .Include(a => a.TypeFile)
                                    .Where(x => x.UserId == Convert.ToInt32(userID) && x.IsActive == true && x.FileName.Contains(keyword))
                                    .OrderByDescending(x => x.DateUploaded)
                                    .Take(10)
                                    .ToList();
                if (ls2 == null)
                {
                    return PartialView("GroupFindFilesSharePartial", null);
                }
                else
                {
                    return PartialView("GroupFindFilesSharePartial", ls2);
                }
            }
        }

        // Tìm kiếm người dùng chia sẻ

        [HttpPost]
        public IActionResult FindUsersSharedOne(string keyword)
        {
            var userID = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                List<User> ls1 = new List<User>();
                ls1 = _context.Users.Include(x => x.Department)
                                    .Include(x => x.Role)
                                    .AsNoTracking()
                                    .Where(x => x.UserId != Convert.ToInt32(userID))
                                    .OrderByDescending(x => x.UserId)
                                    .Take(10)
                                    .ToList();
                return PartialView("OneFindUsersSharePartial", ls1);
            }
            else
            {

                List<User> ls2 = new List<User>();
                ls2 = _context.Users.Include(x => x.Department)
                                    .Include(x => x.Role)
                                    .AsNoTracking()
                                    .Where(x => x.UserId != Convert.ToInt32(userID) && x.Username.Contains(keyword))
                                    .OrderByDescending(x => x.UserId)
                                    .Take(10)
                                    .ToList();
                if (ls2 == null)
                {
                    return PartialView("OneFindUsersSharePartial", null);
                }
                else
                {
                    return PartialView("OneFindUsersSharePartial", ls2);
                }
            }

        }

        // Tím kiếm file  chia sẻ cho một người dùng

        [HttpPost]
        public IActionResult FindFilesSharedOne(string keyword)
        {
            var userID = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                List<File> ls1 = new List<File>();
                ls1 = _context.Files.AsNoTracking()
                                    .Include(a => a.TypeFile)
                                    .Where(x => x.UserId == Convert.ToInt32(userID) && x.IsActive == true)
                                    .OrderByDescending(x => x.DateUploaded)
                                    .Take(10)
                                    .ToList();
                return PartialView("OneFindFilesSharePartial", ls1);
            }
            else
            {

                List<File> ls2 = new List<File>();
                ls2 = _context.Files.AsNoTracking()
                                    .Include(a => a.TypeFile)
                                    .Where(x => x.UserId == Convert.ToInt32(userID) && x.IsActive == true && x.FileName.Contains(keyword))
                                    .OrderByDescending(x => x.DateUploaded)
                                    .Take(10)
                                    .ToList();
                if (ls2 == null)
                {
                    return PartialView("OneFindFilesSharePartial", null);
                }
                else
                {
                    return PartialView("OneFindFilesSharePartial", ls2);
                }
            }
        }


    }
}
