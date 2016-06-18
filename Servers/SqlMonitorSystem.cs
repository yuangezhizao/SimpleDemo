using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLL;
using Commons;

namespace Servers
{
    public class SqlMonitorSystem
    {
        public void Begin()
        {
            Task.Factory.StartNew(LockedVolid);
        }
        /// <summary>
        /// 查看是否存在数据库死锁
        /// </summary>
        private void LockedVolid()
        {
            SqlMonitorBll monitor = new SqlMonitorBll();
            while (true)
            {
                try
                {
                    bool flag = monitor.SqlLocked();
                    if (flag)
                    {
                        EmailServer.SendMail("www4数据库出现死锁，请处理", "数据库死锁", new[] { "chenjie@manmanbuy.com" });
                    }
                    Thread.Sleep(300000);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                    break;
                }
               
            }
           
        }

     
    }
}
