using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace GuardWinServer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            //string proessFullName = SystemConfig.ToGuardProcessPath + SystemConfig.ToGuardProcessName + ".exe";
            //Process.Start(proessFullName, "restart");
            //GuardSystem.DoMain();
            var servicesToRun = new ServiceBase[]
            {
                new CloudGuardServer()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
