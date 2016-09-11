using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using BLL;
using Commons;
using Mode;
using FastVerCode;
using Newtonsoft.Json.Linq;
namespace Servers
{
    public class JdOrderServer
    {
        public string Eid { get; set; }
        public string Fp { get; set; }
        protected static RegexOptions ro = RegexOptions.Singleline | RegexOptions.IgnoreCase;
 
        public string UserName { get; set; }

        public SiteUserInfo User { get; set; }
        public static string jdUserAgent { get; set; }

        public const string MdCaturl = "http://cart.jd.com/cart/dynamic/reBuyForOrderCenter.action?action=AddSkus&nums={1}&wids={0}";
        public JdOrderServer()
        {
            //Eid = "8E54E4CB9E9D66F99E2423476A2D0BA7B177D9FDE30A39033C6C7191346B6BCE9B084078401AEE3569559D0D83DBB8C4";
            //Fp = "f3595909a05d0c87bd723f1cbfe6a82e";
            if (string.IsNullOrEmpty(Fp))
                Fp = "4231a9e01b4f44e79d1deb02b427b7c8";
            if (string.IsNullOrEmpty(Eid))
                Eid = "ED743BE582053C240F57BED2A721971997408F3D8C96D43F75CF85E78B35F755D881606EFF3F83340C8CC2105F73B493";
            if (string.IsNullOrEmpty(jdUserAgent))
            {
                //jdUserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:47.0) Gecko/20100101 Firefox/47.0";
                jdUserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A403 Safari/8536.25";
            }
        }

        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="name"></param>
        public void SetUserInfo(string name)
        {
            UserName = name;
            User = new SiteUserInfoBll().GetOngeUser(UserName);

            if (!string.IsNullOrEmpty(User.Fp))
                Fp = User.Fp;
            if (!string.IsNullOrEmpty(User.Eid))
                Eid = User.Eid;
            if (!string.IsNullOrEmpty(User.UserAgent))
                jdUserAgent = User.UserAgent;


        }
        private string AjaxPostRequest(string url, string param)
        {
            HtmlAnalysis req = new HtmlAnalysis();
            req.RequestAccept = "text/plain, */*; q=0.01";
            req.RequestContentType = "application/x-www-form-urlencoded";
            req.RequestAccept = "text/javascript, application/javascript, application/ecmascript, application/x-ecmascript, */*; q=0.01";

            if (!string.IsNullOrEmpty(User.LoginCookies))
            {
                req.Headers.Add("Cookie", User.LoginCookies);
            }
            if (!string.IsNullOrEmpty(jdUserAgent))
            {
                req.RequestUserAgent = jdUserAgent;
            }
            return req.HttpRequest(url, param);
        }


