using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;
using BLL.Sprider.Stock;
using Commons;
using Mode;
using Servers;

namespace AamirKhan
{
    public partial class Domain : Form
    {
        public Domain()
        {
            InitializeComponent();
          
            MessageCenter.ProgressBarControl(progressBar);
            MessageCenter.RegisterMessageControl(rtbMsg);
            SpiderTimer.Interval = 5000;
            SpiderTimer.Start();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
        
            StockDayReport();
        }

        public void StockDayReport()
        {
            new StockMonitorBll().DelCurrentDatXqStock();
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

            var dqt = new StockSpiderServer(prolist) {ThreadCount = count};
            dqt.AllCompleted += AllCompleted;
            dqt.OneCompleted += Onecompleted;
            dqt.Start();

            progressBar.Minimum = 0;
            progressBar.Step = 1;
        }

        private void getNewStock()
        {
            new TaskFactory().StartNew(new SiteFactory().StockInfoManager.GetALlStockInfo);
        }


        public void AllCompleted(QueueThreadPlusBase<StockInfo>.CompetedEventArgs cea)
        {

            if(cbxVolid.Checked)
                new StockMonitorBll().VolidReportCount();
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
                catch(Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
                //MessageBox.Show(lstrow.SubItems[1].Text);
            }
        }

        private void AccentToolStripMenuItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Accent pst = new Accent();
            pst.ShowDialog();
        }

        private void webBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowserForm pst = new webBrowserForm();
            pst.ShowDialog();
          
    
  
           
        }

        private void SpiderTimer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                return;
            if (dtpTime.Text != @"0:00:00")
            {
                if (DateTime.Now.Hour == dtpTime.Value.Hour && DateTime.Now.Minute == dtpTime.Value.Minute &&
                  DateTime.Now.Second > 30&& DateTime.Now.Second < 36 )
                {

                    LogServer.WriteLog("定时任务开始执行", "StockDayReport");
                    StockDayReport();
                    getNewStock();
                }
            }
            if (DateTime.Now < Convert.ToDateTime("09:30") || DateTime.Now > Convert.ToDateTime("15:00")) return;
            if (DateTime.Now > Convert.ToDateTime("11:30") && DateTime.Now < Convert.ToDateTime("13:30")) return;
            new TaskFactory().StartNew(new StockInfoBll().StockMonitor);
        }

        private void cbxTime_CheckedChanged(object sender, EventArgs e)
        {
            SpiderTimer.Start();
        }
    }
}
