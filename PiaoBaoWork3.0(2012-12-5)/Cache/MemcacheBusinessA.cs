using System;
using Memcached.ClientLibrary;

namespace PbProject.Cache
{
    /// <summary>
    /// 业务数据缓存A
    /// </summary>
    public class MemcacheBusinessA : IMemcacheAgent
    {
        private static MemcacheBusinessA instance;
        private static MemcachedClient mc;
        public MemcacheBusinessA()
        {
            string[] serverlist = { CacheServerDefines.BUSINESSCACHESERVER_A };
            SockIOPool pool = SockIOPool.GetInstance(CacheServerDefines.BUSINESSCACHESERVERNAME_A);
            pool.SetServers(serverlist);
            pool.InitConnections = 50;
            pool.MinConnections = 10;
            pool.MaxConnections = 50;
            pool.SocketConnectTimeout = 1000;
            pool.SocketTimeout = 3000;
            pool.MaintenanceSleep = 30;
            pool.Failover = true;
            pool.Nagle = false;
            pool.Initialize();
        }
        /// <summary>
        /// 初始化memcacheclient
        /// </summary>
        private static void InitMemcacheClient()
        {
            mc = new MemcachedClient();
            mc.PoolName = CacheServerDefines.BUSINESSCACHESERVERNAME_A;
            mc.EnableCompression = false;
        }
        public static MemcacheBusinessA GetInstance() 
        {
            if (instance == null)
                instance = new MemcacheBusinessA();
            if (mc == null)
                InitMemcacheClient();
            return instance;
        }

        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public bool SetValToCache<T>(string keyname, string identity, T val)
        {
            string key = string.Format("{0}_{1}", keyname, identity);
            return mc.Set(key, (object)val);
            // 关闭池，关闭sockets
            //SockIOPool.GetInstance().Shutdown();
        }
        public bool SetValToCache<T>(string keyname, int identity, T val)
        {
            string key = string.Format("{0}_{1}", keyname, identity);
            return mc.Set(key, (object)val);
            // 关闭池，关闭sockets
            //SockIOPool.GetInstance().Shutdown();
        }
        public bool SetValToCache<T>(string keyname, long identity, T val)
        {
            string key = string.Format("{0}_{1}", keyname, identity);
            return mc.Set(key, (object)val);
            // 关闭池，关闭sockets
            //SockIOPool.GetInstance().Shutdown();
        }

        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool SetValToCache<T>(string keyname, string identity, T val, DateTime expiry)
        {
            string key = string.Format("{0}_{1}", keyname, identity);
            return mc.Set(key, (object)val, expiry);
        }
        public bool SetValToCache<T>(string keyname, int identity, T val, DateTime expiry)
        {
            string key = string.Format("{0}_{1}", keyname, identity);
            return mc.Set(key, (object)val, expiry);
        }
        public bool SetValToCache<T>(string keyname, long identity, T val, DateTime expiry)
        {
            string key = string.Format("{0}_{1}", keyname, identity);
            return mc.Set(key, (object)val, expiry);
        }

        /// <summary>
        /// 从缓存中读取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T GetValFromCache<T>(string keyname, string identity)
        {
            string key = string.Format("{0}_{1}", keyname, identity);
            T obj = default(T);
            obj = (T)mc.Get(key);
            return obj;
        }
        public T GetValFromCache<T>(string keyname, int identity)
        {
            string key = string.Format("{0}_{1}", keyname, identity);
            T obj = default(T);
            obj = (T)mc.Get(key);
            return obj;
        }
        public T GetValFromCache<T>(string keyname, long identity)
        {
            string key = string.Format("{0}_{1}", keyname, identity);
            T obj = default(T);
            obj = (T)mc.Get(key);
            return obj;
        }

        /// <summary>
        /// 从缓存中删除
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool DelValFromCache(string keyname, string identity)
        {
            string key = string.Format("{0}_{1}", keyname, identity);
            return mc.Delete(key);
        }
        public bool DelValFromCache(string keyname, int identity)
        {
            string key = string.Format("{0}_{1}", keyname, identity);
            return mc.Delete(key);
        }
        public bool DelValFromCache(string keyname, long identity)
        {
            string key = string.Format("{0}_{1}", keyname, identity);
            return mc.Delete(key);
        }
    }
}
