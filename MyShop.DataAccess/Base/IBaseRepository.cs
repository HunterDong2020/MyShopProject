using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Base
{
    public interface IBaseRepository<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 通过 传入SQL参数条件 获取实体对象
        /// Add by Jason.Song on 2018/10/18
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="parameters">SQL参数</param>
        /// <param name="instanceName">数据库连接实例</param>
        /// <param name="operationType">标注此数据库实例是支持读写、只读、只写功能</param>
        /// <returns>返回实体对象</returns>
        T GetEntityByParameters<T>(SqlParameters parameters);

        /// <summary>
        /// 通过 传入SQL参数条件 获取实体对象集合
        /// Add by Jason.Song on 2018/10/18
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="parameters">SQL参数</param>
        /// <param name="multiSql">多表关联查询扩展</param>
        /// <param name="instanceName">数据库连接实例</param>
        /// <param name="operationType">标注此数据库实例是支持读写、只读、只写功能</param>
        /// <returns>返回实体对象集合</returns>
        IEnumerable<T> GetEntitiesByParameters<T>(SqlParameters parameters, string multiSql = null);

        /// <summary>
        /// 通过 传入SQL参数条件 物理删除数据 (谨慎使用)
        /// Add by Jason.Song on 2018/12/20
        /// </summary>
        /// <typeparam name="T">实体泛型</typeparam>
        /// <param name="parameters">SQL参数【不能为空】</param>
        /// <param name="instanceName">数据库连接实例</param>
        /// <param name="operationType">标注此数据库实例是支持读写、只读、只写功能</param>
        /// <returns></returns>
        int PhysicallyDeleteDataByParameters<T>(SqlParameters parameters);
    }

}
