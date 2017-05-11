using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annotations.WebPlugins.Mvc.ModuleActivator;

namespace EmilePlugins.Mvc.ModuleActivator
{
    public class UrlHelper
    {
#if NETFULL
        private readonly Microsoft.Owin.OwinContext _owinContext;

        [Obsolete("Please use UrlHelper(DashboardContext) instead. Will be removed in 2.0.0.")]
        public UrlHelper([NotNull] IDictionary<string, object> owinEnvironment)
        {
            if (owinEnvironment == null) throw new ArgumentNullException(nameof(owinEnvironment));
            _owinContext = new Microsoft.Owin.OwinContext(owinEnvironment);
        }
#endif

        private readonly DashboardContext _context;

        public UrlHelper([NotNull] DashboardContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _context = context;
        }

        public string To(string relativePath)
        {
            return
#if NETFULL
                _owinContext?.Request.PathBase.Value ??
#endif
                _context.Request.PathBase
                + relativePath;
        }

        public string Home()
        {
            return To("/");
        }

        public string JobDetails(string jobId)
        {
            return To("/jobs/details/" + jobId);
        }

        public string LinkToQueues()
        {
            return To("/jobs/enqueued");
        }

        public string Queue(string queue)
        {
            return To("/jobs/enqueued/" + queue);
        }
    }

}
