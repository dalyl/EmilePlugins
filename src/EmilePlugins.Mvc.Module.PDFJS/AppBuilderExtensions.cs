using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Mvc.Module.PDFJS
{
    using Annotations.WebPlugins.Mvc.ModuleActivator;
    using EmilePlugins.Mvc.ModuleActivator.Owin;
    using global::Owin;
    using Microsoft.Owin.Infrastructure;
    using System.ComponentModel;
    using BuildFunc = Action<Func<
        IDictionary<string, object>,
        Func<
        Func<IDictionary<string, object>, Task>,
        Func<IDictionary<string, object>, Task>
        >>>;
    using EmilePlugins.Mvc.ModuleActivator;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UsePdfview([NotNull] this IAppBuilder builder)
        {
            return builder.UsePdfview($"/{Resource.Setting.ModuleName}");
        }
        
        public static IAppBuilder UsePdfview(
            [NotNull] this IAppBuilder builder,
            [NotNull] string pathMatch)
        {
            return builder.UsePdfview(pathMatch, new DashboardOptions());
        }
      
       
        public static IAppBuilder UsePdfview(
            [NotNull] this IAppBuilder builder,
            [NotNull] string pathMatch,
            [NotNull] DashboardOptions options
          )
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (pathMatch == null) throw new ArgumentNullException(nameof(pathMatch));
            if (options == null) throw new ArgumentNullException(nameof(options));

            SignatureConversions.AddConversions(builder);

            Resource.Setting.ModuleName = pathMatch.Trim('/').Replace("/", ".");
            builder.Map(pathMatch, subApp => subApp.UseOwin().UseModuleActivator(options, Resource.Setting.Routes));

            return builder;
        }

        private static BuildFunc UseOwin(this IAppBuilder builder)
        {
            return middleware => builder.Use(middleware(builder.Properties));
        }
    }
}
