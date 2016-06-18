using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Commons;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.Comments
{
    public class AmazonComments : Amazon, IComments
    {
        public string SiteName
        {
            get
            {
               return "亚马逊";
            }
            set { SiteName = value; }
        }
        private const string UrlMode = "http://www.amazon.cn/product-reviews/{0}/ref=cm_cr_pr_top_link_2?ie=UTF8&pageNumber={1}&showViewpoints=0&sortBy=bySubmissionDateDescending"; //一般的品论


        public List<CommentInfo> GetCommentsFirstPage(string itemUrl)
        {
            string proid = RegGroupsX<string>(itemUrl, "/product/(?<x>.*?)$");
            string url = string.Format(UrlMode, proid, 0);
            HtmlAnalysis requert = new HtmlAnalysis { RequestReferer = itemUrl };
            string page = requert.HttpRequest(url);
            if (page.Contains("目前还没有用户评论"))
                return null;
            var list = RegGroupCollection(page, "<div style=\"margin-left:0.5em;\">(?<x>.*?)<span\nclass=\"crVotingButtons\">");
            if (list == null)
                return null;
            return (from Match item in list select getCommentNode(item.Groups["x"].Value)).Where(c => c != null).ToList();
        }

        private CommentInfo getCommentNode(string item)
        {
            CommentInfo comm = new CommentInfo();
            comm.Content = RegGroupsX<string>(item, "<div class=\"reviewText\">(?<x>.*?)</div>");
            comm.Content = CommentBase.FiterContent(comm.Content, 6);
            if (string.IsNullOrEmpty(comm.Content))
                return null;
            comm.UserName = RegGroupsX<string>(item, "<span style = \"font-weight: bold;\">(?<x>.*?)</span>");
            string time = RegGroupsX<string>(item, "<nobr>(?<x>.*?)</nobr>");
           
            if(!string.IsNullOrEmpty(time))
            {
                time = time.Replace("年", "-").Replace("月", "-").Replace("日", "");
                DateTime sendTime = DateTime.MinValue;
                if (DateTime.TryParse(time, out sendTime))
                    comm.SendTime = sendTime;
            }
            comm.Score = RegGroupsX<int>(item, "swSprite s_star_(?<x>\\d+)_\\d+");
            return comm;

        }

        public List<CommentInfo> GetAllComments(string itemUrl)
        {
            throw new NotImplementedException();
        }
    }
}
