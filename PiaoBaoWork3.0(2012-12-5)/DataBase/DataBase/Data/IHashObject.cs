namespace DataBase.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface IHashObject : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        void Add(string[] keys, params object[] values);
        bool CheckSetValue(string key, object value);
        IHashObject Clone();
        void CopyTo(IDictionary<string, object> dict);
        T GetValue<T>(string key);
        T GetValue<T>(string key, T defaultValue);
    }
}

