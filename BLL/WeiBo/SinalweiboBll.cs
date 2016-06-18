using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Commons;
using DataBase;
using Mode;

namespace BLL.WeiBo
{
    /// <summary>
    /// 新浪微博
    /// </summary>
    public class SinalweiboBll : IWeiboBll
    {
        // setup1: https://api.weibo.com/oauth2/authorize?client_id=2046894446&redirect_uri=http://www.chinaqnx.cn
        // setup2:getAccessToken()
        //http://open.weibo.com/tools/console  测试地址  

        public static string Appkey;
        public static string AppSecret;
        public static string RedirectUri;
        public string[] RePostContents = { "不错", "准备入手", "赞","手慢无" };
        static Random rdm = new Random();

        public SinalweiboBll()
        {
            Appkey = ConfigurationManager.AppSettings["SinawbAppkey"];
            AppSecret = ConfigurationManager.AppSettings["SinawbAppSecret"];
            RedirectUri = ConfigurationManager.AppSettings["SinawbReduri"];
        }

     
        /// <summary>
        ///  发表一则微博
        /// </summary>
        /// <param name="contents"></param>
        public void write(string contents)
        {
            TxWeiboUserDB ubd = new TxWeiboUserDB();
            var userList = ubd.LoadAllUser().FindAll(p => p.Grade > 9 && p.WbType == "sina");
            for (int i = 0; i < userList.Count; i++)
            {
                var timespan = DateTime.Now - userList[i].UpdateTime;
                if (timespan.TotalDays > 7)
                {
                    LogServer.WriteLog("微博授权超过七天失效" + userList[i].NickTwb, "WeiboError");
                    continue;
                }

                int replate = 0;
                do
                {
                    try
                    {
                        var temcominfo = System.Web.HttpUtility.UrlEncode(contents);
                        string postparam = "https://api.weibo.com/2/statuses/update.json?status=" + temcominfo + "&access_token" + userList[i].AccessToken;
                        var sult = postRequest(postparam, userList[i].AccessToken);
                        repost(sult, " @" + userList[i].NickTwb);
                        break;
                    }
                    catch (Exception ex)
                    {
                        LogServer.WriteLog(ex.ToString(), "WeiboError");
                        Thread.Sleep(1000 * 30);
                        replate++;
                    }
                } while (replate < 3);
            }


        }

        public void write(string contents, string picUrl)
        {

            TxWeiboUserDB ubd = new TxWeiboUserDB();
            var userList = ubd.LoadAllUser().FindAll(p => p.Grade > 9 && p.WbType == "sina");
            for (int i = 0; i < userList.Count; i++)
            {
                var timespan = DateTime.Now - userList[i].UpdateTime;
                if (timespan.TotalDays > 7)
                {
                    LogServer.WriteLog("微博授权超过七天失效" + userList[i].NickTwb, "WeiboError");
                    continue;
                }

                int replate = 0;
                do
                {
                    try
                    {
                        int temp = i;
                        string url = "https://upload.api.weibo.com/2/statuses/upload.json";

                        var temcominfo = System.Web.HttpUtility.UrlEncode(contents);
                        if (string.IsNullOrEmpty(temcominfo))
                            return;
                        NameValueCollection postParam = new NameValueCollection();
                        postParam.Add("status", temcominfo);
                        postParam.Add("access_token", userList[i].AccessToken);
                        var res = HtmlAnalysis.HttpPostData(url, picUrl, postParam, userList[i].AccessToken);
                        if (res == "")
                        {
                            LogServer.WriteLog("发送失败：url：\t" + url + "\t" + postParam+"\t"+ userList[temp].NickTwb, "WeiboError");
                        }
                        else
                        Task.Factory.StartNew(() => repost(res, " @" + userList[temp].NickTwb));

                        break;
                    }
                    catch (Exception ex)
                    {
                        LogServer.WriteLog(ex.ToString(), "WeiboError");
                        Thread.Sleep(1000*30);
                        replate++;
                    }
                } while (replate < 3);
            }

        }
       
