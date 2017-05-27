using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Common.DynamicAssembly
{
    /// <summary>
    /// 设置接口实现类自定义标注属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public class CategoryInfoAttribute : Attribute
    {
        public string Category { get; set; }

        public string Describtion { get; set; }

        /// <summary>
        /// 设置实现类自定义标注属性
        /// </summary>
        /// <param name="category"></param>
        /// <param name="describtion"></param>
        public CategoryInfoAttribute(string category, string describtion)
        {
            this.Category = category;
            this.Describtion = describtion;
        }
    }
}
