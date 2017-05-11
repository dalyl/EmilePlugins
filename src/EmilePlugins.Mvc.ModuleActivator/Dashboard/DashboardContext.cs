using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Annotations.WebPlugins.Mvc.ModuleActivator;

namespace EmilePlugins.Mvc.ModuleActivator
{
    public abstract class DashboardContext
    {
        protected DashboardContext( [NotNull] DashboardOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            Options = options;
        }

        public DashboardOptions Options { get; }

        public Match UriMatch { get; set; }

        public DashboardRequest Request { get; protected set; }
        public DashboardResponse Response { get; protected set; }
    }
}
