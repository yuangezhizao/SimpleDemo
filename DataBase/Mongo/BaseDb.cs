using System;
using Commons;
using MongoDB.Driver;

namespace DataBase.Mongo
{
    public class BaseDB
    {

        public const string connentString = "mongodb://sa:1q2w3e@192.168.1.218";
        public BaseDB(string collention)
        {
            var client = new MongoClient(connentString);
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase("mmbCloudMongoDB");
           
            //获得Users集合,如果数据库中没有，先新建一个
            Collection = db.GetCollection(collention);
        }
     

        public MongoCollection Collection { get; set; }

        public void Insert<T>(T obj)
        {
            try
            {
                //执行插入操作
                Collection.Insert<T>(obj);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "mongoDB");
            }
        }

        //public void Update(IMongodbModelUpdate obj)
        //{
        //    try
        //    {
        //        var client = new MongoClient(connentString);
        //        MongoServer server = client.GetServer();
        //        MongoDatabase db = server.GetDatabase(dbName);
        //        if (string.IsNullOrEmpty(collention))
        //            collention = obj.ToString().Replace('.', '_');

        //        MongoCollection col = db.GetCollection(collention);

        //        var update = obj.ToDocument();

        //        //执行更新操作

        //        col.Update(Query.EQ("key", obj.key), update);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogServer.WriteLog(ex, "mongoDB");
        //    }

        //}
    }
}
