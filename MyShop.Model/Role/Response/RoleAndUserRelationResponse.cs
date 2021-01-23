using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Role.Response
{
    public class RoleAndUserRelationResponse
    {
        public RoleAndUserRelationResponse()
        {
            count = 0;
            data = new List<RoleAndUserRelationEntity>();
        }

        public List<RoleAndUserRelationEntity> data { get; set; }

        public int count { get; set; }

        public int code { get; set; }

        public string msg { get; set; }
    }

    

}
