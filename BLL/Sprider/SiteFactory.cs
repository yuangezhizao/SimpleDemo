using BLL.Sprider;
using BLL.Sprider.classInfo;
using BLL.Sprider.Comments;
using BLL.Sprider.ProList;
using BLL.Sprider.ProList.Api;
using BLL.Sprider.SitePromo;
using BLL.Sprider.SmsApi;
using BLL.Sprider.Stock;
using Mode;


namespace BLL
{
    public class SiteFactory
    {
        public int SiteId { get; set; }

        /// <summary>
        /// 商城类别
        /// </summary>
        public  ISiteClassBLL SiteClassManager
        {
            get
            {
                switch (SiteId)
                {
                    case 1:
                        return new JdClassInfo();
                    case 2:
                        return new YiwugouClassInfo();
                    case 3:
                        return new DangdangClassInfo();
                    case 4:
                        return new AmazonClassInfo();
                    case 6:
                        return new SuningClassInfo();
                    case 8:
                        return new GmClassInfo();
                    case 9:
                        return new NewEggClassInfo();
                    case 10:
                        return new TmallClassInfo();
                    case 11:
                        return new YiXunClassInfo();
                    case 13:
                        return new YhdClassInfo();
                    case 30:
                        return new TaoXieClass();
                    case 33:
                        return new OkBuyClassInfo();
                    case 36:
                        return new VipClassInfo();
                    case 42:
                        return new WoMaiClassInfo();
                    case 43:
                        return new LeFengClassInfo();
                    case 52:
                        return new JuMeiClassInfo();
                    case 61:
                        return new SfbestClassInfo();
                    case 123:
                        return new OkHqbInfo();
                    case 127:
                        return new UnieclubClassInfo();
                    case 129:
                        return new JianyiClass();
                    case 135:
                        return new YesmywineClassInfo();
                    case 142:
                        return new HuaweiGwClass();
                    case 161:
                        return new FeiniuClassInfo();
                    case 166:
                        return new HangowaClassInfo();
                    case 169:
                        return new LbxcnClassInfo();
                    case 170:
                        return new KfzjClassInfo();
                    case 177:
                        return new GjmjClassInfo();
                    case 181:
                        return new VmeiClassInfo();
                    case 185:
                        return new SundianClassInfo();
                    case 189:
                        return new CnbuyersClassInfo();
                    case 194:
                        return new HaierClassInfo();
                    case 196:
                        return new ShopRobamClassInfo();
                    case 243:
                        return new YIerYaoClass();
                    case 246:
                        return new YmatouClassInfo();
                    case 248:
                        return new YiyaoClassInfo();
                    case 256:
                        return new KadClassInfo();
                    case 257:
                        return new YaofangwangClassinfo();
                    case 258:
                        return new QlkClassInfo();
                    case 259:
                        return new JxdyfClassInfo();
                    case 261:
                        return new MiaClassInfo();
                    case 264:
                        return new EhaoyaoClassinfo();
                    case 265:
                        return new KaixinrenClassInfo();
                    case 268:
                        return new BaoshuiguojiClassInfo();
                    case 272:
                        return new KjtClassinfo();
                  case 276:
                        return new ZmmmClassInfo();
                
                    default:
                        return null;
                }


            }
        }
       /// <summary>
       /// 商城活动优惠信息
       /// </summary>
        public ISitePromo SitePromoManager
        {
            get
            {
                switch (SiteId)
                {
                    case 8:
                        return new GmSitePromo();
                }
                return null;

            }
        }
        /// <summary>
        /// 商品
        /// </summary>
        public IProList ProListManager
        {
            get
            {
                switch (SiteId)
                {
                    case 1:
                        return new JdProList();
                    case 4:
                        return new AmazonList();
                    case 8:
                        return new GmProList();
                    case 13:
                        return new YhdList();
                }
                return null;

            }
        }

        public IApiProList ProIApiManager
        {
            get
            {
                switch (SiteId)
                {
                    case 241:
                        return new KaolaiProductsApi();
                    case 266:
                        return new MeTaoProductsApi();
                    case 2:
                        return new YiWuGouProList();
                    case 999:
                        return new SfdaProList();
                }
                return null;

            }
        }

        public IStockInfoBll StockInfoManager
        {
            get { return new StockInfoBll(); }
        }

        /// <summary>
        /// 爬取评论
        /// </summary>
        public IComments CommentsManager
        {
            get
            {
                switch (SiteId)
                {
                    case 1:
                        return new JdComments();
                    case 4:
                        return new AmazonComments();
                    case 6:
                        return new SuningComments();
                    case 8:
                        return new GmComments();
                    case 11:
                        return new YixunComments();
                    case 13:
                        return new YhdComments();
                    default:
                        return null;
                }
                

            }


        }

        public ISmsApi SmsApiManager
        {
            get
            {
                return new Hellotrue(new SmsManger
                {
                    ApiUrl = "http://api.hellotrue.com/api/do.php",
                    UserName = "api-rmm0nm29",
                    ServerName = "Hellotrue",
                    Pwd = "62415109"
                    
                });
            }
        }
    }
}
