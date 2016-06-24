using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace AamirKhan
{
    /// <summary>
    /// 队列多线程,T 代表处理的单个类型~
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class QueueThreadPlusBase<T>
    {
        #region 变量&属性
        /// <summary>
        /// 待处理结果
        /// </summary>
        private class PendingResult
        {
            /// <summary>
            /// 待处理值
            /// </summary>
            public T PendingValue { get; set; }
            /// <summary>
            /// 是否有值
            /// </summary>
            public bool IsHad { get; set; }
        }
        /// <summary>
        /// 线程数
        /// </summary>
        public int ThreadCount
        {
            get { return _mThreadCount; }
            set { _mThreadCount = value; }
        }
        private int _mThreadCount = 5;
        /// <summary>
        /// 取消=True
        /// </summary>
        public bool Cancel { get; set; }
        /// <summary>
        /// 线程列表
        /// </summary>
        //List<Thread> _mThreadList;
        /// <summary>
        /// 完成队列个数
        /// </summary>
        private volatile int _mCompletedCount;
        /// <summary>
        /// 队列总数
        /// </summary>
        private  int _mQueueCount;
        /// <summary>
        /// 全部完成锁
        /// </summary>
        private object _mAllCompletedLock = new object();
        /// <summary>
        /// 完成的线程数
        /// </summary>
        private int _mCompetedCount;
        /// <summary>
        /// 队列锁
        /// </summary>
        private object _mPendingQueueLock = new object();
        private Queue<T> m_InnerQueue;
        #endregion


        #region 事件相关
        /// <summary>
        /// 全部完成事件
        /// </summary>
        public event Action<CompetedEventArgs> AllCompleted;
        /// <summary>
        /// 单个完成事件
        /// </summary>
        public event Action<int,T, CompetedEventArgs> OneCompleted;
        /// <summary>
        /// 引发全部完成事件
        /// </summary>
        /// <param name="args"></param>
        private void OnAllCompleted(CompetedEventArgs args)
        {
            if (AllCompleted != null)
            {
                try
                {
                    AllCompleted(args);//全部完成事件
                }
                catch { }
            }
        }
        /// <summary>
        /// 引发单个完成事件
        /// </summary>
        /// <param name="pendingValue"></param>
        /// <param name="args"></param>
        private void OnOneCompleted(int index, T pendingValue, CompetedEventArgs args)
        {
            if (OneCompleted != null)
            {
                try
                {
                    OneCompleted(index,pendingValue, args);
                }
                catch { }

            }
        }
        #endregion

        #region 构造
        public QueueThreadPlusBase(IEnumerable<T> collection)
        {
            m_InnerQueue = new Queue<T>(collection);
            _mQueueCount = m_InnerQueue.Count;
        }

        private IEnumerable<T> addcollection;
        /// <summary>
        /// 用于异步导入数据
        /// </summary>
        /// <param name="collection"></param>
        public void addBindDate(IEnumerable<T> collection)
        {
            if(addcollection == null)
                addcollection = collection;
            addcollection.Intersect(collection);
            _mQueueCount += collection.Count();
        }

        #endregion

        #region 主体
        /// <summary>
        /// 初始化线程
        /// </summary>
        private void InitThread()
        {
            //_mThreadList = new List<Thread>();
            for (var i = 0; i < ThreadCount; i++)
            {
                Task task = new Task(obj => InnerDoWork((int)obj), i);
                task.Start();
                //Task.Factory.StartNew(obj => Start((int)obj), i);
            }
        }
        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            InitThread();
        }
        /// <summary>
        /// 线程工作
        /// </summary>
        private void InnerDoWork(int index)
        {
            try
            {
                Exception doWorkEx = null;
                DoWorkResult doworkResult = DoWorkResult.ContinueThread;

              
                var t = CurrentPendingQueue;
                while (!Cancel && t.IsHad)
                {
                    try
                    {
                        doworkResult = DoWork(index,t.PendingValue);
                       
                    }
                    catch (Exception ex)
                    {
                        doWorkEx = ex;
                    }
                    _mCompletedCount++;
                    int precent = _mCompletedCount * 100 / _mQueueCount;
                    OnOneCompleted(index,t.PendingValue, new CompetedEventArgs() { CompetedPrecent = precent,CompletedCount = _mCompletedCount, QueueCount= _mQueueCount, InnerException = doWorkEx });
                    if (doworkResult == DoWorkResult.AbortAllThread)
                    {
                        Cancel = true;
                        break;
                    }
                    if (doworkResult == DoWorkResult.AbortCurrentThread)
                    {
                        break;
                    }
                    t = CurrentPendingQueue;
                }

                lock (_mAllCompletedLock)
                {
                    _mCompetedCount++;
                    if (_mCompetedCount == ThreadCount)
                    {
                        OnAllCompleted(new CompetedEventArgs() { CompetedPrecent = 100 });
                    }
                }

            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 子类重写
        /// </summary>
        /// <param name="pendingId"></param>
        /// <returns></returns>
        protected virtual DoWorkResult DoWork(int index,T pendingId)
        {
            return DoWorkResult.ContinueThread;
        }
        /// <summary>
        /// 获取当前结果
        /// </summary>
        private PendingResult CurrentPendingQueue
        {
            get
            {
                lock (_mPendingQueueLock)
                {
                    PendingResult t = new PendingResult();
                    if (m_InnerQueue.Count != 0)
                    {
                        t.PendingValue = m_InnerQueue.Dequeue();
                        t.IsHad = true;
                    }
                    else
                    {
                        if (addcollection != null && addcollection.Count() > 0)
                        {
                            m_InnerQueue = new Queue<T>(addcollection);
                            t.PendingValue = m_InnerQueue.Dequeue();
                            t.IsHad = true;
                            addcollection = null;
                        }
                        else
                        {
                            t.PendingValue = default(T);
                            t.IsHad = false;
                        }
                   
                    }
                    return t;
                }
            }
        }

        #endregion

        #region 相关类&枚举
        /// <summary>
        /// dowork结果枚举
        /// </summary>
        public enum DoWorkResult
        {
            /// <summary>
            /// 继续运行，默认
            /// </summary>
            ContinueThread = 0,
            /// <summary>
            /// 终止当前线程
            /// </summary>
            AbortCurrentThread = 1,
            /// <summary>
            /// 终止全部线程
            /// </summary>
            AbortAllThread = 2
        }
        /// <summary>
        /// 完成事件数据
        /// </summary>
        public class CompetedEventArgs : EventArgs
        {
            public CompetedEventArgs()
            {

            }
            /// <summary>
            /// 完成百分率
            /// </summary>
            public int CompetedPrecent { get; set; }
            /// <summary>
            /// 已完成队列数
            /// </summary>
            public int CompletedCount { get; set; }
            /// <summary>
            /// 总队列数
            /// </summary>
            public int QueueCount { get; set; }
            

            /// <summary>
            /// 异常信息
            /// </summary>
            public Exception InnerException { get; set; }
        }
        #endregion

    }
}