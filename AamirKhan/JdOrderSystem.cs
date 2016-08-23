using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using BLL;
using Commons;

namespace AamirKhan
{
    //public class JdOrderSystem
    //{
    //    public string Eid { get; set; }
    //    public string Fp { get; set; }
    //    protected static RegexOptions  ro = RegexOptions.Singleline | RegexOptions.IgnoreCase;
    //    public static string jdCookies { get; set; }

    //    public static string jdUserAgent { get; set; }

    //    public const  string MdCaturl= "http://cart.jd.com/cart/dynamic/reBuyForOrderCenter.action?action=AddSkus&nums={1}&wids={0}";
    //    public JdOrderSystem()
    //    {
    //        Eid = "8E54E4CB9E9D66F99E2423476A2D0BA7B177D9FDE30A39033C6C7191346B6BCE9B084078401AEE3569559D0D83DBB8C4";
    //        Fp = "f3595909a05d0c87bd723f1cbfe6a82e";
    //        var cookies = new SiteCookiesBll().GetOneByDomain("www.jd.com");
    //        if (cookies != null)
    //        {
    //            jdCookies = cookies.Cookies;
    //            if (jdCookies.Contains("eid"))
    //            {
    //                Eid = Regex.Match(jdCookies, "eid:(?<x>.*?);", ro).Groups["x"].Value;
    //                Fp = Regex.Match(jdCookies, "fp:(?<x>.*?);", ro).Groups["x"].Value;
    //            }
    //            jdUserAgent = cookies.UserAgent;
    //        }
    //    }

    //    public void Login(string name, string pwd)
    //    {
    //        string url = "https://passport.jd.com/new/login.aspx?ReturnUrl=http%3A%2F%2Fwww.jd.com%2F";
    //        string cookies1 = HtmlAnalysis.GetResponseCookies("http://www.jd.com");
    //        string cookies = "";
    //        HtmlAnalysis rq  = new HtmlAnalysis();
    //        rq.Headers.Add("X-Requested-With", "XMLHttpRequest");
    //        rq.RequestContentType = "application/x-www-form-urlencoded; charset=utf-8";
    //        rq.RequestReferer = "https://passport.jd.com/new/login.aspx?ReturnUrl=https%3A%2F%2Fwww.jd.com%2F";
    //        rq.Headers.Add("Cookie",  Fp + "=" + Eid);
    //        rq.RequestUserAgent = jdUserAgent;
    //        var logpage= rq.HttpRequest(url);
    //        if (rq.ResultResponseHeader!=null&&rq.ResultResponseHeader.ContainsKey("Cookie"))
    //        {
    //            cookies = rq.ResultResponseHeader["Cookie"].Replace("Path=/;","").Replace("HttpOnly;", "").Replace("  ,", "");
    //        }
    //        var uuid = Regex.Match(logpage, "name=\"uuid\" value=\"(?<x>.*?)\"").Groups["x"].Value;
         
    //        string loginurl = "https://passport.jd.com/uc/loginService?r=0.36799998356697805&version=2015&ReturnUrl=http%3A%2F%2Fwww.jd.com%2F&uuid=" + uuid;

    //        string token = Regex.Match(logpage, "name=\"_t\" id=\"token\" value=\"(?<x>.*?)\"").Groups["x"].Value;
    //        string lgkey = Regex.Match(logpage, "<input type=\"hidden\" name=\"(?<x>\\w+)\" value=\"(?<y>\\w+)\"/>", ro).Groups["x"].Value;
    //        string lgvalue = Regex.Match(logpage, "<input type=\"hidden\" name=\"(?<x>\\w+)\" value=\"(?<y>\\w+)\"/>", ro).Groups["y"].Value;
    //        //string loginType = Regex.Match(logpage, "id=\"loginType\" value=\"(?<x>.*?)\"", ro).Groups["y"].Value; 

    //        Dictionary<string, string> paramlist = new Dictionary<string, string>();
    //        paramlist.Add("uuid", uuid);
    //        paramlist.Add("machineNet", "");
    //        paramlist.Add("machineCpu", "");
    //        paramlist.Add("machineDisk", "");
    //        paramlist.Add("eid", Eid);
    //        paramlist.Add("fp", Fp);
    //        paramlist.Add("_t", token);
    //        paramlist.Add("loginType", "f");
    //        paramlist.Add(lgkey, lgvalue);
    //        paramlist.Add("loginname", HttpUtility.UrlEncode(name));
    //        paramlist.Add("nloginpwd", pwd);
    //        paramlist.Add("loginpwd", pwd);
    //        paramlist.Add("chkRememberMe", "on");
    //        string reqCookie = jdCookies + ";" + cookies + Fp + "=" + Eid;
    //        if (rq.Headers.ContainsKey("Cookie"))
    //        {
    //            rq.Headers["Cookie"] = reqCookie;
    //        }
    //        else
    //        {
    //            rq.Headers.Add("Cookie", reqCookie);
    //        }
        
    //        paramlist.Add("authcode", "");
    //        string param = "";
    //        foreach (var item in paramlist)
    //        {
    //            param += item.Key + "=" + item.Value + "&";
    //        }
    
    //        rq.RequestAccept = "text/plain, */*; q=0.01";
    //        rq.RequestMethod = "POST";

