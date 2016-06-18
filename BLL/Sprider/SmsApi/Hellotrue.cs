using System;
using System.Threading;
using Commons;
using DataBase;
using Mode;

namespace BLL.Sprider.SmsApi
{
    public class Hellotrue : ISmsApi
    {
        public static string Token;
        public  SmsManger smsManger { get; set; }
        public Hellotrue(SmsManger sms)
        {
            smsManger = new SmsManger
            {
                ApiUrl = sms.ApiUrl,
                UserName = sms.UserName,
                Pwd = sms.Pwd

            };
            if (string.IsNullOrEmpty(Token))
            {
                GetToken();
            }
            //smsManger.Token = Token;


        }

        private void request(SmsManger mgr)
        {
            string url = string.Format("{0}?action={1}&token={2}&{3}", mgr.ApiUrl,mgr.Action,Token, mgr.Param??"");
            HtmlAnalysis request = new HtmlAnalysis();
            mgr.Message = request.HttpRequest(url);
            var his = new APIRequstHistroy
            {
                Summary = mgr.Action,
                RequestUrl = string.Format("{0}?action={1}&{2}", mgr.ApiUrl, mgr.Action,mgr.Param ?? ""),
                Result = mgr.Message,
                CreateTime = DateTime.Now
            };
            new ApiRequstHistroyDB().AddApiHistory(his);

        }
        /// <summary>
        ///  获取验证码
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="catid">项目编号</param>
        /// <returns></returns>
        public string GetValidateMsg(string phone, string catid)
        {
            int error = 0;
            do
            {
                Thread.Sleep(10000);
                smsManger.Action = "getMessage";
                smsManger.Param = string.Format("sid={0}&phone={1}", catid, phone);
                request(smsManger);
                var page = smsManger.Message;
                if (page.Contains("还没有接收到短信"))
                {
                    error++;
                    continue;
                }
             
                return page;
            } while (error<5);
            LogServer.WriteLog("还没有接收到短信", "SmsServer");
            return "";
        }

   


        private void GetToken()
        {
            smsManger.Action = "loginIn";
            smsManger.Param = string.Format("&name={0}&password={1}", smsManger.UserName,smsManger.Pwd);
            request(smsManger);
            Token = smsManger.Message.Replace("1|", "");
        }

        public string GetPhoneNum(string catid)
        {

            smsManger.Action = "getPhone";
            smsManger.Param = string.Format("sid={0}", catid);
            request(smsManger);
           return smsManger.Message.Replace("1|", "");
        }

        public string GetPhoneNum(string catid,string phone)
        {

            smsManger.Action = "getPhone";
            smsManger.Param = string.Format("sid={0}&phone={1}", catid,phone);
            request(smsManger);
            return smsManger.Message.Replace("1|", "");
        }

        public string GetSentMessageStatus(string phone,string catid)
        {
            smsManger.Action = "getSentMessageStatus";
            smsManger.Param = string.Format("sid={0}&phone={1}", catid, phone);
            request(smsManger);
            return smsManger.Message;
        }

        /// <summary>
        /// 加入黑名单
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="catid"></param>
        /// <returns></returns>
        public string AddBlacklist(string phone,string catid)
        {
            smsManger.Action = "addBlacklist";
            smsManger.Param = string.Format("sid={0}&phone={1}", catid, phone);
            request(smsManger);
            return smsManger.Message;
        }
        /// <summary>
        /// 释放指定手机号
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="catid"></param>
        /// <returns></returns>
        public string CancelRecv(string phone, string catid)
        {
            smsManger.Action = "cancelRecv";
            smsManger.Param = string.Format("sid={0}&phone={1}", catid, phone);
            request(smsManger);
            return smsManger.Message;
        }

        /// <summary>
        ///获取账户信息  1|余额|等级|批量取号数|用户类型
        /// </summary>
        /// <returns></returns>
        public string getSummary()
        {
            smsManger.Action = "getSummary";

            request(smsManger);
            return smsManger.Message;
        }

    }
}
