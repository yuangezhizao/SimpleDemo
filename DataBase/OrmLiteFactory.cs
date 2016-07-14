namespace DataBase
{
   public class OrmLiteFactory
   {
       private const string Connent = @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\App_Data\SiteBase.db;Integrated Security=True;User Instance=True";

       public static string ConnectionString
       {
           get
           {
               if (System.Configuration.ConfigurationManager.AppSettings["ConnectionString"] == null)
               {
                   return Connent;
               }
               return System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
           }

       }

       public static string MmbDbConnectionString {
           get { return System.Configuration.ConfigurationManager.AppSettings["mmbDB"]; } }

       public static string MmbpriceDbConnectionString
       {
           get { return System.Configuration.ConfigurationManager.AppSettings["mmbpriceDB"]; }
       }

       public static string MmbUpdateTempEConnectionString
       {
           get { return System.Configuration.ConfigurationManager.AppSettings["mmbUpdateTempEDB"]; }
       }

       public static string SpiderDbConnectionString
       {
           get { return System.Configuration.ConfigurationManager.AppSettings["LocalDB"]; }
          
       }
       public static string ZnmDbConnectionString
       {
           get { return System.Configuration.ConfigurationManager.AppSettings["znmDB"]; }
       }
       

       //   var dbFactory = new OrmLiteConnectionFactory(
    //ConnectionString,  
    //SqlServerDialect.Provider); 
    }
         
}
