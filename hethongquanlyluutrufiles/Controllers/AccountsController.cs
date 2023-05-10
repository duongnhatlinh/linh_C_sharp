using AspNetCoreHero.ToastNotification.Abstractions;
using hethongquanlyluutrufiles.Extension;
using hethongquanlyluutrufiles.helpper;
using hethongquanlyluutrufiles.Models;
using hethongquanlyluutrufiles.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace hethongquanlyluutrufiles.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly hethongquanlyluutrufileContext _context;
        public INotyfService _notyfService { get; }
        public AccountsController(hethongquanlyluutrufileContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        public IActionResult ChuyenTrang()
        {
            var AccountID = HttpContext.Session.GetString("UserId");
            if (AccountID != null)
            {
                var _user = _context.Users.AsNoTracking().SingleOrDefault(x => x.UserId == Convert.ToInt32(AccountID));
                if (_user != null)
                {
                    if (_user.IsAdminOrUser == false)
                    {
                        return RedirectToAction("TrangChuAdmin", "Home", new { area = "Admin" });
                    }    
                    else
                    {
                        return RedirectToAction("TrangChuUser", "Home"); 
                    }
                }

            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }



        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel Account)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Utilities.IsValidEmail(Account.Email) == false)
                    {
                        TempData["ErrorEmail"] = "Vui lòng nhập đúng dạng email";
                        return RedirectToAction("Register", "Accounts");
                    }
                    var acc = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email.ToLower() == Account.Email.ToLower());
                    if (acc != null)
                    {
                        TempData["ErrorMessage"] = "Email của bạn đã được sử dụng";
                        return RedirectToAction("Register", "Accounts");
                    }
                    string salt = Utilities.GetRandomKey();
                    User _user = new User
                    {
                        Username = Account.Username,
                        Phone = Account.Phone.Trim().ToLower(),
                        Email = Account.Email.Trim().ToLower(),
                        Password = (Account.Password + salt.Trim()).ToMD5(),
                        Salt = salt,
                        IsAdminOrUser = true

                    };
                    try
                    {
                        _context.Users.Add(_user);
                        await _context.SaveChangesAsync();
                        //Lưu Session 
                        HttpContext.Session.SetString("UserId", _user.UserId.ToString());
                        HttpContext.Session.SetString("Username", _user.Username.ToString());

                        //Identity
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name,_user.Username),
                            new Claim("UserId", _user.UserId.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        _notyfService.Success("Đăng ký thành công");
                        return RedirectToAction("Create", "Users");
                    }
                    catch
                    {
                        return View(Account);
                    }
                }
                else
                {
                    return View(Account);
                }
            }
            catch
            {
                return View(Account);
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            var AccountID = HttpContext.Session.GetString("UserId");

            if (AccountID != null)
            {
                return RedirectToAction("ChuyenTrang", "Accounts");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel acc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Utilities.IsValidEmail(acc.Email) == false)
                    {
                        TempData["ErrorEmail"] = "Vui lòng nhập đúng dạng email";
                        return RedirectToAction("Register", "Accounts");
                    }

                    var _user  = _context.Users.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == acc.Email);

                    if (_user == null) return RedirectToAction("Register", "Accounts");
                    var pass = (acc.Password + _user.Salt.Trim()).ToMD5();
                    if (_user.Password != pass)
                    {
                        TempData["ErrorMessage"] = "Thông tin đăng nhập chưa chính xác";
                        return RedirectToAction("Login", "Accounts");
                    }

                    //Luu Session 
                    HttpContext.Session.SetString("UserId", _user.UserId.ToString());
                    HttpContext.Session.SetString("Username", _user.Username.ToString());
                    var taikhoanID = HttpContext.Session.GetString("UserId");

                    //Identity
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,_user.Username),
                        new Claim("UserId", _user.UserId.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    _notyfService.Success("Đăng nhập thành công");
                    return RedirectToAction("ChuyenTrang", "Accounts");
                }
            }
            catch
            {
                _notyfService.Success("Tài khoản chưa được đăng ký");
                return RedirectToAction("Register", "Accounts");
            }
            return View(acc);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login", "Accounts");
        }


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
                        return RedirectToAction("ChuyenTrang", "Accounts");
                    }
                }
            }
            catch
            {
                _notyfService.Success("Thay đổi mật khẩu không thành công");
                return RedirectToAction("ChuyenTrang", "Accounts");
            }
            _notyfService.Success("Thay đổi mật khẩu không thành công");
            return RedirectToAction("ChuyenTrang", "Accounts");
        }
    }
}
