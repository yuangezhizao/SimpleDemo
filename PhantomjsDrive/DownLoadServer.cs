namespace PhantomjsDrive
{
    public class DownLoadServer : PhantomjsBase
    {
        public string GetJdListInfo(string url)
        {

            return DownLoad(@"D:\aa\trunk\PhantomjsDrive\js\DownLoad.js", url);
        }

        public string GetFengquListInfo(string url)
        {

            return DownLoad(url);
        }

        public string DownLoadpage(string url, string method= "GET")
        {
            if (method.ToUpper() == "POST")
                return DownLoadPost(url);
            else
                return DownLoad(url);

        }
    }
}
