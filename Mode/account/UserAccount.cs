using System;
using ServiceStack.DataAnnotations;
namespace Mode.account
{
    public class UserAccount
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string NickName { get; set; }
        public string UserNo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Balance { get; set; }
        public decimal Position { get; set; }
        public decimal Availablefund { get; set; }
        public decimal Deficit { get; set; }
        public decimal Profit { get; set; }
        public decimal Transactionfees { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
