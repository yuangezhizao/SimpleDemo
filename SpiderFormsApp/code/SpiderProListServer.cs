using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using BLL;
using BLL.Sprider.ProList;
using Commons;
using Mode;

namespace SpiderFormsApp.code
{
    public class SpiderProListServer
    {
        /// <summary>
        /// 退出程序
        /// </summary>
        public static bool Quitout { get; set; }
        public DateTime appStartTime { get; set; }
        public int appTotalCount { get; set; }
        public DateTime appStopTime { get; set; }
        public int ThreadCount { get; set; }
        public int ProListCount { get; set; }
        /// <summary>
        /// 当前ProListUrl索引
        /// </summary>
        public static int CurrenList { get; set; }
        private static List<RegProListInfo> RegProList { get; set; }
        private static List<SiteClassInfo> SiteClassList { get; set; }
        private static List<ClassInfo> ClassList { get; set; }
        public void CaseInit(SpiderConfig config)
        {
            int error = 0;
            do
            {
                try
                {
                    RegProList = new RegProListBll().GetRegProList();
                    MessageCenter.ShowBox("正则数据导入完毕！", 2);
                    SiteClassList = new SiteClassBll().GetBingCat(config.ClassInfoId,config.SiteInfoId);
                    MessageCenter.ShowBox("更新数据导入完毕！", 2);
                    ClassList = new ClassInfoBll().GetAllCatInfo();
                    MessageCenter.ShowBox("分类数据导入完毕！", 2);
                    ProListCount = SiteClassList.Count;
                    break;
                }
                catch (Exception ex)
                {
                    error++;
                    Thread.Sleep(60000);
                    LogServer.WriteLog(ex);
                }
            } while (error<5);

        }

        public void Begin()
        {
            appStartTime = DateTime.Now;
            appTotalCount = 0;
            appStopTime = DateTime.MinValue;
            if (ThreadCount > ProListCount)
            {
                ThreadCount = ProListCount;
            }
            if (ThreadCount == 0)
            {
                MessageCenter.ShowBox("获取不到需要更新的数据！更新结束", 2);
                return;
            }

            for (int i = 0; i < ThreadCount; i++)
            {
                var task = Task.Factory.StartNew(Startupdate);
             
                task.ContinueWith(
                    t => MessageCenter.ShowBox(string.Format("线程{0}更新结束还有{1}线程在运行", Thread.CurrentThread.Name, --ThreadCount), 2));
            }
        }

        private void Startupdate()
        {
            Thread.Sleep(10);
      
            var proinfo = getNextProduct();
            MessageCenter.ShowBox("线程" + Thread.CurrentThread.Name + "开始更新", 2);
            while (proinfo != null && !Quitout)
            {
                //if (proinfo.SiteId != 4)
                //{
                //  proinfo=  getNextProduct();
                //    continue;
                //}
                try
                {
                    SiteFactory factory = new SiteFactory();
                    factory.SiteId = proinfo.SiteId;
                    var proListManager = factory.ProListManager;
                    proListManager.CatInfo = ClassList.SingleOrDefault(p => p.Id == proinfo.BindClassId);
                    proListManager.Reginfo = RegProList.SingleOrDefault(p => p.SiteId == proinfo.SiteId);
                    proListManager.SiteCatInfo = proinfo;

                    SaveProduct(proListManager);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "server");
                    proinfo = getNextProduct();
                    continue;
                }

                proinfo = getNextProduct();

            }
            
        }

        //再分配一个类别
        private SiteClassInfo getNextProduct()
        {
            SiteClassInfo np = null;
            Monitor.Enter(this);
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = (CurrenList + 1).ToString();
            }
            if (CurrenList < ProListCount && SiteClassList != null)
            {
                np = SiteClassList[CurrenList];
            }
            CurrenList++;
            Monitor.Exit(this);

