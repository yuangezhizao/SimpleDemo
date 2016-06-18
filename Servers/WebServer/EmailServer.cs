using System.Linq;
using System.Text.RegularExpressions;
using Mode;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Servers
{
   // [Authenticate]
    public class EmailServer : Service
    {
        private const RegexOptions Ro = RegexOptions.Singleline | RegexOptions.IgnoreCase;

        /// <summary>
        /// 邮件发送帐号(多个，防止单个帐号发送邮件频繁被封)
        /// </summary>
        private static readonly List<Tuple<string, string>> SendMailAccount = new List<Tuple<string, string>> { 
            (new Tuple<string,string>("mmbupdate@163.com","188update")),
            (new Tuple<string,string>("mmbupdate01@163.com","188update")),
            (new Tuple<string,string>("mmbupdate02@163.com","188update")),
            (new Tuple<string,string>("mmbupdate03@163.com","188update")),
            (new Tuple<string,string>("mmbupdate04@163.com","188update")),
            (new Tuple<string,string>("mmbupdate05@163.com","188update")),
            (new Tuple<string,string>("mmbupdate06@163.com","188update"))
            
        };

        static readonly Random Rand = new Random();

        static readonly object LockMailForsend = new object();

        /// <summary>
        /// 发送异常到邮箱
        /// </summary>
        /// <param name="body">邮件内容（html）</param>
        /// <param name="subject">邮件标题（通过邮件标题来限制发送频率）</param>

        /// <param name="toaddress">接收邮件的地址</param>
        public static bool SendMail(string body, string subject, string[] toaddress = null)
        {
            lock (LockMailForsend)
            {
                if (toaddress == null || toaddress.Length == 0)
                {
                    return false;
                }
       
                MailMessage msg = new MailMessage();
                foreach (var item in toaddress)
                {
                    msg.To.Add(item);
                }
                //随机筛选一组用户
                int index = Rand.Next(0, SendMailAccount.Count);
                string username = SendMailAccount[index].Item1;
                string password = SendMailAccount[index].Item2;
                //发件人信息
                msg.From = new MailAddress(username, "慢慢买更新工具", Encoding.UTF8);
                msg.Subject = subject; //邮件标题
                msg.SubjectEncoding = Encoding.UTF8; //标题编码
                msg.Body = body; //邮件主体
                msg.BodyEncoding = Encoding.UTF8;
                msg.IsBodyHtml = true; //是否HTML
                msg.Priority = MailPriority.High;
                SmtpClient client = new SmtpClient
                {
                    Credentials = new System.Net.NetworkCredential(username, password),
                    Port = 25,
                    Host = "smtp.163.com",
                    EnableSsl = true
                };
                try
                {
                    client.Send(msg);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    msg.Dispose();
                    client.Dispose();
                }
            }
        }

        public object Any(EmailInfo request)
        {
            //var session = SessionAs<CustomSession>();
            //if (!session.IsAuthenticated)
            //    return new EmailResponse { msgCode:1002, msgContent: \"Auth Error\"" };
            if (string.IsNullOrEmpty(request.Body))
                return new EmailResponse { MsgCode = 1003,MsgContent="empty of body" };
            if (string.IsNullOrEmpty(request.Subject))
                return new EmailResponse { MsgCode=1004, MsgContent="empty of Subject" };
            if (string.IsNullOrEmpty(request.sendToName))
                return new EmailResponse { MsgCode=1005, MsgContent="empty of To" };
    
            string[] toaddress = request.sendToName.Contains(";") ? request.sendToName.Split(';') : new[] { request.sendToName };
          
            if (toaddress.Any(t => ! Regex.IsMatch(t,@"^.+\@(\[?)[a-zA-Z0-9\-\.]+\.([a-zA-Z]{2,3}|[0-9]{1,3})(\]?)$", Ro)))
            {
                return new EmailResponse { MsgCode=1006, MsgContent="send to name error" };
            }
            if(SendMail(request.Body, request.Subject, toaddress))
                return new EmailResponse {MsgCode=1001, MsgContent= "ok"};
            return new EmailResponse { MsgCode=-1, MsgContent= "system error" };
        }
    }

    public class EmailResponse
    {
        public int MsgCode {   get; set; }
        public string MsgContent {  get; set; }
      
    }
}
