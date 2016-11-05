namespace DataBase.LogCommon
{
    using DataBase.Common;
    using System;
    using System.Collections;
    using System.Web;

    internal static class SqlLog
    {
        private static Hashtable queues = Hashtable.Synchronized(new Hashtable());

        private static MessageQueue GetQueue(string queueId)
        {
            MessageQueue queue = (MessageQueue) queues[queueId];
            if (queue == null)
            {
                queue = new MessageQueue();
                queues.Add(queueId, queue);
            }
            return queue;
        }

        internal static string GetQueueId(HttpContext context)
        {
            if (((context != null) && (context.Session != null)) && !context.Request.IsLocal)
            {
                return context.Session.SessionID;
            }
            return "localhost";
        }

        internal static void Log(string message, DateTime startTime, long executeElapsedMilliseconds, int rowCount, long fetchElapsedMilliseconds, long elapsedMilliseconds)
        {
            ScriptLogItem item = new ScriptLogItem {
                message = message,
                startTime = startTime,
                executeElapsedMilliseconds = (int) executeElapsedMilliseconds,
                fetchElapsedMilliseconds = (int) fetchElapsedMilliseconds,
                rowCount = rowCount,
                elapsedMilliseconds = (int) elapsedMilliseconds
            };
            GetQueue(GetQueueId(HttpContext.Current)).AddMessage(item);
        }

        internal static object WaitFor(string queudId, HttpResponse response)
        {
            return GetQueue(queudId).WaitFor(response);
        }
    }
}

