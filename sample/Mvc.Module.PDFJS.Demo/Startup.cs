using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mvc.Module.PDFJS.Demo.Startup))]
namespace Mvc.Module.PDFJS.Demo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
