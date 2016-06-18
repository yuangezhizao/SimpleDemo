using System;
using System.Collections.Generic;
using DataBase;

namespace BLL
{
    public class SqlMonitorBll
    {
        private List<DateTime> _lockedCount;

        public SqlMonitorBll()
        {
            _lockedCount = new List<DateTime>();
        }

        public bool SqlLocked()
        {
            SqlMonitorDB db = new SqlMonitorDB();
            if (db.IsLocked())
            {
                _lockedCount.Add(DateTime.Now);
                if (_lockedCount.Count > 2)
                {
                    _lockedCount.Clear();
                    return true;
                }
            }
            else if (_lockedCount.Count > 0)
                _lockedCount.Clear();
            return false;
        }
    }
}
