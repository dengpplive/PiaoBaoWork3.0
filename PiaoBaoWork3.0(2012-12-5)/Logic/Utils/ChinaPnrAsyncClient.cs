using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace PbProject.ConsoleServerProc.Utils
{
    public class StateObject
    {
        public Socket WorkSocket = null;
        public const int BufferSize = 512;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder ResponseData = new StringBuilder();
    }

    /// <summary>
    /// 异步连接，目前不赞成使用
    /// </summary>
    public class ChinaPnrAsyncClient
    {
        private ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private ManualResetEvent receiveDone =
            new ManualResetEvent(false);
        private String response = String.Empty;

        private Socket client = null;
        private string serverIP = null;
        private int port = 6180;

        /// <summary>
        /// 默认端口6180
        /// </summary>
        /// <param name="serverIP"></param>
        public ChinaPnrAsyncClient(string serverIP)
        {
            if (serverIP.IndexOf(':') > 0)
            {
                string[] ipAndPort = serverIP.Split(':');
                this.serverIP = ipAndPort[0].Trim();
                this.port = Convert.ToInt32(ipAndPort[1].Trim());
            }
            else
                this.serverIP = serverIP;
        }

        public ChinaPnrAsyncClient(string serverIP, int port)
        {
            this.serverIP = serverIP;
            this.port = port;
        }

        /// <summary>
        /// 开启一个异步Socket连接
        /// </summary>
        private void StartClient()
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(serverIP), port);
                client = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                client.SendTimeout = 20;
                client.ReceiveTimeout = 20;
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 关闭异步Socket连接，并释放资源
        /// </summary>
        private void CloseClient()
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        /// <summary>
        /// 发送字符串格式数据，并返回文本结果
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Send(String data)
        {
            try
            {
                connectDone.Reset();
                sendDone.Reset();
                receiveDone.Reset();

                StartClient();

                response = string.Empty;
                byte[] byteData = Encoding.Default.GetBytes(data);
                client.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), client);
                sendDone.WaitOne();

                Receive();

                CloseClient();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //接收数据服务端回调数据
        private void Receive()
        {
            StateObject state = new StateObject();
            state.WorkSocket = client;

            client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
              new AsyncCallback(ReceiveCallback), state);
            receiveDone.WaitOne();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            if (client.Connected)
            { }
            else
            { }
            connectDone.Set();
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);

                sendDone.Set();
            }
            catch (Exception)
            {

            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.WorkSocket;
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.ResponseData.Append(Encoding.Default.GetString(state.Buffer, 0, bytesRead));

                    client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    if (state.ResponseData.Length > 1)
                    {
                        response = state.ResponseData.ToString();
                    }

                    receiveDone.Set();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
