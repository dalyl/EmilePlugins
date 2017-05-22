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

    public enum ServiceStatus
    {
        Define,
        Load,
        Run,
        Stop,
    }

    public interface IPluginService
    {
        string Name { get; }

        ServiceStatus State { get; } 
    }

    public interface ITrayHost
    {

        void SystemBubble(string title,string message);

    }


    public interface ITrayService: IPluginService
    {

        void OnClick(ITrayHost Tary);

        bool IsDraw { get; set; }

        string Image { get;  }

    }
}
