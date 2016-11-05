using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Logic.ControlBase;
using System.Threading;
using PbProject.Model;
using PbProject.Logic.Order;
using PbProject.ConsoleServerProc.Utils;
using PbProject.WebCommon.Utility;
namespace PbProject.ConsoleServerProc
{
    public class B2BAutoTicket : Common
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public B2BAutoTicket()
        {

        }
        /// <summary>
        /// 用于记录日志
        /// </summary>
        /// <param name="LogType"></param>
        /// <param name="Data"></param>
        public delegate void B2BShowLog(int LogType, string Data);
        public B2BShowLog m_Log = null;
        /// <summary>
        /// 操作数据库类
        /// </summary>
        BaseDataManage Manage = new BaseDataManage(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Dal.dll");




        /// <summary>
        ///  记录订单日志
        /// </summary>
        /// <param name="OrderId">订单号</param>
        /// <param name="LogContent">订单日志内容</param>
        /// <param name="LogType">日志类型 1支付宝 2汇付</param>
        /// <returns></returns>
        public bool OrderLog(string OrderId, string LogContent, ListParam LP, string LogType)
        {
            bool Issuc = false;
            try
            {
                LogContent = LogContent.Replace("自动出票启动！", "");
                PbProject.Model.Log_Tb_AirOrder m_OrderLog = new PbProject.Model.Log_Tb_AirOrder();
                m_OrderLog.id = Guid.NewGuid();
                m_OrderLog.OrderId = OrderId;
                m_OrderLog.OperType = "修改";
                m_OrderLog.OperTime = DateTime.Now;
                m_OrderLog.OperContent = LogContent;
                m_OrderLog.OperLoginName = LP != null ? LP.UninAllName : "系统管理员";
                m_OrderLog.OperUserName = LP != null ? LP.UserName : "系统管理员";
                m_OrderLog.WatchType = 2;
                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(m_OrderLog);
                Issuc = Manage.ExecuteNonQuerySQLInfo(tempSql);
            }
            catch (Exception ex)
            {
                if (LogType == "1")
                {
                    //记录日志
                    PnrAnalysis.LogText.LogWrite(ex.Message + "\r\r", "Alipay_OrderLogErr");
                }
                else if (LogType == "2")
                {
                    //记录日志
                    PnrAnalysis.LogText.LogWrite(ex.Message + "\r\r", "China_OrderLogErr");
                }
            }
            return Issuc;
        }

        #region 汇付
        Thread m_ThreadChinapnr = null;
        int m_ChinapnrRefreshTime = 30;//单位秒
        /// <summary>
        /// 开始汇付自动出票工作线程
        /// </summary>
        public void ChinapnrStart(List<string> CpyNoList, List<ListParam> LPList, B2BShowLog Log)
        {
            try
            {
                if (m_ThreadChinapnr == null)
                {
                    m_ThreadChinapnr = new Thread(delegate()
                    {
                        ChinapnrWork(CpyNoList, LPList, Log);
                    });
                    m_ThreadChinapnr.Start();
                }
                else
                {
                    if (m_ThreadChinapnr.IsAlive == false)
                    {
                        m_ThreadChinapnr = new Thread(delegate()
                        {
                            ChinapnrWork(CpyNoList, LPList, Log);
                        });
                        m_ThreadChinapnr.Start();
                    }
                }
                if (Log != null)
                {
                    Log(3, "汇付天下自动出票开始扫描数据...");
                }
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite(ex.Message + "\r\r", "China_OpenErr");
            }
        }
        /// <summary>
        /// 获取供应商控制系统
        /// </summary>
        /// <param name="CpyNo"></param>
        /// <returns></returns>
        public string GetGYParameters(string CpyNo)
        {
            string result = "";
            string sqlWhere = string.Format(" CpyNo=left('{0}',12) ", CpyNo.Trim(new char[] { '\'' }));
            List<Bd_Base_Parameters> ltParamter = Manage.CallMethod("Bd_Base_Parameters", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Parameters>;
            if (ltParamter != null)
            {
                result = BaseParams.getParams(ltParamter).KongZhiXiTong;
            }
            return result;
        }


        /// <summary>
        /// 出票流程
        /// </summary>
        private void ChinapnrWork(List<string> CpyNoList, List<ListParam> LPList, B2BShowLog Log)
        {
            try
            {
                List<string> removeList = new List<string>();
                foreach (string CpyNo in CpyNoList)
                {
                    string KonZhiXT = GetGYParameters(CpyNo);
                    if (KonZhiXT != null && !KonZhiXT.Contains("|22|"))//开启B2B自动出票
                    {
                        removeList.Add(CpyNo);
                    }
                }
                if (removeList.Count > 0)
                {
                    foreach (string item in removeList)
                    {
                        if (CpyNoList.Contains(item))
                        {
                            Log(3, string.Format("公司编号:{0} B2B自动出票未开启\r\n", item));
                            CpyNoList.Remove(item);
                        }
                    }
                }
                PbProject.Logic.Order.Tb_Ticket_OrderBLL orderMange = new PbProject.Logic.Order.Tb_Ticket_OrderBLL();
                PbProject.Logic.Order.Tb_Ticket_PassengerBLL passengerManage = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL();

                ChinaPnrClient client = new ChinaPnrClient(System.Configuration.ConfigurationManager.AppSettings["HuifuAutoIP"]);
                while (true)
                {
                    try
                    {
                        PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();

                        string sqlWhere = string.Format(" left(CPCpyNo,12) in({0}) and  cast( isnull(AutoPrintTimes,'0') as int) < 3 and PolicyType=1 and AutoPrintFlag=2  and OrderStatusCode=3 and A10<>1 ", string.Join(",", CpyNoList.ToArray()));
                        List<PbProject.Model.Tb_Ticket_Order> orderList = Manage.CallMethod("Tb_Ticket_Order", "GetList", null, new object[] { sqlWhere }) as List<PbProject.Model.Tb_Ticket_Order>;


                        if (orderList.Count == 0)
                        {
                            Thread.Sleep(m_ChinapnrRefreshTime * 1000);
                            continue;
                        }

                        foreach (PbProject.Model.Tb_Ticket_Order order in orderList)
                        {
                            if (order.CPCpyNo.Length >= 12)
                            {
                                PbProject.Model.User_Company mTopcom = new PbProject.Logic.User.User_CompanyBLL().GetCompany(order.CPCpyNo.Substring(0, 12));
                                PbProject.Logic.ControlBase.Bd_Base_ParametersBLL Bp = new PbProject.Logic.ControlBase.Bd_Base_ParametersBLL();
                                List<PbProject.Model.Bd_Base_Parameters> ParList = Bp.GetParametersListByCpyNo(mTopcom.UninCode.ToString());
                                BS = PbProject.WebCommon.Utility.BaseParams.getParams(ParList);
                            }
                            else
                            {
                                continue;
                            }

                            ListParam LPM = LPList.Find(delegate(ListParam _pm)
                            {
                                return _pm.CpyNo == order.CPCpyNo.Substring(0, 12);
                            });

                            IList<PbProject.Model.Tb_Ticket_Passenger> orderPassenger = passengerManage.GetPasListByOrderID(order.OrderId);
                            decimal faceValue = 0;
                            foreach (PbProject.Model.Tb_Ticket_Passenger passenger in orderPassenger)
                            {
                                faceValue += passenger.PMFee;
                            }

                            //格式：自动出票方式(1，支付宝本票通；2，汇付天下出票窗)^帐号|是否签约(1，已签约；2，未签)^帐号|密码|支付方式(1，信用账户；2，付款账户)
                            if (string.IsNullOrEmpty(BS.AutoPayAccount.Split('^')[3]) || BS.AutoPayAccount.Split('^')[3].Split('|').Length < 3)
                            {
                                string msg = "未绑定汇付天下账号，不能自动出票，该共功能已停止!，订单ID:" + order.OrderId;
                                //StopChinapnrThread(msg);
                                Log(3, msg);
                                continue;
                            }

                            //格式：CA:xxx//xxx^^^CZ:xxx//xxx^^^MU:xxx//xxx
                            string Acc = "";
                            string pwd = "";
                            string[] CarrList = BS.AutoAccount.Split(new string[] { "^^^" }, StringSplitOptions.RemoveEmptyEntries);//Regex.Split(BS.AutoAccount, "^^^", RegexOptions.IgnoreCase);
                            for (int i = 0; i < CarrList.Length; i++)
                            {
                                if (CarrList[i].Contains(order.CarryCode))
                                {
                                    Acc = CarrList[i].Split(new string[] { "//" }, StringSplitOptions.None)[0].Split(':')[1]; //Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                                    pwd = CarrList[i].Split(new string[] { "//" }, StringSplitOptions.None)[1];//Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                                    break;
                                }
                            }
                            if (string.IsNullOrEmpty(Acc) || string.IsNullOrEmpty(pwd))
                            {
                                string msg = "航空公司帐号和密码为空，不能自动出票，改为手动出票，订单ID:" + order.OrderId;
                                ChinapnrAutofailed(order, msg, LPM, Log);
                                continue;
                            }

                            //根据订单信息，构造参数实体类ChinaPnrParams，参数构造见接口文档
                            ChinaPnrParams chinaPnrParams = new ChinaPnrParams();
                            chinaPnrParams.PNRNo = order.BigCode;
                            chinaPnrParams.GUID = order.OrderId;
                            chinaPnrParams.Airlines = order.CarryCode.Split('/')[0];
                            chinaPnrParams.FaceValue = faceValue.ToString();
                            chinaPnrParams.Username = Acc;
                            chinaPnrParams.B2BPswd = pwd;
                            chinaPnrParams.CPNROper = BS.AutoPayAccount.Split('^')[3].Split('|')[0];
                            chinaPnrParams.CPNRPswd = StringUtils.GetMd5(BS.AutoPayAccount.Split('^')[3].Split('|')[1]);
                            chinaPnrParams.PayType = BS.AutoPayAccount.Split('^')[3].Split('|')[2];
                            chinaPnrParams.PartnerCode = "63";

                            string data = ChinaPnrParams.ChinaPnrParamsToString(chinaPnrParams);


                            ChinapnrTicketWork(orderMange, client, order, data, Convert.ToInt32(BS.AutoPayAccount.Split('^')[1]), LPM, Log);
                        }

                        //在出票结果会出现在ChinaPnrParams中配置的RetURL链接中，在对应的RetURL里执行返回操作，这里业务逻辑结束
                        Thread.Sleep(m_ChinapnrRefreshTime * 1000);
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            if (e is ThreadAbortException)
                                return;
                            // errorLog.Error(e.Message, e);
                            Log(3, e.Message);
                            //ShowChinapnrMsg(string.Format("发生不可预料的异常:{0}该功能暂停，1分钟后重新开始！", e.Message));
                            Log(3, string.Format("发生不可预料的异常:{0}该功能暂停，1分钟后重新开始！", e.Message));
                            Thread.Sleep(60000);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception e)
            {
                try
                {
                    if (e is ThreadAbortException)
                        return;
                    // errorLog.Error(e.Message, e);
                    // ShowChinapnrMsg(string.Format("发生不可预料的异常:{0}该功能停止，如果继续发生请联系开发人员解决，谢谢！", e.Message));
                    Log(3, "发生不可预料的异常:" + e.Message + "该功能停止，如果继续发生请联系开发人员解决，谢谢！");
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 出票Socket调用
        /// </summary>
        /// <param name="orderMange"></param>
        /// <param name="client"></param>
        /// <param name="order"></param>
        /// <param name="comModel"></param>
        /// <param name="data"></param>
        private void ChinapnrTicketWork(PbProject.Logic.Order.Tb_Ticket_OrderBLL orderMange, ChinaPnrClient client, PbProject.Model.Tb_Ticket_Order order, string data, int AutoCount, ListParam Pm, B2BShowLog Log)
        {
            int times = AutoCount;//至少一次
            for (int i = 0; i < times; i++)//调用次数
            {
                //调用ChinaPnrClient对象的Send方法，获取发送结果
                string result = client.Send(data);//合法请求：0011QUERY_VALID；非法请求：0021QUERY_INVALID 错误描述。
                if (result.Contains("0011QUERY_VALID"))
                {
                    order.A10 = "1";//标识出票成功
                    string SQL = " update Tb_Ticket_Order set A10=1 where OrderId='" + order.OrderId + "'";

                    PbProject.Logic.SQLEXBLL.SQLEXBLL_Base ss = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
                    ss.ExecuteNonQuerySQLInfo(SQL);

                    //ShowChinapnrMsg("出票请求成功，订单ID:" + order.OrderId);
                    //infoLog.Info("出票请求成功，订单ID:" + order.OrderId);
                    Log(3, "出票请求成功，订单ID:" + order.OrderId);
                    break;
                }
                else
                {
                    string logDbMsg = "";
                    if (string.IsNullOrEmpty(result))
                    {
                        logDbMsg = "出票请求失败。原因:服务器响应超时";
                        string msg = string.Format("出票请求失败。原因:服务器响应超时,订单ID:{0}", order.OrderId);
                        //ShowChinapnrMsg(msg);
                        //infoLog.Info(msg);
                        Log(3, msg);
                    }
                    else
                    {
                        string errorStr = result.Substring(17, result.Length - 17).Trim();
                        if (errorStr.ToLower().IndexOf("airlines") > 0)
                        {
                            logDbMsg = string.Format("出票请求失败。原因:{0},航空公司二字码:{1}", errorStr, order.CarryCode);
                            string msg = string.Format("出票请求失败。原因:{0},航空公司二字码:{1},订单ID:{2}", errorStr, order.CarryCode, order.OrderId);
                            //ShowChinapnrMsg(msg);
                            //infoLog.Info(msg);
                            Log(3, msg);
                        }
                        else
                        {
                            logDbMsg = string.Format("出票请求失败。原因:{0}", errorStr);
                            string msg = string.Format("出票请求失败。原因:{0},订单ID:{1}", errorStr, order.OrderId);
                            //ShowChinapnrMsg(msg);
                            //infoLog.Info(msg);
                            Log(3, msg);
                        }
                    }
                    if (i == times - 1)
                    {
                        string msg = string.Format("已达到最大失败次数,已改为手动出票,订单ID:{0}", order.OrderId);
                        ChinapnrAutofailed(order, msg, Pm, Log);
                        //FailedLogToDb(order, logDbMsg + ",已改为手动出票");
                        OrderLog(order.OrderId, logDbMsg + ",已改为手动出票", Pm, "2");
                    }
                }
                Thread.Sleep(3000);
            }
        }

        //不能自动出票，设置为手动
        private void ChinapnrAutofailed(PbProject.Model.Tb_Ticket_Order order, string msg, ListParam LPM, B2BShowLog Log)
        {
            string SQL = " update Tb_Ticket_Order set A10=2,AutoPrintFlag=0 where OrderId='" + order.OrderId + "'";

            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base ss = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            ss.ExecuteNonQuerySQLInfo(SQL);
            // ShowChinapnrMsg(msg);
            //infoLog.Info(msg);
            Log(3, msg);
        }

        #endregion



        #region 支付宝
        Thread m_ThreadAlipay = null;
        int m_AlipayRefreshTime = 10;//单位秒
        public void AlipayStart(List<string> CpyNoList, List<ListParam> LPList, B2BShowLog Log)
        {
            try
            {
                if (m_ThreadAlipay == null)
                {
                    m_ThreadAlipay = new Thread(delegate()
                    {
                        AlipayWork(CpyNoList, LPList, Log);
                    });
                    m_ThreadAlipay.Start();
                }
                else
                {
                    if (m_ThreadAlipay.IsAlive == false)
                    {
                        m_ThreadAlipay = new Thread(delegate()
                        {
                            AlipayWork(CpyNoList, LPList, Log);
                        });
                        m_ThreadAlipay.Start();
                    }
                }
                if (Log != null)
                {
                    Log(2, "支付宝本票通自动出票开始扫描数据...");
                }
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite(ex.Message + "\r\r", "Alipay_OpenErr");
            }
        }

        private void AlipayWork(List<string> CpyNoList, List<ListParam> LPList, B2BShowLog Log)
        {
            try
            {
                List<string> removeList = new List<string>();
                foreach (string CpyNo in CpyNoList)
                {
                    string KonZhiXT = GetGYParameters(CpyNo);
                    if (KonZhiXT != null && !KonZhiXT.Contains("|22|"))//开启B2B自动出票
                    {
                        removeList.Add(CpyNo);
                    }
                }
                //移除掉没有开启的B2B运营商
                if (removeList.Count > 0)
                {
                    foreach (string item in removeList)
                    {
                        if (CpyNoList.Contains(item))
                        {
                            Log(2, string.Format("公司编号:{0} B2B自动出票未开启\r\n", item));
                            CpyNoList.Remove(item);
                        }
                    }
                }

                PbProject.Logic.Order.Tb_Ticket_OrderBLL orderMange = new PbProject.Logic.Order.Tb_Ticket_OrderBLL();
                PbProject.Logic.Order.Tb_Ticket_PassengerBLL passengerManage = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL();
                while (true)
                {
                    try//避免异常跳出循环 2013-6-9添加
                    {
                        //王永磊修改语句
                        string sqlWhere = string.Format(" left(CPCpyNo,12) in({0}) and  cast( isnull(AutoPrintTimes,'0') as int) < 3 and PolicyType=1 and AutoPrintFlag=2 and A10<>1  and OrderStatusCode=3 ", string.Join(",", CpyNoList.ToArray()));
                        List<PbProject.Model.Tb_Ticket_Order> orderList = Manage.CallMethod("Tb_Ticket_Order", "GetList", null, new object[] { sqlWhere }) as List<PbProject.Model.Tb_Ticket_Order>;

                        if (orderList == null || orderList.Count == 0)
                        {
                            Thread.Sleep(1);
                            continue;
                        }

                        if (orderList.Count > 0)
                        {
                            Log(2, "支付宝本票通自动出票开始 订单数:" + orderList.Count + "\r\n");
                            for (int i = 0; i < orderList.Count; i++)
                            {
                                PbProject.Model.Tb_Ticket_Order order = orderList[i];
                                ListParam LPM = LPList.Find(delegate(ListParam _pm)
                                {
                                    return _pm.CpyNo == order.CPCpyNo.Substring(0, 12);
                                });
                                if (LPM != null)
                                {
                                    AlipayTicketWork(passengerManage, order, LPM, Log);
                                }
                                Thread.Sleep(1);
                            }
                            Log(2, "支付宝本票通自动出票结束 订单数:" + orderList.Count + "\r\n");
                        }
                        Thread.Sleep(m_AlipayRefreshTime * 1000);
                    }
                    catch (Exception ex)
                    {
                        Log(2, "AlipayWork  while发生不可预料的异常:" + ex.Message + "" + "\r\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Log(2, "AlipayWork发生不可预料的异常:" + ex.Message + "" + "\r\n");
            }
        }
        /// <summary>
        /// 支付宝自动出票
        /// </summary>
        /// <param name="passengerManage"></param>
        /// <param name="order"></param>
        /// <param name="Log"></param>
        private void AlipayTicketWork(Tb_Ticket_PassengerBLL passengerManage, Tb_Ticket_Order order, ListParam LPm, B2BShowLog Log)
        {
            PbProject.Logic.Pay.AliPay alipay = new PbProject.Logic.Pay.AliPay();
            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            string GYCpyNo = order.CPCpyNo;
            if (GYCpyNo.Length >= 12)
            {
                GYCpyNo = GYCpyNo.Substring(0, 12);
                PbProject.Model.User_Company mTopcom = new PbProject.Logic.User.User_CompanyBLL().GetCompany(GYCpyNo);
                PbProject.Logic.ControlBase.Bd_Base_ParametersBLL Bp = new PbProject.Logic.ControlBase.Bd_Base_ParametersBLL();
                List<PbProject.Model.Bd_Base_Parameters> ParList = Bp.GetParametersListByCpyNo(mTopcom.UninCode.ToString());
                BS = PbProject.WebCommon.Utility.BaseParams.getParams(ParList);

                string msg = "";
                string UpdateFiled = "";
                string sql = "";
                try
                {
                    //锁定订单
                    sql = string.Format(" update Tb_Ticket_Order set LockCpyNo='{0}' ,LockTime='{1}',LockLoginName='系统管理员' where OrderId='{2}' ", GYCpyNo, System.DateTime.Now, order.OrderId);
                    Manage.ExecuteNonQuerySQLInfo(sql);

                    bool IsOk = alipay.AutoPay(order, BS, ref msg);
                    //失败请求次数默认
                    int times = 3;
                    try
                    {
                        times = string.IsNullOrEmpty(BS.AutoPayAccount.Split('^')[1]) ? 1 : Convert.ToInt32(BS.AutoPayAccount.Split('^')[1]);
                    }
                    catch (Exception)
                    {
                        Log(2, "错误:失败次数未取到， 订单号:" + order.OrderId + " " + BS.AutoPayAccount + "\r\n");
                    }
                    //请求成功
                    if (IsOk)
                    {
                        //添加操作订单的内容       
                        string LogCon = "于 " + DateTime.Now + " 自动出票启动成功，请等待航空公司出票后系统自动回帖票号";
                        OrderLog(order.OrderId, LogCon, LPm, "1");
                        Log(2, string.Format("出票成功，订单ID{0}", order.OrderId) + "\r\n");
                        UpdateFiled = " A10=1 ,";//标识已请求成功                                      
                    }
                    else
                    {
                        Log(2, string.Format("订单ID{0},出票信息:{1}", order.OrderId, msg) + "\r\n");
                        for (int i = 0; i < times; i++)
                        {
                            IsOk = alipay.AutoPay(order, BS, ref msg);
                            Log(2, string.Format("订单ID{0},出票信息:{1}", order.OrderId, msg) + "\r\n");
                            if (IsOk)
                            {
                                //添加操作订单的内容       
                                string LogCon = "于 " + DateTime.Now + " 自动出票启动成功，请等待航空公司出票后系统自动回帖票号";
                                OrderLog(order.OrderId, LogCon, LPm, "1");
                                Log(2, string.Format("出票成功，订单ID{0}", order.OrderId) + "\r\n");
                                UpdateFiled = " A10=1 ,";//标识已请求成功 
                                break;
                            }
                            if (i >= times - 1)
                            {
                                #region 记录操作日志
                                //记录日志
                                if (msg == "未查到符合支付价格的政策")
                                {
                                    msg = "与航空公司网站价格不符合";
                                }
                                string LogCon = "于 " + DateTime.Now + " 第" + (i + 1).ToString() + "次调用自动出票失败，失败原因：" + msg;
                                OrderLog(order.OrderId, LogCon, LPm, "1");
                                #endregion
                                UpdateFiled = " A10=2,AutoPrintFlag=0 ,";//标识已请求成功                           
                            }
                            Thread.Sleep(1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        Log(2, "发生不可预料的异常:" + ex.Message + "该功能停止，请重新启动，如果继续发生请联系开发人员解决，谢谢！\r\n");
                        if (ex is ThreadAbortException)
                            return;
                    }
                    catch (Exception)
                    {
                    }
                }
                finally
                {
                    //解锁订单
                    sql = string.Format(" update Tb_Ticket_Order set " + UpdateFiled + " LockCpyNo='',LockLoginName='' ,LockTime='{0}' where OrderId='{1}' ", System.DateTime.Now, order.OrderId);
                    Manage.ExecuteNonQuerySQLInfo(sql);
                }
            }
        }

        #endregion


        public void Close()
        {
            try
            {
                //支付宝
                if (m_ThreadAlipay != null && m_ThreadAlipay.IsAlive)
                {
                    m_ThreadAlipay.Abort();
                    m_ThreadAlipay = null;
                    if (m_Log != null)
                    {
                        m_Log(3, "支付宝本票通自动出票停止扫描");
                    }
                }
                //汇付
                if (m_ThreadChinapnr != null && m_ThreadChinapnr.IsAlive)
                {
                    m_ThreadChinapnr.Abort();
                    m_ThreadChinapnr = null;
                    if (m_Log != null)
                    {
                        m_Log(3, "汇付天下自动出票停止扫描");
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
