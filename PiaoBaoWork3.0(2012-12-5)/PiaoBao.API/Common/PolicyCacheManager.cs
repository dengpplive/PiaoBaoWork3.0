using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;


namespace PiaoBao.API.Common
{
    public class PolicyCacheManager
    {
        static Cache cache = HttpRuntime.Cache;
        public static void Set(string key, object data)
        {
            cache.Insert(key, data, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
        }
        public static object Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            return cache.Get(key);
        }
        
    }
}