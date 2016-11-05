namespace DataBase.Data
{
    using DataBase.Utils;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;

    [Serializable]
    public class HashObject : Dictionary<string, object>, IHashObject, IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        public HashObject()
        {
        }

        public HashObject(IDictionary<string, object> dictionary) : base(dictionary)
        {
        }

        protected HashObject(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public HashObject(string[] keys, params object[] values)
        {
            InternalAdd(this, keys, values);
        }

        public void Add(string[] keys, params object[] values)
        {
            InternalAdd(this, keys, values);
        }

        internal static object ChangeType(object value, Type type)
        {
            if ((type == typeof(bool)) && (value.GetType() == typeof(string)))
            {
                return (((string) value) == "1");
            }
            return Convert.ChangeType(value, type, null);
        }

        public bool CheckSetValue(string key, object value)
        {
            if (base.ContainsKey(key))
            {
                return false;
            }
            this[key] = value;
            return true;
        }

        public IHashObject Clone()
        {
            HashObject obj2 = new HashObject();
            foreach (KeyValuePair<string, object> pair in this)
            {
                obj2.Add(pair.Key, pair.Value);
            }
            return obj2;
        }

        public void CopyTo(IDictionary<string, object> dict)
        {
            foreach (KeyValuePair<string, object> pair in this)
            {
                dict[pair.Key] = pair.Value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is HashObject))
            {
                return false;
            }
            HashObject obj2 = (HashObject) obj;
            if (base.Count != obj2.Count)
            {
                return false;
            }
            foreach (string str in base.Keys)
            {
                if (!obj2.ContainsKey(str))
                {
                    return false;
                }
                object obj3 = this[str];
                object obj4 = obj2[str];
                if (((obj3 != null) || (obj4 != null)) && !object.Equals(this[str], obj2[str]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public T GetValue<T>(string key)
        {
            return this.GetValue<T>(key, default(T));
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            object obj2;
            if (!base.TryGetValue(key, out obj2))
            {
                return defaultValue;
            }
            Type type = typeof(T);
            Type underlyingType = type;
            if ((obj2 != null) && ReflectionUtils.IsPrimitiveType(type, out underlyingType))
            {
                return (T) ChangeType(obj2, underlyingType);
            }
            if (((obj2 != null) && !(underlyingType == type)) && (underlyingType.IsEnum && !obj2.GetType().IsEnum))
            {
                return (T) Enum.ToObject(underlyingType, obj2);
            }
            if ((obj2 == null) && underlyingType.IsSubclassOf(typeof(ValueType)))
            {
                return defaultValue;
            }
            return (T) obj2;
        }

        private static void InternalAdd(HashObject obj, string[] keys, object[] values)
        {
            if (keys.Length != values.Length)
            {
                throw new InvalidOperationException("Keys和Values的长度不一致！");
            }
            if (keys.Length == 0)
            {
                throw new InvalidOperationException("添加数据必须有一项！");
            }
            for (int i = 0; i < keys.Length; i++)
            {
                obj.Add(keys[i], values[i]);
            }
        }

        public object this[string key]
        {
            get
            {
                object obj2;
                try
                {
                    obj2 = base[key];
                }
                catch (KeyNotFoundException)
                {
                    throw new KeyNotFoundException(string.Format("关键字“{0}”不在HashObject中。", key));
                }
                return obj2;
            }
            set
            {
                base[key] = value;
            }
        }
    }
}

