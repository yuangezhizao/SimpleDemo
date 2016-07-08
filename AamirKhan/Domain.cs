using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;
using BLL.Sprider.Stock;
using Commons;
using Mode;

namespace AamirKhan
{
    public partial class Domain : Form
    {
        public Domain()
        {
            InitializeComponent();
            MessageCenter.ProgressBarControl(progressBar);
            MessageCenter.RegisterMessageControl(rtbMsg);
            SpiderTimer.Interval = 1000*60;
            SpiderTimer.Start();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //List<ProInfo> prolist = new List<ProInfo>();
            //for (int i = 0; i < 50; i++)
            //{
            //    prolist.Add(new ProInfo { Id = i });
            //}
            var prolist = new StockInfoBll().GetAllinfo();
            int count;
            int.TryParse(txtTheadCount.Text, out count);

            progressBar.Maximum = prolist.Count;
            txTotalTask.Text = progressBar.Maximum.ToString();

            lvTheadDetial.Items.Clear();
            for (var i = 0; i < count; i++)
            {
                ListViewItem li = new ListViewItem();
                li.SubItems[0].Text = (i + 1).ToString();
                li.SubItems.Add("准备更新");
                li.SubItems.Add("");
                lvTheadDetial.Items.Add(li);
            }

            DownLoadQueueThread dqt = new DownLoadQueueThread(prolist);
            dqt.ThreadCount = count;
            dqt.AllCompleted += AllCompleted;
            dqt.OneCompleted += Onecompleted;
            dqt.Start();

            //List<ProInfo> prolist1 = new List<ProInfo>();
            //for (int i = 0; i < 100; i++)
            //{
            //    prolist1.Add(new ProInfo { Id = i });
            //}
            //dqt.addBindDate(prolist1);
            progressBar.Minimum = 0;
            progressBar.Step = 1;
            //progressBar.Maximum =150;

        }



        public void AllCompleted(QueueThreadPlusBase<StockInfo>.CompetedEventArgs cea)
        {
            LogServer.WriteLog("所有任务已经完成");
        }

        public void Onecompleted(int index, StockInfo pro, QueueThreadPlusBase<StockInfo>.CompetedEventArgs cea)
        {
            //progressBar.Value = cea.CompletedCount;
            MessageCenter.ProgressBarShow(cea.CompletedCount);
            var msg =  pro.StockNo + "执行已经完成,总任务数量" + cea.QueueCount + "已完成" + cea.CompletedCount + "已完成" +
                      cea.CompetedPrecent + "%";
            MessageCenter.ListViewMsg(index, msg);
            MessageCenter.ShowBox(msg,2);
            //LogServer.WriteLog(pro.Id+"执行已经完成,总任务数量"+cea.QueueCount+"已完成"+cea.CompletedCount+"已完成"+cea.CompetedPrecent+ "%");
        }

        private void Domain_Load(object sender, EventArgs e)
        {
            lvTheadDetial.GridLines = true;
            lvTheadDetial.FullRowSelect = true;
            lvTheadDetial.View = View.Details;
            lvTheadDetial.Scrollable = true;
            lvTheadDetial.MultiSelect = false; 
            lvTheadDetial.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvTheadDetial.Columns.Add("编号", 30, HorizontalAlignment.Center);
            lvTheadDetial.Columns.Add("执行情况", 400, HorizontalAlignment.Left);
            lvTheadDetial.Columns.Add("时间", 80, HorizontalAlignment.Left);
            lvTheadDetial.MouseDown += threadstatus_MouseDown;
            MessageCenter.RegisterListViewMsgBoxControl(lvTheadDetial);
        }

        void threadstatus_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    var mMbRpt = lvTheadDetial.PointToClient(MousePosition);
                    ListViewItem lstrow = lvTheadDetial.GetItemAt(mMbRpt.X, mMbRpt.Y);
                    Clipboard.SetDataObject(lstrow.SubItems[1].Text);
                }
                catch
                {

                }
                //MessageBox.Show(lstrow.SubItems[1].Text);
            }
        }

        private void AccentToolStripMenuItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.Hide();
            //this.Enabled = false;
            //MessageCenter.Dispose();
            Accent pst = new Accent();
            pst.ShowDialog();
        }

        private void webBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.Hide();
            //this.Enabled = false;
            //MessageCenter.Dispose();
            webBrowserForm pst = new webBrowserForm();
            pst.ShowDialog();
        }

        private void SpiderTimer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Hour == 15 && DateTime.Now.Minute == 5)
            {
                new SiteFactory().StockInfoManager.GetALlStockInfo();
            }
        }
    }
}
