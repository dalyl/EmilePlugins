using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Mvc.ModuleActivator
{
    public abstract class DashboardRequest
    {
        public abstract string Method { get; }
        public abstract string Path { get; }
        public abstract string PathBase { get; }

        public abstract string LocalIpAddress { get; }
        public abstract string RemoteIpAddress { get; }

        public abstract string GetQuery(string key);
        public abstract Task<IList<string>> GetFormValuesAsync(string key);
    }
}
