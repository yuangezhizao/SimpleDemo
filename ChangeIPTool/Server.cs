using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

using DotRas;

namespace ChangeIPTool
{
    class Server
    {
        List<string> historyIps = new List<string>();
        private int IPLoopCount = 3;//允许重复次数  拨号3次还是重复的ip 
        private int DialFaildSleepTime = 30000;

       
        /// <summary>
        /// ip转换
        /// </summary>
        /// <param name="kdlj">宽带连接名称 如：本地连接，宽带连接</param>
        /// <param name="userName">帐号</param>
        /// <param name="pwd">密码</param>
        public void ChangeIp(string kdlj,string userName,string pwd)
        {
            //hx.Mset.IsChangeIp = hx.Mset.IsChangeIp;
            LogServer.WriteLog("开始准备更换IP","changeIp");

            HANDUPCON:
            string oldIpAddress;
            RasConnection oldConn;
            GetIpAddress(out oldIpAddress, out oldConn);
            string entryName = "";
            if (oldConn != null)
            {
                entryName = oldConn.EntryName;
                RasIPInfo ipAddresses = (RasIPInfo)oldConn.GetProjectionInfo(RasProjectionType.IP);
                string oldIp = ipAddresses.IPAddress.ToString();
 
                LogServer.WriteLog("现在名称:"+ entryName + "IP是" + oldIp, "changeIp");

                try
                {
                    LogServer.WriteLog("开始挂断", "changeIp");
                    oldConn.HangUp(10 * 1000);
                    //Thread.Sleep(hx.Mset.RasHangUpSleepTime);
                    if (RasConnection.GetActiveConnectionById(oldConn.EntryId) != null)
                    {
                        LogServer.WriteLog("结束挂断失败，重新挂断", "changeIp");
                        goto HANDUPCON;
                    }
                    oldConn = null;
                    LogServer.WriteLog("结束挂断" , "changeIp");
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog("宽带连接挂断失败，" + ex.Message, "changeIp");
                }
            }
            CHANGEIP:
            try
            {
                //  var dt = SqliteHelper.GetDataTable("select * from sys_config");
                RasDialer rs = new RasDialer();
                if (entryName == "")
                {
                    entryName = kdlj;// dt.Rows[0]["SC_NetEntryName"].ToString();
                }
                rs.EntryName = entryName;
                rs.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
                rs.Credentials = new NetworkCredential(userName, pwd);
                rs.Dial();
                rs.Dispose();

                LogServer.WriteLog("开始重新拨号", "changeIp");
            }
            catch (Exception ex)
            {
                //addlog("宽带连接拨号失败，" + ex.Message);
                //Thread.Sleep(hx.Mset.DialFaildSleepTime);
                LogServer.WriteLog("宽带连接拨号失败" + ex.Message, "changeIp");
                Thread.Sleep(30000);
                goto CHANGEIP;
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

                //addlog("现在的IP是" + ipAddresses);


            }
            //main.pppoeact = true;
            LogServer.WriteLog("更换成功", "changeIp");
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

        public static DateTime lastUpdate;
        public void TimerDoing(string kdlj, string userName, string pwd)
        {
            DateTime logTime = LogServer.ReadLogRowNo("shieldSpider");
            if (lastUpdate < logTime)
            {
                try
                {
                    ChangeIp(kdlj, userName, pwd);
                    lastUpdate = DateTime.Now.AddMinutes(5);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "changeIp");
                }
            
            }
        }

    }
}
