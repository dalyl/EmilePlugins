using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Common.DynamicAssembly
{
    public class TransparentAgent : MarshalByRefObject
    {
        private const BindingFlags bfi = BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;

        public TransparentAgent() { }
   
        public T Create<T>(string assemblyPath, string typeName, object[] args)
        {
            string assemblyFile = AssemblyHelper.LoadAssemblyFile(assemblyPath, typeName);
            return (T)Activator.CreateInstanceFrom(assemblyFile, typeName, false, bfi, null, args, null, null).Unwrap();
        }
    }
}
