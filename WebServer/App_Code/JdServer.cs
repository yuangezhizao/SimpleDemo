using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using BLL;
using Commons;
using Mode;
using Newtonsoft.Json.Linq;

namespace WebServer
{
    public class JdServer
    {

        public static JdJosApi JosApi { get; set; }

        public  JdServer()
        {
            if(JosApi == null)
            {
                JosApi = new JdJosApiBll().GetJosApi(1);
            }
        }

        //private const string authUrl = "https://oauth.jd.com/oauth/authorize?";
        public string GetSingleCode(string searchinfo)
        {
            searchinfo = searchinfo.Trim();
            if (string.IsNullOrEmpty(searchinfo))
                return "";

            string skuid ="";
            string result = "";
            if (!string.IsNullOrEmpty(searchinfo) && searchinfo.Length > 5 && searchinfo.Length < 20)
            {
                float tempid;
                if (float.TryParse(searchinfo, out tempid))
                {
                    skuid = searchinfo;
                }
            }
            else
            {

                string id = Regex.Match(searchinfo, "(?<x>\\d{6,15})", RegexOptions.Singleline).Groups["x"].Value;
                if(!string.IsNullOrEmpty(id) && id.Length > 5 && id.Length < 20)
                    skuid = id;

            }
            if (string.IsNullOrEmpty(skuid) || skuid.Length < 5 || skuid.Length > 20)
                return "";


            if (JosApi == null)
            {
                JosApi = new JdJosApiBll().GetJosApi(1);
            }
            if (JosApi == null)
            {
                return "";
            }
            string param = "{\"promotionType\":\"7\",\"materialId\":\"https://item.jd.com/"+ skuid + ".html\",\"unionId\":\"50814\",\"subUnionId\":\"\",\"siteSize\":\"\",\"siteId\":\"\",\"channel\":\"PC\",\"webId\":\"534656795\",\"extendId\":\"\",\"ext1\":\"\"}";
            string url =string.Format("https://api.jd.com/routerjson?v=2.0&method=jingdong.service.promotion.getcode&app_key={0}&access_token={1}&360buy_param_json={2}&timestamp={3}", JosApi.AppKey, JosApi.AccessToken, param,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var request = new WebClient();
            var pageData = request.DownloadData(url);
            string pageHtml = Encoding.UTF8.GetString(pageData).Replace("\\","");
            if (pageHtml.Contains("获取代码成功"))
            {
                result = Regex.Match(pageHtml, "\"url\":\"(?<x>.*?)\"", RegexOptions.Singleline).Groups["x"].Value;
                return result;
            }
            if (pageHtml.Contains("商品不在推广中"))
            {
                return "商品不在推广中";
            }
            LogServer.WriteLog("url:" + url + "\t" + pageHtml, "flError");
            return "";
       
        }

        public static bool  GetAccessToken(string code)
        {
            string url = "https://oauth.jd.com/oauth/token?grant_type=authorization_code&client_id="+ JdApiConfig.AppKey + "&client_secret=" + JdApiConfig.AppSecret + "&scope=read&redirect_uri=" + JdApiConfig.BackApiUrl + "&code="+ code ;

            var request = new WebClient();
            var pageData = request.DownloadData(url);
            string pageHtml = Encoding.Default.GetString(pageData);
            LogServer.WriteLog(pageHtml);

            try
            {
                if(!pageHtml.Contains("access_token"))
                    return false;

                JToken json = JToken.Parse(pageHtml);
                string uid = json["uid"].Value<string>();
                string nikename = json["user_nick"].Value<string>();
                double expiresIn = json["expires_in"].Value<double>();
                string accessToken = json["access_token"].Value<string>();
                double time = json["time"].Value<double>();

                var accessTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)).AddMilliseconds(time);
                var expiresTime = accessTime.AddSeconds(expiresIn);
                JdJosApi item = new JdJosApi { AppKey = JdApiConfig.AppKey,AppSecret = JdApiConfig.AppSecret,BackApiUrl = JdApiConfig.BackApiUrl,UserId = uid,UserNick = nikename ,ExpiresTime = expiresTime ,AccessTime = accessTime  ,AccessToken = accessToken};
                new JdJosApiBll().AddJosApi(item);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex);
            }

            return true;

        }

        public string GetJdCode()
        {
            string url = JdApiConfig.AuthUrl+ "response_type=code&client_id="+ JdApiConfig.AppKey + "&redirect_uri=" + JdApiConfig.BackApiUrl;
            return url;
        }

        //https://api.jd.com/routerjson?360buy_param_json={"end_date":"2013-12-01 00:00:00","order_state":"WAIT_SELLER_STOCK_OUT,WAIT_GOODS_RECEIVE_CONFIRM","page":"1","page_size":"20","start_date":"2013-05-01 00:00:00"}&access_token=12345678-b0e1-4d0c-9d10-a998d9597d75&app_key=123456780233FA31AD94AA59CFA65305&method=360buy.order.search&v=2.0

        private string baserequest(string method,string ClientParam)
        {
            //https://api.jd.com/routerjson?v=2.0&method=jingdong.service.promotion.appReport&app_key=&360buy_param_json={"time":"","siteKey":"","ext1":"","ext2":"","ext3":""}&timestamp=2016-11-21 16:35:38&sign=CA9266000B88F55CA1162625D0031AAA
            var param = "method=" + method+ "&access_token=&app_key" + JdApiConfig.AppKey+ "&sign="+ JdApiConfig.AppSecret;
          

            //IJdClient client = new DefaultJdClient(url, appkey, appsecret);

            //ServicePromotionAppReportRequest req = new ServicePromotionAppReportRequest();

            //req.time = "jingdong"; req.siteKey = "jingdong"; req.ext1 = "jingdong"; req.ext2 = "jingdong"; req.ext3 = "jingdong";

            //ServicePromotionAppReportResponse response = client.Execute(req, token, DateTime.Now.ToLocalTime());

            return "";
        }
    }
}