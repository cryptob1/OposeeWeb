using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using OposeeLibrary.Utilities;

namespace oposee.Models.API
{
    public static class MultipartFiles
    {
        public static ImageResponse GetMultipartImage(HttpFileCollection files, string fileKey, string folderPath, int thumbWidth,
                    int thumbHeight, int imgWidth, int imgHeight, bool deleteOriginal, bool GenThumbnail, bool GenImage, string folderPathSite, bool GenBanner = false)
        {

            string _SiteRoot = WebConfigurationManager.AppSettings["SiteImgPath"];
            string _SiteURL = WebConfigurationManager.AppSettings["SiteImgURL"];


            var file = files.Count > 0 ? files[fileKey] : null;
            ImageResponse response = new ImageResponse();

            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                string extension = Path.GetExtension(file.FileName).ToLower();
                string[] arr = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };

                if (arr.Contains(extension))
                {
                    string _NewFileName = Guid.NewGuid() + extension;

                    string tempfilePath = _SiteRoot + @"\" + "TempImages" + @"\" + _NewFileName;
                    Helper.CreateDirectories(_SiteRoot + @"\" + "TempImages" + @"\");

                    string newFilePath = _SiteRoot + @"\" + folderPath + @"\";
                    Helper.CreateDirectories(_SiteRoot + @"\" + folderPath + @"\");


                    string strIamgeURLfordb = _SiteURL + "/" + folderPathSite + "/" + _NewFileName;

                    file.SaveAs(tempfilePath);
                    string thumbnailresizename = "";
                    string Imageresizename = "";
                    string ImageBannerresizename = "";
                    string imageresizenametmp = "";

                    if (GenThumbnail == true)
                    {
                        thumbnailresizename = ResizeImage.Resize_Image_Thumb(tempfilePath, newFilePath, "_T_" + _NewFileName, 400, 400);

                    }

                    if (GenImage == true)
                    {

                        //Scale up the image 
                        imageresizenametmp = ResizeImage.ScaleImage(tempfilePath, tempfilePath, "_S_" + _NewFileName, 650, 650);

                        //Imageresizename = ResizeImage.Resize_Image_Thumb(tempfilePath + imageresizenametmp, newFilePath, "_A_" + imageresizenametmp, 1428, 689);
                        Imageresizename = ResizeImage.CropImage(tempfilePath + imageresizenametmp, newFilePath, "_A_" + imageresizenametmp, 0, 50, 640, 360);
                    }

                    if (GenBanner == true)
                    {
                        //Banner Image (event listing)
                        ImageBannerresizename = ResizeImage.CropImage(tempfilePath + imageresizenametmp, newFilePath, "_B_" + imageresizenametmp, 0, 100, 640, 270);


                    }

                    //if (deleteOriginal == true)
                    //{
                    //    if (File.Exists(imagePath))
                    //    {
                    //        File.Delete(imagePath);
                    //    }
                    //}


                    response.ThumbnailURL = _SiteURL + "/" + folderPathSite + "/" + thumbnailresizename;
                    response.ImageURL = _SiteURL + "/" + folderPathSite + "/" + Imageresizename;
                    response.BannerImage_URL = _SiteURL + "/" + folderPathSite + "/" + ImageBannerresizename;
                    response.IsSuccess = true;
                    response.ResponseMessage = "";

                    return response;
                }
                else
                {
                    response.IsSuccess = false;
                    response.ResponseMessage = "File Extension not supported";

                    return response;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.ResponseMessage = "File Not found";

                return response;
            }



        }




        public static string GetVideo(HttpFileCollection files, string fileKey)
        {
            var file = files.Count > 0 ? files[fileKey] : null;
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var extension = Path.GetExtension(file.FileName).ToLower();
                string videoName = Guid.NewGuid() + extension;

                string currentYear = DateTime.Now.Year.ToString();
                string currentMonth = DateTime.Now.Month.ToString();

                string VideoPath = ConfigurationManager.AppSettings["RootPath"].ToString() + "Videos\\" + currentYear + "\\" + currentMonth + "\\";
                string VideoUrl = ConfigurationManager.AppSettings["WebPath"].ToString() + "Content/Uploads/Videos/" + currentYear + "/" + currentMonth + "/" + videoName;

                if (!Directory.Exists(VideoPath))
                {
                    Directory.CreateDirectory(VideoPath);
                }
                file.SaveAs(VideoPath + videoName);
                return VideoUrl;
            }
            else
            {
                return "";
            }
        }
    }
}