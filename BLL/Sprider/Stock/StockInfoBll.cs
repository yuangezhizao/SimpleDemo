
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Commons;
using DataBase.Stock;
using Mode;
using NetDimension.Json.Linq;
using SpriderProxy;


namespace BLL.Sprider.Stock
{
    public class StockInfoBll: IStockInfoBll
    {

        private static SiteCookies _xqCookies;
        public static SiteCookies XqCookies {
            get
            {
                if (_xqCookies == null)
                {
                    _xqCookies = new SiteCookiesBll().GetOneByDomain("xueqiu.com");
                    if(string.IsNullOrEmpty(_xqCookies?.Cookies))
                        _xqCookies=new SiteCookies();
                    if (_xqCookies.UpdateTime.AddMinutes(30) >= DateTime.Now) return _xqCookies;
                    var result = HtmlAnalysis.GetResponseCookies("https://xueqiu.com/");
                    if (string.IsNullOrEmpty(result)) return _xqCookies;
                    _xqCookies.Cookies = result;
                    new SiteCookiesBll().SaveCookies(_xqCookies);
                }
                return _xqCookies;
            }
        }

        //private List<StockInfo> oldlist;
        public void GetALlStockInfo()
        {
            //string[] listType ={ "10","11","20", "21","27","15","25", "33", "14.1.1", "14.2.1", "14.3", "2850020", "24.1", "24.2", "24.3", "2850021", "285002", "2850013", "2850014", "2850022", "26" };
            string[] listType = { "10", "11", "20", "21", "27", "15", "25", "33",  "26" };
            foreach (var type in listType)
            {
                Addstock(type);
            }
           
        }

        private void Addstock(string type)
        {
            var oldlist = GetAllinfo();
            if (oldlist == null)
                return;
            var stocks = new List<StockInfo>();
            var page = Eastmoney.GetStockInfo(type);
            var item = JObject.Parse(page);
            foreach (var obj in item["rank"])
            {
                string[] templist = obj.ToString().Split(',');
                if (templist.Length < 13)
                    continue;
                var scode = templist[1];
                var sname = templist[2];
                var stockTypeadd = "";
                if (type == "10" || type == "11")
                    stockTypeadd = "sh";
                else if (type == "20" || type == "21")
                    stockTypeadd = "sz" ;
                else if (scode.IndexOf("000", StringComparison.Ordinal) == 0)
                    stockTypeadd = "sh";
                else if (scode.IndexOf("399", StringComparison.Ordinal)==0)
                    stockTypeadd = "sz";
                if (oldlist.Exists(c => c.StockNo == scode))
                    continue;
                decimal currentprice;
                decimal.TryParse(templist[3], out currentprice);
                decimal amp;
                decimal.TryParse(templist[4], out amp);
                decimal amper;
                decimal.TryParse(templist[5].Replace("%", ""), out amper);
                decimal zhenfu;
                decimal.TryParse(templist[6], out zhenfu);
                int volume;
                int.TryParse(templist[7], out volume);
                int turnover;
                int.TryParse(templist[8], out turnover);
                decimal oldprice;
                decimal.TryParse(templist[9], out oldprice);
                decimal startprice;
                decimal.TryParse(templist[10], out startprice);
                decimal maxprice;
                decimal.TryParse(templist[11], out maxprice);
                decimal minprice;
                decimal.TryParse(templist[12], out minprice);
                StockInfo newski = new StockInfo
                {
                    StockNo = scode,
                    StockName = sname,
                    CurrentPrice = currentprice,
                    Amplitude = amp,
                    AmplitudePercent = amper,
                    Huanshou = 0,
                    Liangbi = 0,
                    Maxprice = maxprice,
                    Minprice = minprice,
                    Oldprice = oldprice,
                    Startprice = startprice,
                    Volume = volume,
                    Turnover = turnover,
                    Zhenfu = zhenfu,
                    StockType = type,
                    StockTypeAdd = stockTypeadd,
                    CreateDate = DateTime.Now
                };
                oldlist.Add(newski);
                stocks.Add(newski);
                if (stocks.Count > 50)
                {
                    new StockinfoDB().AddStockinfo(stocks);
                    stocks.Clear();
                }

            }
            if (stocks.Count > 0)
                new StockinfoDB().AddStockinfo(stocks);
        }
        
