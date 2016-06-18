using System;
using System.Text.RegularExpressions;
using Commons;
using Mode;
using SpriderProxy;
using System.Collections.Generic;
using DataBase;
namespace BLL.WeiBo
{
    

    /// <summary>
    /// 腾讯微博
    /// </summary>
    public class TengXunBll : IWeiboBll
    {

        //  private const string client_secret = "4a54f1862453ff299acebaa86c8fd9b1";
        //  private const string refresh_token = "3247a1485a6dfef02251c6eda4693b42";
        //  private const string openkey = "5373CB19AA4C11FE7D7FABB908A19CA4";
        private const string ClientId = "801524916";
        private const string AccessToken = "cbaf8341315e35a5b63fe3ec3908bc7c";
        private const string Openid = "61158ee2d53df7a34817c74496956f47";

        private const string Baseparam = "&access_token=" + AccessToken +
                                         "&oauth_consumer_key=" + ClientId +
                                         "&openid=" + Openid +
                                         "&oauth_version=2.a&clientip=122.245.206.107&format=xml";

        public void write(string contents)
        {
            string paramlist = "content=" + contents + Baseparam;

            string url = "https://open.t.qq.com/api/t/add?" + paramlist;
            const string url1 =
                "http://open.t.qq.com/api/friends/fanslist?format=json&reqnum=20&startindex=0&mode=0&install=0&sex=0&oauth_consumer_key=" +
                ClientId + "&access_token=" + AccessToken + "&openid=" + Openid +
                "&clientip=122.245.206.107&oauth_version=2.a";
            HtmlAnalysis analysis = new HtmlAnalysis { RequestMethod = "POST" };
            analysis.RequestMethod = "GET";
            var cc = analysis.HttpRequest(url1);

            var bb = analysis.HttpRequest(url);



            string result = HtmlAnalysis.HttpRequestFromPost(url, paramlist, "utf-8");

            //const string url1 = "http://open.t.qq.com/api/friends/fanslist?format=json&reqnum=20&startindex=0&mode=0&install=0&sex=0&oauth_consumer_key=" + ClientId + "&access_token=" + AccessToken + "&openid=" + Openid + "&clientip=122.245.206.107&oauth_version=2.a";
            HtmlAnalysis.HttpRequestFromPost(url1, paramlist, "utf-8");

        }

        //趋势/转播热榜
        public void Tophot()
        {
            const string url = " http://open.t.qq.com/api/other/gettopreadd";
            const string paramlist = "&reqnum=300&type=5&country=1&province=33&city=3302" + Baseparam;

            var result = HtmlAnalysis.HttpRequestFromPost(url, paramlist, "utf-8");

        }

        /// <summary>
        /// 根据经纬度的值获取感兴趣的地理位置的相关信息
        /// </summary>
        public void GetPoi()
        {
            //30.050929,121.164877 
            string url = "https://open.t.qq.com/api/lbs/get_poi";

            const string paramlist = "&latitude=30.050929&longitude=121.164877&radius=1000" + Baseparam;

            var result = HtmlAnalysis.HttpRequestFromPost(url, paramlist, "utf-8");
        }
        /// <summary>
        /// 获取我的粉丝列表
        /// </summary>
        public void getMyFrinds()
        {

            string url = " https://open.t.qq.com/api/friends/fanslist";
            const string paramlist = "reqnum=30&startindex=0" + Baseparam;
            url += "?" + paramlist;

            var result = HtmlAnalysis.Gethtmlcode(url);

        }


        /// <summary>
        /// 获取用户的粉丝
        /// </summary>
        /// <param name="key">openid|name</param>
        /// <param name="value"></param>
        public void getUserFrinds(string key, string value)
        {
            string url = " https://open.t.qq.com/api/friends/user_fanslist";

            if (key == "openid")
            {
                url += "?openid=" + value;
            }
            else
                url += "?name=" + value;

            url += "&reqnum=30&startindex=0" + Baseparam;


            var result = HtmlAnalysis.Gethtmlcode(url);
        }
        /// <summary>
        /// 获取我关注的用户信息
        /// </summary>
        public void Getmyidollist()
        {
            string url = " https://open.t.qq.com/api/friends/idollist";
            const string paramlist = "reqnum=30&startindex=0" + Baseparam;
            url += "?" + paramlist;
            var result = HtmlAnalysis.Gethtmlcode(url);
        }

        /// <summary>
        /// 获取用户关注的用户信息
        /// </summary>
        public void GetUseridollist(string key, string value)
        {
            string url = " https://open.t.qq.com/api/friends/user_idollist";

            if (key == "openid")
            {
                url += "?openid=" + value;
            }
            else
                url += "?name=" + value;

            url += "&reqnum=30&startindex=0" + Baseparam;


            var result = HtmlAnalysis.Gethtmlcode(url);

            SaveUseridols(result);


        }


