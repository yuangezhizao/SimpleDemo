using System;
using System.Windows.Forms;
using Servers;

namespace SpiderFormsApp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //new SpriderSystem().Start();
            Application.Run(new MainForm());
        }
    }
}
