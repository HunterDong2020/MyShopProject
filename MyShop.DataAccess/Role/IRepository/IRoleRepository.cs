using MyShop.DataAccess.Base;
using MyShop.Model.Role;
using MyShop.Model.Role.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DataAccess.Role.IRepository
{
    public interface IRoleRepository : IBaseRepository<RoleEntity>
    {
        RoleEntity QueryAdminRoleOne(RoleRequest request);

        List<RoleEntity> QueryRoleList(RoleRequest request, out int total);

        bool InsertAdminUser(RoleEntity entity);

        bool DeleteRole(RoleRequest request);

        List<MenuEntity> QueryRoleBindMenuListByUserName(string userName);
    }
}
