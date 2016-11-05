using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PbProject.WebCommon.Web.Cache
{
    public class CacheManage
    {
        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="cachename">缓存名称</param>
        /// <param name="data">缓存数据</param>
        /// <param name="minutes">缓存时间分钟</param>
        /// <param name="option">名称标识</param>
        public void SetCacheData(string cachename, Object data, int minutes, string option)
        {
            cachename = cachename + option;
            if (minutes == -1)
            {
                System.Web.HttpRuntime.Cache.Insert(cachename, data, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.Zero);
            }
            else
            {
                System.Web.HttpRuntime.Cache.Insert(cachename, data, null, DateTime.UtcNow.AddMinutes(minutes), TimeSpan.Zero);
            }
        }
        /// <summary>
        ///  获取缓存
        /// </summary>
        /// <param name="CacheName">缓存名称</param>
        /// <param name="option">名称标识</param>
        /// <returns></returns>
        public Object GetCacheData(string CacheName, string option)
        {
            CacheName = CacheName + option;
            Object CacheData = null;
            if (System.Web.HttpRuntime.Cache[CacheName] == null)
            {
                CacheData = null;
            }
            else
            {
                CacheData = System.Web.HttpRuntime.Cache[CacheName];
            }
            return CacheData;
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            System.Collections.IDictionaryEnumerator idic = HttpRuntime.Cache.GetEnumerator();
            while (idic.MoveNext())
            {
                HttpRuntime.Cache.Remove(idic.Key.ToString());
            }
        }
    }
}
