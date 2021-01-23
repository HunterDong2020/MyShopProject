using MyShop.DataAccess.Base;
using MyShop.Model.Role;
using MyShop.Model.Role.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DataAccess.Role.IRepository
{
    public interface IMenuAndRoleRelationRepository : IBaseRepository<MenuAndRoleRelationEntity>
    {
        List<MenuAndRoleRelationEntity> QueryRoleAndMenuList(MenuAndRoleRelationRequest request);

        bool SetRoleAndMenuRelation(List<MenuAndRoleRelationEntity> list, string roleId);
    }
}
