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
                //可以使用HttpContext.SignInAsync方法的重载来定义持久化cookie存储用户认证信息，例如下面的代码就定义了用户登录后30分钟内cookie都会保留在客户端计算机硬盘上，
                //即便用户关闭了浏览器，30分钟内再次访问站点仍然是处于登录状态，除非调用Logout方法注销登录。
                //注意其中的AllowRefresh属性，如果AllowRefresh为true，表示如果用户登录后在超过50%的ExpiresUtc时间间隔内又访问了站点，就延长用户的登录时间（其实就是延长cookie在客户端计算机硬盘上的保留时间），
                //例如本例中我们下面设置了ExpiresUtc属性为30分钟后，那么当用户登录后在大于15分钟且小于30分钟内访问了站点，那么就将用户登录状态再延长到当前时间后的30分钟。但是用户在登录后的15分钟内访问站点是不会延长登录时间的，
                //因为ASP.NET Core有个硬性要求，是用户在超过50%的ExpiresUtc时间间隔内又访问了站点，才延长用户的登录时间。
                //如果AllowRefresh为false，表示用户登录后30分钟内不管有没有访问站点，只要30分钟到了，立马就处于非登录状态（不延长cookie在客户端计算机硬盘上的保留时间，30分钟到了客户端计算机就自动删除cookie）
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                    IsPersistent = false,
                    AllowRefresh = true
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
