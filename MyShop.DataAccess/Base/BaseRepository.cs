
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 创建数据库连接，并打开连接
        /// 连接字符串写在 json 配置文件里面
        /// </summary>
        /// <returns>IDbConnection</returns>
        public IDbConnection CreateDbConnection()
        {
            IDbConnection con = null;
            string connectionString = DbConnectionStringConfig.Default.MyShopConnectionString;
            con = new SqlConnection(connectionString);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("数据库连接错误:" + ex.Message);
            }

            return con;
        }

        /// <inheritdoc />
        /// <summary>
        /// 通过 传入SQL参数条件 获取实体对象
        /// Add by Jason.Song on 2018/10/18
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="parameters">SQL参数</param>
        /// <param name="instanceName">数据库连接实例</param>
        /// <param name="operationType">标注此数据库实例是支持读写、只读、只写功能</param>
        /// <returns>返回实体对象</returns>
        public T GetEntityByParameters<T>(SqlParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.TableName))
                parameters.TableName = typeof(T).Name;

            string selectSql;
            if (typeof(T) == typeof(int))
            {
                selectSql = $"SELECT COUNT(1) FROM {parameters.TableName} WITH(NOLOCK) " + parameters.BuildWhereSql();
            }
            else
            {
                selectSql = $"SELECT * FROM {parameters.TableName} WITH(NOLOCK) " + parameters.BuildWhereSql() + parameters.BuildSortSql();
            }
            using (var conn = CreateDbConnection())
            {
                return typeof(T) == typeof(int) ? conn.ExecuteScalar<T>(selectSql, parameters.BuildParameters()) : conn.QueryFirstOrDefault<T>(selectSql, parameters.BuildParameters());
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 通过 传入SQL参数条件 获取实体对象集合
        /// Add by Jason.Song on 2018/10/18
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="parameters">SQL参数【不能为空】</param>
        /// <param name="multiSql">多表关联查询扩展</param>
        /// <param name="instanceName">数据库连接实例</param>
        /// <param name="operationType">标注此数据库实例是支持读写、只读、只写功能</param>
        /// <returns>返回实体对象集合</returns>
        public IEnumerable<T> GetEntitiesByParameters<T>(SqlParameters parameters, string multiSql = null)
        {
            if (string.IsNullOrWhiteSpace(parameters.TableName))
                parameters.TableName = typeof(T).Name;
            //SQL查询语句
            string sql;
            if (parameters.EnablePaging)
            {
                var compute = parameters.BuildComputeCondition();
                var selectSql = multiSql ?? $@"SELECT * FROM {parameters.TableName} WITH(NOLOCK) {parameters.BuildWhereSql()}";
                sql = $@";WITH _data AS ( {selectSql} ), 
                _count AS(SELECT COUNT(1) AS TotalRecords FROM _data) {compute.Item1}
                SELECT * FROM _data CROSS APPLY _count {compute.Item2} {parameters.BuildSortSql()} 
                OFFSET ( {parameters.PageIndex} - 1 ) * {parameters.PageSize} ROWS FETCH NEXT {parameters.PageSize} ROWS ONLY";
            }
            else
            {
                var limit = parameters.Limit > 0 ? $"TOP {parameters.Limit}" : "";
                sql = multiSql ?? $"SELECT {limit} * FROM {parameters.TableName} WITH(NOLOCK) "
                            + parameters.BuildWhereSql() + parameters.BuildSortSql();
            }

            using (var conn = CreateDbConnection())
            {
                return conn.Query<T>(sql, parameters.BuildParameters());
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 通过 传入SQL参数条件 物理删除数据 (谨慎使用)
        /// Add by Jason.Song on 2018/12/20
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="parameters">SQL参数【不能为空】</param>
        /// <param name="instanceName">数据库连接实例</param>
        /// <param name="operationType">标注此数据库实例是支持读写、只读、只写功能</param>
        /// <returns></returns>
        public int PhysicallyDeleteDataByParameters<T>(SqlParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.TableName))
                parameters.TableName = typeof(T).Name;

            var whereSql = parameters.BuildWhereSql();
            if (string.IsNullOrWhiteSpace(whereSql)) return 0;
            var deleteSql = $"DELETE FROM {parameters.TableName} " + whereSql;
            using (var conn = CreateDbConnection())
            {
                return conn.Execute(deleteSql, parameters.BuildParameters());
            }
        }
    }
}
