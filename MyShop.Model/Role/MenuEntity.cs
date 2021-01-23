using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Role
{
    public class MenuEntity
    {
        public string Id { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }
        /// <summary>
        /// 菜单地址
        /// </summary>
        public string MenuUrl { get; set; }
        /// <summary>
        /// 菜单图标样式
        /// </summary>
        public string MenuFontCss { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int MenuSort { get; set; }
        /// <summary>
        /// 父菜单
        /// </summary>
        public string ParentMenuId { get; set; }
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

        private string _parentName = "";
        public string ParentName { get { return _parentName; } set { _parentName = value; } }

        public bool NoCheck { get; set; }

        public string MenuId { get; set; }
    }
}
