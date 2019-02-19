using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using ImageBlurFilter;

namespace OposeeLibrary.Utilities
{
    public static class ResizeImage
    {
        public static string Resize_Image_Thumb(string filePath, string newFilePath, string newImgName, int newWidth, int newHeight)
        {
            string newImagePath;

            try
            {
                newImagePath = newFilePath + newImgName;

                Image imgPhoto = Image.FromFile(filePath);

                // Do not reszie image if its width is <= newWidth  ///
                if (imgPhoto.Width <= newWidth)
                {
                    imgPhoto.Save(newImagePath);
                    imgPhoto.Dispose();
                    return newImgName;
                }

                int sourceWidth, sourceHeight, sourceX, sourceY, destX, destY;
                double nPercent, nPercentW, nPercentH;
                sourceWidth = imgPhoto.Width;
                sourceHeight = imgPhoto.Height;
                sourceX = 0;
                sourceY = 0;
                destX = 0;
                destY = 0;

                nPercent = 0;
                nPercentW = 0;
                nPercentH = 0;

                // nPercentW = (Convert.ToDouble(newWidth) / Convert.ToDouble(sourceWidth));
                //nPercentH = (Convert.ToDouble(newHeight) / Convert.ToDouble(sourceHeight));

                nPercentW = ((float)newWidth / (float)sourceWidth);
                nPercentH = ((float)newHeight / (float)sourceHeight);

                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentH;
                }
                else
                {
                    nPercent = nPercentW;
                }
                

                if (nPercent > 1)
                    nPercent = 1;

                int destWidth = (int)Math.Round(sourceWidth * nPercent);
                int destHeight = (int)Math.Round(sourceHeight * nPercent);
                
                Bitmap bmPhoto = new Bitmap(destWidth <= newWidth ? destWidth : newWidth, destHeight < newHeight ? destHeight : newHeight,
       PixelFormat.Format32bppRgb);

                Graphics grPhoto = System.Drawing.Graphics.FromImage(bmPhoto);
                grPhoto.Clear(System.Drawing.Color.White);
                grPhoto.InterpolationMode = InterpolationMode.Default;
                grPhoto.CompositingQuality = CompositingQuality.HighQuality;
                grPhoto.SmoothingMode = SmoothingMode.HighQuality;

                grPhoto.DrawImage(imgPhoto,
                new System.Drawing.Rectangle(destX, destY, destWidth, destHeight),
                new System.Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                System.Drawing.GraphicsUnit.Pixel);

                grPhoto.Dispose();

                bmPhoto.Save(newImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                bmPhoto.Dispose();

                imgPhoto.Dispose();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return newImgName;
        }

        public static string ScaleImage(string filePath, string newFilePath, string newImgName, int maxWidth, int maxHeight, bool needToFill = true)
        {
            string newImagePath;

            try
            {

                var bmp = new Bitmap(maxWidth, maxHeight);
                newImagePath = newFilePath + newImgName;

                Image imgPhoto = Image.FromFile(filePath);

                var ratioX = (double)maxWidth / imgPhoto.Width;
                var ratioY = (double)maxHeight / imgPhoto.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(imgPhoto.Width * ratio);
                var newHeight = (int)(imgPhoto.Height * ratio);

                var newImage = new Bitmap(newWidth, newHeight);

                using (var graphics = Graphics.FromImage(bmp))
                    graphics.DrawImage(imgPhoto, 0, 0, newWidth, newHeight);

                bmp.Save(newImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                bmp.Dispose();

                

            }
            catch
            {

            }
            return newImgName;
        }

        public static string CropAndResizeImage(string filePath, string newFilePath, string newImgName, int newWidth, int newHeight, int x1, int y1, int x2, int y2)
        {
            string newImagePath;

            try
            {
                var bmp = new Bitmap(newWidth, newHeight);

                newImagePath = newFilePath + newImgName;

                Image imgPhoto = Image.FromFile(filePath);

                Graphics g = Graphics.FromImage(bmp);

                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                int width = x2 - x1;
                int height = y2 - y1;

                g.DrawImage(imgPhoto, new Rectangle(0, 0, newWidth, newHeight), x1, y1, width, height, GraphicsUnit.Pixel);

                var memStream = new MemoryStream();

                bmp.Save(newImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                bmp.Dispose();

                imgPhoto.Dispose();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return newImgName;
        }

        //string imageresizename = ResizeImage.CropImage(tempfilePath + strTempImageSave, newFilePath, "_R_" + strTempImageSave, 0, 200,600,400);
        public static string CropImage(string filePath, string newFilePath, string newImgName, int x1, int y1, int x2, int y2)
        {
            return CropAndResizeImage(filePath, newFilePath, newImgName, x2 - x1, y2 - y1, x1, y1, x2, y2);
        }

        public static string GetTempFilePath(bool flag)
        {
            if (flag == false)
            {
                //----Temparory SubDirectory Create
                if (!Directory.Exists(WebConfigurationManager.AppSettings["SiteImgPath"] + @"\" + "ProfileImage"))
                {
                    Directory.CreateDirectory(WebConfigurationManager.AppSettings["SiteImgPath"] + @"\" + "ProfileImage");
                }
            }
            else
            {
                Directory.Delete(WebConfigurationManager.AppSettings["SiteImgPath"] + @"\" + "ProfileImage", true);
            }

            return WebConfigurationManager.AppSettings["SiteImgPath"] + @"\" + "ProfileImage" + @"\";
        }


        public static System.Drawing.Image DownloadImageFromUrl(string imageUrl)
        {
            System.Drawing.Image image = null;

            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();

                image = System.Drawing.Image.FromStream(stream);

                webResponse.Close();
            }
            catch (Exception ex)
            {
                return null;
            }

            return image;
        }

        public static string Download_Image(string imageUrl)
        {
            try
            {
                System.Drawing.Image image = DownloadImageFromUrl(imageUrl);
                var filePath = GetTempFilePath(false);
                string rootPath = filePath;
                string newImageName = Guid.NewGuid().ToString() + ".png";
                string fileName = System.IO.Path.Combine(rootPath, newImageName);
                image.Save(fileName);
                return newImageName;
            }
            catch (Exception ex)
            {
                return ex.ToString();

            }
        }

        


        public static string Get_BlurImage(string _TempImage, string _NewImagePath, ImageBlurFilter.ExtBitmap.BlurType _BlurType)
        {
            System.Drawing.Bitmap selectedSource = null;
            System.Drawing.Bitmap bitmapResult = null;

            System.IO.StreamReader streamReader = new System.IO.StreamReader(_TempImage);
            selectedSource = (Bitmap)Bitmap.FromStream(streamReader.BaseStream);
            streamReader.Close();

            ImageBlurFilter.ExtBitmap.BlurType blurType = _BlurType;
            bitmapResult = selectedSource.ImageBlurFilter(blurType);


            string fileExtension = System.IO.Path.GetExtension(_TempImage).ToUpper();
            System.Drawing.Imaging.ImageFormat imgFormat = System.Drawing.Imaging.ImageFormat.Png;

            if (fileExtension == "BMP")
            {
                imgFormat = System.Drawing.Imaging.ImageFormat.Bmp;
            }
            else if (fileExtension == "JPG")
            {
                imgFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
            }
            else if (fileExtension == "PNG")
            {
                imgFormat = System.Drawing.Imaging.ImageFormat.Png;
            }

            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(_NewImagePath + "_Blur_" + System.IO.Path.GetFileName(_TempImage), false);
            bitmapResult.Save(streamWriter.BaseStream, imgFormat);
            streamWriter.Flush();
            streamWriter.Close();

            bitmapResult = null;
            return "_Blur_" + System.IO.Path.GetFileName(_TempImage);

        }


        public static string ImageToBase64(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

    }
}
