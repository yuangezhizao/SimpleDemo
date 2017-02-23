using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using DotRas;
using MHttpHelper;

namespace ChangeIPTool
{
    class Server
    {
        List<string> historyIps = new List<string>();
        private int IPLoopCount = 3; //允许重复次数  拨号3次还是重复的ip 
        private int DialFaildSleepTime = 30000;

        public static string OldIpAddress { get; set; }
        public static string CurrentIpAddress { get; set; }


        public void SendMsgtoServer()
        {
            //GetWebClient
            if (Process.GetProcessesByName("GetWebClient").Any())
            {
                new WebClient().DownloadDataAsync(new Uri("http://service.manmanbuy.com/mmb.ashx?method=ipduankai"));
            }
 
        }

        /// <summary>
        /// 宽带ip转换
        /// </summary>
        /// <param name="kdlj">宽带连接名称 如：本地连接，宽带连接</param>
        /// <param name="userName">帐号</param>
        /// <param name="pwd">密码</param>
        public void ChangeIp(string kdlj, string userName, string pwd)
        {

            SendMsgtoServer();

            //hx.Mset.IsChangeIp = hx.Mset.IsChangeIp;
            LogServer.WriteLog("开始准备更换IP", "changeIp");

            HANDUPCON:
            string oldIpAddress;
            RasConnection oldConn;
            GetIpAddress(out oldIpAddress, out oldConn);
            string entryName = "";
            if (oldConn != null)
            {

                try
                {
                    entryName = oldConn.EntryName;
                    RasIPInfo ipAddresses = (RasIPInfo) oldConn.GetProjectionInfo(RasProjectionType.IP);
                    string oldIp = ipAddresses.IPAddress.ToString();
                    LogServer.WriteLog("现在名称:" + entryName + "IP是" + oldIp, "changeIp");
                    OldIpAddress = oldIp;
                    LogServer.WriteLog("开始挂断", "changeIp");
                    oldConn.HangUp(10*1000);
                    //Thread.Sleep(hx.Mset.RasHangUpSleepTime);
                    if (RasConnection.GetActiveConnectionById(oldConn.EntryId) != null)
                    {
                        LogServer.WriteLog("结束挂断失败，重新挂断", "changeIp");
                        goto HANDUPCON;
                    }
                    oldConn = null;
                    LogServer.WriteLog("结束挂断", "changeIp");
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog("宽带连接挂断失败，" + ex.Message, "changeIp");
                }
            }
            int error = 0;
            do
            {
                try
                {
                    //  var dt = SqliteHelper.GetDataTable("select * from sys_config");
                    RasDialer rs = new RasDialer();
                    if (entryName == "")
                    {
                        entryName = kdlj; // dt.Rows[0]["SC_NetEntryName"].ToString();
                    }
                    rs.EntryName = entryName;
                    rs.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
                    rs.Credentials = new NetworkCredential(userName, pwd);
                    rs.Dial();
                    rs.Dispose();
                    LogServer.WriteLog("开始重新拨号", "changeIp");
                    break;
                }
                catch (Exception ex)
                {
                    error++;
                    LogServer.WriteLog("宽带重新拨号失败" + ex.Message, "changeIp");
                    Thread.Sleep(30000);
                }
            } while (error < 5);
            if (error >= 5)
            {
                LogServer.WriteLog("宽带重新拨号5次数失败", "changeIp");
                return;
            }

            if (oldConn != null)
            {
                string ipAddresses;
                GetIpAddress(out ipAddresses, out oldConn);
                if (oldIpAddress == ipAddresses)
                {
                    //addlog("IP和上次重复，重新拨号");
                    LogServer.WriteLog("IP和上次重复，重新拨号", "changeIp");
                    Thread.Sleep(DialFaildSleepTime);
                    goto HANDUPCON;
                }
                else
                {
                    if (historyIps.Contains(ipAddresses))
                    {
                        LogServer.WriteLog("IP和前" + IPLoopCount + "次重复，重新拨号", "changeIp");
                        Thread.Sleep(DialFaildSleepTime);
                        goto HANDUPCON;
                    }
                    if (historyIps.Count >= IPLoopCount)
                    {
                        historyIps.RemoveAt(0);
                        historyIps.Add(ipAddresses);
                    }
                    historyIps.Add(ipAddresses);
                }
                CurrentIpAddress = ipAddresses;
                //addlog("现在的IP是" + ipAddresses);


            }
            //main.pppoeact = true;
            LogServer.WriteLog("更换成功原ip:"+oldIpAddress+"当前ip:"+ CurrentIpAddress, "changeIp");
            //addlog("更换成功.. ");
        }