        /// <summary>
        /// 转发
        /// </summary>
        /// <param name="wb"></param>
        public void repost(string wb,string frends)
        {
            TxWeiboUserDB ubd = new TxWeiboUserDB();
            var userList = ubd.LoadAllUser().FindAll(p=>p.Grade==0&& p.WbType=="sina");

            string url = "https://api.weibo.com/2/statuses/repost.json";
            string id = Regex.Match(wb, "\"id\":(?<x>\\d+),", RegexOptions.Singleline).Groups["x"].Value;
            for (int i = 0; i < userList.Count; i++)
            {
               
                var timespan = DateTime.Now - userList[i].UpdateTime;
                if (timespan.TotalDays > 7)
                {
                    LogServer.WriteLog("转发微博授权超过七天失效" + userList[i].NickTwb, "WeiboError");
                    continue;
                }

                Thread.Sleep(1000*rdm.Next(10, 60));
                string tempcon = RePostContents[rdm.Next(0, RePostContents.Length - 1)] + frends;
                tempcon = System.Web.HttpUtility.UrlEncode(tempcon);
                
                string param = "?is_comment=3&status=" + tempcon + "&id=" + id + "&access_token=" +
                               userList[i].AccessToken;
                var res = postRequest(url + param, userList[i].AccessToken);

                if (res == "")
                {
                    LogServer.WriteLog("转发失败：url：\t" + url + "\t" + param + "\t" + userList[i].NickTwb, "WeiboError");
                }
                else
                id = Regex.Match(res, "\"id\":(?<x>\\d+),", RegexOptions.Singleline).Groups["x"].Value;
            }

            //HtmlAnalysis.HttpPostData(url, "pic", url, postParam, AccessToken);

        }
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="code"></param>
        public void getAccessToken(string code)
        {
            string url = "https://api.weibo.com/oauth2/access_token?client_id=" + Appkey + "&client_secret="+AppSecret+"&grant_type=authorization_code&code=" + code + "&redirect_uri=" + RedirectUri;
            string result = postRequest(url,"");
            string accessToken = Regex.Match(result, "\"access_token\":\"(?<x>.*?)\"").Groups["x"].Value;
            string uid = Regex.Match(result, "\"uid\":\"(?<x>.*?)\"").Groups["x"].Value;
            UsersSave(accessToken, uid);


        }
        /// <summary>
        /// 保存授权用户信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="uid"></param>
        private void UsersSave(string accessToken, string uid)
        {
            string url = "https://api.weibo.com/2/users/show.json?access_token=" + accessToken + "&uid=" + uid;
            HtmlAnalysis hserver = new HtmlAnalysis();
            string result = hserver.HttpRequest(url);
            var user = ServiceStack.Text.JsonSerializer.DeserializeFromString<Dictionary<string, string>>(result);
            if (user == null)
                return;
            string sex = user["gender"];
            if (sex == "m")
                sex = "1";
           else if (sex == "f")
               sex = "2";
           else
               sex = "0";
            WbUser tb = new WbUser
            {
                IdTwb = "sl_" + user["id"],
                AccessToken=accessToken,
                CityCodeTwb = user["city"],
                ProvinceCodeTwb = user["province"],
                CountryCodeTwb="cn",
                NickTwb = user["screen_name"],
                SexTwb = sex,
                IdolnumTwb = int.Parse(user["friends_count"]),
                NameTwb = user["name"],
                Allowactmsg=user["name"]=="true",
                HeadTwb = user["profile_image_url"],
                HttpsHead = user["profile_url"],
                FromTwb = "",
                OpenidTwb = user["id"],
                IsFansTwb="",
                IsidolTwb="",
                IsrealnameTwb = "",
                IsVip = user["verified"],
                Createdat = user["created_at"],
                Description=user["description"],
                FansnumTwb=int.Parse(user["followers_count"]),
                IsUsed=true,
                WbType = "sina",
                Grade = user["id"]=="3170063202"?10: 0,
                UpdateTime=DateTime.Now,
                CreateDate=DateTime.Now
            };
          
           new TxWeiboUserDB().SaveUserInfo(tb);
        }



        ////趋势/转播热榜
        //public void Tophot()
        //{
        //    string url = " https://api.weibo.com/oauth2/access_token";
        //    string result = HtmlAnalysis.HttpRequestFromPost(url, Baseparam + "&grant_type=ec2ce81b35e561f559025bf1c82b028c", "utf-8");
            
        //}

        private string postRequest(string url, string AccessToken)
        {
            HtmlAnalysis al = new HtmlAnalysis
            {
                RequestContentType = "application/x-www-form-urlencoded",
                RequestAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
            };
            al.RanAgent = false;
            al.HasCookies = false;
            al.RequestMethod = "POST";
            if (AccessToken != "")
            {
                al.Headers = new Dictionary<string, string>
                {
                    {"Accept-Language", "zh-cn,en-us;"},
                    {"Accept-Encoding", "gzip, deflate"},
                    {"Authorization", "OAuth2 " + AccessToken}
                };
            }
            return al.HttpRequest(url);
        }






    }


}
