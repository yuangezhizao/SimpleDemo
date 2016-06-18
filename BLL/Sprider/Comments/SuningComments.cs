using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Commons;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.Comments
{
    public class SuningComments : Suning, IComments
    {
        public string SiteName
        {
            get { return "苏宁易购"; }
            set { SiteName = value; }
        }
        //http://zone.suning.com/review/json/product_reviews/000000000103630989--total-g-848700---10-2-getItem.html?callback=getItem
        private const string UrlMode = "http://zone.suning.com/review/pro_review/{0}-0-{1}--.html"; //一般的品论
        private const string UrlModeimg = "http://club.jd.com/productpage/p-{0}-s-4-t-0-p-{1}.html"; //有图平路
        public string CommentType { get; set; }
        public List<CommentInfo> GetCommentsFirstPage(string itemUrl)
        {
            string itemid = RegGroupsX<string>(itemUrl, "(?<x>\\d+)");

            string comUrl = string.Format(UrlMode, itemid, "1");
            string page = HtmlAnalysis.Gethtmlcode(comUrl);
            if (page.Contains("暂无商品评价") || page=="")
                return null;
            var list = RegGroupCollection(page, "<div class=\"comment-item fix\">(?<x>.*?)<div class=\"optionbox\">");

            return (from Match item in list select getCommentNode(item.Groups["x"].Value)).Where(c => c != null).ToList();
        }


        private CommentInfo getCommentNode(string item)
        {
            CommentInfo comm = new CommentInfo();
            string content= RegGroupsX<string>(item, "<div class=\"content\">\\s*<p>(?<x>.*?)<a");
            if (content == null)
                return null;
            content =WordCenter.FilterHtml(content);

            comm.Content = CommentBase.FiterContent(content, 6);
            if (string.IsNullOrEmpty(comm.Content))
                return null;
            comm.UserName = RegGroupsX<string>(item, "<p><span href=\"\">(?<x>.*?)</span></p>");
            comm.SendTime = RegGroupsX<DateTime>(item, "return false;\">(?<x>.*?)</a>");

            comm.Score = RegGroupsX<int>(item, "swSprite s_star_(?<x>\\d+)_\\d+");
            return comm;

        }

        public List<CommentInfo> GetAllComments(string itemUrl)
        {
            throw new NotImplementedException();
        }
    }
}