        public List<StockInfo> GetAllinfo()
        {
            return new StockinfoDB().GetAllinfo();
        }

        public void GetXueqiuStockDetial(StockInfo info)
        {
            //if(info.CurrentPrice==0)
            //    return;
            //GetRzrqInfo(info);
            //return;
            var code = (info.StockTypeAdd+ info.StockNo).ToUpper();
        
            string url = $"https://xueqiu.com/v4/stock/quote.json?code={code}&_=1465259721266";
            HtmlAnalysis request = new HtmlAnalysis();
            request.RequestAccept = "application/json, text/javascript, */*; q=0.01";
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            if (XqCookies != null)
                request.Headers.Add("Cookie", XqCookies.Cookies);
            request.RequestUserAgent =
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";

            int error = 0;
            var currentInfo = "";
            do
            {
                try
                {
                    currentInfo = request.HttpRequest(url);
                    if (string.IsNullOrEmpty(currentInfo))
                    {
                        error++;
                        Thread.Sleep(3000);
                    }
                    else
                    {
                        break;
                    }
                
                }
                catch (Exception)
                {
                    Thread.Sleep(3000);
                    error++;
                }    
               


            } while (error>10);
     

            JObject item;
            try
            {
                item = JObject.Parse(currentInfo.Replace("%",""));
            }
            catch (Exception ex)
            {
                LogServer.WriteLog("url="+url+"page="+ currentInfo + ex.Message,"StockDetial");
                item = null;
            }
          
            if (item == null)
            {
                LogServer.WriteLog($"error json {info.StockNo} url:{url},detial:{currentInfo}", "StockDetial");
            }
            if (item != null)
            {
                var xqItem = item[code];
                if (xqItem == null)
                {
                    LogServer.WriteLog($"error json code not find {info.StockNo} url:{url},detial:{currentInfo}",
                        "StockDetial");
                    return;
                }
                try
                {
                    XqStockDayReport xq = new XqStockDayReport();
                    xq.StockNo = code;


                    if (xqItem["exchange"].ToString() != "")
                        xq.Exchange = xqItem["exchange"].Value<string>();

                    xq.Code = xqItem["code"].Value<string>();
                    xq.StockName = xqItem["name"].Value<string>();
                    xq.CurrentPrice = xqItem["current"].Value<decimal>();
                    xq.Range = xqItem["percentage"].Value<float>();
                    xq.RangeRatio = xqItem["change"].Value<float>();
                    xq.OpenPrice = xqItem["open"].Value<decimal>();
                    xq.Maxprice = xqItem["high"].Value<decimal>();
                    xq.Minprice = xqItem["low"].Value<decimal>();
                    xq.Closeprice = xqItem["close"].Value<decimal>();
                    xq.LastCloseprice = xqItem["last_close"].Value<decimal>();
                    xq.High52Week = xqItem["high52week"].Value<float>();
                    xq.Low52Week = xqItem["low52week"].Value<float>();
                    xq.Volume = xqItem["volume"].Value<float>();
                    if (xqItem["volumeAverage"].ToString() != "")
                        xq.VolumeAverage = xqItem["volumeAverage"].Value<float>();
                    if (xqItem["marketCapital"].ToString() != "")
                        xq.MarketCapital =  xqItem["marketCapital"].Value<float>();
                    if (xqItem["eps"].ToString() != "")
                        xq.Eps = xqItem["eps"].Value<float>();


                    if (xqItem["pe_ttm"].ToString() != "")
                        xq.Pettm = xqItem["pe_ttm"].Value<float>();
                    if (xqItem["pe_lyr"].ToString() != "")
                        xq.PElyr = xqItem["pe_lyr"].Value<float>();
                    if (xqItem["beta"].ToString() != "")
                        xq.Beta = xqItem["beta"].Value<float>();

                    if (xqItem["totalShares"].ToString() != "")
                        xq.TotalShares = xqItem["totalShares"].Value<float>();


                    //xqItem["totalShares"].Value<float>();
                    if (xqItem["afterHours"].ToString() != "")
                        xq.AfterHours = xqItem["afterHours"].Value<float>();
                    if (xqItem["afterHoursPct"].ToString() != "")
                        xq.AfterHoursPct = xqItem["afterHoursPct"].Value<float>();
                    if (xqItem["afterHoursChg"].ToString() != "")
                        xq.AfterHoursChg = xqItem["afterHoursChg"].Value<float>();
                    xq.UpdateAt = xqItem["updateAt"].ToString() != "" ? xqItem["updateAt"].Value<float>() : 0;
                    // xqItem["updateAt"].Value<float>();
                    if (xqItem["dividend"].ToString() != "")
                        xq.Dividend = xqItem["dividend"].Value<float>();
                
                    xq.Yield = xqItem["yield"].ToString() != "" ? xqItem["yield"].Value<float>() : 0;
                    xq.Turnoverrate = xqItem["turnover_rate"].ToString() != "" ? xqItem["turnover_rate"].Value<float>() : 0;
                    //xqItem["turnover_rate"].Value<float>();
                    xq.InstOwn = xqItem["instOwn"].ToString() != "" ? xqItem["instOwn"].Value<float>() : 0;
                    // xqItem["instOwn"].Value<float>();
                    xq.Risestop = xqItem["rise_stop"].ToString() != "" ? xqItem["rise_stop"].Value<float>() : 0;
                    //  xqItem["rise_stop"].Value<float>();
                    xq.FallStop = xqItem["fall_stop"].ToString() != "" ? xqItem["fall_stop"].Value<float>() : 0;
                    // xqItem["fall_stop"].Value<float>();
                    xq.CurrencyUnit = xqItem["currency_unit"].Value<string>();
                    if (xqItem["amount"].ToString() != "")
                    {
                        var tempat = xqItem["amount"].Value<string>();
                        if (tempat.Contains("E"))
                        {
                            xq.Amount = float.Parse(tempat, System.Globalization.NumberStyles.Float);
                        }
                        else
                        {
                            float tempft;
                            float.TryParse(tempat, out tempft);
                            xq.Amount = tempft;
                        }

                    }

                    xq.NetAssets = xqItem["net_assets"].ToString() != "" ? xqItem["net_assets"].Value<float>() : 0;

             
                    xq.Hasexist = xqItem["hasexist"].Value<string>();
                    xq.HasWarrant = xqItem["has_warrant"].Value<string>();

                    xq.Type = xqItem["type"].ToString() != "" ? xqItem["type"].Value<float>() : 0;

                    xq.Flag = xqItem["flag"].ToString() != "" ? xqItem["flag"].Value<int>() : 0;

                    xq.RestDay = xqItem["rest_day"].Value<string>();

                    xq.Amplitude = xqItem["amplitude"].ToString() != "" ? xqItem["amplitude"].Value<float>() : 0;

                    xq.LotSize = xqItem["lot_size"].ToString() != "" ? xqItem["lot_size"].Value<float>() : 0;
                    //xqItem["lot_size"].Value<float>();
                    xq.MinOrderQuantity = xqItem["min_order_quantity"].ToString() != ""
                        ? xqItem["min_order_quantity"].Value<float>()
                        : 0; // xqItem["min_order_quantity"].Value<float>();
                    xq.MaxOrderQuantity = xqItem["max_order_quantity"].ToString() != ""
                        ? xqItem["max_order_quantity"].Value<float>()
                        : 0; // xqItem["max_order_quantity"].Value<float>();
                    xq.TickSize = xqItem["tick_size"].ToString() != "" ? xqItem["tick_size"].Value<float>() : 0;
                    // xqItem["tick_size"].Value<float>();
                    xq.KzzStockSymbol = xqItem["kzz_stock_symbol"].Value<string>();
                    xq.KzzStockName = xqItem["kzz_stock_name"].Value<string>();
                    xq.KzzStockCurrent = xqItem["kzz_stock_current"].ToString() != ""
                        ? xqItem["kzz_stock_current"].Value<float>()
                        : 0; //xqItem["kzz_stock_current"].Value<float>();
                    xq.KzzConvertPrice = xqItem["kzz_convert_price"].ToString() != ""
                        ? xqItem["kzz_convert_price"].Value<float>()
                        : 0; //xqItem["kzz_convert_price"].Value<float>();
                    xq.kzzcovertValue = xqItem["kzz_covert_value"].ToString() != ""
                        ? xqItem["kzz_covert_value"].Value<float>()
                        : 0; // xqItem["kzz_covert_value"].Value<float>();
                    xq.KzzCpr = xqItem["kzz_cpr"].ToString() != "" ? xqItem["kzz_cpr"].Value<float>() : 0;
                    //xqItem["kzz_cpr"].Value<float>();
                    xq.KzzPutbackPrice = xqItem["kzz_putback_price"].ToString() != ""
                        ? xqItem["kzz_putback_price"].Value<float>()
                        : 0; // xqItem["kzz_putback_price"].Value<float>();
                    xq.KzzConvertTime = xqItem["kzz_convert_time"].Value<string>();
                    xq.KzzRedemptPrice = xqItem["kzz_redempt_price"].ToString() != ""
                        ? xqItem["kzz_redempt_price"].Value<float>()
                        : 0; //xqItem["kzz_redempt_price"].Value<float>();
                    xq.KzzStraightPrice = xqItem["kzz_straight_price"].ToString() != ""
                        ? xqItem["kzz_straight_price"].Value<float>()
                        : 0; // xqItem["kzz_straight_price"].Value<float>();
                    xq.KzzStockPercent = xqItem["kzz_stock_percent"].Value<string>();
                    xq.Pb = xqItem["pb"].ToString() != "" ? xqItem["pb"].Value<float>() : 0; // xqItem["pb"].Value<float>();
                    xq.BenefitBeforeTax = xqItem["benefit_before_tax"].ToString() != ""
                        ? xqItem["benefit_before_tax"].Value<float>()
                        : 0; // xqItem["benefit_before_tax"].Value<float>();
                    xq.BenefitAfterTax = xqItem["benefit_after_tax"].ToString() != ""
                        ? xqItem["benefit_after_tax"].Value<float>()
                        : 0; //  xqItem["benefit_after_tax"].Value<float>();
                    xq.ConvertBondRatio = xqItem["convert_bond_ratio"].Value<string>();
                    xq.Totalissuescale = xqItem["totalissuescale"].Value<string>();
                    xq.Outstandingamt = xqItem["outstandingamt"].Value<string>();
                    xq.Maturitydate = xqItem["maturitydate"].Value<string>();
                    xq.RemainYear = xqItem["remain_year"].Value<string>();
                    xq.ConvertRate = xqItem["convertrate"].ToString() != "" ? xqItem["convertrate"].Value<float>() : 0;
                    // xqItem["convertrate"].Value<float>();
                    xq.Interestrtmemo = xqItem["interestrtmemo"].Value<string>();
                    xq.ReleaseDate = xqItem["release_date"].Value<string>();
                    xq.Circulation = xqItem["circulation"].ToString() != "" ? xqItem["circulation"].Value<float>() : 0;
                    // xqItem["circulation"].Value<float>();
                    xq.ParValue = xqItem["par_value"].ToString() != "" ? xqItem["par_value"].Value<float>() : 0;
                    //  xqItem["par_value"].Value<float>();
                    xq.DueTime = xqItem["due_time"].ToString() != "" ? xqItem["due_time"].Value<float>() : 0;
                    // xqItem["due_time"].Value<float>();
                    xq.ValueDate = xqItem["value_date"].Value<string>();
                    xq.DueDate = xqItem["due_date"].Value<string>();
                    xq.Publisher = xqItem["publisher"].Value<string>();
                    xq.RedeemType = xqItem["redeem_type"].Value<string>();
                    xq.BondType = xqItem["bond_type"].Value<string>();
                    xq.Warrant = xqItem["warrant"].Value<string>();
                    xq.SaleRrg = xqItem["sale_rrg"].Value<string>();
                    xq.Rate = xqItem["rate"].Value<string>();
                    xq.AfterHourVol = xqItem["after_hour_vol"].ToString() != "" ? xqItem["after_hour_vol"].Value<int>() : 0;
                    // xqItem["after_hour_vol"].Value<int>();
                    xq.FloatShares = xqItem["float_shares"].ToString() != "" ? xqItem["float_shares"].Value<float>() : 0;
                    //xqItem["float_shares"].Value<float>();

                    if (xqItem["float_market_capital"].ToString() != "")
                    {
                        var tempat = xqItem["float_market_capital"].Value<string>();
                        if (tempat.Contains("E"))
                        {
                            xq.FloatMarketCapital = float.Parse(tempat, System.Globalization.NumberStyles.Float);
                        }
                        else
                        {
                            float tempft;
                            float.TryParse(tempat, out tempft);
                            xq.FloatMarketCapital = tempft;
                        }

                    }

                    //xq.FloatMarketCapital = xqItem["float_market_capital"].Value<string>();
                    xq.DisnextPayDate = xqItem["disnext_pay_date"].Value<string>();
                    //xq.ConvertRate = xqItem["convert_rate"].Value<float>();
                    if (xqItem["psr"].ToString() != "")
                        xq.Psr = xqItem["psr"].Value<float>();
                    xq.CreateDate = DateTime.Now;


                    //,\"float_market_capital\":\"3.88908818298E9\",\"disnext_pay_date\":\"\",\"convert_rate\":\"0.0\",\"psr\":\"5.4811\"}}"

                    new XqStockDayReportDB().AddXqStockDayReport(xq);
                }
                catch (Exception ex)
                {

                    LogServer.WriteLog($"error json {info.StockNo} url:{url},detial:{currentInfo},ex:{ex.Message}",
                        "StockDetial");
                }
            }
        }

