using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Mvc.ModuleActivator
{
    internal class CommandDispatcher : IDashboardDispatcher
    {
        private readonly Func<DashboardContext, bool> _command;

        public CommandDispatcher(Func<DashboardContext, bool> command)
        {
            _command = command;
        }

#if NETFULL
        [Obsolete("Use the `CommandDispatcher(Func<DashboardContext, bool>)` ctor instead. Will be removed in 2.0.0.")]
        public CommandDispatcher(Func<RequestDispatcherContext, bool> command)
        {
            _command = context => command(RequestDispatcherContext.FromDashboardContext(context));
        }
#endif

        public Task Dispatch(DashboardContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (!"POST".Equals(request.Method, StringComparison.OrdinalIgnoreCase))
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return Task.FromResult(false);
            }

            if (_command(context))
            {
                response.StatusCode = (int)HttpStatusCode.NoContent;
            }
            else
            {
                response.StatusCode = 422;
            }

            return Task.FromResult(true);
        }
    }

}
