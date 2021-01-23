using MyShop.Model.Base;
using MyShop.Model.Role;
using MyShop.Model.Role.Request;
using MyShop.Model.Role.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Core.Role.IService
{
    public interface IRoleService
    {
        AdminRoleResponse QueryAdminRoleListPage(RoleRequest request);

        BaseResponse InsertAdminRole(RoleEntity entity);

        bool BathDeleteAdminRole(RoleRequest request);

        BaseResponse BindRoleAndUserRelation(List<RoleAndUserRelationEntity> list);

        RoleAndUserRelationResponse QueryRoleBindUser(RoleAndUserRelationRequest request);

        BaseResponse BindMenuAndRole(List<MenuAndRoleRelationEntity> role, string roleId);

        List<AdminMenuResponse> QueryRoleBindMenuListByUserName(string userName);
    }
}
