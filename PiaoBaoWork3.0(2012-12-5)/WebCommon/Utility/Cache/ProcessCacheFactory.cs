using System;
using System.Collections;

namespace PbProject.WebCommon.Utility.Cache
{
    public class ProcessCacheFactory
    {
        private static Hashtable _CacheContainer = null;

        private static void InitContainer()
        {
            if (_CacheContainer == null)
                _CacheContainer = Hashtable.Synchronized(new Hashtable());
            else
            {
                try 
                {
                    _CacheContainer.Clear();
                }
                catch { }
                finally { _CacheContainer = null; }
                
                _CacheContainer = Hashtable.Synchronized(new Hashtable());
            }
        }

        public static ICachedObject<T> GetCachedObject<T>(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            if (_CacheContainer == null)
            {
                InitContainer();
            }

            if (_CacheContainer.ContainsKey(name))
            {
                object result = _CacheContainer[name];
                return (ICachedObject<T>)result;
            }
            return null;
        }

        public static bool AppendCachedObject<T>(string name, ICachedObject<T> obj)
        {
            if (string.IsNullOrEmpty(name) || obj == null)
                throw new Exception("Name and Object cannot be null or empty.");

            if (_CacheContainer == null)
            {
                InitContainer();
            }

            _CacheContainer[name] = obj;

            return true;
        }
    }
}
