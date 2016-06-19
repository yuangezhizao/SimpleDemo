using System;
using System.Linq;
using System.Windows.Forms;
using Commons;

namespace SpiderFormsApp.code
{
    public class MessageCenter
    {
        public MessageCenter()
        { StopAppendMsg = false; }

        //public void addLog(String s)
        //{
        //    LogServer.WriteLog(s);
        //}
        public static bool StopAppendMsg { get; set; }
        public static void Dispose()
        {
            _priceUpdataMsgbox = null;
            _priceUpdataErrbox = null;
        }
        static RichTextBox _priceUpdataMsgbox;
        static RichTextBox _priceUpdataErrbox;
        static ListView _listViewMsgBox;
        static readonly object PriceUpdataMsgSync = new object();//同步锁
        static readonly object PriceUpdataErrSync = new object();//同步锁
        delegate void ShowMessageHandler(string msgText);

        private delegate void ListViewMessageHandler(int Itemid, string site, string url, string msgText);

        public static void RegisterListViewMsgBoxControl(ListView control)
        {
            if (_listViewMsgBox == null)
                _listViewMsgBox = control;
        }
        public static void ListViewMsg(int Itemid ,string site,string url, string msgText)
        {
            if (_listViewMsgBox.InvokeRequired)
            {
                _listViewMsgBox.Invoke(new ListViewMessageHandler(ListViewMsg), Itemid, site, url, msgText);
            }
            else
            {
                _listViewMsgBox.Items[Itemid].SubItems[1].Text = site;
                _listViewMsgBox.Items[Itemid].SubItems[2].Text = url;
                _listViewMsgBox.Items[Itemid].SubItems[3].Text = msgText;
                _listViewMsgBox.Items[Itemid].SubItems[4].Text = DateTime.Now.ToString("HH:mm:ss");

            }
        }

        public static void RegisterMessageControl(RichTextBox control)
        {
            if (_priceUpdataMsgbox == null)
                _priceUpdataMsgbox = control;
        }
        public static void RegistErrorControl(RichTextBox control)
        {
            if (_priceUpdataErrbox == null)
                _priceUpdataErrbox = control;
        }
        private static void ShowMsg(string msgText)
        {
            if (_priceUpdataMsgbox.InvokeRequired)
            {
                _priceUpdataMsgbox.Invoke(new ShowMessageHandler(ShowMsg), msgText);
            }
            else
            {
                lock (PriceUpdataMsgSync)
                {
                    if (StopAppendMsg)
                        return;
                    if (_priceUpdataMsgbox.Lines.Count() > 300)
                        _priceUpdataMsgbox.Clear();
                    if (msgText == "所有线程都已经退出")
                    {
                        _priceUpdataMsgbox.Enabled = false;
                    }
                    _priceUpdataMsgbox.ScrollToCaret();
                    _priceUpdataMsgbox.AppendText(string.Format("{0} {1}\t\n", DateTime.Now.ToString("HH:mm:ss"), msgText));
                }
            }
        }
        private static void ShowError(string errText)
        {
            if (_priceUpdataErrbox == null)
            {
                return;
            }
            if (_priceUpdataErrbox.InvokeRequired)
            {
                _priceUpdataErrbox.Invoke(new ShowMessageHandler(ShowError), errText);
            }
            else
            {
                lock (PriceUpdataErrSync)
                {
                    if (StopAppendMsg)
                        return;
                    if (_priceUpdataErrbox.Lines.Count() > 300)
                        _priceUpdataErrbox.Clear();
                    _priceUpdataErrbox.ScrollToCaret();
                    _priceUpdataErrbox.AppendText(string.Format("{0} {1}\t\n", DateTime.Now.ToString("HH:mm:ss"), errText));
                }
            }
        }
        /// <summary>
        /// 在窗口显示错误信息
        /// </summary>
        /// <param name="s">错误信息</param>
        /// <param name="type">类型 1 错误2 消息</param>
        public static void ShowBox(string s, int type)
        {
            if (type == 1)
            {
                ShowError(s);
                LogServer.WriteLog(s);

            }
            if (type == 2)
            {
                ShowMsg(s);
            }


        }
    }
}
