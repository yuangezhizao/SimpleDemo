using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase;
using Mode;

namespace BLL
{
    public class SiteCookiesBll
    {
        public void SaveCookies(SiteCookies cookies)
        {
            new DomainCookiesDb().SaveCookies(cookies);
        }
        public SiteCookies GetOneByDomain(string domain)
        {
            return new DomainCookiesDb().GetOneByDomain(domain);
        }
    }
}
