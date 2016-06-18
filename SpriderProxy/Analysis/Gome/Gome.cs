using System;
using System.Collections.Generic;

namespace SpriderProxy.Analysis.Gome
{
   public class Gome : BaseSiteInfo
   {


       public override bool ValidCatId(string catId)
       {
           if (string.IsNullOrEmpty(catId))
               return false;
           if (catId.Length < 8 || catId.Length > 11)
               return false;
           return true;
       }

       private const string commentUrl = "http://www.gome.com.cn/ec/homeus/n/product/gadgets/prdevajsonp/appraiseModuleAjax.jsp?callback=all&productId={0}&pagenum={1}&_this=cmt1";
       /// <summary>
       /// 获取评论地址
       /// </summary>
       /// <param name="catid">分类Id</param>
       /// <param name="pageid">页号</param>
       /// <returns></returns>
       public string getCommentUrl(string catid, int pageid)
       {
           return string.Format(commentUrl, catid, pageid);
       }
   }
}
