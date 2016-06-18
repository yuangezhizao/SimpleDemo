using System;
using ServiceStack.DataAnnotations;
namespace Mode.account
{
    public class OrderInfo
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string NickName { get; set; }
        public int UserId { get; set; }
        public string StockNo { get; set; }
        public string StockName { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public decimal Balance { get; set; }
        public decimal Deficit { get; set; }
        public decimal Profit { get; set; }
        public decimal Transactionfees { get; set; }
        public string Directions { get; set; }
        public decimal Score { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
