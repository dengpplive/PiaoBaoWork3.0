namespace DataBase.Common
{
    using System;

    public sealed class ScriptLogItem
    {
        public int elapsedMilliseconds;
        public int executeElapsedMilliseconds;
        public int fetchElapsedMilliseconds;
        public string message;
        public int rowCount;
        public DateTime startTime;
    }
}

