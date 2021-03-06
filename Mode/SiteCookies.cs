﻿using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class SiteCookies
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string Domain { get; set; }
        public string Url { get; set; }
        public string Cookies { get; set; }
        public string UserAgent { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime CreateDate { get; set; }
    }
}