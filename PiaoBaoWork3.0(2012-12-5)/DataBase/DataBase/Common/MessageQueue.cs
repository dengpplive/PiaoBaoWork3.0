namespace DataBase.Common
{
    using System;
    using System.Collections;
    using System.Threading;
    using System.Web;

    internal class MessageQueue
    {
        private static Queue queue = new Queue();
        private static ManualResetEvent waitHandle = new ManualResetEvent(false);

        public void AddMessage(object message)
        {
            lock (queue)
            {
                queue.Enqueue(message);
                waitHandle.Set();
            }
        }

        public ArrayList WaitFor(HttpResponse response)
        {
            while (!waitHandle.WaitOne(10, false))
            {
                if (!response.IsClientConnected)
                {
                    return null;
                }
            }
            ArrayList list = new ArrayList();
            lock (queue)
            {
                while (queue.Count > 0)
                {
                    list.Add(queue.Dequeue());
                }
            }
            waitHandle.Reset();
            return list;
        }
    }
}

