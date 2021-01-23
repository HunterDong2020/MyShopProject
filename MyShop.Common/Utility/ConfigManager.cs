using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Common.Utility
{
    public class ConfigManager
    {
        private static IConfiguration configuration;

        public static IConfigurationSection GetSection(string name) =>
            configuration.GetSection(name);

        //public static string GetSectionValue(string name) =>
        //    GetSection(name).get_Value();

        public static void InitConfigurantion(IConfiguration config)
        {
            configuration = config;
        }

        public static IConfiguration Configuration
        {
            get
            {
                if (configuration == null)
                {
                    throw new ArgumentNullException();
                }
                return configuration;
            }
        }
    }
}
