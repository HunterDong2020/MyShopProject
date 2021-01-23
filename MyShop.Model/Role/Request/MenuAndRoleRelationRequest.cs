using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Role.Request
{
    public class MenuAndRoleRelationRequest
    {
        public string Id { get; set; }

        public string MenuId { get; set; }

        public string RoleId { get; set; }
    }
}
