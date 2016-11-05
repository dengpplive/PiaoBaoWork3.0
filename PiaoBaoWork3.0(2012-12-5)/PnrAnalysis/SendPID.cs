﻿using System;
using System.Collections.Generic;
using System.Text;
using PBPid.WebManage;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using PnrAnalysis.Model;
using System.Web.Security;
using System.IO;
using System.Text.RegularExpressions;
namespace PnrAnalysis
{
    /// <summary>
    /// PID客户端发送指令类
    /// </summary>
    public class SendNewPID
    {
        /// <summary>
        /// 发送指令 新版PID
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string SendCommand(ParamObject param)
        {
            string recvData = string.Empty;
            //用于调试 需屏蔽            
            //=======================
            //if (param.ServerIP.Contains("192.168.8.3"))
            //{
            //    param.ServerIP = param.ServerIP.Replace("192.168.8.3", "203.88.210.227");
            //}
            //=======================
            //设置参数
            WebManage.ServerIp = param.ServerIP;//"192.168.2.107";// 
            WebManage.ServerPort = param.ServerPort;// 360;//
            //WebManage.WebUserName = param.WebUserName;
            //WebManage.WebPwd = param.WebPwd;
            param.code = param.code.Replace("\n", "").Replace("\r", "^");
            //去掉ig
            param.code = param.code.ToLower().StartsWith("ig|") ? param.code.Trim().Substring(3).ToLower() : param.code.ToLower();
            //去掉封口
            //param.code = (param.code.Trim().EndsWith("@") || param.code.Trim().EndsWith(@"\/")) ? param.code.Trim().Substring(0, param.code.Trim().LastIndexOf("|")) : param.code.Trim();
            //发送
            recvData = WebManage.SendCommand(param.code, param.Office, param.IsPn, param.IsAllResult, param.ServerIP, param.ServerPort);
            if (recvData == null)
            {
                recvData = "";
            }
            recvData = recvData.Replace("^", "\r").ToUpper();
            //在发送一次  分离编码和预订编码不在重发
            if (recvData.Contains("LEASE WAIT - TRANSACTION IN PROGRESS") && !param.code.ToLower().Contains("|sp") && !param.code.ToLower().Contains("nm1"))
            {
                StringBuilder sbLog = new StringBuilder();
                sbLog.Append("\r\n时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\nIP:" + param.ServerIP + ":" + param.ServerPort + "\r\nOffice：" + param.Office + "\r\n发送指令：" + param.code.ToLower() + "\r\n接收数据:\r\n");
                recvData = WebManage.SendCommand(param.code, param.Office, param.IsPn, param.IsAllResult, param.ServerIP, param.ServerPort);
                if (recvData == null)
                {
                    recvData = "";
                }
                recvData = recvData.Replace("^", "\r").ToUpper();
                sbLog.Append(recvData + "\r\n\r\n");
                //记录这个日志
                PnrAnalysis.LogText.LogWrite(sbLog.ToString(), "LEASE_WAIT");
            }
            return recvData;
        }


        #region 发送PID  旧版PID
        private int _ReceiveTimeout = 100 * 1000;//接受数据超时时间
        private int _SendTimeout = 100 * 1000;//发送数据超时时间
        /// <summary>
        /// 登陆Pid
        /// </summary> 
        private Socket Login(string BlankIP, string BlankPort, string piduser, string pidpwd)
        {
            Socket sock = null;
            //发送4次数据包
            try
            {
                Byte[] sendL1 = new Byte[162];
                Byte[] sendL2 = new Byte[162];
                Byte[] sendL3 = new Byte[162];
                Byte[] sendL4 = new Byte[162];

                IPAddress test1 = IPAddress.Parse(BlankIP);
                IPEndPoint point1 = new IPEndPoint(test1, int.Parse(BlankPort));

                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, this._SendTimeout);
                sock.SendTimeout = this._SendTimeout;
                sock.ReceiveTimeout = this._ReceiveTimeout;
                sock.Connect(point1);
                if (sock.Connected)
                {
                    sendL1 = new EtermChar.LoginEterm().Login1(piduser, pidpwd);
                    //第一次发送数据
                    sock.Send(sendL1, sendL1.Length, 0);
                    Byte[] RecvL1 = new Byte[5000];
                    int btL1 = sock.Receive(RecvL1, RecvL1.Length, 0);

                    Thread.Sleep(1);


                    //第二次发送数据 ?
                    Byte[] sendOne2 = new EtermChar.LoginEterm().Login2();
                    System.Array.Copy(sendOne2, 0, sendL2, 0, sendOne2.Length);

                    int sLen = sock.Send(sendOne2, 0, sendOne2.Length, 0); //sock.Send(sendL2, sendL2.Length, 0);
                    byte[] recv1 = new byte[1024 * 10];
                    int btL2 = sock.Receive(recv1);

                    Thread.Sleep(1);

                    //第三次发送数据                 
                    Byte[] sendOne3 = new EtermChar.LoginEterm().Login3();
                    System.Array.Copy(sendOne3, 0, sendL3, 0, sendOne3.Length);
                    sLen = sock.Send(sendL3, sendL3.Length, 0);
                    recv1 = new byte[1024 * 10];
                    int btL3 = sock.Receive(recv1);

                    Thread.Sleep(1);

                    //第四次发送数据
                    Byte[] sendOne4 = new EtermChar.LoginEterm().Login4();
                    System.Array.Copy(sendOne4, 0, sendL4, 0, sendOne4.Length);
                    sock.Send(sendL4, sendL4.Length, 0);
                    recv1 = new byte[1024 * 10];
                    int btL4 = sock.Receive(recv1);

                    Thread.Sleep(10);
                }
            }
            catch (SocketException)
            {
                sock.Close();
                sock = null;
            }
            return sock;
        }

        public string Send(ParamObject param)
        {
            Socket skt = null;
            string value = "";
            try
            {
                skt = Login(param.ServerIP, param.ServerPort.ToString(), param.WebUserName, param.WebPwd);
                skt.SendTimeout = 5000;
                skt.ReceiveTimeout = 5000;
                param.code = param.code.Replace("\r\n", "\r").Replace("\n", "\r").Trim('\r');
                //汉字转换成拼音
                param.code = new EtermChar.CharSet().GetPinyin(param.code);

                Byte[] send1 = new EtermChar.CodeOperation().ReturnBlackCode(param.code);
                //2 接收
                Byte[] Recv1 = new Byte[3000];
                Byte[] oldReCode = new Byte[3000];
                if (skt.Connected)
                {
                    int available = skt.Available;
                    //1 发送
                    skt.Send(send1, send1.Length, 0);

                    Byte[] byteMessage = new Byte[3000];
                    try
                    {
                        Recv1 = new Byte[5000];
                        int counts = 0, c = 0;
                        try
                        {
                            byteMessage = new Byte[3000];
                            while (c < 3)
                            {
                                counts = skt.Receive(byteMessage, byteMessage.Length, 0);
                                Array.Copy(byteMessage, 0, Recv1, 0, counts);
                                value = value + new EtermChar.CharSet().CodeStr(Recv1, System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "ChineseFEC2.xml");
                                if (new EtermChar.CodeOperation().IsEnd(Recv1))
                                {
                                    c = 5;
                                }
                            }
                            value = ReplaceCode(value);
                        }
                        catch (SocketException ex)
                        {
                            skt.Close();
                        }
                    }
                    catch (SocketException ex)
                    {
                        skt.Close();
                    }
                }
            }
            finally
            {
                if (skt != null)
                {
                    if (skt.Connected)
                    {
                        skt.Close();
                    }
                    skt = null;
                }
            }
            return value;
        }

