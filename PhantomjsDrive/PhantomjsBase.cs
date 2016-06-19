using System;
using System.Diagnostics;

namespace PhantomjsDrive
{
    //注意 需要配置环境变量 PATH 下添加 D:\casperjs\bin （Phantomjs.exe 所在目录）
    //官网地址 http://phantomjs.org
    public class PhantomjsBase
    {
        public static string PhantomjsPath { get; set; }
        public string DownLoadbuycmd(string url)
        {
            string path = PhantomjsPath ?? Environment.CurrentDirectory;
            string arguments = string.Format(@"phantomjs --load-images=false {0} {1}", path + "Config\\DownLoad.js", url);
            arguments = arguments.Replace("&", "^&");//cmd 转码
            using (Process p = new Process
            {
                StartInfo =
                {
                    FileName = @"cmd.exe",  //配置电脑的环境变量 phantomjs所在目录(path   D:\casperjs\bin )
                    WorkingDirectory=path,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            })
            {
                p.Start();
                p.StandardInput.WriteLine(arguments);
                p.StandardInput.Close();
                p.StandardInput.Dispose();
                string strRst;
                try
                {
                    strRst = p.StandardOutput.ReadToEnd();
                }
                catch (Exception)
                {
                    return "";
                }
                p.Close();
                return strRst;
            }
        }

        protected string DownLoad(string url)
        {
            string path = PhantomjsPath ?? Environment.CurrentDirectory+"\\";
            string arguments = string.Format(@"--load-images=false {0} {1}", path + "Config\\DownLoad.js", url);
            var result = DownLoad(path, arguments);
            return result;
        }

        protected string DownLoadPost(string url)
        {
            string path = PhantomjsPath ?? Environment.CurrentDirectory + "\\";
            string arguments = string.Format(@"--load-images=false {0} {1}", path + "Config\\DownLoadPost.js", url);
            var result = DownLoad(path, arguments);
            return result;
        }

        protected string DownLoad(string path, string arguments)
        {

            #region 启动进程
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = path + "Config\\phantomjs.exe",
                WorkingDirectory = path,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                Arguments = arguments
            };
            try
            {
                var p = Process.Start(psi);
                var result = p.StandardOutput.ReadToEnd();
                p.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion
        }
    }
}
