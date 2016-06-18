using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using SpriderProxy.Analysis;
using NetDimension.Json.Linq;

namespace BLL.Sprider.Comments
{
    public class JdComments : JingDong, IComments
    {
        private const string UrlMode = "http://club.jd.com/productpage/p-{0}-s-0-t-0-p-{1}.html"; //一般的品论
        private const string UrlModeimg = "http://club.jd.com/productpage/p-{0}-s-4-t-0-p-{1}.html"; //有图平路
        public string CommentType { get; set; }

        public List<CommentInfo> GetCommentsFirstPage(string itemUrl)
        {
            string proid = RegGroupsX<string>(itemUrl, "(?<x>\\d+)");
            string url = "";
            if (string.IsNullOrEmpty(CommentType) || CommentType == "1")
                url = string.Format(UrlMode, proid, 0);
            else if (CommentType == "2")
                url = string.Format(UrlModeimg, proid, 0);

            HtmlAnalysis requert = new HtmlAnalysis {RequestReferer = itemUrl};
            string json = requert.HttpRequest(url);
            if (json.Contains("\"comments\":[],") || !json.Contains("comments"))
                return null;

            JObject obj = JObject.Parse(json);
            if (obj["comments"] == null)
                return null;
            JArray list = (JArray) obj["comments"];

            return list.Select(getCommentNode).Where(c => c != null).ToList();
        }

        private CommentInfo getCommentNode(JToken item)
        {
            try
            {

                string content = CommentBase.FiterContent(item["content"].ToString(), 10);
                if (string.IsNullOrEmpty(content))
                    return null;
                string createTime = item["creationTime"].ToString();
                DateTime ctime;
                DateTime.TryParse(createTime, out ctime);
                StringBuilder imags = new StringBuilder();
                StringBuilder BigImags = new StringBuilder();
                if (item["images"] != null)
                {
                    JArray images = (JArray) item["images"];
                    for (int j = 0; j < images.Count(); j++)
                    {
                        imags.Append(images[j]["imgUrl"]);
                        imags.Append(";");
                    }
                }
                int score;
                int.TryParse(item["score"].ToString(), out score);
                if (item["showOrderComment"] != null && item["showOrderComment"]["content"] != null)
                {
                    string ordercom = item["showOrderComment"]["content"].ToString();
                    var imglist = RegGroupCollection(ordercom, "src=\"(?<x>.*?)\"|src='(?<x>.*?)'");
                    if (imglist != null && imglist.Count > 0)
                    {
                        for (int i = 0; i < imglist.Count; i++)
                        {
                            BigImags.Append(imglist[i].Groups["x"].Value);
                            BigImags.Append(";");
                        }
                    }

                }
                CommentInfo info = new CommentInfo
                {
                    UserName = item["nickname"].ToString(),
                    ComSmallImg = imags.ToString(),
                    CommBigImg = BigImags.ToString(),
                    SendTime = ctime,
                    Score = score,
                    Content = content
                };

                return info;
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "CommentSpiderError");
                return null;
            }

        }


        public List<CommentInfo> GetAllComments(string itemUrl)
        {
            return null;
        }

        public string SiteName
        {
            get { return "京东商城"; }
            set
            {
                SiteName = value;
            }
        }
    }
}
