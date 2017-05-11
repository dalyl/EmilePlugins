using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annotations.WebPlugins.Mvc.ModuleActivator;

namespace EmilePlugins.Mvc.ModuleActivator
{
    public interface IDashboardDispatcher
    {
        Task Dispatch([NotNull] DashboardContext context);
    }



}