       /// <summary>
       /// 保存当前用户 关注的用户
       /// </summary>
       /// <param name="xmllist"></param>
        public void SaveUseridols(string xmllist)
        {
            if (!xmllist.Contains("<errcode>0</errcode>") || !xmllist.Contains("<msg>ok</msg>") || !xmllist.Contains("<ret>0</ret>"))
            {
                LogServer.WriteLog(xmllist, "TxweiboServer");
                return;
            }
            string userListText = BaseSiteInfo.RegGroupsX<string>(xmllist,"<data>(?<x>.*?)</data>");

            var list = BaseSiteInfo.RegGroupCollection(userListText,"<info>(?<x>.*?)</info>");
            List<WbUser> tblist = new List<WbUser>();
            foreach (Match match in list)
            {
                string item = match.Groups["x"].Value;
                WbUser tb = new WbUser();
                tb.AccessToken = "";
                tb.CityCodeTwb = BaseSiteInfo.RegGroupsX<string>(item, "<city_code>(?<x>.*?)</city_code>");
                tb.CountryCodeTwb = BaseSiteInfo.RegGroupsX<string>(item, "<country_code>(?<x>.*?)</country_code>");
                tb.FansnumTwb = BaseSiteInfo.RegGroupsX<int>(item, "<fansnum>(?<x>.*?)</fansnum>");
                tb.HeadTwb = BaseSiteInfo.RegGroupsX<string>(item, "<head>(?<x>.*?)</head>");
                tb.HttpsHead = BaseSiteInfo.RegGroupsX<string>(item, "<https_head>(?<x>.*?)</https_head>");
                tb.IdolnumTwb = BaseSiteInfo.RegGroupsX<int>(item, "<idolnum>(?<x>.*?)</idolnum>");
                tb.IsFansTwb = BaseSiteInfo.RegGroupsX<string>(item, "<isfans>(?<x>.*?)</isfans>");
                tb.IsidolTwb = BaseSiteInfo.RegGroupsX<string>(item, "<isidol>(?<x>.*?)</isidol>");
                tb.IsrealnameTwb = BaseSiteInfo.RegGroupsX<string>(item, "<IsrealnameTwb>(?<x>.*?)</IsrealnameTwb>");
                tb.IsVip = BaseSiteInfo.RegGroupsX<string>(item, "<isvip>(?<x>.*?)</isvip>");
                tb.LocationTwb = BaseSiteInfo.RegGroupsX<string>(item, "<location>(?<x>.*?)</location>");
                tb.NameTwb = BaseSiteInfo.RegGroupsX<string>(item, "<name>(?<x>.*?)</name>");
                tb.NickTwb = BaseSiteInfo.RegGroupsX<string>(item, "<nick>(?<x>.*?)</nick>");
                tb.OpenidTwb = BaseSiteInfo.RegGroupsX<string>(item, "<openid>(?<x>.*?)</openid>");
                tb.ProvinceCodeTwb = BaseSiteInfo.RegGroupsX<string>(item, "<province_code>(?<x>.*?)</province_code>");
                tb.SexTwb = BaseSiteInfo.RegGroupsX<string>(item, "<sex>(?<x>.*?)</sex>");
                tb.TagTwb = BaseSiteInfo.RegGroupsX<string>(item, "<tag>(?<x>.*?)</tag>");
                tb.FromTwb = BaseSiteInfo.RegGroupsX<string>(item, "<from>(?<x>.*?)</from>");
                tb.IdTwb ="tx_"+ BaseSiteInfo.RegGroupsX<string>(item, "<id>(?<x>.*?)</id>");
                tb.TextTwb = BaseSiteInfo.RegGroupsX<string>(item, "<text>(?<x>.*?)</text>");
                tb.TimestampTwb = BaseSiteInfo.RegGroupsX<string>(item, "<timestamp>(?<x>.*?)</timestamp>");
                tb.IsUsed = true;
                tb.UpdateTime = DateTime.Now;
                tb.CreateDate = DateTime.Now;
                tb.WbType = "tengxun";
                tblist.Add(tb);
            }

            TxWeiboUserDB.AddUserInfo(tblist);
        }

        public void write(string contents, string pic)
        {
            throw new System.NotImplementedException();
        }


        public void getAccessToken(string code)
        {
            throw new System.NotImplementedException();
        }


        public void UsersShow()
        {
            throw new System.NotImplementedException();
        }
    }

  
}
