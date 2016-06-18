using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Commons;
using Mode;
using NetDimension.Json.Linq;
using SpriderProxy.Analysis;

namespace BLL.Sprider.Comments
{
    class YixunComments : YiXun, IComments
    {
        public string SiteName
        {
            get
            {
                return "易迅网";
            }
            set { SiteName = value; }
        }


        private const string UrlMode = "http://pinglun.yixun.com/json1.php?mod=reviews&act=getreviews&jsontype=str&pid={0}&type=allreview&page={1}"; //一般的品论
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


            if (!json.Contains("user_id"))
                return null;



            if (json.Contains("\"comments\":[],") || !json.Contains("comments"))
            {
                json = WordCenter.DecodeUnicode(json);
                var reglist = RegGroupCollection(json, "\"id\":(?<x>.*?)\\}");
                if (reglist == null)
                    return null;
                return (from Match item in reglist select GetCommentNode(item.Groups["x"].Value)).Where(c => c != null).ToList();
                
            }

            JObject obj = JObject.Parse(json);
            if (obj["comments"] == null)
                return null;
            JArray list = (JArray) obj["comments"];

            return list.Select(GetCommentNode).Where(c => c != null).ToList();

        }

        private CommentInfo GetCommentNode(string item)
        {
            string content = RegGroupsX<string>(item, "\"content\":\"(?<x>.*?)\"");
            content = CommentBase.FiterContent(content, 10);
            if (string.IsNullOrEmpty(content))
                return null;
            CommentInfo comment = new CommentInfo();
            string userName = RegGroupsX<string>(item, "\"user_name\":\"(?<x>.*?)\"");
            comment.UserName = string.IsNullOrEmpty(userName) ? "匿名用户" : userName;
            comment.Score = RegGroupsX<int>(item, "\"star\":(?<x>\\d+)");
            
            comment.Content = content;
            int timespan = RegGroupsX<int>(item, "\"create_time\":(?<x>\\d+),");
            if (timespan > 0)
            {
                comment.SendTime = getSendDate(timespan);
            }
            if (item.Contains("picture_urls") && !item.Contains("\"picture_urls\":[]"))
            {
                string tempimg = RegGroupsX<string>(item, "\"picture_urls\":\\[(?<x>.*?)\\]");

                if (string.IsNullOrEmpty(tempimg)) return comment;
                var imgs = tempimg.Replace("\\", "").Replace("\"", "").Split(',');
                StringBuilder imglist = new StringBuilder();
                foreach (string img in imgs)
                {
                    imglist.AppendFormat(img.Contains("http") ? "{0}," : "http://img10.360buyimg.com/test/s80x80_{0},", img);
                }
                comment.ComSmallImg = imglist.ToString();
                comment.CommBigImg = comment.ComSmallImg.Replace("80x80", "340x3400");
            }
            return comment;
        }

        private CommentInfo GetCommentNode(JToken item)
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
                StringBuilder bigImags = new StringBuilder();
                if (item["images"] != null)
                {
                    JArray images = (JArray)item["images"];
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
                            bigImags.Append(imglist[i].Groups["x"].Value);
                            bigImags.Append(";");
                        }
                    }

                }
                CommentInfo info = new CommentInfo
                {
                    UserName = item["nickname"].ToString(),
                    ComSmallImg = imags.ToString(),
                    CommBigImg = bigImags.ToString(),
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

        private DateTime getSendDate(int dbTime)
        {
            DateTime result = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            result = result.AddSeconds(dbTime);
            //返回的是格林尼治时间 零时区 北京时间为东八区 所以要加上八个小时
            result = result.AddHours(8);
            //格式化的时候 一定要去掉秒 不然比较的时候 网页上取到的部分是不带秒的 造成一直判断成功 重复入库
            return result;
        }

        public List<CommentInfo> GetAllComments(string itemUrl)
        {
            throw new NotImplementedException();
        }
    }
}
