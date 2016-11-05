using System;
using System.Collections;
using System.Collections.Generic;

namespace PbProject.WebCommon.Utility.Cache
{
    public class HashTableCachedObject<T> : ICachedObject<T>
    {
        protected string _Name = string.Empty;
        protected int _ExpireSeconds = 0;
        protected int _Capacity = 50;
        protected Hashtable _Items = null;

        public bool AutoRefreshWhenRead { get; set; }

        public HashTableCachedObject()
        {
            _Name = Guid.NewGuid().ToString().ToLower();
        }

        public HashTableCachedObject(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name cannot be empty.");
            //  强制为小写
            name = name.Trim().ToLower();
            if (name.Length == 0)
                throw new Exception("Name cannot be empty.");
            _Name = name;
        }

        public string Name
        { 
            get 
            {
                return _Name;
            } 
        }

        /// <summary>
        /// 记录过期时间(单位:秒);0 = 不过期; > 0 = 过期的秒数
        /// </summary>
        public int ExpireSeconds
        {
            get { return _ExpireSeconds; }
            set { _ExpireSeconds = value > 0 ? value : 0; }
        }

        public int Capacity
        {
            get { return _Capacity; }
            set { _Capacity = value > 0 ? value : 500; }
        }

        protected void InitItemTable()
        {
            if (_Items != null)
            {
                try { _Items.Clear(); }
                catch { }
                _Items = null;
            }
            _Items = Hashtable.Synchronized(new Hashtable(Capacity));
        }

        public static HashTableCachedObject<T> CreateInstance(string name, bool autoRefreshWhenRead, int expireSeconds, int capacity)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name cannot be null or empty.");

            HashTableCachedObject<T> result = new HashTableCachedObject<T>(name);
            result.AutoRefreshWhenRead = autoRefreshWhenRead;
            result.ExpireSeconds = expireSeconds;
            result.Capacity = capacity;
            return result;
        }

        public ICollection GetAllKeys()
        {
            return _Items.Keys;
        }

        public List<T> GetAllItems()
        {
            if (_Items == null)
                InitItemTable();

            List<T> result = null;
            if (_Items.Count < 1)
                return result;

            try 
            {
                lock (_Items.SyncRoot)
                {
                    if (_Items.Count < 1)
                        return result;

                    result = new List<T>(_Items.Count);
                    CacheObjectItem<T> cacheItem = null;
                    foreach (DictionaryEntry item in _Items)
                    {
                        cacheItem = (CacheObjectItem<T>)item.Value;
                        result.Add(cacheItem.GetValue());
                    }
                }
            }
            catch { }
            finally { }
            
            return result;
        }

        public T GetItem(object key)
        {
            if (_Items == null)
                InitItemTable();

            CacheObjectItem<T> obj = null;
            if (_Items.ContainsKey(key))
            {
                try
                {
                    obj = (CacheObjectItem<T>)_Items[key];
                    if (!obj.IsExpired)
                    {
                        //  如果每次自动刷新时间
                        if (AutoRefreshWhenRead)
                            obj.Refresh();
                        return obj.GetValue();
                    }
                    //  item已过期，删除
                    lock (_Items.SyncRoot)
                    {
                        _Items.Remove(key);   
                    } 
                }
                catch { }
            }
            return default(T);
        }

        public void SetItem(object key, T obj)
        {
            if (key == null || obj == null)
                return;

            if (_Items == null)
                InitItemTable();

            CacheObjectItem<T> objItem = CacheObjectItem<T>.GetObjectItem(obj, _ExpireSeconds);
            objItem.Refresh();
            _Items[key] = objItem;
        }

        /// <summary>
        /// Removes the element with the specified key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(object key)
        {
            try
            {
                _Items.Remove(key);  
            }
            catch { }
        }

        /// <summary>
        /// Remove all elements
        /// </summary>
        public void Clear()
        {
            try { _Items.Clear(); }
            catch { }
        }
    }
}
