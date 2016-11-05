namespace DataBase.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface IHashObjectList : IList<IHashObject>, ICollection<IHashObject>, IEnumerable<IHashObject>, IEnumerable
    {
        void Add(string[] keys, params object[] values);
        IHashObject Find(string key, object value);
        IHashObjectList GetRange(int index, int count);
        void Sort(string key, bool ascending);
    }
}

