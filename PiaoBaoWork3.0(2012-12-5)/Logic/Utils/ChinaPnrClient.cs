using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;


namespace PbProject.ConsoleServerProc.Utils
{
    public class ChinaPnrParams
    {
        /// <summary>
        /// 签名key（必须）。
        /// </summary>
        public string Key { get { return System.Configuration.ConfigurationManager.AppSettings["HuifuAutoKey"]; } }
        /// <summary>
        /// 版本号（必须）。目前固定为10。如版本升级，能向前兼容
        /// </summary>
        public string Version { get { return "10"; } }
        /// <summary>
        /// 自动出票的请求类型（必须）。‘Q’ － 全自动出票请求,‘I’ － 运价查询
        /// </summary>
        public string ReqType { get { return "Q"; } }
        /// <summary>
        /// PNR号（必须）。指航空公司出票记录的大编号
        /// </summary>
        public string PNRNo { get; set; }
        /// <summary>
        /// 小编码标记（可选）。空或者‘N’表示大编码‘Y’表示小编码
        /// </summary>
        public string IsSmallPnr { get; set; }
        /// <summary>
        /// 此PNR号对应的唯一编码（必须）
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 航空公司（必须）。航空公司2 位码（具体需查询）
        /// </summary>
        public string Airlines { get; set; }
        /// <summary>
        /// 票面价（必须）。指明此PNR 号的票面总价
        /// </summary>
        public string FaceValue { get; set; }
        /// <summary>
        /// 参考扣点（可选）。指明此PNR 号的参考扣点的值，出票时，不能低于这个扣点；可以为空，没有就选择最高的扣点
        /// </summary>
        public string AgentPct { get; set; }
        /// <summary>
        /// 航空公司账号（必须）。指对应航空公司的账号，即B2B网站的登录用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 航空公司登陆密码（必须）。指对应航空公司的密码，即B2B网站的登录密码
        /// </summary>
        public string B2BPswd { get; set; }
        /// <summary>
        /// 支付方式（必须）。1：表示“信用帐户支付”；2：表示“付款账户支付”。不接受其它值
        /// </summary>
        public string PayType { get; set; }
        /// <summary>
        /// 钱管家操作号（可选）。该字段不为空，支付账号采用这个字段；该字段为空，支付账号采用“Username”。大小写敏感
        /// </summary>
        public string CPNROper { get; set; }
        /// <summary>
        /// 交易密码（必须）。指钱管家帐号的支付密码，如果账号在汇付提供的网页上配置的，不用做任何处理直接发送，否则需要先做MD5 加密在发送
        /// </summary>
        public string CPNRPswd { get; set; }
        /// <summary>
        /// 返回地址（必须）。用来接收出票服务器的出票结果的地址
        /// </summary>
        public string RetURL { get { return System.Configuration.ConfigurationManager.AppSettings["HuifuAutoUrl"]; } }
        /// <summary>
        /// 合作编码（必须）。指合作伙伴的编码，其中，52：表示“517 平台”，如果为空，表示非合作伙伴发起的交易，每个平台的PartnerCode 的值都不一样，具体的请联系汇付天下技术提供
        /// </summary>
        public string PartnerCode { get; set; }
        /// <summary>
        /// 航空公司附加信息（可选）。航空公司的附加信息，对于不同的航空公司表示不同的含义，比如国航表示“大客户编码”。
        /// </summary>
        public string AirlineInfo { get; set; }
        /// <summary>
        /// 保留字段1（可选）。变长128位
        /// </summary>
        public string Rsv1 { get; set; }
        /// <summary>
        /// 保留字段2（可选）。变长128位
        /// </summary>
        public string Rsv2 { get; set; }
        /// <summary>
        /// 签名（必须）。
        /// 各接口所列有的请求参数和返回参数如无个别说明，都需要参与签名，参与签名的数据
        /// 体为：按照每个接口中包含的参数值（不包含参数名）的顺序（按接口表格中从左到右，
        /// 从上到下的顺序）进行字符串相加。然后用平台和支付窗共同维护的一个KEY与
        /// 以上相加后的字符串再相加，最后再用MD5算法编码。
        /// </summary>
        public string ChkValue { get; set; }
        /// <summary>
        /// 来源类型（可选）。
        /// 0 或者空：登录密码、支付密码等信息由平台自己配置
        /// 1：登录密码、支付密码等信息是在汇付提供的网页上配置，若是这种情况，请参照《PNR支付窗BS 端技术接口规范》
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 组合实体类ChinaPnrParams中的参数,例如：0310VERSION=10&REQTYPE=Q&PNRNO=NXQPGB&ISSMALLPNR=Y&GUID=450824&
        /// AIRLINES=CA&FACEVALUE=1000.00&AGENTPCT=3.0&USERNAME=test&B2BPSWD=6
        /// 54321&PAYTYPE=1&CPNRPSWD=E10ADC3949BA59ABBE56E057F20F883E&RETURL=ht
        /// tp://www.chianpnr.com/test.do&PARTNERCODE=50&AIRLINEINFO=&RSV1=gao&RSV2=sh
        /// an&ChkValue=07D42444CCD348ADD333F8E7E3717AA3
        /// （其中ChkValue 的值为字符串
        /// KEY+”10QNXQPGBY450824CA1000.003.0test6543211E10ADC3949BA59ABBE56E057F20F
        /// 883Ehttp://www.chianpnr.com/test.do50gaoshan”拼接后做MD5 运算得到的。此方法未作此处理）
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string ChinaPnrParamsToString(ChinaPnrParams parameter)
        {
            List<string> fields = EntityClassUtils<ChinaPnrParams>.GetField(parameter);
            List<string> values = EntityClassUtils<ChinaPnrParams>.GetValue(parameter);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i] == "Key")
                {
                    continue;
                }
                if (fields[i] == "ChkValue" && string.IsNullOrEmpty(values[i]))
                {
                    StringBuilder singedStr = new StringBuilder();
                    for (int n = 0; n < fields.Count; n++)
                    {
                        if (string.IsNullOrEmpty(values[n]))
                            continue;
                        if (fields[n] == "Key")
                            continue;
                        if (fields[n] == "ChkValue")
                            continue;
                        singedStr.Append(values[n]);
                    }
                    string checkValue = parameter.Key + singedStr.ToString();
                    string md5 = StringUtils.GetMd5(checkValue);
                    result.Append(fields[i].ToUpper()).Append("=").Append(md5).Append("&");
                    continue;
                }
                if (string.IsNullOrEmpty(values[i]))
                    continue;
                result.Append(fields[i].ToUpper()).Append("=").Append(values[i]).Append("&");
            }
            result = result.Remove(result.Length - 1, 1);
            string headLen = result.Length.ToString();
            if (4 - headLen.Length > 0)
            {
                for (int i = 0; i < 4 - headLen.Length; i++)
                {
                    headLen = "0" + headLen;
                }
            }
            return headLen + result.ToString();
        }

    }

    //同步连接
    public class ChinaPnrClient
    {
        private Socket client = null;
        private string serverIP = null;
        private byte[] buffer = new byte[512];
        private int port = 6180;
        IPEndPoint remoteEP = null;
        /// <summary>
        /// 默认端口6180
        /// </summary>
        /// <param name="serverIP"></param>
        public ChinaPnrClient(string serverIP)
        {
            if (serverIP.IndexOf(':') > 0)
            {
                string[] ipAndPort = serverIP.Split(':');
                this.serverIP = ipAndPort[0].Trim();
                this.port = Convert.ToInt32(ipAndPort[1].Trim());
            }
            else
                this.serverIP = serverIP;
            remoteEP = new IPEndPoint(IPAddress.Parse(this.serverIP), this.port);
        }

        public ChinaPnrClient(string serverIP, int port)
        {
            this.serverIP = serverIP;
            this.port = port;
            remoteEP = new IPEndPoint(IPAddress.Parse(this.serverIP), this.port);
        }

        public string Send(string data)
        {
            try
            {
                Connect();

                byte[] dataBuffer = Encoding.Default.GetBytes(data);
                client.Send(dataBuffer);

                return Receive(15);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Connect()
        {
            client = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            client.SendTimeout = 20;
            client.ReceiveTimeout = 20;

            client.Connect(remoteEP);
        }

        private void Close()
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="timeOut">超时时间（单位：秒）</param>
        /// <returns></returns>
        private string Receive(int timeOut)
        {
            Array.Clear(buffer, 0, buffer.Length);

            int readLen = 0;
            int errorCount = 0;
            while (true)
            {
                try
                {
                    if (errorCount > timeOut)
                        return "";
                    if (readLen > 0)
                        return Encoding.GetEncoding("gb2312").GetString(buffer, 0, readLen);
                    readLen = client.Receive(buffer);
                }
                catch (Exception)
                {
                    errorCount++;
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
