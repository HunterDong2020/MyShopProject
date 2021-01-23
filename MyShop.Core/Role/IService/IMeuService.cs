using MyShop.Model.Base;
using MyShop.Model.Role;
using MyShop.Model.Role.Request;
using MyShop.Model.Role.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Core.Role.IService
{
    public interface IMeuService
    {
        List<MenuEntity> GetAllMenuList();

        MenuEntity GetMenuById(string id);

        BaseResponse DeleteMenuById(string id);

        BaseResponse InsertOrUpdateAdminMenu(MenuEntity entity,string type);

        List<MenuAndRoleRelationEntity> GetRoleAndMenuList(MenuAndRoleRelationRequest request);
    }
}
