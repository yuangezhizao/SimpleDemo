using ServiceStack.DataAnnotations;
using System;
namespace Mode
{
    public class SiteClassInfo
    {
          [AutoIncrement]
        public int Id { get; set; }

        public int SiteId { get; set; }

        public string ClassId { get; set; }

        public string ClassName { get; set; }

        public string ParentClass { get; set; }

        public string ParentName { get; set; }

        public string ClassCrumble { get; set; }

        public string ParentUrl { get; set; }

        public string Urlinfo { get; set; }

        public bool HasChild { get; set; }

        public int TotalProduct { get; set; }

        public bool IsBind { get; set; }

        public bool IsDel { get; set; }

        public int BindClassId { get; set; }

        public string BindClassName { get; set; }

        public bool IsHide { get; set; }

        public DateTime UpdateTime { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
