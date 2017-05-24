using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmilePlugins.Windows.ServiceHost.Models
{
    public class ScriptSubmitModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string Body { get; set; }
    }
}
