using MyShop.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Role.Request
{
    public class AdminUserRequest : BaseRequest
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string PassWord { get; set; }

        public int IsDelete { get; set; }

        public List<string> IdList { get; set; }

        public string RoleId { get; set; }
    }
}
