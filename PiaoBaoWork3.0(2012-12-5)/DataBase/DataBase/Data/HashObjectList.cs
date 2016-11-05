namespace DataBase.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    [Serializable]
    public class HashObjectList : List<IHashObject>, IHashObjectList, IList<IHashObject>, ICollection<IHashObject>, IEnumerable<IHashObject>, IEnumerable
    {
        private bool hasLongValue;
        [NonSerialized]
        private IDictionary<string, Hashtable> indexMap;

        public HashObjectList()
        {
        }

        public HashObjectList(int capacity) : base(capacity)
        {
        }

        public void Add(IHashObject item)
        {
            base.Add(item);
            if (this.indexMap != null)
            {
                this.AddIndexItem(item);
            }
        }

        public void Add(string[] keys, params object[] values)
        {
            HashObject item = new HashObject(keys, values);
            this.Add(item);
        }

        private void AddIndexItem(IHashObject item)
        {
            if (this.indexMap != null)
            {
                foreach (KeyValuePair<string, Hashtable> pair in this.indexMap)
                {
                    object obj2;
                    string key = pair.Key;
                    Hashtable hashtable = pair.Value;
                    if (item.TryGetValue(key, out obj2))
                    {
                        hashtable[obj2] = item;
                    }
                }
            }
        }

        public void AddRange(IEnumerable<IHashObject> collection)
        {
            base.AddRange(collection);
            this.indexMap = null;
        }

        private void BuildIndex(string key, Hashtable index)
        {
            this.hasLongValue = false;
            foreach (IHashObject obj2 in this)
            {
                object obj3;
                if (obj2.TryGetValue(key, out obj3))
                {
                    if ((!this.hasLongValue && (obj3 != null)) && (obj3.GetType() == typeof(long)))
                    {
                        this.hasLongValue = true;
                    }
                    index[obj3] = obj2;
                }
            }
        }

        public void Clear()
        {
            base.Clear();
            this.indexMap = null;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is HashObjectList))
            {
                return false;
            }
            HashObjectList list = (HashObjectList) obj;
            if (base.Count != list.Count)
            {
                return false;
            }
            for (int i = 0; i < base.Count; i++)
            {
                if (!base[i].Equals(list[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public IHashObject Find(string key, object value)
        {
            Hashtable index = this.GetIndex(key);
            IHashObject obj2 = (IHashObject) index[value];
            if (((obj2 == null) && (value != null)) && ((value.GetType() == typeof(ulong)) && this.hasLongValue))
            {
                long num = Convert.ToInt64(value);
                obj2 = (IHashObject) index[num];
            }
            return obj2;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private Hashtable GetIndex(string key)
        {
            Hashtable hashtable;
            if (this.indexMap == null)
            {
                this.indexMap = new Dictionary<string, Hashtable>();
            }
            if (!this.indexMap.TryGetValue(key, out hashtable))
            {
                hashtable = new Hashtable();
                this.indexMap[key] = hashtable;
                this.BuildIndex(key, hashtable);
            }
            return hashtable;
        }

        public IHashObjectList GetRange(int index, int count)
        {
            List<IHashObject> range = base.GetRange(index, count);
            IHashObjectList list2 = new HashObjectList(count);
            foreach (IHashObject obj2 in range)
            {
                list2.Add(obj2);
            }
            return list2;
        }

        public void Insert(int index, IHashObject item)
        {
            base.Insert(index, item);
            if (this.indexMap != null)
            {
                this.AddIndexItem(item);
            }
        }

        public void InsertRange(int index, IEnumerable<IHashObject> collection)
        {
            base.InsertRange(index, collection);
            this.indexMap = null;
        }

        public bool Remove(IHashObject item)
        {
            bool flag = base.Remove(item);
            if (this.indexMap != null)
            {
                this.RemoveIndexItem(item);
            }
            return flag;
        }

        public int RemoveAll(Predicate<IHashObject> match)
        {
            this.indexMap = null;
            return this.RemoveAll(match);
        }

        public void RemoveAt(int index)
        {
            if (this.indexMap != null)
            {
                this.RemoveIndexItem(base[index]);
            }
            base.RemoveAt(index);
        }

        private void RemoveIndexItem(IHashObject item)
        {
            if (this.indexMap != null)
            {
                foreach (KeyValuePair<string, Hashtable> pair in this.indexMap)
                {
                    object obj2;
                    string key = pair.Key;
                    Hashtable hashtable = pair.Value;
                    if (item.TryGetValue(key, out obj2))
                    {
                        hashtable.Remove(obj2);
                    }
                }
            }
        }

        public void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            this.indexMap = null;
        }

        public void Sort(string key, bool ascending)
        {
            base.Sort(delegate (IHashObject obj1, IHashObject obj2) {
                if (!ascending)
                {
                    return obj2.GetValue<IComparable>(key).CompareTo(obj1.GetValue<IComparable>(key));
                }
                return obj1.GetValue<IComparable>(key).CompareTo(obj2.GetValue<IComparable>(key));
            });
        }
    }
}

