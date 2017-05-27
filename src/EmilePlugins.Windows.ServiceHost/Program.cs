using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using EmilePlugins.Windows.ServiceDefinition;
using EmilePlugins.Windows.ServiceTray;
using System.Runtime.InteropServices;

namespace EmilePlugins.Windows.ServiceHost
{
    public class Program
    {
        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public static void Main(string[] args)
        {
            var Title = typeof(Program).Namespace;
            Console.Title = Title;
            IntPtr intptr = FindWindow("ConsoleWindowClass", Title);
            if (intptr != IntPtr.Zero)
            {
                ShowWindow(intptr, 0);//隐藏这个窗口
            }
            var path = $@"d:\ServiceHost\{DateTime.Today.ToString("yyyy-MM-dd")}";
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            using (StreamWriter sw = new StreamWriter($@"{path}\{DateTime.Now.ToString("HH-mm-ss")}.txt")) {
                Console.SetOut(sw);

                var tray = new TrayHost();
                ServiceContainer.Provider.Services.Add(WebHost.Instance);
                ServiceContainer.Provider.Services.Add(tray);
                WebHost.Instance.Start();
                tray.Start();
            }
        }
    }

    internal sealed class WebHost : ITrayService, IPluginService, IDisposable
    {
        private WebHost() { }

        public static readonly WebHost Instance = new WebHost();

        public IWebHost Host { get; set; }

        public void Start()
        {
            if (Host == null)
            {
                lock (this)
                {
                    if (Host == null)
                    {
                        Host = new WebHostBuilder()
                               .UseKestrel()
                               .UseContentRoot(Directory.GetCurrentDirectory())
                               .UseIISIntegration()
                               .UseStartup<Startup>()
                               .UseApplicationInsights()
                               .Build();
                        Task.Run(() => Host.Run());
                        State = ServiceStatus.Run;
                    }
                }
            }
        }

        public void OnClick(ITrayHost Tary)
        {
            System.Diagnostics.Process.Start("http://localhost:5001/");
        }

        public string Name => "web 寄宿服务";

        public ServiceStatus State { get; private set; } = ServiceStatus.Load;

        public bool IsDraw { get; set; } = false;

        public string Image { get; } = string.Empty;

        public void Dispose()
        {
            if (Host != null)
            {
                lock (this)
                {
                    if (Host != null)
                    {
                        Host.Dispose();
                        State = ServiceStatus.Stop;
                    }
                }
            }
        }
    }

   
}
