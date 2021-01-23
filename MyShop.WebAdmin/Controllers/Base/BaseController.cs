using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyShop.Model.Role;

namespace MyShop.WebAdmin.Controllers.Base
{
    public class BaseController : Controller
    {
        public AdminUserEntity CurrentLoginUser
        {
            get
            {
                var principal = HttpContext.User;
                if (principal != null)
                {
                    return new AdminUserEntity()
                    {
                        UserName = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value,
                        Id = principal.Claims.FirstOrDefault(x => x.Type == "Id")?.Value ?? ""
                    };
                }
                return null;
            }
        }
    }
}
