using Microsoft.CSharp;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Common.DynamicAssembly
{
    public static class DynamicServiceExtensions
    {
        public static void AddDynamicService(this IServiceCollection services)
        {
            services.AddTransient<DynamicService>();
        }
    }

    public class DynamicService 
    {

        static ConcurrentDictionary<Type, object> _DynamicServices = new ConcurrentDictionary<Type, object>();

        public TaskResult<string> CreateAssembly(string AssemblyName, string Code, string Path = "DynamicAssemblys")
        {
            var result = new TaskResult<string>();
            result.AddErrors(CompilerAssembly(AssemblyName, Code));
            return result.AddContent( $"{Path}\\{AssemblyName}.dll");
        }

        TaskResult CompilerAssembly(string AssemblyFullName, string SourceCode)
        {
            var result = new TaskResult();

            // 1.Create a new CSharpCodePrivoder instance  
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();
          
            // 2.Sets the runtime compiling parameters by crating a new CompilerParameters instance  
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");

            // objCompilerParameters.GenerateInMemory = true;
            // Load the resulting assembly into memory  
            objCompilerParameters.GenerateInMemory = false;
            objCompilerParameters.OutputAssembly = $"{AssemblyFullName}.dll";

            // 3.CompilerResults: Complile the code snippet by calling a method from the provider  
            CompilerResults cr = objCSharpCodePrivoder.CompileAssemblyFromSource(objCompilerParameters, SourceCode);

            if (cr.Errors.HasErrors) return result.AddErrors<CompilerErrorCollection, CompilerError>(cr.Errors, (error) => $"Compiling Error Line: { error.Line } - { error.ErrorText }");
            return result;
        }

        public TaskResult<string> GenerateExecutable(string exeFullName, string sourceCode)
        {
            var result = new TaskResult<string>();

            // 1.Create a new CSharpCodePrivoder instance  
            var Compiler = new CSharpCodeProvider();

            // 2.Sets the runtime compiling parameters by crating a new CompilerParameters instance  
            CompilerParameters Parameters = new CompilerParameters();
            Parameters.ReferencedAssemblies.Add("System.dll");
            Parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            Parameters.GenerateExecutable = true;
            Parameters.GenerateInMemory = false;
            Parameters.MainClass = "DynamiclyProgram";
            Parameters.IncludeDebugInformation = true;

            Parameters.OutputAssembly  = exeFullName;

            // 3.CompilerResults: Complile the code snippet by calling a method from the provider  
            CompilerResults cr = Compiler.CompileAssemblyFromSource(Parameters, sourceCode);

            if (cr.Errors.HasErrors) return result.AddErrors<CompilerErrorCollection, CompilerError>(cr.Errors, (error) => $"Compiling Error Line: { error.Line } - { error.ErrorText }");
            return result.AddContent(cr.PathToAssembly);
        }

        public T Get<T>()   where T : class
        {
            object service = null;
            if(  _DynamicServices.TryGetValue(typeof(T),out service))
            {
                var manager = service as ServiceManager<T>;
                if (manager == null) return null;
                return manager.Proxy;
            }
            return null;
        }

        public T Register<T>(Func<string> findAssembly) where T : class
        {
            using (ServiceManager<T> manager = new ServiceManager<T>(findAssembly))
            {
                _DynamicServices.AddOrUpdate(typeof(T), manager, (key, oldValue) => manager);
                return  manager.Proxy;
            }
        }

        public void DisposService<T>() where T : class
        {
            object service = null;
            if (_DynamicServices.TryRemove(typeof(T), out service))
            {
                var manager = service as ServiceManager<T>;
                if (manager == null) return;
                 manager.Unload();
            }
        }

    }

    public class DynamicAppDomain
    {


        /// <summary>  
        /// Interface that can be run over the remote AppDomain boundary.  
        /// </summary>  
        public interface IRemoteInterface
        {
            object Invoke(string lcMethod, object[] Parameters);
        }

        /// <summary>  
        /// Factory class to create objects exposing IRemoteInterface  
        /// </summary>  
        public class RemoteLoaderFactory : MarshalByRefObject
        {
            private const BindingFlags bfi = BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;
            public RemoteLoaderFactory() { }
            public IRemoteInterface Create(string assemblyFile, string typeName, object[] constructArgs)
            {
                return (IRemoteInterface)Activator.CreateInstanceFrom(
                         assemblyFile, typeName, false, bfi, null, constructArgs,
                         null, null, null).Unwrap();
            }
        }


        TaskResult DynamiclyDomain(string source)
        {
            var AssemblyName = "DynamicalCode";
            var ClassName = "Dynamicly.HelloWorld";
            var ActionName = "HelloWorld";
            var AssemblyResult = DynamiclyCompiler(source, AssemblyName);
            if (AssemblyResult.Succeeded == false) return AssemblyResult;
            if (System.IO.File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\{AssemblyName}.dll") == false) return AssemblyResult.AddError($"Error:{AssemblyName} create fail! ");

            // 0. Create an addtional AppDomain  
            AppDomainSetup objSetup = new AppDomainSetup();
            objSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            AppDomain objAppDomain = AppDomain.CreateDomain("DynamiclyAppDomain", null, objSetup);

            // 1. Invoke the method by using Reflection  
            RemoteLoaderFactory factory = (RemoteLoaderFactory)objAppDomain.CreateInstance("RemoteAccess", "RemoteAccess.RemoteLoaderFactory").Unwrap();

            // with help of factory, create a real 'LiveClass' instance  
            object objObject = factory.Create($"{AssemblyName}.dll", "Dynamicly.HelloWorld", null);

            if (objObject == null) return new TaskResult("Error: Couldn't load class.");

            // *** Cast object to remote interface, avoid loading type info  
            IRemoteInterface objRemote = (IRemoteInterface)objObject;

            object[] objCodeParms = new object[1];
            objCodeParms[0] = "Allan.";
            string strResult = (string)objRemote.Invoke("GetTime", objCodeParms);

            //Dispose the objects and unload the generated DLLs.  
            objRemote = null;
            AppDomain.Unload(objAppDomain);
            System.IO.File.Delete("DynamicalCode.dll");
            return AssemblyResult;
        }

        TaskResult<string> DynamiclyCompiler(string strSourceCode, string AssemblyName)
        {
            var result = new TaskResult<string>();

            // 1.Create a new CSharpCodePrivoder instance  
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();

            // 2.Sets the runtime compiling parameters by crating a new CompilerParameters instance  
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            objCompilerParameters.GenerateExecutable  = true;
            objCompilerParameters.GenerateInMemory = false;
            objCompilerParameters.MainClass = "DynamiclyProgram";
            objCompilerParameters.IncludeDebugInformation = true;

            objCompilerParameters.OutputAssembly = $"{AssemblyName}.dll";


           // 3.CompilerResults: Complile the code snippet by calling a method from the provider  
           CompilerResults cr = objCSharpCodePrivoder.CompileAssemblyFromSource(objCompilerParameters, strSourceCode);

            if (cr.Errors.HasErrors) return result.AddErrors<CompilerErrorCollection, CompilerError>(cr.Errors, (error) => $"Compiling Error Line: { error.Line } - { error.ErrorText }");
            return result.AddContent(cr.PathToAssembly);
        }

        public static Assembly LoadAssemblyFromFile(string fileFullName)
        {
            byte[] b = System.IO.File.ReadAllBytes(fileFullName);
            Assembly asm = Assembly.Load(b);
            return asm;
        }


    }

}
