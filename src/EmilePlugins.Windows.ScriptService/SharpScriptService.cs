using EmilePlugins.Windows.ServiceDefinition;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Windows.ScriptService
{
    public class SharpScriptService : IPluginService, IDisposable
    {

        public string Name => "C# 脚本服务";

        public ServiceStatus State { get; set; } = ServiceStatus.Define;

        // file type to register
        const string FileType = @"Directory\Background";

        // context menu name in the registry
        const string KeyName = "SharpScriptService";

        // context menu text
        const string MenuText = "Sharp Script";


        public SharpScriptService()
        {
            bool isRegistered = FileShellExtension.IsRegistered(FileType, KeyName, false);

            if (isRegistered)
            {
                return;
            }

            string menuCommand = " ";

            // register the context menu
            FileShellExtension.Register(FileType, KeyName, MenuText, menuCommand, false);

        }

        public void Dispose()
        {
            bool isRegistered = FileShellExtension.IsRegistered(FileType, KeyName, false);

            if (isRegistered)
            {
                // unregister the context menu
                FileShellExtension.Unregister(FileType, KeyName, false);
            }
        }
    }
}
