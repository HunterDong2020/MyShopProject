using MyShop.Model.Base;
using MyShop.Model.Role;
using MyShop.Model.Role.Request;
using MyShop.Model.Role.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Core.Role.IService
{
    public interface IAdminUserService
    {
        AdminUserEntity QueryAdminUserEntity(AdminUserRequest request);

        AdminUserResponse QueryUserListPage(AdminUserRequest request);

        BaseResponse InsertOrUpdateAdminUser(AdminUserEntity entity, string type);

        bool BathStartOrLimitAdminUser(AdminUserRequest request);
    }
}
