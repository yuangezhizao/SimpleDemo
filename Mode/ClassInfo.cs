using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class ClassInfo
    {
         [AutoIncrement]
        public int Id { get; set; }

        public string CatName { get; set; }
        
        public int ParentId { get; set; }

        public string ParentName { get; set; }

        public string CatCrumbleIds { get; set; }

        public string CatCrumbleNames { get; set; }

        public bool HasChild { get; set; }

        public bool IsHide { get; set; }

        public int Sort { get; set; }

        public int Level { get; set; }

        public bool IsDel { get; set; }

        public string SEOWords { get; set; }

        public string SpellWord { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateTime { get; set; }

        
    }
}
