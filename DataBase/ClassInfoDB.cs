using System;
using System.Collections.Generic;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
    public class ClassInfoDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public ClassInfoDB()
        {
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<ClassInfo>();
            }
        }

        public int AddCatInfo(ClassInfo cat)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                 db.Insert(cat);
                 return db.Scalar<ClassInfo, int>(x => Sql.Max(x.Id));
            }

        }
        public void UpdateCatInfo(ClassInfo cat)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.Update(cat);
            }

        }

        public List<ClassInfo> GetAllCatInfo()
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                return db.Select<ClassInfo>();
            }
        }



        public ClassInfo getCat(int catId)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                return db.SingleById<ClassInfo>(catId);
            }
        }
        public List<ClassInfo> GetChildCatById(int id)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                return db.Select<ClassInfo>(p => p.ParentId == id);
            }
        }

        public bool DelClass(ClassInfo cat)
        {
            if (cat == null) throw new ArgumentNullException("cat");
            using (var db = _dbFactory.OpenDbConnection())
            {
                cat.IsDel = true;
                int res = db.UpdateOnly(cat,
                    u => u.IsDel, u => u.Id == cat.Id);
                return res > 0;
            }
        }
    }
}
