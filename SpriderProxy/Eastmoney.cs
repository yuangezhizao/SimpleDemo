using Commons;

namespace SpriderProxy
{
    public class Eastmoney : BaseSiteInfo
    {
        //http://hqres.eastmoney.com/EMQuote_Center2.0/js/list.min.js  

        private static string token = "7bc05d0d4c3c22ef9fca8c2a912d779c";


        public override bool ValidCatId(string catId)
        {
            return true;
        }
        private static string ADataModel(string style)
        {
          
            string cmd;

            switch (style)
            {
                case "33": //沪深A股
                    cmd = "C._A";
                    break;
                case "10": //沪A股
                    cmd = "C.2";
                    break;
                case "11": //沪B股
                    cmd = "C.3";
                    break;
                case "20": //深证A股
                    cmd = "C._SZAME";
                    break;
                case "21": //深证B股
                    cmd = "C.7";
                    break;
                case "27": //创业板
                    cmd = "C.80";
                    break;
                case "15": //上海指数
                    cmd = "C.1";
                    break;
                case "25": //深圳指数
                    cmd = "C.5";
                    break;
                case "14.1.1":
                    cmd = "C._DEBT_SH_G";
                    break;
                case "14.2.1":
                    cmd = "C._DEBT_SH_Q";
                    break;
                case "14.3":
                    cmd = "C._DEBT_SH_Z";
                    break;
                case "2850020":
                    cmd = "C._DEBT_SH_H";
                    break;
                case "24.1":
                    cmd = "C._DEBT_SZ_G";
                    break;
                case "24.2":
                    cmd = "C._DEBT_SZ_Q";
                    break;
                case "24.3":
                    cmd = "C._DEBT_SZ_Z";
                    break;
                case "2850021":
                    cmd = "C._DEBT_SZ_H";
                    break;
                case "285002":
                    cmd = "C.__285002";
                    break;
                case "2850013":
                    cmd = "C.__2850013";
                    break;
                case "2850014":
                    cmd = "C.__2850014";
                    break;
                case "2850022": //风险警示板
                    cmd = "C._AB_FXJS";
                    break;
                case "26": //中小板
                    cmd = "C._ME";
                    break;
                default: //板块概念
                    cmd = "C.BK0" + style.Substring(style.Length - 3, style.Length) + "1";
                    break;
            }
           // string sortType = "C";// var sortType=-1 //pageSize=10000 sortRule=
            var host = "http://nufm.dfcfw.com/EM_Finance2014NumericApplication/JS.aspx";
            var reqparams = "?type=CT&cmd=" + cmd + "&sty=FCOIATA&sortType=C&sortRule=-1&page=1&pageSize=10000&js=var%20quote_123%3d{rank:[(x)],pages:(pc)}&&token=" + token+"&jsName=quote_123";
            return host + reqparams;
        }

        public static string GetStockInfo(string style)
        {
            string url = ADataModel(style);
            string page =new HtmlAnalysis().HttpRequest(url);
           string result= RegGroupsX<string>(page, "var quote_123=(?<x>.*?)$");
            return result;
        }
    }
}
