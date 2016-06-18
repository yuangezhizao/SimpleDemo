using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using NetDimension.Json.Linq;
using SpriderProxy.Analysis;

namespace BLL.Sprider.Comments
{
    public class GmComments : JingDong, IComments
    {
        public string SiteName
        {
            get
            {
              return "国美在线";
            }
            set { SiteName = value; }
        }

        private const string UrlMode = "http://www.gome.com.cn/ec/homeus/n/product/gadgets/prdevajsonp/appraiseModuleAjax.jsp?&productId={0}&pagenum={1}"; //一般的品论
        private const string UrlModeimg = "http://club.jd.com/productpage/p-{0}-s-4-t-0-p-{1}.html"; //有图平路
        public string CommentType { get; set; }

        public List<CommentInfo> GetCommentsFirstPage(string itemUrl)
        {
            string proid = RegGroupsX<string>(itemUrl, "http://item.gome.com.cn/(?<x>.*?)(-|.html)");
            string url = "";
            if (string.IsNullOrEmpty(CommentType) || CommentType == "1")
                url = string.Format(UrlMode, proid, 1);
            else if (CommentType == "2")
                url = string.Format(UrlModeimg, proid, 1);

            HtmlAnalysis requert = new HtmlAnalysis { RequestReferer = itemUrl };
            string json = requert.HttpRequest(url);
            if (json.Contains("\"totalCount\":0") || !json.Contains("totalCount") || json.Contains(" \"msg\": \"数据不存在\""))
                return null;

            JObject obj = JObject.Parse(json);
            if (obj["evaList"] == null || obj["evaList"]["Evalist"] == null)
                return null;
            return obj["evaList"]["Evalist"].Children().Select(getCommentNode).Where(c => c != null).ToList();
        }

        private CommentInfo getCommentNode(JToken item)
        {
            try
            {

                string content = CommentBase.FiterContent(item["appraiseElSum"].ToString(), 6);
                if (string.IsNullOrEmpty(content))
                    return null;

                if (content.Contains("默认好评"))
                    return null;
                string createTime = item["post_time"].ToString();
                DateTime ctime;
                DateTime.TryParse(createTime, out ctime);
                StringBuilder imags = new StringBuilder();
                if (item["pic"] != null)
                {
                    
                    foreach (JToken jToken in item["pic"].Children())
                    {
                        imags.Append(jToken);
                        imags.Append(";");
                    }
                  
                }
                int score;
                int.TryParse(item["mscore"].ToString(), out score);
                string nickName = item["loginname"].ToString();
                if (nickName.Length > 10)
                {
                    nickName = nickName.Substring(0,3) + "***" + nickName.Substring(nickName.Length - 3);
                }
                CommentInfo info = new CommentInfo
                {
                    UserName = nickName,
                    ComSmallImg = imags.ToString(),
                    CommBigImg = imags.ToString(),
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
            throw new NotImplementedException();
        }
    }
}
