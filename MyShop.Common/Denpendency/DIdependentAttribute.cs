using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Common.Denpendency
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class DIdependentAttribute : Attribute
    {
        public DIdependentAttribute(DependentType dIType)
        {
            this.DIType = dIType;
        }
        public DIdependentAttribute()
        {
            DIType = DependentType.Scoped;
        }
        public DependentType DIType { get; }
    }

    /// <summary>
    /// Transient： 每一次GetService都会创建一个新的实例
    /// Scoped：  在同一个Scope内只初始化一个实例 ，可以理解为（ 每一个request级别只创建一个实例，同一个http request会在一个 scope内）
    /// Singleton ：整个应用程序生命周期以内只创建一个实例
    /// </summary>
    public enum DependentType
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        Singleton,
        /// <summary>
        /// Transient模式
        /// </summary>
        Transient,
        /// <summary>
        /// Scoped模式
        /// </summary>
        Scoped
    }

}
