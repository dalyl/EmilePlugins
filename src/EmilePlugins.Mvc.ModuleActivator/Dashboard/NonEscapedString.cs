using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Mvc.ModuleActivator
{
    public class NonEscapedString
    {
        private readonly string _value;

        public NonEscapedString(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
