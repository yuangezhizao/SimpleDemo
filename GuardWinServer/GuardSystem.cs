﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace GuardWinServer
{

    
    /// <summary>
    /// 守护服务 确保某一个执行程序一直在执行
    /// 可以用作云更新 这样不需要一个一个替换应用程序了
    /// </summary>
    public class GuardSystem
    {
        public static string Localmd5 { get; set; }

        /// <summary>
        /// 标识进程停止是否是在更新程序集
        /// </summary>
        public static bool UpdateRun;
        public static void DoMain()
        {
            string proessFullName = SystemConfig.ToGuardProcessPath + SystemConfig.ToGuardProcessName + ".exe";

            if (string.IsNullOrEmpty(SystemConfig.ToGuardProcessName))
                return;
            //验证守护进程是否已启动
            //if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Count() > 1)
            //{
            //    LogServer.WriteLog(Process.GetCurrentProcess().ProcessName + "-- isruning", "GuardSystem");
            //    return;
            //}
            //监视工具是否停止
            var couth = new Thread(() =>
            {
                while (true)
                {

                    if (!Process.GetProcessesByName(SystemConfig.ToGuardProcessName).Any() && !UpdateRun)
                    {
                        if (File.Exists(proessFullName))
                        {
              
                            Process.Start(proessFullName, "restart");
                            LogServer.WriteLog(proessFullName + " --restart UpdateRun:" + UpdateRun, "GuardSystem");
                        }
                    }
                 
                    Thread.Sleep(10000);
                }
                // ReSharper disable once FunctionNeverReturns
            })
            { IsBackground = false };
            couth.Start();

            if (File.Exists(SystemConfig.ToGuardProcessPath + SystemConfig.ToGuardProcessName + ".md5"))
            {
                Localmd5 = File.ReadAllText(SystemConfig.ToGuardProcessPath + SystemConfig.ToGuardProcessName + ".md5");
            }
            string md5Url = SystemConfig.CloudScourUrl + SystemConfig.ToGuardProcessName + ".md5";
            string mzurl = SystemConfig.CloudScourUrl + SystemConfig.ToGuardProcessName + ".mz";
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
                        var bytes = client.DownloadData(mzurl);
                        LogServer.WriteLog(Process.GetCurrentProcess().ProcessName + "----start " + mzurl, "GuardSystem");
                        bytes = CompressServer.Decompress(bytes);
                        string json = Encoding.UTF8.GetString(bytes);

                        var fdict = ServiceStack.Text.JsonSerializer.DeserializeFromString<Dictionary<string, byte[]>>(json);
                        File.WriteAllText("stop", "");
                        DateTime start = DateTime.Now;
                  
                        while (Process.GetProcessesByName(SystemConfig.ToGuardProcessName).Any())
                        {
                            TimeSpan span = DateTime.Now - start;
                            //如果30分钟任务还没结束、就强制结束进程替换
                            if (span.TotalMinutes > 30||SystemConfig.IsAutoStop)
                            {
                                foreach (var item in Process.GetProcessesByName(SystemConfig.ToGuardProcessName))
                                {
                                    item.Kill();
                                }
                                break;
                            }
                            if(!SystemConfig.IsAutoStop)
                            Thread.Sleep(5000);
                        }
                        if (File.Exists("stop"))
                        {
                            File.Delete("stop");
                        }
                 
                        foreach (var item in fdict)
                        {
                            try
                            {
                                var temppath = SystemConfig.ToGuardProcessPath + item.Key;
                                temppath = temppath.Substring(0, temppath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
                                if (!string.IsNullOrEmpty(temppath) && !Directory.Exists(temppath))
                                {
                                    Directory.CreateDirectory(temppath);
                                }
                                File.WriteAllBytes(SystemConfig.ToGuardProcessPath+ item.Key, CompressServer.Decompress(item.Value));
                                LogServer.WriteLog("下载并解压dll  " + item.Key, "GuardSystem");
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine(ex);
                                LogServer.WriteLog(ex, "GuardSystem");
                            }
                        }
                        Localmd5 = servermd5;
                        File.WriteAllText(SystemConfig.ToGuardProcessPath + SystemConfig.ToGuardProcessName + ".md5", servermd5);
                        //清除缓存的文件列表信息
                        Process.Start(proessFullName, "restart");
                        UpdateRun = false;
                        LogServer.WriteLog("云更新完成--"+ proessFullName, "GuardSystem");
                    }
                    //15分钟
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    UpdateRun = false;
                    LogServer.WriteLog(ex, "GuardSystem");
                    //Console.WriteLine(ex);
                    Thread.Sleep(10000);
                }
            }
        }

    }
}
