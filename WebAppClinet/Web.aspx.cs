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
using System.Collections.Generic;
using System.Net;
using System.Text;
using BLL.Sprider.Stock;
using FastVerCode;
using Mode;
using Mode.account;

namespace WebAppClinet
{
    public partial class Web : System.Web.UI.Page
    {
        protected JsonServiceClient Client { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            //new SpriderSystem().SaveSitePromo();

            //new SpriderSystem().SendWeibo();
            //WeiboFactory factory = new WeiboFactory { Domain = "sina" };
            //factory.Auth("3ad593b75a3f603cc68aa7e7edeed8cb");
            //new SiteClassBll().SaveTommbData();

            //new SpriderSystem().SaveSiteCate(293);
            //new SpriderSystem().UpdateSiteCat(161);
            //new SiteClassBll().UpdatemmbsiteClass(293);
            //UserAccountSystem();
            //test();
            //jdlogion();
            //new SiteClassBll().UpdatemmbsiteClass(10);
            //loadjdUser();
            test();
            //var list = new MmbProductItemsBll().GetItem(1, 1000);
            //   new SpriderSystem().UpdateSiteCat(8);
            // jdwxPrice();



        }


        private void jdlogion()
        {

           

            var param = "";
            var checkcodeTxt = "";
            var checkCodeRid = "";
            var payPassword = "";
            var mobileForPresale = "";
            var presalePayType = "";// 预售验证手机号


            if (!string.IsNullOrEmpty(checkcodeTxt))
            {
                param = param + "submitOrderParam.checkcodeTxt=" + checkcodeTxt;
            }
            param = param + "&overseaPurchaseCookies="; //+$("#overseaPurchaseCookies").val();

            //if (!isEmpty($("#checkCodeDiv").html()))
            //{
            //    checkCodeRid = $("#checkcodeRid").val();
            //}
            if (!string.IsNullOrEmpty(checkCodeRid)) //验证码
            {
                param = param + "&submitOrderParam.checkCodeRid=" + checkCodeRid;
            }
            if (!string.IsNullOrEmpty(payPassword)) //支付密码
            {
                param = param + "&submitOrderParam.payPassword=" + payPassword;
                    //encodeURIComponent(stringToHex(payPassword));
            }
            var sopNotPutInvoice = ""; //必须
            if (!string.IsNullOrEmpty(sopNotPutInvoice))
            {
                param = param + "&submitOrderParam.sopNotPutInvoice=" + sopNotPutInvoice;//$("#sopNotPutInvoice").val();
            }
            else {
                param = param + "&submitOrderParam.sopNotPutInvoice=false";
            }

            if (!string.IsNullOrEmpty(mobileForPresale))
            {
                param = param + "&submitOrderParam.presaleMobile=" + mobileForPresale;
            }
            var trackID ="";
            if (!string.IsNullOrEmpty(trackID))
            {
                param = param + "&submitOrderParam.trackID=" + trackID;
            }
            var regionId = "";
            if (!string.IsNullOrEmpty(regionId))
            {
                param += "&regionId=" + regionId;
            }

            var easyBuyFlag = ""; //$("#easyBuyFlag").val(); 一键购
            if (easyBuyFlag == "1" || easyBuyFlag == "2")
            {
                param += "&ebf=" + easyBuyFlag;
            }

            var ignorePriceChange = "0"; //$('#ignorePriceChange').val();
            if (!string.IsNullOrEmpty(ignorePriceChange))
            {
                param += "&submitOrderParam.ignorePriceChange=" + ignorePriceChange;
            }

            var onlinepaytype = "0"; //$(".payment-item.item-selected").attr('onlinepaytype');
            var paymentId = "4"; //$(".payment-item.item-selected").attr('payid');


            //if ($(".payment-item[onlinepaytype=1]").length == 0 ||$(".payment-item[onlinepaytype=1]").hasClass("payment-item-disabled")){
            //    param += "&submitOrderParam.btSupport=0";
            //}else{
            //    param += "&submitOrderParam.btSupport=1";
            //}
            param += "&submitOrderParam.btSupport=0";

            var eid = ""; //$('#eid').val();
            if (!string.IsNullOrEmpty(eid))
            {
                param += "&submitOrderParam.eid=" + eid;
            }
            var fp = ""; //$('#fp').val();
            if (!string.IsNullOrEmpty(fp))
            {
                param += "&submitOrderParam.fp=" + fp;
            }
            var isBestCoupon = "";//$('#isBestCoupon').val();
            if (!string.IsNullOrEmpty(isBestCoupon))
            {
                param += "&submitOrderParam.isBestCoupon=" + isBestCoupon;
            }
            string action = "http://trade.jd.com/shopping/order/submitOrder.action";


            HtmlAnalysis req = new HtmlAnalysis();
            //req.RequestMethod = "POST";

            var cookies = new SiteCookiesBll().GetOneByDomain("www.jd.com");
            if (cookies != null)
            {
                req.Headers.Add("Cookie", cookies.Cookies);
                req.RequestUserAgent = cookies.UserAgent;
            }
            //string orderpage = req.HttpRequest("http://trade.jd.com/shopping/order/getOrderInfo.action");
            //req.RequestMethod = "POST";
            //var page = req.HttpRequest(action + param);

            //string js1 = "https://payrisk.jd.com/fp.html";
            //var page1= HtmlAnalysis.Gethtmlcode(js1);
            //var jsres = JsContext.Responsejs(page1,"");
            //string logionurl = "https://passport.jd.com/new/login.aspx?ReturnUrl=https%3A%2F%2Fwww.jd.com%2F";
            //var page = new DownLoadServer().DownLoadpage(logionurl);
            //string uuid = Regex.Match(page, "name=\"uuid\" value=\"(?<x>.*?)\"/>", RegexOptions.RightToLeft).Groups["x"].Value;
            //string token = Regex.Match(page, "name=\"_t\" id=\"token\" value=\"(?<x>.*?)\"").Groups["x"].Value;
            //string lgkey = Regex.Match(page, "<input type=name=\"hidden\" name=\"(?<x>.*?)\" value=\"(?<y>.*?)\" />", RegexOptions.RightToLeft).Groups["x"].Value;
            //string lgvalue = Regex.Match(page, "<input type=name=\"hidden\" name=\"(?<x>.*?)\" value=\"(?<y>.*?)\" />", RegexOptions.RightToLeft).Groups["y"].Value;
            //string logionpage = HtmlAnalysis.Gethtmlcode(logionurl);<input type="hidden" name="ZjKEwOTQFR" value="nDhTc" />


            //string url = "https://passport.jd.com/uc/loginService";
            //Dictionary<string, string> paramlist = new Dictionary<string, string>();
            //paramlist.Add("uuid", uuid);
            //paramlist.Add("machineNet", "");
            //paramlist.Add("machineCpu", "");
            //paramlist.Add("machineDisk", "");
            //paramlist.Add("eid", "053618F3B3A99AA10BBC005C578D0400A15A7ACB6D570B43ACEBBC908FA3581EE25724D49A76544C8A01D658B9DE278B");
            //paramlist.Add("fp", "39c8c858e06c01164b9c5aa46580c703");
            //paramlist.Add("_t", token);
            //paramlist.Add(lgkey, lgvalue);
            //paramlist.Add("loginname", "地狱狂壟");
            //paramlist.Add("nloginpwd", "chengzho347");
            //paramlist.Add("loginpwd", "chengzho347");
            //paramlist.Add("chkRememberMe", "on");

            //string param = "";

            //foreach (var item in paramlist)
            //{
            //    param += item.Key + "=" + item.Value + "&";
            //}

            //var Page =  req.HttpRequest(url+"?"+param);
            var page2 = req.HttpRequest("http://order.jd.com/center/list.action");

            var url = "";
            //  url=  "http://cart.jd.com/cart/dynamic/reBuyForOrderCenter.action?action=AddSkus&wids=10108554443&nums=1";
            //var page3 = req.HttpRequest(url);  //addcat


            //url = "http://trade.jd.com/shopping/dynamic/payAndShip/getAdditShipment.action?paymentId=4&shipParam.reset311=0&resetFlag=0000000000&shipParam.onlinePayType=0";

            url = "http://trade.jd.com/shopping/order/getOrderInfo.action?rid=0.49538216742194985";
            var page = req.HttpRequest(url);
            //url = "http://trade.jd.com/shopping/order/getOrderInfo.action";


            url = "http://trade.jd.com/shopping/dynamic/payAndShip/getVenderInfo.action?shipParam.payId=1&shipParam.pickShipmentItemCurr=false&shipParam.onlinePayType=0";
            req.RequestMethod = "POST";
            req.RequestAccept = "text/plain, */*; q=0.01";
            req.RequestContentType = "application/x-www-form-urlencoded";
            req.RequestReferer = "http://trade.jd.com/shopping/order/getOrderInfo.action?rid=1469494481688";
            req.Headers.Add("Origin", "http://trade.jd.com");
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            var page5 = req.HttpRequest(url); //选择支付方式

            url =
                "http://trade.jd.com/shopping/dynamic/payAndShip/getAdditShipment.action?paymentId=1&shipParam.reset311=0&resetFlag=0000000000&shipParam.onlinePayType=0";
            var page6 = req.HttpRequest(url);


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
                    CreateDate=DateTime.Now

                };
                newlist.Add(userinfo);
            }
            new SiteUserInfoBll().addUser(newlist);
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
            var pageinfo1 = HtmlAnalysis.Gethtmlcode("http://d.10jqka.com.cn/v2/time/hs_600662/last.js");


