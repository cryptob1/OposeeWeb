using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OposeeLibrary.PushNotfication
{
    public class PushNotifications
    {
        public const string Success = "Success";
        public const string success = "success";
        public const string Failure = "Failure";
        public const string NoAccountExists = "NoAccountExists";
        protected string SetStringValue(string str)
        {
            return str.ToLower() == "null" ? string.Empty : str;
        }


        #region "New Code for iOS Notification"
        public string SendNotification_IOS(string DeviceToken, string _Message, string NotificationType)
        {
            String sResponseFromServer = String.Empty;
            try
            {
                if (!String.IsNullOrEmpty(DeviceToken))
                {
                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    string GoogleAppID = ConfigurationManager.AppSettings["GoogleAppIDIos"];
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));
                    tRequest.Headers.Add(string.Format("Sender: id={0}", ConfigurationManager.AppSettings["SENDER_ID"]));
                    tRequest.ContentType = "application/json";

                    var payload = new
                    {
                        to = DeviceToken,
                        priority = "high",
                        //content_available = true,
                        //type = NotificationType,
                        notification = new
                        {
                            body = _Message,
                            title = "Opozee App",
                            //badge = 1,
                            //type = NotificationType
                        },
                        data = new
                        {
                            //type = NotificationType
                        }
                    };

                    string postbody = JsonConvert.SerializeObject(payload).ToString();
                    Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                    tRequest.ContentLength = byteArray.Length;

                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                    {
                                        sResponseFromServer = tReader.ReadToEnd();
                                    }
                            }
                        }
                    }
                }
                return sResponseFromServer;
            }
            catch (Exception ex)
            {
                return ex.Message + "\n" + ex.StackTrace;
            }
        }
        #endregion

        public string SendNotification_Android(string DeviceToken, string DeviceMessage, string NotificationType, string questId)
        {
            String sResponseFromServer = String.Empty;
            try
            {
                if (!String.IsNullOrEmpty(DeviceToken))
                {
                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    string GoogleAppID = ConfigurationManager.AppSettings["GoogleAppID"];
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));
                    tRequest.Headers.Add(string.Format("Sender: id={0}", ConfigurationManager.AppSettings["SENDER_ID"]));
                    tRequest.ContentType = "application/json";

                    DeviceMessage = "{\"message\": \"" + DeviceMessage + "\",\"title\": \"Opozee App\", \"type\" : \"" + NotificationType + "\",\"questId\":"+ questId +"}";

                    var payload = new
                    {
                        to = DeviceToken,
                        priority = "high",
                        content_available = true,
                        data = new
                        {
                            body = DeviceMessage,
                            title = "Opozee App",
                            badge = 1
                        },

                    };

                string postbody = JsonConvert.SerializeObject(payload).ToString();
                Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    sResponseFromServer = tReader.ReadToEnd();
                                }
                        }
                    }
                }
            }
                return sResponseFromServer;
        }
            catch (Exception ex)
            {
                return ex.Message + "\n" + ex.StackTrace;
            }
}

void push_OnChannelDestroyed(object sender)
{
    //throw new NotImplementedException();
}



void push_OnServiceException(object sender, Exception error)
{
    //throw new NotImplementedException();
}

    }
}
