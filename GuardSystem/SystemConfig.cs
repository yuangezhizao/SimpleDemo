using System.Collections.Generic;

namespace GuardSystem
{
    public static class SystemConfig
    {
        /// <summary>
        /// 下载压缩包的路径
        /// </summary>
        public static readonly string CloudScourUrl = @"http://bijiatool.manmanbuy.com/uptools/";
        /// <summary>
        /// 需要打包的源文件的地址
        /// </summary>
        public static string PackFilePath = @"C:\Users\Administrator\Desktop\ShopRobot\source";
        /// <summary>
        /// 需要排除的文件
        /// </summary>
        public static string PackExcludeFilesRex = @"\\assets\\|\\js\\|\\View\\|.pdb$|.vshost.";
        public static readonly bool IsAutoStop = true;
        /// <summary>
        /// 
        /// </summary>
        public static string ToGuardProcessName = "ShopRobot";
     
    }
}
