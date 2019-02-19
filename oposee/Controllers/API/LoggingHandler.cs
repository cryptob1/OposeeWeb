using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using OposeeLibrary.Utilities;
using System.Web.Script.Serialization;
using static OposeeLibrary.Utilities.LogHelper;

namespace oposee.Controllers.API
{
    public class LoggingHandler : System.Net.Http.DelegatingHandler
    {
        protected override System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            //LogRequest(request);

            return base.SendAsync(request, cancellationToken).ContinueWith(task =>
            {


                var response = task.Result;

                LogResponse(response);

                return response;
            });

        }

        static string BytesToStringConverted(byte[] bytes)
        {
            using (var stream = new System.IO.MemoryStream(bytes))
            {
                using (var streamReader = new System.IO.StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        

        static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (System.IO.Compression.GZipStream stream = new System.IO.Compression.GZipStream(new System.IO.MemoryStream(gzip),
                System.IO.Compression.CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[gzip.Length];
                using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, gzip.Length);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }



        private void LogRequest(System.Net.Http.HttpRequestMessage request)
        {
            if (Convert.ToBoolean(System.Web.Configuration.WebConfigurationManager.AppSettings["APILoggerRequest"].ToString()) == true)
            {
                try
                {
                    
                    if (!request.Content.IsMimeMultipartContent())
                    {
                        (request.Content ?? new StringContent("")).ReadAsStringAsync().ContinueWith(x =>
                        {
                           
                            var requestMessage = request.Content.ReadAsByteArrayAsync();

                            if (request.Method.ToString() == "GET")
                            {
                                LogHelper.CreateAPILog(""
                                                        + request.Method + " "
                                                        + request.GetCorrelationId() + " "
                                                        + " REQUEST: " + request.RequestUri
                                                        , " " + request.RequestUri.ToString()
                                                        , BytesToStringConverted(requestMessage.Result)
                                                        , ErrorType.APILog);
                            }
                            else
                            {
                                try
                                {
                                    LogHelper.CreateAPILog(""
                                                           + request.Method + " "
                                                           + request.GetCorrelationId() + " "
                                                           + " REQUEST: " + request.RequestUri
                                        , " " + request.RequestUri.ToString()
                                        , JObject.Parse(BytesToStringConverted(requestMessage.Result)).ToString()
                                        , ErrorType.APILog);
                                }
                                catch (Exception ex)
                                {
                                    var a = BytesToStringConverted(requestMessage.Result).ToString();
                                    var dict = HttpUtility.ParseQueryString(a);
                                    var json = new JavaScriptSerializer().Serialize(
                                                        dict.AllKeys.ToDictionary(k => k, k => dict[k])
                                               );

                                    LogHelper.CreateAPILog(""
                                                        + request.Method + " "
                                                        + request.GetCorrelationId() + " "
                                                        + " REQUEST: " + request.RequestUri
                                                        , request.RequestUri.ToString()
                                                        , JObject.Parse(json).ToString()
                                                        , ErrorType.APILog);
                                }

                            }
                        });
                    }
                    //});

                }
                catch (Exception ex)
                {
                    LogHelper.CreateLog(ex);
                }
            }
        }

        private void LogResponse(HttpResponseMessage response)
        {
            if (Convert.ToBoolean(WebConfigurationManager.AppSettings["APILoggerResponse"].ToString()) == true)
            {
                try
                {
                   
                    if (!response.Content.IsMimeMultipartContent())
                    {

                        var request = response.RequestMessage;
                        (response.Content ?? new StringContent("")).ReadAsStringAsync().ContinueWith(x =>
                        {
                            
                            dynamic responseMessage;

                            if (response.IsSuccessStatusCode)
                                responseMessage = response.Content.ReadAsByteArrayAsync();
                            else
                                responseMessage = Encoding.UTF8.GetBytes(response.ReasonPhrase);

                          

                            if (Convert.ToBoolean(WebConfigurationManager.AppSettings["APILogger"].ToString()) == true)
                            {

                                LogHelper.CreateAPILog(""
                                                      + request.Method + " "
                                                      + request.GetCorrelationId() + " "
                                                      + " **RESPONSE**: " + request.RequestUri
                                                      , " " + request.RequestUri.ToString()
                                                       , JObject.Parse(BytesToStringConverted(Decompress(responseMessage.Result))).ToString()
                                                      , ErrorType.APILog);
                            }
                            else
                            {
                                LogHelper.CreateAPILog(""
                                                 + request.Method + " "
                                                 + request.GetCorrelationId() + " "
                                                 + " **RESPONSE**: " + request.RequestUri
                                                 , " " + request.RequestUri.ToString()
                                                  , JObject.Parse(BytesToStringConverted(responseMessage.Result)).ToString()
                                                 , ErrorType.APILog);
                            }

                        });
                    }
                    
                }
                catch (Exception ex)
                {
                    LogHelper.CreateLog(ex);
                }
            }
        }

        private string Username(HttpRequestMessage request)
        {
            var values = new List<string>().AsEnumerable();
            if (request.Headers.TryGetValues("my-custom-header-for-current-user", out values) == false) return "<anonymous>";

            return values.First();
        }
    }
}