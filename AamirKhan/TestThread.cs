using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Commons;

namespace AamirKhan
{
    /// <summary>
    /// 下载线程对了.
    /// </summary>
    public class DownLoadQueueThread : QueueThreadBase<ProInfo>
    {

      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list">下载的列表ID</param>
        public DownLoadQueueThread(IEnumerable<ProInfo> list) : base(list)
        {
        
        }

        

        /// <summary>
        /// 每次多线程都到这里来,处理多线程
        /// </summary>
        /// <param name="pendingId">列表ID</param>
        /// <returns></returns>
        protected override DoWorkResult DoWork(ProInfo pendingId)
        {
            try
            {
                LogServer.WriteLog(pendingId.Id+"正在执行中", "DownLoadQueueThread");
                LogServer.WriteLog("JdCouponServer执行完成", "JdCoupon");
                Thread.Sleep(1000 * 3);
                //..........多线程处理....
                return DoWorkResult.ContinueThread;//没有异常让线程继续跑..

            }
            catch (Exception)
            {

                return DoWorkResult.AbortCurrentThread;//有异常,可以终止当前线程.当然.也可以继续,
                //return  DoWorkResult.AbortAllThread; //特殊情况下 ,有异常终止所有的线程...
            }

            //return base.DoWork(pendingValue);
        }
    }

    public class ProInfo
    {
        public  int Id { get; set; }
    }


}