            var head =
                HtmlAnalysis.Gethtmlcode("http://stockpage.10jqka.com.cn/spService/002801/Header/realHeader");
            //                                                                                                                            现价         涨跌百分比     涨跌金额            成交量                         成交额         开盘价      昨收           最高        最低        换手            市盈率(动) 内盘         外盘        均价           振幅       涨停         跌停                          委比      委差
            //{"stockcode":"600662","stockname":"\u5f3a\u751f\u63a7\u80a1","fieldcode":"1150","fieldname":"\u516c\u4ea4","fieldjp":"gj","xj":"11.21","zdf":"-3.69%","zde":"-0.43","cjl":"34.81 \u4e07\u624b","cje":"3.92 \u4ebf\u5143","kp":"11.46","zs":"11.64","zg":"11.49","zd":"11.00","hs":"3.31%","syl":"81.26","np":189715,"wp":150836,"jj":"11.27","zf":"4.21%","zt":"12.80","dt":"10.48","field":"0.00","wb":"7.86","wc":132,"buy1":"11.20","buy1data":20,"buy2":"11.19","buy2data":185,"buy3":"11.18","buy3data":143,"buy4":"11.17","buy4data":230,"buy5":"11.16","buy5data":328,"sell1":"11.21","sell1data":111,"sell2":"11.22","sell2data":185,"sell3":"11.23","sell3data":196,"sell4":"11.24","sell4data":81,"sell5":"11.25","sell5data":201}
            //CookieContainer list = new CookieContainer();
            //SimulationCookie.GetCookie("https://passport.jd.com/new/login.aspx?ReturnUrl=http://www.jd.com/",
            //    cookie =>
            //    {
            //        list.Add(cookie);
            //    });

