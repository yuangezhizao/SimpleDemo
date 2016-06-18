using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
   public class ImageInfo
    {
        
       /// <summary>
       /// 图片名称
       /// </summary>
       [PrimaryKey]
       public string ImgName { get; set; }

       /// <summary>
       ///图片路径
       /// </summary>
       public string ImgPath { get; set; }

       /// <summary>
       /// url地址
       /// </summary>
       public string ImgUrl { get; set; }
       /// <summary>
       /// 图片二进制码
       /// </summary>
       public byte[] ImgByte { get; set; }
       /// <summary>
       /// 原始地址
       /// </summary>
       public string FromUrl { get; set; }

       public DateTime CreateDate { get; set; }

       public bool IsDel { get; set; }
    }
}
