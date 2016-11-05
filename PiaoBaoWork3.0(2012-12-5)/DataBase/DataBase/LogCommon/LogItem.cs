namespace DataBase.LogCommon
{
    using DataBase.Data;
    using System;

    internal class LogItem
    {
        public long BusinessElapsedMilliseconds;
        public string ConnectionString;
        public HashObject Data;
        public System.DateTime DateTime;
        public long ElapsedMilliseconds;
        public string IPAddress;
        public string Params;
        public string Url;
    }
}

