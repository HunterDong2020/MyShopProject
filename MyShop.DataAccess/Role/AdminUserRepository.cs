using Microsoft.Extensions.Configuration;
using MyShop.DataAccess.Base;
using MyShop.DataAccess.Role.IRepository;
using MyShop.Model.Role;
using MyShop.Model.Role.Request;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using MyShop.Common.Denpendency;

namespace MyShop.DataAccess.Role
{
    [DIdependent]
    public class AdminUserRepository : BaseRepository<AdminUserEntity>, IAdminUserRepository
    {
        public AdminUserEntity QueryAdminUserOne(AdminUserRequest request)
        {
            DynamicParameters dp = new DynamicParameters();
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(" select [Id],[UserName],[PassWord],[IsDelete],[CreateUser],[CreateTime],[UpdateUser],[UpdateTime],[RealName],[MobilePhone],[Remark] from tblAdminUser with(nolock) where 1=1 ");
            if (!string.IsNullOrEmpty(request.UserName))
            {
                strSQL.Append(" and  UserName=@UserName ");
                dp.Add("UserName", request.UserName, DbType.String, ParameterDirection.Input, 50);
            }
            if (!string.IsNullOrEmpty(request.PassWord))
            {
                strSQL.Append(" and  PassWord=@PassWord ");
                dp.Add("PassWord", request.UserName, DbType.String, ParameterDirection.Input, 50);
            }
            using (DbConnection conn = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                return conn.Query<AdminUserEntity>(strSQL.ToString(), request).FirstOrDefault();
            }
        }

        public List<AdminUserEntity> QueryUserMangeList(AdminUserRequest request, out int total)
        {
            total = 0;
            List<AdminUserEntity> list = new List<AdminUserEntity>();
            StringBuilder sq = new StringBuilder();
            sq.Append(" select * from ( ");
            sq.Append(" select {2} ");
            sq.Append(" from dbo.tblAdminUser a with(nolock) ");
            sq.Append(" where {0} ");
            sq.Append(" ) c where {1} ");
            DynamicParameters dp = new DynamicParameters();
            string where_1 = " 1=1 ";
            if (!string.IsNullOrWhiteSpace(request.UserName))
            {
                where_1 += " and a.UserName=@UserName";
                dp.Add("UserName", request.UserName, DbType.String);
            }

            dp.Add("PageIndex", request.PageIndex, DbType.Int32, ParameterDirection.Input);
            dp.Add("PageSize", request.PageSize, DbType.Int32, ParameterDirection.Input);

            string sql_list = string.Format(sq.ToString(), where_1, " c.Num > (@PageIndex - 1) * @PageSize and c.Num <= @PageIndex * @PageSize", "ROW_NUMBER() over(order by a.CreateTime desc) as Num,* ");

            string sql_count = string.Format(sq.ToString(), where_1, "1=1", "count(0) as nums");

            using (IDbConnection conn = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                total = conn.Query<int>(sql_count, dp).FirstOrDefault();
                list = conn.Query<AdminUserEntity>(sql_list, dp).ToList();
            }
            return list;
        }

        public bool InsertAdminUser(AdminUserEntity entity)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into tblAdminUser (");
            strSql.Append("Id,UserName,PassWord,CreateUser,IsDelete,UpdateUser,UpdateTime,RealName,MobilePhone,Remark)");
            strSql.Append(" values (");
            strSql.Append("@Id,@UserName,@PassWord,@CreateUser,@IsDelete,@UpdateUser,@UpdateTime,@RealName,@MobilePhone,@Remark)");

            using (IDbConnection connection = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                return connection.Execute(strSql.ToString(), entity) > 0;
            }
        }

        public bool UpdateAdminUser(AdminUserEntity entity)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" update dbo.tblAdminUser set PassWord=@PassWord,");
            strSql.Append(" RealName=@RealName,MobilePhone=@MobilePhone,Remark=@Remark where UserName = @UserName ");

            using (IDbConnection connection = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                return connection.Execute(strSql.ToString(), entity) > 0;
            }
        }

        public bool BathStartOrLimitAdminUser(AdminUserRequest request)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [dbo].[tblAdminUser] where Id in @ids ");
            using (IDbConnection connection = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                return connection.Execute(strSql.ToString(), new { ids = request.IdList.ToArray(), IsDelete = request.IsDelete }) > 0;
            }
        }
    }
}
