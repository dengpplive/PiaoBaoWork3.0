using System;

namespace PbProject.Cache
{
    public class MemcacheControl
    {
        /// <summary>
        /// 获取基础数据缓存池
        /// </summary>
        /// <param name="systemID">系统ID</param>
        /// <returns></returns>
        public static IMemcacheAgent GetBasicMemcachePool(int systemID)
        {
            IMemcacheAgent result = null;
            switch (systemID % 2)
            {
                case 0:
                    result = MemcacheBasicA.GetInstance();
                    break;
                case 1:
                    result = MemcacheBasicB.GetInstance();
                    break;
            }
            return result;
        }
        /// <summary>
        /// 获取业务数据缓存池
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        public static IMemcacheAgent GetBusinessMemcachePool(int systemID)
        {
            IMemcacheAgent result = null;
            switch (systemID % 2)
            {
                case 0:
                    result = MemcacheBusinessA.GetInstance();
                    break;
                case 1:
                    result = MemcacheBusinessB.GetInstance();
                    break;
            }
            return result;
        }
    }
}
