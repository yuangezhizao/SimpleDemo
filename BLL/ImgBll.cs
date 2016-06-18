using System;
using Commons;
using DataBase;
using Mode;

namespace BLL
{
    public class ImgBll
    {
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="url">图片地址</param>
        public ImageInfo GetImg(string url)
        {

            ImageInfo img = new ImageDB().ImageByUrl(url);
            if (img == null)
            { 
                img = SaveImg(url);
            }
            return img;
        }

        public ImageInfo SaveImg(string url)
        {
            ImageInfo img = new ImageInfo();
            ImageServer server = new ImageServer();
            img.ImgPath = "/ProPic/" + DateTime.Now.ToString("yyyyMM") + "/";
            img.ImgName = server.DoloadImg(url, img.ImgPath);
            string tempUrl = img.ImgPath + "Thumb/220" + img.ImgName;
            img.ImgUrl = img.ImgPath + img.ImgName;
            img.FromUrl = url;
            img.CreateDate = DateTime.Now;
            img.IsDel = false;
            img.ImgByte = server.GetPictureData(tempUrl);
            new ImageDB().AddSiteInfo(img);
            return img;
        }


    }
}
