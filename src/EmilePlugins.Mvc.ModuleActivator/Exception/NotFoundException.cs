using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Mvc.ModuleActivator
{

    public interface IWebException
    {
        int StutusCode { get; }

        Func<object> Redirect { get; }
    }

    public class NotFoundException : Exception, IWebException
    {
        public int StutusCode { get; } = 404;

        public Func<object> Redirect { get; } = null;

    }
}
