using Dapper;
using MyShop.DataAccess.Base;
using MyShop.DataAccess.Role.IRepository;
using MyShop.Model.Role;
using MyShop.Model.Role.Request;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using System.Data.Common;
using MyShop.Common.Utility;

namespace MyShop.DataAccess.Role
{
    public class RoleRepository : BaseRepository<RoleEntity>, IRoleRepository
    {
        public RoleEntity QueryAdminRoleOne(RoleRequest request)
        {
            DynamicParameters dp = new DynamicParameters();
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(" select * from tblAdminRole with(nolock) where 1=1 ");
            if (!string.IsNullOrEmpty(request.Id))
            {
                strSQL.Append(" and  Id=@Id ");
                dp.Add("Id", request.Id, DbType.String, ParameterDirection.Input, 50);
            }
            if (!string.IsNullOrEmpty(request.RoleName))
            {
                strSQL.Append(" and  RoleName=@RoleName ");
                dp.Add("RoleName", request.RoleName, DbType.String, ParameterDirection.Input, 50);
            }
            using (IDbConnection conn = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                return conn.Query<RoleEntity>(strSQL.ToString(), request).FirstOrDefault();
            }
        }

        public List<RoleEntity> QueryRoleList(RoleRequest request, out int total)
        {
            total = 0;
            List<RoleEntity> list = new List<RoleEntity>();
            StringBuilder sq = new StringBuilder();
            sq.Append(" select * from ( ");
            sq.Append(" select {2} ");
            sq.Append(" from dbo.tblAdminRole a with(nolock) ");
            sq.Append(" where {0} ");
            sq.Append(" ) c where {1} ");
            DynamicParameters dp = new DynamicParameters();
            string where_1 = " 1=1 ";
            if (!string.IsNullOrWhiteSpace(request.RoleName))
            {
                where_1 += " and a.RoleName=@RoleName";
                dp.Add("RoleName", request.RoleName, DbType.String);
            }

            dp.Add("PageIndex", request.PageIndex, DbType.Int32, ParameterDirection.Input);
            dp.Add("PageSize", request.PageSize, DbType.Int32, ParameterDirection.Input);

            string sql_list = string.Format(sq.ToString(), where_1, " c.Num > (@PageIndex - 1) * @PageSize and c.Num <= @PageIndex * @PageSize", "ROW_NUMBER() over(order by a.CreateTime desc) as Num,* ");

            string sql_count = string.Format(sq.ToString(), where_1, "1=1", "count(0) as nums");

            using (IDbConnection conn = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                total = conn.Query<int>(sql_count, dp).FirstOrDefault();
                list = conn.Query<RoleEntity>(sql_list, dp).ToList();
            }
            return list;
        }

        public bool InsertAdminUser(RoleEntity entity)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into tblAdminRole (");
            strSql.Append("Id,RoleName,IsDelete,CreateUser,UpdateUser)");
            strSql.Append(" values (");
            strSql.Append("@Id,@RoleName,@IsDelete,@CreateUser,@UpdateUser)");

            using (IDbConnection connection = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                return connection.Execute(strSql.ToString(), entity) > 0;
            }
        }

        public bool DeleteRole(RoleRequest request)
        {
            //1.删除角色
            StringBuilder strSqlDelRole = new StringBuilder();
            strSqlDelRole.Append("delete from [dbo].[tblAdminRole] where Id in @ids ");
            //2.删除角色和菜单绑定关系
            StringBuilder strSQLDelMenu = new StringBuilder();
            strSQLDelMenu.Append(" delete from [dbo].[tblMenuAndRoleRelation] where RoleId=@ids ");
            //2.删除角色和用户绑定关系
            StringBuilder strSQLDelUser = new StringBuilder();
            strSQLDelUser.Append(" delete from [dbo].[tblRoleAndUserRelation] where RoleId=@ids ");

            using (IDbConnection connection = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();
                try
                {
                    connection.Execute(strSqlDelRole.ToString(), new { ids = request.IdList.ToArray() }, transaction);
                    connection.Execute(strSQLDelMenu.ToString(), new { ids = request.IdList.ToArray() }, transaction);
                    connection.Execute(strSQLDelUser.ToString(), new { ids = request.IdList.ToArray() }, transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    NLogHelper.Default.Info($"删除角色异常:{ex.ToString() + ex.StackTrace}");
                    transaction.Rollback();
                    return false;
                }
            }
            return true;



        }

        public List<MenuEntity> QueryRoleBindMenuListByUserName(string userName)
        {
            DynamicParameters dp = new DynamicParameters();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select distinct m.* from [dbo].tblAdminMenu as m with(nolock) ");
            strSql.Append(" left join [dbo].tblMenuAndRoleRelation as mr with(nolock) on m.Id = mr.MenuId ");
            strSql.Append(" left join [dbo].tblAdminRole as pr with(nolock) on pr.Id = mr.RoleId ");
            strSql.Append(" where mr.RoleId in  ");
            strSql.Append(" (select ur.RoleId from [dbo].[tblRoleAndUserRelation] as ur with(nolock) ");
            strSql.Append(" left join [dbo].[tblAdminUser] as u with(nolock) on ur.UserMasterId = u.Id where u.UserName = @UserName ) ");

            dp.Add("UserName", userName, DbType.String, ParameterDirection.Input, 50);
            using (IDbConnection connection = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                return connection.Query<MenuEntity>(strSql.ToString(), dp).ToList();
            }
        }
    }
}
