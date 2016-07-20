using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BLL;
using Mode;

namespace AamirKhan
{
    public partial class webBrowserForm : Form
    {
        public webBrowserForm()
        {
            InitializeComponent();
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
            DomainCookies cook = new DomainCookies
            {
                Domain=domain,
                Url=txtUrl.Text,
                Cookies = txtCookies.Text,
                UserAgent = txtUserAgent.Text
            };
            new DomainCookiesBll().SaveCookies(cook);

            //string html = webBrowser.DocumentText;
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
        #endregion

 
    }
}
