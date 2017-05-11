using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Annotations.WebPlugins.Mvc.ModuleActivator;

namespace EmilePlugins.Mvc.ModuleActivator
{
    public static class RouteCollectionExtensions
    {
        public static void AddRazorPage(
            [NotNull] this RouteCollection routes,
            [NotNull] string pathTemplate,
            [NotNull] Func<Match, RazorPage> pageFunc)
        {
            if (routes == null) throw new ArgumentNullException(nameof(routes));
            if (pathTemplate == null) throw new ArgumentNullException(nameof(pathTemplate));
            if (pageFunc == null) throw new ArgumentNullException(nameof(pageFunc));

            routes.Add(pathTemplate, new RazorPageDispatcher(pageFunc));
        }
         

        public static void AddCommand(
            [NotNull] this RouteCollection routes,
            [NotNull] string pathTemplate,
            [NotNull] Func<DashboardContext, bool> command)
        {
            if (routes == null) throw new ArgumentNullException(nameof(routes));
            if (pathTemplate == null) throw new ArgumentNullException(nameof(pathTemplate));
            if (command == null) throw new ArgumentNullException(nameof(command));

            routes.Add(pathTemplate, new CommandDispatcher(command));
        }
         

        
    }

}
