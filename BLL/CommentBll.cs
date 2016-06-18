using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase;
using Mode;

namespace BLL
{
    public class CommentBll
    {
        public void AddSiteProInfo(List<CommentInfo> siteList)
        {
            if (!siteList.Any())
                return;
            new CommentDb().AddSiteProInfo(siteList);
        }

        public DateTime GetCommentEndDate(int bjid, int siteid)
        {
            return new CommentDb().GetCommentEndDate(bjid, siteid);
        }

        /// <summary>
        /// 获取最新品论的 商品Id
        /// </summary>
        /// <returns></returns>
        public int GetLastComentItemId()
        {
            return new CommentDb().GetLastComentItemId();
        }
    }
}
