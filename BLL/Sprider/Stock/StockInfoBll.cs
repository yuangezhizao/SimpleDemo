
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using Commons;
using DataBase.Stock;
using Mode;
using NetDimension.Json.Linq;


namespace BLL.Sprider.Stock
{
    public class StockInfoBll: IStockInfoBll
    {

        private static SiteCookies xqCookies;
        public static SiteCookies XqCookies {
            get
            {
                if (xqCookies == null)
                {
                    xqCookies = new SiteCookiesBll().GetOneByDomain("xueqiu.com");

                    if (xqCookies.UpdateTime.AddMinutes(30) >= DateTime.Now) return xqCookies;
                    var result = HtmlAnalysis.GetResponseCookies("https://xueqiu.com/");
                    if (string.IsNullOrEmpty(result)) return xqCookies;
                    xqCookies.Cookies = result;
                    new SiteCookiesBll().SaveCookies(xqCookies);
                }
                return xqCookies;
            }
        }

        private List<StockInfo> oldlist;
        public void GetALlStockInfo()
        {
            //GetStockDetial();
            oldlist = new StockinfoDB().GetAllinfo() ?? new List<StockInfo>();

            var files = DocumentServer.GetAllFileNameByPath(@"F:\d");

            foreach (var file in files)
            {
                GetStockExcel(@"F:\d\" + file);
            }
  
            return;
            //for (int i = 1; i < 11; i++)
            //{
            //    GetStockExcel(@"F:\s\"+i+".xls");
            //}
            //return;
            //for (int j = 0; j < 3; j++)
            //{
            //    int totalpage = 1;
            //    for (int i = 1; i < totalpage + 1; i++)
            //    {
            //        string url = $"http://quote.tool.hexun.com/hqzx/quote.aspx?type=2&market={j}&sorttype=3&updown=up&page={i}&count=50&time=080540";
            //        var page = HtmlAnalysis.Gethtmlcode(url);
            //        if (i == 1)
            //            totalpage = RegGroupsX<int>(page, "dataArr,(?<x>\\d+),");
            //        page = page.Replace("\r\n", "");
            //        page = RegGroupsX<string>(page, "dataArr =(?<x>.*?);StockListPage");

            //        if (string.IsNullOrEmpty(page))
            //            return;
            //        GetStockInfo(page);
            //    }
            //}
        }



        public void GetStockInfo(string page)
        {
            var list = JArray.Parse(page);
            if (list == null)
                return;
            var stocks = new List<StockInfo>();
            foreach (var token in list)
            {
                try
                {
                    var item = new StockInfo
                    {
                        StockNo = token[0].Value<string>(),
                        StockName = token[1].Value<string>(),
                        CurrentPrice = token[2].Value<decimal>(),
                        Amplitude = token[3].Value<float>(),
                        Oldprice = token[4].Value<decimal>(),
                        Startprice = token[5].Value<decimal>(),
                        Maxprice = token[6].Value<decimal>(),
                        Minprice = token[7].Value<decimal>(),
                        Volume = token[8].Value<int>(),
                        Turnover = token[9].Value<int>(),
                        Huanshou = token[10].Value<decimal>(),
                        Zhenfu = token[11].Value<decimal>(),
                        Liangbi = token[12].Value<decimal>(),
                    };

                    if(oldlist.Any(c => c.StockNo == item.StockNo))
                        continue;
                    stocks.Add(item);
                    oldlist.Add(item);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }


            }

            if (stocks.Count > 0)
            {
                new StockinfoDB().AddStockinfo(stocks);
            }
        }

        public void GetStockExcel(string fileinfo)
        {
            var page = DocumentServer.GetExcelToJson(fileinfo);
            var list = JArray.Parse(page);
            if (list == null)
                return;
            var stocks = new List<StockInfo>();
            foreach (var token in list)
            {
                try
                {
                    var name = token["F1"].Value<string>();
                    if (name.Contains("更新时间")|| name.Contains("代码"))
                        continue;
                    var item = new StockInfo
                    {
                        StockNo = token["F1"].Value<string>(),
                        StockName = token["F2"].Value<string>(),
                        CurrentPrice = token["F3"].Value<decimal>(),
                        Amplitude =float.Parse( token["F4"].Value<string>().Replace("%","")),
                        Oldprice = token["F11"].Value<decimal>(),
                        Startprice = token["F10"].Value<decimal>(),
                        Volume = token["F8"].Value<int>(),
                        Turnover =0,
                        Huanshou = 0,
                        Zhenfu = 0,
                        Liangbi = 0,
                        Maxprice = token["F12"].Value<decimal>(),
                        Minprice = token["F13"].Value<decimal>()
                    };
                    if (oldlist.Any(c => c.StockNo == item.StockNo))
                        continue;
                    stocks.Add(item);
                    oldlist.Add(item);
                    if (stocks.Count > 300)
                    {
                        new StockinfoDB().AddStockinfo(stocks);
                        stocks.Clear();
                    }

                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            
            }
            if (stocks.Count > 0)
            {
                new StockinfoDB().AddStockinfo(stocks);
            }
        }

        public List<StockInfo> GetAllinfo()
        {
            return new StockinfoDB().GetAllinfo();
        }

        //public void GetStockDetial(StockInfo info)
        //{
        //    var cnum = RegGroupsX<string>(info.StockNo, "(?<x>[A-Za-z]{2})");
        //    var num = RegGroupsX<string>(info.StockNo, "(?<x>\\d+)");
        //    var durl = $"http://chart.windin.com/hqserver/HQProxyHandler.ashx?windcode={num}.{cnum.ToUpper()}";
        //    var mainurl = $"http://www.windin.com/home/stock/html/{num}.{cnum.ToUpper()}.shtml?&t=1&q={num}";
        //    //var hq = $"http://hq.sinajs.cn/rn=&list={info.StockNo},{info.StockNo}_i,bk_new_dzxx";
        //    //var hqpage = HtmlAnalysis.Gethtmlcode(hq);
        //    var dpage = HtmlAnalysis.Gethtmlcode(durl);
        //    var mpage = HtmlAnalysis.Gethtmlcode(mainurl);
        
        //    string hy = (RegGroupsX<string>(mpage, "相关行业板块</td><td>(?<x>.*?)</td>") ?? "").Replace("\r", "");
        //    string dq = (RegGroupsX<string>(mpage, "相关地域板块</td><td>(?<x>.*?)</td>") ?? "").Replace("\r", "");
        //    string ix = (RegGroupsX<string>(mpage, "所属指数成分</td><td>(?<x>.*?)</td>") ?? "").Replace("\r", "");

        //    var dconn = RegGroupsX<string>(dpage, "(?<x>\\{.*?\\})");
        //    if (string.IsNullOrEmpty(dconn))
        //        return;
        //    var winItem = JObject.Parse(dconn);

        //    try
        //    {
        //        StockDayReport sdr = new StockDayReport
        //        {
        //            StockNo = num,
        //            StockName = info.StockName,
        //            Amount = winItem["Amount"].Value<float>(),
        //            CurrentPrice = winItem["Price"].Value<decimal>(),
        //            MarketValue = winItem["MarketValue"].Value<float>(),
        //            Maxprice = winItem["High"].Value<decimal>(),
        //            Minprice = winItem["Low"].Value<decimal>(),
        //            Volume = winItem["Volumn"].Value<int>(),
        //            Exchange = winItem["Exchange"].Value<float>(),
        //            Pe = winItem["PE"].Value<decimal>(),
        //            Pb = winItem["PB"].Value<decimal>(),
        //            Range = winItem["Range"].Value<float>(),
        //            RangeRatio = winItem["RangeRatio"].Value<float>(),
        //            ZhenFu = winItem["ZhenFu"].Value<decimal>(),
        //            OpenPrice = winItem["Open"].Value<decimal>(),
        //            Zdf5 = winItem["zdf5"].Value<float>(),
        //            Zdf10 = winItem["zdf10"].Value<float>(),
        //            Zdf20 = winItem["zdf20"].Value<float>(),
        //            Zdf60 = winItem["zdf60"].Value<float>(),
        //            Zdf120 = winItem["zdf120"].Value<float>(),
        //            Zdf250 = winItem["zdf250"].Value<float>(),
        //            PElyr = winItem["PElyr"].Value<decimal>(),
        //            IndexNumber = ix,
        //            Industry = hy,
        //            Area = dq,
        //            IsStop = winItem["IsStop"].Value<bool>(),
        //            CreateDate = DateTime.Now.Date

        //        };

        //        new StockDayReportDb().AddStockinfo(sdr);

        //    }

        //    catch (Exception ex)
        //    {
        //        LogServer.WriteLog(ex);
                
        //    }
        //}

        //public void GetThsStockDetial(StockInfo info)
        //{
        //    //var durl = $"http://chart.windin.com/hqserver/HQProxyHandler.ashx?windcode={num}.{cnum.ToUpper()}";
        //    var number = RegGroupsX<int>(info.StockNo, "(?<x>\\d+)");
        //    if (number < 1)
        //    {
        //        LogServer.WriteLog("error number"+ info.StockNo, "StockDetial");
        //        return;
        //    }
        //    var url = $"http://stockpage.10jqka.com.cn/spService/{number}/Header/realHeader";
        //    var currentInfo = HtmlAnalysis.Gethtmlcode(url);
        //    if (string.IsNullOrEmpty(currentInfo))
        //    {
        //        LogServer.WriteLog($"error detial {info.StockNo} url:{url}" , "StockDetial");
        //        return;
        //    }
        //    var Item = JObject.Parse(currentInfo);
        //    if (Item == null)
        //    {
        //        LogServer.WriteLog($"error json {info.StockNo} url:{url},detial:{currentInfo}", "StockDetial");
        //    }

        //    var pageingo = HtmlAnalysis.Gethtmlcode("$http://stockpage.10jqka.com.cn/{number}/");
      
        //}

        public void GetXueqiuStockDetial(StockInfo info)
        {
            var code = info.StockNo.ToUpper();
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
                        continue;
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
     

            JObject Item=new JObject();
            try
            {
                Item = JObject.Parse(currentInfo);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog("url="+url+"page="+ currentInfo + ex.Message,"StockDetial");
                Item = null;
            }
          
            if (Item == null)
            {
                LogServer.WriteLog($"error json {info.StockNo} url:{url},detial:{currentInfo}", "StockDetial");
            }
            var xqItem = Item[code];
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
                xq.MarketCapital = xqItem["marketCapital"].Value<float>();
                xq.Eps = xqItem["eps"].Value<float>();
                if (xqItem["pe_ttm"].ToString() != "")
                    xq.Pettm = xqItem["pe_ttm"].Value<float>();
                if (xqItem["pe_lyr"].ToString() != "")
                    xq.PElyr = xqItem["pe_lyr"].Value<float>();
                if (xqItem["beta"].ToString() != "")
                    xq.Beta = xqItem["beta"].Value<float>();
                xq.TotalShares = xqItem["totalShares"] != null ? xqItem["totalShares"].Value<float>() : 0;
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
                        float tempft = 0;
                        float.TryParse(tempat, out tempft);
                        xq.Amount = tempft;
                    }

                }
                xq.NetAssets = xqItem["net_assets"].Value<float>();
                xq.Hasexist = xqItem["hasexist"].Value<string>();
                xq.HasWarrant = xqItem["has_warrant"].Value<string>();
                xq.Type = xqItem["type"].Value<float>();
                xq.Flag = xqItem["flag"]?.Value<int>() ?? 0; //xqItem["flag"].Value<int>();
                xq.RestDay = xqItem["rest_day"].Value<string>();
                xq.Amplitude = xqItem["amplitude"]?.Value<float>() ?? 0; // xqItem["amplitude"].Value<float>();
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

        //public void GetStockDetial()
        //{
        //    LogServer.WriteLog("dayRepord start..." + "stock");
        //    var skdb = new StockinfoDB();
        //    var allskif = skdb.GetAllinfo();
        //    List<StockDayReport> list = new List<StockDayReport>();
        //    foreach (var item in allskif)
        //    {
        //        var cnum = RegGroupsX<string>(item.StockNo, "(?<x>[A-Za-z]{2})");
        //        var num = RegGroupsX<string>(item.StockNo, "(?<x>\\d+)");
        //        var durl = $"http://chart.windin.com/hqserver/HQProxyHandler.ashx?windcode={num}.{cnum.ToUpper()}";
        //        var mainurl = $"http://www.windin.com/home/stock/html/{num}.{cnum.ToUpper()}.shtml?&t=1&q={num}";
        //        var dpage = HtmlAnalysis.Gethtmlcode(durl);
        //        var mpage = HtmlAnalysis.Gethtmlcode(mainurl);
        //        string hy = (RegGroupsX<string>(mpage, "相关行业板块</td><td>(?<x>.*?)</td>") ?? "").Replace("\r", "");
        //        string dq = (RegGroupsX<string>(mpage, "相关地域板块</td><td>(?<x>.*?)</td>") ?? "").Replace("\r", "");
        //        string ix = (RegGroupsX<string>(mpage, "所属指数成分</td><td>(?<x>.*?)</td>") ?? "").Replace("\r", "");
                
        //        var dconn = RegGroupsX<string>(dpage, "(?<x>\\{.*?\\})");
        //        if(string.IsNullOrEmpty(dconn))
        //            continue;
        //        var winItem = JObject.Parse(dconn);
             
        //        try
        //        {
        //            StockDayReport sdr = new StockDayReport
        //            {
        //                StockNo = num,
        //                StockName = item.StockName,
        //                Amount = winItem["Amount"].Value<float>(),
        //                CurrentPrice = winItem["Price"].Value<decimal>(),
        //                MarketValue = winItem["MarketValue"].Value<float>(),
        //                Maxprice = winItem["High"].Value<decimal>(),
        //                Minprice = winItem["Low"].Value<decimal>(),
        //                Volume = winItem["Volumn"].Value<int>(),
        //                Exchange = winItem["Exchange"].Value<float>(),
        //                Pe = winItem["PE"].Value<decimal>(),
        //                Pb = winItem["PB"].Value<decimal>(),
        //                Range = winItem["Range"].Value<float>(),
        //                RangeRatio = winItem["RangeRatio"].Value<float>(),
        //                ZhenFu = winItem["ZhenFu"].Value<decimal>(),
        //                OpenPrice = winItem["Open"].Value<decimal>(),
        //                Zdf5 = winItem["zdf5"].Value<float>(),
        //                Zdf10 = winItem["zdf10"].Value<float>(),
        //                Zdf20 = winItem["zdf20"].Value<float>(),
        //                Zdf60 = winItem["zdf60"].Value<float>(),
        //                Zdf120 = winItem["zdf120"].Value<float>(),
        //                Zdf250 = winItem["zdf250"].Value<float>(),
        //                PElyr = winItem["PElyr"].Value<decimal>(),
        //                IndexNumber = ix,
        //                Industry = hy,
        //                Area = dq,
        //                IsStop = winItem["IsStop"].Value<bool>(),
        //                CreateDate = DateTime.Now.Date
                        
        //            };
        //            list.Add(sdr);
        //            if (list.Count == 200)
        //            {
        //                new StockDayReportDb().AddStockinfo(list);
        //                list.Clear();
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            LogServer.WriteLog(ex);
        //            continue;
        //        }
        //    }

        //    if (list.Count >0)
        //    {
        //        new StockDayReportDb().AddStockinfo(list);
        //        list.Clear();
        //    }
        //    LogServer.WriteLog("dayRepord finshed..."+"stock");

        //}


        public void GetNewStockInfo()
        {

            int totalpage = 2;
            for (int i = 1; i <= totalpage; i++)
            {
                string url = $"https://xueqiu.com/proipo/query.json?page={i}&size=30&order=desc&orderBy=list_date&stockType=&column=symbol%2Cname%2Conl_subcode%2Clist_date%2Cactissqty%2Conl_actissqty%2Conl_submaxqty%2Conl_subbegdate%2Conl_unfrozendate%2Conl_refunddate%2Ciss_price%2Conl_frozenamt%2Conl_lotwinrt%2Conl_lorwincode%2Conl_lotwiner_stpub_date%2Conl_effsubqty%2Conl_effsubnum%2Conl_onversubrt%2Coffl_lotwinrt%2Coffl_effsubqty%2Coffl_planum%2Coffl_oversubrt%2Cnapsaft%2Ceps_dilutedaft%2Cleaduwer%2Clist_recomer%2Cacttotraiseamt%2Conl_rdshowweb%2Conl_rdshowbegdate%2Conl_distrdate%2Conl_drawlotsdate%2Cfirst_open_price%2Cfirst_close_price%2Cfirst_percent%2Cfirst_turnrate%2Cstock_income%2Conl_lotwin_amount%2Clisted_percent%2Ccurrent%2Cpe_ttm%2Cpb%2Cpercent%2Chasexist&type=quote&_=1471824353300";

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

                var oldlist = GetAllinfo();
                if(oldlist==null||oldlist.Count==0)
                    return;
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
                    stocks.Add(newski);

                }
                if (stocks.Count > 0)
                    new StockinfoDB().AddStockinfo(stocks);
            }
          
        }