        #region 公共方法
        /// <summary>
        /// 初始化用户信息
        /// </summary>
        /// <param name="userName"></param>
        public void userInit(string userName)
        {
            User = new SiteUserInfoBll().GetOngeUser(userName);
            if (User == null)
                return;
            Login(User.UserName, User.UserPwd);
            SaveAddress();
            SavePay();
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public bool Login(string name, string pwd)
        {
            string url = "https://passport.jd.com/new/login.aspx?ReturnUrl=http%3A%2F%2Fwww.jd.com%2F";
            //string cookies1 = HtmlAnalysis.GetResponseCookies("http://www.jd.com");
            string cookies = "";
            HtmlAnalysis rq = new HtmlAnalysis();
            rq.Headers.Add("X-Requested-With", "XMLHttpRequest");
            rq.RequestContentType = "application/x-www-form-urlencoded; charset=utf-8";
            rq.RequestReferer = "https://passport.jd.com/new/login.aspx?ReturnUrl=https%3A%2F%2Fwww.jd.com%2F";
            rq.Headers.Add("Cookie", Fp + "=" + Eid);
            rq.RequestUserAgent = jdUserAgent;
            var logpage = rq.HttpRequest(url);
            if (rq.ResultResponseHeader != null && rq.ResultResponseHeader.ContainsKey("Set-Cookie"))
            {
                cookies = rq.ResultResponseHeader["Set-Cookie"].Replace("Path=/;", "").Replace("HttpOnly;", "").Replace("  ,", "");
            }
            var uuid = Regex.Match(logpage, "name=\"uuid\" value=\"(?<x>.*?)\"").Groups["x"].Value;

            string loginurl = "https://passport.jd.com/uc/loginService?r=0.36799998356697805&version=2015&ReturnUrl=http%3A%2F%2Fwww.jd.com%2F&uuid=" + uuid;

            string token = Regex.Match(logpage, "name=\"_t\" id=\"token\" value=\"(?<x>.*?)\"").Groups["x"].Value;
            string lgkey = Regex.Match(logpage, "<input type=\"hidden\" name=\"(?<x>\\w+)\" value=\"(?<y>\\w+)\"/>", ro).Groups["x"].Value;
            string lgvalue = Regex.Match(logpage, "<input type=\"hidden\" name=\"(?<x>\\w+)\" value=\"(?<y>\\w+)\"/>", ro).Groups["y"].Value;

            string authcode = "";
            //if (logpage.Contains("<input id=\"authcode\" type=\"text\" class=\"itxt itxt02\" name=\"authcode\" tabindex=\"5\">"))
            //{
            //    string imgurl = "https:" + Regex.Match(logpage, "src2=\"(?<x>.*?)\"").Groups["x"].Value.Replace("amp;", "");
            //    if (!string.IsNullOrEmpty(imgurl))
            //    {
            //        //string feiniuurl = "https://reg.feiniu.com/patcha/image";
            //        WebClient myWebClient = new WebClient();
            //        var imge = myWebClient.DownloadData(imgurl);
            //        //var filename = ImageServer.DoloadImg(imge, "ddd");

            //        //string wlfile = "";

            //        //string key1 = VerCode.GetUserInfo("maiden", "52zhuzhu");
            //        string returnMess = VerCode.RecByte_A(imge, imge.Length, "maiden", "52zhuzhu", "");
            //        if (!string.IsNullOrEmpty(returnMess) && returnMess.Contains("|"))
            //        {
            //            authcode = returnMess.Substring(0, returnMess.IndexOf('|'));
            //        }
            //    }

            //}
       
            Dictionary<string, string> paramlist = new Dictionary<string, string>();
            paramlist.Add("uuid", uuid);
            paramlist.Add("machineNet", "");
            paramlist.Add("machineCpu", "");
            paramlist.Add("machineDisk", "");
            paramlist.Add("eid", Eid);
            paramlist.Add("fp", Fp);
            paramlist.Add("_t", token);
            paramlist.Add("loginType", "f");
            paramlist.Add(lgkey, lgvalue);
            paramlist.Add("loginname", HttpUtility.UrlEncode(name));
            paramlist.Add("nloginpwd", pwd);
            paramlist.Add("loginpwd", pwd);
            paramlist.Add("chkRememberMe", "on");
            var jdcookies = new SiteCookiesBll().GetOneByDomain("www.jd.com");
            string reqCookie = jdcookies + ";" + cookies + Fp + "=" + Eid;
            if (rq.Headers.ContainsKey("Cookie"))
            {
                rq.Headers["Cookie"] = reqCookie;
            }
            else
            {
                rq.Headers.Add("Cookie", reqCookie);
            }

            paramlist.Add("authcode", authcode);
            string param = "";
            foreach (var item in paramlist)
            {
                param += item.Key + "=" + item.Value + "&";
            }

            rq.RequestAccept = "text/plain, */*; q=0.01";
            rq.RequestMethod = "POST";

            var page = rq.HttpRequest(loginurl, param.TrimEnd('&'));
            //string newcookie = "";
            //\u8bf7\u5237\u65b0\u9875\u9762\u540e\u91cd\u65b0\u63d0\u4ea4 刷新页面 重新提交
            if (page.Contains("success"))
            {
                if (rq.ResultResponseHeader != null && rq.ResultResponseHeader.ContainsKey("Set-Cookie"))
                {
                    var logincookies = rq.ResultResponseHeader["Set-Cookie"].Replace("Path=/;", "").Replace("HttpOnly;", "").Replace("  ,", "");
                    //newcookie = logincookies + reqCookie;
                    SiteUserInfoBll bll = new SiteUserInfoBll();
                    var item = bll.GetOngeUser(name);
                    if (item != null)
                    {
                        item.LoginCookies = logincookies;
                        item.Fp = Fp;
                        item.Eid = Eid;
                        item.UserAgent = jdUserAgent;
                        item.LoginUpdatetime = DateTime.Now;
                        bll.UpdateUser(item);
                        User = item;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <returns></returns>
        public bool SubmitOrder()
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
            param = param + "?overseaPurchaseCookies="; //+$("#overseaPurchaseCookies").val();

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
            var trackID = "";
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

            //var onlinepaytype = "0"; //$(".payment-item.item-selected").attr('onlinepaytype');
            //var paymentId = "4"; //$(".payment-item.item-selected").attr('payid');


            //if ($(".payment-item[onlinepaytype=1]").length == 0 ||$(".payment-item[onlinepaytype=1]").hasClass("payment-item-disabled")){
            //    param += "&submitOrderParam.btSupport=0";
            //}else{
            //    param += "&submitOrderParam.btSupport=1";
            //}
            param += "&submitOrderParam.btSupport=0";

            if (!string.IsNullOrEmpty(Eid))
            {
                param += "&submitOrderParam.eid=" + Eid;
            }

            if (!string.IsNullOrEmpty(Fp))
            {
                param += "&submitOrderParam.fp=" + Fp;
            }
            var isBestCoupon = "";//$('#isBestCoupon').val();
            if (!string.IsNullOrEmpty(isBestCoupon))
            {
                param += "&submitOrderParam.isBestCoupon=" + isBestCoupon;
            }
            string action = "http://trade.jd.com/shopping/order/submitOrder.action";


            HtmlAnalysis req = new HtmlAnalysis();
            //req.RequestMethod = "POST";

            if (User != null && !string.IsNullOrEmpty(User.LoginCookies))
            {
                req.Headers.Add("Cookie", User.LoginCookies);
            }
            if (!string.IsNullOrEmpty(jdUserAgent))
            {
                req.RequestUserAgent = jdUserAgent;
            }
         
            req.RequestMethod = "POST";
            req.RequestReferer = "http://trade.jd.com/shopping/order/getOrderInfo.action?rid=1470279644415";
            req.RequestAccept = "application/json, text/javascript, */*; q=0.01";

            var orderpage = req.HttpRequest(action + param);
            if (orderpage.Contains("\"success\":true"))
            {
                return true;
            }
            LogServer.WriteLog(orderpage, "orderinfo");
            return false;
        }

        public bool Qianggou(string skuid,string num)
        {
            HtmlAnalysis req = new HtmlAnalysis();
            req.RequestAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            req.RequestContentType = "application/x-www-form-urlencoded";
            req.Headers.Add("Cookie",User.LoginCookies);
            req.RequestAutoRedirect = false;
            string seckillurl = $"http://marathon.jd.com/seckill/seckill.action?skuId={skuid}&num={num}&rid=1470626643";
            req.HttpRequest(seckillurl);
            var reqcookie =req.ResultResponseHeader["Set-Cookie"];
            if (req.Headers.ContainsKey("Cookie"))
            {
                req.Headers["Cookie"] = User.LoginCookies + reqcookie;
            }
            else
            {
                req.Headers.Add("Cookie", User.LoginCookies + reqcookie);
            }

            string isSupportCodPaymenturl = "http://marathon.jd.com/async/isSupportCodPayment.action?skuId=1131251";

            string getUsualAddressList = "http://marathon.jd.com/async/getUsualAddressList.action?skuId=808171";

            var addinfo = req.HttpRequest(getUsualAddressList);

            //dizhi            //marathon.jd.com/async/isSupportCodPayment.action?skuId=808171
            string url = "http://marathon.jd.com/async/isSupportCodPayment.action?skuId=" + skuid;
            string param1 ="orderParam.provinceId=15&orderParam.cityId=1158&orderParam.countyId=46345&orderParam.townId=52176";
            req.RequestMethod = "POST";
            req.RequestAccept = "application/json, text/javascript, */*; q=0.01";
            req.RequestReferer = seckillurl;
            var testck =
                "__jda=122270672.1426248074.1470460530.1470633823.1470640776.6; __jdv=122270672|direct|-|none|-; __jdu=1426248074; TrackID=1cqketbDpTiT0D5I8AGkbQgGs6houAJYwS-XT0Y8pB8r-2guVQCIJEtiLHgl3LOg7CxzxhRJqxEllpWwIM-UyuNfxsFh8BRr6AN5GaOLRwgU; pinId=4i7VHIeyj0sHLR6tPsamvg; unick=%E5%9C%B0%E7%8B%B1%E7%8B%82%E5%A3%9F; _tp=rV1Yooh83DhUGcgoxdY2yT%2B4Na6lTZzPRI8O3woIJEkcZ3Gz3tVCwVXoHvNxEGa4; _pst=%E5%9C%B0%E7%8B%B1%E7%8B%82%E5%A3%9F; user-key=111c5702-98e3-4d40-821e-1bd89c5ad472; cn=0; ipLoc-djd=1-72-2819-0; ipLocation=%u5317%u4EAC; __jdc=122270672; areaId=1; pin=%E5%9C%B0%E7%8B%B1%E7%8B%82%E5%A3%9F; thor=5DF2900E9CE6D60AE5589E1056990A622D5A1EDA0D265ADFE6CE073CF45AA6DCB0CD700C5175C375928039EA4B88BEE498EFF08D7B341555D362A777E8362BA5704559F694DE9EC324C5129BA7EB334FB9F93765D7644F79C8F8FFF4E50D9D6E47378DAA0EFDD3DDE1E99E0D1C232B2E2179729E6F97384E5CA0EB7031D3265B; ceshi3.com=Xzg3E6MDex-KBRgG5vFpgV4f-VfObBbcqJfhT8wgE1w; __jdb=122270672.29.1426248074|6.1470640776; seckillSku=808171; seckillSid=; seckillSku=808171; seckillSid=; seckill808171=7hcd7wfuRgLSZYIp15t0BtpAQoXqa0KtNYY";
            req.Headers["Cookie"] = testck;
                var saveaddress = req.HttpRequest(url, param1);

            //pay
            string payurl = $"http://marathon.jd.com/async/calcuOrderPrice.action?skuId={skuid}&num=1";
            string payparam = "provinceId=15&cityId=1158&countyId=46345&townId=52176&paymentType=4&codTimeType=3";
            //var Paymenturl=req.HttpRequest(getUsualAddressList);
            var payjson = req.HttpRequest(payurl, payparam);

            string param = "orderParam.name=陈杰&orderParam.addressDetail=余姚市阳明街道长新路49号&orderParam.mobile=135****7526&orderParam.email=&orderParam.provinceId=15&orderParam.cityId=1158&orderParam.countyId=46345&orderParam.townId=52176&orderParam.paymentType=4&orderParam.password=&orderParam.invoiceTitle=4&orderParam.invoiceContent=1&orderParam.invoiceCompanyName=&orderParam.usualAddressId=138275686&skuId=1131251&num=1&orderParam.provinceName=浙江&orderParam.cityName=宁波市&orderParam.countyName=余姚市&orderParam.townName=城区&orderParam.codTimeType=3&orderParam.mobileKey=005daf2bc12f6d6aa5aa7c337ef721c3";
            string action = "http://marathon.jd.com/seckill/submitOrder.action?skuId="+skuid;
            req.RequestAccept = "text/plain, */*; q=0.01";


     
       

            var orderpage = req.HttpRequest(action ,param);
            return true;
        }

        /// <summary>
        /// 加入购物车
        /// </summary>
        public void AddCat(string skuid, string num)
        {
            var url = string.Format(MdCaturl, skuid, num);
            HtmlAnalysis req = new HtmlAnalysis();
            req.RequestUserAgent = jdUserAgent;
            if(User!=null&&!string.IsNullOrEmpty(User.LoginCookies))
            req.Headers.Add("Cookie", User.LoginCookies);
            var catpage = req.HttpRequest(url);
        }

        public void CatDetial()
        {
            string url = "http://trade.jd.com/shopping/dynamic/payAndShip/getAdditShipment.action";
            string param = "paymentId=1&shipParam.reset311=0&resetFlag=1000000000&shipParam.onlinePayType=0";
            string page = AjaxPostRequest(url, param);

            var catinfo = JObject.Parse(page);
            if(catinfo?["orderPrice"] == null)
                return;
            decimal freight = 0;
            if (catinfo["orderPrice"]["freight"] != null)
            {
                freight = catinfo["orderPrice"]["freight"].Value<decimal>();
            }

            decimal promotionPrice = 0;
            if (catinfo["orderPrice"]["promotionPrice"] == null)
            {
                promotionPrice = catinfo["orderPrice"]["promotionPrice"].Value<decimal>();
            }

            decimal payPrice = 0;
            if (catinfo["orderPrice"]["payPrice"] != null)
            {
                payPrice = catinfo["orderPrice"]["payPrice"].Value<decimal>();
            }
            if (freight + promotionPrice != payPrice)
            {
                LogServer.WriteLog(page, "orderinfo");
            }
        }

        public void ClearCat()
        {
            string url = "http://cart.jd.com/batchRemoveSkusFromCart.action";
            // http://cart.jd.com/removeSkuFromCart.action?rd=0.5644592723419413
            //parm="venderId=8888&pid=847963&ptype=1&packId=0&targetId=0";
            string param = "";//locationId=15-1158-46345;
            var page = AjaxPostRequest(url, param);
        }
        /// <summary>
        /// 保存支付方式（需购物车里有产品）
        /// </summary>
        public void SavePay()
        {
            //string addressinfoUrl = "http://easybuy.jd.com/address/getEasyBuyList.action";

            //var adds = req.HttpRequest(addressinfoUrl);

            //var addressId = Regex.Match(adds, "id=\"addresssDiv-(?<x>\\d+)\">", ro).Groups["x"].Value;
            //if (string.IsNullOrEmpty(addressId))
            //{
            //   return;
            //}
            //var savePaymenturl = "http://easybuy.jd.com/address/savePayment.action";
            //var savePayparam = $"addressId={addressId}&paymentId=1&pickId=0&pickName=";
            //var avePaymentpage = req.HttpRequest(savePaymenturl, savePayparam);
            //string payAndShip = "http://trade.jd.com/shopping/async/payAndShip/verifySelfPickCanUse.action";
            //var payAndShippage = req.HttpRequest(payAndShip, "shipParam.payId=1&shipParam.regionId=-1");//货到付款

            //string RecommendSelfPickurl = "http://trade.jd.com/shopping/async/payAndShip/getRecommendSelfPick.action";
            //var avePaymentpage = req.HttpRequest(RecommendSelfPickurl, "paymentId=1");//货到付款


            //string url = "http://trade.jd.com/shopping/dynamic/payAndShip/getVenderInfo.action";
            //string param = "shipParam.payId=1&shipParam.pickShipmentItemCurr=false&shipParam.onlinePayType=0";
            ////string paramOnline = "shipParam.payId=4&shipParam.pickShipmentItemCurr=false&shipParam.onlinePayType=3";
            //var payAndShippg = req.HttpRequest(url, param);//货到付款


            HtmlAnalysis req = new HtmlAnalysis();
            req.RequestMethod = "POST";
            req.RequestAccept = "text/plain, */*; q=0.01";
            req.RequestContentType = "application/x-www-form-urlencoded";
            if (!string.IsNullOrEmpty(User.LoginCookies))
            {
                req.Headers.Add("Cookie", User.LoginCookies);
            }
            if (!string.IsNullOrEmpty(jdUserAgent))
            {
                req.RequestUserAgent = jdUserAgent;
            }
   
            var payAndShipUrl = "http://trade.jd.com/shopping/dynamic/payAndShip/getAdditShipment.action";

            var resultpage = req.HttpRequest(payAndShipUrl, "paymentId=1&shipParam.reset311=0&resetFlag=0000000000&shipParam.onlinePayType=0");//货到付款
            if(string.IsNullOrEmpty(resultpage))
                return;
            resultpage = resultpage.Replace("\\t", "").Replace("\\r", "").Replace("\\n", "");
            LogServer.WriteLog(resultpage, "JdpayAndShip");

        }

        public void SaveAddress()
        {
            var url = "http://easybuy.jd.com/address/addAddress.action";
            var name = HttpUtility.UrlEncode("陈杰");
            var consigneeAddress = HttpUtility.UrlEncode("阳明街道长新路49号 阳明电商产业园4号楼 2F 上易网络");
            var fullAddress= HttpUtility.UrlEncode("浙江宁波市余姚市城区阳明街道长新路49号 阳明电商产业园4号楼 2F 上易网络");
            var addressAlias = HttpUtility.UrlEncode("公司");
            var param =$"addressInfoParam.consigneeName={name}&addressInfoParam.provinceId=15&addressInfoParam.cityId=1158&addressInfoParam.countyId=46345&addressInfoParam.townId=52176&addressInfoParam.consigneeAddress={consigneeAddress}&addressInfoParam.mobile=13586777526&addressInfoParam.fullAddress={fullAddress}&addressInfoParam.phone=&addressInfoParam.email=&addressInfoParam.addressAlias={addressAlias}&addressInfoParam.easyBuy=undefined";
            HtmlAnalysis req = new HtmlAnalysis();
            req.RequestMethod = "POST";
            req.RequestAccept = "text/plain, */*; q=0.01";
            req.RequestContentType = "application/x-www-form-urlencoded";
            req.RequestReferer = "http://easybuy.jd.com/address/getEasyBuyList.action";
            if (!string.IsNullOrEmpty(User.LoginCookies))
            {
                req.Headers.Add("Cookie", User.LoginCookies);
            }
            if (!string.IsNullOrEmpty(jdUserAgent))
            {
                req.RequestUserAgent = jdUserAgent;
            }

            var adds = req.HttpRequest("http://easybuy.jd.com/address/getEasyBuyList.action");

            var addressId = Regex.Match(adds, "id=\"addresssDiv-(?<x>\\d+)\">", ro).Groups["x"].Value;
            if (!string.IsNullOrEmpty(addressId))
            {
                return;
            }
            var res = req.HttpRequest(url, param);
            if(string.IsNullOrEmpty(res))
                LogServer.WriteLog(res, "JdOrderServer");

            //http://easybuy.jd.com/address...ssAllDefaultById.action addressId= 138324768 //设置为默认地址
        }

        public void mincatInfo()
        {
            string url = "http://cart.jd.com/cart/miniCartServiceNew.action";
            HtmlAnalysis req = new HtmlAnalysis();
            req.RequestAccept = "text/plain, */*; q=0.01";
            req.RequestContentType = "application/x-www-form-urlencoded";
            req.RequestAccept = "text/javascript, application/javascript, application/ecmascript, application/x-ecmascript, */*; q=0.01";
            req.RequestReferer = "http://cart.jd.com/addToCart.html";
            if (!string.IsNullOrEmpty(User.LoginCookies))
            {
                req.Headers.Add("Cookie", User.LoginCookies);
            }
            if (!string.IsNullOrEmpty(jdUserAgent))
            {
                req.RequestUserAgent = jdUserAgent;
            }
            var mincart = req.HttpRequest(url);

        }

        #endregion

    }
}
