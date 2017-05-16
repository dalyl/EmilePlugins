using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc.Module.PDFJS.Demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Down()
        {
            var path = $@"{AppDomain.CurrentDomain.BaseDirectory}\Files\12-20170505-043723.pdf";
            //return File(path, "application/octet-stream", "043723.pdf");
            //byte[] data = File.ReadAllBytes(filePath);
            //MemoryStream ms = new MemoryStream(data);
            var stream = new MemoryStream();
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                int length = (int)fileStream.Length;
                byte[] data = new byte[length];
                fileStream.Position = 0;
                fileStream.Read(data, 0, length);
                stream.Write(data, 0, length);
            }
            return File(stream, "application/octet-stream", "043723.pdf");
        }

        public ActionResult Preview()
        {
            return View();
        }
    }
}