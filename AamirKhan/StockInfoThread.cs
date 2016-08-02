using System;
using System.Collections.Generic;
using System.Threading;
using BLL.Sprider.Stock;
using Commons;
using Mode;

namespace AamirKhan
{
    /// <summary>
    /// 下载线程对了.
    /// </summary>
    public class StockInfoThread : QueueThreadPlusBase<StockInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list">下载的列表ID</param>
        public StockInfoThread(IEnumerable<StockInfo> list) : base(list)
        {
        }

        /// <summary>
        /// 每次多线程都到这里来,处理多线程
        /// </summary>
        /// <param name="item">列表ID</param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override DoWorkResult DoWork(int index, StockInfo item)
        {
            try
            {
                Thread.Sleep(1000 * 1);
                if(item==null)
                    return DoWorkResult.ContinueThread;
                new StockInfoBll().GetXueqiuStockDetial(item);
                //new StockInfoBll().GetStockDetial(item);
                LogServer.WriteLog("线程："+ index +"编号："+ item.StockNo+"正在执行中", "StockDayReport");
                return DoWorkResult.ContinueThread;//没有异常让线程继续跑..
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "StockDayReportError");
                return DoWorkResult.AbortCurrentThread;//有异常,可以终止当前线程.当然.也可以继续,
                //return  DoWorkResult.AbortAllThread; //特殊情况下 ,有异常终止所有的线程...
            }

            //return base.DoWork(pendingValue);
        }
    }

}
