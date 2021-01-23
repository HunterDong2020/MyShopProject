using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
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

namespace MyShop.Web
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
            RepositoryAddScoped(services);
            ServiceAddScoped(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitDbConnectionStringConfig();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
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
        /// 初始化链接字符串
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