            return np;

        }


        private void SaveProduct(IProList proListManager)
        {
            string pageUrl = proListManager.getListUrl();

            //var paNum = proListManager.Reginfo.PageStart + proListManager.Reginfo.PageStep;
            //string tempPageUrl = pageUrl.Replace("$page", paNum.ToString());
            //string pageHtml = proListManager.DownLoadPage(tempPageUrl, proListManager.AreaInfoId);
            //if (string.IsNullOrEmpty(pageHtml))
            //{
            //    if (proListManager.ErrorInfo != null)
            //        new SaveSpiderErrorBll().SaveSpiderError(proListManager.ErrorInfo);
            //          return;
            //}

            int maxPage = 1;
        
            List<SiteProInfo> list = new List<SiteProInfo>();
            SiteProInfoBll probll = new SiteProInfoBll();

           
            for (int i = 0; i < maxPage; i++)
            {
                if (Quitout)
                    break;
                int paNum = proListManager.Reginfo.PageStart + proListManager.Reginfo.PageStep * i;
                string tempPageUrl = pageUrl.Replace("$page", paNum.ToString());
                Stopwatch downLoadtime = new Stopwatch();
                downLoadtime.Start();
                string pageHtml = proListManager.DownLoadPage(tempPageUrl, proListManager.AreaInfoId);
                downLoadtime.Stop();
                if (string.IsNullOrEmpty(pageHtml))
                {
                    if (proListManager.ErrorInfo != null)
                        new SaveSpiderErrorBll().SaveSpiderError(proListManager.ErrorInfo);
                    continue;
                }
             
                if (i == 0)
                {
                     maxPage = proListManager.getPageNum(pageHtml);
                    if (!proListManager.HasProducts(pageHtml))
                    {
                        if (proListManager.ErrorInfo != null)
                        {
                            proListManager.ErrorInfo.UrlPath = pageUrl;
                            new SaveSpiderErrorBll().SaveSpiderError(proListManager.ErrorInfo);
                            return;
                        }
                    }
                }
                string content = string.IsNullOrEmpty(proListManager.Reginfo.ListsReg) ? pageHtml : proListManager.GetProConent(pageHtml);
                MatchCollection mcsingle = proListManager.GetSigleProduct(content);
                if(mcsingle== null)
                {
                    MessageCenter.ShowBox("[线程" + Thread.CurrentThread.Name + "]" + proListManager.SiteName + " " + proListManager.CatInfo.CatName + " 第" + (i + 1) + "页 单个产品匹配错误" , 2);
                    continue;
                }

                if (proListManager.ErrorInfo != null)
                {
                    new SaveSpiderErrorBll().SaveSpiderError(proListManager.ErrorInfo);
                    proListManager.ErrorInfo = null;
                    continue;
                }
                int tempUpdate = 0;
                int temperror = 0;
                int tempadd = 0;
                Stopwatch detialtime = new Stopwatch();
                detialtime.Start();
                foreach (Match item in mcsingle)
                {
                    SiteProInfo pro = new SiteProInfo();
                    string singlePro = item.ToString();
                    try
                    {
                         pro = GetProduct(singlePro, proListManager);
                    }
                    catch (Exception ex)
                    {
                        LogServer.WriteLog(ex);
                        continue;
                    }
                   
                    if (proListManager.ErrorInfo != null)
                    {
                        
                        new SaveSpiderErrorBll().SaveSpiderError(proListManager.ErrorInfo);
                        proListManager.ErrorInfo = null;
                        temperror++;
                        continue;
                    }
                    if (pro == null)
                    {
                        temperror++;
                        continue;
                    }
                    var oldpro = probll.FindOne(pro.SiteSkuId);
                    if (oldpro != null)
                    {
                        if (pro.SpPrice != oldpro.SpPrice || pro.SpName != oldpro.SpName)
                        {
                            new HisSiteProInfoBll().AddHissiteproinfo(pro);
                        }
                        if (oldpro.FloorPrice < pro.FloorPrice)
                        {
                            pro.FloorPrice = oldpro.FloorPrice;
                        }
                        pro.CreateDate = oldpro.CreateDate;
                        probll.UpdateProinfo(pro);
                        tempUpdate++;
                    }
                    else
                    {
                        pro.ClassId = proListManager.CatInfo.Id;
                        //detial
                        getProDetial(pro, proListManager);
                        

                        list.Add(pro);
                        new HisSiteProInfoBll().AddHissiteproinfo(pro);
                        tempadd++;

                  

                    }
                       
                }
                detialtime.Stop();
                probll.addSiteProinfo(list);
                list.Clear();
                MessageCenter.ShowBox("[线程" + Thread.CurrentThread.Name + "]" + proListManager.SiteName + " " + proListManager.CatInfo.CatName + " 第" + (i + 1) + "/" + maxPage + "页" + tempUpdate + "/" + tempadd + "/" + temperror + " t1:" + downLoadtime.Elapsed.TotalSeconds.ToString("0") + "s " + "t2:" + detialtime.Elapsed.TotalSeconds.ToString("0") + "s", 2);
            }
            if (list.Count == 0)
                return;

        }

        private SiteProInfo GetProduct(string singleHtml,IProList proListManager)
        {
            SiteProInfo item = new SiteProInfo();
            item.ProUrl = proListManager.GetSpUrl(singleHtml);
            if (proListManager.ErrorInfo != null)
            {
                new SaveSpiderErrorBll().SaveSpiderError(proListManager.ErrorInfo);
                proListManager.ErrorInfo = null;
                return null;
            }
            item.SiteSkuId = proListManager.GetItemSku(item.ProUrl);
            item.SpName = proListManager.GetSpName(singleHtml);
            if (proListManager.ErrorInfo != null)
            {
                new SaveSpiderErrorBll().SaveSpiderError(proListManager.ErrorInfo);
                proListManager.ErrorInfo = null;
                return null;
            }
            item.SpPrice = proListManager.GetSpPrice(singleHtml, item.SiteSkuId);
            if (item.SpPrice <= 0)
                return null;
            item.FloorPrice = item.SpPrice;
            if (proListManager.ErrorInfo != null)
            {
                new SaveSpiderErrorBll().SaveSpiderError(proListManager.ErrorInfo);
                proListManager.ErrorInfo = null;
                return null;
            }
            item.smallPic = proListManager.GetSmallPic(singleHtml);
            if (proListManager.ErrorInfo != null)
            {
                new SaveSpiderErrorBll().SaveSpiderError(proListManager.ErrorInfo);
                proListManager.ErrorInfo = null;
                return null;
            }
            item.CommenCount = proListManager.GetComments(singleHtml);
            if (proListManager.ErrorInfo != null)
            {
                new SaveSpiderErrorBll().SaveSpiderError(proListManager.ErrorInfo);
                proListManager.ErrorInfo = null;
                return null;
            }
           
            item.SiteCat = proListManager.SiteCatInfo.ClassId;
            item.SiteId = proListManager.SiteCatInfo.SiteId;
            item.CreateDate = DateTime.Now;
            item.UpdateTime = DateTime.Now;
            return item;
        }


        private void getProDetial(SiteProInfo pro, IProList proListManager)
        {
            string detial = proListManager.DownLoadPage(pro.ProUrl, proListManager.AreaInfoId);
            pro.SellType = proListManager.GetSellType(detial);
            pro.BigPic = proListManager.GetBigPic(detial);
            pro.spBrand = proListManager.GetBrand(detial);
            pro.spSkuDes = proListManager.GetSkuDes(detial);
            pro.Otherpic = proListManager.GetOtherpic(detial);
            pro.ShopName = proListManager.GetShopName(detial);
        }
   
        public void getItemsByApi()
        {
            int[] Sites = {2};
            Parallel.ForEach(Sites, num =>
            {
                Stopwatch t1 = new Stopwatch();
                t1.Start();
                LogServer.WriteLog("商城分类更新开始执行 siteid：" + num, "RunInfo");
                try
                {
                    // new SiteFactory { SiteId = num }.ProIApiManager.AddNewProducts();
                    new SiteFactory { SiteId = num }.ProIApiManager.GetAllProducts();
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog("商城分类更新异常 siteid：" + num + "\r\n" + ex.Message, "RunInfo");
                }
                t1.Stop();
                LogServer.WriteLog(string.Format("商城分类更新结束 siteid：{0}耗时{1}小时{2}分钟{3}秒", num, t1.Elapsed.Hours, t1.Elapsed.Minutes, t1.Elapsed.Seconds), "RunInfo");
            });
        }
    }
}
