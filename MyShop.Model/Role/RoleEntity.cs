using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Role
{
    public class RoleEntity
    {
        public string Id { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 1：删除
        /// </summary>
        public int IsDelete { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public string CreateTimeStr { get { return CreateTime.ToString("yyyy-MM-dd HH:mm:ss"); } }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateUser { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
