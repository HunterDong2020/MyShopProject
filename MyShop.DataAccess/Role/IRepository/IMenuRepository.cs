using MyShop.DataAccess.Base;
using MyShop.Model.Role;
using MyShop.Model.Role.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DataAccess.Role.IRepository
{
    public interface IMenuRepository : IBaseRepository<MenuEntity>
    {
        List<MenuEntity> QueryALLMenuList(AdminMenuRequest request);

        bool DeleteMenuById(List<string> ids);

        bool InsertAdminMenu(MenuEntity entity);

        bool UpdateAdminMenu(MenuEntity entity);
    }
}
