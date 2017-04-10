using BLL;
using Commons;
using Servers;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BLL.Sprider.Stock;
using Mode;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhantomjsDrive;

namespace WebAppClinet
{
    public partial class Web : System.Web.UI.Page
    {
        protected JsonServiceClient Client { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

            //new SpriderSystem().SaveSiteCate(241);
            //new SpriderSystem().UpdateSiteCat(241);
            //new SiteClassBll().UpdatemmbsiteClass(241);
            //new StockInfoBll().GetNewStockInfo();
            //SubmitOrder("");
            //var aa = ranCode();
            //loadjdUser();
            test();
            //string[] cats =
            //{
            //    "1000",
            //    "3000",
            //    "38000",
            //    "4000",
            //    "2000",
            //    "2000",
            //    "230000",
            //    "5000",
            //    "225000",
            //     "43000",
            //      "34000",
            //      "36000",
            //    "37000"
            //};
            //foreach (string cat in cats)
            //{
            //    juhuasuan(cat);
            //}
            //qiangtaobao();


        }
        //static Random ran = new Random();
        //private string ranCode()
        //{
        //    float rannum = ran.Next(0, 1000) / 1000;
        //    int num = int.Parse(Math.Round(2147483647 * rannum).ToString());
        //    var res = num ^ 2147483647 & 47427992;
        //    return res.ToString();
        //}

        private void juhuasuan(string cat)
        {
  

            int maxpage = 2;
            for (int i = 1; i < maxpage; i++)
            {
                try
                {
                    string url = $"https://ju.taobao.com/json/tg/ajaxGetItemsV2.json?callback=define&page={i}&psize=20&type=0&frontCatId={cat}";
                    var page = HtmlAnalysis.Gethtmlcode(url).Replace("define(", "").TrimEnd(')');
                    if (string.IsNullOrEmpty(page))
                        continue;
                 
                    var token = JToken.Parse(page);
                    if (i == 1)
                    {
                        maxpage = token["totalPage"].Value<int>();
                    }
                    LogServer.WriteLog(page);
                }
                catch (Exception ex)
                {
                    
                    LogServer.WriteLog(ex);
                }
       
            }
        }

