    using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLL;
using BLL.WeiBo;
using Commons;
using Mode;

namespace Servers
{
   public class SpriderSystem
    {
       static readonly int[] Sites = { 1, 3, 4, 6, 8, 9, 11, 13, 30, 33, 36, 42, 43, 52, 61, 123, 127, 129, 135, 142, 161, 166, 170, 177, 181, 185, 189, 194, 196, 243, 246, 248, 256, 257, 276 };
       public void Start()
       {
           LogServer.WriteLog("SpriderSystem Start", "RunInfo");
           var taskList = new SpiderConfigBll().LoadSpiderconfig();
           if (taskList == null)
           {
               LogServer.WriteLog("方案列表获取失败.....", "RunInfo");
               return;
           }
           var aliveList = taskList.Where(p => p.IsAlive);
           foreach (SpiderConfig item in aliveList)
           {
               SpiderConfig item1 = item;
               LogServer.WriteLog(item.TaskName + "\t开始执行", "RunInfo");
               Task.Factory.StartNew(() => CaseSystem(item1));
               Thread.Sleep(100);
           }

       }

       private void CaseSystem(SpiderConfig config)
       {
           if (config.StartTime == DateTime.MinValue)
               config.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 8:00:00");
           if (config.StopTime == DateTime.MinValue)
               config.StopTime = DateTime.MaxValue;
           var timeArea = config.StopTime - config.StartTime;
           if (timeArea.TotalSeconds < 0)
               return;
           int totalCount = 1;
           while (true)
           {
               if (DateTime.Now < config.StartTime)
               {
                   TimeSpan temp = config.StartTime - DateTime.Now;
                   LogServer.WriteLog(config.TaskName + "将在" + temp + "/s 后 执行 ", "RunInfo");
                   Thread.Sleep((int)temp.TotalMilliseconds);
               }
               if (DateTime.Now > config.StopTime)
               {
                   DateTime tempStop = config.StopTime;
                   do
                   {
                       config.StartTime = config.TimeSpan < 24*3600 ? config.StartTime.AddDays(1) : config.StartTime.AddSeconds(config.TimeSpan);
                   } while (config.StartTime < DateTime.Now);
                   config.StopTime = config.StartTime.Add(timeArea);
                   if (tempStop.AddSeconds(config.TimeSpan) > DateTime.Now)
                   {
                       config.TaskRemark = "今天更新结束,将在" + config.StartTime + "开始执行 ";
                       new SpiderConfigBll().SaveSpiderConfig(config);
                       TimeSpan temp = config.StartTime - DateTime.Now;
                       Thread.Sleep((int) temp.TotalMilliseconds);
                   }
                   else
                   {
                       config.TaskRemark = "程序已超过轮询间隔时间没有更新 将立即更新";
                       new SpiderConfigBll().SaveSpiderConfig(config);
                       Thread.Sleep(10);
                   }

                   LogServer.WriteLog(config.TaskName + "\t" + config.TaskRemark, "RunInfo");


               }


               Stopwatch t1 = new Stopwatch();
               t1.Start();
               try
               {
                   LogServer.WriteLog(config.TaskName + "\t开始执行运行 " + totalCount + "次", "RunInfo");
                   SpiderSystem(config);
               }
               catch (Exception ex)
               {
                   LogServer.WriteLog(ex);
               }

               t1.Stop();
               config.StartTime = config.StartTime.AddSeconds(config.TimeSpan);
               config.StopTime = config.StartTime.Add(timeArea);
               new SpiderConfigBll().SaveSpiderConfig(config);


               //double lessTime = config.TimeSpan * 1000 - t1.ElapsedMilliseconds;
               double lessTime = (config.StartTime - DateTime.Now).TotalMilliseconds;

               if (lessTime < 0)
               {
                   lessTime = 10;
                   config.TaskRemark = "更新完毕 运行 " + totalCount + "次，耗时" + t1.Elapsed + " 超出间隔时间 请优化程序或者调整间隔时间";
                   LogServer.WriteLog(config.TaskName + "\t执行时间超过间隔时间 运行 " + totalCount + "次", "RunInfo");
               }
               else
               {
                   config.TaskRemark = "更新完毕 运行 " + totalCount + "次，耗时" + t1.Elapsed + "/s 将在" + config.StartTime.AddSeconds(config.TimeSpan) + "开始执行sleep:" + (lessTime/3600000).ToString("0.00")+"小时";
                   LogServer.WriteLog(config.TaskName + "\t" + config.TaskRemark, "RunInfo");
               }
      
               Thread.Sleep((int) lessTime);
               totalCount++;
           }
       }

       private bool updateCatByid(int siteid)
       {
           try
           {
               Stopwatch t1 = new Stopwatch();
               t1.Start();
               LogServer.WriteLog("商城分类更新开始执行 siteid：" + siteid, "RunInfo");
               new SiteFactory { SiteId = siteid }.SiteClassManager.UpdateSiteCat();
               t1.Stop();
               LogServer.WriteLog(string.Format("商城分类更新结束 siteid：{0}耗时{1}小时{2}分钟{3}秒", siteid, t1.Elapsed.Hours, t1.Elapsed.Minutes, t1.Elapsed.Seconds), "RunInfo");
           }
           catch (Exception ex)
           {
               LogServer.WriteLog("商城分类更新异常 siteid：" + siteid + "\r\n" + ex.Message, "RunInfo");
               return false;
           }
           return true;
       }

