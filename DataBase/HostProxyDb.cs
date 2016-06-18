using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
    public class HostProxyDb : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public HostProxyDb()
        {
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<HostProxy>();
            }
        }

        public void SaveHostProxy(List<HostProxy> list)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.InsertAll(list);
                }
                catch (Exception ex)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        SaveHostProxy(list[i]);
                    }
                    LogServer.WriteLog(ex, "DBError");
                }
            }
        }

        public bool Exist(HostProxy host)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                   return db.Exists<HostProxy>(p => p.IpAddress == host.IpAddress);
                }
                catch (Exception)
                {

                    return false;
                }
            }
        }

        public void SaveHostProxy(HostProxy host)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.Save(host);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                }
            }
        }

        public List<HostProxy> LoadHostProxy()
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    return db.Select(db.From<HostProxy>().OrderBy(p=>p.VolidTotalCount));
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    return null;
                }
               
            }
        }

        public void UpdateHostProxy(HostProxy hostProxy)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.Update(hostProxy);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                   
                }

            }
        }

        public void DelHostProxy(HostProxy hostProxy)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.Delete(hostProxy);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");

                }

            }
        }
    }
}
