namespace WebServer
{
    public class JdApiConfig
    {

        public static string AppKey => System.Configuration.ConfigurationManager.AppSettings["JDAppKey"] ?? "";
        public static string AppSecret => System.Configuration.ConfigurationManager.AppSettings["JDAppSecret"] ?? "";

        public const string AuthUrl = "https://oauth.jd.com/oauth/authorize?";

        public const string BackApiUrl = "http://fl.ljsprayer.com/api.aspx";
    }
}