       /// <summary>
       /// 更新分类
       /// </summary>
       private void UpdateSiteCat()
       {
           //Action<object> catAction = updateCatByid;

           //Task[] taskList = new Task[Sites.Length];
           //for (int i = 0; i < taskList.Length; i++)
           //{
           //    int siteid = Sites[i];
           //    taskList[i] = new Task(catAction, siteid);
           //}

           //int max = 10;
           //if (max > Sites.Length)
           //    max = Sites.Length;

           //for (int i = 0; i < max; i++)
           //{
           //    taskList[i].Start();
           //    taskList[i].Wait();
           //}
           //Sites.AsParallel().WithDegreeOfParallelism(10).Select(updateCatByid);

           Parallel.ForEach(Sites, num =>
           {
               Stopwatch t1 = new Stopwatch();
               t1.Start();
               LogServer.WriteLog("商城分类更新开始执行 siteid：" + num, "RunInfo");
               try
               {
                   new SiteFactory { SiteId = num }.SiteClassManager.UpdateSiteCat();
               }
               catch (Exception ex)
               {
                   LogServer.WriteLog("商城分类更新异常 siteid：" + num + "\r\n" + ex.Message, "RunInfo");
               }
               t1.Stop();
               LogServer.WriteLog(string.Format("商城分类更新结束 siteid：{0}耗时{1}小时{2}分钟{3}秒", num, t1.Elapsed.Hours, t1.Elapsed.Minutes, t1.Elapsed.Seconds), "RunInfo");
           });
       }

       public void UpdateSiteCat(int siteId)
       {
           Stopwatch t1 = new Stopwatch();
           t1.Start();
           LogServer.WriteLog("商城分类更新开始执行 siteid：" + siteId, "RunInfo");
           SiteFactory sf = new SiteFactory {SiteId = siteId};
           sf.SiteClassManager.UpdateSiteCat();
           t1.Stop();
           LogServer.WriteLog(string.Format("商城分类更新结束 siteid：{0}耗时{1}小时{2}分钟{3}秒", siteId, t1.Elapsed.Hours, t1.Elapsed.Minutes, t1.Elapsed.Seconds), "RunInfo");
       }

       public void SaveSiteCate()
       {

           Parallel.ForEach(Sites, num =>
           {
               SiteFactory sf = new SiteFactory {SiteId = num};
               sf.SiteClassManager.SaveAllSiteClass();
           });
       }

       public void SaveSiteCate(int siteid)
       {
           //int[] sites = { siteid };
           //Parallel.ForEach(sites, num =>
           //{
           //    SiteFactory sf = new SiteFactory();
           //    sf.SiteId = num;
           //    sf.SiteClassManager.SaveAllSiteClass();
           //});

           SiteFactory sf = new SiteFactory { SiteId = siteid };
           sf.SiteClassManager.SaveAllSiteClass();
      
       }

       private void SpiderSystem(SpiderConfig config)
       {
           switch (config.CaseType)
           {
               case 1:
                    UpdateSiteCat();
                    break;
               case 3:
                    UpdateSiteCat(10);
                   break;
           }
       }

       public static bool IsWeiboRuning ;
       public void SendWeibo()
       {
           if (IsWeiboRuning)
           {
               LogServer.WriteLog("正在运行", "WeiboMsg");
               return;
           }
           LogServer.WriteLog("开始执行", "WeiboMsg");
           IsWeiboRuning = true;
           int count = 0;
           try
           {
             
               var list = new PromotionsBll().LoadPromotions();

               if (list==null || list.Count == 0)
               {
                   IsWeiboRuning = false;
                   return;
               }
               foreach (PromotionsInfo item in list)
               {
                   if (item.PromoStopTime < DateTime.Now||item.PromoStartTime>DateTime.Now)
                       continue;
                
                   WeiboFactory factory = new WeiboFactory {Domain = "sina"};
                   string img;
                   if (item.PromoPic.Contains(','))
                   {
                       img = item.PromoPic.Substring(0, item.PromoPic.IndexOf(','));
                   }
                   else
                       img = item.PromoPic;
                   factory.WeiboManager.write(item.PromoWeibo, img);
                   count++;
                   LogServer.WriteLog("发布成功 id:" +item.Id +"title"+item.PromoTitle+"本次更新第"+ count + "条", "WeiboMsg");
                   Random rdm = new Random();
                   Thread.Sleep(1000 * 60 * rdm.Next(8,20));
               }
               SaveSitePromo();
            
           }
           catch (Exception ex)
           {
               LogServer.WriteLog(ex);
           }
           LogServer.WriteLog("一次更新结束共发布" + count + "条", "WeiboMsg");
           IsWeiboRuning = false;
       }
       /// <summary>
       /// 获取促销信息
       /// </summary>
       public void SaveSitePromo()
       {
           SiteFactory sitefac = new SiteFactory();
           sitefac.SiteId = 8;
           LogServer.WriteLog("促销信息开始获取 siteid：" + sitefac.SiteId, "RunInfo");
           sitefac.SitePromoManager.SaveSitePromo();
           LogServer.WriteLog("促销信息获取结束 siteid：" + sitefac.SiteId, "RunInfo");
       }


       public void LoadBand()
       {
           SiteFactory sitefac = new SiteFactory();
           sitefac.SiteId = 10;
           sitefac.SiteClassManager.LoadBand();
       }
    }
}
