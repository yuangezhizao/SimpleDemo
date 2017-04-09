using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class APIRequstHistroy
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string Summary { get; set; }
        public string RequestUrl { get; set; }
        public string Result { get; set; }
        public DateTime CreateTime { get; set; }
    }
}