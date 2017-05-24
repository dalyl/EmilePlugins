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

namespace EmilePlugins.Windows.ServiceHost.Controllers
{
    public class ScriptController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult>  Excute(ScriptSubmitModel Model)
        {
            var context = new ActionContext(this.HttpContext, this.RouteData, this.ControllerContext.ActionDescriptor);
               
            var html =await RenderView(context, $"Template/Common.cshtml", Model);

            return Content(html);
        }

        public async Task<string> RenderView(ActionContext context, string viewName, object model = null)
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
