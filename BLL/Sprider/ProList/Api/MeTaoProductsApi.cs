using System;
using System.Collections.Generic;
using Commons;
using DataBase;
using Mode;
using NetDimension.Json.Linq;
using SpriderProxy.Analysis;

namespace BLL.Sprider.ProList.Api
{
    public class MeTaoProductsApi : Metao, IApiProList
    {
        private const string ProModeurl = "http://www.metao.com/oversea/0/{0}/0";

        public MeTaoProductsApi()
        {
            Baseinfo = new SiteInfoDB().SiteById(266);
        }

        public void GetAllProducts()
        {
            int totalPage = 2;
            for (int i = 1; i < totalPage; i++)
            {
                string prourl = string.Format(ProModeurl, i);
                string homepage = HtmlAnalysis.Gethtmlcode(prourl);
                if (i == 1)
                {
                    dynamic obj = JObject.Parse(homepage);
                    totalPage = obj.totalPage;
                }
              
                addproduct(homepage);
            }


        }

        private void addproduct(string homepage)
        {
            dynamic obj = JObject.Parse(homepage);
            List<SiteProInfo> items = new List<SiteProInfo>();
            foreach (var item in obj.data)
            {
                try
                {
                    SiteProInfo proinfo = new SiteProInfo();
                    proinfo.SiteSkuId = item.id;
                    proinfo.SpName = item.name;
                    proinfo.smallPic = item.photo;
                    proinfo.SpPrice = item.totalPrice;
                    proinfo.SearchFiles = item.tags;
                    proinfo.CreateDate = DateTime.Now;
                    proinfo.SiteId = Baseinfo.SiteId;
                    proinfo.SellType = 1;
                    proinfo.ProUrl = string.Format("http://detail.metao.com/products/{0}", proinfo.SiteSkuId);
                    items.Add(proinfo);
                    if (items.Count > 29)
                    {
                        new SiteProInfoDB().AddSitePro(items);
                        items.Clear();
                    }

                }
                catch
                {

                }


            }

            if (items.Count > 0)
            {
                new SiteProInfoDB().AddSitePro(items);
                items.Clear();
            }
        }


        public void AddNewProducts()
        {
            throw new NotImplementedException();
        }
    }
}
