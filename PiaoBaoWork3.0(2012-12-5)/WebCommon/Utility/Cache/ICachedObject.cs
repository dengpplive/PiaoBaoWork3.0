using System.Collections.Generic;

namespace PbProject.WebCommon.Utility.Cache
{
    public interface ICachedObject<T>
    {
        /// <summary>
        /// true = 访问Item时刷新读取时间
        /// </summary>
        bool AutoRefreshWhenRead { get; set; }
        /// <summary>
        /// 存在CacheContainer中的唯一标识
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 记录过期时间(单位:秒);0 = 不过期; > 0 = 过期的秒数
        /// </summary>
        int ExpireSeconds { get; set; }
        /// <summary>
        /// 初始化容量
        /// </summary>
        int Capacity { get; set; }
        /// <summary>
        /// 获取全部元素
        /// </summary>
        /// <returns></returns>
        List<T> GetAllItems();
        /// <summary>
        /// 获取元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetItem(object key);
        /// <summary>
        /// 写入元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        void SetItem(object key, T obj);
    }
}
