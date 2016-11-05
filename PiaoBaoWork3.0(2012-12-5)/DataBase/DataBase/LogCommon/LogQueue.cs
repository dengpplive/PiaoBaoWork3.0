namespace DataBase.LogCommon
{
    using DataBase.Configuration;
    using DataBase.Data;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading;
    using System.Web;
    using System.Web.SessionState;

    internal static class LogQueue
    {
        private const string cLogItemKey = "__LogItem";
        public static readonly string ConnectionString = null;
        private static string connectionStringSessionVar = null;
        private static bool logEnabled = true;
        private static string[] logSessionFieldList = null;
        private static string[] logSessionVarList = null;
        public static readonly Queue<LogItem> Queue = new Queue<LogItem>();
        public static readonly ManualResetEvent WaitHandle = new ManualResetEvent(false);

        static LogQueue()
        {
            ConnectionString = Settings.LogModuleConnectionString;
            if (string.IsNullOrEmpty(ConnectionString))
            {
                ConnectionString = ConfigurationManager.AppSettings["LogModule.DbConn"];
            }
            logEnabled = !string.IsNullOrEmpty(ConnectionString);
            connectionStringSessionVar = ConfigurationManager.AppSettings["LogModule.ConnectionStringSessionVar"];
            string str = ConfigurationManager.AppSettings["LogModule.LogSessionVar"];
            if (!string.IsNullOrEmpty(str))
            {
                string[] strArray = str.Split(new char[] { ',' });
                if (strArray.Length > 0)
                {
                    logSessionVarList = new string[strArray.Length];
                    logSessionFieldList = new string[strArray.Length];
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        string str2;
                        string str3;
                        string str4 = strArray[i].Trim();
                        string[] strArray2 = str4.Split(new char[] { '=' });
                        if (strArray2.Length == 1)
                        {
                            str2 = str4;
                            str3 = str4;
                        }
                        else
                        {
                            str2 = strArray2[0].Trim();
                            str3 = strArray2[1].Trim();
                        }
                        logSessionFieldList[i] = str2;
                        logSessionVarList[i] = str3;
                    }
                }
            }
        }

        public static void Add(LogItem item)
        {
            if (logEnabled)
            {
                lock (Queue)
                {
                    Queue.Enqueue(item);
                }
                WaitHandle.Set();
            }
        }

        public static LogItem GetLogItem(HttpContext context)
        {
            return (LogItem) context.Items["__LogItem"];
        }

        public static void LogSessionVar(LogItem item, HttpSessionState session)
        {
            if (session != null)
            {
                object obj2;
                if (!string.IsNullOrEmpty(connectionStringSessionVar))
                {
                    obj2 = session[connectionStringSessionVar];
                    if (obj2 != null)
                    {
                        item.ConnectionString = (string) obj2;
                    }
                }
                if (logSessionVarList != null)
                {
                    if (item.Data == null)
                    {
                        item.Data = new HashObject();
                    }
                    for (int i = 0; i < logSessionVarList.Length; i++)
                    {
                        string str = logSessionVarList[i];
                        obj2 = session[str];
                        if (obj2 != null)
                        {
                            string str2 = logSessionFieldList[i];
                            item.Data[str2] = obj2;
                        }
                    }
                }
            }
        }

        public static void SetLogItem(HttpContext context, LogItem value)
        {
            context.Items["__LogItem"] = value;
        }
    }
}

