using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;
using Commons;
using Servers;
using SpiderFormsApp.code;

namespace SpiderFormsApp
{
    public partial class MainForm : Form
    {
  
        public MainForm()
        {
            
            InitializeComponent();
            DataInit();
            Text = "Spider v2.0";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            new SqlMonitorSystem().Begin();
        }

        private void DataInit()
        {
            MessageCenter.RegistErrorControl(rtbError);
            MessageCenter.RegisterMessageControl(rtbMsg);
            SpiderConfigBll config = new SpiderConfigBll();
            try
            {
                var list = config.LoadSpiderconfig();
                if (list.Count > 0)
                {
                    cbbCaseInfo.DataSource = list;
                    cbbCaseInfo.DisplayMember = "TaskName";
                    cbbCaseInfo.ValueMember = "Id";
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex);
            }
          
          
        }



        private void btnStart_Click(object sender, EventArgs e)
        {
            var config = new SpiderConfigBll().GetSpiderConfig(Convert.ToInt32(cbbCaseInfo.SelectedValue));
            SpiderProListServer spserver = new SpiderProListServer();
            spserver.CaseInit(config);
            spserver.ThreadCount = 1;
            spserver.Begin();
            btnStart.Enabled = false;
        }



        private void 抓取方案配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            SpiderProListServer.Quitout = true;
            btnStart.Enabled = true;
        }

        private void rtbError_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private static bool IsHostRun;
        private void btnHostProxy_Click(object sender, EventArgs e)
        {
            MessageCenter.ShowBox("代理服务获取开始！", 2);
            new HostServer().LoadHost();
            MessageCenter.ShowBox("代理服务获取完毕！", 2);
        }

        private void validHost()
        {
            while (IsHostRun)
            {
                try
                {
                    MessageCenter.ShowBox("代理服务验证开始执行！", 2);
                    new HostServer().validallHost();
                    MessageCenter.ShowBox("代理服务验证结束！", 2);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            }
        }

        private void HostTimer_Tick(object sender, EventArgs e)
        {
            if (!btnHostProxy.Enabled)
                return;
            int second = DateTime.Now.Second;
            int minute = DateTime.Now.Minute;
            int hour = DateTime.Now.Hour;

            //将可用的代理发到服务器端供客户端调用  5分钟执行一次
            if (minute % 5 == 0 && second == 0)
            {
                Task.Factory.StartNew(postHost);
            }
            //代理验证是否可用（快速验证） 3分钟执行一次
            if (minute % 3 == 0 && second == 0)
            {
                Task.Factory.StartNew(validAvailable);
            }
            //从网站上获取代理 2小时执行一次
            if (hour % 2 == 0 && minute == 30 && second == 0)
            {
                Task.Factory.StartNew(loadhost);
            }
        }

        private void postHost()
        {
            MessageCenter.ShowBox("代理服务开始发送！", 2);
            new HostServer().postHost();
            MessageCenter.ShowBox("代理服务发送结束！", 2);
        }
        private void loadhost()
        {
            MessageCenter.ShowBox("代理服务获取开始！", 2);
            new HostServer().LoadHost();
            MessageCenter.ShowBox("代理服务获取完毕！", 2);
        }
        private void validAvailable()
        {
            MessageCenter.ShowBox("代理服务测试可用IP开始！", 2);
            new HostServer().validAvailable();
            MessageCenter.ShowBox("代理服务测试可用IP结束！", 2);
        }

        

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            IsHostRun = true;
            HostTimer.Interval = 1000;
            HostTimer.Start();
            //代理验证是否可用 所以ip代价进行验证
            Task.Factory.StartNew(validHost);


        }

        private void 评论抓取ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Enabled = false;
            MessageCenter.Dispose();
            Comments pst = new Comments(this);
            pst.ShowDialog();
        }

        private void btnApiSpider_Click(object sender, EventArgs e)
        {
            new SpiderProListServer().getItemsByApi();
        }

        private void browserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Enabled = false;
            MessageCenter.Dispose();
            webBrowserSystem pst = new webBrowserSystem(this);
            pst.ShowDialog();
        }
    }
}
