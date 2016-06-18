using System;
using System.Collections.Generic;
using DataBase;
using Mode;

namespace BLL
{
   public class SpiderConfigBll
    {
       public void SaveSpiderConfig(SpiderConfig spiderConfig)
       {
           new SpiderConfigDB().SaveSpiderConfig(spiderConfig);
       }

       /// <summary>
       /// 加载所有任务
       /// </summary>
       /// <returns></returns>
       public List<SpiderConfig> LoadSpiderconfig()
       {
           return new SpiderConfigDB().LoadSpiderconfig();
       }

       public SpiderConfig GetSpiderConfig(int id)
       {
           return new SpiderConfigDB().GetById(id);
       }
    }
}
