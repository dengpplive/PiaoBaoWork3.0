using System;
using System.Web;

namespace PbProject.WebCommon.Web.Cache
{
    public class IISSingleCachedObject<T>
    {
        private string _Name = "";

        public IISSingleCachedObject(string name)
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
        public static bool AppendCacheObject(string name, HttpContext context, T obj, int expireSeconds)
        {
            DateTime dtExpire = DateTime.Now.AddSeconds(expireSeconds);

            return AppendCacheObject(name, context, obj, dtExpire);
        }

        /// <summary>
        /// 添加Cache对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        /// <param name="dtExpire">如果大于当前时间则为过期时间点，否则不过期</param>
        /// <returns></returns>
        public static bool AppendCacheObject(string name, HttpContext context, T obj, DateTime dtExpire)
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
            catch { }
            return bRet;
        }

        public static T GetInstanceFromHttpContext(string name, HttpContext context)
        {
            T result = default(T);

            if (string.IsNullOrEmpty(name) || context == null)
                return result;

            name = name.Trim().ToLower();
            if (name.Length == 0)
                return result;

            try
            {
                object obj = context.Cache[name];
                if (obj != null)
                    result = (T)obj;
            }
            catch { }
            
            return result;
        }
    }
}
