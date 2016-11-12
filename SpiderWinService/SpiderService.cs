using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using BLL;
using Commons;


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
            LogServer.WriteLog("OnStart....", "timeinfo");
            System.Timers.Timer sysTimer = new System.Timers.Timer();
            sysTimer.Elapsed += SysTimer_Elapsed;
            sysTimer.Interval = 1000 * 60;
            sysTimer.AutoReset = true;
            sysTimer.Enabled = true;
        }
        private void SysTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            LogServer.WriteLog(DateTime.Now.ToString(),"timeinfo");

            //if ((DateTime.Now.Hour == 20 ||DateTime.Now.Hour == 20) &&
            //    DateTime.Now.Minute == 1)
            //{
            //    getItemsByApi();
            //}

            //在高峰期发送
            //if ((DateTime.Now.Hour == 7 || DateTime.Now.Hour == 11 || DateTime.Now.Hour == 17 || DateTime.Now.Hour == 20) && DateTime.Now.Minute == 1)
            //    new SpriderSystem().SendWeibo();

                //每6小时运行一次获取优惠的信息
                //if (DateTime.Now.Hour%6 == 0 && DateTime.Now.Minute==5)
                //{
                //    new SpriderSystem().SaveSitePromo();
                //}

        }
        private void getItemsByApi()
        {
            int[] Sites = { 2 };
            Parallel.ForEach(Sites, num =>
            {
                Stopwatch t1 = new Stopwatch();
                t1.Start();
                LogServer.WriteLog("商城分类更新开始执行 siteid：" + num, "RunInfo");
                try
                {
                    // new SiteFactory { SiteId = num }.ProIApiManager.AddNewProducts();
                    new SiteFactory { SiteId = num }.ProIApiManager.GetAllProducts();
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog("商城分类更新异常 siteid：" + num + "\r\n" + ex.Message, "RunInfo");
                }
                t1.Stop();
                LogServer.WriteLog(string.Format("商城分类更新结束 siteid：{0}耗时{1}小时{2}分钟{3}秒", num, t1.Elapsed.Hours, t1.Elapsed.Minutes, t1.Elapsed.Seconds), "RunInfo");
            });
        }
        protected override void OnStop()
        {
            LogServer.WriteLog("OnStop....", "timeinfo");
        }
    }

   
}
