using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BLL.Sprider.Stock;
using Commons;
using Mode;
using Newtonsoft.Json.Linq;

namespace Servers
{
    public class BondSpiderServer
    {
        private const string SingleUrl = "http://bond.money.hexun.com/all_bond/{0}.shtml";

        private const string SingleDetial = "http://hq.sinajs.cn/rn=g1yon&list=sh010107";
        private BondInfo AddBondInfo(string num)
        {
            var defaultTime = DateTime.Parse("1900-01-01");
            string url = string.Format(SingleUrl, num);
            var page = HtmlAnalysis.Gethtmlcode(url);
            var conntent = Regex.Match(page, "<td bgColor=\"#cccccc\">(?<x>.*?)</table>").Groups["x"].Value;
            if (string.IsNullOrEmpty(conntent))
                return null;
            var list = Regex.Matches(conntent, "<td.*?>(?<x>.*?)</td>");
            if (list.Count < 58)
                return null;
            try
            {
                var bond = new BondInfo();
                bond.Companyname = list[3].Groups["x"].Value;
                bond.BondNo = list[5].Groups["x"].Value.Trim();
                bond.ShortName = list[7].Groups["x"].Value.Trim();
                DateTime temprTime;
                bond.ReleaseDate = DateTime.TryParse(list[9].Groups["x"].Value, out temprTime)
                    ? temprTime
                    : defaultTime;
                bond.MarketDate = DateTime.TryParse(list[11].Groups["x"].Value, out temprTime)
                    ? temprTime
                    : defaultTime;

                string iamount = list[13].Groups["x"].Value.Trim();
                float tempIssuedAmount;
                float.TryParse(iamount, out tempIssuedAmount);
                bond.IssuedAmount = tempIssuedAmount;
                bond.Denomination = list[15].Groups["x"].Value.Trim();
                bond.IssuedPrice = list[17].Groups["x"].Value.Trim();
                bond.Deadline = list[19].Groups["x"].Value.Trim();
                float temprate;
                string strtemprate = list[21].Groups["x"].Value.Trim();
                float.TryParse(strtemprate, out temprate);
                bond.AnnualInterestRate = temprate;
                strtemprate = list[23].Groups["x"].Value.Trim();
                float.TryParse(strtemprate, out temprate);
                bond.AnnualInterestRateNow = temprate;
                string jixiDay = list[25].Groups["x"].Value.Trim();
                if (!string.IsNullOrEmpty(jixiDay) && jixiDay.IndexOf('.') > 0)
                {
                    jixiDay = bond.MarketDate.ToString("yyyy-").Trim() + jixiDay.Replace('.', '-');
                    bond.JixiDay = DateTime.TryParse(jixiDay, out temprTime) ? temprTime : defaultTime;
  
                }
                else
                {
                    bond.JixiDay = defaultTime;
                }
                bond.ExpirationTime = DateTime.TryParse(list[27].Groups["x"].Value, out temprTime)
                    ? temprTime
                    : defaultTime;
                strtemprate = list[29].Groups["x"].Value.Trim();
                float.TryParse(strtemprate, out temprate);
                bond.RedemptionPrice = temprate;
                bond.IssuedBeginDate = DateTime.TryParse(list[31].Groups["x"].Value.Trim(), out temprTime)
                    ? temprTime
                    : defaultTime;

                bond.IssuedEndDate = DateTime.TryParse(list[33].Groups["x"].Value.Trim(), out temprTime)
                    ? temprTime
                    : defaultTime;

                bond.SubscriptionObject = list[35].Groups["x"].Value.Trim();
                float.TryParse(list[37].Groups["x"].Value.Trim(), out temprate);
                bond.Bondvalue = temprate;
                bond.TaxStatus = list[39].Groups["x"].Value.Trim();
                int tempgread;
                int.TryParse(list[41].Groups["x"].Value.Trim(), out tempgread);
                bond.CreditRating = tempgread;
                bond.Issuer = list[43].Groups["x"].Value.Trim();
                bond.InterestType = list[45].Groups["x"].Value.Trim();
                bond.Guarantor = list[47].Groups["x"].Value.Trim();
                bond.IssuedType = list[49].Groups["x"].Value.Trim();
                bond.IssuedObject = list[51].Groups["x"].Value.Trim();
                bond.UnderwritingAgency = list[53].Groups["x"].Value.Trim();
                bond.Bondtype = list[55].Groups["x"].Value.Trim();
                bond.Remork = list[57].Groups["x"].Value.Trim();
                bond.CreateDate = DateTime.Now;
                bond.UpdateTime = DateTime.Now;
                return bond;
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "BondSpider");
                return null;
            }
        }

        public void UpdateallBond()
        {
            var list = new List<BondInfo>();
            var bll = new BondInfoBll();
            var bonds = bll.GetAllBondinfo();


             string allurl = "http://bond.money.hexun.com/quote/alltables.htm";
            
            allurl = "http://bond.money.hexun.com/quote/default.aspx?market=2";
            string page = "";

            for (int i = 1; i < 56; i++)
            {
                allurl =string.Format("http://vip.stock.finance.sina.com.cn/quotes_service/api/json_v2.php/Market_Center.getHQNodeDataSimple?page={0}&num=80&sort=symbol&asc=1&node=hs_z&_s_r_a=page",i);
                 page = HtmlAnalysis.Gethtmlcode(allurl);
                if(page=="null")
                    break;
                var codeToken = JArray.Parse(page);
                foreach (JToken token in codeToken)
                {
                    string tempcode = token["code"].Value<string>();
                    if(string.IsNullOrEmpty(tempcode))
                        continue;
                    if (bonds.Any(c => c.BondNo == tempcode))
                        continue;
                    var item = AddBondInfo(tempcode);
                    if (item != null)
                    {
                        list.Add(item);
                        bonds.Add(item);
                    }

                    if (list.Count == 100)
                    {
                        bll.AddBondInfo(list);
                        list.Clear();
                    }
                }
            }





          
            //var codelist = Regex.Matches(page, "all_bond/(?<x>\\d{4,20}).shtml|/corporate_bond/(?<x>\\d{4,20}).shtml|/treasury_bond/(?<x>\\d{4,20}).shtml");
            //if (codelist.Count == 0)
            //    return;
            //foreach (Match match in codelist)
            //{
            //    string tempcode = match.Groups["x"].Value.Trim();
            //    if (bonds.Any(c => c.BondNo == tempcode))
            //        continue;
            //    var item = AddBondInfo(tempcode);
            //    if (item != null)
            //    {
            //        list.Add(item);
            //        bonds.Add(item);
            //    }
                    
            //    if (list.Count == 100)
            //    {
            //        bll.AddBondInfo(list);
            //        list.Clear();
            //    }
            //}
            if (list.Count > 0)
                bll.AddBondInfo(list);

        }

    }
}
