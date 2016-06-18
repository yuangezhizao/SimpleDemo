using System;
using System.Collections.Generic;
using System.Linq;

using System.Text.RegularExpressions;
using Commons;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.Comments
{
    class YhdComments : Yhd, IComments
    {
        public string SiteName
        {
            get
            {
                return "一号店";
            }
            set
            {
                SiteName = value;
            }
        }
        private const string UrlMode = "http://e.yhd.com/front-pe/productExperience/proExperienceAction!ajaxView_pe.do?product.id={0}&merchantId=1&filter.orderType=newest&pagenationVO.currentPage={1}"; //一般的品论

        private const string UrlModeimg = "http://club.jd.com/productpage/p-{0}-s-4-t-0-p-{1}.html"; //有图平路
        public string CommentType { get; set; }
        public List<CommentInfo> GetCommentsFirstPage(string itemUrl)
        {
            string proid = RegGroupsX<string>(itemUrl, "(?<x>\\d+)");
        
            //string.Format("item.m.yhd.com/item/{0}", proid);
            var page = HtmlAnalysis.Gethtmlcode(string.Format("http://item.m.yhd.com/item/{0}", proid));
            if (! regIsMatch(page, " <span class=\"comment\">（(?<x>\\d+)条评论）</span>"))
                return null;
            string productId = RegGroupsX<string>(page,
                "<a href=\"http://item.m.yhd.com/item/desc-(?<x>\\d+)\" class=\"pd_product-desc\"");
            string url = "";
            if (string.IsNullOrEmpty(CommentType) || CommentType == "1")
                url = string.Format(UrlMode, productId, 0);
            else if (CommentType == "2")
                url = string.Format(UrlModeimg, productId, 0);

            var commentPage = HtmlAnalysis.Gethtmlcode(string.Format(url, proid));
            var list = RegGroupCollection(commentPage, "item good-comment(?<x>.*?)</dl>");
            if (list == null || list.Count == 0)
                return null;
            return (from Match item in list select getCommentNode(item.Groups["x"].Value)  ).Where(c=>c!= null).ToList();
        }

        private CommentInfo getCommentNode(string item)
        {
            item = item.Replace("\\r", "").Replace("\\t", "").Replace("\\n", "").Replace("\\", "");
            CommentInfo res = new CommentInfo();
            res.Score = RegGroupsX<int>(item, "<span class=\"star s(?<x>\\d+)\">");
            res.Content = RegGroupsX<string>(item, "<span class=\"redTag\">.*?</span>(?<x>.*?)</span>");

            res.Content = CommentBase.FiterContent(res.Content, 6);
            if (string.IsNullOrEmpty(res.Content))
                return null;
            res.SendTime = RegGroupsX<DateTime>(item, @"(?<x>\d{4}(\-|\/|\.)\d{1,2}\1\d{1,2} \d{1,2}:\d{1,2}:\d{1,2})");
            res.UserName = RegGroupsX<string>(item, "userNick=\"(?<x>.*?)\"");
            if (item.Contains("晒单"))
            {
                var imglist = RegGroupCollection(item, "<li data-tpc=\"SHINE\"><img width=\"46\" height=\"46\" src=\"(?<x>.*?)\">");
                if(imglist!=null)
                {
                    foreach (Match match in imglist)
                    {
                        res.ComSmallImg += match.Groups["x"].Value+";";
                    }
                    res.CommBigImg = res.ComSmallImg.Replace("_40x40", "");
                }


            }

            return res;
        }

        public List<CommentInfo> GetAllComments(string itemUrl)
        {
            throw new NotImplementedException();
        }
    }
}
