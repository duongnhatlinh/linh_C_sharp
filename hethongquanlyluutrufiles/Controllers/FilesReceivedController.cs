using AspNetCoreHero.ToastNotification.Abstractions;
using hethongquanlyluutrufiles.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using File = hethongquanlyluutrufiles.Models.File;

namespace hethongquanlyluutrufiles.Controllers
{
    public class FilesReceivedController : Controller
    {
        private readonly hethongquanlyluutrufileContext _context;
        public INotyfService _notyfService { get; }

        public FilesReceivedController(hethongquanlyluutrufileContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        
        public IActionResult Index(int? page)
        {
            var userID = HttpContext.Session.GetString("UserId");
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

            var sharefile = _context.SharedFiles
                            .AsNoTracking()
                            .Include(x => x.File)
                            .Include(x => x.File.User)
                            .Where(x => x.SharedWithUserId == Convert.ToInt32(userID) && x.File.IsActive == true)
                            .OrderByDescending(x => x.DateShared);
            PagedList<SharedFile> models = new PagedList<SharedFile>(sharefile, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }
      
        public ActionResult Details(int? id, int? sharefileid)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (sharefileid == null)
            {
                return NotFound();
            }

            var file = _context.Files
                .Include(f => f.TypeFile)
                .Include(f => f.User)
                .FirstOrDefault(m => m.FileId == id);


            if (file == null)
            {
                return NotFound();
            }

            var sharefile = _context.SharedFiles.AsNoTracking().FirstOrDefault(x => x.SharedFileId == sharefileid);
            if (sharefile.Notify == true)
            {
                sharefile.Notify = false;
                _context.Update(sharefile);
                _context.SaveChangesAsync();
            }
            
            return View(file);
        }


        public async Task<IActionResult> Download(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.Files
                .Include(f => f.TypeFile)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FileId == id);
            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }
        [HttpPost, ActionName("Download")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadConfirmed(int id)
        {
            var file = await _context.Files.FindAsync(id);
            string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "FileUploaded");
            string filePath = Path.Combine(uploadsPath, file.FilePath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.FilePath);
        }
    }
}
