using Microsoft.Extensions.DependencyInjection;
using MyShop.Common.Denpendency;
using MyShop.Core.Role.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyShop.WebAdmin.Filter
{
    public static class DependentAddDataService
    {
        /// <summary>
        /// 自动注入扩展方法【通过特性声明】
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDataServiceDenpendency(this IServiceCollection services)
        {
            var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var referencedAssemblies = System.IO.Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom).ToArray();

            var dependentDLL = referencedAssemblies.Where(p => p.FullName.Contains("MyShop.Core") || p.FullName.Contains("MyShop.DataAccess")).ToList();
            //接口的程序集
            var interfaceTypes = dependentDLL.SelectMany(p => p.GetTypes()).Where(x => x.IsInterface).ToArray();

            dependentDLL.ForEach(p =>
            {
                //获取所有对象
                var dataClassList = p.GetTypes()
                //查找是否包含DI特性并且查看是否是抽象类
                .Where(a => a.GetCustomAttributes(true).Select(t => t.GetType()).Contains(typeof(DIdependentAttribute)) && !a.IsAbstract).ToList();

                //class的程序集
                var implementTypes = dataClassList.Where(x => x.IsClass).ToArray();

                foreach (var implementType in implementTypes)
                {
                    var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));
                    var Di = (DIdependentAttribute)implementType.GetCustomAttributes(true).FirstOrDefault(d => d.GetType() == typeof(DIdependentAttribute));
                    //class有接口，用接口注入
                    if (interfaceType != null)
                    {
                        switch (Di.DIType)
                        {
                            case DependentType.Singleton:
                                services.AddSingleton(interfaceType, implementType);
                                break;
                            case DependentType.Scoped:
                                services.AddScoped(interfaceType, implementType);
                                break;
                            default:
                                services.AddTransient(interfaceType, implementType);
                                break;
                        }
                    }
                    else //class没有接口，直接注入class
                    {
                        switch (Di.DIType)
                        {
                            case DependentType.Singleton:
                                services.AddSingleton(implementType);
                                break;
                            case DependentType.Scoped:
                                services.AddScoped(implementType);
                                break;
                            default:
                                services.AddTransient(implementType);
                                break;
                        }
                    }
                }

            });

            return services;
        }
    }
}
