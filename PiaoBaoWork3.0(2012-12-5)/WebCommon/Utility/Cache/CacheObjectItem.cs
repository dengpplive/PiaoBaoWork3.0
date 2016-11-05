using System;

namespace PbProject.WebCommon.Utility.Cache
{
    public class CacheObjectItem<T>
    {
        private T _Object = default(T);
        private DateTime _CreateTime = DateTime.MinValue;
        private DateTime _LastRefreshTime = DateTime.MinValue;
        private int _ExpireSeconds = 0;
        
        /// <summary>
        /// 记录是否过期;true = expired
        /// </summary>
        public bool IsExpired
        {
            get { return _ExpireSeconds > 0 && _LastRefreshTime.AddSeconds(_ExpireSeconds) < DateTime.Now ? true : false; }
        }

        public CacheObjectItem(int expires)
        {
            _CreateTime = DateTime.Now;
            _ExpireSeconds = expires > 0 ? expires : 0;
        }
        public CacheObjectItem(T obj,int expires)
        {
            if (obj == null)
                throw new Exception("Object cannot be null.");
            _CreateTime = DateTime.Now;
            _ExpireSeconds = expires > 0 ? expires : 0;
            _Object = obj;
        }

        public void Refresh()
        {
            _LastRefreshTime = DateTime.Now;
        }

        public T GetValue()
        {
            return _Object;
        }

        public void SetValue(T obj)
        {
            _Object = obj;
        }

        public static CacheObjectItem<T> GetObjectItem(T obj,int expires)
        {
            return new CacheObjectItem<T>(obj,expires);
        }
    }
}
