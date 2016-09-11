using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChangeIPTool
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            timerInfo.Interval = 30000;
            timerInfo.Start();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            new Server().ChangeIp(txtLinkName.Text, txtUserName.Text, txtpwd.Text);
        }

        private void timerInfo_Tick(object sender, EventArgs e)
        {
            new Server().TimerDoing(txtLinkName.Text, txtUserName.Text, txtpwd.Text);
        }

        private void btmTime_Click(object sender, EventArgs e)
        {
            int min;

            if (!int.TryParse(txtTimeSpan.Text, out min))
            {
                min = 10;
            }

            timerIpChang.Interval = min * 1000*60;
            timerIpChang.Start();
            btmTime.Text = "定时开始";
            btmTime.Enabled = false;
        }

        private void timerIpChang_Tick(object sender, EventArgs e)
        {
            new Server().ChangeIp(txtLinkName.Text, txtUserName.Text, txtpwd.Text);
        }

        private void btnlySubmit_Click(object sender, EventArgs e)
        {
            new Server().Roustreconnect(txtLyUrl.Text, txtlyName.Text, txtlyPwd.Text);
        }
    }
}
