using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using BLL;
using Commons;
using Mode;

namespace Servers
{
    public class HostProxyServer
    {

        #region 抓取代理
        private const RegexOptions Ro = RegexOptions.Singleline | RegexOptions.IgnoreCase;
        public bool StopDownLoadProxy = false;
        public void LoadHost()
        {
            Kuaidaili();
            //GetProxy();
            //XicidownLoad();
            //validHost();
            //postHost();
        }
       
        private void XicidownLoad()
        {
            int total = 2;
            const string url = "http://www.xici.net.co/wn/";
            //var num1 = ran.Next(10000, 99999);
            //string num2 = Guid.NewGuid().ToString().Replace("-", "");
            HtmlAnalysis html = new HtmlAnalysis();

            html.Headers.Add("Cookie", "CNZZDATA4793016=cnzz_eid%3D439368589-1420247114-%26ntime%3D1420416086; visid_incap_257263=qcJrT1itQl2Uy8H7M1HdPh5Cp1QAAAAAQUIPAAAAAADbb0l5HBRfzWo6H2qin3T1; incap_ses_200_257263=5WMUOoZSsmBMNBTYYovGAoHnqVQAAAAAC+s1PXgYYuoGJy1kBbZNAw==");
  
        
            for (int i = 1; i < total; i++)
            {
                string tempurl = url + i;
       

 
                var pageInfo = html.HttpRequest(tempurl);
                if (i == 1)
                {
                    string maxpage = Regex.Match(pageInfo, "<a href=\"/(nn|wn)/\\d+\">(?<x>\\d+)</a> <a class=next_page rel=next href=\"/(nn|wn)/\\d\">下一页|<a href=\"/(nn|wn)/\\d+\">(?<x>\\d+)</a> <a class=\"next_page\" rel=\"next\" href=\"/(nn|wn)/2\">下一页", Ro).Groups["x"].Value;
                    int.TryParse(maxpage, out total);
                }
                var lit = Regex.Matches(pageInfo, "(<tr class=\"\">|<tr class=\"odd\">)(?<x>.*?)</tr>", Ro);
                List<HostProxy> hosts = new List<HostProxy>();
                foreach (Match tr in lit)
                {
                    string row = tr.ToString();
                    var tds = Regex.Matches(row, "<td>(?<x>.*?)</td>", Ro);
                    if (tds.Count < 8)
                        continue;
                    HostProxy host = new HostProxy();
                    string coy = Regex.Match(tds[1].Value, "alt=\"(?<x>.*?)\"", Ro).Groups["x"].Value.ToLower();
                    host.Country = coy;
                    host.IpAddress = tds[2].Groups["x"].Value;
                    host.IpPort = tds[3].Groups["x"].Value;
                    host.Area = WordCenter.FilterHtml(tds[4].Groups["x"].Value);
                    host.Niming = tds[5].Groups["x"].Value;
                    host.HttpType = tds[6].Groups["x"].Value.ToLower();
                    //if (host.HttpType != "http")
                    //    continue;
                    string contime = Regex.Match(tds[8].Value, "title=\"(?<x>.*?)\"", Ro).Groups["x"].Value;
                    double sec1;

                    double.TryParse(contime.Replace("秒", ""), out sec1);

                    host.ConnectionTime = sec1;
                    double sec2;
                    contime = Regex.Match(tds[7].Value, "title=\"(?<x>.*?)\"", Ro).Groups["x"].Value;
                    double.TryParse(contime.Replace("秒", ""), out sec2);
                    if (sec1 > 2 || sec2 > 2)
                        continue;
                    host.Speed = sec2;
                    host.VolidTime = DateTime.Now;
                    host.CreateDate = DateTime.Now;
                    host.VolidFaildCount = 0;
                    host.VolidTotalCount = 0;
                    host.IsDel = false;
                    host.Available = false;
                    host.DataSource = "http://www.xici.net.co";
                    hosts.Add(host);
                }
                if (hosts.Count > 0)
                {
                    addHost(hosts);
                }

            }


        }

