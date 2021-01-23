using MyShop.DataAccess.Base;
using MyShop.Model.Role;
using MyShop.Model.Role.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DataAccess.Role.IRepository
{
    public interface IRoleAndUserRelationRepository : IBaseRepository<RoleAndUserRelationEntity>
    {
        List<RoleAndUserRelationEntity> QueryRoleBindUser(RoleAndUserRelationRequest request, out int total);

        bool InsertRoleAndUserRelation(List<RoleAndUserRelationEntity> entity);
    }
}
