using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Collections;
using System.Text.RegularExpressions;

namespace Commons
{
    /// <summary>
    /// 模拟浏览器记录cookie信息
    /// </summary>
    public class SimulationCookie
    {
        private static Cache RunCache
        {
            get { return HttpRuntime.Cache; }
        }

        /// <summary>
        /// 设置当前域名下的cookie 信息
        /// </summary>
        /// <param name="host"></param>
        /// <param name="cookies"></param>
        public static void SetCookie(string host, CookieCollection cookies)
        {
            try
            {
                if (cookies != null && cookies.Count > 0)
                {
                    lock (GetLock(host))
                    {
                        if (RunCache[host] != null)
                        {
                            RunCache.Remove(host);
                        }
                        List<Cookie> clist = new List<Cookie>();
                        foreach (Cookie item in cookies)
                        {
                            if (!item.Expired)
                            {
                                clist.Add(item);
                            }
                        }
                        RunCache.Add(host, clist, null, DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 获取当前域下的cookie信息
        /// </summary>
        /// <param name="host"></param>
        /// <param name="setcookie"></param>
        public static void GetCookie(string host, Action<Cookie> setcookie)
        {
            try
            {
                if (setcookie == null)
                {
                    return;
                }
                var domain = Regex.Match(host, "\\.(?<x>.*\\..*?)$").Groups["x"].Value;
                lock (GetLock(host))
                {
                    int count = 0;
                    foreach (DictionaryEntry de in RunCache)
                    {
                        var key = (de.Key as string);
                        if (!string.IsNullOrEmpty(key) && key.Contains(domain))
                        {
                            List<Cookie> clist;
                            if ((clist = de.Value as List<Cookie>) != null && clist.Count > 0)
                            {
                                foreach (var item in clist)
                                {
                                    if (!item.Expired)
                                    {
                                        count++;
                                        setcookie(item);
                                    }
                                }
                            }
                        }
                    }
                    if (count == 0)
                    {
                        //默认传递一个cookie信息
                        setcookie(new Cookie { Name = "Hm_lvt_" + Guid.NewGuid().ToString("N"), Domain = host, Value = Guid.NewGuid().ToString(), Expires = DateTime.Now.AddDays(1) });
                        setcookie(new Cookie { Name = Guid.NewGuid().ToString("N"), Domain = host, Value = Guid.NewGuid().ToString(), Expires = DateTime.Now.AddDays(1) });
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 锁对象集合
        /// </summary>
        private static Dictionary<string, object> _lockDict = new Dictionary<string, object>();
        /// <summary>
        /// 根据缓存名称获取锁对象
        /// </summary>
        /// <param name="key">缓存名称</param>
        /// <returns>缓存名称对应的锁对象</returns>
        public static object GetLock(string key)
        {
            if (_lockDict.ContainsKey(key))
            {
                return _lockDict[key];
            }
            lock (_lockDict)
            {
                if (_lockDict.ContainsKey(key))
                {
                    return _lockDict[key];
                }
                object tempL = new object();
                _lockDict[key] = tempL;
                return tempL;
            }
        }
    }
}
