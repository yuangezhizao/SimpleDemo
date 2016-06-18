using System.Collections.Generic;
using System.Linq;
using Mode;
using MongoDB.Driver.Builders;

namespace DataBase.Mongo
{
    public class SiteClassBandDb : BaseDB
    {
        public SiteClassBandDb() : base("SiteClassBand")
        {
        }

        public void Save(IEnumerable<SiteClassBand> catBands)
        {
            Collection.InsertBatch(catBands);
        }

        public List<SiteClassBand> FindBySiteId(int siteId)
        {
            var query =  Query.EQ("SiteID", siteId);
            return Collection.FindAs<SiteClassBand>(query).ToList();
        }
    }
}
