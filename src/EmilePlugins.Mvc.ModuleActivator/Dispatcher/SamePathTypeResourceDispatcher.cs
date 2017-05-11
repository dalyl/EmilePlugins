using Annotations.WebPlugins.Mvc.ModuleActivator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Mvc.ModuleActivator
{
    public class SamePathTypeResourceDispatcher : EmbeddedResourceDispatcher
    {
        private readonly Assembly _assembly;
        private readonly string _baseNamespace;
        private readonly string _fileType;
        private readonly List<string> _resourceNames;
        private readonly ResourceFilter _filter;

        public SamePathTypeResourceDispatcher([NotNull] string contentType, [NotNull] string fileType, [NotNull] Assembly assembly, string baseNamespace, [NotNull]  ResourceFilter filter , params string[] resourceNames) : base(contentType, assembly, null)
        {
            _assembly = assembly;
            _baseNamespace = baseNamespace;
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            if (fileType == null) throw new ArgumentNullException(nameof(fileType));
            _fileType = fileType;
            _filter = filter;
            _resourceNames = new List<string>();
            _resourceNames.AddRange(resourceNames);
        }

        public SamePathTypeResourceDispatcher([NotNull] string contentType, [NotNull] string fileType, [NotNull] Assembly assembly, string baseNamespace, IEnumerable<string> resourceNames, [NotNull]  ResourceFilter filter ) : base(contentType, assembly, null)
        {
            _assembly = assembly;
            _baseNamespace = baseNamespace;
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            if (fileType == null) throw new ArgumentNullException(nameof(fileType));
            _fileType = fileType;
            _filter = filter;
            _resourceNames = new List<string>();
            _resourceNames.AddRange(resourceNames);
        }

        public SamePathTypeResourceDispatcher([NotNull] string contentType, [NotNull] string fileType, [NotNull] Assembly assembly, string baseNamespace, [NotNull] ResourceFilter filter ) : base(contentType, assembly, null)
        {
            _assembly = assembly;
            _baseNamespace = baseNamespace;
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            if (fileType == null) throw new ArgumentNullException(nameof(fileType));
            _fileType = fileType;
            _filter = filter;
            _resourceNames = new List<string>();
            var _Resources = _assembly.GetManifestResourceNames();
            foreach (var res in _Resources)
            {
                if (res.Contains(_baseNamespace) && res.Contains($".{fileType}"))
                {
                    var name = res.Replace($"{_baseNamespace}.", "");
                    _resourceNames.Add(name);
                }
            }
        }

        protected override void WriteResponse(DashboardRequest request, DashboardResponse response)
        {
            var resourceName =  _filter(_resourceNames, request);
            WriteResource(response, _assembly, $"{_baseNamespace}.{resourceName}");
        }

      
    }


   

}
