using MyShop.DataAccess.Base;
using MyShop.DataAccess.Role.IRepository;
using MyShop.Model.Role;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dapper;
using MyShop.Model.Role.Request;
using System.Linq;
using MyShop.Common.Denpendency;

namespace MyShop.DataAccess.Role
{
    [DIdependent]
    public class RoleAndUserRelationRepository : BaseRepository<RoleAndUserRelationEntity>, IRoleAndUserRelationRepository
    {
        public List<RoleAndUserRelationEntity> QueryRoleBindUser(RoleAndUserRelationRequest request, out int total)
        {
            total = 0;
            List<RoleAndUserRelationEntity> list = new List<RoleAndUserRelationEntity>();
            StringBuilder sq = new StringBuilder();
            sq.Append(" select * from ( ");
            sq.Append(" select {2} from tblRoleAndUserRelation r inner join tblAdminUser u on r.UserMasterId = u.Id   ");
            sq.Append(" inner join tblAdminRole o on o.Id = r.RoleId ");
            sq.Append(" where {0} ");
            sq.Append(" ) c where {1} ");
            DynamicParameters dp = new DynamicParameters();
            string where_1 = " 1=1 ";
            if (!string.IsNullOrWhiteSpace(request.UserName))
            {
                where_1 += " and u.UserName=@UserName";
                dp.Add("UserName", request.UserName, DbType.String, ParameterDirection.Input, 50);
            }
            if (!string.IsNullOrWhiteSpace(request.RoleId))
            {
                where_1 += " and r.RoleId=@RoleId";
                dp.Add("RoleId", request.RoleId, DbType.String, ParameterDirection.Input, 50);
            }
            dp.Add("PageIndex", request.PageIndex, DbType.Int32, ParameterDirection.Input);
            dp.Add("PageSize", request.PageSize, DbType.Int32, ParameterDirection.Input);

            string sql_list = string.Format(sq.ToString(), where_1, " c.Num > (@PageIndex - 1) * @PageSize and c.Num <= @PageIndex * @PageSize", "ROW_NUMBER() over(order by r.CreateTime desc) as Num,o.RoleName,u.UserName,r.CreateTime,r.CreateUser,u.RealName,u.MobilePhone ");

            string sql_count = string.Format(sq.ToString(), where_1, "1=1", "count(0) as nums");

            using (IDbConnection conn = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                total = conn.Query<int>(sql_count, dp).FirstOrDefault();
                list = conn.Query<RoleAndUserRelationEntity>(sql_list, dp).ToList();
            }
            return list;
        }

        public bool InsertRoleAndUserRelation(List<RoleAndUserRelationEntity> entity)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into tblRoleAndUserRelation (");
            strSql.Append("Id,RoleId,UserMasterId,IsDelete,CreateUser,UpdateUser)");
            strSql.Append(" values (");
            strSql.Append("@Id,@RoleId,@UserMasterId,@IsDelete,@CreateUser,@UpdateUser)");

            using (IDbConnection connection = new SqlConnection(DbConnectionStringConfig.Default.MyShopConnectionString))
            {
                return connection.Execute(strSql.ToString(), entity) > 0;
            }
        }
    }
}
