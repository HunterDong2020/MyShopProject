using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Role
{
    public class AdminUserEntity
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string PassWord { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string MobilePhone { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }    

        /// <summary>
        /// 1：删除
        /// </summary>
        public int IsDelete { get; set; }

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
