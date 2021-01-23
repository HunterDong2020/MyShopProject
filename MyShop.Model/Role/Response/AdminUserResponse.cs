using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Role.Response
{
    public class AdminUserResponse
    {

        public AdminUserResponse()
        {
            count = 0;
            data = new List<AdminUserEntity>();
        }

        public List<AdminUserEntity> data { get; set; }

        public int count { get; set; }

        public int code { get; set; }

        public string msg { get; set; }
    }
}
