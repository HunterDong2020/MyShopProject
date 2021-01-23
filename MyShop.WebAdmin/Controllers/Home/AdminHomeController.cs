using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyShop.Common.Utility;
using MyShop.Core.Role.IService;
using MyShop.WebAdmin.Controllers.Base;

namespace MyShop.WebAdmin.Controllers.Home
{
    [Authorize]
    public class AdminHomeController : BaseController
    {
        private readonly ILogger<AdminHomeController> _logger;
        private readonly IRoleService _roleService;
        public AdminHomeController(ILogger<AdminHomeController> logger, IRoleService roleServer)
        {
            _logger = logger;
            _roleService = roleServer;
        }

        public IActionResult Index()
        {
            var menuList = _roleService.QueryRoleBindMenuListByUserName(base.CurrentLoginUser.UserName);
            ViewData["LeftMenus"] = menuList;
            ViewBag.Contact = base.CurrentLoginUser.UserName;

            return View("~/Views/Home/Index.cshtml");
        }

        public IActionResult Error()
        {
            return View("~/Views/Home/Error.cshtml");
        }
    }
}
