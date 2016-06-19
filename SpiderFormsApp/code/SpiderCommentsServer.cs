using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BLL;
using Commons;
using Mode;

namespace SpiderFormsApp.code
{
    /// <summary>
    /// 抓取评论服务
    /// </summary>
    public class SpiderCommentsServer
    {

        //public SpiderCommentsServer()
        //{
        //    //设置默认值
        //    ThreadCount = 10;
        //    MinProductId = 0;
        //    SiteIds = "";
        //}

        /// <summary>
        /// 开启线程数
        /// </summary>
        public static int ThreadCount { private get; set; }
        public int MaxProductId { get; set; }
        public int MinProductId { get; set; }
        public static bool Continue { private get; set; }
        public static string SiteIds { private get; set; }
        private static bool Quit { get; set; }
        private List<MmbProductItems> Items= new List<MmbProductItems>();

        /// <summary>
        /// 加载需要抓取评论的商品（异步运行）
        /// </summary>
        private void GetItems()
        {

            int proId = MaxProductId;
            Console.WriteLine(@"/***************加载产品信息开始***************/");
            while (proId > MinProductId && !Quit)
            {
                int tempcount;
                lock (Items)
                {
                    tempcount = Items.Count;
                }
                if (tempcount > 50000)
                {
                    Console.WriteLine(@"需处理的产品大于5万，暂停加载数据一分钟后继续加载");
                    Thread.Sleep(1000 * 60);
                    continue;
                }
                var list = new MmbProductItemsBll().GetItem(proId - 1000, proId, SiteIds);
                if (list != null && list.Count > 0)
                {
                    lock (Items)
                    {
                        Items.AddRange(list);
                    }
                }
                proId = proId - 1000;
                Console.WriteLine(@"加载产品信息proId{0} 已加载加载{1}条数据[{2}]", proId, Items.Count,DateTime.Now.ToShortTimeString());
                Thread.Sleep(1000);  
            }
            Console.WriteLine(@"/***************加载产品信息截止***************/");
            
        }

        public void AppStart()
        {
            Quit = false;
            Console.WriteLine(@"评论抓取工具开始运行");
            MaxProductId = Continue ? new CommentBll().GetLastComentItemId() : new MmbProductItemsBll().GetProductMaxId();
            Task.Factory.StartNew(GetItems);
            while (Items.Count<ThreadCount)
            {
                Thread.Sleep(3000);
            }
            for (int i = 0; i < ThreadCount; i++)
            {
                Task task = new Task(obj => Start((int) obj), i);
                task.Start();
                //Task.Factory.StartNew(obj => Start((int)obj), i);
            }

        }

        public static void AppStop()
        {
            
            Quit = true;
        }

        private void Start(int index)
        {
            Thread.Sleep(100);
         
            MessageCenter.ListViewMsg(index,"", "", "开始运行");
            var proItem = GetNextItem();
            while (proItem != null && !Quit)
            {
                SiteFactory analysis = new SiteFactory {SiteId = proItem.siteid};
               
                var commentsManager = analysis.CommentsManager;
                if (commentsManager == null)
                {
                    proItem = GetNextItem();
                    continue;
                }
            
                List<CommentInfo> list = new List<CommentInfo>();
                try
                {
                    list = commentsManager.GetCommentsFirstPage(proItem.spurl);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "CommentSpiderError");
                }
                if (null == list)
                {
                    MessageCenter.ListViewMsg(index, commentsManager.SiteName, proItem.spurl, "该商品没有品论");
                    proItem = GetNextItem();
                    continue;
                }
                var endTime = new CommentBll().GetCommentEndDate(proItem.Id, proItem.siteid);
                foreach (CommentInfo item in list)
                {
                    item.Bjid = proItem.Id;
                    item.Spid = proItem.spid;
                    item.SiteId = proItem.siteid;
                    item.BrandId = proItem.ppid ?? 0;
                    item.BrandName = proItem.pingpai;
                    item.ClassId = proItem.classid;
                    item.CreateDate = DateTime.Now;
                    item.IsDel = false;
                }
                list = list.FindAll(c => c.SendTime > endTime);
               
                new CommentBll().AddSiteProInfo(list);
                string msg = string.Format(@"共抓取{0}条记录", list.Count);
                Console.WriteLine(@"[{4}线程{0}]:{1}消息{3},{2}", index, commentsManager.SiteName, proItem.spurl, msg, DateTime.Now.ToShortTimeString());
                MessageCenter.ListViewMsg(index, commentsManager.SiteName, proItem.spurl, msg);
                proItem = GetNextItem();
            }
            Console.WriteLine(@"线程{0}更新结束", Thread.CurrentThread.ManagedThreadId);
            MessageCenter.ListViewMsg(index, "", "", "更新完毕");
        }

       
        private MmbProductItems GetNextItem()
        {
            MmbProductItems item = null;
            lock (Items)
            {
                if (Items.Count > 0)
                    Items.RemoveAt(0);
                if (Items.Count>0 && Items != null)
                {
                    item = Items[0];
                }
            }
            return item;
        }
    }
}
