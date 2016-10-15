using System;
using System.Text;
using System.Windows.Forms;
using Commons;
using Newtonsoft.Json.Linq;

namespace ChangeIPTool
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            timerInfo.Interval = 30000;
            timerInfo.Start();
            string config = DocumentServer.GetJsonConfig("IpConfig");
            if (string.IsNullOrEmpty(config))
                return;
            var conJson = JToken.Parse(config);
            txtLinkName.Text = conJson["LinkName"].Value<string>();
            txtUserName.Text = conJson["UserName"].Value<string>();
            txtpwd.Text = conJson["Pwd"].Value<string>();
            txtTimeSpan.Text = conJson["TimeSpan"].Value<string>();

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            StringBuilder config = new StringBuilder("{");
            config.AppendFormat("\"LinkName\":\"{0}\",\"UserName\":\"{1}\",\"Pwd\":\"{2}\",\"TimeSpan\":\"{3}\"", txtLinkName.Text, txtUserName.Text, txtpwd.Text,txtTimeSpan.Text);
            config.Append("}");
            DocumentServer.SetJsonConfig(config.ToString(), "IpConfig");
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
            btmTime.Text = @"定时开始";
            btmTime.Enabled = false;
        }

        private void timerIpChang_Tick(object sender, EventArgs e)
        {
            try
            {
                new Server().ChangeIp(txtLinkName.Text, txtUserName.Text, txtpwd.Text);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex);
            }

        }

    }
}
