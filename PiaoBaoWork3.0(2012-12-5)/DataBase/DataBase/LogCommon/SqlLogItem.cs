namespace DataBase.LogCommon
{
    using System;

    internal class SqlLogItem : LogItem
    {
        public string Error;
        public long ExecuteElapsedMilliseconds;
        public long FetchElapsedMilliseconds;
        public int RowCount;
        public string Sql;
    }
}