    //        var Page = rq.HttpRequest(loginurl ,param.TrimEnd('&'));
    //        string newcookie = "";
    //        if (Page.Contains("success"))
    //        {
    //            if (rq.ResultResponseHeader != null && rq.ResultResponseHeader.ContainsKey("Set-Cookie"))
    //            {
    //                var logincookies = rq.ResultResponseHeader["Set-Cookie"].Replace("Path=/;", "").Replace("HttpOnly;", "").Replace("  ,", "");
    //                newcookie = logincookies + reqCookie;
    //                SiteUserInfoBll bll= new SiteUserInfoBll();
    //                var item = bll.GetOngeUser(name);
    //                if (item != null)
    //                {
    //                    item.LogCookies = logincookies;
    //                    item.LogCookiesUpdatetime=DateTime.Now;
    //                    bll.UpdateUser(item);
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 提交订单
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool SubmitOrder()
    //    {
    //        var param = "";
    //        var checkcodeTxt = "";
    //        var checkCodeRid = "";
    //        var payPassword = "";
    //        var mobileForPresale = "";
    //        var presalePayType = "";// 预售验证手机号


    //        if (!string.IsNullOrEmpty(checkcodeTxt))
    //        {
    //            param = param + "submitOrderParam.checkcodeTxt=" + checkcodeTxt;
    //        }
    //        param = param + "?overseaPurchaseCookies="; //+$("#overseaPurchaseCookies").val();

    //        //if (!isEmpty($("#checkCodeDiv").html()))
    //        //{
    //        //    checkCodeRid = $("#checkcodeRid").val();
    //        //}
    //        if (!string.IsNullOrEmpty(checkCodeRid)) //验证码
    //        {
    //            param = param + "&submitOrderParam.checkCodeRid=" + checkCodeRid;
    //        }
    //        if (!string.IsNullOrEmpty(payPassword)) //支付密码
    //        {
    //            param = param + "&submitOrderParam.payPassword=" + payPassword;
    //            //encodeURIComponent(stringToHex(payPassword));
    //        }
    //        var sopNotPutInvoice = ""; //必须
    //        if (!string.IsNullOrEmpty(sopNotPutInvoice))
    //        {
    //            param = param + "&submitOrderParam.sopNotPutInvoice=" + sopNotPutInvoice;//$("#sopNotPutInvoice").val();
    //        }
    //        else {
    //            param = param + "&submitOrderParam.sopNotPutInvoice=false";
    //        }

    //        if (!string.IsNullOrEmpty(mobileForPresale))
    //        {
    //            param = param + "&submitOrderParam.presaleMobile=" + mobileForPresale;
    //        }
    //        var trackID = "";
    //        if (!string.IsNullOrEmpty(trackID))
    //        {
    //            param = param + "&submitOrderParam.trackID=" + trackID;
    //        }
    //        var regionId = "";
    //        if (!string.IsNullOrEmpty(regionId))
    //        {
    //            param += "&regionId=" + regionId;
    //        }

    //        var easyBuyFlag = ""; //$("#easyBuyFlag").val(); 一键购
    //        if (easyBuyFlag == "1" || easyBuyFlag == "2")
    //        {
    //            param += "&ebf=" + easyBuyFlag;
    //        }

    //        var ignorePriceChange = "0"; //$('#ignorePriceChange').val();
    //        if (!string.IsNullOrEmpty(ignorePriceChange))
    //        {
    //            param += "&submitOrderParam.ignorePriceChange=" + ignorePriceChange;
    //        }

    //        var onlinepaytype = "0"; //$(".payment-item.item-selected").attr('onlinepaytype');
    //        var paymentId = "4"; //$(".payment-item.item-selected").attr('payid');


    //        //if ($(".payment-item[onlinepaytype=1]").length == 0 ||$(".payment-item[onlinepaytype=1]").hasClass("payment-item-disabled")){
    //        //    param += "&submitOrderParam.btSupport=0";
    //        //}else{
    //        //    param += "&submitOrderParam.btSupport=1";
    //        //}
    //        param += "&submitOrderParam.btSupport=0";

    //        if (!string.IsNullOrEmpty(Eid))
    //        {
    //            param += "&submitOrderParam.eid=" + Eid;
    //        }
  
    //        if (!string.IsNullOrEmpty(Fp))
    //        {
    //            param += "&submitOrderParam.fp=" + Fp;
    //        }
    //        var isBestCoupon = "";//$('#isBestCoupon').val();
    //        if (!string.IsNullOrEmpty(isBestCoupon))
    //        {
    //            param += "&submitOrderParam.isBestCoupon=" + isBestCoupon;
    //        }
    //        string action = "http://trade.jd.com/shopping/order/submitOrder.action";


    //        HtmlAnalysis req = new HtmlAnalysis();
    //        //req.RequestMethod = "POST";

    //        if (!string.IsNullOrEmpty(jdCookies))
    //        {
    //            req.Headers.Add("Cookie",jdCookies);
    //        }
    //        if (!string.IsNullOrEmpty(jdUserAgent))
    //        {
    //            req.RequestUserAgent = jdUserAgent;
    //        }
    //        req.RequestMethod = "POST";
    //        req.RequestReferer = "http://trade.jd.com/shopping/order/getOrderInfo.action?rid=1470279644415";
    //        req.RequestAccept = "application/json, text/javascript, */*; q=0.01";

    //        var orderpage = req.HttpRequest(action + param);
    //        if (orderpage.Contains("\"success\":true"))
    //        {
    //            return true;
    //        }
    //        LogServer.WriteLog(orderpage,"orderinfo");
    //        return false;
    //    }

    //    /// <summary>
    //    /// 加入购物车
    //    /// </summary>
    //    public void AddCat(string skuid, string num)
    //    {
    //        var url = string.Format(MdCaturl, skuid, num);
           
    //         HtmlAnalysis req=new HtmlAnalysis();
    //        req.RequestUserAgent = jdUserAgent;
    //        req.Headers.Add("Cookie", jdCookies);
    //       var catpage= req.HttpRequest(url);
    //    }
    //}


}
