using Commons;
using Mode;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;

namespace DataBase
{
    public class mmbSiteClassInfoDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public mmbSiteClassInfoDB()
        {
            _dbFactory = new OrmLiteConnectionFactory(mmbpriceDBConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<SiteClassInfo>();
            }
        }

        public List<SiteClassInfo> getAllSiteCatInfo(int siteid)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    return db.Select<SiteClassInfo>(p => p.SiteId==siteid);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                throw;
            }
        }

        public  void AddSiteClass(IEnumerable<SiteClassInfo> classList)
        {
            if (classList == null) throw new ArgumentNullException("classList");
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.InsertAll(classList);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }

            }
        }

        public void addmmbSIteClass(List<SiteClassInfo> lits)
        {
            if (lits == null || lits.Count == 0)
                return;

            _dbFactory = new OrmLiteConnectionFactory(mmbDBConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                foreach (SiteClassInfo item in lits)
                {
                    int haschild = item.HasChild ? 1 : 0;
                    string sql = "insert into JD_Shop_Class (siteid,classid,classname,parentclass,parentname,classCrumble,parentUrl,urlinfo,HasChild,procount ,IsBind ,isshow , UpdateTime, CreateDate)values({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},{9},{10},{11},'{12}','{13}')";
                    string addsql = string.Format(sql, item.SiteId, item.ClassId, item.ClassName, item.ParentClass, item.ParentName, item.ClassCrumble, item.ParentUrl, item.Urlinfo, haschild, item.TotalProduct, 0, 1, item.CreateDate, item.UpdateTime);
                    int count = db.ExecuteNonQuery(addsql);
                }

            }

        }
       

        public void UpdateSiteClass(SiteClassInfo catinfo)
        {
            if (catinfo == null) throw new ArgumentNullException("catinfo");
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.Update(catinfo);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }

            }
        }

        public List<SiteClassInfo> getmmbSiteClass(int siteid)
        {
            List<SiteClassInfo> lits = null;
            _dbFactory = new OrmLiteConnectionFactory(mmbDBConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                // var bb=  db.SqlList<Dictionary<int, List<string>>>("select id as SiteId,sitename as SiteName,sitelogo as SiteLogo,fk as Domain,smallLogo as smallLogo from JD_hzSite");
                lits = db.SelectFmt<SiteClassInfo>("select id,siteid,classid,classname,parentclass,parentname,classCrumble,parentUrl,urlinfo,HasChild,procount as TotalProduct ,smallclassid as  BindClassId,IsBind ,isshow as IsHide,getdate() as UpdateTime,getdate() as CreateDate,0 as IsDel from  JD_Shop_Class where siteid={0}" , siteid);

            }
            return lits;
        }

    }
}
