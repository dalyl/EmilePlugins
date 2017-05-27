using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Common.DynamicAssembly
{
    public class AssemblyHelper
    {
        /// <summary>
        /// 获取泛型类中指定属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string CategoryInfo(Type meetType)
        {
            object[] attrList = meetType.GetCustomAttributes(typeof(CategoryInfoAttribute), false);
            if (attrList != null)
            {
                CategoryInfoAttribute categoryInfo = (CategoryInfoAttribute)attrList[0];
                return categoryInfo.Category;
            }
            return "";
        }

        public static string LoadAssemblyFile(string assemblyPlugs, string typeName)
        {
            string path = string.Empty;
            DirectoryInfo d = new DirectoryInfo(assemblyPlugs);
            foreach (FileInfo file in d.GetFiles("*.dll"))
            {
               // Assembly assembly = Assembly.LoadFile(file.FullName);
                Assembly assembly = LoadAssemblyFromFile(file.FullName);
                Type type = assembly.GetType(typeName, false);
                if (type != null)
                {
                    path = file.FullName;
                }
            }
            return path;
        }

        public static Assembly LoadAssemblyFromFile(string fileFullName)
        {
            byte[] b = System.IO.File.ReadAllBytes(fileFullName);
            Assembly asm = Assembly.Load(b);
            return asm;
        }

    }
}