        //v_sh600886="1~国投电力~600886~7.01~7.05~7.03~418294~203031~215263~7.01~1148~7.00~13534~6.99~1901~6.98~2972~6.97~1799~7.02~1700~7.03~1423~7.04~13370~7.05~2649~7.06~4457~14:59:56/7.02/230/B/161440/11919|14:59:56/7.01/120/S/84120/11916|14:59:51/7.01/53/S/37203/11911|14:59:46/7.01/96/S/67296/11908|14:59:41/7.01/11/S/7711/11905|14:59:31/7.01/69/S/48369/11901~20160811150546~-0.04~-0.57~7.08~6.99~7.02/418294成交量/294236426成交额~418294~29424成交额~0.62（换手率）~8.74（市盈率）~~7.08最高价~6.99最低价~1.28震幅~475.70流通市值 ~475.70总市值  ~1.71（市净率）~7.76涨停价 ~6.35跌停价  ~";
        //v_sh600886="1~国投电力~600886~当前价~昨收~今开~成交量~盘外~盘内~买一价~买一量 卖一 价量6.99~1901~6.98~2972~6.97~1799~7.02~1700~7.03~1423~7.04~13370~7.05~2649~7.06~4457~逐笔（时间价格成交量买入）40/11919|14:59:56/7.01/120/S/84120/11916|14:59:51/7.01/53/S/37203/11911|14:59:46/7.01/96/S/67296/11908|14:59:41/7.01/11/S/7711/11905|14:59:31/7.01/69/S/48369/11901~日期~涨价额~涨跌幅~最高~最低
        //var testpage = HtmlAnalysis.Gethtmlcode("http://web.sqt.gtimg.cn/q=sh600886?r=0.2674542577107566");

        //var pageinfo1 = HtmlAnalysis.Gethtmlcode("http://d.10jqka.com.cn/v2/time/hs_600662/last.js");

        public void stockMonitor()
        {
            var head = HtmlAnalysis.Gethtmlcode("http://stockpage.10jqka.com.cn/spService/002801/Header/realHeader");
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
