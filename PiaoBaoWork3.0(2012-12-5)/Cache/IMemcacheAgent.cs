using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PbProject.Cache
{
    public interface IMemcacheAgent
    {
        /// <summary>
        /// 写入值到缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetValToCache<T>(string keyname, string identity, T val);
        bool SetValToCache<T>(string keyname, int identity, T val);
        bool SetValToCache<T>(string keyname, long identity, T val);
        /// <summary>
        /// 指定过期时间写入值到缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        bool SetValToCache<T>(string keyname, string identity, T val, DateTime expiry);
        bool SetValToCache<T>(string keyname, int identity, T val, DateTime expiry);
        bool SetValToCache<T>(string keyname, long identity, T val, DateTime expiry);
        /// <summary>
        /// 从缓存中读取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetValFromCache<T>(string keyname, string identity);
        T GetValFromCache<T>(string keyname, int identity);
        T GetValFromCache<T>(string keyname, long identity);
        /// <summary>
        /// 从缓存中删除值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool DelValFromCache(string keyname, string identity);
        bool DelValFromCache(string keyname, int identity);
        bool DelValFromCache(string keyname, long identity);
    }
}
