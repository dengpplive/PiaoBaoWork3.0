using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading;
using PbProject.Dal.ControlBase;
using PbProject.Model;
using DataBase.Data;
using DataBase.Unique;
using PbProject.Logic.ControlBase;
using PBPid.WebManage;
using PbProject.ConsoleServerProc;
using PnrAnalysis;
using PbProject.WebCommon.Utility;

//自动取消编码及订单类
namespace ConsoleServerProc
{
    public class XEPNRInfo
    {
        /// <summary>
        /// 公司编号列表
        /// </summary>
        public List<string> CompanyNoList = new List<string>();

        /// <summary>
        /// 取消编码时限（默认60分钟）
        /// </summary>
        public int XEMinutes = 60;

        //离飞机起飞时间时限（默认60分钟）

        /// <summary>
        /// 遍历间隔时间（分钟）
        /// </summary>
        public int InterMinutes = 1;
        /// <summary>
        /// 两天内的订单
        /// </summary>
        public int day = 2;

        /// <summary>
        /// 尝试次数
        /// </summary>
        public int ReTryCount = 0;

        /// <summary>
        /// 停止服务标志
        /// </summary>
        public bool EndFlag = false;
    }

    /// <summary>
    /// 取消编码处理类
    /// </summary>
    public static class XePNR
    {
        public static XEPNRInfo m_XePNRInfo = new XEPNRInfo();
        private static BaseDataManage Manage = new BaseDataManage(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Dal.dll");
        /// <summary>
        /// 界面上显示日志
        /// </summary>
        /// <param name="LogType">1</param>
        /// <param name="Data">数据</param>
        public delegate void XePnrShowLog(int LogType, string Data);

        public static Thread m_XePNRThread = null;

        /// <summary>
        /// 界面显示日志
        /// </summary>
        public static XePnrShowLog XeLog = null;

        /// <summary>
        /// 选择的供应和落地运营商信息
        /// </summary>
        public static List<ListParam> LPList = null;


        //开始服务
        public static void StartServer()
        {
            if (m_XePNRThread != null)
            {
                try
                {
                    m_XePNRThread.Abort();
                }
                catch (Exception ex)
                {
                    //记录错误日志
                    Log.Record("XePnrError.log", ex, "StartServer：停止自动取消PNR处理线程出错！");
                }
                m_XePNRThread = null;
            }

            m_XePNRThread = new Thread(new ThreadStart(XEPNRProcess));
            m_XePNRThread.Start();
        }

        //停止服务
        public static void StopServer()
        {
            if (m_XePNRThread != null)
            {
                try
                {
                    m_XePNRThread.Abort();
                }
                catch (Exception ex)
                {
                    //记录错误日志
                    Log.Record("XePnrError.log", ex, "StopServer：停止自动取消PNR处理线程出错！");
                }
                m_XePNRThread = null;
            }
        }
        /// <summary>
        /// 获取供应商控制系统
        /// </summary>
        /// <param name="CpyNo"></param>
        /// <returns></returns>
        public static string GetGYParameters(string CpyNo)
        {
            string result = "";
            string sqlWhere = string.Format(" CpyNo=left('{0}',12) ", CpyNo.Trim(new char[]{'\''}));
            List<Bd_Base_Parameters> ltParamter = Manage.CallMethod("Bd_Base_Parameters", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Parameters>;
            if (ltParamter != null)
            {
                result = BaseParams.getParams(ltParamter).KongZhiXiTong;
            }
            return result;
        }
        /// <summary>
        /// 记录指令到数据库
        /// </summary>
        /// <param name="sendins"></param>
        /// <returns></returns>
        public static bool LogData(Tb_SendInsData sendins)
        {
            bool Insert = false;
            try
            {
                if (sendins != null)
                {
                    List<string> sqlList = new List<string>();
                    List<string> Removelist = new List<string>();
                    Removelist.Add("id");
                    sqlList.Add(PbProject.Dal.Mapping.MappingHelper<Tb_SendInsData>.CreateInsertModelSql(sendins, Removelist));
                    if (sqlList.Count > 0)
                    {
                        string errMsg = "";
                        Insert = Manage.ExecuteSqlTran(sqlList, out errMsg);
                    }
                }
            }
            catch (Exception)
            {
            }
            return Insert;
        }
        //自动取消处理线程
        public static void XEPNRProcess()
        {
            string SendIns = string.Empty;
            string RecvData = string.Empty;
            string Office = string.Empty;
            string ErrMsg = string.Empty;
            string Pnr = string.Empty;
            string OrderId = string.Empty;
            PnrAnalysis.FormatPNR pnrFormat = new FormatPNR();
            while (true)
            {
                try
                {
                    SendIns = string.Empty;
                    RecvData = string.Empty;
                    Office = string.Empty;
                    ErrMsg = string.Empty;
                    Pnr = string.Empty;
                    OrderId = string.Empty;

                    //组织订单过滤条件
                    //订单来源：白屏预订PNR
                    string tmpSQL = " OrderSourceType=1 " +
                        //订单状态：新订单，等待支付
                        "and OrderStatusCode=1 " +
                        //支付状态：未付
                        "and PayStatus=0 " +
                        //预订公司编号（归属的落地运营商编号范围）
                        "and left(OwnerCpyNo,12) in (" + string.Join(",", m_XePNRInfo.CompanyNoList.ToArray()) + ") " +
                        //预定时间超过设置取消时间值
                        " and DateDiff(minute,CreateTime,getdate())>" + m_XePNRInfo.XEMinutes.ToString() +
                        //3天内的预定订单
                        " and DateDiff(day,CreateTime,getdate())<" + XePNR.m_XePNRInfo.day.ToString();

                    //取消订单表
                    List<Tb_Ticket_Order> list = Manage.CallMethod("Tb_Ticket_Order", "GetList", null, new object[] { tmpSQL }) as List<Tb_Ticket_Order>;
                    //滤过的PNR
                    List<string> tempPntList = new List<string>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        //停止服务，则退出处理
                        if (m_XePNRInfo.EndFlag || XeLog == null)
                        {
                            return;
                        }
                        Tb_Ticket_Order tmpTicketOrder = list[i];
                        ///控制权限
                        string KonZhiXT = GetGYParameters(tmpTicketOrder.OwnerCpyNo);
                        //是否关闭后台自动取消编码功能 
                        if (KonZhiXT != null && KonZhiXT.Contains("|98|"))
                        {
                            continue;
                        }
                        #region 过滤掉已经处理的PNR
                        if (tempPntList.Contains("'" + tmpTicketOrder.PNR + "'"))
                        {
                            continue;
                        }
                        #endregion

                        #region 检查同一编码、同一预订公司下是否存在已付款的订单
                        //包括收银 
                        string tmpSQL3 = "  PNR='" + tmpTicketOrder.PNR + "'"
                            + " and CreateTime>='" + tmpTicketOrder.CreateTime.ToString("yyyy-MM-dd") + "' and ( (OrderStatusCode in(3,4) and  PayStatus=0)  or   PayStatus=1 or OrderStatusCode=4 )";
                        bool flag1 = (bool)Manage.CallMethod("Tb_Ticket_Order", "IsExist", null, new object[] { tmpSQL3 });
                        if (flag1)
                        {
                            if (tmpTicketOrder.PNR != "")
                            {
                                tempPntList.Add("'" + tmpTicketOrder.PNR + "'");
                            }
                            //存在已经支付的订单，略过
                            //记录日志
                            Log.Record("XePnr.log", "订单号：" + tmpTicketOrder.OrderId + "，PNR编号：" + tmpTicketOrder.PNR + "存在已经付款订单，略过该PNR...");
                            continue;
                        }
                        #endregion 检查同一编码、同一预订公司下是否存在已付款的订单

                        #region 提取并检查PNR是否已经出票或RR状态，如果是则略过
                        string GYCpyNo = tmpTicketOrder.OwnerCpyNo;
                        if (GYCpyNo.Length >= 12)
                        {
                            GYCpyNo = GYCpyNo.Substring(0, 12);
                        }
                        List<Bd_Base_Parameters> baseParamList = Manage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + GYCpyNo + "'" }) as List<Bd_Base_Parameters>;
                        ConfigParam config = Bd_Base_ParametersBLL.GetConfigParam(baseParamList);
                        if (config == null)
                        {
                            XeLog(1, string.Format("订单号{0},落地运营商未设置使用配置信息,请设置！", tmpTicketOrder.OrderId));
                            //移除该落地运营商 该落地运营商未设置配置参数
                            if (m_XePNRInfo.CompanyNoList.Contains("'" + GYCpyNo + "'"))
                            {
                                m_XePNRInfo.CompanyNoList.Remove("'" + GYCpyNo + "'");
                            }
                            continue;
                        }
                        OrderId = tmpTicketOrder.OrderId;
                        Office = string.IsNullOrEmpty(tmpTicketOrder.Office) ? config.Office.Split('^')[0] : tmpTicketOrder.Office;
                        if (string.IsNullOrEmpty(Office))
                        {
                            XeLog(1, string.Format("订单号{0}中没有Office或者落地运营商没有设置Office,请检查！", OrderId));
                            //移除该落地运营商 该落地运营商未设置配置参数
                            if (m_XePNRInfo.CompanyNoList.Contains("'" + GYCpyNo + "'"))
                            {
                                m_XePNRInfo.CompanyNoList.Remove("'" + GYCpyNo + "'");
                            }
                            continue;
                        }
                        Pnr = tmpTicketOrder.PNR;
                        if (string.IsNullOrEmpty(tmpTicketOrder.PNR) || (tmpTicketOrder.PNR.Trim() == ""))
                        {
                            XeLog(1, string.Format("订单号{0}中没有PNR,更改为已取消订单！", OrderId));

                            //更改订单状态为已经取消订单
                            string tempSql = "update Tb_Ticket_Order set OrderStatusCode=2,TicketStatus=6  where id='" + tmpTicketOrder.id + "'";
                            Manage.ExecuteNonQuerySQLInfo(tempSql);
                            continue;
                        }

                        Tb_SendInsData SendModel = new Tb_SendInsData();
                        ParamObject PM = new ParamObject();
                        PM.ServerIP = config.WhiteScreenIP;
                        PM.ServerPort = int.Parse(config.WhiteScreenPort);
                        PM.Office = Office;
                        //发送指令
                        SendIns = "RT" + Pnr;
                        PM.code = SendIns;
                        PM.IsPn = true;

                        SendModel.SendIns = SendIns;//发送指令
                        SendModel.SendInsType = 13;//扫描程序发送的指令
                        SendModel.SendTime = System.DateTime.Now;//发送时间                       
                        SendModel.Office = Office;
                        SendModel.ServerIPAndPort = PM.ServerIP + ":" + PM.ServerPort;
                        SendModel.UserAccount = "扫描程序";
                        SendModel.CpyNo = GYCpyNo;
                        //返回数据
                        RecvData = SendNewPID.SendCommand(PM);
                        SendModel.RecvData = RecvData;
                        SendModel.RecvTime = System.DateTime.Now;
                        //记录指令到数据库
                        LogData(SendModel);

                        //指令日志
                        XeLog(1, string.Format("\r\n【编码:{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n订单号：{3}\r\n", Pnr, SendIns, RecvData, tmpTicketOrder.OrderId));
                        if (RecvData.Contains("授权"))
                        {
                            //更改订单状态为已经取消订单
                            string tempSql = "update Tb_Ticket_Order set OrderStatusCode=2,TicketStatus=6  where id='" + tmpTicketOrder.id + "'";
                            Manage.ExecuteNonQuerySQLInfo(tempSql);
                            if (tmpTicketOrder.PNR != "")
                            {
                                tempPntList.Add("'" + tmpTicketOrder.PNR + "'");
                            }
                            XeLog(1, string.Format("订单号{0}中{1} {2}，只取消订单不取消编码！", OrderId, Pnr, RecvData));
                            continue;
                        }
                        string PnrStatus = pnrFormat.GetPnrStatus(RecvData, out ErrMsg);
                        if (!PnrStatus.ToUpper().Contains("HK") && !PnrStatus.ToUpper().Contains("HL") && !PnrStatus.ToUpper().Contains("NO"))//(PnrStatus.Contains("RR") || PnrStatus.Contains("XX"))
                        {
                            if (tmpTicketOrder.PNR != "")
                            {
                                tempPntList.Add("'" + tmpTicketOrder.PNR + "'");
                            }
                            XeLog(1, string.Format("订单号{0}中{1}状态为{2},只取消订单，不处理PNR！", OrderId, Pnr, PnrStatus));

                            //更改订单状态为已经取消订单
                            string tempSql = "update Tb_Ticket_Order set OrderStatusCode=2,TicketStatus=6  where id='" + tmpTicketOrder.id + "'";
                            Manage.ExecuteNonQuerySQLInfo(tempSql);
                            continue;
                        }
                        #endregion 提取并检查PNR是否已经出票或RR状态，如果是则略过

                        #region 发送取消PNR指令并检查结果

                        if (PnrStatus.ToUpper().Contains("HK") || PnrStatus.ToUpper().Contains("HL") || PnrStatus.ToUpper().Contains("NO"))
                        {
                            //取消编码
                            SendIns = "RT" + Pnr + "|XEPNR@" + Pnr;
                            PM.code = SendIns;
                            PM.IsPn = false;

                            SendModel = new Tb_SendInsData();
                            SendModel.SendIns = SendIns;//发送指令
                            SendModel.SendInsType = 13;//扫描程序发送的指令
                            SendModel.SendTime = System.DateTime.Now;//发送时间                       
                            SendModel.Office = Office;
                            SendModel.ServerIPAndPort = PM.ServerIP + ":" + PM.ServerPort;
                            SendModel.UserAccount = "扫描程序";
                            SendModel.CpyNo = GYCpyNo;
                            //返回数据
                            RecvData = SendNewPID.SendCommand(PM);
                            SendModel.RecvData = RecvData;
                            SendModel.RecvTime = System.DateTime.Now;
                            //记录指令到数据库
                            LogData(SendModel);

                            //指令日志
                            XeLog(1, string.Format("\r\n【订单号{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", OrderId, SendIns, RecvData));

                            if (RecvData.ToUpper().Contains("CANCELLED"))
                            {
                                //加入不需要取消的列表
                                tempPntList.Add("'" + tmpTicketOrder.PNR + "'");
                                //取消编码成功后 取消订单
                                List<string> ListSQL = new List<string>();
                                //修改订单数据
                                string tempSql = "update Tb_Ticket_Order set OrderStatusCode=2,TicketStatus=6  where id='" + tmpTicketOrder.id + "'";
                                ListSQL.Add(tempSql);//
                                //修改乘客数据
                                tempSql = "update Tb_Ticket_Passenger set TicketStatus=6  where OrderId='" + OrderId + "'";
                                ListSQL.Add(tempSql);//1.添加订单日志

                                //取消编码 订单日志
                                Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                                OrderLog.id = Guid.NewGuid();
                                OrderLog.OperContent = "订单超过" + m_XePNRInfo.XEMinutes.ToString() + "分钟未能成功支付,系统于" + System.DateTime.Now + "订单自动取消"; ;
                                OrderLog.OperLoginName = "管理员";
                                OrderLog.OperTime = DateTime.Now;
                                OrderLog.OperType = "修改";
                                OrderLog.OperUserName = "管理员";
                                OrderLog.OrderId = OrderId;
                                OrderLog.WatchType = 1;
                                //订单出票公司信息
                                ListParam TicketLP = LPList.Find(delegate(ListParam _tempLP)
                                {
                                    return tmpTicketOrder.OwnerCpyNo.Contains(_tempLP.CpyNo);
                                });
                                if (TicketLP != null)
                                {
                                    OrderLog.CpyName = TicketLP.UninAllName;
                                    OrderLog.CpyNo = TicketLP.CpyNo;
                                    OrderLog.CpyType = 1;
                                }
                                else
                                {
                                    OrderLog.CpyName = tmpTicketOrder.OwnerCpyName;
                                    OrderLog.CpyNo = tmpTicketOrder.OwnerCpyNo;
                                    OrderLog.CpyType = 1;
                                }
                                tempSql = PbProject.Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                                ListSQL.Add(tempSql);
                                //取消订单
                                if (Manage.ExecuteSqlTran(ListSQL, out ErrMsg))
                                {
                                    XeLog(1, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 订单号:" + OrderId + " 订单已取消");
                                }
                            }
                        }
                        #endregion 发送取消PNR指令并检查结果
                    }

                    #region //修改滤过的PNR状态
                    if (tempPntList.Count > 0)
                    {
                        string sqlWhere = " update Log_Pnr set  Flag=1 where   Pnr in(" + string.Join(",", tempPntList.ToArray()) + ")";
                        Manage.ExecuteNonQuerySQLInfo(sqlWhere);
                    }

                    #endregion

                    #region    //生成了编码 没有生成订单的编码
                    string tmpSQL2 = " left(CpyNo,12) in (" + string.Join(",", m_XePNRInfo.CompanyNoList.ToArray()) + ") and DateDiff(minute,OperTime,getdate())>" + m_XePNRInfo.XEMinutes.ToString() + " and OrderFlag=0 and Flag=0 and RetryCount<" + m_XePNRInfo.ReTryCount;
                    //过滤掉没有连接成功的PNR
                    List<string> lstRepeat = new List<string>();
                    //取消PNR表 没有写入订单的数据
                    List<Log_Pnr> list2 = Manage.CallMethod("Log_Pnr", "GetList", null, new object[] { tmpSQL2 }) as List<Log_Pnr>;
                    for (int j = 0; j < list2.Count; j++)
                    {
                        //停止服务，则退出处理
                        if (m_XePNRInfo.EndFlag)
                        {
                            return;
                        }
                        Log_Pnr tmpLogPnr = list2[j];
                        ///控制权限
                        string KonZhiXT = GetGYParameters(tmpLogPnr.CpyNo);
                        //是否关闭后台自动取消编码功能 
                        if (KonZhiXT != null && KonZhiXT.Contains("|98|"))
                        {
                            continue;
                        }
                        string GYCpyNo = tmpLogPnr.CpyNo.Length >= 12 ? tmpLogPnr.CpyNo.Substring(0, 12) : tmpLogPnr.CpyNo;
                        //勾选了该落地运营商的才取消编码
                        if (!m_XePNRInfo.CompanyNoList.Contains("'" + GYCpyNo + "'"))
                        {
                            continue;
                        }
                        //不取消滤过的PNR
                        if (tmpLogPnr.PNR != "" && tempPntList.Contains("'" + tmpLogPnr.PNR + "'"))
                        {
                            tmpLogPnr.Flag = true;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<Log_Pnr>.CreateUpdateModelSql(tmpLogPnr, "id");
                            Manage.ExecuteNonQuerySQLInfo(tempSql);
                            continue;
                        }

                        #region 取消PNR
                        string pnr = tmpLogPnr.PNR;
                        //pnr为空不处理  //Office为空不处理
                        if (string.IsNullOrEmpty(pnr) || string.IsNullOrEmpty(tmpLogPnr.OfficeCode))
                        {
                            tmpLogPnr.Flag = true;
                            string tempSql = PbProject.Dal.Mapping.MappingHelper<Log_Pnr>.CreateUpdateModelSql(tmpLogPnr, "id");
                            Manage.ExecuteNonQuerySQLInfo(tempSql);
                            continue;
                        }
                        ////IP端口为空不处理
                        string[] strIPPort = tmpLogPnr.A7.Split('|');
                        if (strIPPort.Length == 2)
                        {
                            if (pnr.Trim() == "")
                            {
                                tmpLogPnr.Flag = true;
                                string tempSql = PbProject.Dal.Mapping.MappingHelper<Log_Pnr>.CreateUpdateModelSql(tmpLogPnr, "id");
                                Manage.ExecuteNonQuerySQLInfo(tempSql);
                                continue;
                            }


                            string ip = strIPPort[0];
                            int Port = 0;
                            int.TryParse(strIPPort[1], out Port);

                            Tb_SendInsData SendModel = new Tb_SendInsData();
                            ParamObject PM = new ParamObject();
                            PM.ServerIP = ip;
                            PM.ServerPort = Port;
                            PM.Office = tmpLogPnr.OfficeCode;

                            //发送指令
                            SendIns = "RT" + pnr;
                            PM.code = SendIns;

                            SendModel.SendIns = SendIns;//发送指令
                            SendModel.SendInsType = 13;//扫描程序发送的指令
                            SendModel.SendTime = System.DateTime.Now;//发送时间                       
                            SendModel.Office = Office;
                            SendModel.ServerIPAndPort = PM.ServerIP + ":" + PM.ServerPort;
                            SendModel.UserAccount = "扫描程序";
                            SendModel.CpyNo = GYCpyNo;
                            //过滤连接不通的IP端口
                            if (lstRepeat.Contains(SendModel.ServerIPAndPort))
                            {
                                continue;
                            }
                            //返回数据
                            RecvData = SendNewPID.SendCommand(PM);
                            SendModel.RecvData = RecvData;
                            SendModel.RecvTime = System.DateTime.Now;
                            //记录指令到数据库
                            LogData(SendModel);
                            //过滤连接不通的IP端口
                            if (RecvData.Contains("由于连接方在一段时间后没有正确答复或连接的主机没有反应") || RecvData.Contains("不知道这样的主机"))
                            {
                                lstRepeat.Add(SendModel.ServerIPAndPort);
                            }
                            //指令日志
                            XeLog(1, string.Format("\r\n【编码:{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", Pnr, SendIns, RecvData));
                            string PnrStatus = pnrFormat.GetPnrStatus(RecvData, out ErrMsg);
                            if (!PnrStatus.Contains("RR") && !PnrStatus.Contains("XX") && PnrStatus != "")
                            {
                                if (PnrStatus.ToUpper().Contains("HK") || PnrStatus.ToUpper().Contains("HL") || PnrStatus.ToUpper().Contains("NO"))
                                {
                                    //发送指令
                                    SendIns = "RT" + pnr + "|XePNR@" + pnr;
                                    PM.code = SendIns;

                                    SendModel = new Tb_SendInsData();
                                    SendModel.SendIns = SendIns;//发送指令
                                    SendModel.SendInsType = 13;//扫描程序发送的指令
                                    SendModel.SendTime = System.DateTime.Now;//发送时间                       
                                    SendModel.Office = Office;
                                    SendModel.ServerIPAndPort = PM.ServerIP + ":" + PM.ServerPort;
                                    SendModel.UserAccount = "扫描程序";
                                    SendModel.CpyNo = GYCpyNo;
                                    //返回数据
                                    RecvData = SendNewPID.SendCommand(PM);
                                    SendModel.RecvData = RecvData;
                                    SendModel.RecvTime = System.DateTime.Now;
                                    //记录指令到数据库
                                    LogData(SendModel);

                                    //指令日志
                                    XeLog(1, string.Format("\r\n【编码:{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", pnr, SendIns, RecvData));
                                    if (RecvData.ToUpper().Contains("CANCELLED"))
                                    {
                                        //取消成功    
                                        tmpLogPnr.Flag = true;
                                    }
                                    else
                                    {
                                        //取消失败
                                        tmpLogPnr.Flag = false;
                                        tmpLogPnr.RetryCount++;
                                    }
                                    string tempSql = PbProject.Dal.Mapping.MappingHelper<Log_Pnr>.CreateUpdateModelSql(tmpLogPnr, "id");
                                    Manage.ExecuteNonQuerySQLInfo(tempSql);
                                }
                            }
                            else
                            {
                                tmpLogPnr.Flag = true;
                                string tempSql = PbProject.Dal.Mapping.MappingHelper<Log_Pnr>.CreateUpdateModelSql(tmpLogPnr, "id");
                                Manage.ExecuteNonQuerySQLInfo(tempSql);
                            }
                        }
                        #endregion
                    }
                    //更新数据库
                    //if (UpdateSQL.Count > 0)
                    //{
                    //    Manage.ExecuteSqlTran(UpdateSQL, out ErrMsg);
                    //}
                    #endregion
                }
                catch (Exception ex)
                {
                    //记录错误日志
                    Log.Record("XePnrError.log", ex, "XEPNRProcess：自动取消PNR处理过程出错！");
                }
                //遍历时间间隔
                Thread.Sleep(m_XePNRInfo.InterMinutes * 1000);//分钟
            }
        }
    }
}
