using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Windows.ServiceDefinition
{
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
