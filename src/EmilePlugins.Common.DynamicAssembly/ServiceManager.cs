using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmilePlugins.Common.DynamicAssembly
{
    public class ServiceManager<T> : IDisposable where T : class
    {
        private AppDomain ctorProxy = null;

        /// <summary>
        /// 应用程序运行域容器
        /// </summary>
        public AppDomain CotrProxy
        {
            get { return ctorProxy; }
        }

        private T proxy = default(T);

        public T Proxy
        {
            get
            {
                if (proxy == null)
                {
                    proxy = (T)InitProxy(AssemblyPlugs);
                }
                return proxy;
            }
        }

        public Func<string> FindPlugs { get; private set; }

        private string assemblyPlugs;

        /// <summary>
        /// 外挂插件程序集目录路径
        /// </summary>
        public string AssemblyPlugs
        {
            get
            {
                assemblyPlugs = FindPlugs == null ? string.Empty : FindPlugs();
                if (assemblyPlugs.Equals(""))
                {
                    assemblyPlugs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
                }
                if (!Directory.Exists(assemblyPlugs))
                {
                    Directory.CreateDirectory(assemblyPlugs);
                }
                return assemblyPlugs;
            }
            set { assemblyPlugs = value; }
        }

        public ServiceManager(Func<string> findPlugs=null )
        {
            if (proxy == null)
            {
                proxy = (T)InitProxy(AssemblyPlugs);
            }

            FindPlugs = findPlugs;
        }

        private T InitProxy(string assemblyPlugs)
        {
            try
            {
                //AppDomain.CurrentDomain.SetShadowCopyFiles();
                //Get and display the friendly name of the default AppDomain.
                //string callingDomainName = Thread.GetDomain().FriendlyName;

                //Get and display the full name of the EXE assembly.
                //string exeAssembly = Assembly.GetEntryAssembly().FullName;
                //Console.WriteLine(exeAssembly);

                AppDomainSetup ads = new AppDomainSetup();
                ads.ApplicationName = "Shadow";

                //应用程序根目录
                ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

                //子目录（相对形式）在AppDomainSetup中加入外部程序集的所在目录，多个目录用分号间隔
                ads.PrivateBinPath = assemblyPlugs;
                //设置缓存目录
                ads.CachePath = ads.ApplicationBase;
                //获取或设置指示影像复制是打开还是关闭
                ads.ShadowCopyFiles = "true";
                //获取或设置目录的名称，这些目录包含要影像复制的程序集
                ads.ShadowCopyDirectories = ads.ApplicationBase;

                ads.DisallowBindingRedirects = false;
                ads.DisallowCodeDownload = true;

                ads.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

                //Create evidence for the new application domain from evidence of
                Evidence adevidence = AppDomain.CurrentDomain.Evidence;

                // Create the second AppDomain.
                ctorProxy = AppDomain.CreateDomain($"AD {typeof(T).Name}", adevidence, ads);

                //Type.GetType("Kernel.TypeLibrary.MarshalByRefType").Assembly.FullName
                string assemblyName = Assembly.GetExecutingAssembly().GetName().FullName;
                //string assemblyName = typeof(MarshalByRefType).Assembly.FullName

                // Create an instance of MarshalByRefObject in the second AppDomain. 
                // A proxy to the object is returned.

                // Console.WriteLine("CtorProxy:" + Thread.GetDomain().FriendlyName);

                //TransparentFactory factory = (IObjcet)ctorProxy.CreateInstance("Kernel.TypeLibrary",  "Kernel.TypeLibrary.TransparentFactory").Unwrap();
                TransparentAgent factory = (TransparentAgent)ctorProxy.CreateInstanceAndUnwrap(assemblyName,typeof(TransparentAgent).FullName);

                Type meetType = typeof(T);
                string typeName = AssemblyHelper.CategoryInfo(meetType);

                object[] args = new object[0];
                string assemblyPath = ctorProxy.SetupInformation.PrivateBinPath;

                //IObjcet ilive = factory.Create(@"E:\Plug\Kernel.SimpleLibrary.dll", "Kernel.SimpleLibrary.PlugPut", args);
                T obj = factory.Create<T>(assemblyPath, typeName, args);

                return obj;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 卸载应用程序域
        /// </summary>
        public void Unload()
        {
            try
            {
                if (ctorProxy != null)
                {
                    AppDomain.Unload(ctorProxy);
                    ctorProxy = null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Dispose()
        {
            this.Unload();
        }
    }
}