        //替换错误字符
        private string ReplaceCode(string str)
        {
            string strNew = "";
            strNew = str.Replace("正确的记录洁号", "正确的记录编号");
            strNew = str.Replace("因考虑到鹿享效率问题暚除AV指令后的PB可以支持外暚其他指令的的PB暂潦净支持摚", "因考虑到共享效率问题，除AV指令后的PB可以支持外，其他指令的的PB暂时不支持");
            strNew = str.Replace("航空公司使用自慷出疗潦限, 请检鹃PNR", "航空公司使用自动出票时限, 请检查PNR");
            return strNew;
        }

        #endregion


        #region 接口发送指令

        public RePnrObj ISendIns(PnrParamObj InputParam)
        {
            RePnrObj PnrObj = new RePnrObj();
            //指令日志
            StringBuilder sbLog = new StringBuilder();
            sbLog.AppendFormat("================时间:{0}  IP：{1} 端口:{2}  PID通道:{3}======================\r\n", System.DateTime.Now, InputParam.ServerIP, InputParam.ServerPort, InputParam.UsePIDChannel);

            List<IPassenger> AdultPasList = new List<IPassenger>();
            List<IPassenger> ChildPasList = new List<IPassenger>();
            List<IPassenger> YingerPasList = new List<IPassenger>();
            foreach (IPassenger pas in InputParam.PasList)
            {
                if (pas.PassengerType == 1)
                {
                    AdultPasList.Add(pas);
                }
                else if (pas.PassengerType == 2)
                {
                    ChildPasList.Add(pas);
                }
                else if (pas.PassengerType == 3)
                {
                    YingerPasList.Add(pas);
                }
            }
            try
            {
                #region 发送指令
                //拼英管理
                PinYingMange Pinyin = new PinYingMange();
                FormatPNR pnrformat = new FormatPNR();
                ECParam ecParam = new ECParam();
                ecParam.ECIP = InputParam.ServerIP;
                ecParam.ECPort = InputParam.ServerPort.ToString();
                ecParam.PID = InputParam.PID;
                ecParam.KeyNo = InputParam.KeyNo;
                ecParam.UserName = InputParam.UserName;
                string Office = (InputParam.Office == "" ? "" : "&" + InputParam.Office + "$");
                SendEC sendec = new SendEC(ecParam);
                //设置IP port
                WebManage.ServerIp = InputParam.ServerIP;
                WebManage.ServerPort = InputParam.ServerPort;
                //设置返回数据
                PnrObj.Office = InputParam.Office;
                PnrObj.ServerIP = InputParam.ServerIP;
                PnrObj.ServerPort = InputParam.ServerPort.ToString();
                ParamObject pm = new ParamObject();
                pm.ServerIP = InputParam.ServerIP;
                pm.ServerPort = InputParam.ServerPort;
                pm.Office = InputParam.Office;


                //格式化成人定编码字符串               
                string errMsg = "", AdultIns = "";
                string sendTime = "", recvTime = "";
                //不是
                if (InputParam.IsInterface == 0)
                {
                    //成人定编码指令
                    if (AdultPasList.Count > 0)
                    {
                        AdultIns = GetYuDingIns(InputParam, AdultPasList);
                        if (InputParam.UsePIDChannel == 2)
                        {
                            AdultIns = AdultIns.Split('@')[0] + "@";
                            AdultIns = AdultIns.Replace("\n", "").Replace("\r", "^");
                            sbLog.AppendFormat("时间:{0}\r\n发送成人预订编码指令:\r\n{1}\r\n", System.DateTime.Now, AdultIns);
                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            pm.code = AdultIns;
                            pm.IsPn = false;
                            //发送
                            string recvData = SendCommand(pm);
                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvData);
                            //指令信息
                            PnrObj.InsList.Add(AdultIns + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, recvData);
                            PnrObj.AdultYudingList.Add(AdultIns + " Office:" + InputParam.Office, recvData);

                            //在发送一次
                            //if (recvData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS"))
                            //{
                            //    sbLog.AppendFormat("时间:{0}\r\n发送成人预订编码指令:\r\n{1}\r\n", System.DateTime.Now, AdultIns);
                            //    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //    recvData = SendCommand(pm);
                            //    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvData);
                            //    //指令信息
                            //    PnrObj.InsList.Add(AdultIns + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, recvData);
                            //    PnrObj.AdultYudingList.Add(AdultIns + " Office:" + InputParam.Office, recvData);
                            //}

                            if (!recvData.ToUpper().Contains("UNABLE") && !recvData.Contains("超时") && !recvData.Contains("无法从传输连接中读取数据") && !recvData.Contains("目标机器") && !recvData.Contains("服务器忙"))
                            {
                                //成人编码
                                PnrObj.AdultPnr = pnrformat.GetPNR(recvData, out errMsg);
                                sbLog.AppendFormat("时间:{0}\r\n解析出成人PNR:{1}\r\n", System.DateTime.Now, PnrObj.AdultPnr);
                            }
                        }
                        else
                        {
                            sbLog.AppendFormat("时间:{0}\r\n发送成人预订编码指令:\r\n{1}\r\n", System.DateTime.Now, AdultIns + Office);
                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            string recvData = sendec.SendData(AdultIns, out errMsg);
                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //指令信息
                            PnrObj.InsList.Add(AdultIns + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, recvData);
                            PnrObj.AdultYudingList.Add(AdultIns + " Office:" + InputParam.Office, recvData);

                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvData);
                            if (!recvData.ToUpper().Contains("UNABLE") && !recvData.Contains("超时") && !recvData.Contains("无法从传输连接中读取数据") && !recvData.Contains("目标机器") && !recvData.Contains("服务器忙"))
                            {
                                //成人编码
                                PnrObj.AdultPnr = pnrformat.GetPNR(recvData, out errMsg);
                                sbLog.AppendFormat("时间:{0}\r\n解析出成人PNR:{1}\r\n", System.DateTime.Now, PnrObj.AdultPnr);
                            }
                        }
                        if (string.IsNullOrEmpty(PnrObj.AdultPnr) || PnrObj.AdultPnr.Trim().Length != 6)
                        {
                            //成人编码生成失败
                            PnrObj.AdultPnr = "成人编码生成失败";
                            sbLog.AppendFormat("时间:{0}\r\n成人编码生成失败\r\n", System.DateTime.Now);
                        }
                    }
                    //儿童
                    if (ChildPasList != null && ChildPasList.Count > 0)
                    {
                        //格式化成人定编码字符串
                        string _ChildIns = GetYuDingIns(InputParam, ChildPasList);
                        string recvChildData = "";
                        if (InputParam.UsePIDChannel == 2)
                        {
                            _ChildIns = _ChildIns.Split('@')[0] + "@";
                            _ChildIns = _ChildIns.Replace("\n", "").Replace("\r", "^");
                            sbLog.AppendFormat("时间:{0}\r\n发送儿童预订编码指令:\r\n{1}\r\n", System.DateTime.Now, _ChildIns);
                            //发送
                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            pm.code = _ChildIns;
                            pm.IsPn = false;
                            //发送
                            recvChildData = SendCommand(pm);
                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvChildData);
                            //指令信息
                            PnrObj.InsList.Add(_ChildIns + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, recvChildData);
                            PnrObj.ChildYudingList.Add(_ChildIns + " Office:" + InputParam.Office, recvChildData);

                            //在发送一次
                            //if (recvChildData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS") || recvChildData.ToUpper().Contains("NEED EXST/CBBG PASSENGER NAME"))
                            //{
                            //    sbLog.AppendFormat("时间:{0}\r\n发送儿童预订编码指令:\r\n{1}\r\n", System.DateTime.Now, _ChildIns);
                            //    //发送
                            //    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //    recvChildData = SendCommand(pm);
                            //    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvChildData);
                            //    //指令信息
                            //    PnrObj.InsList.Add(_ChildIns + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, recvChildData);
                            //    PnrObj.ChildYudingList.Add(_ChildIns + " Office:" + InputParam.Office, recvChildData);
                            //}

                        }
                        else
                        {
                            sbLog.AppendFormat("时间:{0}\r\n发送儿童预订编码指令:\r\n{1}\r\n", System.DateTime.Now, _ChildIns + Office);
                            //返回儿童编码字符串
                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            recvChildData = sendec.SendData(_ChildIns, out errMsg);
                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvChildData);

                            //指令信息
                            PnrObj.InsList.Add(_ChildIns + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, recvChildData);
                            PnrObj.ChildYudingList.Add(_ChildIns + " Office:" + InputParam.Office, recvChildData);
                        }
                        if (!recvChildData.ToUpper().Contains("UNABLE") && !recvChildData.Contains("超时") && !recvChildData.Contains("无法从传输连接中读取数据") && !recvChildData.Contains("目标机器") && !recvChildData.Contains("服务器忙"))
                        {
                            //儿童编码
                            PnrObj.childPnr = pnrformat.GetPNR(recvChildData, out errMsg);
                            if (!string.IsNullOrEmpty(PnrObj.childPnr) && PnrObj.childPnr.Trim().Length == 6)
                            {
                                //没有预订成人编码直接取输入的成人编码
                                if (string.IsNullOrEmpty(PnrObj.AdultPnr) && !string.IsNullOrEmpty(InputParam.AdultPnr) && InputParam.AdultPnr.Trim().Length == 6)
                                {
                                    PnrObj.AdultPnr = InputParam.AdultPnr.Trim();
                                }
                                sbLog.AppendFormat("时间:{0}\r\n解析儿童PNR:\r\n{1}\r\n", System.DateTime.Now, PnrObj.childPnr);
                                //有成人编码时备注
                                if (!string.IsNullOrEmpty(PnrObj.AdultPnr) && PnrObj.AdultPnr.Trim().Length == 6)
                                {
                                    //儿童编码备注成人编码信息
                                    if (InputParam.UsePIDChannel == 2)
                                    {
                                        string rmkIns = string.Format("RT{0}|SSR OTHS {1} ADULT PNR IS {2}|@", PnrObj.childPnr.ToUpper(), InputParam.CarryCode, PnrObj.AdultPnr.ToUpper());
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\n{1}\r\n", System.DateTime.Now, rmkIns);
                                        //发送
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        pm.code = rmkIns;
                                        pm.IsPn = false;
                                        string recvrmk = SendCommand(pm);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvrmk);
                                        //指令信息
                                        PnrObj.InsList.Add(rmkIns + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, recvrmk);

                                        if (recvChildData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS"))
                                        {
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\n{1}\r\n", System.DateTime.Now, rmkIns);
                                            //发送
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            recvChildData = SendCommand(pm);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvrmk);
                                            //指令信息
                                            PnrObj.InsList.Add(rmkIns + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, recvrmk);
                                        }
                                        //EU航空单独处理备注信息


                                    }
                                    else
                                    {
                                        string rmkIns = string.Format("IG|RT{0}|SSR OTHS {1} ADULT PNR IS {2}|@{3}#1", PnrObj.childPnr.ToUpper(), InputParam.CarryCode, PnrObj.AdultPnr.ToUpper(), Office);
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, rmkIns);
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        string recvrmk = sendec.SendData(rmkIns, out errMsg);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvrmk);

                                        //指令信息
                                        PnrObj.InsList.Add(rmkIns + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, recvrmk);
                                    }
                                }
                                if (InputParam.IsGetSpecialPrice == 0)
                                {
                                    //获取儿童RT数据
                                    string sendRT = "", RTRecvData = "";
                                    if (InputParam.UsePIDChannel == 2)
                                    {
                                        sendRT = string.Format("RT{0}", PnrObj.childPnr);
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                        pm.code = sendRT;
                                        pm.IsPn = true;
                                        //发送
                                        RTRecvData = SendCommand(pm);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                        //指令信息
                                        PnrObj.InsList.Add(sendRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRecvData);

                                        if (RTRecvData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS") || RTRecvData.Contains("超时") || RTRecvData.Contains("服务器忙") || RTRecvData.Contains("无法从传输连接中读取数据"))
                                        {
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                            pm.code = sendRT;
                                            pm.IsPn = true;
                                            //发送
                                            RTRecvData = SendCommand(pm);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                            //指令信息
                                            PnrObj.InsList.Add(sendRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRecvData);
                                        }

                                        //儿童编码对应的大编码
                                        PnrObj.childPnrToBigPNR = pnrformat.GetBigCode(RTRecvData, out errMsg);
                                        //获取儿童编码对应的大编码
                                        if (PnrObj.childPnrToBigPNR == "" || PnrObj.childPnrToBigPNR.Length != 6)
                                        {
                                            string sendChildRTR = string.Format("rt{0}|RTR", PnrObj.childPnr);
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendChildRTR);
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                            pm.code = sendChildRTR;
                                            pm.IsPn = true;
                                            //发送
                                            string ChildRTRRecvData = SendCommand(pm);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, ChildRTRRecvData);
                                            //指令信息
                                            PnrObj.InsList.Add(sendChildRTR + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, ChildRTRRecvData);
                                            //儿童编码对应的大编码
                                            PnrObj.childPnrToBigPNR = pnrformat.GetBigCode(ChildRTRRecvData, out errMsg);

                                            if (ChildRTRRecvData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS") || ChildRTRRecvData.Contains("超时") || ChildRTRRecvData.Contains("服务器忙") || ChildRTRRecvData.Contains("无法从传输连接中读取数据"))
                                            {
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendChildRTR);
                                                pm.code = sendChildRTR;
                                                pm.IsPn = true;
                                                //发送
                                                ChildRTRRecvData = SendCommand(pm);
                                                //if (ChildRTRRecvData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS"))
                                                //{
                                                //    ChildRTRRecvData = SendCommand(pm);
                                                //}
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, ChildRTRRecvData);
                                                //指令信息
                                                PnrObj.InsList.Add(sendChildRTR + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, ChildRTRRecvData);
                                                //儿童编码对应的大编码
                                                PnrObj.childPnrToBigPNR = pnrformat.GetBigCode(ChildRTRRecvData, out errMsg);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        sendRT = string.Format("IG|(eas)rt{0}{1}#1", PnrObj.childPnr, Office);
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        RTRecvData = sendec.SendData(sendRT, out errMsg);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                        //指令信息
                                        PnrObj.InsList.Add(sendRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRecvData);
                                        if (RTRecvData.Contains("超时") || RTRecvData.Contains("服务器忙") || RTRecvData.Contains("无法从传输连接中读取数据"))
                                        {
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                            RTRecvData = sendec.SendData(sendRT, out errMsg);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                            //指令信息
                                            PnrObj.InsList.Add(sendRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRecvData);
                                        }
                                        //儿童编码对应的大编码
                                        PnrObj.childPnrToBigPNR = pnrformat.GetBigCode(RTRecvData, out errMsg);
                                        //获取儿童编码对应的大编码
                                        if (PnrObj.childPnrToBigPNR == "" || PnrObj.childPnrToBigPNR.Length != 6)
                                        {
                                            string sendRTR = string.Format("IG|(eas)rt{0}|RTR{1}#1", PnrObj.childPnr, Office);
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            string RTRRecvData = sendec.SendData(sendRTR, out errMsg);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRRecvData);

                                            //指令信息
                                            PnrObj.InsList.Add(sendRTR + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRRecvData);
                                            if (RTRRecvData.Contains("超时") || RTRRecvData.Contains("服务器忙") || RTRRecvData.Contains("无法从传输连接中读取数据"))
                                            {
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                                RTRRecvData = sendec.SendData(sendRTR, out errMsg);
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRRecvData);
                                                //指令信息
                                                PnrObj.InsList.Add(sendRTR + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRRecvData);
                                            }
                                            //儿童编码对应的大编码
                                            PnrObj.childPnrToBigPNR = pnrformat.GetBigCode(RTRRecvData, out errMsg);
                                        }
                                    }
                                    //儿童RT数据
                                    PnrObj.childPnrRTContent = RTRecvData;
                                }
                                //获取儿童Pat数据
                                string sendPat = "", PATRecvData = "";
                                if (InputParam.UsePIDChannel == 2)
                                {
                                    sendPat = string.Format("rt{0}|pat:a*ch", PnrObj.childPnr);
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令【儿童出成人票】:\r\n{1}\r\n", System.DateTime.Now, sendPat);

                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                    pm.code = sendPat;
                                    pm.IsPn = false;
                                    //发送
                                    PATRecvData = SendCommand(pm);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                    //指令信息
                                    PnrObj.InsList.Add(sendPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, PATRecvData);
                                    if (PATRecvData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS") || PATRecvData.Contains("超时") || PATRecvData.Contains("服务器忙") || PATRecvData.Contains("无法从传输连接中读取数据"))
                                    {
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                        pm.code = sendPat;
                                        pm.IsPn = false;
                                        //发送
                                        PATRecvData = SendCommand(pm);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                        //指令信息
                                        PnrObj.InsList.Add(sendPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, PATRecvData);
                                    }

                                    //是否儿童出成人票
                                    if (InputParam.ChildIsAdultEtdz == "1")
                                    {
                                        //赋值儿童pat内容
                                        PnrObj.CHDToAdultPatCon = PATRecvData;

                                        //儿童出成人票 成人PAT内容获取
                                        sendPat = string.Format("rt{0}|pat:a", PnrObj.childPnr);
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令【儿童出成人票】:\r\n{1}\r\n", System.DateTime.Now, sendPat);

                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                        pm.code = sendPat;
                                        pm.IsPn = false;
                                        //发送
                                        PATRecvData = SendCommand(pm);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                        //指令信息
                                        PnrObj.InsList.Add(sendPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, PATRecvData);

                                        if (PATRecvData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS") || PATRecvData.Contains("超时") || PATRecvData.Contains("服务器忙") || PATRecvData.Contains("无法从传输连接中读取数据"))
                                        {
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                            pm.code = sendPat;
                                            pm.IsPn = false;
                                            //发送
                                            PATRecvData = SendCommand(pm);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                            //指令信息
                                            PnrObj.InsList.Add(sendPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, PATRecvData);
                                        }
                                    }
                                }
                                else
                                {
                                    sendPat = string.Format("IG|rt{0}|pat:a*ch{1}#1", PnrObj.childPnr, Office);
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    PATRecvData = sendec.SendData(sendPat, out errMsg);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                    //指令信息
                                    PnrObj.InsList.Add(sendPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, PATRecvData);
                                    if (PATRecvData.Contains("超时") || PATRecvData.Contains("服务器忙") || PATRecvData.Contains("无法从传输连接中读取数据"))
                                    {
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        PATRecvData = sendec.SendData(sendPat, out errMsg);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                        //指令信息
                                        PnrObj.InsList.Add(sendPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, PATRecvData);
                                    }
                                    //是否儿童出成人票
                                    if (InputParam.ChildIsAdultEtdz == "1")
                                    {
                                        //赋值儿童pat内容
                                        PnrObj.CHDToAdultPatCon = PATRecvData;

                                        sendPat = string.Format("IG|rt{0}|pat:a{1}#1", PnrObj.childPnr, Office);
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令【儿童出成人票】:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        PATRecvData = sendec.SendData(sendPat, out errMsg);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                        //指令信息
                                        PnrObj.InsList.Add(sendPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, PATRecvData);

                                        if (PATRecvData.Contains("超时") || PATRecvData.Contains("服务器忙") || PATRecvData.Contains("无法从传输连接中读取数据"))
                                        {
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令【儿童出成人票】:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            PATRecvData = sendec.SendData(sendPat, out errMsg);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                            //指令信息
                                            PnrObj.InsList.Add(sendPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, PATRecvData);
                                        }
                                    }
                                }
                                PnrObj.PatList[1] = PATRecvData;

                            }
                            else
                            {
                                //儿童编码生成失败
                                PnrObj.childPnr = "未能解析出儿童编码！";
                                sbLog.AppendFormat("时间:{0}\r\n未能解析出儿童编码\r\n", System.DateTime.Now);
                            }
                        }
                        else
                        {
                            //儿童编码生成失败
                            PnrObj.childPnr = "儿童编码生成失败！";
                            sbLog.AppendFormat("时间:{0}\r\n儿童编码生成失败\r\n", System.DateTime.Now);
                        }
                    }
                    if (!string.IsNullOrEmpty(PnrObj.AdultPnr) && PnrObj.AdultPnr.Trim().Length == 6)
                    {
                        //婴儿
                        if (YingerPasList != null && YingerPasList.Count > 0)
                        {
                            string RTAdult = "", RTContent = "";
                            if (InputParam.UsePIDChannel == 2)
                            {
                                RTAdult = string.Format("RT{0}", PnrObj.AdultPnr);
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, RTAdult);
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                pm.code = RTAdult;
                                pm.IsPn = true;
                                //发送
                                RTContent = SendCommand(pm);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTContent);
                                //指令信息
                                PnrObj.InsList.Add(RTAdult + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTContent);
                            }
                            else
                            {
                                RTAdult = string.Format("IG|(eas)RT{0}{1}#1", PnrObj.AdultPnr, Office);
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, RTAdult);
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                RTContent = sendec.SendData(RTAdult, out errMsg);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTContent);

                                //指令信息
                                PnrObj.InsList.Add(RTAdult + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTContent);
                            }
                            //解析
                            PnrModel PnrModel = pnrformat.GetPNRInfo(PnrObj.AdultPnr, RTContent, false, out errMsg);
                            if (PnrModel != null && PnrModel._LegList.Count > 0)
                            {
                                string pinyin = "";
                                //解析出来的第一程航段序号
                                string SkyNum = PnrModel._LegList[0].SerialNum;
                                string YIns = "";//婴儿备注指令
                                //循环婴儿
                                foreach (IPassenger IYingerPas in YingerPasList)
                                {
                                    //成人编码备注婴儿信息
                                    StringBuilder sbYingerRMK = new StringBuilder();
                                    ISkyLeg skyLeg = InputParam.SkyList[0];
                                    if (PnrModel._PassengerList.Count > 0)
                                    {
                                        //循环成人
                                        for (int i = 0; i < PnrModel._PassengerList.Count; i++)
                                        {
                                            PassengerInfo _AdultPas = PnrModel._PassengerList[i];
                                            if (PinYingMange.IsChina(IYingerPas.PassengerName))
                                            {
                                                pinyin = PinYingMange.GetSpellByChinese(IYingerPas.PassengerName.Substring(0, 1)) + "/" + PinYingMange.GetSpellByChinese(PinYingMange.RepacePinyinChar(IYingerPas.PassengerName.Substring(1)));
                                            }
                                            else
                                            {
                                                pinyin = IYingerPas.PassengerName;
                                            }
                                            if (InputParam.UsePIDChannel == 2)
                                            {
                                                sbYingerRMK.AppendFormat("RT{0}|XN:IN/{1}INF({2})/p{3}\r", PnrObj.AdultPnr, IYingerPas.PassengerName, FormatPNR.DateToStr(IYingerPas.PasSsrCardID, DataFormat.monthYear), _AdultPas.SerialNum);
                                                sbYingerRMK.AppendFormat("SSR INFT {0} NN1 {1} {2} {3} {4} {5} {6}/P{7}/S{8}\r", InputParam.CarryCode, skyLeg.fromCode + skyLeg.toCode, skyLeg.CarryCode.Replace("*", "") + skyLeg.FlightCode, skyLeg.Space.Substring(0, 1), FormatPNR.DateToStr(skyLeg.FlyStartDate, DataFormat.dayMonth), pinyin, FormatPNR.DateToStr(IYingerPas.PasSsrCardID, DataFormat.dayMonthYear), _AdultPas.SerialNum, SkyNum);
                                                sbYingerRMK.Append("@");
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sbYingerRMK.ToString());
                                                YIns = sbYingerRMK.ToString().Replace("\r", "^");
                                                //发送备注
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                                pm.code = YIns;
                                                pm.IsPn = false;
                                                //发送
                                                string yingerRMKRecv = SendCommand(pm);
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, yingerRMKRecv);
                                                //指令信息
                                                PnrObj.InsList.Add(YIns + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, yingerRMKRecv);
                                                //检测备注是否成功
                                                if (!pnrformat.INFMarkIsOK(yingerRMKRecv, out errMsg))
                                                {
                                                    //有可能没有备注进去 在发一次
                                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sbYingerRMK.ToString());

                                                    pm.code = YIns;
                                                    pm.IsPn = false;
                                                    //发送
                                                    yingerRMKRecv = SendCommand(pm);
                                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, yingerRMKRecv);
                                                    //指令信息
                                                    PnrObj.InsList.Add(YIns + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, yingerRMKRecv);
                                                }
                                            }
                                            else
                                            {
                                                sbYingerRMK.AppendFormat("IG|RT{0}|XN:IN/{1}INF({2})/p{3}\r\n", PnrObj.AdultPnr, IYingerPas.PassengerName, FormatPNR.DateToStr(IYingerPas.PasSsrCardID, DataFormat.monthYear), _AdultPas.SerialNum);
                                                sbYingerRMK.AppendFormat("SSR INFT {0} NN1 {1} {2} {3} {4} {5} {6}/P{7}/S{8}\r\n", InputParam.CarryCode, skyLeg.fromCode + skyLeg.toCode, skyLeg.CarryCode.Replace("*", "") + skyLeg.FlightCode, skyLeg.Space.Substring(0, 1), FormatPNR.DateToStr(skyLeg.FlyStartDate, DataFormat.dayMonth), pinyin, FormatPNR.DateToStr(IYingerPas.PasSsrCardID, DataFormat.dayMonthYear), _AdultPas.SerialNum, SkyNum);
                                                sbYingerRMK.AppendFormat("@{0}#1", Office);
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sbYingerRMK.ToString());
                                                YIns = sbYingerRMK.ToString();
                                                //发送备注
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                string yingerRMKRecv = sendec.SendData(YIns, out errMsg);
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                                //指令信息
                                                PnrObj.InsList.Add(YIns + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, yingerRMKRecv);

                                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, yingerRMKRecv);
                                                //检测备注是否成功
                                                if (!pnrformat.INFMarkIsOK(yingerRMKRecv, out errMsg))
                                                {
                                                    //有可能没有备注进去 在发一次
                                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sbYingerRMK.ToString());
                                                    yingerRMKRecv = sendec.SendData(YIns, out errMsg);
                                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, yingerRMKRecv);
                                                    //指令信息
                                                    PnrObj.InsList.Add(YIns + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, yingerRMKRecv);
                                                }
                                            }
                                            //移除该成人
                                            PnrModel._PassengerList.Remove(_AdultPas);
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                sbLog.AppendFormat("数据解析失败 时间:{0}\r\n编码{1}数据:\r\n{2}\r\n", System.DateTime.Now, PnrObj.AdultPnr, RTContent);
                            }
                            //获取婴儿Pat内容    
                            string sendYinerPat = "", YinerPATRecvData = "";
                            if (InputParam.UsePIDChannel == 2)
                            {
                                sendYinerPat = string.Format("rt{0}|pat:a*in", PnrObj.AdultPnr);
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendYinerPat);
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                pm.code = sendYinerPat;
                                pm.IsPn = false;
                                //发送
                                YinerPATRecvData = SendCommand(pm);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, YinerPATRecvData);
                                //指令信息
                                PnrObj.InsList.Add(sendYinerPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, YinerPATRecvData);

                                if (YinerPATRecvData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS") || YinerPATRecvData.Contains("超时") || YinerPATRecvData.Contains("服务器忙") || YinerPATRecvData.Contains("无法从传输连接中读取数据"))
                                {
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendYinerPat);
                                    pm.code = sendYinerPat;
                                    pm.IsPn = false;
                                    //发送
                                    YinerPATRecvData = SendCommand(pm);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, YinerPATRecvData);
                                    //指令信息
                                    PnrObj.InsList.Add(sendYinerPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, YinerPATRecvData);
                                }
                            }
                            else
                            {
                                sendYinerPat = string.Format("IG|rt{0}|pat:a*in{1}#1", PnrObj.AdultPnr, Office);
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendYinerPat);
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                YinerPATRecvData = sendec.SendData(sendYinerPat, out errMsg);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, YinerPATRecvData);
                                //指令信息
                                PnrObj.InsList.Add(sendYinerPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, YinerPATRecvData);
                                if (YinerPATRecvData.Contains("超时") || YinerPATRecvData.Contains("服务器忙") || YinerPATRecvData.Contains("无法从传输连接中读取数据"))
                                {
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendYinerPat);
                                    YinerPATRecvData = sendec.SendData(sendYinerPat, out errMsg);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, YinerPATRecvData);
                                    //指令信息
                                    PnrObj.InsList.Add(sendYinerPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, YinerPATRecvData);
                                }
                            }
                            PnrObj.PatList[2] = YinerPATRecvData;
                        }
                        if (InputParam.IsGetSpecialPrice == 0)
                        {
                            //获取成人RT内容   
                            string sendAdultRT = "", AdultRTRecvData = "";
                            if (InputParam.UsePIDChannel == 2)
                            {
                                sendAdultRT = string.Format("rt{0}", PnrObj.AdultPnr);
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRT);
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                pm.code = sendAdultRT;
                                pm.IsPn = true;
                                //发送
                                AdultRTRecvData = SendCommand(pm);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRecvData);
                                //指令信息
                                PnrObj.InsList.Add(sendAdultRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, AdultRTRecvData);

                                if (AdultRTRecvData.Contains("超时") || AdultRTRecvData.Contains("服务器忙") || AdultRTRecvData.Contains("无法从传输连接中读取数据"))
                                {
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRT);
                                    pm.code = sendAdultRT;
                                    pm.IsPn = true;
                                    //发送
                                    AdultRTRecvData = SendCommand(pm);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRecvData);
                                    //指令信息
                                    PnrObj.InsList.Add(sendAdultRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, AdultRTRecvData);
                                }

                                //成人编码对应的大编码
                                PnrObj.AdultPnrToBigPNR = pnrformat.GetBigCode(AdultRTRecvData, out errMsg);
                                //获取成人编码对应的大编码
                                if (PnrObj.AdultPnrToBigPNR == "" || PnrObj.AdultPnrToBigPNR.Length != 6)
                                {
                                    string sendAdultRTR = string.Format("rt{0}|RTR", PnrObj.AdultPnr);
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRTR);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                    pm.code = sendAdultRTR;
                                    pm.IsPn = true;
                                    //发送
                                    string AdultRTRRecvData = SendCommand(pm);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRRecvData);
                                    //指令信息
                                    PnrObj.InsList.Add(sendAdultRTR + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, AdultRTRRecvData);
                                    if (AdultRTRRecvData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS") || AdultRTRRecvData.Contains("超时") || AdultRTRRecvData.Contains("服务器忙") || AdultRTRRecvData.Contains("无法从传输连接中读取数据"))
                                    {
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRTR);
                                        pm.code = sendAdultRTR;
                                        pm.IsPn = true;
                                        //发送
                                        AdultRTRRecvData = SendCommand(pm);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRRecvData);
                                        //指令信息
                                        PnrObj.InsList.Add(sendAdultRTR + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, AdultRTRRecvData);
                                    }
                                    //成人编码对应的大编码
                                    PnrObj.AdultPnrToBigPNR = pnrformat.GetBigCode(AdultRTRRecvData, out errMsg);
                                }
                            }
                            else
                            {
                                sendAdultRT = string.Format("IG|(eas)rt{0}{1}#1", PnrObj.AdultPnr, Office);
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRT);
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                AdultRTRecvData = sendec.SendData(sendAdultRT, out errMsg);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRecvData);
                                //指令信息
                                PnrObj.InsList.Add(sendAdultRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, AdultRTRecvData);
                                if (AdultRTRecvData.Contains("超时") || AdultRTRecvData.Contains("服务器忙") || AdultRTRecvData.Contains("无法从传输连接中读取数据"))
                                {
                                    sendAdultRT = string.Format("IG|(eas)rt{0}{1}#1", PnrObj.AdultPnr, Office);
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRT);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    AdultRTRecvData = sendec.SendData(sendAdultRT, out errMsg);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRecvData);
                                    //指令信息
                                    PnrObj.InsList.Add(sendAdultRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, AdultRTRecvData);
                                }

                                //成人编码对应的大编码
                                PnrObj.AdultPnrToBigPNR = pnrformat.GetBigCode(AdultRTRecvData, out errMsg);
                                //获取成人编码对应的大编码
                                if (PnrObj.AdultPnrToBigPNR == "" || PnrObj.AdultPnrToBigPNR.Length != 6)
                                {
                                    string sendAdultRTR = string.Format("IG|(eas)rt{0}|RTR{1}#1", PnrObj.AdultPnr, Office);
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRTR);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    string AdultRTRRecvData = sendec.SendData(sendAdultRTR, out errMsg);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRRecvData);
                                    //指令信息
                                    PnrObj.InsList.Add(sendAdultRTR + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, AdultRTRRecvData);

                                    if (AdultRTRRecvData.Contains("超时") || AdultRTRRecvData.Contains("服务器忙") || AdultRTRRecvData.Contains("无法从传输连接中读取数据"))
                                    {
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRTR);
                                        AdultRTRRecvData = sendec.SendData(sendAdultRTR, out errMsg);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRRecvData);
                                        //指令信息
                                        PnrObj.InsList.Add(sendAdultRTR + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, AdultRTRRecvData);
                                    }
                                    //成人编码对应的大编码
                                    PnrObj.AdultPnrToBigPNR = pnrformat.GetBigCode(AdultRTRRecvData, out errMsg);
                                }
                            }
                            //成人RT数据
                            PnrObj.AdultPnrRTContent = AdultRTRecvData;
                        }


                        //获取成人Pat数据
                        string sendAdultPat = "", AdultPATRecvData = "";
                        if (InputParam.UsePIDChannel == 2)
                        {
                            sendAdultPat = string.Format("rt{0}|pat:a", PnrObj.AdultPnr);
                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultPat);
                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            pm.code = sendAdultPat;
                            pm.IsPn = false;
                            //发送
                            AdultPATRecvData = SendCommand(pm);
                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultPATRecvData);
                            //指令信息
                            PnrObj.InsList.Add(sendAdultPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, AdultPATRecvData);
                            if (AdultPATRecvData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS") || AdultPATRecvData.Contains("超时") || AdultPATRecvData.Contains("服务器忙") || AdultPATRecvData.Contains("无法从传输连接中读取数据"))
                            {
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultPat);
                                pm.code = sendAdultPat;
                                pm.IsPn = false;
                                //发送
                                AdultPATRecvData = SendCommand(pm);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultPATRecvData);
                                //指令信息
                                PnrObj.InsList.Add(sendAdultPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, AdultPATRecvData);
                            }
                        }
                        else
                        {
                            sendAdultPat = string.Format("IG|rt{0}|pat:a{1}#1", PnrObj.AdultPnr, Office);
                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultPat);
                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            AdultPATRecvData = sendec.SendData(sendAdultPat, out errMsg);
                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultPATRecvData);
                            //指令信息
                            PnrObj.InsList.Add(sendAdultPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, AdultPATRecvData);
                            if (AdultPATRecvData.Contains("超时") || AdultPATRecvData.Contains("服务器忙") || AdultPATRecvData.Contains("无法从传输连接中读取数据"))
                            {
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultPat);
                                AdultPATRecvData = sendec.SendData(sendAdultPat, out errMsg);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultPATRecvData);
                                //指令信息
                                PnrObj.InsList.Add(sendAdultPat + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, AdultPATRecvData);
                            }
                        }
                        PnrObj.PatList[0] = AdultPATRecvData;
                    }
                }
                else
                {
                    #region 有编码时获取实体RePnrObj
                    if (InputParam.IPnr.Trim() != "" && InputParam.IPnr.Trim().Length == 6)
                    {
                        //获取成人RT内容   
                        string sendRT = "", RTRecvData = "";
                        PnrModel pnrModel = null;
                        //发送PAT指令
                        if (InputParam.UsePIDChannel == 2)
                        {
                            sendRT = string.Format("rt{0}", InputParam.IPnr);
                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            pm.code = sendRT;
                            pm.IsPn = true;
                            //发送
                            RTRecvData = SendCommand(pm);
                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                            //指令信息
                            PnrObj.InsList.Add(sendRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRecvData);
                            if (RTRecvData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS") || RTRecvData.Contains("超时") || RTRecvData.Contains("服务器忙") || RTRecvData.Contains("无法从传输连接中读取数据"))
                            {
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                pm.code = sendRT;
                                pm.IsPn = true;
                                //发送
                                RTRecvData = SendCommand(pm);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                //指令信息
                                PnrObj.InsList.Add(sendRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRecvData);
                            }
                            pnrModel = pnrformat.GetPNRInfo(InputParam.IPnr, RTRecvData, false, out errMsg);
                            string BigPnrCode = pnrformat.GetBigCode(RTRecvData, out errMsg);
                            //成人编码
                            if (pnrModel._PasType == "1")
                            {
                                PnrObj.AdultPnr = InputParam.IPnr;
                                PnrObj.AdultPnrToBigPNR = BigPnrCode;
                                PnrObj.AdultPnrRTContent = RTRecvData;
                            }
                            else if (pnrModel._PasType == "2")
                            {
                                PnrObj.childPnr = InputParam.IPnr;
                                PnrObj.childPnrToBigPNR = BigPnrCode;
                                PnrObj.childPnrRTContent = RTRecvData;
                            }

                            if (pnrModel._PasType == "1")
                            {
                                sendRT = string.Format("rt{0}|pat:a", PnrObj.AdultPnr);
                            }
                            else if (pnrModel._PasType == "2")
                            {
                                sendRT = string.Format("rt{0}|pat:a*ch", PnrObj.childPnr);
                            }
                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            pm.code = sendRT;
                            pm.IsPn = false;
                            //发送
                            RTRecvData = SendCommand(pm);
                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                            //指令信息
                            PnrObj.InsList.Add(sendRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRecvData);

                            if (RTRecvData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS") || RTRecvData.Contains("超时") || RTRecvData.Contains("服务器忙") || RTRecvData.Contains("无法从传输连接中读取数据"))
                            {
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                pm.code = sendRT;
                                pm.IsPn = false;
                                //发送
                                RTRecvData = SendCommand(pm);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                //指令信息
                                PnrObj.InsList.Add(sendRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRecvData);
                            }
                        }
                        else
                        {
                            sendRT = string.Format("IG|(eas)rt{0}{1}#1", InputParam.IPnr, Office);
                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);

                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            RTRecvData = sendec.SendData(sendRT, out errMsg);
                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                            //指令信息
                            PnrObj.InsList.Add(sendRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRecvData);

                            if (RTRecvData.Contains("超时") || RTRecvData.Contains("服务器忙") || RTRecvData.Contains("无法从传输连接中读取数据"))
                            {
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                RTRecvData = sendec.SendData(sendRT, out errMsg);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                //指令信息
                                PnrObj.InsList.Add(sendRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRecvData);

                            }

                            pnrModel = pnrformat.GetPNRInfo(InputParam.IPnr, RTRecvData, false, out errMsg);
                            string BigPnrCode = pnrformat.GetBigCode(RTRecvData, out errMsg);
                            //成人编码
                            if (pnrModel._PasType == "1")
                            {
                                PnrObj.AdultPnr = InputParam.IPnr;
                                PnrObj.AdultPnrToBigPNR = BigPnrCode;
                                PnrObj.AdultPnrRTContent = RTRecvData;
                            }
                            else if (pnrModel._PasType == "2")
                            {
                                PnrObj.childPnr = InputParam.IPnr;
                                PnrObj.childPnrToBigPNR = BigPnrCode;
                                PnrObj.childPnrRTContent = RTRecvData;
                            }

                            if (pnrModel._PasType == "1")
                            {
                                sendRT = string.Format("IG|rt{0}|pat:a{1}#1", InputParam.IPnr, Office);
                            }
                            else if (pnrModel._PasType == "2")
                            {
                                sendRT = string.Format("IG|rt{0}|pat:a*ch{1}#1", InputParam.IPnr, Office);
                            }
                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            RTRecvData = sendec.SendData(sendRT, out errMsg);
                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                            //指令信息
                            PnrObj.InsList.Add(sendRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRecvData);

                            if (RTRecvData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS") || RTRecvData.Contains("超时") || RTRecvData.Contains("服务器忙") || RTRecvData.Contains("无法从传输连接中读取数据"))
                            {
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                RTRecvData = sendec.SendData(sendRT, out errMsg);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                //指令信息
                                PnrObj.InsList.Add(sendRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRecvData);
                            }
                        }
                        if (pnrModel._PasType == "1")
                        {
                            PnrObj.PatList[0] = RTRecvData;
                        }
                        else if (pnrModel._PasType == "2")
                        {
                            PnrObj.PatList[1] = RTRecvData;
                        }
                        PnrObj.PnrType = pnrModel._PasType;
                    }
                    #endregion

                }
                #endregion

                //内容转换到实体
                ConToModel(PnrObj, out errMsg);
            }
            catch (Exception ex)
            {
                sbLog.Append(ex.TargetSite.ToString() + "\r\n");
            }
            finally
            {
                sbLog.AppendFormat("结束================================================================\r\n");
                //记录日志
                LogText.LogWrite(sbLog.ToString(), "ILog");
            }
            return PnrObj;
        }
        /// <summary>
        /// 将编码内容解析到实体
        /// </summary>
        /// <param name="PnrObj"></param>
        /// <param name="errMsg"></param>
        public static void ConToModel(RePnrObj PnrObj, out string errMsg)
        {
            FormatPNR pnrformat = new FormatPNR();
            errMsg = "";
            #region Pnr内容解析
            //成人
            if (PnrObj.AdultPnrRTContent != null && PnrObj.AdultPnrRTContent != "" && PnrObj.AdultPnr.Trim().Length == 6)
            {
                PnrObj.PnrList[0] = pnrformat.GetPNRInfo(PnrObj.AdultPnr, PnrObj.AdultPnrRTContent, false, out errMsg);
            }
            //儿童
            if (PnrObj.childPnrRTContent != null && PnrObj.childPnrRTContent != "" && PnrObj.childPnr.Trim().Length == 6)
            {
                PnrObj.PnrList[1] = pnrformat.GetPNRInfo(PnrObj.childPnr, PnrObj.childPnrRTContent, false, out errMsg);
            }
            #endregion
            #region Pat内容解析
            //成人pat
            if (PnrObj.PatList[0] != null && PnrObj.PatList[0].Trim() != "" && PnrObj.PatList[0].Trim().Length > 30)
            {
                PnrObj.PatModelList[0] = pnrformat.GetPATInfo(PnrObj.PatList[0].Trim(), out errMsg);
            }
            //儿童pat
            if (PnrObj.PatList[1] != null && PnrObj.PatList[1].Trim() != "" && PnrObj.PatList[1].Trim().Length > 30)
            {
                PnrObj.PatModelList[1] = pnrformat.GetPATInfo(PnrObj.PatList[1].Trim(), out errMsg);
            }
            //婴儿pat
            if (PnrObj.PatList[2] != null && PnrObj.PatList[2].Trim() != "" && PnrObj.PatList[2].Trim().Length > 30)
            {
                PnrObj.PatModelList[2] = pnrformat.GetPATInfo(PnrObj.PatList[2].Trim(), out errMsg);
            }
            if (PnrObj.CHDToAdultPatCon != null && PnrObj.CHDToAdultPatCon.Trim() != "")
            {
                PnrObj.CHDToAdultPat = pnrformat.GetPATInfo(PnrObj.CHDToAdultPatCon.Trim(), out errMsg);
            }
            #endregion

        }



        /// <summary>
        /// 组合预订编码指令
        /// </summary>
        /// <param name="InputParam"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        private string GetYuDingIns(PnrParamObj InputParam, List<IPassenger> pas)
        {
            StringBuilder sb = new StringBuilder("NM");
            bool IsAddLine = false;//姓名项字符超过80加一个\r
            int a = 0;
            bool IsChild = false;
            for (int i = 0; i < pas.Count; i++)
            {
                if (pas[i].PassengerType != 3)
                {
                    if (pas[i].PassengerType == 2)
                    {
                        IsChild = true;
                    }
                    if (pas[i].PassengerType == 2)
                    {
                        //儿童
                        if (PinYingMange.IsChina(pas[i].PassengerName.Trim()))
                        {
                            if (pas[i].PassengerName.EndsWith("CHD"))
                            {
                                pas[i].PassengerName = pas[i].PassengerName.Substring(0, pas[i].PassengerName.LastIndexOf("CHD"));
                            }
                        }
                        else
                        {
                            if (pas[i].PassengerName.EndsWith(" CHD"))
                            {
                                pas[i].PassengerName = pas[i].PassengerName.Substring(0, pas[i].PassengerName.LastIndexOf(" CHD"));
                            }
                        }
                        //if (InputParam.CarryCode.ToUpper() != "CZ")
                        //{
                        pas[i].PassengerName += "CHD";
                        //}
                        sb.Append("1" + pas[i].PassengerName.Trim());
                    }
                    else
                    {
                        //成人
                        sb.Append("1" + pas[i].PassengerName.Trim());
                    }

                    //一屏幕显示不完加\r
                    int len = System.Text.Encoding.ASCII.GetByteCount(sb.ToString());
                    if (len > 80 && !IsAddLine)
                    {
                        sb.Append("\r");
                        IsAddLine = true;
                    }
                    a++;
                }
            }
            sb.Append("\r");

            decimal decDiscount = 0m;
            DateTime endTimeChd = DateTime.Parse("2013-09-25 23:59:59");
            string[] Space = new string[InputParam.SkyList.Count];
            for (int i = 0; i < InputParam.SkyList.Count; i++)
            {
                decimal.TryParse(InputParam.SkyList[i].Discount, out decDiscount);
                if (IsChild)
                {
                    if (InputParam.ChildIsAdultEtdz == "1" || InputParam.SkyList[i].Space.ToUpper() == "F" || InputParam.SkyList[i].Space.ToUpper() == "C" || InputParam.SkyList[i].Space.ToUpper() == "Y" || decDiscount > 100)
                    {
                        //排除子舱位
                        Space[i] = InputParam.SkyList[i].Space.ToUpper().Substring(0, 1);
                    }
                    else
                    {
                        Space[i] = "Y";
                    }
                    //EU只能是O舱
                    if (InputParam.CarryCode.ToUpper() == "EU" && DateTime.Compare(System.DateTime.Now, endTimeChd) < 0)
                    {
                        Space[i] = "O";
                    }
                }
                else
                {
                    Space[i] = InputParam.SkyList[i].Space.ToUpper().Substring(0, 1);
                }
                sb.Append("SS " + InputParam.SkyList[i].CarryCode.Replace("*", "") + InputParam.SkyList[i].FlightCode.Replace("*", "") + " " + Space[i] + " " + FormatPNR.DateToStr(InputParam.SkyList[i].FlyStartDate, DataFormat.dayMonthYear) + " " + InputParam.SkyList[i].fromCode.ToUpper() + InputParam.SkyList[i].toCode.ToUpper() + " " + a + "\r");
            }
            if (!string.IsNullOrEmpty(InputParam.CTTel) && InputParam.CTTel != InputParam.CTCTPhone)
            {
                sb.Append("CT" + InputParam.CTTel + "\r");
            }
            //落地运营商公司电话号码
            //if (!string.IsNullOrEmpty(InputParam.LuoDiCTTel) && InputParam.LuoDiCTTel != InputParam.CTTel)
            //{
            //    sb.Append("CT" + InputParam.LuoDiCTTel + "\r");
            //}

            //证件号处理
            StringBuilder sbCHLD = new StringBuilder();
            string cid = "";
            for (int i = 0; i < pas.Count; i++)
            {
                if (IsChild)//儿童
                {
                    cid = pas[i].PasSsrCardID;
                    string regDate = @"^\d{4}-\d{2}-\d{2}$";
                    try
                    {
                        if (Regex.IsMatch(pas[i].PasSsrCardID, regDate))
                        {
                            cid = cid.Replace("-", "").Replace(":", "").Replace(" ", "");
                        }
                        if (Regex.IsMatch(pas[i].ChdBirthday, regDate))//儿童标识CHLD
                        {
                            sbCHLD.Append("SSR CHLD " + InputParam.CarryCode.ToUpper() + " HK1/" + FormatPNR.DateToStr(pas[i].ChdBirthday, DataFormat.dayMonthYear) + "/P" + (i + 1).ToString() + "\r");
                        }
                    }
                    catch
                    {
                    }
                    sb.Append("SSR FOID " + InputParam.CarryCode.ToUpper() + " HK/NI" + cid + "/P" + (i + 1) + "\r");
                }
                else
                {
                    if (pas[i].PassengerType == 1)//成人
                    {
                        cid = pas[i].PasSsrCardID;
                        try
                        {
                            string regDate = @"^\d{4}-\d{2}-\d{2}$";
                            if (Regex.IsMatch(cid, regDate))
                            {
                                // DateTime.Parse(cid);
                                // cid = FormatPNR.DateToStr(cid, DataFormat.dayMonthYear);
                                cid = cid.Replace("-", "").Replace(":", "").Replace(" ", "");
                            }
                        }
                        catch { }
                        sb.Append("SSR FOID " + InputParam.CarryCode.ToUpper() + " HK/NI" + cid + "/P" + (i + 1) + "\r");
                    }
                }
                //航空公司卡号
                if (!string.IsNullOrEmpty(pas[i].CpyandNo))
                {
                    sb.Append("SSR FQTV " + InputParam.CarryCode.ToUpper() + " HK/" + InputParam.CarryCode.ToUpper() + pas[i].CpyandNo + "/P" + (i + 1) + "\r");
                }
            }

            if (!string.IsNullOrEmpty(InputParam.CTCTPhone))
            {
                string CarryCode = InputParam.CarryCode.ToUpper();
                string CTCTContactTel = InputParam.CTCTPhone;
                if (CarryCode == "MF")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT {1}\r", CarryCode, CTCTContactTel);
                }
                else if (CarryCode == "ZH" || CarryCode == "HU")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT {1}\r", CarryCode, CTCTContactTel);
                }
                else if (CarryCode == "CA")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT{1}\r", CarryCode, CTCTContactTel);
                }
                else if (CarryCode == "CZ")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCP{1}\r", CarryCode, CTCTContactTel);
                }
                else if (CarryCode == "MU" || CarryCode == "FM" || CarryCode == "KN")
                {
                    //CTCT项
                    //sb.AppendFormat("OSI {0} CTCT{1}/PN\r", CarryCode, CTCTContactTel);
                    sb.AppendFormat("OSI {0} CTCT{1}\r", CarryCode, CTCTContactTel);
                }
                else
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT{1}\r", CarryCode, CTCTContactTel);
                }
            }

            //落地运营商手机号码         
            if (InputParam.LuoDiCTCTPhone != InputParam.CTCTPhone && !string.IsNullOrEmpty(InputParam.LuoDiCTCTPhone))
            {
                string CarryCode = InputParam.CarryCode.ToUpper();
                string CTCTContactTel = InputParam.LuoDiCTCTPhone;
                if (CarryCode == "MF")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT {1}\r", CarryCode, CTCTContactTel);
                }
                else if (CarryCode == "ZH" || CarryCode == "HU")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT {1}\r", CarryCode, CTCTContactTel);
                }
                else if (CarryCode == "CA")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT{1}\r", CarryCode, CTCTContactTel);
                }
                else if (CarryCode == "CZ")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCP{1}\r", CarryCode, CTCTContactTel);
                }
                else if (CarryCode == "MU" || CarryCode == "FM" || CarryCode == "KN")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT{1}\r", CarryCode, CTCTContactTel);
                }
                else
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT{1}\r", CarryCode, CTCTContactTel);
                }
            }

            //儿童CHLD
            if (sbCHLD.ToString() != "")
            {
                sb.Append(sbCHLD.ToString());
            }

            //起飞前1小时
            DateTime t = DateTime.Parse("1900-1-1 " + InputParam.SkyList[0].FlyStartTime.Insert(2, ":") + ":00");
            t = t.AddHours(-InputParam.FlyAdvanceTime);
            string time = t.Hour.ToString().PadLeft(2, '0') + t.Minute.ToString().PadLeft(2, '0');
            //TKTL/2330/26OCT11/KMG226
            string tempOffice = InputParam.Office;
            if (tempOffice.ToLower() == "lhw108")
            {
                tempOffice = "lhw148";
            }
            sb.Append("TKTL/" + time + "/" + FormatPNR.DateToStr(InputParam.SkyList[0].FlyStartDate, DataFormat.dayMonth) + "/" + tempOffice);
            sb.Append("\r");
            if (InputParam.UsePIDChannel == 0)
            {
                sb.Append("@&" + InputParam.Office + "$");
            }
            else
            {
                sb.Append("@&" + InputParam.Office);
            }
            if (InputParam.UsePIDChannel == 0)
            {
                return sb.ToString().ToUpper().Replace("\r", "\n");
            }
            else
            {
                return sb.ToString().ToUpper();
            }
        }


        #endregion
    }
}

