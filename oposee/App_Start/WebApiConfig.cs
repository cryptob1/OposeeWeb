using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using oposee.Library.API;
using System.Web.Configuration;
using oposee.Controllers.API;
namespace oposee
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();
            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            config.Filters.Add(new BasicAuthenticationAttribute());

            if (Convert.ToBoolean(WebConfigurationManager.AppSettings["APILogger"].ToString()) == true)
            {
                config.MessageHandlers.Add(new LoggingHandler());
            }

            //API Defalte Compression
            if (Convert.ToBoolean(WebConfigurationManager.AppSettings["APIDefalteCompression"].ToString()) == true)
            {
                config.Filters.Add(new DeflateCompressionAttribute());
            }
        }
    }
}
