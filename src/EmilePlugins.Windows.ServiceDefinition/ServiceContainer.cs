using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Windows.ServiceDefinition
{
    public sealed class ServiceContainer : IDisposable
    {
        private ServiceContainer()
        {

        }

        public readonly static ServiceContainer Provider = new ServiceContainer();

        public List<IPluginService> Services = new List<IPluginService>();

        public IEnumerable<ITrayService> TrayServices
        {
            get
            {
                return from ser in Services.Where(s => s is ITrayService) select ser as ITrayService;
            }
        }

        public void Dispose()
        {
            Services.ForEach(it=> {
                if (it is IDisposable) (it as IDisposable).Dispose();
            });
        }
    }

}
