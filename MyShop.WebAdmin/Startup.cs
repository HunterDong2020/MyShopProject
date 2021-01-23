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
            //���core���л�����ĸĬ��Сд������
            services.AddControllersWithViews().AddJsonOptions(p =>
            {
                p.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            //���ViewBag�����ı�������
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            //�����Ȩ֧�֣������ʹ��Cookie�ķ�ʽ�����õ�¼ҳ���û��Ȩ��ʱ����תҳ��
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
                {
                    o.LoginPath = new PathString("/AdminLogin/Index");            //��¼·�������ǵ��û���ͼ������Դ��δ���������֤ʱ�����򽫻Ὣ�����ض���������·����
                    o.AccessDeniedPath = new PathString("/AdminHome/Error");     //��ֹ����·�������û���ͼ������Դʱ����δͨ������Դ���κ���Ȩ���ԣ����󽫱��ض���������·����
                    o.SlidingExpiration = true; //Cookie���Է�Ϊ�����Եĺ���ʱ�Եġ� ��ʱ�Ե���ָֻ�ڵ�ǰ�������������Ч�������һ���رվ�ʧЧ���������ɾ������ �����Ե���ָCookieָ����һ������ʱ�䣬�����ʱ�䵽��֮ǰ����cookieһֱ��Ч�������һֱ��¼�Ŵ�cookie�Ĵ��ڣ��� slidingExpriation�������ǣ�ָʾ�������cookie��Ϊ������cookie�洢�����ǻ��Զ����Ĺ���ʱ�䣬��ʹ�û������ڵ�¼��һֱ�������һ��ʱ���ȴ�Զ�ע����Ҳ����˵����10���¼�ˣ������������õ�TimeOutΪ30���ӣ����slidingExpriationΪfalse,��ô10: 30�Ժ���ͱ������µ�¼�����Ϊtrue�Ļ�����10: 16��ʱ����һ����ҳ�棬�������ͻ�֪ͨ��������ѹ���ʱ���޸�Ϊ10: 46��
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

        #region ��չ����
        /// <summary>
        /// Repositoryע��
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
        /// Serviceע��
        /// </summary>
        /// <param name="services"></param>
        public void ServiceAddScoped(IServiceCollection services)
        {
            services.AddTransient<IAdminUserService, AdminUserService>();
            services.AddTransient<IMeuService, MenuService>();
            services.AddTransient<IRoleService, RoleService>();
        }

        /// <summary>
        /// ��ʼ�����ݿ������ַ���
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
