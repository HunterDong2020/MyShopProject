using MyShop.DataAccess.Base;
using MyShop.DataAccess.Role.IRepository;
using MyShop.Model.Role;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using MyShop.Model.Role.Request;

namespace MyShop.DataAccess.Role
{
    public class MenuRepository : BaseRepository<MenuEntity>, IMenuRepository
    {
        public List<MenuEntity> QueryALLMenuList(AdminMenuRequest request)
        {
            DynamicParameters dp = new DynamicParameters();
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(" select * from tblAdminMenu with(nolock) where 1=1 ");
            if (!string.IsNullOrEmpty(request.MenuId))
            {
                strSQL.Append(" and Id=@Id ");
                dp.Add("Id", request.MenuId, System.Data.DbType.String, System.Data.ParameterDirection.Input, 50);
            }
            using (DbConnection conn = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                return conn.Query<MenuEntity>(strSQL.ToString(), dp).ToList();
            }
        }

        public bool DeleteMenuById(List<string> ids)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(" delete from tblAdminMenu where Id in @Ids ");
            using (DbConnection conn = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                return conn.Execute(strSQL.ToString(), new { Ids = ids.ToArray() }) > 0;
            }
        }

        public bool InsertAdminMenu(MenuEntity entity)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append("  insert into [dbo].[tblAdminMenu] (Id,[MenuName],[MenuUrl],[MenuFontCss],[MenuSort],[ParentMenuId],[CreateUser],[UpdateUser]) values (@Id, @MenuName, @MenuUrl, @MenuFontCss, @MenuSort, @ParentMenuId, @CreateUser, @UpdateUser) ");
            using (DbConnection conn = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                return conn.Execute(strSQL.ToString(), entity) > 0;
            }
        }

        public bool UpdateAdminMenu(MenuEntity entity)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append("update [dbo].[tblAdminMenu] set MenuName=@MenuName,MenuUrl=@MenuUrl,MenuSort=@MenuSort,UpdateUser=@UpdateUser,UpdateTime=@UpdateTime where Id=@Id ");
            using (DbConnection conn = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                return conn.Execute(strSQL.ToString(), entity) > 0;
            }
        }
    }
}
