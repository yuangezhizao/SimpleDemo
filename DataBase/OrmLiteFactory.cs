using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBase
{
   public class OrmLiteFactory
   {
       private const string Connent = @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\App_Data\SiteBase.db;Integrated Security=True;User Instance=True";

       public static string ConnectionString
       {
           get { 
               if (System.Configuration.ConfigurationManager.AppSettings["ConnectionString"] == null)
               {
                   return Connent;
               }
               return System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
                }
   
       }

       public static string mmbDBConnectionString {
           get { return System.Configuration.ConfigurationManager.AppSettings["mmbDB"]; }
           set { mmbDBConnectionString = value; } }

       public static string mmbpriceDBConnectionString
       {
           get { return System.Configuration.ConfigurationManager.AppSettings["mmbpriceDB"]; }
       }

       public static string mmbUpdateTempEConnectionString
       {
           get { return System.Configuration.ConfigurationManager.AppSettings["mmbUpdateTempEDB"]; }
       }

       public static string SpiderDBConnectionString
       {
           get { return System.Configuration.ConfigurationManager.AppSettings["LocalDB"]; }
           set { mmbDBConnectionString = value; }
       }
       public static string ZnmDBConnectionString
       {
           get { return System.Configuration.ConfigurationManager.AppSettings["znmDB"]; }
       }
       

       //   var dbFactory = new OrmLiteConnectionFactory(
    //ConnectionString,  
    //SqlServerDialect.Provider); 
    }
         
}