        private void proxy360()
        {

            const string url = "http://www.proxy360.cn/Region/China";
            HtmlAnalysis html = new HtmlAnalysis();
            var pageInfo = html.HttpRequest(url);

            string conntent = Regex.Match(pageInfo, "<div id=\"ctl00_ContentPlaceHolder1_upProjectList\">(?<x>.*?)<div id=\"divFoot\">", Ro).Groups["x"].Value;

           // string conntent = pageInfo.Substring(pageInfo.IndexOf("<div id=\"ctl00_ContentPlaceHolder1_upProjectList\">"),pageInfo.IndexOf("<b>友情链接:</b>"));

            var list = Regex.Matches(conntent, "<div class=\"proxylistitem\" name=\"list_proxy_ip\">(?<x>.*?)<div style=\"width:100px; float:left;\">",Ro);
            List<HostProxy> hosts = new List<HostProxy>();
            foreach (Match item in list)
            {
                string ipinfo = item.ToString();

                var tempid = Regex.Matches(ipinfo, "<span class=\"tbBottomLine\" style=\"width:\\d+px;\">\\s*(?<x>.*?)\\s*</span>|<span class=\"tbBottomLine \" style=\"width:\\d+px;\">(?<x>.*?)</span>", Ro);
                if (tempid.Count < 8)
                    continue;
                double speed;
                double.TryParse(tempid[6].Groups["x"].Value, out speed);
                HostProxy host = new HostProxy
                {
                    IpPort = tempid[1].Groups["x"].Value,
                    IsDel = false,
                    IpAddress = tempid[0].Groups["x"].Value,
                    Area ="",
                    HttpType = "未知",
                    Niming = WordCenter.FilterHtml(tempid[2].Groups["x"].Value),
                    VolidFaildCount = 0,
                    VolidTime = DateTime.Now,
                    VolidTotalCount = 0,
                    Available=false,
                    ConnectionTime=0,
                    CreateDate=DateTime.Now,
                    DataSource="http://www.proxy360.cn",
                    Country = WordCenter.FilterHtml(tempid[3].Groups["x"].Value),
                    Speed = speed
                };

                hosts.Add(host);

            }
            if (hosts.Count > 0)
            {
                addHost(hosts);
            }
            
        }
        /// <summary>
        /// 快代理网站代理获取
        /// </summary>
        private void Kuaidaili()
        {
            // const string urlmode = "http://www.kuaidaili.com/proxylist/{0}/";
            const string urlmode = "http://www.kuaidaili.com/free/inha/{0}/";

            HtmlAnalysis html = new HtmlAnalysis();

            int total = 2;
            // html.Header = new Dictionary<string, string>();
            //  html.Header.Add("Cookie", "_ga=GA1.2.708367395.1410253321; Hm_lvt_7ed65b1cc4b810e9fd37959c9bb51b31=1420613276; _gat=1; Hm_lpvt_7ed65b1cc4b810e9fd37959c9bb51b31=1420613642");
            for (int i = 1; i < total + 1; i++)
            {
                List<HostProxy> hosts = new List<HostProxy>();
                string tempurl = string.Format(urlmode, i);
                var page = html.HttpRequest(tempurl);
                if (i == 1)
                {
                    string maxpage = Regex.Match(page, "href=\"/(free/inha|proxylist)/\\d+/\">(?<x>\\d+)</a></li>\\s*<li>页</li>", Ro).Groups["x"].Value;
                    int.TryParse(maxpage, out total);
                }
                string conn = Regex.Match(page, "(<div id=\"list\">|<div id=\"list\" style=\"margin-top:15px;\">)(?<x>.*?)<div id=\"listnav\">", Ro).Groups["x"].Value;
                if (string.IsNullOrEmpty(conn))
                    continue;
                var list = Regex.Matches(conn, "<tr>(?<x>.*?)</tr>", Ro);
                foreach (Match item in list)
                {
                    string tr = item.Groups["x"].Value;
                    if (string.IsNullOrEmpty(tr))
                        continue;
                    if (tr.Contains("匿名度"))
                        continue;
                    var td = Regex.Matches(tr, "<td>(?<x>.*?)</td>");
                    if (td.Count < 5)
                        continue;
                    HostProxy host = new HostProxy
                    {
                        VolidTime = DateTime.Now,
                        CreateDate = DateTime.Now,
                        VolidFaildCount = 0,
                        VolidTotalCount = 0,
                        IsDel = false,
                        Available = false
                    };
                    host.IpAddress = td[0].Groups["x"].Value;
                    host.IpPort = td[1].Groups["x"].Value;
                    host.Niming = td[2].Groups["x"].Value;
                    host.HttpType = td[3].Groups["x"].Value;
                    host.Country = "中国";
                    if (urlmode.Contains("/free/inh"))
                        host.Area =td[4].Groups["x"].Value;
                    else
                        host.Area = td[5].Groups["x"].Value;
                    host.Area = WordCenter.FilterHtml(host.Area);
                    host.DataSource = "http://www.kuaidaili.com";
                    hosts.Add(host);
                }


                if (hosts.Count > 0)
                {
                    addHost(hosts);
                }

            }

        }

