using System;
using System.Collections.Generic;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
   public class RegProListDB : OrmLiteFactory
    {
       private static OrmLiteConnectionFactory _dbFactory;

       public static void AddRegProList(IEnumerable<RegProListInfo> regProList)
       {
           if (regProList == null) throw new ArgumentNullException("regProList");
           _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
           //_dbFactory = new OrmLiteConnectionFactory(ZnmDBConnectionString, SqlServerDialect.Provider);
           try
           {
               using (var db = _dbFactory.OpenDbConnection())
               {
                   db.InsertAll(regProList);
               }
           }
           catch (Exception ex)
           {
               LogServer.WriteLog(ex, "DBError");
               throw;
           }
       }

       public List<RegProListInfo> GetRegProList()
       {
           //ReFreshProList();
           //_dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
           _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
           try
           {
               using (var db = _dbFactory.OpenDbConnection())
               {
                   return db.Select<RegProListInfo>();
               }
           }
           catch (Exception ex)
           {
               LogServer.WriteLog(ex, "DBError");
               throw;
           }

       }

       public void ReFreshProList()
       {
           List<RegProListInfo> lits;
           _dbFactory = new OrmLiteConnectionFactory(mmbpriceDBConnectionString, SqlServerDialect.Provider);
           try
           {
               using (var db = _dbFactory.OpenDbConnection())
               {
                   lits =
                       db.SqlList<RegProListInfo>(
                           "select 0 as id,siteid,urladdress as ProListUrl,listsReg,singleReg,UrlReg,PriceReg,TitleReg,PicReg,isSellReg,CommentCountReg,commentUrlReg,idReg as SkuReg,shopIDReg,maxpageReg,pageStart,pageStep,'' as Remark,getdate() as UpdateTime from JD_Reg_SiteList where [type]=0");
               }
               _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
               using (var db = _dbFactory.OpenDbConnection())
               {
                   db.CreateTable<RegProListInfo>(true);
               }
               AddRegProList(lits);
           }
           catch (Exception ex)
           {
               LogServer.WriteLog(ex, "DBError");
               throw;
           }
       }
       /// <summary>
       /// sqllit 删除时候 文件大小不便 需要执行此方法来让文件变小
       /// </summary>
       public void Clear()
       {
           _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
           using (var db = _dbFactory.OpenDbConnection())
           {
               db.ExecuteNonQuery("VACUUM");
           }

       }

    }
}
