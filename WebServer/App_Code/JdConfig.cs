namespace WebServer
{
    public class JdApiConfig
    {

        public static string AppKey
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["JDAppKey"] ?? ""; }
        }

        public static string AppSecret
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["JDAppSecret"] ?? ""; }
        }

        public const string AuthUrl = "https://oauth.jd.com/oauth/authorize?";

        public const string BackApiUrl = "http://fl.ljsprayer.com/api.aspx";
    }
}