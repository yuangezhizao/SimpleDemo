
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
            GetStockDetial();
            return;
            oldlist = new StockinfoDB().GetAllinfo() ?? new List<StockInfo>();

            var files = DocumentServer.GetAllFileNameByPath(@"F:\d");

            foreach (var file in files)
            {
                GetStockExcel(@"F:\d\" + file);
            }

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
