using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace OposeeLibrary.Utilities
{
    public static class LogHelper
    {

        public enum ErrorType
        {
            
            Notification,
            System,
            User,
            Exception,
            APILog
        }


        public static async Task<Boolean> CreateLog(Exception ex, bool SystemException = true)
        {

            if (Convert.ToBoolean(WebConfigurationManager.AppSettings["Logger"].ToString()) == false)
            {
                return false;
            }

            return await Task.Factory.StartNew(() =>
            {

                try
                {

                    string filePath = "";
                    if (SystemException == true)
                    {
                        filePath = ConfigurationManager.AppSettings["LogFilePath"] + "ExceptionLog.txt";
                    }
                    else
                    {
                        filePath = ConfigurationManager.AppSettings["LogFilePath"] + "UserLog.txt";
                    }
                    Helper.CreateDirectories(ConfigurationManager.AppSettings["LogFilePath"]);

                    if (!File.Exists(filePath))
                    {
                        File.Create(filePath).Dispose();
                        using (StreamWriter writer = new StreamWriter(filePath, true))
                        {
                            writer.WriteLine("Message :" + ex.Message + " Method: " + ex.ToString() + " TargetSite: " + ex.TargetSite.ToString() + ex.TargetSite + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                            writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                        }
                    }
                    else
                    {
                        using (StreamWriter writer = new StreamWriter(filePath, true))
                        {
                            writer.WriteLine("Message :" + ex.Message + " Method: " + ex.ToString() + " TargetSite: " + ex.TargetSite.ToString() + ex.TargetSite + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                            writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                        }

                    }
                    return true;
                }
                catch
                {

                    return false;
                }
            });
        }

        public static async Task<Boolean> CreateLog(string ex, bool SystemException = true)
        {
            if (Convert.ToBoolean(WebConfigurationManager.AppSettings["Logger"].ToString()) == false)
            {
                return false;
            }

            return await Task.Factory.StartNew(() =>
            {

                try
                {
                    string filePath = "";


                    if (SystemException == true)
                    {

                        filePath = ConfigurationManager.AppSettings["LogFilePath"] + "ExceptionLog.txt";
                    }
                    else
                    {
                        filePath = ConfigurationManager.AppSettings["LogFilePath"] + "UserLog.txt";
                    }

                    Helper.CreateDirectories(ConfigurationManager.AppSettings["LogFilePath"]);

                    if (!File.Exists(filePath))
                    {
                        File.Create(filePath).Dispose();

                        using (StreamWriter writer = new StreamWriter(filePath, true))
                        {
                            writer.WriteLine("Message :" + ex.ToString() + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                            writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                        }
                    }
                    else
                    {

                        using (StreamWriter writer = new StreamWriter(filePath, true))
                        {
                            writer.WriteLine("Message :" + ex.ToString() + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                            writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                        }
                    }

                    return true;

                }
                catch
                {
                    return false;

                }


            });

        }


        public static Boolean CreateLog2(Exception ex, ErrorType _ErrorType = ErrorType.Exception)
        {

            if (Convert.ToBoolean(WebConfigurationManager.AppSettings["Logger"].ToString()) == false)
            {
                return false;
            }


            try
            {

                string filePath = "";

                if (ErrorType.Exception == _ErrorType)
                {
                    filePath = ConfigurationManager.AppSettings["LogFilePath"] + "\\ExceptionLog.txt";
                }
                else if (ErrorType.User == _ErrorType)
                {
                    filePath = ConfigurationManager.AppSettings["LogFilePath"] + "\\UserLog.txt";
                }
                
                else if (ErrorType.Notification == _ErrorType)
                {
                    filePath = ConfigurationManager.AppSettings["LogFilePath"] + "\\Notification.txt";
                }
               

                Helper.CreateDirectories(ConfigurationManager.AppSettings["LogFilePath"]);

                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine("Message :" + ex.Message + " TargetSite: " + ex.TargetSite.ToString() + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }
                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine("Inner :" + ex.InnerException.ToString() + "Message :" + ex.Message + " TargetSite: " + ex.TargetSite.ToString() + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }

                }

                return true;
            }
            catch
            {

                return false;
            }





        }


        public static Boolean CreateLog3(Exception ex, HttpRequestMessage Request, ErrorType _ErrorType = ErrorType.Exception)
        {

            if (Convert.ToBoolean(WebConfigurationManager.AppSettings["Logger"].ToString()) == false)
            {
                return false;
            }


            try
            {

                string filePath = "";

                if (ErrorType.Exception == _ErrorType)
                {
                    filePath = ConfigurationManager.AppSettings["LogFilePath"] + "\\ExceptionLog.txt";
                }
                else if (ErrorType.User == _ErrorType)
                {
                    filePath = ConfigurationManager.AppSettings["LogFilePath"] + "\\UserLog.txt";
                }
                
                else if (ErrorType.Notification == _ErrorType)
                {
                    filePath = ConfigurationManager.AppSettings["LogFilePath"] + "\\Notification.txt";
                }
               
                Helper.CreateDirectories(ConfigurationManager.AppSettings["LogFilePath"]);

                string _otherstuff = "";

                try
                {
                    HttpContent requestContent = Request.Content;
                    string jsonContent = requestContent.ReadAsStringAsync().Result;
                    Request.RequestUri.ToString();


                    _otherstuff = "REQUST : " + Request.RequestUri.ToString() + " BODY :" + jsonContent.ToString() + " ";
                }
                catch
                {
                }

                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine("" + _otherstuff + "Message :" + ex.Message + " TargetSite: " + ex.TargetSite.ToString() + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }
                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        string _innerexception = (ex.InnerException == null) ? "" : ex.InnerException.ToString();
                        writer.WriteLine("" + _otherstuff + "Inner :" + _innerexception + " Message :" + ex.Message + " TargetSite: " + ex.TargetSite.ToString() + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }

                }

                return true;
            }
            catch
            {

                return false;
            }





        }


        public static Boolean CreateLog2(string logmessage, ErrorType _ErrorType = ErrorType.User)
        {
            if (Convert.ToBoolean(WebConfigurationManager.AppSettings["Logger"].ToString()) == false)
            {
                return false;
            }



            try
            {
                string filePath = "";

                if (ErrorType.Exception == _ErrorType)
                {
                    filePath = ConfigurationManager.AppSettings["LogFilePath"] + "\\ExceptionLog.txt";
                }
                else if (ErrorType.User == _ErrorType)
                {
                    filePath = ConfigurationManager.AppSettings["LogFilePath"] + "\\UserLog.txt";
                }
                
                else if (ErrorType.Notification == _ErrorType)
                {
                    filePath = ConfigurationManager.AppSettings["LogFilePath"] + "\\Notification.txt";
                }
               

                Helper.CreateDirectories(ConfigurationManager.AppSettings["LogFilePath"]);

                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine("Message :" + logmessage.ToString() + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }
                }
                else
                {

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine("Message :" + logmessage.ToString() + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }
                }

                return true;

            }
            catch
            {
                return false;

            }



        }

        /// <summary>
        /// LogHelper.CreateAPILog("AddSearch", JsonConvert.SerializeObject(input).ToString(), ErrorType.APILog);
        /// 
        /// </summary>
        /// <param name="Api"></param>
        /// <param name="logmessage"></param>
        /// <param name="_ErrorType"></param>
        /// <returns></returns>

        public static Boolean CreateAPILog(string Info, string Api, string logmessage, ErrorType _ErrorType = ErrorType.User)
        {


            if (Convert.ToBoolean(WebConfigurationManager.AppSettings["APILogger"].ToString()) == false)
            {
                return false;
            }



            try
            {
                string filePath = "";
                string _Readvalue = "";

                if (ErrorType.APILog == _ErrorType)
                {
                    filePath = ConfigurationManager.AppSettings["LogFilePath"] + "\\APILog.txt";
                }

                Helper.CreateDirectories(ConfigurationManager.AppSettings["LogFilePath"]);

                if (File.Exists(filePath))
                {
                    _Readvalue = File.ReadAllText(filePath);
                }

                //if (!_Readvalue.Contains(Api))
                //{
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine(Info + " : " + logmessage.ToString() + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }
                }
                else
                {

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine(Info + " : " + logmessage.ToString() + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }
                }
                //}

                return true;

            }
            catch
            {
                return false;

            }




        }


    }
}
