using System.Text.RegularExpressions;
using System.Web;
using BLL;
using BLL.Sprider.classInfo;
using BLL.WeiBo;
using Commons;
using PhantomjsDrive;
using Servers;
using ServiceStack;
using System;
using System.Net;
using System.Text;
using BLL.Sprider.Stock;
using FastVerCode;
using Mode.account;

namespace WebAppClinet
{
    public partial class Web : System.Web.UI.Page
    {
        protected JsonServiceClient Client { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            //new SpriderSystem().SaveSitePromo();
            //new ImgBll().GetImg("http://img12.360buyimg.com/n7/jfs/t157/128/1892120941/109099/ffdb7ad6/53be5465N4d6453aa.jpg");
            //new SpriderSystem().SendWeibo();
            //WeiboFactory factory = new WeiboFactory { Domain = "sina" };
            //factory.Auth("3ad593b75a3f603cc68aa7e7edeed8cb");
            //new SiteClassBll().SaveTommbData();
            //Download();
            //new SpriderSystem().SaveSiteCate(13);
            //new SpriderSystem().UpdateSiteCat(13);
            //new SiteClassBll().UpdatemmbsiteClass(13);
            //UserAccountSystem();
            test();
            //new SpriderSystem().SaveAllSiteCate();
            //new SpriderSystem().LoadBand();
           //new SpriderSystem().Start();
            //new SinalweiboBll().write("aaa");
            //new SiteClassBll().UpdatemmbsiteClass(10);
            //test();
            //var list = new MmbProductItemsBll().GetItem(1, 1000);
            //   new SpriderSystem().UpdateSiteCat(8);
            // jdwxPrice();
            //test2();
            //test3();
            //pingan();
            //test1();
            //yikeman();
        }

        
        private void test3()
        {
            string url = "http://www.joyj.com/go/411040";
            url =
                "http://union.click.jd.com/jda?e=&p=AyIOZRlfEgMTBVcaXyUCEQZUG14cMlZYDUUEJVtXQhQQRQtaV1MJBABAHUBZCQVbFgMTB1ASRExHTlplThBCAhp%2BB2A7aVprUDNyDBV%2BYXcLXVcZMhUCUx5bFQIRN1UfUxMAEwdcK2t0cCJROxtaFAMTBlUcWRcyEzdVHl8VBBAAXBteEgIWN1I%3D&t=W1dCFBBFC1pXUwkEAEAdQFkJBVsWAxMHUBJETEdOWg%3D%3D&a=fCg9UgoiAwwHO1BcXkQYFFlhc3l8c1FdQ1wzVRBSUll%2bAQAPDSwjLw%3d%3d&refer=norefer";

            PhantomjsBase.PhantomjsPath = Request.PhysicalApplicationPath;
            string page = new DownLoadServer().DownLoadbuycmd("http://www.jd.com/");
           var aa = page;
        }



