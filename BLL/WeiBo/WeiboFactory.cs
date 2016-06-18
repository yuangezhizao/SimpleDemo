namespace BLL.WeiBo
{
    public class WeiboFactory
    {
        public string Domain { get; set; }

 


        public IWeiboBll WeiboManager
        {
            get
            {
                if (Domain == null)
                    return null;
                switch (Domain.ToLower().Trim())
                {
                    case "sina":
                        return new SinalweiboBll();
                    case "tengxun":
                        return new TengXunBll();

                }
                return null;

            }
        }

        public  void Auth(string code)
        {
            WeiboManager.getAccessToken(code);
        }
    }
}
