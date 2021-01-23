using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Common.Utility
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public static class AppSettingConfig
    {
        /// <summary>
        /// 加密解密key
        /// </summary>
        public static string EncryptKey = ConfigManager.Configuration["AppSetting:EncryptKey"];
        /// <summary>
        /// 超级管理员
        /// </summary>
        public static string SuperAdminAccount = ConfigManager.Configuration["AppSetting:SuperAdminAccount"];
    }
}
