using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace GuardWinServer
{
    public class CompressServer
    {
        /// <summary>
        /// 对byte数组进行压缩
        /// </summary>
        /// <param name="data">待压缩的byte数组</param>
        /// <returns>压缩后的byte数组</returns>
        public static byte[] Compress(byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(data, 0, data.Length);
            zip.Close();
            byte[] buffer = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buffer, 0, buffer.Length);
            ms.Close();
            return buffer;
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// 将程序压缩并打包
        /// </summary>
        public static void PackServerPluse()
        {
            if (!Directory.Exists(SystemConfig.PackFilePath)||string.IsNullOrEmpty(SystemConfig.ToGuardProcessName))
                return;

            var list = GetAllFiles(new DirectoryInfo(SystemConfig.PackFilePath));
            
            var fdict = new Dictionary<string, byte[]>();

            foreach (var file in list)
            {
                if(Regex.IsMatch(file, SystemConfig.PackExcludeFilesRex,RegexOptions.Singleline))
                    continue;
                fdict.Add(file.Replace(SystemConfig.PackFilePath, ""), Compress(File.ReadAllBytes(file)));
              
            }
       
            string strFilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("\\", "/") + "Pack/";
            if (!Directory.Exists(strFilePath))
            {
                Directory.CreateDirectory(strFilePath);
            }
            var bytes = Encoding.UTF8.GetBytes(ServiceStack.Text.JsonSerializer.SerializeToString(fdict));
            bytes = Compress(bytes);
            File.WriteAllBytes(strFilePath + SystemConfig.ToGuardProcessName + ".mz", bytes);
            var getMd5 = new MD5CryptoServiceProvider();
            byte[] hashByte = getMd5.ComputeHash(bytes);
            string resule = BitConverter.ToString(hashByte);
            resule = resule.Replace("-", "");
            File.WriteAllText(strFilePath + SystemConfig.ToGuardProcessName + ".md5", resule);
        }


        private static List<string> GetAllFiles(DirectoryInfo dir) //搜索文件夹中的文件
        {
            var fileList = dir.GetFiles().Select(fi => fi.FullName).ToList();
            var allDir = dir.GetDirectories();
            foreach (var d in allDir)
            {
                fileList.AddRange(GetAllFiles(d));
            }
            return fileList;
        }

    }
}
