using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Commons;

namespace AamirKhan
{
    public partial class Domain : Form
    {
        public Domain()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            List<ProInfo> prolist = new List<ProInfo>();
            for (int i = 0; i < 100; i++)
            {
                prolist.Add(new ProInfo { Id = i });
            }
            DownLoadQueueThread dqt = new DownLoadQueueThread(prolist);
            dqt.AllCompleted += AllCompleted;
            dqt.OneCompleted += onecompleted;
            dqt.Start();

            List<ProInfo> prolist1 = new List<ProInfo>();
            for (int i = 0; i < 100; i++)
            {
                prolist1.Add(new ProInfo { Id = i });
            
            }
            dqt.addBindDate(prolist1);
        }
        public void AllCompleted(QueueThreadBase<ProInfo>.CompetedEventArgs cea)
        {
            LogServer.WriteLog("所有任务已经完成");
        }

        public void onecompleted(ProInfo pro, QueueThreadBase<ProInfo>.CompetedEventArgs cea)
        {
            LogServer.WriteLog(pro.Id+"执行已经完成,总任务数量"+cea.QueueCount+"已完成"+cea.CompletedCount+"已完成"+cea.CompetedPrecent+ "%");
        }
    }
}
