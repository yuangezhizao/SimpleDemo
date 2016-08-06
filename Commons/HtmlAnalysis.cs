using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Ude;

namespace Commons
{
    public class HtmlAnalysis
    {
      
        #region 属性及参数配置

        /// <summary>
        /// 用户代理信息
        /// </summary>
        private static readonly string[] UserAgentList =
        { 
            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-us) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50",
            "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_8; en-us) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50",
            "Mozilla/4.0 (compatible; MSIE 4.0; MSN 2.5; Windows 95)",
            "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0;",
            "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0)",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)",
            "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.6; rv:2.0.1) Gecko/20100101 Firefox/4.0.1",
            "Opera/9.80 (Macintosh; Intel Mac OS X 10.6.8; U; en) Presto/2.8.131 Version/11.11",
            "Opera/9.80 (Windows NT 6.1; U; en) Presto/2.8.131 Version/11.11",
            "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/6.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)",
            "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)",
            "Mozilla/5.0 (Windows NT 6.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1",
            "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:41.0) Gecko/20100101 Firefox/41.0",
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.155 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_0) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.56 Safari/535.11",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; TencentTraveler 4.0)",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; The World)",   
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; SE 2.X MetaSr 1.0; SE 2.X MetaSr 1.0; .NET CLR 2.0.50727; SE 2.X MetaSr 1.0)",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; 360SE)",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Avant Browser)",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Maxthon 2.0)",
            //"Mozilla/5.0 (compatible; Yahoo! Slurp China; http://misc.yahoo.com.cn/help.html)", 
            //"iaskspider/2.0(+http://iask.com/help/help_index.html)",
            //"Sogou web spider/3.0(+http://www.sogou.com/docs/help/webmasters.htm#07)",
            //"Baiduspider+(+http://www.baidu.com/search/spider.htm)",
            "Mozilla/5.0 (Linux; U; Android 2.1-update1; ja-jp; SonyEricssonSO-01B Build/2.0.2.B.0.29) AppleWebKit/530.17 (KHTML, like Gecko) Version/4.0 Mobile Safari/530.17"
        };

        public string RequestUserAgent { get; set; }
        public string[] headKey =
        {
            "Accept", "Connection", "Content-Length","Content-Type","Expect","Date","Host","If-Modified-Since","Range","Referer","Transfer-Encoding","User-Agent"
        };
        public Dictionary<string, string> ResultResponseHeader { get; set; }
        private object HEAD_LOCK = new object();
        /// <summary>
        /// GET 或者 POST
        /// </summary>
        public string RequestMethod { get; set; }
        /// <summary>
        /// Accept 头信息
        /// </summary>
        public string RequestAccept { get; set; }
        /// <summary>
        /// 标题头信息
        /// </summary>
        public string RequestContentType { get; set; }



        /// <summary>
        /// 告诉服务器我是从哪个页面链接过来的
        /// </summary>
        public string RequestReferer { get; set; }

        /// <summary>
        /// 设置请求头信息
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// 是否和intent保持永久连接
        /// </summary>
        public bool RequestKeepAlive { get; set; }
        /// <summary>
        /// 超时时间（毫秒）
        /// </summary>
        public int RequestTimeout { get; set; }

        /// <summary>
        /// 编码格式
        /// </summary>
        public Encoding RequestEncoding { get; set; }
        /// <summary>
        /// 设置cookies信息
        /// </summary>
        public CookieContainer RequestCookies { get; set; }

        /// <summary>
        /// 是否跟随重定向响应
        /// </summary>
        public bool RequestAutoRedirect { get; set; }

        /// <summary>
        /// 随机设置用户代理信息
        /// </summary>
        public bool RanAgent { get; set; }

        /// <summary>
        /// 在请求的时候是否传送cookies
        /// </summary>
        public bool HasCookies { get; set; }
        /// <summary>
        /// 设置代理信息
        /// </summary>
        public WebProxy RequesProxy { get; set; }
        

        static int _nonius;
        static readonly Regex IpPortReg = new Regex("(?<ip>\\d+\\.\\d+\\.\\d+\\.\\d+):(?<port>\\d*)");

        public string httpsGet(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                new RemoteCertificateValidationCallback(CheckValidationResult);

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8"; //默认编码
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            string result = reader.ReadToEnd();
            response.Close();
            return result;

        }

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        { // Always accept
         
            return true; //总是接受
        }

        public HtmlAnalysis()
        {
            RequestMethod = "GET";
            RequestAccept = "text/html, application/xhtml+xml, */*";
            RequestContentType = "text/html";
            Headers = new Dictionary<string, string>
            {
                {"Accept-Language", "zh-cn,en-us;"},
                {"Accept-Encoding", "gzip, deflate"}
            };
            RequestUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36";
            RequestKeepAlive = false;
            RequestTimeout = 60000;
            RequestCookies = new CookieContainer();
            RequestEncoding = Encoding.UTF8;
            RequestAutoRedirect = true;
            RanAgent = false;
            HasCookies = false;
            RequesProxy = null;
        }

        private static readonly Random Rang = new Random();

        #endregion

        public static string GetResponseHeader(string url,string key)
        {
            try
            {
                WebRequest myRequest = WebRequest.Create(url);
                using (WebResponse myResponse = myRequest.GetResponse())
                {
                    return myResponse.Headers.Get(key);
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetResponseCookies(string url)
        {
            return GetResponseHeader(url, "Set-Cookie");
        }

        /// <summary>
        /// GET 方式抓取网页信息
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns>网页信息</returns>
        public string HttpRequest(string url)
        {
            if (string.IsNullOrEmpty(url) || (!url.Contains("http://") && !url.Contains("https://")))
                return "";
            int retry = 0;
            while (true)
            {
                string paraminfo = "";
                string strGethtml = "";
                try
                {
                   
                    if (url.Contains("?") && RequestMethod.ToUpper() == "POST")
                    {
                        paraminfo = url.Substring(url.IndexOf('?') + 1, url.Length - url.IndexOf('?') - 1);
                        url = url.Substring(0, url.IndexOf('?'));
                    }
                    HttpWebRequest mywr = (HttpWebRequest)WebRequest.Create(url);
                    mywr.Proxy = RequesProxy;
                    mywr.Method = RequestMethod;
                    mywr.Accept = RequestAccept;
                    mywr.ContentType = RequestContentType;
                    mywr.AllowAutoRedirect = RequestAutoRedirect;
                    if (!string.IsNullOrEmpty(RequestReferer))
                        mywr.Referer = RequestReferer;
                    mywr.UserAgent = RanAgent
                        ? UserAgentList[Rang.Next(0, UserAgentList.Length)]
                        : RequestUserAgent;
                    if (Headers != null && Headers.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> item in Headers)
                        {
                            mywr.Headers.Add(item.Key, item.Value);
                        }
                    }
                    mywr.KeepAlive = RequestKeepAlive;
                    mywr.Timeout = RequestTimeout;
                    if (RequestCookies.Count > 0)
                        mywr.CookieContainer = RequestCookies;
                    if (HasCookies)
                    {
                        if (mywr.CookieContainer == null)
                        {
                            mywr.CookieContainer = new CookieContainer();
                        }
                        SimulationCookie.GetCookie(mywr.Host, cookie => mywr.CookieContainer.Add(cookie));
                    }
                    //CookieContainer cookieContainer = new CookieContainer();
                    //CookieCollection cookies = cookieContainer.GetCookies(new Uri(url));
                    if (ResultResponseHeader != null)
                    {
                        foreach (var item in ResultResponseHeader)
                        {
                            mywr.Headers.Add(item.Key, item.Value);
                        }

                    }
                    //把参数用流对象写入request对象中
                    if (paraminfo != "")
                    {
                        byte[] postbyte = Encoding.ASCII.GetBytes(paraminfo);
                        mywr.ContentLength = postbyte.Length;
                        Stream newStream = mywr.GetRequestStream();
                        newStream.Write(postbyte, 0, postbyte.Length);
                        newStream.Close();
                    }

                    HttpWebResponse mywrp = (HttpWebResponse)mywr.GetResponse();
                    //mywrp.Headers.ToString();
                    if (mywrp.ResponseUri.AbsoluteUri != url)
                    {
                        LogServer.WriteLog("url old:" + url + "new " + mywrp.ResponseUri, "UrlChange");
                        //return "";
                    }

                    if (HttpRuntime.Cache["httpHead"] == null && (ResultResponseHeader == null || ResultResponseHeader.Count == 0))
                    {
                        lock (HEAD_LOCK)
                        {
                            HttpRuntime.Cache.Add("httpHead", "httpHead", null, DateTime.Now.AddMinutes(30),
                               System.Web.Caching.Cache.NoSlidingExpiration,
                               System.Web.Caching.CacheItemPriority.NotRemovable, null);
                            ResultResponseHeader = new Dictionary<string, string>();
                            if (ResultResponseHeader == null || ResultResponseHeader.Count == 0)
                            {
                                for (int i = 0; i < mywrp.Headers.Count; i++)
                                {
                                    if (headKey.Contains(mywrp.Headers.Keys[i]))
                                        continue;
                                    if (mywrp.Headers.Keys[i] == "Set-Cookie")
                                    {
                                        ResultResponseHeader.Add("Cookie", mywrp.Headers.Get(i));
                                    }
                                    else
                                    {
                                        ResultResponseHeader.Add(mywrp.Headers.Keys[i], mywrp.Headers.Get(i));
                                    }
                                }
                            }
                        }
                    }
                    //SimulationCookie.SetCookie(mywr.Host, mywrp.Cookies);
                    Stream responseStream = mywrp.GetResponseStream();
                    if (mywrp.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        if (responseStream != null)
                            responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                    }

                    int scode = (Int32)mywrp.StatusCode;
                    if (scode != 200)
                        return "Error StatusCode:" + scode;

                    Encoding encodingType;
                    string tempType = mywrp.Headers["Content-Type"] ?? "";
                    tempType = tempType.ToLower();
                    if (tempType.Contains("gbk") || tempType.Contains("gb2312") || tempType.Contains("utf-8"))
                    {

                        if (tempType.Contains("gbk") || tempType.Contains("gb2312"))
                            encodingType = Encoding.GetEncoding("GBK");
                        else
                            encodingType = Encoding.UTF8;

                        if (responseStream == null) return strGethtml;
                        StreamReader sr = new StreamReader(responseStream, encodingType);
                        strGethtml = sr.ReadToEnd().Trim();
                        mywrp.Close();
                        sr.Close();
                        return strGethtml;
                    }
                    else
                    {
                        #region 将html文件流转换成字节
                        List<byte> blist = new List<byte>();
                        byte[] buff = new byte[2048];
                        int read;
                        while (responseStream != null && (read = responseStream.Read(buff, 0, buff.Length)) > 0)
                        {
                            blist.AddRange(buff.Take(read));
                        }
                        var bytes = blist.ToArray();
                        #endregion
                        ICharsetDetector cdet = new CharsetDetector();
                        cdet.Feed(bytes, 0, bytes.Length);
                        cdet.DataEnd();
                        //如果自动识别程序未能识别页面编码，则使用程序指定
                        if (string.IsNullOrEmpty(cdet.Charset))
                        {
                            encodingType = RequestEncoding;
                        }
                        else
                        {
                            encodingType = Encoding.GetEncoding(cdet.Charset);
                        }
                        strGethtml = encodingType.GetString(bytes);
                    }

                    if (mywrp.StatusCode == HttpStatusCode.Redirect)
                    {
                        strGethtml += string.Format("ResponseUri:{0}", mywrp.ResponseUri);
                    }

                    mywrp.Close();
                    return strGethtml;

                    //if (responseStream == null) return strGethtml;
                    //StreamReader sr = new StreamReader(responseStream, RequestEncoding);
                    //strGethtml = sr.ReadToEnd().Trim();
                    //mywrp.Close();
                    //sr.Close();
                }
                catch (Exception ex)
                {
                    if (retry < 3)
                    {
                        retry++;
                        if ( RequestMethod.ToUpper() == "POST")
                        {
                            url = url + "?" + paraminfo;
                        }
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (ex is WebException)
                    {
                        var webEx = ex as WebException;
                        if (RequesProxy != null)
                        {
                            LogServer.WriteLog("url:" + url + "Proxy:" + RequesProxy.Address + "\t" + webEx.Message, "HtmlAnalysis");
                        }
                        else
                            LogServer.WriteLog("url:" + url + "" + webEx.Message, "HtmlAnalysis");
                        return "";
                    }

                    LogServer.WriteLog("get url:" + url + ex, "HtmlAnalysis");
                    return "";
                }
            }
        }

        public string HttpRequest(string url,string param)
        {
            if (string.IsNullOrEmpty(url) || (!url.Contains("http://") && !url.Contains("https://")))
                return "";
            int retry = 0;
            while (true)
            {
                string paraminfo = "";
                string strGethtml = "";
                try
                {

                    if (!string.IsNullOrEmpty(param))
                    {
                        if (RequestMethod.ToUpper() == "POST")
                            paraminfo = param;
                        else
                            url = url + "?" + param;

                    }

                    HttpWebRequest mywr = (HttpWebRequest)WebRequest.Create(url);
                    mywr.Proxy = RequesProxy;
                    mywr.Method = RequestMethod;
                    mywr.Accept = RequestAccept;
                    mywr.ContentType = RequestContentType;
                    mywr.AllowAutoRedirect = RequestAutoRedirect;
                    if (!string.IsNullOrEmpty(RequestReferer))
                        mywr.Referer = RequestReferer;
                    mywr.UserAgent = RanAgent
                        ? UserAgentList[Rang.Next(0, UserAgentList.Length)]
                        : RequestUserAgent;
                    if (Headers != null && Headers.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> item in Headers)
                        {
                            mywr.Headers.Add(item.Key, item.Value);
                        }
                    }
                    mywr.KeepAlive = RequestKeepAlive;
                    mywr.Timeout = RequestTimeout;
                    if (RequestCookies.Count > 0)
                        mywr.CookieContainer = RequestCookies;
                    if (HasCookies)
                    {
                        if (mywr.CookieContainer == null)
                        {
                            mywr.CookieContainer = new CookieContainer();
                        }
                        SimulationCookie.GetCookie(mywr.Host, cookie => mywr.CookieContainer.Add(cookie));
                    }
                 
                    //把参数用流对象写入request对象中
                    if (paraminfo != "")
                    {
                        byte[] postbyte = Encoding.ASCII.GetBytes(paraminfo);
                        mywr.ContentLength = postbyte.Length;
                        Stream newStream = mywr.GetRequestStream();
                        newStream.Write(postbyte, 0, postbyte.Length);
                        newStream.Close();
                    }

                    HttpWebResponse mywrp = (HttpWebResponse)mywr.GetResponse();
                    //mywrp.Headers.ToString();
                    if (mywrp.ResponseUri.AbsoluteUri != url)
                    {
                        LogServer.WriteLog("url old:" + url + "new " + mywrp.ResponseUri, "UrlChange");
                        //return "";
                    }
                    if(ResultResponseHeader==null)
                        ResultResponseHeader=new Dictionary<string, string>();
                    else if (ResultResponseHeader.Count > 0)
                    {
                        ResultResponseHeader.Clear();
                    }
            
                    for (int i = 0; i < mywrp.Headers.Count; i++)
                    {
                        if (headKey.Contains(mywrp.Headers.Keys[i]))
                            continue;
                       
                        ResultResponseHeader.Add(mywrp.Headers.Keys[i], mywrp.Headers.Get(i));
                    }

                    //SimulationCookie.SetCookie(mywr.Host, mywrp.Cookies);
                    Stream responseStream = mywrp.GetResponseStream();
                    if (mywrp.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        if (responseStream != null)
                            responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                    }

                    int scode = (Int32)mywrp.StatusCode;
                    if (scode != 200)
                        return "Error StatusCode:" + scode;

                    Encoding encodingType;
                    string tempType = mywrp.Headers["Content-Type"] ?? "";
                    tempType = tempType.ToLower();
                    if (tempType.Contains("gbk") || tempType.Contains("gb2312") || tempType.Contains("utf-8"))
                    {

                        if (tempType.Contains("gbk") || tempType.Contains("gb2312"))
                            encodingType = Encoding.GetEncoding("GBK");
                        else
                            encodingType = Encoding.UTF8;

                        if (responseStream == null) return strGethtml;
                        StreamReader sr = new StreamReader(responseStream, encodingType);
                        strGethtml = sr.ReadToEnd().Trim();
                        mywrp.Close();
                        sr.Close();
                        return strGethtml;
                    }
                    else
                    {
                        #region 将html文件流转换成字节
                        List<byte> blist = new List<byte>();
                        byte[] buff = new byte[2048];
                        int read;
                        while (responseStream != null && (read = responseStream.Read(buff, 0, buff.Length)) > 0)
                        {
                            blist.AddRange(buff.Take(read));
                        }
                        var bytes = blist.ToArray();
                        #endregion
                        ICharsetDetector cdet = new CharsetDetector();
                        cdet.Feed(bytes, 0, bytes.Length);
                        cdet.DataEnd();
                        //如果自动识别程序未能识别页面编码，则使用程序指定
                        if (string.IsNullOrEmpty(cdet.Charset))
                        {
                            encodingType = RequestEncoding;
                        }
                        else
                        {
                            encodingType = Encoding.GetEncoding(cdet.Charset);
                        }
                        strGethtml = encodingType.GetString(bytes);
                    }

                    if (mywrp.StatusCode == HttpStatusCode.Redirect)
                    {
                        strGethtml += string.Format("ResponseUri:{0}", mywrp.ResponseUri);
                    }

                    mywrp.Close();
                    return strGethtml;

                    //if (responseStream == null) return strGethtml;
                    //StreamReader sr = new StreamReader(responseStream, RequestEncoding);
                    //strGethtml = sr.ReadToEnd().Trim();
                    //mywrp.Close();
                    //sr.Close();
                }
                catch (Exception ex)
                {
                    if (retry < 3)
                    {
                        retry++;
                   
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (ex is WebException)
                    {
                        var webEx = ex as WebException;
                        if (RequesProxy != null)
                        {
                            LogServer.WriteLog("url:" + url + "Proxy:" + RequesProxy.Address + "\t" + webEx.Message, "HtmlAnalysis");
                        }
                        else
                            LogServer.WriteLog("url:" + url + "" + webEx.Message, "HtmlAnalysis");
                        return "";
                    }

                    LogServer.WriteLog("get url:" + url + ex, "HtmlAnalysis");
                    return "";
                }
            }
        }

        /// <summary>
        /// GET 方式抓取网页信息
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="encodingType">utf8、Default 网页编码格式</param>
        /// <param name="ranAgent">随机的用户代理</param>
        /// <returns>网页信息</returns>
        public static string Gethtmlcode(string url, string encodingType = "utf-8", bool ranAgent = false)
        {
            if (string.IsNullOrEmpty(url) || (!url.Contains("http://") && !url.Contains("https://")))
                return "";
            int retry = 0;
            while (true)
            {
                Encoding EncodingType;
                try
                {
                    HttpWebRequest mywr = (HttpWebRequest)WebRequest.Create(url);
                    mywr.Method = "GET";
                    mywr.Accept = "text/html, application/xhtml+xml, */*";
                    mywr.ContentType = "text/html";
                    mywr.UserAgent = ranAgent
                        ? UserAgentList[Rang.Next(0, UserAgentList.Length)]
                        : "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36";
                    mywr.CookieContainer = null;
                    mywr.Headers.Add("Accept-Language", "zh-cn,en-us;");
                    mywr.Headers.Add("Accept-Encoding", "gzip, deflate");
                    mywr.KeepAlive = false;
                    mywr.Timeout = 60000;
                    if (mywr.CookieContainer == null)
                    {
                        mywr.CookieContainer = new CookieContainer();
                    }
                    SimulationCookie.GetCookie(mywr.Host, cookie => mywr.CookieContainer.Add(cookie));

                    HttpWebResponse mywrp = (HttpWebResponse)mywr.GetResponse();
                    SimulationCookie.SetCookie(mywr.Host, mywrp.Cookies);
                    Stream responseStream = mywrp.GetResponseStream();
                    if (mywrp.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        if (responseStream != null)
                            responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                    }

                    int scode = (Int32)mywrp.StatusCode;
                    if (scode != 200)
                        return "Error StatusCode:" + scode;


                    // string strGethtml = "";
                    //if (responseStream == null) return strGethtml;
                    //StreamReader sr = new StreamReader(responseStream,
                    //    encodingType == "utf8" ? Encoding.UTF8 : Encoding.Default);
                    //strGethtml = sr.ReadToEnd().Trim();
                    //mywrp.Close();
                    //sr.Close();
                    //return strGethtml;


                    string strGethtml = "";

                    string tempType = mywrp.Headers["Content-Type"] ?? "";
                    tempType = tempType.ToLower();
                    if (tempType.Contains("gbk") || tempType.Contains("gb2312") || tempType.Contains("utf-8"))
                    {

                        if (tempType.Contains("gbk") || tempType.Contains("gb2312"))
                            EncodingType = Encoding.GetEncoding("GBK");
                        else
                            EncodingType = Encoding.UTF8;

                        if (responseStream == null) return strGethtml;
                        StreamReader sr = new StreamReader(responseStream, EncodingType);
                        strGethtml = sr.ReadToEnd().Trim();
                        mywrp.Close();
                        sr.Close();
                        return strGethtml;
                    }
                    else
                    {
                        #region 将html文件流转换成字节
                        List<byte> blist = new List<byte>();
                        byte[] buff = new byte[2048];
                        int read;
                        while (responseStream != null && (read = responseStream.Read(buff, 0, buff.Length)) > 0)
                        {
                            blist.AddRange(buff.Take(read));
                        }
                        var bytes = blist.ToArray();
                        #endregion
                        ICharsetDetector cdet = new CharsetDetector();
                        cdet.Feed(bytes, 0, bytes.Length);
                        cdet.DataEnd();
                        //如果自动识别程序未能识别页面编码，则使用程序指定
                        if (string.IsNullOrEmpty(cdet.Charset))
                        {
                            EncodingType = encodingType == "utf-8" ? Encoding.UTF8 : Encoding.Default;
                        }
                        else
                        {
                            EncodingType = Encoding.GetEncoding(cdet.Charset);
                        }
                        strGethtml = EncodingType.GetString(bytes);
                    }

                    if (mywrp.StatusCode == HttpStatusCode.Redirect)
                    {
                        strGethtml += string.Format("ResponseUri:{0}", mywrp.ResponseUri);
                    }

                    mywrp.Close();
                    return strGethtml;


                }
                catch (Exception ex)
                {
                    if (retry < 3)
                    {
                        retry++;
                        Thread.Sleep(1000);
                        continue;
                    }
                    LogServer.WriteLog("get url:" + url + ex, "HtmlAnalysis");
                    return "";
                }
            }
        }

        /// <summary>
        /// GET方式抓取网页信息
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="encodingType">utf-8、Default 网页编码格式</param>
        /// <param name="ranAgent">随机的用户代理</param>
        /// <param name="cookies">cookie 信息</param>
        /// <returns>网页信息</returns>
        public static string Gethtmlcode(string url, CookieContainer cookies, string encodingType= "utf-8",  bool ranAgent = false)
        {
            if (string.IsNullOrEmpty(url) || (!url.Contains("http://") && !url.Contains("https://")))
                return "";
            HttpWebRequest mywr = (HttpWebRequest)WebRequest.Create(url);
            mywr.Method = "GET";
            mywr.Accept = "text/html, application/xhtml+xml, */*";
            mywr.ContentType = "text/html";
            mywr.UserAgent = ranAgent
                ? UserAgentList[Rang.Next(0, UserAgentList.Length)]
                : "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36";
            mywr.Headers.Add("Accept-Language", "zh-cn,en-us;");
            mywr.Headers.Add("Accept-Encoding", "gzip, deflate");
            HttpWebResponse mywrp;
            mywr.CookieContainer = cookies;
            string strGethtml = "";
            try
            {
                mywrp = (HttpWebResponse)mywr.GetResponse();
                Stream responseStream = mywrp.GetResponseStream();
                if (mywrp.ContentEncoding.ToLower().Contains("gzip"))
                {
                    if (responseStream != null)
                        responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                if (responseStream != null)
                {
                    StreamReader sr = new StreamReader(responseStream,
                        encodingType == "utf-8" ? Encoding.UTF8 : Encoding.Default);
                    strGethtml = sr.ReadToEnd().Trim();
                    mywrp.Close();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                //超时错误暂时也放到404里面吧  当当有些网页会返回500内部错误 暂时未找到原因 
                if (error.Contains("404") || error.Contains("超时") || error.Contains("500") || error.Contains("503"))
                    return "404";
                return "800";
            }


            int scode = (Int32)mywrp.StatusCode;
            //对301 302等重定向 可能是正确的返回信息 暂未处理
            if (scode != 200)
                return "800";


            return strGethtml;
        }

        /// <summary>
        ///  POST方式抓取网页信息
        /// </summary>
        /// <param name="maiurl"></param>
        /// <param name="paramurl"></param>
        /// <param name="encode"></param>
        /// <param name="ranAgent"></param>
        /// <returns></returns>
        public static string HttpRequestFromPost(string maiurl, string paramurl, string encode, bool ranAgent = true)
        {
            if (string.IsNullOrEmpty(maiurl) || (!maiurl.Contains("http://") && !maiurl.Contains("https://")))
                return "";
            int retry = 0;
            while (true)
            {
                string strHtmlContent = "";
                try
                {
                    Encoding encoding = Encoding.GetEncoding(encode);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(maiurl);
                    request.Method = "POST";
                    request.UserAgent = ranAgent
                        ? UserAgentList[Rang.Next(0, UserAgentList.Length)]
                        : "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36";
                    request.ContentType = "application/x-www-form-urlencoded";

                    request.AllowAutoRedirect = true;
                    byte[] postbyte = Encoding.UTF8.GetBytes(paramurl);
                    request.ContentLength = postbyte.Length;
                    Stream newStream = request.GetRequestStream();
                    newStream.Write(postbyte, 0, postbyte.Length); //把参数用流对象写入request对象中   
                    newStream.Close();
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse(); //获得服务器响应对象  
                    Stream resStream = response.GetResponseStream(); //转成流对象   
                    if (resStream != null)
                    {
                        string tempType = response.Headers["Content-Type"] ?? "";
                        tempType = tempType.ToLower();
                        if (tempType.Contains("gbk") || tempType.Contains("gb2312") || tempType.Contains("utf-8"))
                        {

                            if (tempType.Contains("gbk") || tempType.Contains("gb2312"))
                                encoding = Encoding.GetEncoding("GBK");
                            else
                                encoding = Encoding.UTF8;
                        }

                        StreamReader sr = new StreamReader(resStream, encoding);
                        strHtmlContent = sr.ReadToEnd();
                    }
                    response.Close();
                    return strHtmlContent;
                }
                catch (Exception ex)
                {
                    if (retry < 3)
                    {
                        retry++;
                        Thread.Sleep(1000);
                        continue;
                    }
                    LogServer.WriteLog("post url:" + maiurl + "?" + paramurl + ex, "HtmlAnalysis");
                    return "";
                }
            }

        }

        /// <summary>
        /// 获取代理信息
        /// </summary>
        /// <returns></returns>
        public static WebProxy GetHttpProxy()
        {
            //if (rand.Next(0, 10) == 1)
            //{
            //    return null;
            //}

            if (HttpRuntime.Cache["IPProxy"] == null)
            {
                lock (Rang)
                {
                    if (HttpRuntime.Cache["IPProxy"] == null)
                    {
                        List<string> list = new List<string>();
                        int min = 5;
                        try
                        {
                            WebClient client = new WebClient();
                            var json = client.DownloadString("http://distributedcalculate.manmanbuy.com/IPProxy?format=json");
                            var dict = ServiceStack.Text.JsonSerializer.DeserializeFromString<Dictionary<string, int>>(json);
                            foreach (var item in dict)
                            {
                                list.Add(string.Format("{0}:{1}", item.Key, item.Value));
                            }
                        }
                        catch
                        {
                            min = 1;
                        }
                        HttpRuntime.Cache.Add("IPProxy", list, null, DateTime.Now.AddMinutes(min), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
                    }
                }
            }
            var ipProxydict = HttpRuntime.Cache["IPProxy"] as List<string>;
            if (ipProxydict == null || ipProxydict.Count == 0)
            {
                return null;
            }
            lock (Rang)
            {
                _nonius++;
                if (_nonius >= ipProxydict.Count)
                {
                    _nonius = 0;
                }
                var temp = IpPortReg.Match(ipProxydict[_nonius]);
                return new WebProxy(temp.Groups["ip"].Value, int.Parse(temp.Groups["port"].Value));
            }
        }


        /// <summary>
        /// 二进制文件post 方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileKeyName">参数名称 如：pic</param>
        /// <param name="filePath">地址： 图片地址</param>
        /// <param name="stringDict">其他post参数</param>
        /// <returns></returns>
        public static string HttpPostData(string url, string filePath, NameValueCollection stringDict, string authInfo)
        {
            //string responseContent;
            var memStream = new MemoryStream();
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            // 边界符
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            // 边界符
            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

            // 最后的结束符
            var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");

            // 设置属性
            webRequest.Method = "POST";
            webRequest.Timeout = 30000;
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

            webRequest.Headers.Add("Authorization", "OAuth2 " + authInfo);
            webRequest.AllowWriteStreamBuffering = true;

            if (!string.IsNullOrEmpty(filePath))
            {
                using (Stream stream = webRequest.GetRequestStream())
                {
                    try
                    {
                        byte[] buffer = BuildPostData(boundary, stringDict, filePath);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    finally
                    {
                        stream.Close();
                    }

                }
            }
            string res;
            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    res = reader.ReadToEnd();
                    reader.Close();
                }
                response.Close();
            }

            return res;

            //const string filePartHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: \"image/unknow\"\r\nContent-Transfer-Encoding: binary\r\n\r\n";
            //var header = string.Format(filePartHeader, fileKeyName, filePath);

            //var headerbytes = Encoding.UTF8.GetBytes(header);

            //memStream.Write(beginBoundary, 0, beginBoundary.Length);
            //memStream.Write(headerbytes, 0, headerbytes.Length);

            //var picInfo = GetImageContent(filePath);

            //memStream.Write(picInfo, 0, picInfo.Length);

            //// 写入字符串的Key
            //const string stringKeyHeader = "content-disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            //foreach (byte[] formitembytes in from string key in stringDict.Keys
            //                                 select string.Format(stringKeyHeader, key, stringDict[key])
            //                                     into formitem
            //                                     select Encoding.UTF8.GetBytes(formitem))
            //{
            //    memStream.Write(formitembytes, 0, formitembytes.Length);
            //}

            //// 写入最后的结束边界符
            //memStream.Write(endBoundary, 0, endBoundary.Length);

            //webRequest.ContentLength = memStream.Length;

            //var requestStream = webRequest.GetRequestStream();
            //webRequest.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

            //webRequest.AllowWriteStreamBuffering = true;

            //memStream.Position = 0;
            //var tempBuffer = new byte[memStream.Length];
            //memStream.Read(tempBuffer, 0, tempBuffer.Length);
            //memStream.Close();

            //requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            //requestStream.Close();

            //var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
            ////OAuth2 AccessToken
            //httpWebResponse.Headers.Add("Authorization", "OAuth2 2.00ePQX9DYkYWOC341a34e6c20qrRAh");
            //using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(),
            //                                                Encoding.GetEncoding("utf-8")))
            //{
            //    responseContent = httpStreamReader.ReadToEnd();
            //}

            //httpWebResponse.Close();
            //webRequest.Abort();
            //return responseContent;
        }

        /// <summary>
        /// 设置post 参数
        /// </summary>
        /// <param name="boundary"></param>
        /// <param name="nvc"></param>
        /// <param name="picPath"></param>
        /// <returns></returns>
        private static byte[] BuildPostData(string boundary, NameValueCollection nvc, string picPath)
        {

            //list.Sort(new WeiboParameterComparer());
            MemoryStream stream = new MemoryStream();
            byte[] bytes = Encoding.ASCII.GetBytes(string.Format("\r\n--{0}\r\n", boundary));
            byte[] buffer = Encoding.ASCII.GetBytes(string.Format("\r\n--{0}--", boundary));

            foreach (byte[] formitembytes in from string key in nvc.Keys
                                             select string.Format("content-disposition: form-data; name=\"{0}\"\r\n\r\n{1}", key, nvc[key])
                                                 into formitem
                                                 select Encoding.UTF8.GetBytes(formitem))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Write(formitembytes, 0, formitembytes.Length);
            }
            var picInfo = GetImageContent(picPath);
            stream.Write(bytes, 0, bytes.Length);

            const string format = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: \"image/unknow\"\r\nContent-Transfer-Encoding: binary\r\n\r\n";
            byte[] buffer4 = Encoding.UTF8.GetBytes(string.Format(format, "pic", string.Format("upload{0}", BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0))));
            stream.Write(buffer4, 0, buffer4.Length);
            byte[] buffer5 = picInfo;
            stream.Write(buffer5, 0, buffer5.Length);


            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0L;
            byte[] buffer6 = new byte[stream.Length];
            stream.Read(buffer6, 0, buffer6.Length);
            stream.Close();
            stream.Dispose();
            return buffer6;
        }


        /// <summary>
        /// 将url 地址中的图片转成二进制数
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static byte[] GetImageContent(string url)
        {


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AllowAutoRedirect = true;

            WebProxy proxy = new WebProxy();
            proxy.BypassProxyOnLocal = true;
            proxy.UseDefaultCredentials = true;

            request.Proxy = proxy;

            WebResponse response = request.GetResponse();

            using (Stream stream = response.GetResponseStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Byte[] buffer = new Byte[1024];
                    int current;
                    while (stream != null && (current = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        ms.Write(buffer, 0, current);
                    }
                    return ms.ToArray();
                }
            }
        }




        public static void AsyncHttpRequest(string maiurl, string paramurl)
        {
            using (WebClient client = new WebClient())
            {
                var Postbyte = Encoding.ASCII.GetBytes(paramurl);
                client.UploadDataAsync(new Uri(maiurl), "POST", Postbyte);
            }
        }



        ////http://www.2cto.com/kf/201208/149159.html
        ///// <summary>
        ///// post 提交二进制文件
        ///// </summary>
        ///// <param name="url"></param>
        ///// <param name="timeOut"></param>
        ///// <param name="fileKeyName"></param>
        ///// <param name="filePath"></param>
        ///// <param name="stringDict"></param>
        ///// <returns></returns>
        //private static string HttpPostData(string url, int timeOut, string fileKeyName,
        //                            string filePath, NameValueCollection stringDict)
        //{
        //    string responseContent;
        //    var memStream = new MemoryStream();
        //    var webRequest = (HttpWebRequest)WebRequest.Create(url);
        //    // 边界符
        //    var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        //    // 边界符
        //    var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
        //    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        //    // 最后的结束符
        //    var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");

        //    // 设置属性
        //    webRequest.Method = "POST";
        //    webRequest.Timeout = timeOut;
        //    webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

        //    // 写入文件
        //    const string filePartHeader =
        //        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
        //         "Content-Type: application/octet-stream\r\n\r\n";
        //    var header = string.Format(filePartHeader, fileKeyName, filePath);
        //    var headerbytes = Encoding.UTF8.GetBytes(header);

        //    memStream.Write(beginBoundary, 0, beginBoundary.Length);
        //    memStream.Write(headerbytes, 0, headerbytes.Length);

        //    var buffer = new byte[1024];
        //    int bytesRead; // =0

        //    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
        //    {
        //        memStream.Write(buffer, 0, bytesRead);
        //    }

        //    // 写入字符串的Key
        //    var stringKeyHeader = "\r\n--" + boundary +
        //                           "\r\nContent-Disposition: form-data; name=\"{0}\"" +
        //                           "\r\n\r\n{1}\r\n";

        //    foreach (byte[] formitembytes in from string key in stringDict.Keys
        //                                     select string.Format(stringKeyHeader, key, stringDict[key])
        //                                         into formitem
        //                                         select Encoding.UTF8.GetBytes(formitem))
        //    {
        //        memStream.Write(formitembytes, 0, formitembytes.Length);
        //    }

        //    // 写入最后的结束边界符
        //    memStream.Write(endBoundary, 0, endBoundary.Length);

        //    webRequest.ContentLength = memStream.Length;

        //    var requestStream = webRequest.GetRequestStream();

        //    memStream.Position = 0;
        //    var tempBuffer = new byte[memStream.Length];
        //    memStream.Read(tempBuffer, 0, tempBuffer.Length);
        //    memStream.Close();

        //    requestStream.Write(tempBuffer, 0, tempBuffer.Length);
        //    requestStream.Close();

        //    var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();

        //    using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(),
        //                                                    Encoding.GetEncoding("utf-8")))
        //    {
        //        responseContent = httpStreamReader.ReadToEnd();
        //    }

        //    fileStream.Close();
        //    httpWebResponse.Close();
        //    webRequest.Abort();

        //    return responseContent;
        //}
        //public string DownLoadImg(string url)
        //{
        //    //var aa =  GetImageContent(url);
        //    if (string.IsNullOrEmpty(url) || (!url.Contains("http://") && !url.Contains("https://")))
        //        return "";
        //    int retry = 0;
        //    while (true)
        //    {
        //        string strGethtml = "";
        //        try
        //        {
        //            HttpWebRequest mywr = (HttpWebRequest)WebRequest.Create(url);

        //            mywr.Method = "GET";
        //            mywr.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/*,*/*;q=0.8";
        //            mywr.ContentType = "image/jpeg";

        //            mywr.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36";
        //            mywr.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
        //            mywr.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
        //            mywr.Referer = "http://list.jd.com/list.html?cat=1672,2599,1445&page=379&stock=0&delivery=2&area=1,72,4137";

        //            mywr.KeepAlive = true;
        //            mywr.Timeout = RequestTimeout;


        //            var mywrp = mywr.GetResponse();

        //            Stream responseStream = mywrp.GetResponseStream();
        //            //if (mywrp.ContentEncoding.ToLower().Contains("gzip"))
        //            //{
        //            //    if (responseStream != null)
        //            //        responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
        //            //}

        //            //int scode = (Int32)mywrp.StatusCode;
        //            //if (scode != 200)
        //            //    return "";
        //            if (responseStream == null) return strGethtml;
        //            StreamReader sr = new StreamReader(responseStream, Encoding.UTF8);
        //            strGethtml = sr.ReadToEnd().Trim();
        //            mywrp.Close();
        //            sr.Close();
        //            return strGethtml;
        //        }
        //        catch (Exception ex)
        //        {
        //            if (retry < 3)
        //            {
        //                retry++;
        //                Thread.Sleep(1000);
        //                continue;
        //            }
        //            LogServer.WriteLog("get url:" + url + ex, "HtmlAnalysis");
        //            return "";
        //        }
        //    }

        //}
    }
}
