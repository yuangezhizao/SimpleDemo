using System.Collections.Generic;
using System.Linq;
using Mode;
using MongoDB.Driver.Builders;

namespace DataBase.Mongo
{
    public class SiteBandDb : BaseDB
    {
        public SiteBandDb()
            : base("SiteBandInfo")
        {
        }

        public void Save(IEnumerable<SiteBandInfo> siteBands)
        {
            Collection.InsertBatch(siteBands);
        }

        public List<SiteBandInfo> FindBySiteId(int siteId)
        {
            var query =  Query.EQ("SiteID", siteId);
            return Collection.FindAs<SiteBandInfo>(query).ToList();
        }
    }
}