        public void GetNewStockInfo()
        {

            int totalpage = 2;
            var oldlist = GetAllinfo();
            if (oldlist == null || oldlist.Count == 0)
                return;
            for (int i = 1; i <= totalpage; i++)
            {
                string url =$"https://xueqiu.com/stock/cata/stocklist.json?page={i}&size=30&order=desc&orderby=percent&type=11%2C12&_=1487732149945"; //$"https://xueqiu.com/proipo/query.json?page={i}&size=30&order=desc&orderBy=list_date&stockType=&column=symbol%2Cname%2Conl_subcode%2Clist_date%2Cactissqty%2Conl_actissqty%2Conl_submaxqty%2Conl_subbegdate%2Conl_unfrozendate%2Conl_refunddate%2Ciss_price%2Conl_frozenamt%2Conl_lotwinrt%2Conl_lorwincode%2Conl_lotwiner_stpub_date%2Conl_effsubqty%2Conl_effsubnum%2Conl_onversubrt%2Coffl_lotwinrt%2Coffl_effsubqty%2Coffl_planum%2Coffl_oversubrt%2Cnapsaft%2Ceps_dilutedaft%2Cleaduwer%2Clist_recomer%2Cacttotraiseamt%2Conl_rdshowweb%2Conl_rdshowbegdate%2Conl_distrdate%2Conl_drawlotsdate%2Cfirst_open_price%2Cfirst_close_price%2Cfirst_percent%2Cfirst_turnrate%2Cstock_income%2Conl_lotwin_amount%2Clisted_percent%2Ccurrent%2Cpe_ttm%2Cpb%2Cpercent%2Chasexist&type=quote&_=1471824353300";

                HtmlAnalysis request = new HtmlAnalysis();
                request.RequestAccept = "application/json, text/javascript, */*; q=0.01";
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                if (XqCookies != null)
                    request.Headers.Add("Cookie", XqCookies.Cookies);
                request.RequestUserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
                var currentInfo = request.HttpRequest(url);
                var item = JObject.Parse(currentInfo);

                if (item?["data"] == null)
                {
                    LogServer.WriteLog($"error GetNewStockInfo url:{url},detial:{currentInfo}", "StockDetial");
                    return;
                }
                if (i == 1)
                {
                    int total = item["count"].Value<int>();
                    if (total%30 == 0)
                        totalpage = total/30;
                    else
                    {
                        totalpage = total/30 + 1;
                    }
                }

        
                var stocks = new List<StockInfo>();
                foreach (var obj in item["data"])
                {
                    var scode = obj[0].Value<string>().ToLower();
                    if (oldlist.Exists(c => c.StockNo == scode))
                        continue;
                    StockInfo newski = new StockInfo
                    {
                        StockNo = scode,
                        StockName = obj[1].Value<string>(),
                        CurrentPrice = 0,
                        Amplitude = 0,
                        Huanshou = 0,
                        Liangbi = 0,
                        Maxprice = 0,
                        Minprice = 0,
                        Oldprice = 0,
                        Startprice = 0,
                        Volume = 0,
                        Turnover = 0,
                        Zhenfu = 0
                    };
                    oldlist.Add(newski);
                    stocks.Add(newski);

                }
                if (stocks.Count > 0)
                    new StockinfoDB().AddStockinfo(stocks);
            }
          
        }

