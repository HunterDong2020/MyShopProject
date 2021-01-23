using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Role.Response
{
    public class AdminMenuResponse
    {
        public string MenuId { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string MenuName { get; set; }

        public string MenuUrl { get; set; }

        public List<MenuEntity> MenuList { get; set; }
    }
}
