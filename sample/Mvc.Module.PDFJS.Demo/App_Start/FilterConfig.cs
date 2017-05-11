using System.Web;
using System.Web.Mvc;

namespace Web.Pdf.Preview
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
