using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MHttpHelper;
using Newtonsoft.Json.Linq;

namespace ChangeIPTool
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            timerInfo.Interval = 1000*180; //三分钟执行一次
            timerInfo.Start();
            string config = GetJsonConfig("IpConfig");
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
            SetJsonConfig(config.ToString(), "IpConfig");
            new Server().ChangeIp(txtLinkName.Text, txtUserName.Text, txtpwd.Text);
        }

        private string GetJsonConfig(string fileName)
        {
            string strFilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("\\", "/") + "/Config/";
            if (!Directory.Exists(strFilePath))
                return "";
            if (fileName.IndexOf('.') == -1)
                fileName += ".json";
            string filefullName = strFilePath + fileName;
            FileInfo file = new FileInfo(filefullName);
            if (file.Exists)
            {
                var fileinfo = file.OpenText();
                string content = fileinfo.ReadToEnd();
                fileinfo.Close();
                fileinfo.Dispose();
                return content;
            }
            return "";

        }
        private void SetJsonConfig(string json, string fileName)
        {

            string strFilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("\\", "/") + "/Config/";
            if (!Directory.Exists(strFilePath))
            {
                Directory.CreateDirectory(strFilePath);
            }
            if (fileName.IndexOf('.') == -1)
                fileName += ".json";
            string FileName = strFilePath + fileName;
            FileInfo file = new FileInfo(FileName);
            if (file.Exists)
            { file.Delete(); }
            FileStream stream = file.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            stream.Seek(0, SeekOrigin.End);
            byte[] buffer1 = Encoding.UTF8.GetBytes(json);
            stream.Write(buffer1, 0, buffer1.Length);
            //byte[] buffer2 = new byte[] { Convert.ToByte('\r'), Convert.ToByte('\n') };
            //stream.Write(buffer2, 0, 2);
            stream.Flush();
            stream.Close();

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
