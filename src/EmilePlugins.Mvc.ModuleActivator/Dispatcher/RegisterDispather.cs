using Annotations.WebPlugins.Mvc.ModuleActivator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Mvc.ModuleActivator
{

    public delegate string ResourceFilter(List<string> resources, DashboardRequest request);

    public delegate ResourceFilter FetchFilter();

    public class StaticResource
    {
        public struct Struct
        {
            public Struct(string style, string conentType, FetchFilter getFilter)
            {
                Style = style;
                ContentType = conentType;
                GetFilter = getFilter;
            }

            public string Style { get; set; }

            public string ContentType { get; set; }

            public FetchFilter GetFilter { get; set; }
        }

        public enum Define
        {
            Javascript,
            ScriptMap,
            Locale,
            Cmap,
            Css,
            ImagePng,
            ImageJpg,
            ImageGif
        }


        public readonly static Dictionary<Define, Struct> Defines = new Dictionary<Define, Struct>();


        public readonly static Dictionary<string, Struct> DefineExtensions = new Dictionary<string, Struct>();

        public static DispatcherFilterProvider DispatcherFilter { get; set; } = new DispatcherFilterProvider();

        static StaticResource()
        {
            var Javascript = new Struct("js", "application/javascript",  DispatcherFilter.OtherFilter);
            var ScriptMap = new Struct("map", "application/javascript",  DispatcherFilter.OtherFilter);
            var Locale = new Struct("properties", "application/javascript", DispatcherFilter.LocaleFilter);
            var Cmap = new Struct("bcmap", "text/css", DispatcherFilter.CommonFilter);
            var Css = new Struct("css", "text/css", DispatcherFilter.CommonFilter);
            var ImagePng = new Struct("png", "image/png", DispatcherFilter.ImageFilter);
            var ImageJpg = new Struct("jpg", "image/jpg", DispatcherFilter.ImageFilter);
            var ImageGif = new Struct("gif", "image/gif", DispatcherFilter.ImageFilter);

            Defines.Add(Define.Javascript, Javascript);
            Defines.Add(Define.ScriptMap, ScriptMap);
            Defines.Add(Define.Locale, Locale);
            Defines.Add(Define.Cmap, Cmap);
            Defines.Add(Define.Css, Css);
            Defines.Add(Define.ImagePng, ImagePng);
            Defines.Add(Define.ImageJpg, ImageJpg);
            Defines.Add(Define.ImageGif, ImageGif);
        }

    }

    public class DispatcherFilterProvider
    {
        public DefaultDispatcherFilter Provider = new DefaultDispatcherFilter();
        public FetchFilter OtherFilter => () => { return Provider.OtherFilter; };
        public FetchFilter CommonFilter => () => { return Provider.CommonFilter; };
        public FetchFilter ImageFilter => () => { return Provider.ImageFilter; };
        public FetchFilter LocaleFilter => () => { return Provider.LocaleFilter; };
    }

    public class DefaultDispatcherFilter
    {
       
        public virtual ResourceFilter OtherFilter
        {
            get
            {
                return (resourceNames, request) =>
                {
                    var name = request.Path.Substring(request.Path.LastIndexOf("/") + 1).ToLower();
                    return resourceNames.FirstOrDefault(s => s.ToLower() == name);
                };
            }
        }

        public virtual ResourceFilter CommonFilter
        {
            get
            {
                return (resourceNames, request) =>
                {
                    var index = request.Path.LastIndexOf("/");
                    var requestFile = request.Path.Remove(index, 1).Insert(index, ".");
                    var requestFileName = requestFile.Substring(requestFile.LastIndexOf("/") + 1).ToLower();
                    return resourceNames.FirstOrDefault(s => s.ToLower()== requestFileName);
                };
            }
        }

        public virtual ResourceFilter ImageFilter
        {
            get
            {
                return CommonFilter;
            }
        }

        public virtual ResourceFilter LocaleFilter
        {
            get
            {
                return CommonFilter;
            }
        }

    }


    public class RegisterDispather
    {
        RouteCollection _Routes { get; set; }

        Assembly _Assembly { get; set; }

        string[] _Resources { get; set; }

        DefaultDispatcherFilter _dispatcherFilters { get; set; }

        public RegisterDispather([NotNull] RouteCollection routes,[NotNull] Assembly assembly, DefaultDispatcherFilter defaultDispatcherFilter = null)
        {
            _Routes = routes;
            _Assembly = assembly;
            _dispatcherFilters = defaultDispatcherFilter == null ? new DefaultDispatcherFilter() : defaultDispatcherFilter;
        }

        public void Add([NotNull] string pathTemplate, [NotNull] IDashboardDispatcher dispatcher)
        {
            if (_Routes == null) throw new ArgumentNullException("RouteCollection");
            _Routes.Add(pathTemplate, dispatcher);
        }

        public void AddEmbeddedResource([NotNull] string pathTemplate, [NotNull] StaticResource.Define style, [NotNull] Func<string> GetResourceName)
        {
            if (GetResourceName == null) throw new ArgumentNullException(nameof(GetResourceName));
            var resourceName = GetResourceName();
            AddEmbeddedResource(pathTemplate, style, resourceName);
        }

        public void AddEmbeddedResource([NotNull] string pathTemplate, [NotNull] StaticResource.Define style, string resourceName)
        {
            if (_Routes == null) throw new ArgumentNullException("RouteCollection");
            if (Enum.IsDefined(typeof(StaticResource.Define), style) == false) throw new ArgumentOutOfRangeException(nameof(style));
            var define = StaticResource.Defines[style];
            _Routes.Add(pathTemplate, new EmbeddedResourceDispatcher(define.ContentType, _Assembly, resourceName));
        }

        public void AddCombinedResource([NotNull] string pathTemplate, [NotNull] StaticResource.Define style, [NotNull] Func<string> GetNamespace,  params string[] resourceNames)
        {
            if (_Routes == null) throw new ArgumentNullException("RouteCollection");
            if (GetNamespace == null) throw new ArgumentNullException(nameof(GetNamespace));
            if (Enum.IsDefined(typeof(StaticResource.Define), style) == false) throw new ArgumentOutOfRangeException(nameof(style));
            var define = StaticResource.Defines[style];
            var baseNameSpace = GetNamespace();
            _Routes.Add(pathTemplate, new CombinedResourceDispatcher(define.ContentType, _Assembly, baseNameSpace, resourceNames));
        }

        public void AddSamePathTypeResource([NotNull] string pathTemplate, [NotNull] StaticResource.Define style, [NotNull] Func<string> GetNamespace, ResourceFilter filter = null)
        {
            if(Enum.IsDefined(typeof(StaticResource.Define), style)==false) throw new ArgumentOutOfRangeException(nameof(style));
            var define = StaticResource.Defines[style];
            if (_Resources == null) _Resources = _Assembly.GetManifestResourceNames();
            var baseNameSpace = GetNamespace();
            var resourceNames = RemoveNameSpace(_Resources, baseNameSpace,define.Style);
            _Routes.Add(pathTemplate, new SamePathTypeResourceDispatcher(define.ContentType, define.Style, _Assembly, baseNameSpace, resourceNames, filter == null ? define.GetFilter() : filter));
        }

        IEnumerable<string> RemoveNameSpace(IEnumerable<string> resourceNames,string baseNameSpace,string style)
        {
            var list = new List<string>();
            var count = resourceNames.Count();
            var length = baseNameSpace.Length;
            for (var i = 0; i < count; i++)
            {
                var one = resourceNames.ElementAt(i);
                if (one.Contains(baseNameSpace) == false) continue;
                if (one.EndsWith($".{style}") == false) continue;
                list.Add(one.Remove(0, length + 1));
            }
            return list;
        }
    }
}