       //var testpage = HtmlAnalysis.Gethtmlcode("http://web.sqt.gtimg.cn/q=sh600886?r=0.2674542577107566");

        //var pageinfo1 = HtmlAnalysis.Gethtmlcode("http://d.10jqka.com.cn/v2/time/hs_600662/last.js");



        public void StockMonitor()
        {
            var list =new StockMonitorBll().GetALlStockMonitor();
            if(list==null|| list.Count==0)
                return;
            foreach (var item in list)
            {
                SkMonitor(item);
            }
            LogServer.WriteLog("finished ", "StockMonitor");
           
        }

        private const string Email = "195896636@qq.com";

       public static Dictionary<string,DateTime> MonitList = new Dictionary<string, DateTime>();

        private void SkMonitor(StockMonitor item)
        {
            string detial = GetCurrentStock(item.StockNo);
            if (string.IsNullOrEmpty(detial))
                return;
            var jtk = JToken.Parse(detial);
            if (jtk[item.StockNo] == null)
                return;
            string cprice = jtk[item.StockNo]["current"].ToString();
            decimal ctprice;
            decimal.TryParse(cprice, out ctprice);
            if(ctprice <= 0)
                return;

            item.CtPrice = ctprice;
            sendMsg(item,detial);


        }

        private void sendMsg(StockMonitor item,string detial)
        {
            if (MonitList.ContainsKey(item.StockNo))
            {
                //确保在3个小时之内只会发送一条信息
                var sendtime = MonitList[item.StockNo];
                if(sendtime.AddHours(3)>DateTime.Now)
                    return;
            }
            else
            {
                MonitList.Add(item.StockNo, DateTime.Now);
            }

            if (item.CtPrice < item.MinPrice)
            {
                string title = item.StockNo + "价格监控 currect:" + item.CtPrice + " MinPrice:" + item.MinPrice;
                string body = item.StockNo + " currect:" + item.CtPrice + " MinPrice:" + item.MinPrice + "\r\n" + detial;

                EmailServer.SendMail(body, title, new[] { Email });
            }
            else if (item.CtPrice > item.MaxPrice)
            {
                string title = item.StockNo + "价格监控 currect:" + item.CtPrice + " MaxPrice:" + item.MaxPrice;
                string body = item.StockNo + " currect:" + item.CtPrice + " MaxPrice:" + item.MaxPrice + "\r\n" + detial;

                EmailServer.SendMail(body, title, new[] { Email });
            }

            LogServer.WriteLog(item.StockNo + "价格监控 currect:" + item.CtPrice + " MaxPrice:" + item.MaxPrice, "StockSendMsg");
        }



