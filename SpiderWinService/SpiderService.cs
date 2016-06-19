using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Servers;

namespace SpiderWinService
{
    public partial class SpiderService : ServiceBase
    {
        public SpiderService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ServerCenter();

            System.Timers.Timer sysTimer = new System.Timers.Timer();
            sysTimer.Elapsed += SysTimer_Elapsed;
            sysTimer.Interval = 1000 * 60;
            sysTimer.AutoReset = true;
            sysTimer.Enabled = true;
        }
        private void SysTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //在高峰期发送
            //if ((DateTime.Now.Hour == 7 || DateTime.Now.Hour == 11 || DateTime.Now.Hour == 17 || DateTime.Now.Hour == 20) && DateTime.Now.Minute == 1)
            //    new SpriderSystem().SendWeibo();

            //每6小时运行一次获取优惠的信息
            //if (DateTime.Now.Hour%6 == 0 && DateTime.Now.Minute==5)
            //{
            //    new SpriderSystem().SaveSitePromo();
            //}

        }

        private void ServerCenter()
        {
            //new SpriderSystem().Start();
            //Task.Factory.StartNew(SendWeibo);
        }
        protected override void OnStop()
        {
        }
    }
}
