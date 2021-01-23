using MyShop.DataAccess.Base;
using MyShop.DataAccess.Role.IRepository;
using MyShop.Model.Role;
using MyShop.Model.Role.Request;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using MyShop.Common.Utility;
using MyShop.Common.Denpendency;

namespace MyShop.DataAccess.Role
{
    [DIdependent]
    public class MenuAndRoleRelationRepository : BaseRepository<MenuAndRoleRelationEntity>, IMenuAndRoleRelationRepository
    {
        public List<MenuAndRoleRelationEntity> QueryRoleAndMenuList(MenuAndRoleRelationRequest request)
        {
            DynamicParameters dp = new DynamicParameters();
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(" select * from tblMenuAndRoleRelation with(nolock) where 1=1 ");
            if (!string.IsNullOrEmpty(request.Id))
            {
                strSQL.Append(" and  Id=@Id ");
                dp.Add("Id", request.Id, DbType.String, ParameterDirection.Input, 50);
            }
            if (!string.IsNullOrEmpty(request.RoleId))
            {
                strSQL.Append(" and  RoleId=@RoleId ");
                dp.Add("RoleId", request.RoleId, DbType.String, ParameterDirection.Input, 50);
            }
            if (!string.IsNullOrEmpty(request.MenuId))
            {
                strSQL.Append(" and  MenuId=@MenuId ");
                dp.Add("MenuId", request.MenuId, DbType.String, ParameterDirection.Input, 50);
            }
            using (DbConnection conn = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                return conn.Query<MenuAndRoleRelationEntity>(strSQL.ToString(), request).ToList();
            }
        }

        public bool SetRoleAndMenuRelation(List<MenuAndRoleRelationEntity> list, string roleId)
        {
            StringBuilder strSQLDel = new StringBuilder();
            strSQLDel.Append(" delete from tblMenuAndRoleRelation where RoleId=@RoleId ");

            StringBuilder strSQLInsert = new StringBuilder();
            strSQLInsert.Append(" insert into dbo.tblMenuAndRoleRelation ([Id],[MenuId] ,[RoleId],[IsDelete] ,[CreateUser],[UpdateUser]) values (@Id, @MenuId, @RoleId, @IsDelete, @CreateUser, @UpdateUser) ");

            using (IDbConnection connection = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();
                try
                {
                    connection.Execute(strSQLDel.ToString(), new { RoleId = roleId }, transaction);
                    if (list != null && list.Count > 0)
                    {
                        connection.Execute(strSQLInsert.ToString(), list, transaction);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    NLogHelper.Default.Info($"角色设置绑定菜单异常:{ex.ToString() + ex.StackTrace}");
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }
    }
}
