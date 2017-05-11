using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Web.Pdf.Preview.Startup))]
namespace Web.Pdf.Preview
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
