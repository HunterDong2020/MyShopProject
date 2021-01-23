using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyShop.Common.Utility;
using MyShop.Core.Role;
using MyShop.Core.Role.IService;
using MyShop.DataAccess.Base;
using MyShop.DataAccess.Role;
using MyShop.DataAccess.Role.IRepository;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyShop.WebAdmin
{
    public class Startup
    {

        public IConfiguration _configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            ConfigManager.InitConfigurantion(builder.Build());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            //解决core序列化首字母默认小写的问题
            services.AddControllersWithViews().AddJsonOptions(p =>
            {
                p.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            //解决ViewBag的中文编码问题
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            //添加授权支持，并添加使用Cookie的方式，配置登录页面和没有权限时的跳转页面
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
                {
                    o.LoginPath = new PathString("/AdminLogin/Index");            //登录路径：这是当用户试图访问资源但未经过身份验证时，程序将会将请求重定向到这个相对路径。
                    o.AccessDeniedPath = new PathString("/AdminHome/Error");     //禁止访问路径：当用户试图访问资源时，但未通过该资源的任何授权策略，请求将被重定向到这个相对路径。
                    o.SlidingExpiration = true; //Cookie可以分为永久性的和临时性的。 临时性的是指只在当前浏览器进程里有效，浏览器一旦关闭就失效（被浏览器删除）。 永久性的是指Cookie指定了一个过期时间，在这个时间到达之前，此cookie一直有效（浏览器一直记录着此cookie的存在）。 slidingExpriation的作用是，指示浏览器把cookie作为永久性cookie存储，但是会自动更改过期时间，以使用户不会在登录后并一直活动，但是一段时间后却自动注销。也就是说，你10点登录了，服务器端设置的TimeOut为30分钟，如果slidingExpriation为false,那么10: 30以后，你就必须重新登录。如果为true的话，你10: 16分时打开了一个新页面，服务器就会通知浏览器，把过期时间修改为10: 46。
                });

            RepositoryAddScoped(services);
            ServiceAddScoped(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {  
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/AHome/Error");
            }
            InitDbConnectionStringConfig();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=AdminHome}/{action=Index}/{id?}");
            });
        }

        #region 拓展方法
        /// <summary>
        /// Repository注入
        /// </summary>
        /// <param name="services"></param>
        public void RepositoryAddScoped(IServiceCollection services)
        {
            services.AddTransient<IAdminUserRepository, AdminUserRepository>();
            services.AddTransient<IMenuAndRoleRelationRepository, MenuAndRoleRelationRepository>();
            services.AddTransient<IMenuRepository, MenuRepository>();
            services.AddTransient<IRoleAndUserRelationRepository, RoleAndUserRelationRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
        }
        /// <summary>
        /// Service注入
        /// </summary>
        /// <param name="services"></param>
        public void ServiceAddScoped(IServiceCollection services)
        {
            services.AddTransient<IAdminUserService, AdminUserService>();
            services.AddTransient<IMeuService, MenuService>();
            services.AddTransient<IRoleService, RoleService>();
        }

        /// <summary>
        /// 初始化数据库连接字符串
        /// </summary>
        private void InitDbConnectionStringConfig()
        {
            var dbConnectionStringConfig = new DbConnectionStringConfig();

            dbConnectionStringConfig.MyShopConnectionString = ConfigManager.Configuration["ConnectionStrings:MyShop"];
            DbConnectionStringConfig.InitDefault(dbConnectionStringConfig);
        }
        #endregion
    }
}
