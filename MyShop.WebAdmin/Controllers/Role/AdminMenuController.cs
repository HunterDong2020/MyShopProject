using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyShop.Common.Utility;
using MyShop.Core.Role.IService;
using MyShop.Model.Base;
using MyShop.Model.Role;
using MyShop.Model.Role.Response;
using MyShop.WebAdmin.Controllers.Base;
using Newtonsoft.Json;

namespace MyShop.WebAdmin.Controllers.Role
{
    [Authorize]
    public class AdminMenuController : BaseController
    {
        private readonly IMeuService _menuService;
        private readonly ILogger<AdminMenuController> _logger;
        public AdminMenuController(ILogger<AdminMenuController> logger, IMeuService menuServer)
        {
            _logger = logger;
            _menuService = menuServer;
        }

        public IActionResult Index()
        {
            return View("~/Views/Role/AdminMenu.cshtml");
        }

        public IActionResult MenuFormIndex(string id, string parentmenu, string type)
        {
            ViewBag.MenuId = id;
            ViewBag.ParentMenuName = parentmenu;
            ViewBag.Type = type;

            return View("~/Views/Role/AdminMenuForm.cshtml");
        }

        /// <summary>
        /// 获取所有菜单信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetALLMenu(string roleId, string menuName)
        {
            roleId = string.IsNullOrEmpty(roleId) ? "" : roleId;
            menuName = string.IsNullOrEmpty(menuName) ? "" : menuName;

            var allMenu = _menuService.GetAllMenuList();
            var menuIquery = allMenu.OrderBy(p => p.ParentMenuId).ThenBy(p => p.MenuSort).ToList();
            if (roleId == "" && menuName == "")
            {
                return Json(menuIquery);
            }
            if (menuName != "" && roleId == "")
            {
                return Json(menuIquery.Where(p => p.MenuName.Contains(menuName)).ToList());
            }
            if (menuIquery.Count() > 0)
            {
                var roleAndMenu = _menuService.GetRoleAndMenuList(new Model.Role.Request.MenuAndRoleRelationRequest());
                menuIquery.ForEach(p =>
                {
                    p.NoCheck = roleAndMenu.Where(u => u.MenuId == p.Id && u.RoleId == roleId).Count() > 0;
                });
            }
            return Json(menuIquery);
        }

        [HttpPost]
        public JsonResult GetMenuById(string id)
        {
            var menuEntity = _menuService.GetMenuById(id);
            return Json(menuEntity);
        }

        [HttpPost]
        public JsonResult DeleteMenu(string id)
        {
            var res = _menuService.DeleteMenuById(id);
            return Json(res);
        }

        [HttpPost]
        public JsonResult InsertOrUpdateAdminMenu(string param, string type)
        {
            if (string.IsNullOrEmpty(param) || string.IsNullOrEmpty(type))
            {
                return Json(new BaseResponse { IsSuccess = false, Msg = "参数不能为空" });
            }
            var menuEntity = JsonConvert.DeserializeObject<MenuEntity>(param);
            if (type == "insert")
            {
                menuEntity.Id = Guid.NewGuid().ToString("N").ToUpper();
            }
            else if (type == "update")
            {
                menuEntity.Id = menuEntity.MenuId;
            }
            menuEntity.CreateUser = base.CurrentLoginUser.UserName;
            menuEntity.UpdateUser = base.CurrentLoginUser.UserName;
            menuEntity.MenuFontCss = "";
            menuEntity.UpdateTime = DateTime.Now;

            var res = _menuService.InsertOrUpdateAdminMenu(menuEntity, type);
            return Json(res);
        }
    }
}
