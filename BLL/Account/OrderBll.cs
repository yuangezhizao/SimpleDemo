using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using DataBase.Account;
using Mode.account;

namespace BLL.Account
{
   public class OrderBll
    {
        public int SaveOrderInfo(OrderInfo order)
        {
            try
            {
                return new OrderDb().SaveOrderInfo(order);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                return 0;
            }
           
        }
        public OrderInfo GetById(int id)
        {
            try
            {
                return new OrderDb().GetById(id);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                return null;
            }
           
        }
        public void UpdateOrderInfo(OrderInfo order)
        {
            try
            {
                new OrderDb().UpdateOrderInfo(order);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
            }
            
        }
    }
}




