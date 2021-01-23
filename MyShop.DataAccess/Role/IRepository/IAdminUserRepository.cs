using MyShop.DataAccess.Base;
using MyShop.Model.Role;
using MyShop.Model.Role.Request;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MyShop.DataAccess.Role.IRepository
{
    public interface IAdminUserRepository : IBaseRepository<AdminUserEntity>
    {
        AdminUserEntity QueryAdminUserOne(AdminUserRequest request);

        List<AdminUserEntity> QueryUserMangeList(AdminUserRequest request, out int total);

        bool InsertAdminUser(AdminUserEntity entity);

        bool BathStartOrLimitAdminUser(AdminUserRequest request);

        bool UpdateAdminUser(AdminUserEntity entity);
    }
}
