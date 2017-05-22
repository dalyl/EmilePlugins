using EmilePlugins.Windows.ServiceDefinition;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;

namespace EmilePlugins.Windows.ServiceTray
{
    public class TrayHost : ITrayHost, IPluginService
    {
        public string Name => "托盘服务";

        public ServiceStatus State { get; private set; } = ServiceStatus.Load;

        public TrayHost()
        {

        }

        public void Start()
        {
            ApplicationContext applicationContext = new EmileTray(this);
            Application.Run(applicationContext);
            State = ServiceStatus.Run;
        }

        public void SystemBubble(string title, string message) { }
      
    }

    public class EmileTray: ApplicationContext,  IDisposable
    {
      
        public System.ComponentModel.Container Components { get; set; }

        public NotifyIcon NotifyIcon { get; set; }

        public TrayHost Host { get; set; }

        public EmileTray(TrayHost host)
        {
            Host = host;
            InitializeContext();
        }

        private void InitializeContext()
        {
            Components = new System.ComponentModel.Container();
            NotifyIcon = new NotifyIcon(Components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Resources.Resources.smile,
                Text = "Emile Service",
                Visible = true
            };
            Ignite();
            NotifyIcon.ContextMenuStrip.Opening += (obj, arg) => Ignite();
            NotifyIcon.ShowBalloonTip(3, "提示", "我开始运行了！ \n请右键查看功能或退出", ToolTipIcon.Info);
        }

        private void Ignite()
        {
            var services = ServiceContainer.Provider.TrayServices;
            var noDraws = services.Where(s => s.IsDraw == false);
            if (noDraws.Count() == 0) return;

            foreach (var one in services)
            {
                if (one.State == ServiceStatus.Run)
                {
                    NotifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem(one.Name,string.IsNullOrEmpty(one.Image) ?null: GetImageResource(GetResourceName(one.Image, "Images")), (obj,args)=>one.OnClick(Host), one.Name));
                    one.IsDraw = true;
                }
            }

            NotifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Quit", null, Exit, "退出"));
        }

        void Exit(object sender, EventArgs e)
        {
            ServiceContainer.Provider.Dispose();
            ExitThread();
        }

        internal static Image GetImageResource(string ResourceName, string contentFolder = "")
        {
            var image = GetResourceName(ResourceName, contentFolder);
            var assembly = GetExecutingAssembly();
            if (assembly.GetManifestResourceInfo(image) == null) return null;
            using (var stream = assembly.GetManifestResourceStream(image))
            {
                return Image.FromStream(stream);
            }
        }

        internal static Assembly GetExecutingAssembly()
        {
            return typeof(EmileTray).GetTypeInfo().Assembly;
        }

        internal static string GetResourceName(string ResourceName, string contentFolder)
        {
            var Namespace = GetContentFolderNamespace(contentFolder);
            return ResourceName.Replace($"{Namespace}.", "").Replace(".", "/").Replace("..",".");
        }

        internal static string GetContentFolderNamespace(string contentFolder)
        {
            return $"{typeof(EmileTray).Namespace}.Resources.{contentFolder}";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Components != null) { Components.Dispose(); }
        }

        protected override void ExitThreadCore()
        {
            NotifyIcon.Visible = false;
            base.ExitThreadCore();
        }
    }
}
