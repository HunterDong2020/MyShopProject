using MyShop.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Role.Request
{
    public class RoleRequest : BaseRequest
    {
        public string Id { get; set; }

        public string RoleName { get; set; }

        public List<string> IdList { get; set; }
    }
}
