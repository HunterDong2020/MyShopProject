using MyShop.Core.Role.IService;
using MyShop.DataAccess.Role.IRepository;
using MyShop.Model.Role.Request;
using MyShop.Model.Role.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MyShop.Model.Role;
using MyShop.Model.Base;
using MyShop.Common.Utility;
using Newtonsoft.Json;
using MyShop.Common.Denpendency;

namespace MyShop.Core.Role
{
    [DIdependent]
    public class AdminUserService : IAdminUserService
    {
        IAdminUserRepository adminUser;
        public AdminUserService(IAdminUserRepository _adminUser)
        {
            this.adminUser = _adminUser;
        }

        public AdminUserEntity QueryAdminUserEntity(AdminUserRequest request)
        {
            var userEntity = adminUser.QueryAdminUserOne(request);
            return userEntity;
        }

        public AdminUserResponse QueryUserListPage(AdminUserRequest request)
        {
            int total = 0;
            var pageList = adminUser.QueryUserMangeList(request, out total);

            AdminUserResponse page = new AdminUserResponse();
            if (pageList != null && pageList.Count > 0)
            {
                page.code = 0;
                page.msg = "success";
                page.count = total;
                page.data = pageList.Where(p => p.UserName != AppSettingConfig.SuperAdminAccount).ToList();
                //NLogHelper.Default.Info($"service=>查询后台管理用户:{JsonConvert.SerializeObject(page)}");
            }
            else
            {
                page.msg = "无数据";
                page.code = -1;
            }
            return page;
        }

        public BaseResponse InsertOrUpdateAdminUser(AdminUserEntity entity, string type)
        {
            var res = false;
            if (type == "insert")
            {
                var userEntity = adminUser.QueryAdminUserOne(new AdminUserRequest { UserName = entity.UserName });
                if (userEntity != null)
                {
                    return new BaseResponse { IsSuccess = false, Msg = $"{entity.UserName}已经添加，请使用原账号" };
                }
                res = adminUser.InsertAdminUser(entity);
            }
            else
            {
                res = adminUser.UpdateAdminUser(entity);
            }
            NLogHelper.Default.Info($"InsertAdminUser=>{type}用户结果:{res}");
            return new BaseResponse
            {
                IsSuccess = res,
                Msg = res ? "成功" : "操作失败"
            };
        }

        public bool BathStartOrLimitAdminUser(AdminUserRequest request)
        {
            return adminUser.BathStartOrLimitAdminUser(request);
        }
    }
}