            //new SiteFactory().StockInfoManager.GetALlStockInfo();
            //string loginurl =
            //    "https://passport.jd.com/uc/loginService?uuid=bc069405-c238-4af6-90b4-98162ea88ae9&&r=0.376717114952144&version=2015";
            ////http://a.jd.com/coupons.html
            //var loginpage =HtmlAnalysis.Gethtmlcode(loginurl);


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
            string tempurl = "https://xueqiu.com/v4/stock/quote.json?code=SZ000002&_=1465259721266";

            var cookies = new SiteCookiesBll().GetOneByDomain("xueqiu.com");
            //PhantomjsBase.PhantomjsPath = Request.PhysicalApplicationPath;
            //var page = new DownLoadServer().DownLoadpage(tempurl);

            //var aa=   TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)).AddMilliseconds(1464012000000);
            //var bb = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)).AddMilliseconds(1464537599000);
            //string url = "http://chart.windin.com/hqserver/HQProxyHandler.ashx?windcode=600873.SH";
            //string url = "http://licaike.hexun.com/List.action?fundCode=&fundcode=&fundTypes=&isGuaranteedFund=&isQdiiFund=&isIndexFund=&riskLvlt=&nav=&accumulativeNav=&investManner=&fundSize=&fundEstablishDate=&thisYearRate=&weekRiseRate=&monthRiseRate=&month3RiseRate=&month6RiseRate=&yearRiseRate=&year2RiseRate=&year3RiseRate=&cxRank=&htRank=&expan=&companyExpan=&pager.activePage=1";
            //TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)).AddMilliseconds(1464012000000);
            HtmlAnalysis request = new HtmlAnalysis();
            //request.RequestMethod = "post";
            //request.Headers.Add("Upgrade-Insecure-Requests", "1");
            //request.Headers.Add("Origin", "http://licaike.hexun.com");
            //request.RequestContentType = "application/x-www-form-urlencoded";
            //request.RequestReferer = "http://licaike.hexun.com";
            request.RequestAccept = "application/json, text/javascript, */*; q=0.01";
            request.Headers.Add("X-Requested-With","XMLHttpRequest");
            request.RequestReferer = "https://xueqiu.com/s/SZ000002";
            if (cookies != null)
                request.Headers.Add("Cookie", cookies.Cookies);
           // request.Headers.Add("Cookie", "s=n4v12ge0yu; webp=0; bid=dfa57ca958de46f53568cc28e1762269_ipbu7hld; xq_a_token=ed3a6f41cd40749a7026f25f4f3e936379e415ed; xq_r_token=d54ca76f529ff87e19b08c546ed16a464caa90ef; __utmt=1; Hm_lvt_1db88642e346389874251b5a1eded6e3=1468887873,1469001779; Hm_lpvt_1db88642e346389874251b5a1eded6e3=1469001795; __utma=1.2122399928.1465259166.1468888024.1469001780.4; __utmb=1.4.9.1469001795053; __utmc=1; __utmz=1.1465259166.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)");
            request.RequestUserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
            //{"SH603288":{"symbol":"SH603288","exchange":"SH","code":"603288","name":"海天味业","current":"30.09","percentage 跌幅百分比":"-0.59","change （下跌或者上涨）价格":"-0.18","open":"30.39","high":"30.85","low":"29.83","close 收盘价":"30.09","last_close上次收盘价":"30.27","high52week >52周最高":"38.38","low52week52周最低":"24.4","volume成交（手 1/100股）":"3541015.0","volumeAverage":"3826996","marketCapital":"8.143094214E10","eps":"0.3","pe_ttm":"31.4916","pe_lyr":"32.4474","beta":"0.0","totalShares":"2706246000","time":"Mon Jun 06 14:59:47 +0800 2016","afterHours":"0.0","afterHoursPct":"0.0","afterHoursChg":"0.0","updateAt":"1465214405369","dividend":"0.6","yield":"1.99","turnover_rate":"1.31","instOwn":"0.0","rise_stop":"33.3","fall_stop":"27.24","currency_unit":"CNY","amount":"1.07284402E8","net_assets":"2.9381","hasexist":"","has_warrant":"0","type":"11","flag":"1","rest_day":"","amplitude":"3.37","lot_size":"100","min_order_quantity":"0","max_order_quantity":"0","tick_size":"0.01","kzz_stock_symbol":"","kzz_stock_name":"","kzz_stock_current":"0.0","kzz_convert_price":"0.0","kzz_covert_value":"0.0","kzz_cpr":"0.0","kzz_putback_price":"0.0","kzz_convert_time":"","kzz_redempt_price":"0.0","kzz_straight_price":"0.0","kzz_stock_percent":"","pb":"10.24","benefit_before_tax":"0.0","benefit_after_tax":"0.0","convert_bond_ratio":"","totalissuescale":"","outstandingamt":"","maturitydate":"","remain_year":"","convertrate":"0.0","interestrtmemo":"","release_date":"","circulation":"0.0","par_value":"0.0","due_time":"0.0","value_date":"","due_date":"","publisher":"","redeem_type":"F","issue_type":"1","bond_type":"","warrant":"","sale_rrg":"","rate":"","after_hour_vol":"0","float_shares":"269460000","float_market_capital":"8.1080514E9","disnext_pay_date":"","convert_rate":"0.0","psr":"7.0653"}}
            var Page = request.HttpRequest(tempurl);
            

        }



    }
}