using OposeeLibrary.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OposeeLibrary.API
{
    public class JsonResponse
    {
        public static ApiResponse GetResponse(ResponseCode responseMessage, object data,string KeyName="Message")
        {
            ApiResponse response = new ApiResponse();
            response.Response = new Dictionary<string, object>();
            response.Response.Add("Code", Numerics.GetInt(responseMessage));

            switch (responseMessage)
            {
                case ResponseCode.Success:
                    response.Response.Add("Status", "Success");
                    break;
                case ResponseCode.Failure:
                    response.Response.Add("Status", data);
                    break;
                case ResponseCode.Unauthorized:
                    response.Response.Add("Status", "Unauthorized");
                    break;
                //case ResponseCode.BadRequest:
                //    response.Response.Add("Message", " The request is invalid.");
                //    response.Response.Add("ModelState", data);
                //    break;
                   
                case ResponseCode.Exception:
                    response.Response.Add("Status", "Exception: " + data);
                    break;
                case ResponseCode.Info:
                    response.Response.Add("Status", data);
                    break;
            }
         
            if (responseMessage == ResponseCode.Success && data != null)
            {
                response.Response.Add(KeyName, data);
            }
            return response;
        }

    public static string GetResponseString(ResponseCode responseMessage)
    {
        return JsonConvert.SerializeObject(GetResponse(ResponseCode.Unauthorized, null));
    }

    }
    public class ApiResponse
    {
        public Dictionary<string, object> Response { get; set; }
    }

    public enum ResponseCode
    {
        Success,
        Failure,
        Unauthorized,
        Exception,
        Info
    }
}
