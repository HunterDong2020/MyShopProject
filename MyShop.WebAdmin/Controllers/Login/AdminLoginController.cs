using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyShop.Core.Role.IService;
using MyShop.Model.Base;
using Microsoft.AspNetCore.Http;

namespace MyShop.WebAdmin.Controllers.Login
{
    public class AdminLoginController : Controller
    {
        private readonly ILogger<AdminLoginController> _logger;
        private readonly IAdminUserService _adminUser;

        public AdminLoginController(ILogger<AdminLoginController> logger, IAdminUserService adminService)
        {
            _logger = logger;
            this._adminUser = adminService;
        }

        public IActionResult Index()
        {
            return View("~/Views/Login/AdminLogin.cshtml");
        }

        public IActionResult Login(string username, string password)
        {
            username = string.IsNullOrEmpty(username) ? "" : username;
            password = string.IsNullOrEmpty(password) ? "" : password;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Json(new BaseResponse { IsSuccess = false, Msg = "请完善信息后登录" });
            }

            var userEntity = _adminUser.QueryAdminUserEntity(new Model.Role.Request.AdminUserRequest { UserName = username, PassWord = password });
            if (userEntity != null)
            {
                #region 登录认证，存入Cookie
                //登录认证，存入Cookie
                var claims = new List<Claim>(){
                                  new Claim(ClaimTypes.Name,username),new Claim("Id",userEntity.Id)
                             };
                //init the identity instances 
                var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Customer"));
                //signin 
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                    IsPersistent = false,
                    AllowRefresh = false
                });
                #endregion
                return Json(new BaseResponse { IsSuccess = true, Msg = "登录成功" });
            }
            else
            {
                return Json(new BaseResponse { IsSuccess = false, Msg = "用户名或密码输入错误，请重新输入" });
            }
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "AdminLogin");
        }
    }
}
