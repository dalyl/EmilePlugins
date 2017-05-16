using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Annotations.WebPlugins.Mvc.ModuleActivator;
using System.IO;

namespace EmilePlugins.Mvc.ModuleActivator
{


    public class EmbeddedResourceDispatcher : IDashboardDispatcher
    {
        private readonly Assembly _assembly;
        private readonly string _contentType;
        private readonly string _resourceName;
        public EmbeddedResourceDispatcher(
            [NotNull] string contentType,
            [NotNull] Assembly assembly,
            string resourceName)
        {
            if (contentType == null) throw new ArgumentNullException(nameof(contentType));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            _assembly = assembly;
            _resourceName = resourceName;
            _contentType = contentType;
        }

        public Task Dispatch(DashboardContext context)
        {
            var contentType = _contentType;
            context.Response.ContentType = contentType;
            context.Response.SetExpire(DateTimeOffset.Now.AddYears(1));

            WriteResponse(context.Request,context.Response);

            return Task.FromResult(true);
        }


        protected virtual void WriteResponse(DashboardRequest request, DashboardResponse response)
        {
            WriteResource(response, _resourceName);
        }

        protected bool WriteResource(DashboardResponse response, string resourceName)
        {
            if (_assembly.GetManifestResourceInfo(resourceName) == null) return WriteNotFound(response);
            using (var inputStream = _assembly.GetManifestResourceStream(resourceName))
            {
                inputStream.CopyTo(response.Body);
            }
            return true;
        }

        public virtual bool WriteNotFound(DashboardResponse response)
        {
            var NotFoundRes = "Require resource not found in assembly.";
            response.ContentType = "text/html";
            response.SetExpire(DateTimeOffset.Now.AddHours(-1));
            var data= Encoding.UTF8.GetBytes(NotFoundRes);
            response.Body.Write(data, 0, data.Length);
            return false;
        }
    }
}
