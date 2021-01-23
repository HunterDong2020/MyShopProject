using MyShop.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Role.Request
{
    public class RoleAndUserRelationRequest : BaseRequest
    {
        public string RoleId { get; set; }

        public string UserName { get; set; }

    }
}
