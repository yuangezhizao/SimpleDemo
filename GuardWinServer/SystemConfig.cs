

namespace GuardWinServer
{
    public static class SystemConfig
    {
        public static readonly string CloudScourUrl = @"http://bijiatool.manmanbuy.com/uptools/";
        public static readonly string PackFilePath = @"E:\work\cloudmanmanbuy\mmbSpider\bin\Release\";
        public static readonly string PackExcludeFilesRex = @"JdzyConfig.json|.pdb$|.vshost.";
        public static readonly bool IsAutoStop = true;
        public static string ToGuardProcessName = "mmbSpider";
        public static string ToGuardProcessPath = @"C:\Users\Administrator\Desktop\jdzy\";
    }
}
