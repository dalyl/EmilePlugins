using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Annotations.WebPlugins.Mvc.ModuleActivator;

namespace EmilePlugins.Mvc.ModuleActivator
{
    public class CombinedResourceDispatcher : EmbeddedResourceDispatcher
    {
        private readonly Assembly _assembly;
        private readonly string _baseNamespace;
        private readonly string[] _resourceNames;

        public CombinedResourceDispatcher(
            [NotNull] string contentType,
            [NotNull] Assembly assembly,
            string baseNamespace,
            params string[] resourceNames) : base(contentType, assembly, null)
        {
            _assembly = assembly;
            _baseNamespace = baseNamespace;
            _resourceNames = resourceNames;
        }

        protected override void WriteResponse(DashboardRequest request,DashboardResponse response)
        {
            foreach (var resourceName in _resourceNames)
            {
                WriteResource(response, $"{_baseNamespace}.{resourceName}");
            }
        }
    }

   
}
