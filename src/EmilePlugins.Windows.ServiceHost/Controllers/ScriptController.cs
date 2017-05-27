using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Encodings.Web;
using EmilePlugins.Windows.ServiceDefinition;
using EmilePlugins.Windows.ServiceHost.Models;
using EmilePlugins.Common.DynamicAssembly;

namespace EmilePlugins.Windows.ServiceHost.Controllers
{
    public class ScriptController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Excute(ScriptSubmitModel Model)
        {
            if (ModelState.IsValid == false) return Content("提交数据有异常");

            var context = new ActionContext(this.HttpContext, this.RouteData, this.ControllerContext.ActionDescriptor);
            var html = await RenderView(context, $"Template/Executable.cshtml", Model);
            if (string.IsNullOrEmpty(html)) return Content("提交代码转化失败");

            var services = context.HttpContext.RequestServices;
            var Dynamicer = services.GetRequiredService<DynamicService>();

            var Name = $@"{Model.Path}\ScriptProvider.exe";
            if (Directory.Exists(Name)) return Content("脚本已存在");

            var result = Dynamicer.GenerateExecutable(Name, html);
            if (result.Succeeded == false) Content(result.Errors.First());

            StartScript(result.Content);

            return Content(result.Content);
        }

        void StartScript(string cmd)
        {
            cmd = cmd.Replace(" ", string.Empty);
            var run = Task.Run(() =>
            {
                using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                {
                    try
                    {
                        p.StartInfo.FileName = cmd;
                        p.StartInfo.UseShellExecute = true;    //是否使用操作系统shell启动
                        p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                        p.Start();//启动程序
                        p.WaitForExit();//等待程序执行完退出进程
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex);
                    }
                }
            });
        }

        async Task<string> RenderView(ActionContext context, string viewName, object model = null)
        {
            this.ViewData.Model = model;
            var services = context.HttpContext.RequestServices;
            var Engine = services.GetRequiredService<ICompositeViewEngine>();
            var view = Engine.GetView("", viewName, true).View;
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                var vc = new ViewContext(context, view, this.ViewData, this.TempData, sw, new HtmlHelperOptions());
                await vc.View.RenderAsync(vc);
                return sw.ToString();
            }
        }
  


    }


  
}
