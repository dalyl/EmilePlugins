using Annotations.WebPlugins.Mvc.ModuleActivator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace EmilePlugins.Mvc.ModuleActivator.Owin
{
    using MidFunc = Func<
       Func<IDictionary<string, object>, Task>,
       Func<IDictionary<string, object>, Task>
       >;
    using BuildFunc = Action<
        Func<
            IDictionary<string, object>,
            Func<
                Func<IDictionary<string, object>, Task>,
                Func<IDictionary<string, object>, Task>
        >>>;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class MiddlewareExtensions
    {

        public static BuildFunc UseModuleActivator(
            [NotNull] this BuildFunc builder,
            [NotNull] DashboardOptions options,
            [NotNull] RouteCollection routes)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (routes == null) throw new ArgumentNullException(nameof(routes));

            builder(_ => UseModuleActivator(options,  routes));

            return builder;
        }

        public static MidFunc UseModuleActivator(
            [NotNull] DashboardOptions options,
            [NotNull] RouteCollection routes)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (routes == null) throw new ArgumentNullException(nameof(routes));

            return
                next =>
                env =>
                {
                    var owinContext = new OwinContext(env);
                    var context = new OwinDashboardContext( options, env);

                    if (options.Authorization.Any(filter => !filter.Authorize(context)))
                    {
                        return Unauthorized(owinContext);
                    }

                    var findResult = routes.FindDispatcher(owinContext.Request.Path.Value);

                    if (findResult == null)
                    {
                        return next(env);
                    }

                    context.UriMatch = findResult.Item2;

                    return findResult.Item1.Dispatch(context);
                };
        }

        private static Task Unauthorized(IOwinContext owinContext)
        {
            var isAuthenticated = owinContext.Authentication?.User?.Identity?.IsAuthenticated;

            owinContext.Response.StatusCode = isAuthenticated == true
                ? (int)HttpStatusCode.Forbidden
                : (int)HttpStatusCode.Unauthorized;

            return Task.FromResult(0);
        }
    }
}