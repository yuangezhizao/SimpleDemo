using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GuardSystem
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        //[STAThread]
        //static void Main()
        //{
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new Form1());
        //}
        [STAThread]
        static void Main(string[] args)
        {
            //CompressServer.PackServerPluse();
            if (args != null && args.Length > 0)
            {
                if (args[0] == "Pack")
                {
                    if (args.Length > 1)
                        SystemConfig.ToGuardProcessName = args[1];
                    if (args.Length > 2)
                        SystemConfig.PackFilePath = args[2];
                    if (args.Length > 3)
                        SystemConfig.PackExcludeFilesRex = args[3];

                    CompressServer.PackServerPluse();
                }
                else
                {
                    SystemConfig.ToGuardProcessName = args[0];
                    GuardSystem.DoMain();
                }

            }
            else
            {
                GuardSystem.DoMain();
            }

        }

    }
 
}
