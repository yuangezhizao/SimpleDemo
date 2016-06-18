using Commons;
using Mode;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DataBase
{
    public class SiteClassInfoDB : OrmLiteFactory
    {
        private  OrmLiteConnectionFactory _dbFactory;

        public SiteClassInfoDB()
        {
            // _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
          _dbFactory = new OrmLiteConnectionFactory(mmbpriceDBConnectionString, SqlServerDialect.Provider);
          using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<SiteClassInfo>();
            }
        }

        private void init()
        {
            List<SiteClassInfo> list = new List<SiteClassInfo>();
            _dbFactory = new OrmLiteConnectionFactory(mmbpriceDBConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                list = db.Select<SiteClassInfo>();
            }

            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
               db.CreateTable<SiteClassInfo>(true);
                
            }
            int total = list.Count()/1000 + 1;
            for (int i = 0; i < total; i++)
            {
                var templist = new List<SiteClassInfo>();
             
                for (int j = i*1000; j < (i+1)*1000; j++)
                {
                    if (j == list.Count)
                        break;
                    templist.Add(list[j]);
                }
                try
                {

                    using (var db = _dbFactory.OpenDbConnection())
                    {
                        //db.CreateTable<SiteClassInfo>(true);
                        db.InsertAll(templist);
                    }
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }
            }
        
        }

        public List<SiteClassInfo> getAllSiteCatInfo(int siteid)
        {
            //init();
            //test(siteid);
            try
            {
                _dbFactory = new OrmLiteConnectionFactory(mmbpriceDBConnectionString, SqlServerDialect.Provider);
                using (var db = _dbFactory.OpenDbConnection())
                {
                    return db.Select<SiteClassInfo>(p => p.SiteId==siteid && p.IsDel==false).OrderBy(p=>p.UpdateTime).ToList();
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                throw;
            }
        }

        public List<SiteClassInfo> getAllSiteCatInfo1(int siteid)
        {
            //init();
            try
            {
                _dbFactory = new OrmLiteConnectionFactory(ZnmDBConnectionString, SqlServerDialect.Provider);
                using (var db = _dbFactory.OpenDbConnection())
                {
                    return db.Select<SiteClassInfo>(p => p.SiteId == siteid && p.IsDel == false);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                throw;
            }
        }

        //private void test(int siteid)
        //{

        //    List<SiteClassInfo> mmblist = new List<SiteClassInfo>();
        //    List<SiteClassInfo> znmlist = new List<SiteClassInfo>();
        //    _dbFactory = new OrmLiteConnectionFactory(mmbpriceDBConnectionString, SqlServerDialect.Provider);
        //    using (var db = _dbFactory.OpenDbConnection())
        //    {
        //        mmblist = db.Select<SiteClassInfo>(p => p.ParentClass == "4002778");
        //    }
           
           

        //    foreach (SiteClassInfo item in mmblist)
        //    {
        //        if (item.ParentClass != "4002778")
        //            continue;
        //        var tempcat = new SiteClassInfo();
        //        _dbFactory = new OrmLiteConnectionFactory(ZnmDBConnectionString, SqlServerDialect.Provider);
        //        using (var db = _dbFactory.OpenDbConnection())
        //        {
        //            tempcat = db.Single<SiteClassInfo>(p => p.SiteId == item.SiteId && p.ClassId == item.ClassId);
        //        }

        //        if (tempcat != null)
        //        {
        //            item.ParentClass = tempcat.ParentClass;
        //            UpdateSiteClass(item);
        //        }

        //    }
        //}

        public  void AddSiteClass(List<SiteClassInfo> classList)
        {
            if (classList == null) throw new ArgumentNullException("classList");
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    for (int i = 0; i < classList.Count; i++)
                    {
                        if (
                            db.Exists<SiteClassInfo>(
                                p => p.SiteId == classList[i].SiteId && p.ClassId == classList[i].ClassId))
                            //  classList.Remove(classList[i]);
                            continue;
                        db.Insert(classList[i]);
                    }
                    //if (classList.Count>0)
                    //db.InsertAll(classList);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    return;
                }

            }
        }

        public void AddSiteClass(SiteClassInfo siteclass)
        {
            if (siteclass == null) throw new ArgumentNullException("siteclass");
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    if (db.Exists<SiteClassInfo>(p => p.SiteId == siteclass.SiteId && p.ClassId == siteclass.ClassId))
                        return;
                    db.Insert(siteclass);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }

            }
        }

        public bool IsExists(string classid)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                return db.Exists<SiteClassInfo>(new  { ClassId = classid });
            }
        }

        /// <summary>
        /// 解除绑定分类
        /// </summary>
        /// <param name="classId"></param>
        public void UnBindCat(int classId)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.UpdateOnly(new SiteClassInfo{BindClassId=0,IsBind=false,BindClassName=""},
                     u => new { u.BindClassId, u.IsBind ,u.BindClassName}, u => u.BindClassId == classId);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }

            }
        }

        public void UpdateSiteClass(SiteClassInfo catinfo)
        {
            _dbFactory = new OrmLiteConnectionFactory(mmbpriceDBConnectionString, SqlServerDialect.Provider);
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
                  //  throw;
                }

            }
        }

        public bool updateSpiderOnly(SiteClassInfo catinfo)
        {
           
            if (catinfo == null) throw new ArgumentNullException("catinfo");
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    //db.UpdateNonDefaults(new SiteClassInfo { Id = catinfo.Id, ClassName = catinfo.ClassName, ClassId = catinfo.ClassId, ParentClass = catinfo.ParentClass, ParentName = catinfo.ParentName, ParentUrl = catinfo.ParentUrl, UpdateTime = DateTime.Now, HasChild = catinfo.HasChild, Urlinfo = catinfo.Urlinfo, ClassCrumble = catinfo.ClassCrumble }, p => p.Id == catinfo.Id);
                    int res = db.UpdateOnly(catinfo,
                        u => new { u.ClassId, u.ClassName, u.Urlinfo, u.UpdateTime, u.ParentClass, u.ParentUrl, u.ParentName, u.ClassCrumble, u.TotalProduct, u.HasChild }, u => u.Id == catinfo.Id);
                    if (res > 0)
                        return true;
                    return false;
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    return false;
                }
                //int res = db.Update(catinfo);
                //db.Update(catinfo, p => p.ClassId == catinfo.ClassId);

               
            }
        }

        public List<SiteClassInfo> getmmbSiteClass(int siteid)
        {
            List<SiteClassInfo> lits = null;
            _dbFactory = new OrmLiteConnectionFactory(mmbDBConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
               lits = db.SelectFmt<SiteClassInfo>("select id,siteid,classid,classname,parentclass,parentname,classCrumble,parentUrl,urlinfo,HasChild,procount as TotalProduct ,smallclassid as  BindClassId,IsBind ,isshow as IsHide,getdate() as UpdateTime,getdate() as CreateDate from  JD_Shop_Class where siteid={0}" , siteid);

            }
            return lits;
        }

        public SiteClassInfo GetSiteCatById(int id)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                return db.SingleById<SiteClassInfo>(id);
            }
        }

        public void SetIsDel(SiteClassInfo catinfo)
        {
            if (catinfo == null) throw new ArgumentNullException("catinfo");
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.UpdateOnly(new SiteClassInfo{IsDel=true}, p => p.IsDel,p=>p.Id==catinfo.Id);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    //  throw;
                }

            }
        }

        public bool BingCatInfo(SiteClassInfo cat)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                int res = db.UpdateOnly(cat, u => new {u.IsBind, u.BindClassId,u.BindClassName}, u => u.Id == cat.Id);
                return res > 0;
            }
        }




        public List<SiteClassInfo> GetBindCat(string catids, string siteids)
        {
            //init();
            using (var db = _dbFactory.OpenDbConnection())
            {
                if (string.IsNullOrEmpty(catids) && string.IsNullOrEmpty(siteids))
                    return db.Select<SiteClassInfo>(q => q.IsBind);
                if (string.IsNullOrEmpty(catids) && !string.IsNullOrEmpty(siteids))
                    return db.Select<SiteClassInfo>(q => q.IsBind && Sql.In(q.SiteId, siteids.Split(',')));
                if (!string.IsNullOrEmpty(catids) && string.IsNullOrEmpty(siteids))
                    return db.Select<SiteClassInfo>(q => q.IsBind && Sql.In(q.ClassId, catids.Split(',')));
                return
                    db.Select<SiteClassInfo>(
                        q => q.IsBind && Sql.In(q.SiteId, siteids.Split(',')) && Sql.In(q.ClassId, catids.Split(',')));
            }
        }
    }
}