        public void GetIpAddress(out string ipAddress, out RasConnection oldConn)
        {
            ipAddress = "";
            var conns = RasConnection.GetActiveConnections();
            oldConn = conns.FirstOrDefault(conn => conn.Device.DeviceType.ToString().ToLower() == "pppoe");
            if (oldConn != null)
            {
                try
                {
                    RasIPInfo ipAddresses = (RasIPInfo) oldConn.GetProjectionInfo(RasProjectionType.IP);
                    ipAddress = ipAddresses.IPAddress.ToString();
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "changeIp");
                }
            }
        }

        /// <summary>
        /// 路由器拨号转换ip
        /// </summary>
        /// <param name="rouseurl"></param>
        /// <param name="rouseuname"></param>
        /// <param name="rouseupwd"></param>
        public void Roustreconnect(string rouseurl, string rouseuname, string rouseupwd)
        {

            HttpItem item = new HttpItem();
            HttpHelper httper = new HttpHelper();

            try
            {
                string post =
                    "{\"network\":{\"change_wan_status\":{\"proto\":\"pppoe\",\"operate\":\"disconnect\"}},\"method\":\"do \"}\"";
                string posturl = "http://192.168.6.1/stok=a812351afe23c79530df897ec180/ds";
                item.Accept = "application/json, text/javascript, */*; q=0.01";
                item.ContentType = "application/json; charset=UTF-8";
                item.Referer = "http://192.168.6.1/";
                item.URL = posturl;
                item.Method = "POST";
                item.Postdata = post;
                item.ContentType = "application/x-www-form-urlencoded";
                item.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
                item.Header.Add("X-Requested-With", "XMLHttpRequest");
                var stopresult = httper.GetHtml(item);

                LogServer.WriteLog(stopresult.Html, "changeIp");
                return;
                byte[] bytes = Encoding.Default.GetBytes(rouseuname + ":" + rouseupwd);
                string base64str = Convert.ToBase64String(bytes);
                string url = rouseurl.IndexOf("http") >= 0
                    ? rouseurl
                    : ("http://" + rouseurl) + "/userRpm/StatusRpm.htm";
                string urltxt = url + "?Disconnect=" +
                                System.Web.HttpUtility.UrlEncode("断 线", System.Text.Encoding.GetEncoding("gb2312")) +
                                "&wan=1";
                string urltxtc = url + "?Connect=" +
                                 System.Web.HttpUtility.UrlEncode("连 接", System.Text.Encoding.GetEncoding("gb2312")) +
                                 "&wan=1";
                string cookie = "Authorization=Basic%20" + base64str + "; ChgPwdSubTag=";
                item.Referer = urltxt;
                item.ContentType = "textml;charset=gb2312";

                item.URL = urltxt;
                item.Cookie = cookie;

                var page = httper.GetHtml(item);
                Thread.Sleep(1000);
                item.URL = urltxtc;
                var pagelj = httper.GetHtml(item);
                LogServer.WriteLog("断开url:" + urltxt + "\t" + page.Html + "\t链接url:" + urltxtc + "\t" + pagelj,
                    "changeIp");
                LogServer.WriteLog("路由器拨号完成", "changeIp");
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "changeIp");
                //Log4Logger.Error(ex.Source + ":" + ex.Message + "--:" + ex.StackTrace);
            }
        }


        public static DateTime LastUpdate = DateTime.Now;

        public void TimerDoing(string kdlj, string userName, string pwd)
        {
            DateTime logTime = LogServer.ReadLogRowNo("shieldSpider");
            if (LastUpdate < logTime)
            {
                try
                {
                    ChangeIp(kdlj, userName, pwd);
                    LastUpdate = DateTime.Now.AddMinutes(5);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "changeIp");
                }

            }
            else
            {
                //当前是否能够联网
                if (!IsConnectedToInternet())
                {
                    try
                    {
                        LogServer.WriteLog("网络已断开", "changeIp");
                        ChangeIp(kdlj, userName, pwd);
                        LastUpdate = DateTime.Now.AddMinutes(5);
                    }
                    catch (Exception ex)
                    {
                        LogServer.WriteLog(ex, "changeIp");
                    }
                  
                }
                else
                {
                    string url = "http://www.manmanbuy.com/";
                    var page = new HttpHelper().GetHtml(new HttpItem { URL = url }).Html;
                    if (string.IsNullOrEmpty(page)|| page.Length<500)
                    {
                        LogServer.WriteLog("网络已断开,将重新进行连接", "changeIp");
                        new Server().ChangeIp(kdlj, userName, pwd);
                    }
                }
            }

        }

        [DllImport("wininet.dll", EntryPoint = "InternetGetConnectedState")]
        public static extern bool InternetGetConnectedState(out int conState, int reder);

        //参数说明 constate 连接说明 ，reder保留值
        public static bool IsConnectedToInternet()
        {

     
            int Desc = 0;
            return InternetGetConnectedState(out Desc, 0);
        }

    }
}
