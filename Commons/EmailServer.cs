using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Commons
{
   public class EmailServer
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
              //  int index = Rand.Next(0, SendMailAccount.Count);
                string username = "chennysnow@sina.com";// SendMailAccount[index].Item1;
                string password = "chenny@62415109"; //SendMailAccount[index].Item2;
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
                    Host = "smtp.sina.com",
                    EnableSsl = true
                };
                try
                {
                    client.Send(msg);
                    return true;
                }
                catch(Exception ex)
                {
                    LogServer.WriteLog(ex.Message,"EmailServer");
                    return false;
                }
                finally
                {
                    msg.Dispose();
                    client.Dispose();
                }
            }
        }
    }
}
