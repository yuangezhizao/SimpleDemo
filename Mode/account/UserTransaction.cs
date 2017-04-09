using System;
using ServiceStack.DataAnnotations;

namespace Mode.account
{
    public class UserTransaction
    {
        [AutoIncrement]
        public int Id { get; set; }

        public string NickName { get; set; }
        public int UserId { get; set; }
        public string StockName { get; set; }
        public string StockNo { get; set; }
        public int StockCount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Positions { get; set; }
        public int SellType { get; set; }
        public decimal Transactionfees { get; set; }
        public decimal StopProfit { get; set; }
        public decimal StopLoss { get; set; }
        public decimal Score { get; set; }
        public string Argument { get; set; }
        public int OrderId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}