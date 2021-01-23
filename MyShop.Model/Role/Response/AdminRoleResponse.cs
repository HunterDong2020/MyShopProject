using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Role.Response
{
    public class AdminRoleResponse
    {
        public AdminRoleResponse()
        {
            count = 0;
            data = new List<RoleEntity>();
        }

        public List<RoleEntity> data { get; set; }

        public int count { get; set; }

        public int code { get; set; }

        public string msg { get; set; }
    }
}
