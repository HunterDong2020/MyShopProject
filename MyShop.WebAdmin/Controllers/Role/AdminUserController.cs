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
    public class AdminUserController : BaseController
    {
        private readonly ILogger<AdminUserController> _logger;
        private readonly IAdminUserService _isAdminUserService;

        public AdminUserController(ILogger<AdminUserController> logger, IAdminUserService adminUser)
        {
            _logger = logger;
            _isAdminUserService = adminUser;
        }

        public IActionResult Index()
        {
            return View("~/Views/Role/AdminUser.cshtml");
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        public IActionResult GetUserManageList(string userName, string roleId, int limit, int page)
        {
            var paramRequest = new AdminUserRequest
            {
                UserName = userName,
                RoleId = roleId,
                PageIndex = page,
                PageSize = limit
            };
            var result = _isAdminUserService.QueryUserListPage(paramRequest);

            //_logger.LogInformation($"用户数据:{JsonConvert.SerializeObject(result)}");
            return Json(result);
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddAdminUser(string param, string type)
        {
            if (string.IsNullOrEmpty(param) || string.IsNullOrEmpty(type))
                return Json(new BaseResponse { IsSuccess = false, Msg = "请求参数不能为空" });

            var userObj = JsonConvert.DeserializeObject<AdminUserEntity>(param);
            userObj.Id = Guid.NewGuid().ToString("N").ToUpper();
            userObj.CreateTime = DateTime.Now;
            userObj.UpdateTime = DateTime.Now;
            userObj.CreateUser = base.CurrentLoginUser.UserName;
            userObj.UpdateUser = base.CurrentLoginUser.UserName;
            userObj.IsDelete = 0;

            var userInfo = _isAdminUserService.InsertOrUpdateAdminUser(userObj, type);
            return Json(userInfo);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BathDeleteAdminUser(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                return Json(new BaseResponse { IsSuccess = false, Msg = "请求参数不能为空" });

            var res = _isAdminUserService.BathStartOrLimitAdminUser(new AdminUserRequest { IdList = ids.Split(',').ToList() });
            return Json(new BaseResponse { IsSuccess = res, Msg = res ? "成功" : "操作失败" });
        }
    }
}
