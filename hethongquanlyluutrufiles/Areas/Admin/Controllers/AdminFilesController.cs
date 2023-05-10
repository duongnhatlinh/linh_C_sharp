using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using hethongquanlyluutrufiles.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using PagedList.Core;
using Microsoft.AspNetCore.Http;
using System.IO;
using File = hethongquanlyluutrufiles.Models.File;

namespace hethongquanlyluutrufiles.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminFilesController : Controller
    {
        private readonly hethongquanlyluutrufileContext _context;

        public INotyfService _notyfService { get; }

        public AdminFilesController(hethongquanlyluutrufileContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }


        public IActionResult Index(int page = 1, int typeFileID = 0)
        {
            var pageNumber = page;
            var pageSize = 10;

            var userID = HttpContext.Session.GetString("UserId");
            List<File> listfile = new List<File>();

            if (typeFileID != 0)
            {
                listfile = _context.Files
                    .AsNoTracking()
                    .Where(x => x.UserId == Convert.ToInt32(userID) && x.TypeFileId == typeFileID && x.IsActive == true)
                    .Include(x => x.TypeFile)
                    .OrderByDescending(x => x.DateUploaded).ToList();
            }
            else
            {
                listfile = _context.Files
                    .AsNoTracking()
                    .Where(x => x.UserId == Convert.ToInt32(userID) && x.IsActive == true)
                    .Include(x => x.TypeFile)
                    .OrderByDescending(x => x.DateUploaded).ToList();
            }


            PagedList<File> models = new PagedList<File>(listfile.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentTypeFileID = typeFileID;
            ViewBag.CurrentPage = pageNumber;
            ViewData["typefile"] = new SelectList(_context.TypeFiles, "TypeFileId", "TypeFileName");

            return View(models);
        }

        public IActionResult Filtter(int typeFileID = 0)
        {
            var url = $"/Admin/AdminFiles?typeFileID={typeFileID}";
            if (typeFileID == 0)
            {
                url = $"/Admin/AdminFiles";
            }
            return Json(new { status = "success", redirectUrl = url });
        }
        // GET: Admin/AdminFiles/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Admin/AdminFiles/Create
        public IActionResult Create()
        {
            ViewData["typefile"] = new SelectList(_context.TypeFiles, "TypeFileId", "TypeFileName");

            return View();
        }

        // POST: Admin/AdminFiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FileId,UserId,FileName,FilePath,FileSize,DateUploaded,TypeFileId,IsActive")] File f, [FromForm] IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                var filesize = myfile.Length;
                if (myfile == null || filesize == 0)
                {
                    ModelState.AddModelError("", "Vui lòng chọn một file để tải lên.");
                    return View();
                }
                // get the file name and extension
                string fileName = Path.GetFileName(myfile.FileName);
                string fileExtension = Path.GetExtension(fileName);

                // create a unique file name using a GUID
                string uniqueFileName = Guid.NewGuid().ToString().Substring(0, 10) + fileExtension;
                f.FilePath = uniqueFileName;

                // get the path to the uploads directory
                string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "FileUploaded");

                // create the uploads directory if it doesn't exist
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // create the full path to the uploaded file
                string filePath = Path.Combine(uploadsPath, uniqueFileName);


                // copy the file to the uploads directory
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    myfile.CopyTo(fileStream);
                }
                var userID = HttpContext.Session.GetString("UserId");

                f.FileSize = (filesize/1024);
                f.DateUploaded = DateTime.Now;
                f.UserId = Convert.ToInt32(userID);

                _context.Add(f);
                await _context.SaveChangesAsync();
                _notyfService.Success("Tạo file mới thành công");
                return RedirectToAction(nameof(Index));
            }
            ViewData["typefile"] = new SelectList(_context.TypeFiles, "TypeFileId", "TypeFileName");
            return View(f);
        }

        // GET: Admin/AdminFiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
       
            var file = await _context.Files.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }
            ViewData["typefile"] = new SelectList(_context.TypeFiles, "TypeFileId", "TypeFileName");
            return View(file);
        }

        // POST: Admin/AdminFiles/Edit/5
  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Fileid, [Bind("FileId,UserId,FileName,FilePath,FileSize,DateUploaded,TypeFileId,IsActive")] File file)
        {
            if (Fileid != file.FileId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(file);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Chỉnh sửa file thành công");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileExists(file.FileId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["typefile"] = new SelectList(_context.TypeFiles, "TypeFileId", "TypeFileName");
            return View(file);
        }

        // GET: Admin/AdminFiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Admin/AdminFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            File f = await _context.Files.FindAsync(id);
            if(f == null)
            {
                return NotFound();
            }
            f.IsActive = false;
            _context.Update(f);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa file thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool FileExists(int id)
        {
            return _context.Files.Any(e => e.FileId == id);
        }
    }
}
