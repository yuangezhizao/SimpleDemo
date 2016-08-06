using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using BLL;
using Commons;
using FastVerCode;
using Mode;

namespace AamirKhan
{
    public partial class webBrowserForm : Form
    {
        private string reqUrl = "";
        private string reqCookies = "";
        private const string jdloginUrl = "https://passport.jd.com/new/login.aspx?ReturnUrl=https%3A%2F%2Forder.jd.com%2Fcenter%2Flist.action";
        private const string jdOrderUrl = "http://trade.jd.com/shopping/order/getOrderInfo.action";

        private string uname = "";
        private string upwd = "";

        private string jdcookies = "";

        private List<SiteUserInfo> JuserInfo;
        private int CurrentUserIndex;


        public webBrowserForm()
        {
            InitializeComponent();
            ChangeUserAgent("Mozilla/5.0 (Windows NT 10.0; WOW64; rv:47.0) Gecko/20100101 Firefox/47.0");
            webBrowser.ScriptErrorsSuppressed = false;
        }

        private void tbnGo_Click(object sender, EventArgs e)
        {
            string url = txtUrl.Text;
         
            if (string.IsNullOrEmpty(url))
                return;
            if (!url.Contains("http"))
            {
                url = "http://" + url;
            }
            webBrowser.Navigate(url);
        }
        private void webBrowser_NewWindow(object sender, CancelEventArgs e)
        {
            webBrowser.Url = new Uri(((WebBrowser)sender).StatusText);
            e.Cancel = true;
        }
        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser.Document == null)
                return;
            txtUrl.Text = webBrowser.Url.ToString();
            txtCookies.Text = GetCookieString(txtUrl.Text);
            txtUserAgent.Text = GetDefaultUserAgent();
            Regex reg = new Regex(@"(?<=://)([\w-]+\.)+[\w-]+(?<=/?)");
            string domain= reg.Match(txtUrl.Text, 0).Value.Replace("/", string.Empty);

            if (txtUrl.Text.Contains("passport.jd.com"))
            {
                jdlogin(uname, upwd);
            }
            if (txtUrl.Text.Contains("order.jd.com"))
            {
                domain = uname + "_Cookies";
            }
            //if (txtUrl.Text==jdOrderUrl)
            //{
            //    jdBuy();
            //}

            if (reqUrl == txtUrl.Text)
            {
                reqCookies = txtCookies.Text;
            }

            SiteCookies cook = new SiteCookies
            {
                Domain=domain,
                Url=txtUrl.Text,
                Cookies = txtCookies.Text,
                UserAgent = txtUserAgent.Text
            };
            string html = webBrowser.DocumentText;

