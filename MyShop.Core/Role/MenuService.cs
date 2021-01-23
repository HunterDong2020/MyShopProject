using MyShop.Core.Role.IService;
using MyShop.DataAccess.Role.IRepository;
using MyShop.Model.Role.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MyShop.Model.Role;
using MyShop.Model.Base;
using MyShop.Model.Role.Request;
using MyShop.Common.Denpendency;

namespace MyShop.Core.Role
{
    [DIdependent]
    public class MenuService : IMeuService
    {
        IMenuRepository menuRepository;
        IMenuAndRoleRelationRepository menuAndRoleRelation;

        public MenuService(IMenuRepository _menuRepository, IMenuAndRoleRelationRepository _menuAndRole)
        {
            this.menuRepository = _menuRepository;
            this.menuAndRoleRelation = _menuAndRole;
        }

        public List<MenuEntity> GetAllMenuList()
        {
            var list = menuRepository.QueryALLMenuList(new Model.Role.Request.AdminMenuRequest());
            return list;
        }

        public MenuEntity GetMenuById(string id)
        {
            var allMenu = menuRepository.QueryALLMenuList(new AdminMenuRequest());
            var entity = allMenu.Where(p => p.Id == id).FirstOrDefault();
            //获取父菜单名称
            if (entity != null && !string.IsNullOrEmpty(entity.ParentMenuId))
            {
                var parentMenu = allMenu.Where(p => p.Id == entity.ParentMenuId).FirstOrDefault();
                if (parentMenu != null)
                {
                    entity.ParentName = parentMenu.MenuName;
                }
            }
            return entity;
        }

        public BaseResponse InsertOrUpdateAdminMenu(MenuEntity entity, string type)
        {
            var res = false;
            if (type == "insert")
            {
                res = menuRepository.InsertAdminMenu(entity);
            }
            else if (type == "update")
            {
                res = menuRepository.UpdateAdminMenu(entity);
            }
            return new BaseResponse { IsSuccess = res, Msg = res ? "成功" : "失败" };
        }

        public BaseResponse DeleteMenuById(string id)
        {
            var allMenu = menuRepository.QueryALLMenuList(new AdminMenuRequest());
            var deleteIds = new List<string>();
            deleteIds.Add(id);
            //判读是否有子菜单
            var subMenu = allMenu.Where(p => p.ParentMenuId == id).ToList();
            if (subMenu != null)
            {
                subMenu.ForEach(p =>
                {
                    deleteIds.Add(p.Id);
                });
            }
            var res = menuRepository.DeleteMenuById(deleteIds);
            return new BaseResponse { IsSuccess = res, Msg = res ? "成功" : "失败" };
        }

        public List<MenuAndRoleRelationEntity> GetRoleAndMenuList(MenuAndRoleRelationRequest request)
        {
            return menuAndRoleRelation.QueryRoleAndMenuList(request);
        }
    }
}
