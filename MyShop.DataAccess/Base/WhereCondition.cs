using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MyShop.DataAccess.Base
{
    /// <summary>
    /// Where 条件参数属性
    /// Add by Jason.Song on 2018/06/25
    /// </summary>
    public class WhereCondition
    {
        public WhereCondition()
        {
            Value = new List<object>();
        }

        /// <summary>
        /// 与前一个条件的关联
        /// </summary>
        public RelationType RelationType { get; set; } = RelationType.And;
        /// <summary>
        /// 前缀
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 判断条件
        /// </summary>
        public QueryType QueryType { get; set; } = QueryType.Equal;
        /// <summary>
        /// 列值
        /// </summary>
        public List<object> Value { get; set; }
        /// <summary>
        /// 多值关联条件
        /// </summary>
        public RelationType ValueType { get; set; } = RelationType.Or;
    }

    /// <summary>
    /// 关联类型
    /// Add by Jason.Song on 2018/06/25
    /// </summary>
    public enum RelationType
    {
        /// <summary>
        /// And
        /// </summary>
        And,
        /// <summary>
        /// Or
        /// </summary>
        Or
    }

    /// <summary>
    /// 查询语法
    /// Add by Jason.Song on 2018/06/25
    /// </summary>
    public enum QueryType
    {
        /// <summary>
        /// Equal
        /// </summary>
        Equal,
        /// <summary>
        /// LIKE
        /// </summary>
        Like,
        /// <summary>
        /// In
        /// </summary>
        In,
        /// <summary>
        /// BETWEEN
        /// </summary>
        Between
    }

    /// <summary>
    /// 排序类型
    /// Add by Jason.Song on 2018/10/18
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// 升序
        /// </summary>
        Asc,
        /// <summary>
        /// 降序
        /// </summary>
        Desc
    }

    /// <summary>
    /// Sql 条件参数
    /// Add by Jason.Song on 2018/06/25
    /// </summary>
    public class SqlParameters
    {

        /// <summary>
        /// 获取WHERE条件参数
        /// </summary>
        public Dictionary<string, WhereCondition> WhereParameters { get; } = new Dictionary<string, WhereCondition>();
        /// <summary>
        /// 获取排序参数
        /// </summary>
        public Dictionary<string, SortType> SortParameters { get; } = new Dictionary<string, SortType>();
        /// <summary>
        /// 计算条件
        /// 第一个字符为 _sum
        /// 第二个字符为 _sum AS(SELECT SUM(TotalAmount) AS Total FROM _data)
        /// </summary>
        public Dictionary<string, string> ComputeCondition { get; } = new Dictionary<string, string>();
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 启用分页功能，默认False
        /// </summary>
        public bool EnablePaging { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 当前显示记录数
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 查询指定记录数
        /// </summary>
        public int Limit { get; set; }

        #region 条件

        /// <summary>
        /// 添加对象参数 例如： new { Id = 1 }
        /// Add by Jason.Song on 2018/06/25
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="relationType"></param>
        public void AddObjectParameters(object obj, RelationType relationType = RelationType.And)
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                var value = property.GetValue(obj);
                if (value != null)
                    AddParameters(property.Name, property.GetValue(obj), relationType);
            }
        }

        /// <summary>
        /// 添加 AND 参数
        /// Add by Jason.Song on 2018/06/25
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddAndParameters(string name, object value)
        {
            AddParameters(name, value);
        }

        /// <summary>
        /// 添加 OR 参数
        /// Add by Jason.Song on 2018/06/25
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddOrParameters(string name, object value)
        {
            AddParameters(name, value, RelationType.Or);
        }

        /// <summary>
        /// 添加Like参数
        /// Add by Jason.Song on 2018/06/25
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddLikeParameters(string name, params string[] value)
        {
            AddParameters(name, value, RelationType.And, QueryType.Like);
        }

        /// <summary>
        /// 添加Like参数
        /// Add by Jason.Song on 2018/06/25
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="relationType"></param>
        public void AddLikeParameters(string name, object value, RelationType relationType = RelationType.And)
        {
            AddParameters(name, value, relationType, QueryType.Like);
        }

        /// <summary>
        /// 添加Between参数
        /// Add by Jason.Song on 2018/10/18
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddBetweenParameters(string name, params string[] value)
        {
            AddParameters(name, value, RelationType.And, QueryType.Between, RelationType.And);
        }

        /// <summary>
        /// 添加参数
        /// Add by Jason.Song on 2018/06/26
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="relationType"></param>
        /// <param name="queryType"></param>
        /// <param name="valueType"></param>
        public void AddParameters(string name, object value, RelationType relationType = RelationType.And, QueryType queryType = QueryType.Equal, RelationType valueType = RelationType.Or)
        {
            var key = name;
            var prefix = "";
            var cut = name.Split('_');
            if (cut.Length > 1)
            {
                prefix = cut[0] + ".";
                name = cut[1];
            }

            //设置表名
            if (name.Equals("TableName", StringComparison.OrdinalIgnoreCase))
            {
                TableName = value.ToString();
                return;
            }
            ////根据 当前页码 或 当前显示记录数 开启分页查询功能
            //if (name.Equals("PageIndex", StringComparison.OrdinalIgnoreCase) 
            //    || name.Equals("PageSize", StringComparison.OrdinalIgnoreCase)) {
            //    EnablePaging = true;
            //    return;
            //}
            //设置当前页码
            if (name.Equals("PageIndex", StringComparison.OrdinalIgnoreCase))
            {
                EnablePaging = true;
                PageIndex = (int)value;
                return;
            }
            //设置当前显示记录数
            if (name.Equals("PageSize", StringComparison.OrdinalIgnoreCase))
            {
                EnablePaging = true;
                PageSize = (int)value;
                return;
            }
            //查询指定记录数
            if (name.Equals("Limit", StringComparison.OrdinalIgnoreCase))
            {
                Limit = (int)value;
                return;
            }

            if (WhereParameters.TryGetValue(key, out var info))
            {
                if (value is IEnumerable<string> v)
                {
                    info.Value.AddRange(v);
                }
                else
                {
                    info.Value.Add(value);
                }
            }
            else
            {
                info = new WhereCondition { ColumnName = name, RelationType = relationType, QueryType = queryType, ValueType = valueType, Prefix = prefix };
                if (value is IEnumerable<string> v)
                {
                    info.Value.AddRange(v);
                }
                else
                {
                    info.Value.Add(value);
                }
                WhereParameters.Add(key, info);
            }
        }

        #endregion

        #region 排序

        /// <summary>
        /// 添加排序列名参数
        /// Add by Jason.Song on 2018/10/18
        /// </summary>
        /// <param name="name">待排序的列名</param>
        /// <param name="sort">排序类型</param>
        public void AddSortParameter(string name, SortType sort)
        {
            if (!SortParameters.ContainsKey(name))
            {
                SortParameters.Add(name, sort);
            }
        }

        #endregion

        #region 移除

        /// <summary>
        /// 移除参数
        /// Add by Jason.Song on 2018/10/24
        /// </summary>
        /// <param name="name"></param>
        public void RemoveParameters(string name)
        {
            if (WhereParameters.ContainsKey(name))
                WhereParameters.Remove(name);
        }

        #endregion

        #region 添加计算条件

        /// <summary> 
        /// 添加计算条件
        /// </summary>
        /// <param name="computeName">计算名称 _sum</param>
        /// <param name="computeCondition">计算条件 _sum AS(SELECT SUM(TotalAmount) AS Total FROM _data)</param>
        public void AddComputeParameter(string computeName, string computeCondition)
        {
            if (!ComputeCondition.ContainsKey(computeName))
            {
                ComputeCondition.Add(computeName, computeCondition);
            }
        }

        #endregion
    }

    /// <summary>
    /// 参数扩展
    /// Add by Jason.Song on 2018/06/26
    /// </summary>
    public static class ParametersExtended
    {
        /// <summary> 
        /// 生成 Where Sql 语句
        /// Add by Jason.Song on 2018/06/25
        /// </summary>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public static string BuildWhereSql(this SqlParameters sqlParameters)
        {
            if (sqlParameters == null) return "";
            var whereSql = new StringBuilder();
            foreach (var info in sqlParameters.WhereParameters)
            {
                //忽略 PageIndex 当前页数 和 PageSize 显示记录数
                if (info.Value.ColumnName.Equals("PageIndex", StringComparison.OrdinalIgnoreCase)
                    || info.Value.ColumnName.Equals("PageSize", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var spliceSql = new StringBuilder();
                var iCount = info.Value.Value.Count;
                for (var i = 1; i <= iCount; i++)
                {
                    var queryType = info.Value.QueryType == QueryType.Equal
                        ? "="
                        : info.Value.QueryType.ToString().ToUpper();

                    var num = iCount > 1 ? i.ToString() : "";
                    var value = (string.IsNullOrWhiteSpace(info.Value.Prefix) ? "" :
                        Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(info.Value.Prefix.Replace(".", ""))) +
                        info.Value.ColumnName;
                    if (info.Value.QueryType == QueryType.Between && i > 1)
                    {
                        spliceSql.AppendFormat("@{0}{1} {2} ", value, num, info.Value.ValueType.ToString().ToUpper());
                    }
                    if (info.Value.QueryType == QueryType.In)
                    {
                        spliceSql.AppendFormat("{0} {1} @{2} {3} ", info.Value.ColumnName, info.Value.QueryType.ToString().ToUpper(), value, info.Value.ValueType.ToString().ToUpper());
                        break;
                    }
                    else
                    {
                        spliceSql.AppendFormat("{0}[{1}] {2} @{3}{4} {5} ", info.Value.Prefix, info.Value.ColumnName, queryType, value, num, info.Value.ValueType.ToString().ToUpper());
                    }
                }
                spliceSql.Remove(spliceSql.Length - 4, 4);
                if (info.Value.Value.Count > 1 && info.Value.QueryType != QueryType.In)
                {
                    spliceSql.Insert(0, "( ").Append(" ) ");
                }
                whereSql.AppendFormat("{0} {1} ", info.Value.RelationType.ToString().ToUpper(), spliceSql);
            }
            if (whereSql.Length > 0)
            {
                whereSql.Remove(0, 3).Insert(0, " WHERE ");
            }
            return whereSql.ToString();
        }

        /// <summary> 
        /// 生成 排序语句
        /// Add by Jason.Song on 2018/10/18
        /// </summary>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public static string BuildSortSql(this SqlParameters sqlParameters)
        {
            if (sqlParameters == null) return "";
            var sortSql = new StringBuilder();
            foreach (var pair in sqlParameters.SortParameters)
            {
                sortSql.AppendFormat("{0} {1}, ", pair.Key, pair.Value.ToString().ToUpper());
            }

            if (sortSql.Length > 0)
            {
                sortSql.Remove(sortSql.Length - 2, 1).Insert(0, " ORDER BY ");
            }

            return sortSql.ToString();
        }

        /// <summary>
        /// 生成 参数和对应的值
        /// Add by Jason.Song on 2018/06/25
        /// </summary>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public static DynamicParameters BuildParameters(this SqlParameters sqlParameters)
        {
            if (sqlParameters == null) return null;
            var parameters = new DynamicParameters();
            foreach (var info in sqlParameters.WhereParameters)
            {
                if (info.Value.QueryType == QueryType.In)
                {
                    var name = (string.IsNullOrWhiteSpace(info.Value.Prefix) ? "" :
                   Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(info.Value.Prefix.Replace(".", ""))) +
                    info.Value.ColumnName;
                    parameters.Add(name, info.Value.Value.ToArray());
                }
                else
                {
                    var i = 1;
                    foreach (var o in info.Value.Value)
                    {
                        var num = info.Value.Value.Count > 1 ? i.ToString() : "";
                        //var name = $"{info.Value.Prefix + info.Value.ColumnName}{num}";
                        var name = (string.IsNullOrWhiteSpace(info.Value.Prefix) ? "" :
                                        Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(info.Value.Prefix.Replace(".", ""))) +
                                    info.Value.ColumnName + num;
                        var value = info.Value.QueryType == QueryType.Like ? $"%{o.ToString().Replace("'", "''")}%" : o;
                        parameters.Add(name, value);
                        i++;
                    }
                }
            }
            return parameters;
        }

        /// <summary>
        ///
        /// _sum AS(SELECT SUM(TotalAmount) AS Total FROM _data) 
        /// </summary>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public static Tuple<string, string> BuildComputeCondition(this SqlParameters sqlParameters)
        {
            var query = new StringBuilder();
            var apply = new StringBuilder();
            foreach (var pair in sqlParameters.ComputeCondition)
            {
                query.AppendFormat(",{0} ", pair.Value);
                apply.AppendFormat("CROSS APPLY {0} ", pair.Key);
            }
            return new Tuple<string, string>(query.ToString(), apply.ToString());
        }



    }
}
