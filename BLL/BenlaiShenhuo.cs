using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Commons;
using DataBase;
using Mode;

namespace BLL
{
    public class BenlaiShenhuo
    {
        protected RegexOptions ro = RegexOptions.Singleline | RegexOptions.IgnoreCase;
        public string RegionUser()
        {
     
            var smsmanger = new SiteFactory().SmsApiManager;
            string catid = "1773";
            string yaoqing = "YKQcw";
            string phone = smsmanger.GetPhoneNum(catid); 
            HtmlAnalysis request1 = new HtmlAnalysis();
            request1.RequestContentType = "application/x-www-form-urlencoded";
            request1.Headers.Add("Cookie", "JSESSIONID=A96903EAC0B275902F08953A3C87C808; RecommendCityStatus=1; burl=https%3a%2f%2fm%2ebenlai%2ecom%2factivity%2fpullNewReceive%3fcallback%3d0%26showtype%3d4%26invitatorCode%3dqVL4Y%26referSysNo%3dDECAA91A6206EDD4; ASP.NET_SessionId=rizc5rofdjadxcytxo3dhizb; uuk=657efcf3-a63d-4e3c-9718-f4d809c92353; userGuid=aafe002b-730b-4c90-8aa3-cad731bb4db720160618014311; _jzqckmp_v2=1/; _jzqckmp=1/; AppCity=*e5*ae*81*e6*b3*a2; curRecommendation=%e5%ae%81%e6%b3%a2; _pk_id.7.2b60=b6589fc6ab0dc82c.1466228584.1.1466228589.1466228584.; _pk_ses.7.2b60=*; recentcNo=\"135, \"; DeliverySysNo=135; WebSiteSysNo=3; CityPY=nb; city=*e5*ae*81*e6*b3*a2; hsc=1; ProvinceSysNo=28; localcity=135; backUrl=https%253A%252F%252Fm.benlai.com%252Factivity%252FpullNewReceive%253Fcallback%253D0%2526showtype%253D4%2526invitatorCode%253DqVL4Y%2526referSysNo%253DDECAA91A6206EDD4; _pk_id.9.2b60=b6589fc6ab0dc82c.1466228593.1.1466228593.1466228593.; _pk_ses.9.2b60=*; _qzja=1.430535276.1466228583963.1466228583963.1466228583963.1466228593005.1466228593012.https%253A%252F%252Fm_benlai_com.0.0.5.1; _qzjb=1.1466228583963.5.0.0.0; _qzjc=1; _qzjto=5.1.0; Hm_lvt_9a7d729a11da2966935bcb2908a98794=1465949409,1465953691,1466042258,1466121250; Hm_lpvt_9a7d729a11da2966935bcb2908a98794=1466228593; Hm_lvt_7feabb06873cfd158820492f754cc70b=1465949409,1465953691,1466042258,1466121250; Hm_lpvt_7feabb06873cfd158820492f754cc70b=1466228593; CSESSIONID=A96903EAC0B275902F08953A3C87C808; source=2");
            request1.RequestUserAgent =
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.87 Safari/537.36";
            request1.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request1.Headers.Add("Origin", "https://m.benlai.com");
            request1.RequestReferer = "https://m.benlai.com/showReg?comeFromApp=0";
           string url = "https://m.benlai.com/regPhoneVry?phoneNumber="+ phone;
            request1.RequestMethod = "post";
            var page = request1.HttpRequest(url);
            if (!page.Contains("'短信验证码生成成功"))
            {
                LogServer.WriteLog(page);
                return "";
            }
            var msm = smsmanger.GetValidateMsg(phone,catid);
            string code = Regex.Match(msm, "注册验证码为 (?<x>\\d+) \\(本来生活绝不会索取此验证码", ro).Groups["x"].Value;
            if (string.IsNullOrEmpty(code))
            {
                LogServer.WriteLog(code);
                return "";
            }

   
            url = "https://m.benlai.com/activity/receiveAndReg?referSysNo=DECAA91A6206EDD4&invitatorCode=qVL4Y&cellphone=" + phone + "&code="+ code + "&unionId=";
            page = request1.HttpRequest(url);
            LogServer.WriteLog(url + "\t" + page);
            LogServer.WriteLog(phone+"\t"+code,"benlaishenhuo");

            var shy = new SmsHistory
            {
                SmsServer = smsmanger.smsManger.ServerName,
                SmsUserName = smsmanger.smsManger.UserName,
                Phone = phone,
                CaseName = "本来生活168活动",
                Summary = "创建帐号并获取满200减40的券",
                MessageInfo = string.Format("[\"phone\":{0},\"pwd\":{1}]",phone,code),
                CreateDate = DateTime.Now
            };
            new SmsHistoryDB().AddSmsHistory(shy);
            return "1";
            
            url = "https://m.benlai.com/registerPhone?regPhoneNum=" + phone+ "&regPhVerify=" + code+ "&invitationCode="+ yaoqing;
            page = request1.HttpRequest(url);
            string tempurl1 = "https://m.benlai.com/registerByPh?customerID=" + phone + "&invitationCode="+ yaoqing + "&customerPwd=62415109";
            string secc = request1.HttpRequest(tempurl1);
            LogServer.WriteLog(url+"\t"+page);

        }
    }
}
