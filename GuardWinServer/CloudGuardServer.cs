using System.ServiceProcess;

namespace GuardWinServer
{
    public partial class CloudGuardServer : ServiceBase
    {
        public CloudGuardServer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {


            //CompressServer.PackServer(@"E:\work\cloudmanmanbuy\mmbSpider\bin\Release\PackConfig.txt");
            LogServer.WriteLog("OnStart....", "CloudGuardServer");
            GuardSystem.DoMain();

            //System.Timers.Timer sysTimer = new System.Timers.Timer();
            //sysTimer.Elapsed += SysTimer_Elapsed;
            //sysTimer.Interval = 1000 * 60;
            //sysTimer.AutoReset = true;
            //sysTimer.Enabled = true;
        }

        //private void SysTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    LogServer.WriteLog(DateTime.Now.ToString(), "timeinfo");
        //}

        protected override void OnStop()
        {

            //GuardSystem.IsRun = false;
            LogServer.WriteLog("OnStop....", "CloudGuardServer");
            base.OnStop();
        }
    }
}
