using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyShop.Core.Role.IService;
using MyShop.Model.Base;
using MyShop.Model.Role;
using MyShop.Model.Role.Request;
using MyShop.Model.Role.Response;
using MyShop.WebAdmin.Controllers.Base;
using Newtonsoft.Json;

namespace MyShop.WebAdmin.Controllers.Role
{
    [Authorize]
    public class AdminRoleController : BaseController
    {
        private readonly ILogger<AdminRoleController> _logger;
        private readonly IRoleService _roleService;

        public AdminRoleController(ILogger<AdminRoleController> logger, IRoleService roleServer)
        {
            _logger = logger;
            _roleService = roleServer;
        }

        public IActionResult Index()
        {
            return View("~/Views/Role/AdminRole.cshtml");
        }

        public IActionResult RoleBindUser(int isView, string roleId)
        {
            roleId = string.IsNullOrEmpty(roleId) ? "" : roleId;
            //1:表示查看已绑定用户  2;绑定用户
            ViewBag.IsView = isView;
            ViewBag.RoleId = roleId;
            return View("~/Views/Role/AdminRoleBindUser.cshtml");
        }

        public IActionResult RoleBindMenu(string roleId)
        {
            ViewBag.RoleId = roleId;
            return View("~/Views/Role/AdminRoleBindMenu.cshtml");
        }

        public IActionResult GetAdminRoleList(string roleName, int limit, int page)
        {
            try
            {
                var paramRequest = new RoleRequest
                {
                    RoleName = roleName,
                    PageIndex = page,
                    PageSize = limit
                };
                var result = _roleService.QueryAdminRoleListPage(paramRequest);

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"获取角色列表异常:{ex.ToString() + ex.StackTrace}");
                return Json(new AdminRoleResponse() { msg = "异常", code = -1 });
            }
        }

        [HttpPost]
        public IActionResult AddAdminRole(string param)
        {
            if (string.IsNullOrEmpty(param))
                return Json(new BaseResponse { IsSuccess = false, Msg = "请求参数不能为空" });

            var userObj = JsonConvert.DeserializeObject<RoleEntity>(param);
            userObj.Id = Guid.NewGuid().ToString("N").ToUpper();
            userObj.CreateTime = DateTime.Now;
            userObj.UpdateTime = DateTime.Now;
            userObj.CreateUser = base.CurrentLoginUser.UserName;
            userObj.UpdateUser = base.CurrentLoginUser.UserName;
            userObj.IsDelete = 0;

            var userInfo = _roleService.InsertAdminRole(userObj);
            return Json(userInfo);
        }

        [HttpPost]
        public IActionResult DeleteAdminRole(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return Json(new BaseResponse { IsSuccess = false, Msg = "请求参数不能为空" });

            var res = _roleService.BathDeleteAdminRole(new RoleRequest { IdList = ids.Split(',').ToList() });
            return Json(new BaseResponse { IsSuccess = res, Msg = res ? "成功" : "操作失败" });
        }

        [HttpPost]
        public IActionResult BindRoleAndUser(string roleAndUser)
        {
            if (string.IsNullOrEmpty(roleAndUser))
                return Json(new BaseResponse { IsSuccess = false, Msg = "请求参数不能为空" });
            var list = JsonConvert.DeserializeObject<List<RoleAndUserRelationEntity>>(roleAndUser);
            list.ForEach(p =>
            {
                p.Id = Guid.NewGuid().ToString("N").ToUpper();
                p.CreateUser = base.CurrentLoginUser.UserName;
                p.UpdateUser = base.CurrentLoginUser.UserName;
            });
            var res = _roleService.BindRoleAndUserRelation(list);
            return Json(res);
        }

        public IActionResult GetAdminRoleBindUserList(string userName, string roleId, int limit, int page)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return Json(new RoleAndUserRelationResponse() { msg = "关联角色信息失败", code = -1 });
            }
            try
            {
                var paramRequest = new RoleAndUserRelationRequest
                {
                    UserName = userName,
                    RoleId = roleId,
                    PageIndex = page,
                    PageSize = limit
                };
                var result = _roleService.QueryRoleBindUser(paramRequest);

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"获取角色绑定列表异常:{ex.ToString() + ex.StackTrace}");
                return Json(new RoleAndUserRelationResponse() { msg = "异常", code = -1 });
            }
        }

        public IActionResult SetRoleAndMenu(string roleId, string menuIds)
        {
            roleId = string.IsNullOrEmpty(roleId) ? "" : roleId.Trim();
            menuIds = string.IsNullOrEmpty(menuIds) ? "" : menuIds.Trim();
            if (roleId == "")
                return Json(new { result = false, msg = "参数不能为空" });

            var roleList = new List<MenuAndRoleRelationEntity>();
            //新增关联
            if (menuIds != "")
            {
                for (var i = 0; i < menuIds.Split(',').Length; i++)
                {
                    var entity = new MenuAndRoleRelationEntity()
                    {
                        Id = Guid.NewGuid().ToString("N").ToUpper(),
                        RoleId = roleId,
                        MenuId = menuIds.Split(',')[i],
                        CreateTime = DateTime.Now,
                        CreateUser = base.CurrentLoginUser.UserName,
                        UpdateUser = base.CurrentLoginUser.UserName,
                        IsDelete = 0
                    };
                    roleList.Add(entity);
                }
            }
            var res = _roleService.BindMenuAndRole(roleList, roleId);
            return Json(res);
        }
    }
}
