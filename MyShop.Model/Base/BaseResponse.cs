using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Base
{
    public class BaseResponse
    {
        public string Msg { get; set; }

        public int Code { get; set; }

        public bool IsSuccess { get; set; }
    }
}
