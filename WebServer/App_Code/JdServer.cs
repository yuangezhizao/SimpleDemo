using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace WebServer
{
    public class JdServer
    {
        private const string authUrl = "https://oauth.jd.com/oauth/authorize?";
        public void GetSingleCode(string skuid)
        {
            string datetime = DateTime.Now.ToString("yy-MMM-dd HH:mm:ss");
            string siteKey = "";
            var result = baserequest("jingdong.service.promotion.appReport", "time="+ datetime+ "&siteKey=");
     
            //jingdong.service.promotion.
        }

        public void Getsdf(string code)
        {
            string url =
                "https://oauth.jd.com/oauth/token?grant_type=authorization_code&client_id="+ JdApiConfig.AppKey + "&redirect_uri = "+ JdApiConfig.BackApiUrl + "& code = "+ code ;

        }


        public string GetJdCode()
        {
            var request = new WebClient();
            string url = JdApiConfig.AuthUrl+ "response_type=code&client_id="+ JdApiConfig.AppKey + "&redirect_uri=" + JdApiConfig.BackApiUrl;
            var pageData = request.DownloadData(url);
            string pageHtml = Encoding.Default.GetString(pageData);
            return pageHtml;
            //App Key：DFA4CF696594735A7094588C8CF344EB
            //App Secret： 172c8813b68f493c934d5de590c4c993

            //App Key：6720FCC04EAB1CCC9C101D4E9F3AC305
            //App Secret： c8e990e8eaee473facde33fdb3db6b2d 隐藏   重置
        }

        //https://api.jd.com/routerjson?360buy_param_json={"end_date":"2013-12-01 00:00:00","order_state":"WAIT_SELLER_STOCK_OUT,WAIT_GOODS_RECEIVE_CONFIRM","page":"1","page_size":"20","start_date":"2013-05-01 00:00:00"}&access_token=12345678-b0e1-4d0c-9d10-a998d9597d75&app_key=123456780233FA31AD94AA59CFA65305&method=360buy.order.search&v=2.0

        private string baserequest(string method,string ClientParam)
        {
            var param = "method=" + method+ "&access_token=&app_key" + JdApiConfig.AppKey+ "&sign="+ JdApiConfig.AppSecret;

            return "";
        }
    }
}