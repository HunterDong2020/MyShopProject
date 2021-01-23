using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Role.Request
{
    public class AdminMenuRequest
    {
        public string MenuId { get; set; }
        public string MenuName { get; set; }

        public string ParentMenuId { get; set; }
    }
}