        private void qiangtaobao()
        {
            var t = Convert.ToInt32((DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds);

            string url =
                $"https://api.m.taobao.com/h5/mtop.msp.qianggou.queryitembycategoryid/3.0/?v=3.0&api=mtop.msp.qianggou.queryItemByCategoryId&appKey=12574478&t={t}&callback=mtopjsonp1&type=jsonp&sign=dfbbeb5e9f89294ce6a34e5e51b03535&data=%7B%22categoryId%22%3A%22312000%22%2C%22offset%22%3A50%2C%22limit%22%3A50%7D";
            HtmlAnalysis requent = new HtmlAnalysis();

        }

        private void smsServer()
        {
            var smsmanger = new SiteFactory().SmsApiManager;
            string catid = "121";

            string phone = smsmanger.GetPhoneNum(catid);

            var msm = smsmanger.GetValidateMsg(phone, catid);
        }

        private void SubmitOrder(string skuid)
        {
       
            JdOrderServer server = new JdOrderServer();

            //server.gotoUrl("https://btmkt.jd.com/activity/initParty?key=3e0ff0f1cc6c8265f7cadc64355fd40b&cpdad=1DLSUE");
            var JuserInfo = new SiteUserInfoBll().GetAllUser();
            foreach (var user in JuserInfo)
            {

               server.Login(user.UserName, user.UserPwd);

                server.SetUserInfo(user.UserName);
                //server.SaveAddress();

                //server.AddCat($"{skuid},2477473", "1,1");
                server.ClearCat();
                server.AddCat("3062344,2477374", "1,1");
                server.CatDetial();
                server.SubmitOrder();
                //server.ClearCat();
              

            }

        }


        private void loadjdUser()
        {
            var page = DocumentServer.ReadFileInfo(@"C:\Users\Administrator\Downloads\20160722200040011100980004303485_10_jdwmsbzh.txt","default");
            var list = page.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            var newlist = new List<SiteUserInfo>();
            foreach (var item in list)
            {
                var  templist = item.Split(new string[] { "----" }, StringSplitOptions.RemoveEmptyEntries);

                if(templist.Length<5)
                    continue;

                var userinfo = new SiteUserInfo
                {
                    UserName = templist[0],
                    UserPwd = templist[1],
                    PayPwd = templist[2],
                    PhoneNum = templist[3],
                    Domain = "http://www.jd.com/",
                    SiteName = "京东商城",
                    EmailName = templist[4],
                    EmailPwd = templist[5],
                    Remark = templist[6]+" " +templist[7],
                    LoginCookies = "",
                    AddJdCode = "",
                    AddJdDetial = "",
                    AddJdid = "",
                    Fp ="",
                    Eid = "",
                    UserAgent = "",
                    LoginUpdatetime = DateTime.Now,
                    CreateDate =DateTime.Now

                };
                newlist.Add(userinfo);
            }
            new SiteUserInfoBll().addUser(newlist);
        }
        public string Getjstime()
        {//获取javascript时间
            string time;
            var minTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
            var nowTicks = DateTime.Now.ToUniversalTime().Ticks;
            var ticks = (nowTicks - minTicks) / 10000L;
            time = (ticks / 1000).ToString();
            return time;
        }
        private void test()
        {


            string tepurl =
                "https://mdskip.taobao.com/core/initItemDetail.htm?sellerPreview=false&tryBeforeBuy=false&showShopProm=false&household=false&addressLevel=3&cartEnable=true&isForbidBuyItem=false&isApparel=false&isAreaSell=true&isUseInventoryCenter=true&service3C=false&offlineShop=false&queryMemberRight=true&isRegionLevel=true&itemId=534648284377&tmallBuySupport=true&isPurchaseMallPage=false&isSecKill=false&cachedTimestamp=" + Getjstime();

            HtmlAnalysis request = new HtmlAnalysis();

            var itempage =
                HtmlAnalysis.Gethtmlcode(
                    "https://chaoshi.detail.tmall.com/item.htm?spm=a3204.7933263.0.0.sgJTc9&id=534648284377&rewcatid=50514008", "GBK");

            request.RequestAccept = "application/x-javascript;charset=GBK";
            request.RequestReferer =
                "https://chaoshi.detail.tmall.com/item.htm?spm=a3204.7933263.0.0.sgJTc9&id=534648284377&rewcatid=50514008";
            request.RequestUserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
            //request.Headers.Add("Cookie", "_med=dw:1440&dh:900&pw:1440&ph:900&ist:0; sm4=330200; _tb_token_=Ai1uLykCPq; ck1=; uc3=sg2=V361YQ7FVF5AOC08Dydnr4lcmGQqYHl%2FHI2IabXuLd4%3D&nk2=An0OaqLqaGzx&id2=W8t12UU4MPQ%3D&vt3=F8dARVadCIoLCi0aoWg%3D&lg2=V32FPkk%2Fw0dUvg%3D%3D; lgc=albert533; tracknick=albert533; cookie2=3cb7bb7286fac7b2a29a0396e666cdff; t=44d8050f785496a344695478df479174; skt=601fa1de64224d6a; hng=; uss=VT3hPhx5HrabrXglOD1xiUiiBNYkHBY4Bkz72wur7vDi%2BZMC2q1bi7zgZVQ%3D; otherx=e%3D1%26p%3D*%26s%3D0%26c%3D0%26f%3D0%26g%3D0%26t%3D0; cna=o982EXx2ZnsCAXPVOGrgYf+E; cq=ccp%3D1; isg=AqKiGTtwDkVYWBKR0CZXHDvB8yilSaYNLFHtDew7zpXAv0I51IP2HSg9mUy5; l=Ak9Pn9p-c27KkGOcJrAD1me-X-lZ0KOWif-none-match:W/\"4ba27ee27ba7aec0f3bd3234ad9e8fc2\"");
            var page = request.HttpRequest(tepurl);




            var url =
                "http://list.gome.com.cn/cat10000049-00-0-48-1-0-0-0-1-0-0-0-0-0-0-0-0-0.html?page=2&bws=0x12&type=json";
            //HtmlAnalysis request = new HtmlAnalysis();
            //request.RequestAccept = "application/json, text/javascript, */*; q=0.01";
            //request.RequestUserAgent ="Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
            //request.Headers.Add("Cookie", "route=a4740778113c5452684f57d9d18e1433; uid=CjozJVh3CnG1nZS+FcVEAg==; atgregion=22060100%7C%E6%B5%99%E6%B1%9F%E7%9C%81%E5%AE%81%E6%B3%A2%E5%B8%82%E7%A7%91%E6%8A%80%E5%9B%AD%E5%8C%BA%E7%A7%91%E6%8A%80%E5%9B%AD%E5%8C%BA%7C22060000%7C22000000%7C220601001; __clickidc=148419646713113959; __c_visitor=148419646713113959; _idusin=73057384033; DSESSIONID=683cb316953e4df79d1547141bf10f13; _index_ad=0; cartnum=0; s_cc=true; _ga=GA1.3.249638576.1484196470; _gat=1; gpv_pn=%E5%95%86%E5%93%81%E5%88%97%E8%A1%A8%3A%E5%B9%B3%E6%9D%BF%E7%94%B5%E8%A7%86; gpv_p22=no%20value; s_getNewRepeat=1484197930657-New; s_sq=gome-prd%3D%2526pid%253D%2525E5%252595%252586%2525E5%252593%252581%2525E5%252588%252597%2525E8%2525A1%2525A8%25253A%2525E5%2525B9%2525B3%2525E6%25259D%2525BF%2525E7%252594%2525B5%2525E8%2525A7%252586%2526pidt%253D1%2526oid%253Djavascript%25253Avoid(0)%2526ot%253DA; g_co=show; s_ppv=-%2C33%2C18%2C2375");
            //request.Headers.Add("X-Content-Type-Options", "nosniff");

            //request.Headers.Add("X-Frame-Options", "DENY");
            //request.Headers.Add("X-Info", "201-22-58-06cbgk5");
            //request.Headers.Add("X-X-XSS-Protection", "1; mode=block");
            //request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.RequestReferer = "http://list.gome.com.cn/cat10000049.html";
            var Pagegm = request.HttpRequest(url);
            var aa = "ddd";
            //new StockInfoBll().GetRzrqInfo("SH600219"); 
            //new SiteFactory().StockInfoManager.GetALlStockInfo();
            //return;
            //new BondSpiderServer().UpdateallBond();
            //string feiniuurl = "https://reg.feiniu.com/patcha/image";
            //WebClient myWebClient = new WebClient();
            //var imge = myWebClient.DownloadData(feiniuurl);
            //var filename = ImageServer.DoloadImg(imge, "ddd");

            //string wlfile = "";

            //string key1 = VerCode.GetUserInfo("maiden", "52zhuzhu");
            //string returnMess = VerCode.RecByte_A(imge, imge.Length, "maiden", "52zhuzhu", "");

            //new BenlaiShenhuo().RegionUser();

            //return;
            //string key = "LzMuY";
            //string url = "http://api.hellotrue.com/api/do.php?action=loginIn&name=api-rmm0nm29&password=62415109";
            //HtmlAnalysis request1 = new HtmlAnalysis();
            //request1.RequestContentType = "application/x-www-form-urlencoded";
            //request1.Headers.Add("Cookie", "JSESSIONID=83DE329FB6AFC2FE71B6E455FC6F294D; ASP.NET_SessionId=wgdao2pj2kwoa3jlyn4drz3n; uuk=11002ae4-b7de-40ed-9b65-5cf27f758b97; userGuid=4ac9c1a9-bd89-4827-857b-6143cf2a322720160615101926; WebSiteSysNo=1; DeliverySysNo=2; source=2; o_l_state=b9f164854f211ebf80d6ce483975620a; _pk_id.7.2b60=b6589fc6ab0dc82c.1465957175.1.1465957175.1465957175.; _pk_ses.7.2b60=*; CSESSIONID=83DE329FB6AFC2FE71B6E455FC6F294D; Hm_lvt_9a7d729a11da2966935bcb2908a98794=1465949409,1465953691; Hm_lpvt_9a7d729a11da2966935bcb2908a98794=1465957175; Hm_lvt_7feabb06873cfd158820492f754cc70b=1465949409,1465953691; Hm_lpvt_7feabb06873cfd158820492f754cc70b=1465957175; _qzja=1.834142463.1465957174825.1465957174825.1465957174826.1465957174826.1465957174901.https%253A%252F%252Fm_benlai_com.1.0.2.1; _qzjb=1.1465957174825.2.0.0.0; _qzjc=1; _qzjto=2.1.0; _jzqckmp_v2=1/; _jzqckmp=1/");
            //request1.RequestUserAgent =
            //    "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.87 Safari/537.36";
            //request1.Headers.Add("X-Requested-With", "XMLHttpRequest");
            //request1.Headers.Add("Origin", "https://m.benlai.com");
            //request1.RequestReferer = "https://m.benlai.com/showReg?comeFromApp=0";
            //url = "https://m.benlai.com/regPhoneVry?phoneNumber=13586777526";
            //request1.RequestMethod = "post";
            //var page=request1.HttpRequest(url);
            //string token = page.Replace("1|", "");
            //string getphoneurl = "http://api.hellotrue.com/api/do.php?action=getPhone&sid=1773&token="+ token;
            // page= request1.HttpRequest(getphoneurl);
            //string phone = page.Replace("1|", "");
            //string getmessage = "http://api.hellotrue.com/api/do.php?action=getMessage&sid=1773&phone=" + phone +
            //                    "&token=" + token;
            //page = request1.HttpRequest(getmessage);



            //LogServer.WriteLog("phone"+phone+"token"+token+"message="+getmessage);

            //new SiteFactory().StockInfoManager.GetALlStockInfo();
            //return;17074858678
            string tempurl = "https://xueqiu.com/v4/stock/quote.json?code=SH600886";

            string codedetial = "https://xueqiu.com/s/SZ000002";
            var cookies = new SiteCookiesBll().GetOneByDomain("xueqiu.com");
            //PhantomjsBase.PhantomjsPath = Request.PhysicalApplicationPath;
            //var page = new DownLoadServer().DownLoadpage(tempurl);

            //var aa=   TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)).AddMilliseconds(1464012000000);
            //var bb = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)).AddMilliseconds(1464537599000);
            //string url = "http://chart.windin.com/hqserver/HQProxyHandler.ashx?windcode=600873.SH";
            //string url = "http://licaike.hexun.com/List.action?fundCode=&fundcode=&fundTypes=&isGuaranteedFund=&isQdiiFund=&isIndexFund=&riskLvlt=&nav=&accumulativeNav=&investManner=&fundSize=&fundEstablishDate=&thisYearRate=&weekRiseRate=&monthRiseRate=&month3RiseRate=&month6RiseRate=&yearRiseRate=&year2RiseRate=&year3RiseRate=&cxRank=&htRank=&expan=&companyExpan=&pager.activePage=1";
            //TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)).AddMilliseconds(1464012000000);
            //HtmlAnalysis request = new HtmlAnalysis();
            //request.RequestMethod = "post";
            //request.Headers.Add("Upgrade-Insecure-Requests", "1");
            //request.Headers.Add("Origin", "http://licaike.hexun.com");
            //request.RequestContentType = "application/x-www-form-urlencoded";
            //request.RequestReferer = "http://licaike.hexun.com";
            request.RequestAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            //request.RequestReferer = "https://xueqiu.com/s/SZ000002";
            if (cookies != null)
            {
                request.Headers.Add("Cookie", cookies.Cookies);
                request.RequestUserAgent = cookies.UserAgent;
            }
            //var codedetialpage = request.HttpRequest(codedetial);
            // request.Headers.Add("Cookie", "s=n4v12ge0yu; webp=0; bid=dfa57ca958de46f53568cc28e1762269_ipbu7hld; xq_a_token=ed3a6f41cd40749a7026f25f4f3e936379e415ed; xq_r_token=d54ca76f529ff87e19b08c546ed16a464caa90ef; __utmt=1; Hm_lvt_1db88642e346389874251b5a1eded6e3=1468887873,1469001779; Hm_lpvt_1db88642e346389874251b5a1eded6e3=1469001795; __utma=1.2122399928.1465259166.1468888024.1469001780.4; __utmb=1.4.9.1469001795053; __utmc=1; __utmz=1.1465259166.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)");
            //request.RequestUserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/5";
            //{"SH603288":{"symbol":"SH603288","exchange":"SH","code":"603288","name":"海天味业","current":"30.09","percentage 跌幅百分比":"-0.59","change （下跌或者上涨）价格":"-0.18","open":"30.39","high":"30.85","low":"29.83","close 收盘价":"30.09","last_close上次收盘价":"30.27","high52week >52周最高":"38.38","low52week52周最低":"24.4","volume成交（手 1/100股）":"3541015.0","volumeAverage":"3826996","marketCapital":"8.143094214E10","eps":"0.3","pe_ttm":"31.4916","pe_lyr":"32.4474","beta":"0.0","totalShares":"2706246000","time":"Mon Jun 06 14:59:47 +0800 2016","afterHours":"0.0","afterHoursPct":"0.0","afterHoursChg":"0.0","updateAt":"1465214405369","dividend":"0.6","yield":"1.99","turnover_rate":"1.31","instOwn":"0.0","rise_stop":"33.3","fall_stop":"27.24","currency_unit":"CNY","amount":"1.07284402E8","net_assets":"2.9381","hasexist":"","has_warrant":"0","type":"11","flag":"1","rest_day":"","amplitude":"3.37","lot_size":"100","min_order_quantity":"0","max_order_quantity":"0","tick_size":"0.01","kzz_stock_symbol":"","kzz_stock_name":"","kzz_stock_current":"0.0","kzz_convert_price":"0.0","kzz_covert_value":"0.0","kzz_cpr":"0.0","kzz_putback_price":"0.0","kzz_convert_time":"","kzz_redempt_price":"0.0","kzz_straight_price":"0.0","kzz_stock_percent":"","pb":"10.24","benefit_before_tax":"0.0","benefit_after_tax":"0.0","convert_bond_ratio":"","totalissuescale":"","outstandingamt":"","maturitydate":"","remain_year":"","convertrate":"0.0","interestrtmemo":"","release_date":"","circulation":"0.0","par_value":"0.0","due_time":"0.0","value_date":"","due_date":"","publisher":"","redeem_type":"F","issue_type":"1","bond_type":"","warrant":"","sale_rrg":"","rate":"","after_hour_vol":"0","float_shares":"269460000","float_market_capital":"8.1080514E9","disnext_pay_date":"","convert_rate":"0.0","psr":"7.0653"}}
            var Page = request.HttpRequest(tempurl);

      
        }

     

    }
}