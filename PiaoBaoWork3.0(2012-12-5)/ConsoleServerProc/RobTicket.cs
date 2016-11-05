using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PbProject.Logic.ControlBase;
using PbProject.Model;
using PbProject.WebCommon.Utility;
using System.Data;
using PbProject.Logic.Buy;
using PbProject.Dal.Mapping;
using System.Diagnostics;
using PnrAnalysis.Model;
using PnrAnalysis;
using PbProject.Logic.Order;
using PbProject.Logic.Pay;
namespace PbProject.ConsoleServerProc
{
    /// <summary>
    /// 抢票扫描处理类
    /// </summary>
    public class RobTicket : Common
    {
        /// <summary>
        /// 查询航班参数
        /// </summary>
        public class QueryFlightParam
        {
            /// <summary>
            /// 当前用户公司实体
            /// </summary>
            public User_Company m_Company = null;
            /// <summary>
            /// 当前用户实体
            /// </summary>
            public User_Employees m_User = null;
            /// <summary>
            /// 当前用户参数信息
            /// </summary>
            public List<Bd_Base_Parameters> m_BaseParam = null;
            /// <summary>
            /// 落地运营商参数信息
            /// </summary>
            public List<Bd_Base_Parameters> m_ParentParam = null;
            /// <summary>
            /// 用户使用配置信息
            /// </summary>
            public ConfigParam m_Config = null;
            /// <summary>
            /// 订单输入参数
            /// </summary>
            public OrderMustParamModel m_OrderInput = null;

            /// <summary>
            /// 权限 控制系统
            /// </summary>
            public string KonZhiXT = string.Empty;
            /// <summary>
            /// 编码信息
            /// </summary>
            public RePnrObj PnrInfo = null;
            /// <summary>
            /// 抢票成功 true  失败false
            /// </summary>
            public bool IsRobSuccess = false;
            /// <summary>
            /// 航空公司二字码
            /// </summary>
            public string CarryCode = string.Empty;

            /// <summary>
            /// 是否儿童出成人票默认0否 1是
            /// </summary>
            public int IsCHDToAudltTK = 0;
            /// <summary>
            /// 是否允许换编码
            /// </summary>
            public bool AllowChangePNRFlag = false;
            /// <summary>
            /// 当前线程
            /// </summary>
            public ManageThread m_thManage = null;

            /// <summary>
            /// 日志
            /// </summary>
            public StringBuilder sbLog = new StringBuilder();
            //单条日志
            public string strLog = string.Empty;

            /// <summary>
            /// 清空引用指针
            /// </summary>
            public void Dispose()
            {
                if (m_Company != null)
                {
                    m_Company = null;
                }
                if (m_User != null)
                {
                    m_User = null;
                }
                if (m_BaseParam != null)
                {
                    m_BaseParam = null;
                }
                if (m_ParentParam != null)
                {
                    m_ParentParam = null;
                }
                if (m_Config != null)
                {
                    m_Config = null;
                }
                if (m_OrderInput != null)
                {
                    m_OrderInput = null;
                }
                if (PnrInfo != null)
                {
                    PnrInfo = null;
                }
            }

        }
        /// <summary>
        /// 线程管理
        /// </summary>
        public class ManageThread
        {
            /// <summary>
            /// 线程名称
            /// </summary>
            public string m_strThreadName = string.Empty;
            /// <summary>
            /// 当前线程对象
            /// </summary>
            public Thread m_th = null;
            /// <summary>
            /// 线程是否运行结束 true结束 false没有结束
            /// </summary>
            public bool IsOver = true;
            /// <summary>
            /// 订单
            /// </summary>
            public Tb_Ticket_Order m_Order = null;
        }

        /// <summary>
        /// 扫描时间间隔
        /// </summary>
        public int m_ScanTime = 180;//秒
        /// <summary>
        /// 抢票持续时间
        /// </summary>
        public int m_InnerScanTime = 60;//分钟

        /// <summary>
        /// 指定时间范围内没有抢到票的最大次数
        /// </summary>
        public int m_ScanCount = 10;//默认最大10次
        /// <summary>
        /// 用于记录日志
        /// </summary>
        /// <param name="LogType"></param>
        /// <param name="Data"></param>
        public delegate void RobShowLog(int LogType, string Data);

