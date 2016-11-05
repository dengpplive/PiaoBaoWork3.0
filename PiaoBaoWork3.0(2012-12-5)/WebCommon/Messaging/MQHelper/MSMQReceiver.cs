using System;
using System.Messaging;

namespace PbProject.WebCommon.Messaging.MQHelper
{
    public class MSMQReceiver
    {
        public event ProcessMessageHandle OnMessageArrivedHandle = null;

        private MSMQAgent _MQAgent = null;

        public MSMQReceiver(string mqPath)
        {
            InitReceiver(mqPath);
        }

        protected void InitReceiver(string mqPath)
        {
            _MQAgent = MSMQAgent.GetMSMQAgent(mqPath);

            if (_MQAgent == null || _MQAgent.GetQueue() == null)
                throw new Exception("Init receiver failed.");

            if (_MQAgent.GetQueue() == null)
                throw new Exception("Init receiver's queue failed.");
        }

        public void RegisterMessageHandle(ProcessMessageHandle handle)
        {
            this.OnMessageArrivedHandle += handle;
        }

        public bool StartReceiver()
        {
            MessageQueue myQueue = null;
            bool result = false;
            try
            {
                myQueue = _MQAgent.GetQueue();
                if (myQueue == null)
                    return result;
                //异步
                myQueue.PeekCompleted += new PeekCompletedEventHandler(PeekCompleted);
                myQueue.BeginPeek();
                result = true;
            }
            catch { }
            finally { }
            return result;

        }
        public void DisposeMessage()
        {
            _MQAgent.Dispose();
        }

        protected void PeekCompleted(object sender, PeekCompletedEventArgs e)
        {
            System.Messaging.Message msg = null;
            MessageQueue mq = null;

            try
            {
                mq = (MessageQueue)sender;
                mq.Formatter = new BinaryMessageFormatter();
                System.Messaging.Message m = mq.EndPeek(e.AsyncResult);
                mq.ReceiveById(m.Id);
                if (OnMessageArrivedHandle != null)
                {
                    OnMessageArrivedHandle(m.Label, m.Body.ToString());
                }
            }
            catch (Exception ex) { string err = ex.Message; }
            finally
            {
                if (msg != null)
                {
                    try { msg.Dispose(); }
                    catch { }
                    finally { msg = null; }
                }
                //接收下一次事件
                if (mq != null)
                    mq.BeginPeek();
            }
        }

    }
}
