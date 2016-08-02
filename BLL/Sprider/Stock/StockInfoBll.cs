
using System;
using System.Collections.Generic;
using System.Linq;
using Commons;
using DataBase.Stock;
using Mode;
using NetDimension.Json.Linq;
using SpriderProxy.Analysis;

namespace BLL.Sprider.Stock
{
    public class StockInfoBll: HeXun, IStockInfoBll
    {
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
            return;
            for (int j = 0; j < 3; j++)
            {
                int totalpage = 1;
                for (int i = 1; i < totalpage + 1; i++)
                {
                    string url = $"http://quote.tool.hexun.com/hqzx/quote.aspx?type=2&market={j}&sorttype=3&updown=up&page={i}&count=50&time=080540";
                    var page = HtmlAnalysis.Gethtmlcode(url);
                    if (i == 1)
                        totalpage = RegGroupsX<int>(page, "dataArr,(?<x>\\d+),");
                    page = page.Replace("\r\n", "");
                    page = RegGroupsX<string>(page, "dataArr =(?<x>.*?);StockListPage");

                    if (string.IsNullOrEmpty(page))
                        return;
                    GetStockInfo(page);
                }
            }
        }

        public void DayReport()
        {
            GetStockDetial();
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

        public void GetStockDetial(StockInfo info)
        {
            var cnum = RegGroupsX<string>(info.StockNo, "(?<x>[A-Za-z]{2})");
            var num = RegGroupsX<string>(info.StockNo, "(?<x>\\d+)");
            var durl = $"http://chart.windin.com/hqserver/HQProxyHandler.ashx?windcode={num}.{cnum.ToUpper()}";
            var mainurl = $"http://www.windin.com/home/stock/html/{num}.{cnum.ToUpper()}.shtml?&t=1&q={num}";
            //var hq = $"http://hq.sinajs.cn/rn=&list={info.StockNo},{info.StockNo}_i,bk_new_dzxx";
            //var hqpage = HtmlAnalysis.Gethtmlcode(hq);
            var dpage = HtmlAnalysis.Gethtmlcode(durl);
            var mpage = HtmlAnalysis.Gethtmlcode(mainurl);
        
            string hy = (RegGroupsX<string>(mpage, "相关行业板块</td><td>(?<x>.*?)</td>") ?? "").Replace("\r", "");
            string dq = (RegGroupsX<string>(mpage, "相关地域板块</td><td>(?<x>.*?)</td>") ?? "").Replace("\r", "");
            string ix = (RegGroupsX<string>(mpage, "所属指数成分</td><td>(?<x>.*?)</td>") ?? "").Replace("\r", "");

            var dconn = RegGroupsX<string>(dpage, "(?<x>\\{.*?\\})");
            if (string.IsNullOrEmpty(dconn))
                return;
            var winItem = JObject.Parse(dconn);

            try
            {
                StockDayReport sdr = new StockDayReport
                {
                    StockNo = num,
                    StockName = info.StockName,
                    Amount = winItem["Amount"].Value<float>(),
                    CurrentPrice = winItem["Price"].Value<decimal>(),
                    MarketValue = winItem["MarketValue"].Value<float>(),
                    Maxprice = winItem["High"].Value<decimal>(),
                    Minprice = winItem["Low"].Value<decimal>(),
                    Volume = winItem["Volumn"].Value<int>(),
                    Exchange = winItem["Exchange"].Value<float>(),
                    Pe = winItem["PE"].Value<decimal>(),
                    Pb = winItem["PB"].Value<decimal>(),
                    Range = winItem["Range"].Value<float>(),
                    RangeRatio = winItem["RangeRatio"].Value<float>(),
                    ZhenFu = winItem["ZhenFu"].Value<decimal>(),
                    OpenPrice = winItem["Open"].Value<decimal>(),
                    Zdf5 = winItem["zdf5"].Value<float>(),
                    Zdf10 = winItem["zdf10"].Value<float>(),
                    Zdf20 = winItem["zdf20"].Value<float>(),
                    Zdf60 = winItem["zdf60"].Value<float>(),
                    Zdf120 = winItem["zdf120"].Value<float>(),
                    Zdf250 = winItem["zdf250"].Value<float>(),
                    PElyr = winItem["PElyr"].Value<decimal>(),
                    IndexNumber = ix,
                    Industry = hy,
                    Area = dq,
                    IsStop = winItem["IsStop"].Value<bool>(),
                    CreateDate = DateTime.Now.Date

                };

                new StockDayReportDb().AddStockinfo(sdr);

            }

            catch (Exception ex)
            {
                LogServer.WriteLog(ex);
                
            }
        }

        public void GetThsStockDetial(StockInfo info)
        {
            //var durl = $"http://chart.windin.com/hqserver/HQProxyHandler.ashx?windcode={num}.{cnum.ToUpper()}";
            var number = RegGroupsX<int>(info.StockNo, "(?<x>\\d+)");
            if (number < 1)
            {
                LogServer.WriteLog("error number"+ info.StockNo, "StockDetial");
                return;
            }
            var url = $"http://stockpage.10jqka.com.cn/spService/{number}/Header/realHeader";
            var currentInfo = HtmlAnalysis.Gethtmlcode(url);
            if (string.IsNullOrEmpty(currentInfo))
            {
                LogServer.WriteLog($"error detial {info.StockNo} url:{url}" , "StockDetial");
                return;
            }
            var Item = JObject.Parse(currentInfo);
            if (Item == null)
            {
                LogServer.WriteLog($"error json {info.StockNo} url:{url},detial:{currentInfo}", "StockDetial");
            }

            var pageingo = HtmlAnalysis.Gethtmlcode("$http://stockpage.10jqka.com.cn/{number}/");
      
        }

        public void GetXueqiuStockDetial(StockInfo info)
        {
            var code = info.StockNo.ToUpper();
            string url = $"https://xueqiu.com/v4/stock/quote.json?code={code}&_=1465259721266";

            var cookies = new SiteCookiesBll().GetOneByDomain("xueqiu.com");
            HtmlAnalysis request = new HtmlAnalysis();
            request.RequestAccept = "application/json, text/javascript, */*; q=0.01";
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            if (cookies != null)
                request.Headers.Add("Cookie", cookies.Cookies);
            request.RequestUserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
         

            var currentInfo = request.HttpRequest(url);
            var Item = JObject.Parse(currentInfo);
            if (Item == null)
            {
                LogServer.WriteLog($"error json {info.StockNo} url:{url},detial:{currentInfo}", "StockDetial");
            }
            var xqItem = Item[code];
            try
            {
                XqStockDayReport xq = new XqStockDayReport();
                xq.StockNo = code;
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
                xq.Beta = xqItem["beta"].Value<float>();
                xq.TotalShares = xqItem["totalShares"].Value<float>();
                xq.AfterHours = xqItem["afterHours"].Value<float>();
                xq.AfterHoursPct = xqItem["afterHoursPct"].Value<float>();
                xq.AfterHoursChg = xqItem["afterHoursChg"].Value<float>();
                xq.UpdateAt = xqItem["updateAt"].Value<float>();
                if (xqItem["dividend"].ToString() != "")
                    xq.Dividend = xqItem["dividend"].Value<float>();
                xq.Yield = xqItem["yield"].Value<float>();
                xq.Turnoverrate = xqItem["turnover_rate"].Value<float>();
                xq.InstOwn = xqItem["instOwn"].Value<float>();
                xq.Risestop = xqItem["rise_stop"].Value<float>();
                xq.FallStop = xqItem["fall_stop"].Value<float>();
                xq.CurrencyUnit = xqItem["currency_unit"].Value<string>();
                xq.Amount = xqItem["amount"].Value<string>();
                xq.NetAssets = xqItem["net_assets"].Value<float>();
                xq.Hasexist = xqItem["hasexist"].Value<string>();
                xq.HasWarrant = xqItem["has_warrant"].Value<string>();
                xq.Type = xqItem["type"].Value<float>();
                xq.Flag = xqItem["flag"].Value<int>();
                xq.RestDay = xqItem["rest_day"].Value<string>();
                xq.Amplitude = xqItem["amplitude"].Value<float>();
                xq.LotSize = xqItem["lot_size"].Value<float>();
                xq.MinOrderQuantity = xqItem["min_order_quantity"].Value<float>();
                xq.MaxOrderQuantity = xqItem["max_order_quantity"].Value<float>();
                xq.TickSize = xqItem["tick_size"].Value<float>();
                xq.KzzStockSymbol = xqItem["kzz_stock_symbol"].Value<string>();
                xq.KzzStockName = xqItem["kzz_stock_name"].Value<string>();
                xq.KzzStockCurrent = xqItem["kzz_stock_current"].Value<float>();
                xq.KzzConvertPrice = xqItem["kzz_convert_price"].Value<float>();
                xq.kzzcovertValue = xqItem["kzz_covert_value"].Value<float>();
                xq.KzzCpr = xqItem["kzz_cpr"].Value<float>();
                xq.KzzPutbackPrice = xqItem["kzz_putback_price"].Value<float>();
                xq.KzzConvertTime = xqItem["kzz_convert_time"].Value<string>();
                xq.KzzRedemptPrice = xqItem["kzz_redempt_price"].Value<float>();
                xq.KzzStraightPrice = xqItem["kzz_straight_price"].Value<float>();
                xq.KzzStockPercent = xqItem["kzz_stock_percent"].Value<string>();
                xq.Pb = xqItem["pb"].Value<float>();
                xq.BenefitBeforeTax = xqItem["benefit_before_tax"].Value<float>();
                xq.BenefitAfterTax = xqItem["benefit_after_tax"].Value<float>();
                xq.ConvertBondRatio = xqItem["convert_bond_ratio"].Value<string>();
                xq.Totalissuescale = xqItem["totalissuescale"].Value<string>();
                xq.Outstandingamt = xqItem["outstandingamt"].Value<string>();
                xq.Maturitydate = xqItem["maturitydate"].Value<string>();
                xq.RemainYear = xqItem["remain_year"].Value<string>();
                xq.ConvertRate = xqItem["convertrate"].Value<float>();
                xq.Interestrtmemo = xqItem["interestrtmemo"].Value<string>();
                xq.ReleaseDate = xqItem["release_date"].Value<string>();
                xq.Circulation = xqItem["circulation"].Value<float>();
                xq.ParValue = xqItem["par_value"].Value<float>();
                xq.DueTime = xqItem["due_time"].Value<float>();
                xq.ValueDate = xqItem["value_date"].Value<string>();
                xq.DueDate = xqItem["due_date"].Value<string>();
                xq.Publisher = xqItem["publisher"].Value<string>();
                xq.RedeemType = xqItem["redeem_type"].Value<string>();
                xq.BondType = xqItem["bond_type"].Value<string>();
                xq.Warrant = xqItem["warrant"].Value<string>();
                xq.SaleRrg = xqItem["sale_rrg"].Value<string>();
                xq.Rate = xqItem["rate"].Value<string>();
                xq.AfterHourVol = xqItem["after_hour_vol"].Value<int>();
                xq.FloatShares = xqItem["float_shares"].Value<float>();
                xq.FloatMarketCapital = xqItem["float_market_capital"].Value<string>();
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
        public void GetStockDetial()
        {
            LogServer.WriteLog("dayRepord start..." + "stock");
            var skdb = new StockinfoDB();
            var allskif = skdb.GetAllinfo();
            List<StockDayReport> list = new List<StockDayReport>();
            foreach (var item in allskif)
            {
                var cnum = RegGroupsX<string>(item.StockNo, "(?<x>[A-Za-z]{2})");
                var num = RegGroupsX<string>(item.StockNo, "(?<x>\\d+)");
                var durl = $"http://chart.windin.com/hqserver/HQProxyHandler.ashx?windcode={num}.{cnum.ToUpper()}";
                var mainurl = $"http://www.windin.com/home/stock/html/{num}.{cnum.ToUpper()}.shtml?&t=1&q={num}";
                var dpage = HtmlAnalysis.Gethtmlcode(durl);
                var mpage = HtmlAnalysis.Gethtmlcode(mainurl);
                string hy = (RegGroupsX<string>(mpage, "相关行业板块</td><td>(?<x>.*?)</td>") ?? "").Replace("\r", "");
                string dq = (RegGroupsX<string>(mpage, "相关地域板块</td><td>(?<x>.*?)</td>") ?? "").Replace("\r", "");
                string ix = (RegGroupsX<string>(mpage, "所属指数成分</td><td>(?<x>.*?)</td>") ?? "").Replace("\r", "");
                
                var dconn = RegGroupsX<string>(dpage, "(?<x>\\{.*?\\})");
                if(string.IsNullOrEmpty(dconn))
                    continue;
                var winItem = JObject.Parse(dconn);
             
                try
                {
                    StockDayReport sdr = new StockDayReport
                    {
                        StockNo = num,
                        StockName = item.StockName,
                        Amount = winItem["Amount"].Value<float>(),
                        CurrentPrice = winItem["Price"].Value<decimal>(),
                        MarketValue = winItem["MarketValue"].Value<float>(),
                        Maxprice = winItem["High"].Value<decimal>(),
                        Minprice = winItem["Low"].Value<decimal>(),
                        Volume = winItem["Volumn"].Value<int>(),
                        Exchange = winItem["Exchange"].Value<float>(),
                        Pe = winItem["PE"].Value<decimal>(),
                        Pb = winItem["PB"].Value<decimal>(),
                        Range = winItem["Range"].Value<float>(),
                        RangeRatio = winItem["RangeRatio"].Value<float>(),
                        ZhenFu = winItem["ZhenFu"].Value<decimal>(),
                        OpenPrice = winItem["Open"].Value<decimal>(),
                        Zdf5 = winItem["zdf5"].Value<float>(),
                        Zdf10 = winItem["zdf10"].Value<float>(),
                        Zdf20 = winItem["zdf20"].Value<float>(),
                        Zdf60 = winItem["zdf60"].Value<float>(),
                        Zdf120 = winItem["zdf120"].Value<float>(),
                        Zdf250 = winItem["zdf250"].Value<float>(),
                        PElyr = winItem["PElyr"].Value<decimal>(),
                        IndexNumber = ix,
                        Industry = hy,
                        Area = dq,
                        IsStop = winItem["IsStop"].Value<bool>(),
                        CreateDate = DateTime.Now.Date
                        
                    };
                    list.Add(sdr);
                    if (list.Count == 200)
                    {
                        new StockDayReportDb().AddStockinfo(list);
                        list.Clear();
                    }

                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                    continue;
                }
            }

            if (list.Count >0)
            {
                new StockDayReportDb().AddStockinfo(list);
                list.Clear();
            }
            LogServer.WriteLog("dayRepord finshed..."+"stock");

        }
    }
}
