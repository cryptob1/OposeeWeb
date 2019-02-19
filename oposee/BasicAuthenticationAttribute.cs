using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace oposee.Library.API
{
    public class BasicAuthenticationAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
               || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            if (actionContext.Request.Headers.Authorization == null)
            {
               
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                actionContext.Response.Content = new StringContent(OposeeLibrary.API.JsonResponse.GetResponseString(OposeeLibrary.API.ResponseCode.Unauthorized), Encoding.UTF8, "application/json");
            }
            else
            {
                
                string authToken = actionContext.Request.Headers.Authorization.Parameter;
                string decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(authToken));

                string username = decodedToken.Substring(0, decodedToken.IndexOf(":"));
                string password = decodedToken.Substring(decodedToken.IndexOf(":") + 1);

                if (username != ConfigurationManager.AppSettings["ApiUsername"] || password != ConfigurationManager.AppSettings["ApiPassword"])
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    actionContext.Response.Content = new StringContent(OposeeLibrary.API.JsonResponse.GetResponseString(OposeeLibrary.API.ResponseCode.Unauthorized), Encoding.UTF8, "application/json");
                }
            }
        }
    }
}