            new SiteCookiesBll().SaveCookies(cook);

  
        }

        #region 公共方法

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchUrl, string pchCookieName, StringBuilder pchCookieData, ref UInt32 pcchCookieData, int dwFlags, IntPtr lpReserved);
        public static string GetCookieString(string url)
        {
            uint datasize = 1024;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x2000, IntPtr.Zero))
            {
                if (datasize <= 0)
                    return null;
                cookieData = new StringBuilder((int)datasize);
                if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, IntPtr.Zero))
                    return null;
            }
            return cookieData.ToString();
        }

        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);
        const int URLMON_OPTION_USERAGENT = 0x10000001;
        /// <summary>
        /// 修改UserAgent
        /// </summary>
        public static void ChangeUserAgent(string userAgent)
        {
            UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, userAgent, userAgent.Length, 0);
        }

        /// <summary>
        /// 一个很BT的获取IE默认UserAgent的方法
        /// </summary>
        private static string GetDefaultUserAgent()
        {
            WebBrowser wb = new WebBrowser();
            wb.Navigate("about:blank");
            while (wb.IsBusy) Application.DoEvents();
            object window = wb.Document.Window.DomWindow;
            Type wt = window.GetType();
            object navigator = wt.InvokeMember("navigator", BindingFlags.GetProperty,
                null, window, new object[] { });
            Type nt = navigator.GetType();
            object userAgent = nt.InvokeMember("userAgent", BindingFlags.GetProperty,
                null, navigator, new object[] { });
            return userAgent.ToString();
        }




        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll ")]
        public static extern System.IntPtr FindWindowEx(System.IntPtr parent, System.IntPtr childe, string strclass, string strname);
        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, uint wMsg, int wParam, int lParam);
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow", SetLastError = true)]
        private static extern void SetForegroundWindow(IntPtr hwnd);


        private void moniclick()
        {
            const uint BM_CLICK = 0xF5;

            //取得消息框的句柄
            IntPtr p = FindWindowEx(System.IntPtr.Zero, System.IntPtr.Zero, null, "Choose a digital certificate");

            //取得OK按钮的句柄
            IntPtr hwndok = FindWindowEx(p, System.IntPtr.Zero, null, "Ok");

            //显示到前端
            SetForegroundWindow(p);

            //模拟点击按钮
            SendMessage(hwndok, BM_CLICK, 0, 0);

            this.Close();
        }


        #endregion




        private void jdlogin(string username,string pwd)
        {
            HtmlElement loginname = webBrowser.Document.All["loginname"];
            HtmlElement nloginpwd = webBrowser.Document.All["nloginpwd"];

            //$("#loginsubmit").click()
            loginname?.SetAttribute("value", username);
            nloginpwd?.SetAttribute("value", pwd);
            HtmlElement frmlogin = webBrowser.Document.GetElementById("loginsubmit");

           // string vurl = "https://passport.jd.com/uc/showAuthCode?r=0.046490722928461015&version=2015";
           // string uname = HttpUtility.UrlEncode(username);
           //var vpage= HtmlAnalysis.HttpRequestFromPost(vurl, "loginName=" + uname,"utf-8");

           // if (vpage.Contains("\"verifycode\":true"))
           // {
           //     HtmlElement verification = webBrowser.Document.GetElementById("JD_Verification1");
           //     string imgurl = verification?.GetAttribute("src2");
           //     if (!string.IsNullOrEmpty(imgurl))
           //     {
           //         WebClient myWebClient = new WebClient();
           //         var imge = myWebClient.DownloadData("http:" + imgurl);
           //         string returnMess = VerCode.RecByte_A(imge, imge.Length, "maiden", "52zhuzhu", "");
           //         if (returnMess.IndexOf("|", StringComparison.Ordinal) > 0)
           //             returnMess = returnMess.Substring(0, returnMess.IndexOf("|", StringComparison.Ordinal));
           //         HtmlElement Authcode = webBrowser.Document.GetElementById("authcode");
           //         if (Authcode != null) nloginpwd.SetAttribute("value", returnMess);

           //     }
           // }
        

            if (frmlogin != null) frmlogin.InvokeMember("Click");
        }

        private void JdLoginTimer_Tick(object sender, EventArgs e)
        {
            if (JuserInfo == null || JuserInfo.Count == 0)
            {
                JuserInfo= new SiteUserInfoBll().GetAllUser();
                CurrentUserIndex = 0;
            }
            if(CurrentUserIndex >= JuserInfo.Count)
            { CurrentUserIndex = 0; }

            uname = JuserInfo[CurrentUserIndex].UserName;
            upwd = JuserInfo[CurrentUserIndex].UserPwd;
            webBrowser.Navigate(jdloginUrl);
            CurrentUserIndex++;
        }

        private void btnJDLogin_Click(object sender, EventArgs e)
        {
            JuserInfo = new SiteUserInfoBll().GetAllUser();
            CurrentUserIndex = 0;
            JdLoginTimer.Interval = 10000;
            //JdLoginTimer.Start();

            uname = JuserInfo[0].UserName;
            upwd = JuserInfo[0].UserPwd;

            new JdOrderSystem().Login(uname,upwd);
            //webBrowser.Navigate(jdloginUrl);

        }
    }
}
