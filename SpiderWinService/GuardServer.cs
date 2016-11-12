using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace SpiderWinService
{
    /// <summary>
    /// 守护服务 确保某一个执行程序一直在执行
    /// 可以用作云更新 这样不需要一个一个替换应用程序了
    /// </summary>
    public class GuardServer
    {
        public static string Localmd5 { get; set; }
        /// <summary>
        /// 更新服务器地址
        /// </summary>
        public const string Upserver = "http://bijiatool.manmanbuy.com/uptools/";
        /// <summary>
        /// 标识进程停止是否是在更新程序集
        /// </summary>
        public static bool UpdateRun;
        public static void DoMain(string[] args)
        {
            if (args == null || args.Length != 2)
            {
                return;
            }
            //验证守护进程是否已启动
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Count() > 1)
            {
                return;
            }
            //程序集名称
            string assembly = args[0];
            //进程名称
            string course = args[1];
            //监视工具是否停止
            var couth = new Thread(() =>
            {
                while (true)
                {
                    if (!Process.GetProcessesByName(course).Any() && !UpdateRun)
                    {
                        if (File.Exists(course + ".exe"))
                        {
                            Process.Start(course + ".exe", "restart");
                        }
                    }
                    Thread.Sleep(10000);
                }
                // ReSharper disable once FunctionNeverReturns
            }) {IsBackground = false};
            couth.Start();
            if (File.Exists(assembly + ".md5"))
            {
                Localmd5 = File.ReadAllText(assembly + ".md5");
            }
            string md5Url = Upserver + assembly + ".md5";
            string mzurl = Upserver + assembly + ".mz";
            while (true)
            {
                try
                {
                    WebClient client = new WebClient();
                    string servermd5 = client.DownloadString(md5Url);
                    if (servermd5.Length > 50 || servermd5.Length < 10)
                    {
                        servermd5 = "";
                    }

                    if (!string.IsNullOrEmpty(servermd5) && servermd5 != Localmd5)
                    {
                        UpdateRun = true;
                        File.WriteAllText("stop", "");
                        DateTime start = DateTime.Now;
                        while (Process.GetProcessesByName(course).Any())
                        {
                            TimeSpan span = DateTime.Now - start;
                            //如果30分钟任务还没结束、就强制结束进程替换
                            if (span.TotalMinutes > 30)
                            {
                                foreach (var item in Process.GetProcessesByName(course))
                                {
                                    item.Kill();
                                }
                                break;
                            }
                            Thread.Sleep(5000);
                        }
                        if (File.Exists("stop"))
                        {
                            File.Delete("stop");
                        }
                        var bytes = client.DownloadData(mzurl);
                        bytes = Decompress(bytes);
                        string json = Encoding.UTF8.GetString(bytes);
                        var fdict = ServiceStack.Text.JsonSerializer.DeserializeFromString<Dictionary<string, byte[]>>(json);
                        foreach (var item in fdict)
                        {
                            try
                            {
                                var d = Path.GetDirectoryName(item.Key);
                                if (!string.IsNullOrEmpty(d) && !Directory.Exists(d))
                                {
                                    Directory.CreateDirectory(d);
                                }
                                File.WriteAllBytes(item.Key, Decompress(item.Value));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        Localmd5 = servermd5;
                        File.WriteAllText(assembly + ".md5", servermd5);
                        //清除缓存的文件列表信息
                        Process.Start(course + ".exe", "restart");
                        UpdateRun = false;
                    }
                    //15分钟
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    UpdateRun = false;
                    Console.WriteLine(ex);
                    Thread.Sleep(10000);
                }
            }
        }
        public static byte[] Decompress(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true);
            MemoryStream msreader = new MemoryStream();
            byte[] buffer = new byte[0x1000];
            while (true)
            {
                int reader = zip.Read(buffer, 0, buffer.Length);
                if (reader <= 0)
                {
                    break;
                }
                msreader.Write(buffer, 0, reader);
            }
            zip.Close();
            ms.Close();
            msreader.Position = 0;
            buffer = msreader.ToArray();
            msreader.Close();
            return buffer;
        }
    }
}
