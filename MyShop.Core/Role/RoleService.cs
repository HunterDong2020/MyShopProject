using MyShop.Common.Denpendency;
using MyShop.Common.Utility;
using MyShop.Core.Role.IService;
using MyShop.DataAccess.Role.IRepository;
using MyShop.Model.Base;
using MyShop.Model.Role;
using MyShop.Model.Role.Request;
using MyShop.Model.Role.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyShop.Core.Role
{
    [DIdependent]
    public class RoleService : IRoleService
    {
        IRoleRepository adminRole;
        IMenuRepository menuRepository;
        IRoleAndUserRelationRepository roleAndUserRelation;
        IMenuAndRoleRelationRepository menuAndRoleRelation;
        public RoleService(IRoleRepository _adminRole, IRoleAndUserRelationRepository _roleAndUser, IMenuAndRoleRelationRepository _menuAndRole, IMenuRepository _menuRepository)
        {
            this.adminRole = _adminRole;
            this.roleAndUserRelation = _roleAndUser;
            this.menuAndRoleRelation = _menuAndRole;
            this.menuRepository = _menuRepository;
        }

        public AdminRoleResponse QueryAdminRoleListPage(RoleRequest request)
        {
            int total = 0;
            var pageList = adminRole.QueryRoleList(request, out total);

            AdminRoleResponse page = new AdminRoleResponse();
            if (pageList != null && pageList.Count > 0)
            {
                page.code = 0;
                page.msg = "success";
                page.count = total;
                page.data = pageList;
            }
            else
            {
                page.msg = "无数据";
                page.code = -1;
            }
            return page;
        }

        public BaseResponse InsertAdminRole(RoleEntity entity)
        {
            var roleEntity = adminRole.QueryAdminRoleOne(new RoleRequest { RoleName = entity.RoleName });
            if (roleEntity != null)
            {
                return new BaseResponse { Msg = $"角色{entity.RoleName}已经添加" };
            }
            var res = adminRole.InsertAdminUser(entity);
            NLogHelper.Default.Info($"InsertAdminRole=>添加角色结果:{res}");
            return new BaseResponse
            {
                IsSuccess = res,
                Msg = res ? "成功" : "操作失败"
            };
        }

        public bool BathDeleteAdminRole(RoleRequest request)
        {
            return adminRole.DeleteRole(request);
        }

        public BaseResponse BindRoleAndUserRelation(List<RoleAndUserRelationEntity> list)
        {
            var res = roleAndUserRelation.InsertRoleAndUserRelation(list);
            return new BaseResponse { IsSuccess = res, Msg = res ? "成功" : "失败" };
        }

        public RoleAndUserRelationResponse QueryRoleBindUser(RoleAndUserRelationRequest request)
        {
            int total = 0;
            var pageList = roleAndUserRelation.QueryRoleBindUser(request, out total);

            RoleAndUserRelationResponse page = new RoleAndUserRelationResponse();
            if (pageList != null && pageList.Count > 0)
            {
                page.code = 0;
                page.msg = "success";
                page.count = total;
                page.data = pageList;
            }
            else
            {
                page.msg = "无数据";
                page.code = -1;
            }
            return page;
        }

        public BaseResponse BindMenuAndRole(List<MenuAndRoleRelationEntity> role, string roleId)
        {
            var res = menuAndRoleRelation.SetRoleAndMenuRelation(role, roleId);
            return new BaseResponse { IsSuccess = res, Msg = res ? "成功" : "失败" };
        }

        /// <summary>
        /// 根据登录用户名获取菜单信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<AdminMenuResponse> QueryRoleBindMenuListByUserName(string userName)
        {
            //判断是否是超级管理员
            var isAdmin = AppSettingConfig.SuperAdminAccount == userName;
            List<MenuEntity> menulist = new List<MenuEntity>();
            if (isAdmin)
                menulist = menuRepository.QueryALLMenuList(new AdminMenuRequest());
            else
                menulist = adminRole.QueryRoleBindMenuListByUserName(userName);

            List<AdminMenuResponse> menuRes = new List<AdminMenuResponse>();

            if (menulist != null && menulist.Count > 0)
            {
                //一级菜单
                var rootNode = menulist.Where(p => p.MenuName == "菜单根节点").OrderBy(p => p.CreateTime).FirstOrDefault();
                //二级菜单
                var twoLevelNode = menulist.Where(p => p.ParentMenuId == rootNode.Id).ToList();
                foreach (var item in twoLevelNode)
                {
                    //三级菜单
                    var thirdLevalNode = menulist.Where(p => p.ParentMenuId == item.Id).ToList();

                    var res = new AdminMenuResponse
                    {
                        MenuId = item.Id,
                        MenuName = item.MenuName,
                        MenuUrl = item.MenuUrl,
                        MenuList = thirdLevalNode
                    };
                    menuRes.Add(res);
                }
            }
            return menuRes;
        }
    }
}
