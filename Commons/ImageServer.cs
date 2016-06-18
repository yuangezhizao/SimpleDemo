using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Commons
{
   public class ImageServer
    {
        /// <summary> 
        /// 生成缩略图 (正式使用这个函数)
        /// </summary> 
        /// <param name="SourceFile">文件在服务器上的物理地址</param> 
        /// <param name="strSavePathFile">保存在服务器上的路径（物理地址）</param> 
        /// <param name="ThumbWidth">宽度</param> 
        /// <param name="ThumbHeight">高度</param> 
        /// <param name="BgColor">背景</param> 
        public void myGetThumbnailImage(string SourceFile, string strSavePathFile, int ThumbWidth, int ThumbHeight, string BgColor)
        {
            var oImg = System.Drawing.Image.FromFile(SourceFile);
            //小图 
            int intwidth, intheight;
            //当图片的宽度大于高度
            if (oImg.Width > oImg.Height)
            {
                if (ThumbWidth > 0)  //限制了宽度
                {
                    if (oImg.Width > ThumbWidth)
                    {
                        intwidth = ThumbWidth;
                        intheight = (oImg.Height * ThumbWidth) / oImg.Width;
                    }
                    else
                    {
                        intwidth = oImg.Width;
                        intheight = oImg.Height;
                    }
                }
                else//不限制宽度，那就意味着限制高度
                {
                    if (oImg.Height > ThumbHeight)
                    {
                        intwidth = (oImg.Width * ThumbHeight) / oImg.Height;
                        intheight = ThumbHeight;
                    }
                    else
                    {
                        intwidth = oImg.Width;
                        intheight = oImg.Height;
                    }
                }
            }
            else
            {
                if (ThumbHeight > 0)
                {
                    if (oImg.Height > ThumbHeight)
                    {
                        intwidth = (oImg.Width * ThumbHeight) / oImg.Height;
                        intheight = ThumbHeight;
                    }
                    else
                    {
                        intwidth = oImg.Width;
                        intheight = oImg.Height;
                    }
                }
                else
                {
                    if (oImg.Width > ThumbWidth)
                    {
                        intwidth = ThumbWidth;
                        intheight = (oImg.Height * ThumbWidth) / oImg.Width;
                    }
                    else
                    {
                        intwidth = oImg.Width;
                        intheight = oImg.Height;
                    }
                }
            }
            //构造一个指定宽高的Bitmap 
            Bitmap bitmay = new Bitmap(intwidth, intheight);
            Graphics g = Graphics.FromImage(bitmay);
            Color myColor;
            if (BgColor == null)
                myColor = Color.FromName("white");
            else
                myColor = Color.FromName(BgColor);
            //用指定的颜色填充Bitmap 
            g.Clear(myColor);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;// HighQualityBicubic;//最高质量的缩略
            //开始画图 
            g.DrawImage(oImg, new Rectangle(0, 0, intwidth, intheight), new Rectangle(0, 0, oImg.Width, oImg.Height), GraphicsUnit.Pixel);
            bitmay.Save(strSavePathFile, System.Drawing.Imaging.ImageFormat.Jpeg);
            g.Dispose();
            bitmay.Dispose();
            oImg.Dispose();
        }

        public string DoloadImg(string surl, string picurlDir)
        {
  
            String wlDir = HttpContext.Current.Server.MapPath(picurlDir);
            String wlDir2 = HttpContext.Current.Server.MapPath(picurlDir + "/Thumb");

            if (!Directory.Exists(wlDir))
            {
                Directory.CreateDirectory(wlDir);
            }
            if (!Directory.Exists(wlDir2))
            {
                Directory.CreateDirectory(wlDir2);
            }

            string filePath = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            filePath += new Random().Next(99).ToString("00");
            

            try
            {
                String sfile = filePath + ".jpg";
                String wlfile = wlDir + "\\" + sfile;
                WebClient myWebClient = new WebClient();
                myWebClient.DownloadFile(surl, wlfile);

                int[] list ={32,60,80,100,160,220,360};
                foreach (var type in list)
                {
                    String wlfile2 = wlDir2 + "\\" + type + sfile;
                    myGetThumbnailImage(wlfile, wlfile2, type, type, null);
                }

                return sfile;
            }
            catch
            {
                return "";
            }


        }

        public static string DoloadImg(byte[] data, string picurlDir)
        {

            String wlDir = HttpContext.Current.Server.MapPath(picurlDir);
            String wlDir2 = HttpContext.Current.Server.MapPath(picurlDir + "/Thumb");

            if (!Directory.Exists(wlDir))
            {
                Directory.CreateDirectory(wlDir);
            }
            if (!Directory.Exists(wlDir2))
            {
                Directory.CreateDirectory(wlDir2);
            }

            string filePath = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            filePath += new Random().Next(99).ToString("00");


            try
            {
                MemoryStream ms = new MemoryStream(data);
                Image image = System.Drawing.Image.FromStream(ms);

                String wlfile = wlDir + "\\" + filePath + ".jpg";

                image.Save(wlfile);
                ms.Close();
                return wlfile;
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex);
                return "";
            }


        }

        public  byte[] GetPictureData(string imagepath)
       {
           try
           {
               string imgPath = HttpContext.Current.Server.MapPath(imagepath);
               if (!File.Exists(imgPath))
                   return null;
               /**/
               ////根据图片文件的路径使用文件流打开，并保存为byte[] 
               FileStream fs = new FileStream(imgPath, FileMode.Open); //可以是其他重载方法 
               byte[] byData = new byte[fs.Length];
               fs.Read(byData, 0, byData.Length);
               fs.Close();
               return byData;
           }
           catch (Exception)
           {
               return null;
           }
       }

       private static Random randNum= new Random();
        //将文件名转化为时间方式
        private string CreateFilePath(string fext)
        {
            string filePath = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            filePath += new Random().Next(99).ToString("00");
            filePath += "." + fext;
            return filePath;
        }

        public static string CreateImageFromBytes(string fileName, byte[] buffer)
        {
            string file = fileName;
            Image image = BytesToImage(buffer);
            ImageFormat format = image.RawFormat;
            if (format.Equals(ImageFormat.Jpeg))
            {
                file += ".jpeg";
            }
            else if (format.Equals(ImageFormat.Png))
            {
                file += ".png";
            }
            else if (format.Equals(ImageFormat.Bmp))
            {
                file += ".bmp";
            }
            else if (format.Equals(ImageFormat.Gif))
            {
                file += ".gif";
            }
            else if (format.Equals(ImageFormat.Icon))
            {
                file += ".icon";
            }
            System.IO.FileInfo info = new System.IO.FileInfo(file);
            System.IO.Directory.CreateDirectory(info.Directory.FullName);
            File.WriteAllBytes(file, buffer);
            return file;
        }

        /// <summary>
        /// Convert Byte[] to Image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);

            return image;
        }
    }
}
