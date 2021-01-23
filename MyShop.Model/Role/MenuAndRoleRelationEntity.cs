using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Role
{
    public class MenuAndRoleRelationEntity
    {
        public string Id { get; set; }

        public string MenuId { get; set; }

        public string RoleId { get; set; }
        /// <summary>
        /// 1：删除
        /// </summary>
        public int IsDelete { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
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
