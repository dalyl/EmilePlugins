using EmilePlugins.Mvc.Module.PDFJS.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using EmilePlugins.Mvc.ModuleActivator;

namespace EmilePlugins.Mvc.Module.PDFJS
{

    public class Resource
    {

        public readonly static Resource Setting = new Resource();

        internal string ModuleName { get; set; } = "pdf";

        public RouteCollection Routes { get; }


        private readonly string[] Stylesheets =  {
           "viewer.css"
        };

        private readonly string[] Javascripts =  {
            "pdf.js",
            "debugger.js",
            "viewer.js",
        };

        Resource()
        {
            Routes = new RouteCollection();

            Routes.AddRazorPage("/", x => new Viewer());
            Routes.AddRazorPage("/View", x => new Viewer());
            Routes.AddRazorPage("/ViewStream", x => new ViewStream());

            var assembly = GetExecutingAssembly();
            var reses = assembly.GetManifestResourceNames();
            var Register = new RegisterDispather(Routes, assembly);

            #region Embedded static content

            Register.AddCombinedResource($"/css[0-9]+",  StaticResource.Define.Css, () => GetContentFolderNamespace("css"), Stylesheets);
            Register.AddEmbeddedResource($"/js/compatibility", StaticResource.Define.Javascript, () => GetContentResourceName("scripts", "compatibility.js"));
            Register.AddEmbeddedResource($"/js/l10n", StaticResource.Define.Javascript, () => GetContentResourceName("scripts", "l10n.js"));
            Register.AddEmbeddedResource($"/js/debugger", StaticResource.Define.Javascript, () => GetContentResourceName("scripts", "debugger.js"));
            Register.AddEmbeddedResource($"/js/viewer", StaticResource.Define.Javascript, () => GetContentResourceName("scripts", "viewer.js"));
            Register.AddEmbeddedResource($"/js/pdf[0-9]+", StaticResource.Define.Javascript, () => GetContentResourceName("scripts", "pdf.js"));
            Register.AddEmbeddedResource($"/js/pdfworker", StaticResource.Define.Javascript, () => GetContentResourceName("scripts", "pdf.worker.js"));
            Register.AddEmbeddedResource($"/js/jquery", StaticResource.Define.Javascript, () => GetContentResourceName("scripts", "jquery-1.10.2.min.js"));
            Register.AddSamePathTypeResource($"/js/(.)+/map", StaticResource.Define.Javascript, () => GetContentFolderNamespace("scripts"));
            Register.AddSamePathTypeResource($"/cmaps/(.)+/bcmap", StaticResource.Define.Cmap,  () => GetContentFolderNamespace("cmaps"));
            Register.AddSamePathTypeResource($"/images/(.)+/png", StaticResource.Define.ImagePng, () => GetContentFolderNamespace("images"));
            Register.AddSamePathTypeResource($"/images/(.)+/jpg", StaticResource.Define.ImageJpg, () => GetContentFolderNamespace("images"));
            Register.AddSamePathTypeResource($"/images/(.)+/gif", StaticResource.Define.ImageGif, () => GetContentFolderNamespace("images"));
            Register.AddSamePathTypeResource($"/locale/(.)+", StaticResource.Define.Locale, () => GetContentFolderNamespace("locale"));

            #endregion

            #region Razor pages and commands

            #endregion
        }


        internal static string GetResourceName(string ResourceName, string contentFolder)
        {
            var Namespace = GetContentFolderNamespace(contentFolder);
            return ResourceName.Replace($"{Namespace}.", "").Replace(".", "/");
        }

        internal static string GetContentFolderNamespace(string contentFolder)
        {
            return $"{typeof(Resource).Namespace}.Content.{contentFolder}";
        }

        internal static string GetContentResourceName(string contentFolder, string resourceName)
        {
            return $"{GetContentFolderNamespace(contentFolder)}.{resourceName}";
        }

        internal static Assembly GetExecutingAssembly()
        {
            return typeof(Resource).GetTypeInfo().Assembly;
        }
    }
}