        /// <summary>
        /// 添加代理（并对其做简单验证）
        /// </summary>
        /// <param name="hosts"></param>
        private void addHost(List<HostProxy> hosts)
        {
            List<HostProxy> templist = new List<HostProxy>();
            for (int i = 0; i < hosts.Count; i++)
            {
                if (ValidHost(hosts[i].IpAddress, hosts[i].IpPort))
                    templist.Add(hosts[i]);
            }
            if (templist.Count > 0)
                new HostProxyBll().SaveHostProxy(templist);
        }
        #endregion

        #region 代理验证
        private static bool ValidHost(string host, string strport)
        {
            if (
                !Regex.IsMatch(host,
                    @"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))"))
                return false;
            int port;
            if (!int.TryParse(strport, out port))
                return false;

            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    s.ReceiveTimeout = 3000;
                    s.Connect(host, port);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 对所有的代理进行验证（全量验证）
        /// </summary>
        public void validallHost()
        {
            var list = new HostProxyBll().loadHostProxy();

            for (int i = 0; i < list.Count; i++)
            {

                if (!ValidHost(list[i].IpAddress, list[i].IpPort))
                {
                    list[i].IsDel = true;
                    list[i].VolidTime = DateTime.Now;
                    list[i].VolidFaildCount += 1;
                    list[i].VolidTotalCount += 1;
                    new HostProxyBll().UpdateHostProxy(list[i]);
                }
                else
                {
                    list[i].IsDel = false;
                    list[i].VolidTime = DateTime.Now;
                    new HostProxyBll().UpdateHostProxy(list[i]);
                }
            }

            list = new HostProxyBll().loadHostProxy();

            if (!list.Any())
                return;
            list = list.Where(p => !p.IsDel).OrderBy(p => p.VolidTime).ToList();

            foreach (HostProxy hpy in list)
            {
                try
                {
                    HtmlAnalysis tool = new HtmlAnalysis();
                    tool.RequesProxy = new WebProxy(hpy.IpAddress, int.Parse(hpy.IpPort));
                    tool.RequestTimeout = 20000;
                    tool.RequestAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                    tool.Headers = new Dictionary<string, string>();
                    tool.Headers.Add("Accept-Language", "zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3");
                    tool.Headers.Add("Accept-Encoding", "Accept-Encoding");
                    tool.Headers.Add("Cache-Control", "max-age=0");

                    // var page1 = tool.HttpRequest("http://list.tmall.com/search_product.htm?cat=50095168");
                    //var page = tool.HttpRequest("http://list.jd.com/list.html?cat=9987,653,655");
                    Stopwatch loadtime = new Stopwatch();
                    loadtime.Start();
                    var page = tool.HttpRequest("http://list.yhd.com/c23586-0/");
                    loadtime.Stop();
                    hpy.Available = !string.IsNullOrEmpty(page);
                    hpy.VolidTotalCount += 1;
                    hpy.IsDel = !hpy.Available;
                    if (hpy.IsDel)
                        hpy.VolidFaildCount += 1;
                    hpy.VolidTime = DateTime.Now;
                    hpy.ConnectionTime = loadtime.Elapsed.TotalSeconds;
                    new HostProxyBll().UpdateHostProxy(hpy);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            }
        }
        /// <summary>
        /// 定时验证代理是否可用（简单验证）
        /// </summary>
        public void validAvailable()
        {
            var list = new HostProxyBll().loadHostProxy();

            if (!list.Any())
                return;
            list = list.Where(p => p.Available).OrderBy(p => p.VolidTime).ToList();

            for (int i = 0; i < list.Count(); i++)
            {
                if (!ValidHost(list[i].IpAddress, list[i].IpPort))
                {
                    list[i].Available = false;
                    list[i].IsDel = true;
                    list[i].VolidTime = DateTime.Now;
                    list[i].VolidFaildCount += 1;
                    list[i].VolidTotalCount += 1;
                    new HostProxyBll().UpdateHostProxy(list[i]);
                }
            }
        }
        /// <summary>
        /// 发送到服务器上已提供客户端调用
        /// </summary>
        public void postHost()
        {
            var list = new HostProxyBll().loadHostProxy();

            if (!list.Any())
                return;
            list = list.Where(p => p.Available && p.VolidTime>DateTime.Now.AddHours(-6)).ToList();
            StringBuilder iplist = new StringBuilder();
            for (int i = 0; i < list.Count(); i++)
            {
                iplist.AppendFormat("{0}:{1}", list[i].IpAddress, list[i].IpPort);
                if (i < list.Count() - 1)
                    iplist.Append(",");
            }

            if (iplist.Length == 0)
                return;
            NameValueCollection data = new NameValueCollection();
            data.Add("ipstr", iplist.ToString());

            WebClient client = new WebClient();

            client.UploadValues("http://distributedcalculate.manmanbuy.com/IPProxy", "POST", data);

        }
        #endregion
    }
}
