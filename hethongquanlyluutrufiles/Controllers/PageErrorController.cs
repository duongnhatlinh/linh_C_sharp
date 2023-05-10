using Microsoft.AspNetCore.Mvc;

namespace hethongquanlyluutrufiles.Controllers
{
    public class PageErrorController : Controller
    {
        public IActionResult Error()
        {
            return View();
        }
    }
}
