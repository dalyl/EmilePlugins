using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Annotations.WebPlugins.Mvc.ModuleActivator;

namespace EmilePlugins.Mvc.ModuleActivator
{
    public class RouteCollection
    {
        private readonly List<Tuple<string, IDashboardDispatcher>> _dispatchers = new List<Tuple<string, IDashboardDispatcher>>();

        public void Add([NotNull] string pathTemplate, [NotNull] IDashboardDispatcher dispatcher)
        {
            if (pathTemplate == null) throw new ArgumentNullException(nameof(pathTemplate));
            if (dispatcher == null) throw new ArgumentNullException(nameof(dispatcher));

            _dispatchers.Add(new Tuple<string, IDashboardDispatcher>(pathTemplate, dispatcher));
        }

        public void Add(Tuple<string, IDashboardDispatcher> KeyDispatcher)
        {
            if (KeyDispatcher == null) throw new ArgumentNullException(nameof(KeyDispatcher));
            if (KeyDispatcher.Item1 == null) throw new ArgumentNullException(nameof(KeyDispatcher.Item1));
            if (KeyDispatcher.Item2 == null) throw new ArgumentNullException(nameof(KeyDispatcher.Item2));
            _dispatchers.Add(new Tuple<string, IDashboardDispatcher>(KeyDispatcher.Item1, KeyDispatcher.Item2));
        }

        public Tuple<IDashboardDispatcher, Match> FindDispatcher(string path)
        {
            if (path.Length == 0) path = "/";

            foreach (var dispatcher in _dispatchers)
            {
                var pattern = dispatcher.Item1;

                if (!pattern.StartsWith("^", StringComparison.OrdinalIgnoreCase))
                    pattern = "^" + pattern;

                if (!pattern.EndsWith("$", StringComparison.OrdinalIgnoreCase))
                    pattern += "$";

                var match = Regex.Match(
                    path,
                    pattern,
                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);

                if (match.Success)
                {
                    return new Tuple<IDashboardDispatcher, Match>(dispatcher.Item2, match);
                }
            }

            return null;
        }
    }

}
