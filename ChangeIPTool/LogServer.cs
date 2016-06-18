using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ChangeIPTool
{
    public class LogServer
    {
        static string _logfilename = "logfile";
        public static string LogFileName { get { return _logfilename; } set { _logfilename = value; } }

        //public static ILog Log4Net
        //{
        //    get { return (ILog)AppDomain.CurrentDomain.GetData("Log4NetManager"); }
        //}
        /// <summary>
        /// 写入错误日志文件
        /// </summary>
        public static void WriteLog(string msg)
        {
            WriteLog(msg, "Error", LogFileName);
        }

        public static void WriteLog(Exception ex)
        {
            string msg = ex.ToString();
            WriteLog(msg, "Error", LogFileName);
        }
        public static void WriteLog(Exception ex, string fileNamePreFix)
        {
            string msg = ex.ToString();
            WriteLog(msg, fileNamePreFix, LogFileName);
        }
        public static void WriteLog(string msg, string fileNamePreFix)
        {
            WriteLog(msg, fileNamePreFix, LogFileName);
        }

        /// <summary>
        /// 添加错误日志
        /// </summary>
        /// <param name="msg">错误信息</param>
        /// <param name="fileNamePreFix">日志文件名称</param>
        /// <param name="documentName">存放日志的文件夹名称</param>
        public static void WriteLog(string msg, string fileNamePreFix, string documentName)
        {
            string doName = @"Log";
            if (!string.IsNullOrEmpty(documentName))
            {
                doName = @"/" + documentName + @"/" + DateTime.Now.ToString("yyyyMMdd") + @"/";
            }
            string strFilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("\\", "/");
            //strFilePath = strFilePath.Substring(0, strFilePath.Length - 1);
            //strFilePath = strFilePath.Substring(0, strFilePath.LastIndexOf("/")); 
            string logfilePath = strFilePath + "/" + documentName;
            if (GetDirectoryLength(logfilePath) > 1024 * 1024 * 1024 * 1L)
            {
                //超过1GB 数据不写入
                return;
            }
            if (Directory.Exists(logfilePath))
            {
                var logfiles = Directory.GetFileSystemEntries(logfilePath);
                if (logfiles.Length > 15)
                {
                    for (int i = 0; i < logfiles.Length - 15; i++)
                    {
                        DeleteFolder(logfiles[i]);
                    }

                }
            }


            strFilePath = strFilePath + doName;
            if (!Directory.Exists(strFilePath))
            {
                Directory.CreateDirectory(strFilePath);
            }

            msg = string.Format("【Time:{0}】{1}", DateTime.Now, msg);
            Write(strFilePath + fileNamePreFix + ".log", msg);
        }

        public static int FileIndex = 0;

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="fileName">完整路径加文件名</param>
        /// <param name="contentString">日志内容</param>
        /// <returns></returns>
        static readonly object Lockwrite = new object();
        private static void Write(string fileName, string contentString)
        {

            try
            {
                var file = new FileInfo(fileName);
                if (file.Exists && file.Length > 1024 * 1024 * 10)    // 大于 10M 不写入数据
                {
                    return;
                }
                lock (Lockwrite)
                {
                    FileStream stream = file.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                    stream.Seek(0, SeekOrigin.End);
                    byte[] buffer1 = Encoding.GetEncoding("gb2312").GetBytes(contentString);
                    stream.Write(buffer1, 0, buffer1.Length);
                    byte[] buffer2 = { Convert.ToByte('\r'), Convert.ToByte('\n') };
                    stream.Write(buffer2, 0, 2);
                    stream.Flush();
                    stream.Close();
                }
            }
            catch
            {
            }
            //finally
            //{
            //    Monitor.Exit(Lockwrite);
            //}

        }

        /// <summary>
        /// 删除文件夹(包括子文件夹和文件)
        /// </summary>
        /// <param name="dir"></param>
        private static void DeleteFolder(string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                    return;
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                        File.Delete(d);
                    else DeleteFolder(d);
                }
                Directory.Delete(dir);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 获取文件夹的大小
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns>字节k</returns>
        public static long GetDirectoryLength(string dirPath)
        {
            //判断给定的路径是否存在,如果不存在则退出
            if (!Directory.Exists(dirPath))
                return 0;
            //定义一个DirectoryInfo对象
            var di = new DirectoryInfo(dirPath);
            //通过GetFiles方法,获取di目录中的所有文件的大小
            var len = di.GetFiles().Sum(fi => fi.Length);
            //获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
            DirectoryInfo[] dis = di.GetDirectories();
            if (dis.Length <= 0) return len;
            len += dis.Sum(t => GetDirectoryLength(t.FullName));
            return len;
        }

        public static DateTime ReadLogRowNo(string fileName)
        {
            string doName = @"Log";
            if (!string.IsNullOrEmpty(LogFileName))
            {
                doName = @"/" + LogFileName + @"/" + DateTime.Now.ToString("yyyyMMdd") + @"/";
            }
            string strFilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("\\", "/");
            string logfilePath = strFilePath.TrimEnd('/')  + doName;
            string logfile = logfilePath+ fileName + ".log";
            if (!System.IO.File.Exists(logfile))
                return DateTime.MinValue;
            try
            {
                StreamReader sr = new StreamReader(logfile, Encoding.Default);
           
                var text = sr.ReadToEnd();
         
                sr.Dispose();
                if (!string.IsNullOrEmpty(text))
                {
                    string[] list = text.Split('\n');
                    if (list.Length > 2)
                    {
                        string endline = list[list.Length - 2];
                        var time = Regex.Match(endline, "【Time:(?<x>.*?)】", RegexOptions.Singleline).Groups["x"].Value;
                        DateTime updatetime = DateTime.MinValue;
                        DateTime.TryParse(time, out updatetime);
                        return updatetime;

                    }
              
                }
                return DateTime.MinValue;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }

        }
    }
}
