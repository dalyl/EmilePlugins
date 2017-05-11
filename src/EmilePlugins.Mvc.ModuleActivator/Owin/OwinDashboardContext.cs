using Annotations.WebPlugins.Mvc.ModuleActivator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Mvc.ModuleActivator.Owin
{
    public sealed class OwinDashboardContext : DashboardContext
    {
        public OwinDashboardContext(
            [NotNull] DashboardOptions options,
            [NotNull] IDictionary<string, object> environment)
            : base(options)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));

            Environment = environment;
            Request = new OwinDashboardRequest(environment);
            Response = new OwinDashboardResponse(environment);
        }

        public IDictionary<string, object> Environment { get; }
    }
}