        private void test()
        {
            //string feiniuurl = "https://reg.feiniu.com/patcha/image";
            //WebClient myWebClient = new WebClient();
            //var imge= myWebClient.DownloadData(feiniuurl);
            //var filename= ImageServer.DoloadImg(imge, "ddd");
          
            //string wlfile="";

            //string key1=  VerCode.GetUserInfo("maiden","52zhuzhu");
            //string returnMess = VerCode.RecByte_A(imge, imge.Length, "maiden", "52zhuzhu", "");

            new BenlaiShenhuo().RegionUser();
            return;
            string key = "LzMuY";
            string url = "http://api.hellotrue.com/api/do.php?action=loginIn&name=api-rmm0nm29&password=62415109";
            HtmlAnalysis request1 = new HtmlAnalysis();
            request1.RequestContentType = "application/x-www-form-urlencoded";
            request1.Headers.Add("Cookie", "JSESSIONID=83DE329FB6AFC2FE71B6E455FC6F294D; ASP.NET_SessionId=wgdao2pj2kwoa3jlyn4drz3n; uuk=11002ae4-b7de-40ed-9b65-5cf27f758b97; userGuid=4ac9c1a9-bd89-4827-857b-6143cf2a322720160615101926; WebSiteSysNo=1; DeliverySysNo=2; source=2; o_l_state=b9f164854f211ebf80d6ce483975620a; _pk_id.7.2b60=b6589fc6ab0dc82c.1465957175.1.1465957175.1465957175.; _pk_ses.7.2b60=*; CSESSIONID=83DE329FB6AFC2FE71B6E455FC6F294D; Hm_lvt_9a7d729a11da2966935bcb2908a98794=1465949409,1465953691; Hm_lpvt_9a7d729a11da2966935bcb2908a98794=1465957175; Hm_lvt_7feabb06873cfd158820492f754cc70b=1465949409,1465953691; Hm_lpvt_7feabb06873cfd158820492f754cc70b=1465957175; _qzja=1.834142463.1465957174825.1465957174825.1465957174826.1465957174826.1465957174901.https%253A%252F%252Fm_benlai_com.1.0.2.1; _qzjb=1.1465957174825.2.0.0.0; _qzjc=1; _qzjto=2.1.0; _jzqckmp_v2=1/; _jzqckmp=1/");
            request1.RequestUserAgent =
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.87 Safari/537.36";
            request1.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request1.Headers.Add("Origin", "https://m.benlai.com");
            request1.RequestReferer = "https://m.benlai.com/showReg?comeFromApp=0";
            url = "https://m.benlai.com/regPhoneVry?phoneNumber=13586777526";
            request1.RequestMethod = "post";
            var page=request1.HttpRequest(url);
            string token = page.Replace("1|", "");
            string getphoneurl = "http://api.hellotrue.com/api/do.php?action=getPhone&sid=1773&token="+ token;
             page= request1.HttpRequest(getphoneurl);
            string phone = page.Replace("1|", "");
            string getmessage = "http://api.hellotrue.com/api/do.php?action=getMessage&sid=1773&phone=" + phone +
                                "&token=" + token;
            page = request1.HttpRequest(getmessage);



            LogServer.WriteLog("phone"+phone+"token"+token+"message="+getmessage);

            //new SiteFactory().StockInfoManager.GetALlStockInfo();
            //return;17074858678
            //string tempurl = "https://xueqiu.com/v4/stock/quote.json?code=SH603288&_=1465259721266";
            //PhantomjsBase.PhantomjsPath = Request.PhysicalApplicationPath;
            //var page = new DownLoadServer().DownLoadpage(tempurl);

            //var aa=   TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)).AddMilliseconds(1464012000000);
            //var bb = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)).AddMilliseconds(1464537599000);
            //string url = "http://chart.windin.com/hqserver/HQProxyHandler.ashx?windcode=600873.SH";
            //string url = "http://licaike.hexun.com/List.action?fundCode=&fundcode=&fundTypes=&isGuaranteedFund=&isQdiiFund=&isIndexFund=&riskLvlt=&nav=&accumulativeNav=&investManner=&fundSize=&fundEstablishDate=&thisYearRate=&weekRiseRate=&monthRiseRate=&month3RiseRate=&month6RiseRate=&yearRiseRate=&year2RiseRate=&year3RiseRate=&cxRank=&htRank=&expan=&companyExpan=&pager.activePage=1";
            //TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)).AddMilliseconds(1464012000000);
            HtmlAnalysis request = new HtmlAnalysis();
            //request.RequestMethod = "post";
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            //request.Headers.Add("Origin", "http://licaike.hexun.com");
            //request.RequestContentType = "application/x-www-form-urlencoded";
            //request.RequestReferer = "http://licaike.hexun.com";
            request.RequestAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Cookie", "s=n4v12ge0yu; xq_a_token=e091ebfe5ded576016d5799ac18315a69336a5bd; xq_r_token=9881106dea309c7184d4b85b6f56068f266e1c9c; Hm_lvt_1db88642e346389874251b5a1eded6e3=1464827943,1465257477; Hm_lpvt_1db88642e346389874251b5a1eded6e3=1465259166; __utmt=1; __utma=1.2122399928.1465259166.1465259166.1465259166.1; __utmb=1.1.10.1465259166; __utmc=1; __utmz=1.1465259166.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)");
            request.RequestUserAgent ="Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.87 Safari/537.36";
            //{"SH603288":{"symbol":"SH603288","exchange":"SH","code":"603288","name":"海天味业","current":"30.09","percentage 跌幅百分比":"-0.59","change （下跌或者上涨）价格":"-0.18","open":"30.39","high":"30.85","low":"29.83","close 收盘价":"30.09","last_close上次收盘价":"30.27","high52week >52周最高":"38.38","low52week52周最低":"24.4","volume成交（手 1/100股）":"3541015.0","volumeAverage":"3826996","marketCapital":"8.143094214E10","eps":"0.3","pe_ttm":"31.4916","pe_lyr":"32.4474","beta":"0.0","totalShares":"2706246000","time":"Mon Jun 06 14:59:47 +0800 2016","afterHours":"0.0","afterHoursPct":"0.0","afterHoursChg":"0.0","updateAt":"1465214405369","dividend":"0.6","yield":"1.99","turnover_rate":"1.31","instOwn":"0.0","rise_stop":"33.3","fall_stop":"27.24","currency_unit":"CNY","amount":"1.07284402E8","net_assets":"2.9381","hasexist":"","has_warrant":"0","type":"11","flag":"1","rest_day":"","amplitude":"3.37","lot_size":"100","min_order_quantity":"0","max_order_quantity":"0","tick_size":"0.01","kzz_stock_symbol":"","kzz_stock_name":"","kzz_stock_current":"0.0","kzz_convert_price":"0.0","kzz_covert_value":"0.0","kzz_cpr":"0.0","kzz_putback_price":"0.0","kzz_convert_time":"","kzz_redempt_price":"0.0","kzz_straight_price":"0.0","kzz_stock_percent":"","pb":"10.24","benefit_before_tax":"0.0","benefit_after_tax":"0.0","convert_bond_ratio":"","totalissuescale":"","outstandingamt":"","maturitydate":"","remain_year":"","convertrate":"0.0","interestrtmemo":"","release_date":"","circulation":"0.0","par_value":"0.0","due_time":"0.0","value_date":"","due_date":"","publisher":"","redeem_type":"F","issue_type":"1","bond_type":"","warrant":"","sale_rrg":"","rate":"","after_hour_vol":"0","float_shares":"269460000","float_market_capital":"8.1080514E9","disnext_pay_date":"","convert_rate":"0.0","psr":"7.0653"}}
            var Page = request.HttpRequest(url);
            

        }



    }
}