using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Mvc.ModuleActivator
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
   
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AppBuilderExtensions
    {

        public static IAppBuilder UseModuleActivator(
           [NotNull] this IAppBuilder builder,
           [NotNull] string module,
            [NotNull] RouteCollection routes)
        {
            return builder.UseModuleActivator(module, new DashboardOptions(), routes);
        }
     
        public static IAppBuilder UseModuleActivator(
            [NotNull] this IAppBuilder builder,
            [NotNull] string module,
            [NotNull] DashboardOptions options,
            [NotNull] RouteCollection routes
          )
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (module == null) throw new ArgumentNullException(nameof(module));
            if (options == null) throw new ArgumentNullException(nameof(options));

            SignatureConversions.AddConversions(builder);

            builder.Map(module, subApp => subApp.UseOwin().UseModuleActivator(options, routes));

            return builder;
        }

        private static BuildFunc UseOwin(this IAppBuilder builder)
        {
            return middleware => builder.Use(middleware(builder.Properties));
        }
    }
}
