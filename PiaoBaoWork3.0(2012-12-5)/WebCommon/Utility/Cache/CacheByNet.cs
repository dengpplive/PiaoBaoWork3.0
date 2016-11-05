using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace PbProject.WebCommon.Utility.Cache
{
    public class CacheByNet
    {
        public CacheByNet()
        { 
        
        }
        /// <summary>
        /// 获取缓存
        /// </summary> 
        /// <param name="cachename">缓存名称</param> 
        /// <returns></returns>
        public DataSet GetCacheData(string cachename)
        {
            DataSet ds = new DataSet();
            if (System.Web.HttpRuntime.Cache[cachename] == null || System.Web.HttpRuntime.Cache[cachename].ToString() == "")
            {
                ds = null;
            }
            else
            {
                ds = System.Web.HttpRuntime.Cache[cachename] as DataSet;
            }
            return ds;
        }

        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="cachename">缓存名称</param>
        public void SetCacheData(DataSet ds, string cachename, int absolutExpirationTime)
        {
            System.Web.HttpRuntime.Cache.Insert(cachename, ds, null, DateTime.Now.AddMinutes(absolutExpirationTime), System.Web.Caching.Cache.NoSlidingExpiration);
        }
    }
}
