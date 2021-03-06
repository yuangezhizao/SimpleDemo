﻿using System;
using System.Linq;
using System.Windows.Forms;
using Commons;

namespace AamirKhan
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
            _progressBar = null;
        }
        static RichTextBox _priceUpdataMsgbox;
        static RichTextBox _priceUpdataErrbox;
        private static ProgressBar _progressBar;
        static ListView _listViewMsgBox;
        static readonly object PriceUpdataMsgSync = new object();//同步锁
        static readonly object PriceUpdataErrSync = new object();//同步锁
        static readonly object progressBarSync = new object();//同步锁
        delegate void ShowMessageHandler(string msgText);

        delegate void ProgressBarHandler(int addstep);

        private delegate void ListViewMessageHandler(int Itemid,  string msgText);

        public static void RegisterListViewMsgBoxControl(ListView control)
        {
            if (_listViewMsgBox == null)
                _listViewMsgBox = control;
        }
        public static void ListViewMsg(int index , string msgText)
        {
            if (_listViewMsgBox.InvokeRequired)
            {
                _listViewMsgBox.Invoke(new ListViewMessageHandler(ListViewMsg), index, msgText);
            }
            else
            {
                _listViewMsgBox.Items[index].SubItems[0].Text = index.ToString();
                _listViewMsgBox.Items[index].SubItems[1].Text = msgText;
                _listViewMsgBox.Items[index].SubItems[2].Text = DateTime.Now.ToString("HH:mm:ss");

            }
        }
        public static void ProgressBarControl(ProgressBar control)
        {
            if (_progressBar == null)
                _progressBar = control;
        }

        public static void ProgressBarShow(int currentIndex)
        {
            if (_progressBar.InvokeRequired)
            {
                _progressBar.Invoke(new ProgressBarHandler(ProgressBarShow), currentIndex);
            }
            else
            {
                lock (progressBarSync)
                {
                    _progressBar.Value = currentIndex;
                }


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