        public string GetCurrentStock(string num)
        {
            string tempurl = $"https://xueqiu.com/v4/stock/quote.json?code={num}";
            HtmlAnalysis request = new HtmlAnalysis();
        
            if (XqCookies != null)
            {
                request.Headers.Add("Cookie", XqCookies.Cookies);
                request.RequestUserAgent = XqCookies.UserAgent;
            }
            request.RequestAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            return request.HttpRequest(tempurl);
            
        }


        public void GetRzrqInfo(StockInfo info)
        {
           string code = Regex.Match(info.StockNo, "(?<x>\\d+)", RegexOptions.Singleline).Groups["x"].Value;// code.Substring(2);
            string mkt = "1";
            if (info.StockTypeAdd == "sz")
            {
                mkt = "2";
            }

            string url = "http://datainterface.eastmoney.com/EM_DataCenter/JS.aspx?type=FD&sty=MTE&mkt="+ mkt + "&code=" + code +
                         "&st=0&sr=1&p=1&ps=50";
            var page = HtmlAnalysis.Gethtmlcode(url).TrimStart('(').TrimEnd(')');
            if(string.IsNullOrEmpty(page)|| page== "[{stats:false}]")
                return;
            JArray list = JArray.Parse(page);
            List< MarginTrading> tlist = new List<MarginTrading>();

            var olditem = new MarginTradingBll().GetNewestTrading(code);
            DateTime starTime=DateTime.Parse("1900-01-01");
            if (olditem != null)
                starTime = olditem.ReportDate;
            foreach (var item in list)
            {
                var templist = item.ToString().Split(',');
                try
                {
                    MarginTrading mt = new MarginTrading();
                    mt.StockNo = templist[0];
                    mt.StockName = templist[2];
                    mt.ReportDate = DateTime.Parse(templist[4]);

                    if(mt.ReportDate<=starTime)
                        continue;
                    mt.Rqrz = decimal.Parse(templist[3]);

                    mt.Rqchl = decimal.Parse(templist[5]);
                    mt.Rqmcl = decimal.Parse(templist[6]);
                    mt.Rqye = decimal.Parse(templist[7]);
                    mt.Rqyl = decimal.Parse(templist[8]);
                    mt.Rzchl = decimal.Parse(templist[9]);
                    mt.Rzmre = decimal.Parse(templist[10]);
                    mt.Rzrqye = decimal.Parse(templist[11]);
                    mt.Rzye = decimal.Parse(templist[12]);
                    mt.Rzjme = decimal.Parse(templist[13]);

                    tlist.Add(mt);
                }
                catch (Exception ex)
                {
                    continue;
                }


            }
            new MarginTradingBll().AddMarginTrading(tlist);
            //http://data.eastmoney.com/rzrq/detail/000420.html
            //http://datainterface.eastmoney.com/EM_DataCenter/JS.aspx?type=FD&sty=MTE&mkt=1&code=600887&st=0&sr=1&p=1&ps=50&js=var%20YFMtmCig={pages:(pc),data:[(x)]}
            //"600887,融资融券_沪证,伊利股份,2506196565,  2016/11/24,     247700.00,       1758784,        49453078.50,            2616565,        269369738.00,       517995741,              2605102721.50,          2555649643,       248626002"
            //                                                            融券偿还量       融券卖出量      融券余额(元)            融券余量        融资偿还额(元)      融资买入额(元)          融资融券余额            融资余额(元)      融资净买额(元)
        }

