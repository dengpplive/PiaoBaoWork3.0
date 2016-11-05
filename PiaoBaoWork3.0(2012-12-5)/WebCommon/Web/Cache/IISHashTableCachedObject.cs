using System;
using System.Web;
using PbProject.WebCommon.Utility.Cache;

namespace PbProject.WebCommon.Web.Cache
{
    public class IISHashTableCachedObject<T>:HashTableCachedObject<T>
    {
        public IISHashTableCachedObject(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name cannot be empty.");
            //  强制为小写
            name = name.Trim().ToLower();
            if (name.Length == 0)
                throw new Exception("Name cannot be empty.");
            _Name = name;
        }

        /// <summary>
        /// 添加Cache对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        /// <param name="dtExpire">如果大于当前时间则为过期时间点，否则不过期</param>
        /// <returns></returns>
        private static bool AppendCacheObject(string name, HttpContext context, HashTableCachedObject<T> obj, DateTime dtExpire)
        {
            bool bRet = false;
            
            if (context == null || string.IsNullOrEmpty(name))
                return bRet;
            try
            {
                if (context.Cache[name] != null)
                {
                    context.Cache.Remove(name);
                }
                if (dtExpire <= DateTime.Now)
                    context.Cache.Add(name, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                else
                    context.Cache.Add(name, obj, null, dtExpire, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                bRet = true;
            }
            catch {  }
            return bRet;
        }

        /// <summary>
        /// 从HttpContext中获取实例,不存在自动创建;CachedObject中的内容不过期
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IISHashTableCachedObject<T> GetInstanceFromHttpContext(string name, HttpContext context)
        {
            return GetInstanceFromHttpContext(name, context, true, false, 0, 0);
        }

        public static IISHashTableCachedObject<T> GetInstanceFromHttpContext(string name, HttpContext context, bool autoCreateWhenNotExists, bool autoRefreshWhenRead, int expireSeconds, int capacity)
        {
            IISHashTableCachedObject<T> result = null;

            if (string.IsNullOrEmpty(name) || context == null)
                return result;

            name = name.Trim().ToLower();
            if (name.Length == 0)
                return result;

            try
            {
                object obj = context.Cache[name];
                if (obj != null)
                    result = (IISHashTableCachedObject<T>)obj;
                else if (autoCreateWhenNotExists)
                {
                    IISHashTableCachedObject<T> cachedObj = new IISHashTableCachedObject<T>(name);
                    cachedObj.Capacity = capacity;
                    cachedObj.ExpireSeconds = expireSeconds;
                    cachedObj.AutoRefreshWhenRead = autoRefreshWhenRead;
                    //  CachedObject为容器，不过期
                    if (AppendCacheObject(name, context, cachedObj, DateTime.Now.AddHours(-1)))
                        result = cachedObj;
                }
            }
            catch { }
            
            return result;
        }
    }
}
