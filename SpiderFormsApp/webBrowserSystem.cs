using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpiderFormsApp
{
    public partial class webBrowserSystem : Form
    {
        MainForm mainform;
        public webBrowserSystem()
        {
            InitializeComponent();
        }
        public webBrowserSystem(MainForm form)
        {
            InitializeComponent();
            string loginurl =
            "https://login.taobao.com/member/login.jhtml?&enup=false&redirectURL=https%3a%2f%2flist.tmall.com%2fsearch_product.htm%3fcat%3d50024400%26sort%3ds%26auction_tag%3d7809style%3dg%26vmarket%3d0%26search_condition%3d48";

            wbrowser.Navigate(loginurl);
            mainform = form;

        }
        private void tbnGo_Click(object sender, EventArgs e)
        {
            string url = txtUrl.Text;
            if(string.IsNullOrEmpty(url))
                return;
            if (!url.Contains("http"))
            {
                url = "http://" + url;
            }
            wbrowser.Navigate(txtUrl.Text);
        }

        private void btnlogin_Click(object sender, EventArgs e)
        {
        

            if (wbrowser.Document != null)
            {
                HtmlElement loginname = wbrowser.Document.All["TPL_username_1"];
                HtmlElement loginpwd = wbrowser.Document.All["TPL_password_1"];
                loginname?.SetAttribute("value", txtName.Text);
                loginpwd?.SetAttribute("value", txtPwd.Text);
                HtmlElement frmlogin = wbrowser.Document.Forms["J_Form"];
                frmlogin?.InvokeMember("submit");
            }
          
        }

        private void wbrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wbrowser.Document == null)
                return;
            txtUrl.Text = wbrowser.Url.ToString();
            txtCookies.Text = GetCookieString(txtUrl.Text);
            txtUserAgent.Text = GetDefaultUserAgent();
            string html = wbrowser.DocumentText;
        }




        #region 公共方法
        
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchUrl, string pchCookieName, StringBuilder pchCookieData, ref System.UInt32 pcchCookieData, int dwFlags, IntPtr lpReserved);
        public static string GetCookieString(string url)
        {
            uint datasize = 1024;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x2000, IntPtr.Zero))
            {
                if (datasize < 0)
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

        private void webBrowserSystem_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            mainform.Enabled = true;
            mainform.Show();
        }
    }
}
