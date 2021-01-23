using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Role
{
    public class RoleAndUserRelationEntity
    {
        public string Id { get; set; }

        public string RoleId { get; set; }
        /// <summary>
        /// 用户主健
        /// </summary>
        public string UserMasterId { get; set; }

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


        public string RoleName { get; set; }

        public string UserName { get; set; }
        public string RealName { get; set; }

        public string MobilePhone { get; set; }
    }
}
