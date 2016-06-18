using Mode.account;
using ServiceStack.OrmLite;

namespace DataBase.Account
{
    public class OrderDb : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public OrderDb()
        {
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<OrderInfo>();
            }
        }

        public int SaveOrderInfo(OrderInfo order)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.Save(order);
                return order.Id;
            }
        }
        public OrderInfo GetById(int id)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                return db.SingleById<OrderInfo>(id);
            }
        }
        public void UpdateOrderInfo(OrderInfo order)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.Update(order);
            }

        }


    }
}