        //var head =
        //    HtmlAnalysis.Gethtmlcode("http://stockpage.10jqka.com.cn/spService/002801/Header/realHeader");
        //                                                                                                                            现价         涨跌百分比     涨跌金额            成交量                         成交额         开盘价      昨收           最高        最低        换手            市盈率(动) 内盘         外盘        均价           振幅       涨停         跌停                          委比      委差
        //{"stockcode":"600662","stockname":"\u5f3a\u751f\u63a7\u80a1","fieldcode":"1150","fieldname":"\u516c\u4ea4","fieldjp":"gj","xj":"11.21","zdf":"-3.69%","zde":"-0.43","cjl":"34.81 \u4e07\u624b","cje":"3.92 \u4ebf\u5143","kp":"11.46","zs":"11.64","zg":"11.49","zd":"11.00","hs":"3.31%","syl":"81.26","np":189715,"wp":150836,"jj":"11.27","zf":"4.21%","zt":"12.80","dt":"10.48","field":"0.00","wb":"7.86","wc":132,"buy1":"11.20","buy1data":20,"buy2":"11.19","buy2data":185,"buy3":"11.18","buy3data":143,"buy4":"11.17","buy4data":230,"buy5":"11.16","buy5data":328,"sell1":"11.21","sell1data":111,"sell2":"11.22","sell2data":185,"sell3":"11.23","sell3data":196,"sell4":"11.24","sell4data":81,"sell5":"11.25","sell5data":201}
        //CookieContainer list = new CookieContainer();
        //SimulationCookie.GetCookie("https://passport.jd.com/new/login.aspx?ReturnUrl=http://www.jd.com/",
        //    cookie =>
        //    {
        //        list.Add(cookie);
        //    });
    }
}