        /// <summary>
        /// 抢票线程列表
        /// </summary>
        private List<ManageThread> RobThreadList = new List<ManageThread>();
        /// <summary>
        /// 记日志
        /// </summary>
        public RobShowLog m_Log = null;
        /// <summary>
        /// 落地运营商公司信息
        /// </summary>
        public List<ListParam> SelGYTextList = null;
        /// <summary>
        /// 落地运营商公司编号
        /// </summary>
        public List<string> lstCpyNo = null;
        /// <summary>
        /// 操作数据库类
        /// </summary>
        private BaseDataManage Manage = new BaseDataManage(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Dal.dll");
        /// <summary>
        /// 扫描线程
        /// </summary>
        private Thread m_th = null;
        //清理资源线程
        private Thread m_SourceTh = null;

        /// <summary>
        /// 是否运行 true 正在运行 false 停止运行
        /// </summary>
        private bool IsRun = false;
        /// <summary>
        /// 开启扫描
        /// </summary>
        public void StartScan()
        {
            try
            {
                #region 开启扫描
                if (m_th != null && m_th.IsAlive)
                {
                    m_th.Abort();
                    m_th = null;
                }
                m_th = new Thread(delegate()
                {
                    //正在运行
                    IsRun = true;
                    if (m_Log != null)
                    {
                        m_Log(4, "抢票主程序已开启.....\r\n");
                    }
                    ScanTicket();
                });
                //开启线程
                m_th.Start();

                #endregion

                #region 清理线程资源
                while (true)
                {
                    if (IsRun)
                    {
                        m_SourceTh = new Thread(delegate()
                          {
                              //清了集合
                              while (true)
                              {
                                  if (RobThreadList.Count > 0)
                                  {
                                      for (int i = 0; i < RobThreadList.Count; i++)
                                      {
                                          if (RobThreadList[i].IsOver)
                                          {
                                              try
                                              {
                                                  if (RobThreadList[i].m_th != null && RobThreadList[i].m_th.IsAlive)
                                                  {
                                                      RobThreadList[i].m_th.Abort();
                                                      RobThreadList[i] = null;
                                                  }
                                              }
                                              catch (Exception)
                                              {
                                              }
                                              RobThreadList.RemoveAt(i);
                                              break;
                                          }
                                      }
                                      if (!IsRun)
                                      {
                                          break;
                                      }
                                  }
                                  Thread.Sleep(500);
                                  System.Windows.Forms.Application.DoEvents();
                              }
                          });
                        m_SourceTh.Start();
                        //跳出外层循环
                        break;
                    }
                    Thread.Sleep(500);
                    System.Windows.Forms.Application.DoEvents();
                }
                #endregion

            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 停止扫描
        /// </summary>
        public void StopScan()
        {
            try
            {
                IsRun = false;//停止运行
                //终止子线程
                if (RobThreadList.Count > 0)
                {
                    for (int i = 0; i < RobThreadList.Count; i++)
                    {
                        if (RobThreadList[i] != null && RobThreadList[i].IsOver)
                        {
                            if (RobThreadList[i].m_th != null && RobThreadList[i].m_th.IsAlive)
                            {
                                RobThreadList[i].m_th.Abort();
                                RobThreadList[i].m_th = null;
                            }
                        }
                    }
                    RobThreadList.Clear(); ;
                }
                //终止主线程
                if (m_th != null && m_th.IsAlive)
                {
                    try
                    {
                        m_th.Abort();
                        m_th = null;
                    }
                    catch (Exception)
                    {
                    }
                }
                if (m_Log != null)
                {
                    m_Log(4, "抢票主程序已停止.....\r\n");
                }
                //终止清理资源线程
                if (m_SourceTh != null && m_SourceTh.IsAlive)
                {
                    try
                    {
                        m_SourceTh.Abort();
                        m_SourceTh = null;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
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
        /// 抢票处理
        /// </summary>
        private void ScanTicket()
        {
            try
            {
                if (lstCpyNo == null)
                {
                    PnrAnalysis.LogText.LogWrite("没有勾选落地运营商，扫描结束\r\n", "ScanTicketErr");
                    if (m_Log != null)
                    {
                        m_Log(4, "没有勾选落地运营商，扫描结束\r\n");
                    }
                    return;
                }
                while (IsRun)
                {
                    //1.哪些落地运营商开启了抢票功能 只扫描开启的  权限有（订单中 控台权限 当前程序）
                    //2.获取设置了抢票的订单 条件：起飞时间大于当天  设置了抢票标识 订单状态 行程类型
                    //3.订单生成后多少时间内的订单用于抢票扫描范围 超过时间范围内不用抢票
                    //4.根据订单数据查询航班 筛选航班 条件: 航空公司二字码 航班号  离起飞时间至少1个小时之后的航班
                    //5.根据航程匹配基础数据 获取指定舱位折扣 获取比当前订单折扣还要低的舱位 没有找到低折扣就停止扫描
                    //6.生成PNR
                    //7.生成订单
                    //8.通知用户
                    //9.抢到低折扣的舱位或者没有抢到 修改订单抢票状态 终止线程移除线程列表

                    //暂时只支持单程 往返和联程价格有问题(有待研究)    只查询白屏预订的抢票订单 
                    string sqlWhere = " TravelType=1 and OrderSourceType=1  and  OrderStatusCode in(1,3,4) and RobTicketStatus not in(0,2) and  RobTicketCount<=" + this.m_ScanCount +
                        //起飞时间大于当前时间 订单在设置的抢票时间范围内
                    " and AirTime> GetDate() and   dbo.GetRobJGTime(RobTicketCount,OwnerCpyNo,CreateTime,LastScanTime) >0";

                    List<Tb_Ticket_Order> OrderList = Manage.CallMethod("Tb_Ticket_Order", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_Order>;
                    if (OrderList != null && OrderList.Count > 0)
                    {
                        for (int i = 0; i < OrderList.Count; i++)
                        {
                            RobTicketHand(OrderList[i]);
                        }
                    }
                    //扫描时间间隔
                    Thread.Sleep(m_ScanTime * 1000);
                }//While结束
                //结束
                IsRun = false;
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("退出循环，抢票已停止,异常信息：" + ex.Message + "\r\r", "ScanTicket");
                IsRun = false;
            }
            finally
            {
                //如果结束
                if (!IsRun)
                {
                    //终止所有线程
                    try
                    {
                        if (RobThreadList.Count > 0)
                        {
                            for (int i = 0; i < RobThreadList.Count; i++)
                            {
                                if (RobThreadList[i] != null && RobThreadList[i].m_th != null && RobThreadList[i].m_th.IsAlive)
                                {
                                    RobThreadList[i].m_th.Abort();
                                    RobThreadList[i].m_th = null;
                                }
                            }
                            RobThreadList.Clear(); ;
                        }
                    }
                    catch (Exception)
                    {
                    }
                    m_Log(4, "\r\n扫描到抢票主程序已结束,请重新开启\r\n");
                }
            }
        }
        /// <summary>
        /// 处理抢票订单数据
        /// </summary>
        /// <param name="OMPM"></param>
        /// <returns></returns>
        public void RobTicketHand(Tb_Ticket_Order Order)
        {
            TimeSpan ts = System.DateTime.Now - Order.CreateTime;
            string LDCpyNo = Order.OwnerCpyNo.Length >= 12 ? Order.OwnerCpyNo.Substring(0, 12) : Order.OwnerCpyNo;
            if (SelGYTextList != null)
            {
                ListParam useLP = SelGYTextList.Find(delegate(ListParam tempLP)
                 {
                     return tempLP.CpyNo == LDCpyNo;
                 });
                if (useLP != null && useLP.RobInnerTime > 0)
                {
                    //抢票持续时间
                    if (ts.TotalMinutes < useLP.RobInnerTime)
                    {
                        //在指定抢票范围内               
                        Thread th = new Thread(new ParameterizedThreadStart(delegate(Object obj)
                        {
                            ManageThread tempMT = obj as ManageThread;
                            if (tempMT != null)
                            {
                                string CpyNo = tempMT.m_Order.OwnerCpyNo;
                                string KonZhiXT = GetGYParameters(CpyNo);
                                //是否开启抢票功能
                                if (KonZhiXT != null && KonZhiXT.Contains("|100|"))
                                {
                                    QueryFlightParam QFP = new QueryFlightParam();
                                    OrderMustParamModel OMPM = new OrderMustParamModel();
                                    OMPM.Order = tempMT.m_Order;
                                    List<Tb_Ticket_Passenger> PasList = Manage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new object[] { string.Format(" OrderId='{0}' order by PassengerType ", Order.OrderId) }) as List<Tb_Ticket_Passenger>;
                                    OMPM.PasList = PasList;
                                    List<Tb_Ticket_SkyWay> SkyList = Manage.CallMethod("Tb_Ticket_SkyWay", "GetList", null, new object[] { string.Format("OrderId='{0}' order by FromDate ", Order.OrderId) }) as List<Tb_Ticket_SkyWay>;
                                    OMPM.SkyList = SkyList;
                                    QFP.strLog = "\r\n扫描到抢票订单" + Order.OrderId + " 所属公司编号:" + Order.OwnerCpyNo + "开始处理=====================================================================\r\n";
                                    m_Log(4, QFP.strLog);
                                    QFP.sbLog.Append(QFP.strLog);
                                    QFP.m_thManage = tempMT;
                                    QFP.KonZhiXT = KonZhiXT;
                                    QFP.m_OrderInput = OMPM;
                                    //处理订单                               
                                    ScanRobOrder(QFP);
                                    QFP.strLog = "\r\n扫描到抢票订单" + Order.OrderId + "结束处理=====================================================================\r\n";
                                    QFP.sbLog.Append(QFP.strLog);
                                    m_Log(4, QFP.strLog);
                                }
                            }
                        }));
                        ManageThread MT = new ManageThread();
                        MT.m_th = th;
                        MT.m_Order = Order;
                        MT.IsOver = false;
                        MT.m_strThreadName = "线程名称:" + Order.OrderId;
                        //开启扫描线程
                        th.Start(MT);
                        //加入线程列表
                        RobThreadList.Add(MT);
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        //不在抢票时间范围内 抢票过期 修改抢票状态 失败
                        UpdateOrderStatus(Order, 3);
                        m_Log(4, "\r\n扫描订单" + Order.OrderId + " 已超过抢票订单时间范围，抢票过期，扫描失败！\r\n");
                    }
                }
                else
                {
                    m_Log(4, "\r\n落地运营商数据不能为空 useLP\r\n");
                }
            }
            else
            {
                m_Log(4, "\r\n落地运营商数据不能为空 SelGYTextList\r\n");
            }
        }

        /// <summary>
        /// 修改抢票状态 1.抢票中 2.抢票成功 3.抢票失败
        /// </summary>
        /// <param name="Order"></param>
        /// <param name="RobTicketStatus"></param>
        /// <returns></returns>
        public bool UpdateOrderStatus(Tb_Ticket_Order Order, int RobTicketStatus)
        {
            bool IsSuc = false;
            try
            {
                Order.RobTicketStatus = RobTicketStatus;
                string sqlUpdate = string.Format(" update Tb_Ticket_Order set RobTicketStatus={0},RobTicketCount=RobTicketCount+1 where id='{1}' ", RobTicketStatus, Order.id);
                IsSuc = Manage.ExecuteNonQuerySQLInfo(sqlUpdate);
            }
            catch (Exception)
            {
            }
            return IsSuc;
        }
        /// <summary>
        /// 订单对比 查找低于原订单的折扣 还有没有舱位
        /// </summary>
        /// <param name="Pm"></param>
        public void ScanRobOrder(QueryFlightParam QFP)
        {
            OrderMustParamModel Pm = QFP.m_OrderInput;
            Tb_Ticket_Order oldOrder = Pm.Order;
            List<Tb_Ticket_SkyWay> skyList = Pm.SkyList;
            List<Tb_Ticket_Passenger> pasList = Pm.PasList;

            Tb_Ticket_Passenger PasModel = pasList.Find(delegate(Tb_Ticket_Passenger TempPas)
            {
                return TempPas.PassengerType != 3;
            });
            ConfigParam config = null;
            string GYCpyNo = string.Empty;
            try
            {
                //设置数据到实体参数
                SetUseParam(oldOrder.OwnerCpyNo, QFP);
                GYCpyNo = oldOrder.OwnerCpyNo.Trim().Length >= 12 ? oldOrder.OwnerCpyNo.Substring(0, 12) : oldOrder.OwnerCpyNo;
                if (GYCpyNo != "" && QFP != null)
                {

                    #region 航班查询
                    //配置信息
                    config = QFP.m_Config;
                    int TravelType = oldOrder.TravelType;
                    int Port = 0;
                    int.TryParse(config.WhiteScreenPort, out Port);
                    string midCityCode = "";
                    //航空公司二字码
                    QFP.CarryCode = oldOrder.CarryCode.Split('/')[0];
                    QFP.IsCHDToAudltTK = oldOrder.IsCHDETAdultTK;
                    QFP.AllowChangePNRFlag = oldOrder.AllowChangePNRFlag;
                    decimal CurrSpacePrice = PasModel.PMFee;
                    DataRow m_fromDr = null;//第一程航班数据
                    DataRow m_ToDr = null;//第二程航班数据
                    DataSet BaseData = null;//匹配基础数据后的结果数据
                    for (int i = 0; i < skyList.Count; i++)
                    {
                        Tb_Ticket_SkyWay skyway = skyList[i];
                        PbProject.Model.definitionParam.SelectCityParams selectCityParams = new Model.definitionParam.SelectCityParams();
                        //传入参数
                        selectCityParams.cairry = skyway.CarryCode.ToUpper();
                        selectCityParams.fcity = skyway.FromCityCode.ToUpper();
                        selectCityParams.mcity = midCityCode;
                        selectCityParams.tcity = skyway.ToCityCode.ToUpper();
                        selectCityParams.time = skyway.FromDate.ToString("yyyy-MM-dd HH:mm:ss");
                        selectCityParams.Totime = skyway.ToDate.ToString("yyyy-MM-dd HH:mm:ss");
                        selectCityParams.mEmployees = QFP.m_User;
                        selectCityParams.mCompany = QFP.m_Company;
                        selectCityParams.TravelType = TravelType;
                        selectCityParams.IsShowGX = false;
                        if (i == 0)
                        {
                            QFP.strLog = "\r\n  【去程】开始查询航班【航空公司:" + selectCityParams.cairry + " 出发城市:" + selectCityParams.fcity + " 到达城市:" + selectCityParams.tcity + " 出发时间:" + selectCityParams.time + "】 \r\n";
                            m_Log(4, QFP.strLog);
                        }
                        else
                        {
                            QFP.strLog = "\r\n  【回程】开始查询航班【航空公司:" + selectCityParams.cairry + " 出发城市:" + selectCityParams.fcity + " 到达城市:" + selectCityParams.tcity + " 出发时间:" + selectCityParams.time + " \r\n";
                            m_Log(4, QFP.strLog);
                        }
                        QFP.sbLog.Append(QFP.strLog);
                        //用于计时
                        Stopwatch m_Watch = new Stopwatch();
                        m_Watch.Start();
                        //查航班  匹配基础数据
                        //根据抢票订单信息 查找匹配基础舱位折扣的中的价格是否有低于该仓位的价格 有就生成PNR 生成新订单 最后可以发送短信通知分销商
                        AirQurey FlightQuery = new AirQurey(QFP.m_BaseParam, QFP.m_User, QFP.m_Company);
                        BaseData = FlightQuery.RobTicketFlightQuery(selectCityParams);
                        if (BaseData != null && BaseData.Tables.Count > 0)
                        {
                            List<DataRow> drList = new List<DataRow>();
                            string strAirFlight = skyway.CarryCode.ToUpper() + skyway.FlightCode.ToUpper();
                            DateTime dtDefault = System.DateTime.Now;
                            //找出该条航线有没有低折扣的或者比较该舱位价低的折扣
                            foreach (DataTable AirFlight in BaseData.Tables)
                            {
                                //找出改航空公司和航班号的数据
                                if (AirFlight.TableName.ToUpper() == strAirFlight)
                                {
                                    foreach (DataRow dr in AirFlight.Rows)
                                    {
                                        if (dr["StartDate"] != DBNull.Value && dr["StartTime"] != DBNull.Value)
                                        {
                                            //提前一个小时的起飞时间
                                            DateTime FromDate = DateTime.Parse(dr["StartDate"].ToString() + " " + dr["StartTime"].ToString() + ":00").AddHours(-1);
                                            //过滤掉
                                            if (dtDefault > FromDate)
                                            {
                                                continue;
                                            }
                                        }
                                        //.....过滤条件


                                        //加入符合条件的航班到集合
                                        drList.Add(dr);
                                    }
                                }
                            }//end foreach 循环航班数据
                            if (drList.Count > 0)
                            {
                                //寻找低于当前舱位价或者折扣的舱位
                                if (i == 0)
                                {
                                    m_fromDr = GetLowerSpace(CurrSpacePrice, drList);
                                    if (m_fromDr != null)
                                    {
                                        QFP.strLog = "\r\n 订单号:" + oldOrder.OrderId + " 【去程】航班找到低于当前舱位" + skyway.Space + "的折扣 舱位为" + m_fromDr["Space"].ToString() + " 继续生成编码和订单\r\n";
                                    }
                                    else
                                    {
                                        QFP.strLog = "\r\n 订单号:" + oldOrder.OrderId + " 【去程】航班没有找到低于当前舱位" + skyway.Space + "的折扣 扫描结束\r\n";
                                        //订单日志
                                        OrderLog(oldOrder.OrderId, "抢票失败,失败原因:未能找到低于该订单的舱位！", QFP);
                                    }
                                }
                                else
                                {
                                    m_ToDr = GetLowerSpace(CurrSpacePrice, drList);
                                    if (m_ToDr != null)
                                    {
                                        QFP.strLog = "\r\n 订单号:" + oldOrder.OrderId + " 【回程】航班找到低于当前舱位" + skyway.Space + "的折扣 舱位为" + m_ToDr["Space"].ToString() + " 继续生成编码和订单\r\n";
                                    }
                                    else
                                    {
                                        QFP.strLog = "\r\n 订单号:" + oldOrder.OrderId + " 【回程】航班没有找到低于当前舱位" + skyway.Space + "的折扣 扫描结束\r\n";
                                        //订单日志
                                        OrderLog(oldOrder.OrderId, "抢票失败,失败原因:未能找到低于该订单的舱位！", QFP);
                                    }
                                }
                                m_Log(4, QFP.strLog);
                                QFP.sbLog.Append(QFP.strLog);
                                drList.Clear();
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    QFP.strLog = "\r\n 订单号:" + oldOrder.OrderId + " 【去程】航班没有找到符合条件的航班数据 扫描结束\r\n";
                                    OrderLog(oldOrder.OrderId, "抢票失败,失败原因:未能找到低于该订单的舱位！", QFP);
                                }
                                else
                                {
                                    QFP.strLog = "\r\n 订单号:" + oldOrder.OrderId + " 【回程】航班没有找到符合条件的航班数据 扫描结束\r\n";
                                    OrderLog(oldOrder.OrderId, "抢票失败,失败原因:未能找到低于该订单的舱位！", QFP);
                                }
                                m_Log(4, QFP.strLog);
                                QFP.sbLog.Append(QFP.strLog);
                            }
                        }
                        //计时结束
                        m_Watch.Stop();
                        TimeSpan ts = m_Watch.Elapsed;
                        QFP.strLog = "\r\n 方法名:ScanRobOrder 查询航班结束(" + (BaseData == null ? "失败" : "成功") + ") 用时:" + ts.ToString() + " \r\n";
                        m_Log(4, QFP.strLog);
                        QFP.sbLog.Append(QFP.strLog);
                    }//end for 循环航段
                    #endregion

                    #region 生成编码
                    //记录Pnr日志Id=
                    List<string> pnrLogList = new List<string>();
                    //生成编码内部错误
                    string InnerError = "";
                    if (skyList.Count == 1)
                    {
                        if (m_fromDr != null)
                        {
                            //生成新编码
                            CreatePnr(QFP, TravelType, m_fromDr, m_ToDr, ref pnrLogList, out InnerError);
                        }
                        else
                        {
                            QFP.strLog = "\r\n 订单号:" + oldOrder.OrderId + "  " + (TravelType == 2 ? "往返" : TravelType == 3 ? "中转联程" : "单程") + "查询航班失败，扫描结束 \r\n";
                        }
                    }
                    else if (skyList.Count == 2)
                    {
                        if (m_fromDr != null && m_ToDr != null)
                        {
                            //生成新编码
                            CreatePnr(QFP, TravelType, m_fromDr, m_ToDr, ref pnrLogList, out InnerError);
                        }
                        else
                        {
                            m_Log(4, "\r\n 订单号:" + oldOrder.OrderId + "  " + (TravelType == 2 ? "往返" : TravelType == 3 ? "中转联程" : "单程") + "查询航班失败，扫描结束 \r\n");
                        }
                    }
                    else
                    {
                        QFP.strLog = "\r\n订单号:" + oldOrder.OrderId + "  航班查询暂时不支持多程，扫描结束 \r\n";
                    }
                    m_Log(4, QFP.strLog);
                    QFP.sbLog.Append(QFP.strLog);
                    #endregion

                    #region 生成订单

                    if (InnerError == "" && m_fromDr != null)
                    {
                        //成人编码或者儿童编码
                        if (QFP.PnrInfo != null && (oldOrder.IsChdFlag && QFP.PnrInfo.childPnr != "") || (!oldOrder.IsChdFlag && QFP.PnrInfo.AdultPnr != ""))
                        {
                            QFP.strLog = "\r\n订单号:" + oldOrder.OrderId + "  生成" + (oldOrder.IsChdFlag ? "儿童" : "成人") + "编码成功:PNR=" + (oldOrder.IsChdFlag ? QFP.PnrInfo.childPnr : QFP.PnrInfo.AdultPnr) + "  继续下一步生成订单\r\n";
                            CreateOrder(QFP, TravelType, m_fromDr, m_ToDr, pnrLogList);
                        }
                        else
                        {
                            QFP.strLog = "\r\n订单号:" + oldOrder.OrderId + "  生成编码失败，扫描结束 \r\n";
                        }
                    }
                    else
                    {
                        QFP.strLog = "\r\n 订单号:" + oldOrder.OrderId + "  " + (TravelType == 2 ? "往返" : TravelType == 3 ? "中转联程" : "单程") + "生成编码：" + InnerError + " \r\n";
                        OrderLog(oldOrder.OrderId, "抢票失败,失败原因:未生成生成编码," + InnerError + "", QFP);
                    }
                    m_Log(4, QFP.strLog);
                    QFP.sbLog.Append(QFP.strLog);
                    #endregion

                    //清理
                    if (BaseData != null)
                    {
                        BaseData.Dispose();
                        BaseData = null;
                        pnrLogList.Clear();
                    }
                }
                else
                {
                    QFP.strLog = "\r\n 方法名:ScanRobOrder 扫描订单" + oldOrder.OrderId + "公司编号为空 或者 QueryFlightParam=null \r\n";
                    m_Log(4, QFP.strLog);
                    QFP.sbLog.Append(QFP.strLog);
                }
            }
            catch (Exception ex)
            {
                QFP.strLog = "\r\n 方法名:ScanRobOrder 扫描订单" + oldOrder.OrderId + "异常！异常信息:" + ex.Message + " \r\n";
                m_Log(4, QFP.strLog);
                QFP.sbLog.Append(QFP.strLog);
                QFP.IsRobSuccess = false;
            }
            finally
            {
                //更新抢票状态
                UpdateOrderStatus(oldOrder, QFP.IsRobSuccess ? 2 : 3);
                //线程运行结束标志
                QFP.m_thManage.IsOver = true;
                GC.Collect();//强制释放资源
                QFP.Dispose();
                PnrAnalysis.LogText.LogWrite("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + QFP.sbLog.ToString() + "\r\r", "RobTicketFull");
            }
        }
        /// <summary>
        /// 找出比当前订单折扣低的舱位
        /// </summary>
        /// <returns></returns>
        public DataRow GetLowerSpace(decimal CurrPrice, List<DataRow> drList)
        {
            DataRow m_dr = null;
            decimal tempPrice = CurrPrice;
            decimal tempPrice1 = 0;
            if (drList != null && drList.Count > 0)
            {
                //找出其中舱位价最小的数据
                foreach (DataRow dr in drList)
                {
                    if (dr["XSFee"] != DBNull.Value)
                    {
                        if (decimal.TryParse(dr["XSFee"].ToString(), out tempPrice1))
                        {
                            if (tempPrice1 < tempPrice)
                            {
                                tempPrice = tempPrice1;
                                m_dr = dr;
                            }
                        }
                    }
                }
                if (tempPrice >= CurrPrice)
                {
                    m_dr = null;
                }
            }
            return m_dr;
        }
        /// <summary>
        /// 生成编码
        /// </summary>
        /// <param name="CarryCode">航空公司二字码</param>
        /// <param name="Office">预订编码Office</param>
        /// <param name="AdultPnr">儿童订单中儿童编码备注的成人编码</param>
        /// <param name="IsChdETDZAudltTK">是否儿童出成人票</param>
        /// <param name="pList">乘机人列表</param>
        /// <param name="skywaylist">航段列表</param>
        /// <returns></returns>
        public void CreatePnr(QueryFlightParam QFP, int TravelType, DataRow m_FromDr, DataRow m_ToDr, ref List<string> pnrLogList, out string ErrMsg)
        {
            ErrMsg = "";
            List<IPassenger> pasList = new List<IPassenger>();
            List<ISkyLeg> skyList = new List<ISkyLeg>();
            SendNewPID pid = new SendNewPID();
            PnrParamObj PnrParam = new PnrParamObj();
            //必填项 是否开启新版PID发送指令 
            PnrParam.UsePIDChannel = QFP.KonZhiXT != null && QFP.KonZhiXT.Contains("|48|") ? 2 : 0;
            PnrParam.ServerIP = QFP.m_Config.WhiteScreenIP;
            PnrParam.ServerPort = int.Parse(string.IsNullOrEmpty(QFP.m_Config.WhiteScreenPort) ? "0" : QFP.m_Config.WhiteScreenPort);
            PnrParam.Office = QFP.m_Config.Office.Split('^')[0];
            PnrParam.CarryCode = QFP.CarryCode;
            PnrParam.PasList = pasList;
            PnrParam.SkyList = skyList;
            //是否儿童出成人票
            PnrParam.ChildIsAdultEtdz = "0";
            //可选项
            PnrParam.UserName = QFP.m_User != null ? QFP.m_User.LoginName : "";

            //输入的手机号码 预订编码CT项电话
            PnrParam.CTTel = "028-55555555";
            PnrParam.CTCTPhone = "15928636274";


            PnrParam.PID = QFP.m_Config.Pid;
            PnrParam.KeyNo = QFP.m_Config.KeyNo;
            //是否存在婴儿
            bool IsExistINF = false;
            List<Tb_Ticket_Passenger> pList = QFP.m_OrderInput.PasList;
            //乘机人
            foreach (Tb_Ticket_Passenger pas in pList)
            {
                IPassenger p1 = new IPassenger();
                pasList.Add(p1);
                p1.PassengerName = pas.PassengerName;
                p1.PassengerType = pas.PassengerType;
                p1.PasSsrCardID = pas.Cid;
                p1.CpyandNo = pas.A8;
                if (pas.PassengerType == 3)
                {
                    IsExistINF = true;
                }
            }
            //航段
            ISkyLeg leg1 = new ISkyLeg();
            //去程
            if (m_FromDr != null)
            {
                skyList.Add(leg1);
                leg1.CarryCode = m_FromDr["CarrCode"].ToString();
                leg1.FlightCode = m_FromDr["FlightNo"].ToString();
                leg1.FlyStartTime = m_FromDr["StartTime"].ToString().Replace(":", "");
                leg1.FlyEndTime = m_FromDr["EndTime"].ToString().Replace(":", "");
                leg1.FlyStartDate = m_FromDr["StartDate"].ToString();
                leg1.fromCode = m_FromDr["StartCityCode"].ToString();
                leg1.toCode = m_FromDr["ToCityCode"].ToString();
                leg1.Space = m_FromDr["Space"].ToString();
                leg1.Discount = "0";
            }
            //回程
            if (m_ToDr != null)
            {
                skyList.Add(leg1);
                leg1.CarryCode = m_ToDr["CarrCode"].ToString();
                leg1.FlightCode = m_ToDr["FlightNo"].ToString();
                leg1.FlyStartTime = m_ToDr["StartTime"].ToString().Replace(":", "");
                leg1.FlyEndTime = m_ToDr["EndTime"].ToString().Replace(":", "");
                leg1.FlyStartDate = m_ToDr["StartDate"].ToString();
                leg1.fromCode = m_ToDr["StartCityCode"].ToString();
                leg1.toCode = m_ToDr["ToCityCode"].ToString();
                leg1.Space = m_ToDr["Space"].ToString();
                leg1.Discount = "0";
            }
            //生成编码
            RePnrObj pObj = pid.ISendIns(PnrParam);
            //赋给实体
            QFP.PnrInfo = pObj;
            //记录指令
            SaveInsInfo(pObj, QFP.m_User, QFP.m_Company);
            //出票Office
            string PrintOffice = GetPrintOffice(QFP.m_Company.UninCode, QFP.CarryCode);
            QFP.PnrInfo.PrintOffice = PrintOffice;
            string AdultPnr = string.Empty;
            string childPnr = string.Empty;
            //成人预订信息编码记录            
            if (QFP.PnrInfo.AdultYudingList.Count > 0)
            {
                AdultPnr = QFP.PnrInfo.AdultPnr;
                if (string.IsNullOrEmpty(AdultPnr) || AdultPnr.Trim().Length != 6)
                {
                    //提示pnr失败信息
                    string yudingRecvData = QFP.PnrInfo.AdultYudingList.Values[0];
                    ErrMsg = ShowPnrFailInfo(1, yudingRecvData, QFP.PnrInfo);
                    AdultPnr = "";
                }
                //记录编码日志
                YuDingPnrLog(QFP, QFP.PnrInfo.AdultYudingList.Keys[0], QFP.PnrInfo.AdultYudingList.Values[0], AdultPnr, QFP.PnrInfo.Office, ref pnrLogList);
                if (AdultPnr.Length == 6)
                {
                    if ((QFP.PnrInfo.PatModelList[0] == null || QFP.PnrInfo.PatModelList[0].PatList.Count == 0))
                    {
                        ErrMsg = "成人编码" + AdultPnr + "未能PAT取到价格，原因如下:<br />" + QFP.PnrInfo.PatList[0];
                    }
                    //婴儿PAT
                    if (IsExistINF && (QFP.PnrInfo.PatModelList[2] == null || QFP.PnrInfo.PatModelList[2].PatList.Count == 0))
                    {
                        ErrMsg = "成人编码" + AdultPnr + "未能PAT取到婴儿价格，原因如下:<br />" + QFP.PnrInfo.PatList[2];
                    }
                }
            }
            //儿童预订信息编码记录
            if (QFP.PnrInfo.ChildYudingList.Count > 0)
            {
                childPnr = QFP.PnrInfo.childPnr;
                if (string.IsNullOrEmpty(childPnr) || childPnr.Trim().Length != 6)
                {
                    //提示pnr失败信息
                    string yudingRecvData = QFP.PnrInfo.ChildYudingList.Values[0];
                    ErrMsg = ShowPnrFailInfo(2, yudingRecvData, QFP.PnrInfo);
                    childPnr = "";
                }
                //记录编码日志
                YuDingPnrLog(QFP, QFP.PnrInfo.ChildYudingList.Keys[0], QFP.PnrInfo.ChildYudingList.Values[0], childPnr, QFP.PnrInfo.Office, ref pnrLogList);
                if (childPnr.Length == 6)
                {
                    if ((QFP.PnrInfo.PatModelList[1] == null || QFP.PnrInfo.PatModelList[1].PatList.Count == 0))
                    {
                        ErrMsg = "儿童编码" + childPnr + "未能PAT取到价格，原因如下:<br />" + QFP.PnrInfo.PatList[1];
                    }
                    //是否儿童出成人票
                    if (QFP.IsCHDToAudltTK == 1 && (QFP.PnrInfo.CHDToAdultPat == null || QFP.PnrInfo.CHDToAdultPat.PatList.Count == 0))
                    {
                        ErrMsg = "儿童编码" + childPnr + "未能PAT取到价格，原因如下:<br />" + QFP.PnrInfo.CHDToAdultPatCon.Trim();
                    }
                }
            }
        }
        /// <summary>
        /// 保存指令信息到数据库
        /// </summary>
        /// <returns></returns>
        public bool SaveInsInfo(RePnrObj PnrInfo, User_Employees m_user, User_Company m_company)
        {
            bool IsSuc = true;
            string errMsg = "";
            try
            {
                List<string> sqlList = new List<string>();
                if (PnrInfo != null && PnrInfo.InsList.Count > 0)
                {
                    //一组指令ID
                    string GroupID = System.DateTime.Now.Ticks.ToString();
                    DateTime _sendtime = Convert.ToDateTime("1900-01-01");
                    DateTime _recvtime = Convert.ToDateTime("1900-01-01");
                    string UserAccount = m_user.LoginName, CpyNo = m_company.UninCode, serverIPPort = PnrInfo.ServerIP + ":" + PnrInfo.ServerPort, Office = PnrInfo.Office;
                    string[] strArr = null;
                    List<string> Removelist = new List<string>();
                    Removelist.Add("id");
                    foreach (KeyValuePair<string, string> KV in PnrInfo.InsList)
                    {
                        strArr = KV.Key.Split(new string[] { PnrInfo.SplitChar }, StringSplitOptions.None);
                        if (strArr.Length == 4)
                        {
                            Tb_SendInsData ins = new Tb_SendInsData();
                            ins.SendIns = strArr[0];
                            if (DateTime.TryParse(strArr[1], out _sendtime))
                            {
                                ins.SendTime = _sendtime;
                            }
                            if (DateTime.TryParse(strArr[2], out _recvtime))
                            {
                                ins.RecvTime = _recvtime;
                            }
                            if (strArr[3] != "")
                            {
                                ins.Office = strArr[3];
                            }
                            ins.RecvData = KV.Value;
                            ins.Office = Office;
                            ins.ServerIPAndPort = serverIPPort + "|" + GroupID;
                            ins.UserAccount = UserAccount;
                            ins.CpyNo = CpyNo;
                            ins.SendInsType = 14;//抢票
                            sqlList.Add(PbProject.Dal.Mapping.MappingHelper<Tb_SendInsData>.CreateInsertModelSql(ins, Removelist));
                        }
                    }
                    if (sqlList.Count > 0)
                    {
                        IsSuc = Manage.ExecuteSqlTran(sqlList, out errMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                IsSuc = false;
                errMsg = ex.Message + ex.StackTrace.ToString();
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:SaveInsInfo】================================================================\r\n 异常信息:" + errMsg + "\r\n", "SpPatAjax");
            }
            return IsSuc;
        }

        /// <summary>
        /// 预订编码日志
        /// </summary>
        /// <param name="SSContent"></param>
        /// <param name="ResultContent"></param>
        /// <param name="Pnr"></param>
        /// <param name="pnrLogList"></param>
        /// <returns></returns>
        public bool YuDingPnrLog(QueryFlightParam QFP, string SSContent, string ResultContent, string Pnr, string Office, ref List<string> pnrLogList)
        {
            bool Insert = false;
            try
            {
                Log_Pnr logpnr = new Log_Pnr();
                logpnr.CpyNo = QFP.m_Company.UninCode;
                logpnr.CpyName = QFP.m_Company.UninAllName;
                logpnr.CpyType = QFP.m_Company.RoleType;
                logpnr.OperTime = System.DateTime.Now;
                logpnr.OperLoginName = QFP.m_User.LoginName;
                logpnr.OperUserName = QFP.m_User.UserName;
                logpnr.SSContent = SSContent;
                logpnr.ResultContent = ResultContent;
                logpnr.PNR = Pnr;
                logpnr.OfficeCode = Office;
                logpnr.A7 = QFP.PnrInfo.ServerIP + "|" + QFP.PnrInfo.ServerPort;//IP和端口
                Insert = (bool)Manage.CallMethod("Log_Pnr", "Insert", null, new object[] { logpnr });
                if (Insert)
                {
                    pnrLogList.Add("'" + logpnr.id.ToString() + "'");
                }
            }
            catch (Exception ex)
            {
                DataBase.LogCommon.Log.Error("Create.aspx页面YuDingPnrLog", ex);
            }
            return Insert;
        }

        /// <summary>
        /// 获取航空公司出票Office号 
        /// </summary>
        /// <param name="CarryCode"></param>
        /// <param name="defaultOffice"></param>
        /// <returns></returns>
        public string GetPrintOffice(string CpyNo, string CarryCode)
        {
            string PrintOffice = "";
            string sqlWhere = string.Format(" CpyNo='{0}' and AirCode='{1}' ", CpyNo, CarryCode);
            List<Tb_Ticket_PrintOffice> list = Manage.CallMethod("Tb_Ticket_PrintOffice", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_PrintOffice>;
            if (list != null && list.Count > 0)
            {
                if (!string.IsNullOrEmpty(list[0].OfficeCode))
                {
                    PrintOffice = list[0].OfficeCode;
                }
            }
            return PrintOffice;
        }

        /// <summary>
        /// 提示编码生成失败原因
        /// </summary>
        /// <param name="type"></param>
        /// <param name="yudingRecvData"></param>
        /// <returns></returns>
        public string ShowPnrFailInfo(int type, string yudingRecvData, RePnrObj PnrInfo)
        {
            string msg = "";
            if (yudingRecvData.ToUpper().Contains("PLS NM1XXXX/XXXXXX"))
            {
                //PLS NM1XXXX/XXXXXX
                msg = "乘客输入姓名格式错误！原因如下:<br />（" + yudingRecvData.ToUpper() + "）姓名中应加斜线(/),或斜线数量不正确!";
            }
            else if (yudingRecvData.ToUpper().Contains("UNABLE TO SELL.PLEASE") || yudingRecvData.ToUpper().Contains("SEATS"))
            {
                msg = "座位数不足或座位已销售完,请重新预定!";
            }
            else if (yudingRecvData.Contains("不支持的汉字"))
            {
                msg = "乘机姓名中存在航信不支持的汉字，请仔细检查！";
            }
            else if (yudingRecvData.IndexOf("地址无效") != -1 || yudingRecvData.IndexOf("无法从传输连接中读取数据") != -1 || yudingRecvData.IndexOf("无法连接") != -1 || yudingRecvData.IndexOf("强迫关闭") != -1 || yudingRecvData.IndexOf("由于") != -1)
            {
                msg = "与航信连接失败，请重新预订！<br />错误信息:" + yudingRecvData;
            }
            else if (yudingRecvData.IndexOf("超时！") != -1 || yudingRecvData.IndexOf("服务器忙") != -1)
            {
                msg = "系统繁忙,请稍后再试！";
            }
            else if (yudingRecvData.ToUpper().Contains("WSACancelBlockingCall") || yudingRecvData.ToUpper().Contains("TRANSACTION IN PROGRESS") || yudingRecvData.ToUpper().Contains(" FORMAT ") || yudingRecvData.ToUpper().Contains("NO PNR") || yudingRecvData.ToUpper().Contains("CHECK TKT DATE") || yudingRecvData.ToUpper().Contains("为空") || yudingRecvData.ToUpper().Contains("对象的实例"))
            {
                msg = (type == 1 ? "成人" : "儿童") + "编码生成失败！原因如下:<br />" + yudingRecvData;
            }
            else
            {
                string AdultYuDing = "", CHDYuDing = "";
                if (PnrInfo.AdultYudingList.Count > 0)
                {
                    foreach (string key in PnrInfo.AdultYudingList.Keys)
                    {
                        AdultYuDing = PnrInfo.AdultYudingList[key].ToString();
                    }
                }
                if (PnrInfo.ChildYudingList.Count > 0)
                {
                    foreach (string key in PnrInfo.ChildYudingList.Keys)
                    {
                        CHDYuDing = PnrInfo.ChildYudingList[key].ToString();
                    }
                }
                msg = (type == 1 ? "成人" : "儿童") + "编码生成失败！" + (type == 1 ? AdultYuDing : CHDYuDing);
            }
            return msg;
        }
        /// <summary>
        /// 生成订单
        /// </summary>
        /// <param name="QFP"></param>
        /// <param name="TravelType"></param>
        /// <param name="m_FromDr"></param>
        /// <param name="m_ToDr"></param>
        /// <param name="pnrLogList"></param>
        /// <param name="Log"></param>
        /// <returns></returns>
        public bool CreateOrder(QueryFlightParam QFP, int TravelType, DataRow m_FromDr, DataRow m_ToDr, List<string> pnrLogList)
        {
            bool IsSuc = false;
            try
            {
                //订单管理
                Tb_Ticket_OrderBLL OrderManage = new Tb_Ticket_OrderBLL();
                OrderInputParam OrderParam = new OrderInputParam();
                OrderMustParamModel ParamModel = new OrderMustParamModel();
                //加入集合
                OrderParam.OrderParamModel.Add(ParamModel);
                Log_Tb_AirOrder logOrder = new Log_Tb_AirOrder();
                logOrder.OperTime = DateTime.Now;
                logOrder.OperType = "抢票订单";
                logOrder.OperContent = "系统于" + logOrder.OperTime + "创建抢票订单，原订单号:" + QFP.m_OrderInput.Order.OrderId + "  原编码:" + QFP.m_OrderInput.Order.PNR;
                logOrder.OperLoginName = "系统管理员";
                logOrder.OperUserName = "系统管理员";
                logOrder.CpyNo = QFP.m_Company.UninCode;
                logOrder.CpyName = QFP.m_Company.UninName;
                logOrder.CpyType = QFP.m_Company.RoleType;
                logOrder.WatchType = 5;
                //以下全部为赋值
                OrderParam.PnrInfo = QFP.PnrInfo;//编码信息
                ParamModel.LogOrder = logOrder;//订单日志
                List<Tb_Ticket_Passenger> pasList = QFP.m_OrderInput.PasList;
                List<Tb_Ticket_SkyWay> skyList = QFP.m_OrderInput.SkyList;
                Tb_Ticket_Order NewOrder = new Tb_Ticket_Order();

                //修改数据
                //...
                DataAction d = new DataAction();
                #region 乘机人
                decimal YFare = 0;//Y舱位价
                decimal SpacePrice = 0;//舱位价
                decimal ABFee = 0;//基建
                decimal FuelFee = 0; //燃油            
                decimal.TryParse(m_FromDr["FareFee"].ToString(), out YFare);
                decimal.TryParse(m_FromDr["ABFee"].ToString(), out ABFee);
                decimal.TryParse(m_FromDr["FuelAdultFee"].ToString(), out FuelFee);
                decimal.TryParse(m_FromDr["XSFee"].ToString(), out SpacePrice);
                //订单中的总价
                decimal TotalPMPrice = 0m, TotalABFare = 0, TotalRQFare = 0m;
                for (int i = 0; i < pasList.Count; i++)
                {
                    Tb_Ticket_Passenger tpasser = pasList[i];
                    //舱位价
                    tpasser.PMFee = SpacePrice;
                    tpasser.TGQHandlingFee = 0;
                    //婴儿
                    if (tpasser.PassengerType == 3)
                    {
                        decimal _TempFare = (0.1m * YFare) / 10;
                        tpasser.PMFee = d.FourToFiveNum(_TempFare, 0) * 10;
                        tpasser.ABFee = 0m;
                        tpasser.FuelFee = 0m;
                    }
                    else
                    {
                        tpasser.ABFee = ABFee;
                        tpasser.FuelFee = FuelFee;
                        //儿童
                        if (tpasser.PassengerType == 2)
                        {
                            //不是儿童出成人票的儿童
                            if (QFP.IsCHDToAudltTK != 1)
                            {
                                tpasser.ABFee = 0m;
                                tpasser.FuelFee = (0.5m) * FuelFee;
                                tpasser.FuelFee = d.FourToFiveNum(tpasser.FuelFee / 10, 0) * 10;
                                tpasser.PMFee = d.FourToFiveNum(((0.5m) * YFare) / 10, 0) * 10;
                            }
                        }
                    }
                    //总舱位价
                    TotalPMPrice += tpasser.PMFee;
                    //总机建费
                    TotalABFare += tpasser.ABFee;
                    //总燃油费
                    TotalRQFare += tpasser.FuelFee;
                }
                #endregion

                #region 航段信息
                YFare = 0;//Y舱位价
                SpacePrice = 0;//舱位价
                ABFee = 0;//基建
                FuelFee = 0; //燃油   
                List<string> SpaceList = new List<string>();
                List<string> DisacountList = new List<string>();
                List<string> CarryCodeList = new List<string>();
                List<string> FlightCodeList = new List<string>();
                List<string> TravelList = new List<string>();
                List<string> AirTimeList = new List<string>();
                List<string> TravelCodeList = new List<string>();
                for (int i = 0; i < skyList.Count; i++)
                {
                    Tb_Ticket_SkyWay SkyWay = skyList[i];
                    if (i == 0)
                    {
                        SkyWay.Space = m_FromDr["Space"].ToString();
                        SkyWay.Discount = m_FromDr["DiscountRate"].ToString();
                        decimal.TryParse(m_FromDr["FareFee"].ToString(), out YFare);
                        decimal.TryParse(m_FromDr["ABFee"].ToString(), out ABFee);
                        decimal.TryParse(m_FromDr["FuelAdultFee"].ToString(), out FuelFee);
                        decimal.TryParse(m_FromDr["XSFee"].ToString(), out SpacePrice);
                        SkyWay.ABFee = ABFee;
                        SkyWay.FuelFee = FuelFee;
                        SkyWay.FareFee = YFare;
                        SkyWay.SpacePrice = SpacePrice;
                    }
                    else
                    {
                        if (m_ToDr != null)
                        {
                            SkyWay.Space = m_ToDr["Space"].ToString();
                            SkyWay.Discount = m_ToDr["DiscountRate"].ToString();
                            decimal.TryParse(m_ToDr["FareFee"].ToString(), out YFare);
                            decimal.TryParse(m_ToDr["ABFee"].ToString(), out ABFee);
                            decimal.TryParse(m_ToDr["FuelAdultFee"].ToString(), out FuelFee);
                            decimal.TryParse(m_ToDr["XSFee"].ToString(), out SpacePrice);
                            SkyWay.ABFee = ABFee;
                            SkyWay.FuelFee = FuelFee;
                            SkyWay.FareFee = YFare;
                            SkyWay.SpacePrice = SpacePrice;
                        }
                    }
                    CarryCodeList.Add(SkyWay.CarryCode.ToUpper());
                    FlightCodeList.Add(SkyWay.FlightCode.ToUpper());
                    SpaceList.Add(SkyWay.Space);
                    DisacountList.Add(SkyWay.Discount);
                    AirTimeList.Add(SkyWay.FromDate.ToString("yyyy-MM-dd HH:mm"));
                    TravelCodeList.Add(SkyWay.FromCityCode + "-" + SkyWay.ToCityCode);
                    TravelList.Add(SkyWay.FromCityName + "-" + SkyWay.ToCityName);
                }
                #endregion

                #region 订单信息
                Tb_Ticket_Order oldOrder = QFP.m_OrderInput.Order;
                decimal.TryParse(m_FromDr["FareFee"].ToString(), out YFare);
                decimal.TryParse(m_FromDr["ABFee"].ToString(), out ABFee);
                decimal.TryParse(m_FromDr["FuelAdultFee"].ToString(), out FuelFee);
                decimal.TryParse(m_FromDr["XSFee"].ToString(), out SpacePrice);
                //抢票订单
                NewOrder.OrderSourceType = 13;
                NewOrder.OrderStatusCode = 1;//新订单等待支付
                NewOrder.PolicyType = 1;//默认b2b
                NewOrder.TicketStatus = 1;
                NewOrder.PolicySource = 1;//本地B2B
                NewOrder.A1 = 0;//未确认
                NewOrder.LinkMan = oldOrder.LinkMan;
                NewOrder.LinkManPhone = oldOrder.LinkMan;
                NewOrder.CreateCpyName = oldOrder.CreateCpyName;
                NewOrder.CreateCpyNo = oldOrder.CreateCpyNo;
                NewOrder.CreateLoginName = oldOrder.CreateLoginName;
                NewOrder.CreateUserName = oldOrder.CreateUserName;
                NewOrder.OwnerCpyNo = oldOrder.OwnerCpyNo;
                NewOrder.OwnerCpyName = oldOrder.OwnerCpyName;
                //是否允许换编码出票
                NewOrder.AllowChangePNRFlag = oldOrder.AllowChangePNRFlag;
                NewOrder.TravelType = oldOrder.TravelType;
                //乘客人数
                NewOrder.PassengerNumber = oldOrder.PassengerNumber;
                NewOrder.PassengerName = oldOrder.PassengerName;
                NewOrder.CarryCode = string.Join("/", CarryCodeList.ToArray());
                NewOrder.FlightCode = string.Join("/", FlightCodeList.ToArray());
                NewOrder.AirTime = DateTime.Parse(AirTimeList[0]);
                NewOrder.Travel = string.Join("/", TravelList.ToArray());
                NewOrder.TravelCode = string.Join("/", TravelCodeList.ToArray());
                NewOrder.Space = string.Join("/", SpaceList.ToArray());
                NewOrder.Discount = string.Join("/", DisacountList.ToArray());
                NewOrder.PMFee = TotalPMPrice;
                NewOrder.ABFee = TotalABFare;
                NewOrder.FuelFee = TotalRQFare;

                NewOrder.PNR = NewOrder.IsChdFlag ? QFP.PnrInfo.childPnr : QFP.PnrInfo.AdultPnr;
                NewOrder.BigCode = NewOrder.IsChdFlag ? QFP.PnrInfo.childPnrToBigPNR : QFP.PnrInfo.AdultPnrToBigPNR;
                NewOrder.Office = QFP.PnrInfo.Office;
                NewOrder.PrintOffice = QFP.PnrInfo.PrintOffice;
                NewOrder.CreateTime = System.DateTime.Now;
                NewOrder.KeGui = NewOrder.KeGui;

                //为儿童订单 且儿童不出成人票
                if (NewOrder.IsChdFlag)
                {
                    if (QFP.IsCHDToAudltTK != 1)
                    {
                        NewOrder.Space = "Y";
                        NewOrder.Discount = "100";
                    }
                }
                else
                {
                    //婴儿价格                           
                    decimal _TempFare = (0.1m * YFare) / 10;
                    NewOrder.BabyFee = d.FourToFiveNum(_TempFare, 0) * 10;
                }
                #endregion

                ParamModel.PasList = QFP.m_OrderInput.PasList;
                ParamModel.SkyList = QFP.m_OrderInput.SkyList;
                ParamModel.Order = NewOrder;
                //修改扫描Log_pnr状态
                if (pnrLogList.Count > 0 && !(QFP.PnrInfo.AdultPnr == "" && QFP.PnrInfo.childPnr == ""))
                {
                    string UpdatePnrLogSQL = string.Format(" update Log_Pnr set OrderFlag=1 where id in({0}) ", string.Join(",", pnrLogList.ToArray()));
                    OrderParam.ExecSQLList.Add(UpdatePnrLogSQL);
                }

                string ErrMsg = "";
                //生成订单
                OrderParam.IsCreatePayDetail = 1;//生成账单数据
                OrderParam.IsRobTicketOrder = true;//抢票订单
                OrderParam.OldRobTicketOrderId = QFP.m_OrderInput.Order.OrderId;//原订单号
                IsSuc = OrderManage.CreateOrder(ref OrderParam, out ErrMsg);
                if (IsSuc)
                {
                    //抢票成功
                    QFP.IsRobSuccess = true;
                    QFP.strLog = "\r\n原订单号:" + oldOrder.OrderId + "抢票成功  生成新" + (oldOrder.IsChdFlag ? "儿童" : "成人") + "编码:PNR=" + (oldOrder.IsChdFlag ? QFP.PnrInfo.childPnr : QFP.PnrInfo.AdultPnr) + "  生成订单成功,新订单号：" + (OrderParam.OrderParamModel.Count > 0 ? OrderParam.OrderParamModel[0].Order.OrderId : "") + "\r\n";
                }
                else
                {
                    QFP.strLog = "\r\n 方法名:CreateOrder 原订单号" + QFP.m_OrderInput.Order.OrderId + "  生成订单失败:" + ErrMsg + " \r\n";
                }
                m_Log(4, QFP.strLog);
                QFP.sbLog.Append(QFP.strLog);
            }
            catch (Exception ex)
            {
                IsSuc = false;
                m_Log(4, "\r\n 方法名:CreateOrder 原订单号" + QFP.m_OrderInput.Order.OrderId + "  生成订单异常信息:" + ex.Message + " \r\n");
            }
            finally
            {

            }
            return IsSuc;
        }

        /// <summary>
        ///  记录记录抢票订单日志
        /// </summary>
        /// <param name="OrderId">订单号</param>
        /// <param name="LogContent">订单日志内容</param>
        /// <param name="LogType">日志类型 1支付宝 2汇付</param>
        /// <returns></returns>
        public bool OrderLog(string OrderId, string LogContent, QueryFlightParam QFP)
        {
            bool Issuc = false;
            try
            {
                PbProject.Model.Log_Tb_AirOrder m_OrderLog = new PbProject.Model.Log_Tb_AirOrder();
                m_OrderLog.id = Guid.NewGuid();
                m_OrderLog.OrderId = OrderId;
                m_OrderLog.OperType = "修改";
                m_OrderLog.OperTime = DateTime.Now;
                m_OrderLog.OperContent = LogContent;
                m_OrderLog.OperLoginName = "系统管理员";
                m_OrderLog.OperUserName = "系统管理员";
                m_OrderLog.WatchType = 2;
                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(m_OrderLog);
                Issuc = Manage.ExecuteNonQuerySQLInfo(tempSql);
            }
            catch (Exception ex)
            {
                //记录日志
                PnrAnalysis.LogText.LogWrite(ex.Message + "\r\r", "RobTicketOrderLog");
            }
            return Issuc;
        }

        /// <summary>
        /// 获取查航班需要的参数
        /// </summary>
        /// <param name="OwnerCpyNo"></param>
        /// <returns></returns>
        public void SetUseParam(string OwnerCpyNo, QueryFlightParam QFP)
        {
            DataBase.Data.HashObject hashObject = new DataBase.Data.HashObject();
            hashObject.Add("OwnerCpyNo", OwnerCpyNo);
            DataTable[] table = Manage.MulExecProc("pro_GetRobTicketInfo", hashObject);
            if (table != null && table.Length > 1)
            {
                if (table.Length == 6)
                {
                    QFP.m_User = MappingHelper<User_Employees>.FillModel(table[0].Rows[0]);
                    QFP.m_Company = MappingHelper<User_Company>.FillModel(table[1].Rows[0]);
                    QFP.m_BaseParam = MappingHelper<Bd_Base_Parameters>.FillModelList(table[2]);
                    QFP.m_ParentParam = MappingHelper<Bd_Base_Parameters>.FillModelList(table[5]);
                    QFP.m_Config = Bd_Base_ParametersBLL.GetConfigParam(QFP.m_ParentParam);
                }
            }
            else
            {
                QFP = null;
            }
        }
    }
}
