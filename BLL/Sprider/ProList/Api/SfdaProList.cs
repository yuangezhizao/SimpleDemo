using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.ProList.Api
{
    public class SfdaProList : Sfda, IApiProList
    {
        public SfdaProList()
        {
            Baseinfo = new SiteInfoDB().SiteById(999)?? new SiteInfo{SiteId=999};
        }


      

        private void addproducts(IEnumerable<string> ids)
        {
            const string proUrlMode = "http://app1.sfda.gov.cn/datasearch/face3/{0}";
            HtmlAnalysis request = new HtmlAnalysis();
            request.RequestAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.RequestContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.RequestUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0";
            List<MedicineInfo> list = new List<MedicineInfo>();
            foreach (string page in ids.Select(proid => string.Format(proUrlMode, proid)).Select(request.HttpRequest))
            {
                list.Add(getProDetial(page));
                if (list.Count > 49)
                {
                    new MedicineDB().AddMedicineInfo(list);
                    list.Clear();
                }

            }
            if (list.Count > 0)
            {
                new MedicineDB().AddMedicineInfo(list);
                list.Clear();
            }
        }

        public void GetAllSite()
        {
            const string urlmode =
                // "http://app1.sfda.gov.cn/datasearch/face3/base.jsp?tableId=96&tableName=TABLE9&title=%CD%F8%C9%CF%D2%A9%B5%EA&bcId=1394682945092808297939426895866&curstart={0}";
                // "http://app1.sfda.gov.cn/datasearch/face3/base.jsp?tableId=96&tableName=TABLE96&title=%CD%F8%C9%CF%D2%A9%B5%EA&bcId=139468294509280829793942689586&curstart={0}";
                //"http://app1.sfda.gov.cn/datasearch/face3/search.jsp?tableId=96&bcId=139468294509280829793942689586&curstart={0}&tableName=TABLE96&State=1&viewtitleName=COLUMN1229&viewsubTitleName=COLUMN1227&State=1&tableView=%25E7%25BD%2591%25E4%25B8%258A%25E8%258D%25AF%25E5%25BA%2597";
                "http://app1.sfda.gov.cn/datasearch/face3/search.jsp?tableId=96&State=1&bcId=139468294509280829793942689586&State=1&curstart={0}&State=1&tableName=TABLE96&State=1&viewtitleName=COLUMN1229&State=1&viewsubTitleName=COLUMN1227&State=1&tableView=%25E7%25BD%2591%25E4%25B8%258A%25E8%258D%25AF%25E5%25BA%2597&State=1";
            HtmlAnalysis request = new HtmlAnalysis();
            request.RequestMethod = "post";
            request.RequestAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.RequestContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.RequestUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0";
            int page = 1;
            for (int i = 1; i <= page; i++)
            {
                try
                {
                    if (i > 1)
                        request.RequestReferer = string.Format(urlmode, i - 1);
                    string url = string.Format(urlmode, i);
                    string homepage = request.HttpRequest(url);
                    if (homepage == "")
                        continue;
                    if (page == 1)
                    {
                        page = RegGroupsX<int>(homepage, "共(?<x>\\d+)页");
                    }
                    string content = RegGroupsX<string>(homepage, "<td height=30><p align=left>(?<x>.*?)</table>");
                    var list = RegGroupCollection(content, "\\&Id=(?<x>\\d+)");
                    if (list == null)
                    {
                        LogServer.WriteLog("第" + i + "页\t" + url, "sfda");
                        continue;
                    }
                    var ids = from Match item in list select item.Groups["x"].Value;
                    addSigelSite(ids);
                    LogServer.WriteLog("第" + i + "页\t" + url, "sfda");
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            }
        }

        private void addSigelSite(IEnumerable<string> ids)
        {
            const string proUrlMode = "http://app1.sfda.gov.cn/datasearch/face3/content.jsp?tableId=96&tableName=TABLE96&tableView=%CD%F8%C9%CF%D2%A9%B5%EA&Id={0}";
            HtmlAnalysis request = new HtmlAnalysis();
            request.RequestAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.RequestContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.RequestUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0";
            List<MedicineSiteInfo> list = new List<MedicineSiteInfo>();
            foreach (string page in ids.Select(proid => string.Format(proUrlMode, proid)).Select(request.HttpRequest))
            {
                var sintinfo = RegGroupCollection(page, "<tr>(?<x>.*?)</tr>");
                if (sintinfo.Count < 13)
                    continue ;
                try
                {
                    MedicineSiteInfo msite = new MedicineSiteInfo
                    {
                        CertificateNo = RegGroupsX<string>(sintinfo[0].Groups["x"].Value, "width=83%>(?<x>.*?)</td>"),
                        ServerArea = RegGroupsX<string>(sintinfo[1].Groups["x"].Value, "width=83%>(?<x>.*?)</td>"),
                        CompanyName = RegGroupsX<string>(sintinfo[2].Groups["x"].Value, "width=83%>(?<x>.*?)</td>"),
                        Gerent = RegGroupsX<string>(sintinfo[3].Groups["x"].Value, "width=83%>(?<x>.*?)</td>"),
                        CompanyAddress = RegGroupsX<string>(sintinfo[4].Groups["x"].Value, "width=83%>(?<x>.*?)</td>"),
                        Province = RegGroupsX<string>(sintinfo[5].Groups["x"].Value, "width=83%>(?<x>.*?)</td>"),
                        SiteName = RegGroupsX<string>(sintinfo[6].Groups["x"].Value, "width=83%>(?<x>.*?)</td>"),
                        SiteIp = RegGroupsX<string>(sintinfo[7].Groups["x"].Value, "width=83%>(?<x>.*?)</td>"),
                        Domian = RegGroupsX<string>(sintinfo[8].Groups["x"].Value, "width=83%>(?<x>.*?)</td>"),
                        ReleaseTime = RegGroupsX<DateTime>(sintinfo[9].Groups["x"].Value, "width=83%>(?<x>.*?)</td>"),
                        ValidityDate = RegGroupsX<DateTime>(sintinfo[10].Groups["x"].Value, "width=83%>(?<x>.*?)</td>"),
                        PostNo = RegGroupsX<string>(sintinfo[11].Groups["x"].Value, "width=83%>(?<x>.*?)</td>"),
                        Remark = RegGroupsX<string>(sintinfo[13].Groups["x"].Value, "000066\">(?<x>.*?)</span>"),
                        CreateTime =DateTime.Now
                    };
                  
                    msite.Domian = msite.Domian.Replace("；", ";");

                    //if (tempcount == 2)
                    //{
                    //    msite.Domian = "www." + msite.Domian;
                    //}
                    //if (tempcount > 3)
                    //{
                    //    msite.Domian = msite.Domian;
                    //}
                   

                    //string tempurl = $"http://{msite.Domian}";
                    //string temppage= HtmlAnalysis.HttpRequest(tempurl);
                    //msite.Usefull = temppage.Contains("page");
                    list.Add(msite);

                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
              
            }
            if (list.Count > 0)
            {
                new MedicineSiteDB().AddMedicineSite(list);
                list.Clear();
            }
        }

        public void GetAllProducts()
        {
       
          //const string urlmode = "http://app1.sfda.gov.cn/datasearch/face3/search.jsp?tableId=25&State=1&bcId=124356560303886909015737447882&State=1&tableName=TABLE25&State=1&viewtitleName=COLUMN167&State=1&viewsubTitleName=COLUMN166,COLUMN170,COLUMN821&State=1&curstart={0}&State=1&tableView=%25E5%259B%25BD%25E4%25BA%25A7%25E8%258D%25AF%25E5%2593%2581&State=1";
            const string urlmode = "http://app1.sfda.gov.cn/datasearch/face3/search.jsp?tableId=36&State=1&bcId=124356651564146415214424405468&State=1&curstart={0}&State=1&tableName=TABLE36&State=1&viewtitleName=COLUMN361&State=1&viewsubTitleName=COLUMN354,COLUMN355,COLUMN356,COLUMN823&State=1&tableView=%25E8%25BF%259B%25E5%258F%25A3%25E8%258D%25AF%25E5%2593%2581&State=1";
            HtmlAnalysis request = new HtmlAnalysis();
            request.RequestMethod = "post";
            request.RequestAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.RequestContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.RequestUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0";
            int page = 1;
            for (int i = 1; i <= page; i++)
            {
                try
                {
                    string url = string.Format(urlmode, i);
                    string homepage = request.HttpRequest(url);
                    if (homepage == "")
                        continue;
                    if (page == 1)
                    {
                        page = RegGroupsX<int>(homepage, "共(?<x>\\d+)页");
                    }
                    string content = RegGroupsX<string>(homepage, "<td height=30><p align=left>(?<x>.*?)</table>");
                    var list = RegGroupCollection(content, "callbackC,'(?<x>.*?)',");
                    if (list == null)
                    {
                        LogServer.WriteLog("第" + i + "页\t" + url, "sfda");
                        continue;
                    }
                    var ids = from Match item in list select item.Groups["x"].Value;
                    addproducts(ids);
                    LogServer.WriteLog("第" + i + "页\t" + url, "sfda");
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            }
            LogServer.WriteLog("抓取完毕共抓取到 " + page, "sfda");

        }

        private MedicineInfo getProDetial(string page)
        {
            var list = RegGroupCollection(page, "<tr>(?<x>.*?)</tr>");
            if (list.Count<15)
                return null;


            try
            {
                MedicineInfo obj = new MedicineInfo();
                obj.ApprovalNum = RegGroupsX<string>(list[0].Groups["x"].Value, "<td bgcolor=\"#eaeaea\" width=83%>(?<x>.*?)</td>");
                obj.ApprovalNum = obj.ApprovalNum.Replace("国药准字", "");
                obj.ProName = RegGroupsX<string>(list[1].Groups["x"].Value, "width=83%>(?<x>.*?)</td>");
                obj.EnglishName = RegGroupsX<string>(list[2].Groups["x"].Value, "width=83%>(?<x>.*?)</td>");
                obj.Dosageforms = RegGroupsX<string>(list[4].Groups["x"].Value, "width=83%>(?<x>.*?)</td>");
                obj.Specification = RegGroupsX<string>(list[5].Groups["x"].Value, "width=83%>(?<x>.*?)</td>");
                obj.ProductionUnit = RegGroupsX<string>(list[6].Groups["x"].Value, "<a href=\".*?\">(?<x>.*?)</a>");
                obj.ProductionAddress = RegGroupsX<string>(list[7].Groups["x"].Value, "width=83%>(?<x>.*?)</td>");
                obj.Category = RegGroupsX<string>(list[8].Groups["x"].Value, "width=83%>(?<x>.*?)</td>");
                obj.ApprovalDate = RegGroupsX<DateTime>(list[10].Groups["x"].Value, "width=83%>(?<x>.*?)</td>");
                obj.DrugbasedCode = RegGroupsX<string>(list[11].Groups["x"].Value, "width=83%>(?<x>.*?)</td>");

                obj.DrugbasedBack = RegGroupsX<string>(list[12].Groups["x"].Value, "width=83%>(?<x>.*?)</td>");
                obj.Remark =WordCenter.FilterHtml(RegGroupsX<string>(list[15].Groups["x"].Value, "<td bgcolor=\"#eaeaea\">(?<x>.*?)</td>"));

                return obj;
            }
            catch (Exception)
            {
                return null;
            }



        }




        public void AddNewProducts()
        {
          //  GetAllSite();
            GetAllProducts();
        }
    }
}
