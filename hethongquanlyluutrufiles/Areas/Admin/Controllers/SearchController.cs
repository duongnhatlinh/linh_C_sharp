using hethongquanlyluutrufiles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace hethongquanlyluutrufile.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly hethongquanlyluutrufileContext _context;

        public SearchController(hethongquanlyluutrufileContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult FindFiles(string keyword)
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
                return PartialView("ListFilesSearchPartial", ls1);
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
                    return PartialView("ListFilesSearchPartial", null);
                }
                else
                {
                    return PartialView("ListFilesSearchPartial", ls2);
                }
            }

        }
        [HttpPost]
        public IActionResult FindUsers(string keyword)
        {
            var userID = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                List<User> ls1 = new List<User>();
                ls1 = _context.Users.Include(x => x.Department).AsNoTracking()
                                    .Where(x => x.UserId != Convert.ToInt32(userID))
                                    .OrderByDescending(x => x.UserId)
                                    .Take(10)
                                    .ToList();
                return PartialView("ListUsersSearchPartial", ls1);
            }
            else
            {

                List<User> ls2 = new List<User>();
                ls2 = _context.Users.Include(x => x.Department).AsNoTracking()
                                    .Where(x => x.UserId != Convert.ToInt32(userID) && x.Username.Contains(keyword))
                                    .OrderByDescending(x => x.UserId)
                                    .Take(10)
                                    .ToList();
                if (ls2 == null)
                {
                    return PartialView("ListUsersSearchPartial", null);
                }
                else
                {
                    return PartialView("ListUsersSearchPartial", ls2);
                }
            }

        }


        // Tìm kiếm file gửi cho toàn bộ người dùng
        [HttpPost]
        public IActionResult FindFilesShared_All(string keyword)
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
                return PartialView("All_ListFilesSharedSearchPartial", ls1);
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
                    return PartialView("All_ListFilesSharedSearchPartial", null);
                }
                else
                {
                    return PartialView("All_ListFilesSharedSearchPartial", ls2);
                }
            }

        }

        // Tìm kiếm file gửi cho nhóm người dùng
        [HttpPost]
        public IActionResult FindFilesShared_Group(string keyword)
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
                return PartialView("Group_ListFilesSharedSearchPartial", ls1);
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
                    return PartialView("Group_ListFilesSharedSearchPartial", null);
                }
                else
                {
                    return PartialView("Group_ListFilesSharedSearchPartial", ls2);
                }
            }
        }

        // Tìm kiếm file gửi cho mỗi người dùng
        [HttpPost]
        public IActionResult FindFilesShared_One(string keyword)
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
                return PartialView("One_ListFilesSharedSearchPartial", ls1);
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
                    return PartialView("One_ListFilesSharedSearchPartial", null);
                }
                else
                {
                    return PartialView("One_ListFilesSharedSearchPartial", ls2);
                }
            }
        }



        // Tìm kiếm gửi mỗi người dùng
        [HttpPost]
        public IActionResult FindUsersShared_One(string keyword)
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
                return PartialView("One_ListUsersSharedSearchPartial", ls1);
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
                    return PartialView("One_ListUsersSharedSearchPartial", null);
                }
                else
                {
                    return PartialView("One_ListUsersSharedSearchPartial", ls2);
                }
            }

        }
    }
}