using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Logic.ControlBase;
using System.Data;
using PbProject.Dal.Mapping;
using System.Threading;
using PbProject.Model;
using PbProject.Logic.PID;
using PnrAnalysis;
using System.Text.RegularExpressions;
using DataBase.Data;
using PbProject.WebCommon.Utility;
namespace PbProject.ConsoleServerProc
{
    public class ListParam
    {
        private string m_LoginName = string.Empty;
        public string LoginName
        {
            get
            {
                return m_LoginName;
            }
            set
            {
                m_LoginName = value;
            }
        }

        private string m_UserName = string.Empty;
        public string UserName
        {
            get
            {
                return m_UserName;
            }
            set
            {
                m_UserName = value;
            }
        }

        private string m_UninAllName = string.Empty;
        public string UninAllName
        {
            get
            {
                return m_UninAllName;
            }
            set
            {
                m_UninAllName = value;
            }
        }
        private string m_CpyNo = string.Empty;
        public string CpyNo
        {
            get
            {
                return m_CpyNo;
            }
            set
            {
                m_CpyNo = value;
            }
        }
        private string m_ShowText = string.Empty;
        public string ShowText
        {
            get
            {
                return m_ShowText;
            }
            set
            {
                m_ShowText = value;
            }
        }

        private int m_RobInnerTime = 60;
        /// <summary>
        /// 抢票持续时间
        /// </summary>
        public int RobInnerTime
        {
            get
            {
                return m_RobInnerTime;
            }
            set
            {
                m_RobInnerTime = value;
            }
        }

        private string m_RobSetting = string.Empty;
        /// <summary>
        /// 抢票设置
        /// </summary>
        public string RobSetting
        {
            get
            {
                return m_RobSetting;
            }
            set
            {
                m_RobSetting = value;
            }
        }
    }

    public class BSPAutoTicket : Common
    {
        BaseDataManage Manage = new BaseDataManage(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Dal.dll");

        public delegate void BSPShowLog(int LogType, string Data);
        /// <summary>
        /// 获取落地运营和供应
        /// </summary>
        /// <returns></returns>
        public List<ListParam> GetGYList()
        {
            DataTable table = Manage.GetLDGY();
            List<ListParam> list = null;
            if (table != null)
            {
                DataView dv = table.DefaultView;
                dv.RowFilter = " RoleType=2 and AccountState=1 ";
                table = dv.ToTable();
                list = MappingHelper<ListParam>.FillModelList(table);
            }
            return list;
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
        /// 开始BSP自动出票
        /// </summary>
        /// <param name="CpyNoList"></param>
        /// <param name="BSPSecond"></param>
        /// <param name="Log"></param>
        public void BSPStart(List<string> CpyNoList, List<ListParam> LPList, int BSPSecond, int BSPCount, BSPShowLog Log)
        {
            while (true)
            {
                Log(0, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "开始处理=================================================\r\n");

                string sqlWhere = string.Format(" left(OwnerCpyNo,12) in({0}) and  cast( isnull(AutoPrintTimes,'0') as int) < {1} and PolicyType=2 and AutoPrintFlag=2  and OrderStatusCode=3 ", string.Join(",", CpyNoList.ToArray()), BSPCount);
                List<Tb_Ticket_Order> OrderList = Manage.CallMethod("Tb_Ticket_Order", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_Order>;
                string KonZhiXT = "";
                if (OrderList != null && OrderList.Count > 0)
                {
                    Log(0, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "处理订单数" + OrderList.Count + "\r\n");
                    foreach (Tb_Ticket_Order Order in OrderList)
                    {
                        if (Order != null)
                        {
                            KonZhiXT = GetGYParameters(Order.OwnerCpyNo);
                            if (KonZhiXT != null && KonZhiXT.Contains("|35|"))//是否开启BSP自动出票
                            {
                                BspHandle(Order, LPList, Log);
                            }
                            else
                            {
                                Log(0, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "订单号:" + Order.OrderId + " 未开启BSP自动出票权限！\r\n");
                            }
                        }
                    }
                }
                Log(0, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "结束处理=================================================\r\n");
                //时间间隔秒
                Thread.Sleep(BSPSecond * 1000);
            }
        }
        /// <summary>
        /// 获取航空公司出票Office号 和打票机号
        /// </summary>
        /// <param name="CarryCode"></param>
        /// <param name="defaultOffice"></param>
        /// <returns></returns>
        public Tb_Ticket_PrintOffice GetPrintOffice(string CpyNo, string CarryCode)
        {
            Tb_Ticket_PrintOffice PrintOffice = null;
            string sqlWhere = string.Format(" CpyNo='{0}' and AirCode='{1}' ", CpyNo, CarryCode);
            List<Tb_Ticket_PrintOffice> list = Manage.CallMethod("Tb_Ticket_PrintOffice", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_PrintOffice>;
            if (list != null && list.Count > 0)
            {
                PrintOffice = list[0];
            }
            return PrintOffice;
        }


        /// <summary>
        /// 获取票号 PNR内容
        /// </summary>
        /// <param name="strRTicket"></param>
        /// <returns></returns>
        public List<string> GetTicketNum(string pnr, string strRTicket, FormatPNR format)
        {
            List<string> PTList = new List<string>();
            string msg = "";
            PnrModel PnrInfo = format.GetPNRInfo(pnr, strRTicket, false, out msg);
            foreach (PnrAnalysis.Model.TicketNumInfo item in PnrInfo._TicketNumList)
            {
                PTList.Add(item.TicketNum + "|" + item.PasName);
            }
            return PTList;
        }
        /// <summary>
        /// 获取需要xe的数据
        /// </summary>
        /// <param name="Recvdata"></param>
        /// <returns></returns>
        public HashObject GetNumList(string Recvdata)
        {
            HashObject hashParam = new HashObject();
            //判断条件XE序号
            List<string> StartXeList = new List<string>();
            //编码信息中的价格项正则
            string PPatern = @"\s*(?<Xe>\d+)\s*\.\s*(?=(FN\/|FC\/|FP\/|EI\/|RMK AUTOMATIC FARE))";
            //RR序号列表
            List<string> RRList = new List<string>();
            //TL序号列表
            List<string> XEList = new List<string>();
            string[] RtArr = Recvdata.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
            //提取XE出票时限序号正则
            string XePattern = @"\s*(?<Xe>\d+)\s*\.\s*(TL|TKTL).*?(?=\w{3}\s*\d{3})\s*";

            //航段正则
            string hPattern = @"\s*(?<RR>\d+)\s*\.\s*(?<carry>\w{2}\d{4})\s{0,}(?<seat>\w{1})\s{0,}(?<flyDate>\w{2}\d{2}\w{3})\s{0,}(?<city>\w{6})\s{0,}(?<state>[^\w]{0,}\w{2}\d{0,1})\s{0,}(?<startTime>[^\d]{0,}\d{4})\s{0,}(?<endTime>[^\d]{0,}\d{4}\+{0,1}\d{0,1})\s{0,}(?<orther>(\w|\s)+)";
            foreach (string item in RtArr)
            {
                Match mch = Regex.Match(item, XePattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                if (mch.Success)
                {
                    XEList.Add(mch.Groups["Xe"].Value.Trim());
                }
                //获取RR所在序号
                Match mch1 = Regex.Match(item, hPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                if (mch1.Success)
                {
                    RRList.Add(mch1.Groups["RR"].Value.Trim());
                }
                //sfc价格时所要叉掉的项列表
                Match mch2 = Regex.Match(item, PPatern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (mch2.Success)
                {
                    StartXeList.Add(mch2.Groups["Xe"].Value.Trim());//"XE" +
                }
            }
            hashParam.Add("XEList", XEList);
            hashParam.Add("RRList", RRList);
            hashParam.Add("StartXeList", StartXeList);

            return hashParam;
        }

        /* BSP常见错误信息
         MRT:HT0LYJ IGNORED   
         INCOMPLETE PNR/FN
         PLEASE CHECK TKT ELEMENT
         CHECK BLINK CODE
         ELE NBR                  
         */
        /// <summary>
        /// BSP订单处理
        /// </summary>
        /// <param name="Order"></param>
        private void BspHandle(Tb_Ticket_Order Order, List<ListParam> LPList, BSPShowLog Log)
        {  //订单日志                              
            StringBuilder sbLog = new StringBuilder();
            if (Order != null)
            {
                //订单出票公司信息
                ListParam TicketLP = LPList.Find(delegate(ListParam _tempLP)
                  {
                      return Order.OwnerCpyNo.Contains(_tempLP.CpyNo);
                  });

                //编码解析类
                PnrAnalysis.FormatPNR pnrFormat = new PnrAnalysis.FormatPNR();
                //判断标识
                List<string> NumTickList = new List<string>();
                List<string> PTList = null;
                List<Tb_Ticket_Passenger> PasList = null;
                try
                {
                    Log(0, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "订单号:" + Order.OrderId + "=======================start=====================\r\n");
                    string GYCpyNo = Order.OwnerCpyNo;
                    if (GYCpyNo.Length >= 12)
                    {
                        GYCpyNo = GYCpyNo.Substring(0, 12);
                    }
                    string CarrayCode = Order.CarryCode.Split('/')[0];
                    Tb_Ticket_PrintOffice PrintOffice = GetPrintOffice(GYCpyNo, CarrayCode);
                    if (PrintOffice == null || PrintOffice.PrintCode == "")
                    {
                        Log(0, string.Format("{0}未设置打票机号,请手动出票!", CarrayCode));
                        sbLog.Append(string.Format("{0}未设置打票机号,请手动出票!", CarrayCode));
                        return;
                    }
                    //出票Office
                    string pOffice = string.IsNullOrEmpty(PrintOffice.OfficeCode) ? Order.PrintOffice : PrintOffice.OfficeCode;
                    string PrintCode = PrintOffice.PrintCode;
                    string Pnr = Order.PNR;//Pnr编码
                    if (pOffice == "")
                    {
                        Log(0, string.Format("{0}出票Office不能为空！", CarrayCode));
                        sbLog.Append(string.Format("{0}出票Office不能为空！", CarrayCode));
                        return;
                    }
                    //获取乘客
                    string sqlWhere = string.Format(" OrderId='{0}' order by PassengerType", Order.OrderId);
                    Tb_Ticket_Passenger pMode = null;
                    PasList = Manage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_Passenger>;
                    if (PasList == null || PasList.Count == 0)
                    {
                        Log(0, string.Format("订单号:{0}没有找到对应的乘客信息!", Order.OrderId));
                        sbLog.Append(string.Format("订单号:{0}没有找到对应的乘客信息!", Order.OrderId));
                        return;
                    }
                    else
                    {
                        pMode = PasList[0];
                    }
                    List<Bd_Base_Parameters> baseParamList = Manage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + GYCpyNo + "'" }) as List<Bd_Base_Parameters>;
                    ConfigParam config = Bd_Base_ParametersBLL.GetConfigParam(baseParamList);
                    //指令管理类
                    //SendInsManage SendManage = new SendInsManage(Order.OwnerCpyNo, GYCpyNo, config);
                    if (config == null)
                    {
                        Log(0, string.Format("订单号{0},公司{1}未设置使用配置信息,请设置！", Order.OrderId, CarrayCode));
                        sbLog.Append(string.Format("订单号{0},公司{1}未设置使用配置信息,请设置！", Order.OrderId, CarrayCode));
                        return;
                    }
                    if (string.IsNullOrEmpty(Order.PNR))
                    {
                        Log(0, string.Format("订单号{0}中没有PNR,请检查！", Order.OrderId));
                        sbLog.Append(string.Format("订单号{0}中没有PNR,请检查！", Order.OrderId));
                        return;
                    }

                    ParamObject PM = new ParamObject();
                    PM.ServerIP = config.WhiteScreenIP;
                    PM.ServerPort = int.Parse(config.WhiteScreenPort);
                    PM.Office = pOffice;

                    //发送指令
                    string SendIns = "RT" + Pnr;
                    //返回数据
                    string Recvdata = string.Empty;
                    PM.code = SendIns;
                    PM.IsPn = true;
                    //  Recvdata = SendNewPID.SendCommand(PM);
                    Recvdata = WriteLogDB(PM, TicketLP);
                    //指令日志
                    Log(0, string.Format("\r\n【订单号{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", Order.OrderId, SendIns, Recvdata));
                    //授权检查
                    if (Recvdata.ToUpper().Contains("授权"))
                    {
                        Log(0, string.Format("订单号{0},编码{1} 出票Office{2},发送指令需要授权！", Order.OrderId, Pnr, pOffice));
                        sbLog.Append(string.Format("订单号{0},编码{1} 出票Office{2},发送指令需要授权！", Order.OrderId, Pnr, pOffice));
                        return;
                    }
                    else if (Recvdata.ToUpper().Contains("CANCELLED"))
                    {
                        Log(0, string.Format("订单号{0},编码{1} 出票Office{2},编码已取消，出票失败！", Order.OrderId, Pnr, pOffice));
                        sbLog.Append(string.Format("订单号{0},编码{1} 出票Office{2},编码已取消，出票失败！", Order.OrderId, Pnr, pOffice));
                        return;
                    }
                    string Msg = "";
                    string Xe = "", RR = "";
                    string PnrStatus = pnrFormat.GetPnrStatus(Recvdata, out Msg);
                    if (PnrStatus.Contains("NO"))
                    {
                        Log(0, string.Format("订单号{0},编码{1} 出票Office{2},编码状态为{3},不能自动出票!！", Order.OrderId, Pnr, pOffice, PnrStatus));
                        sbLog.Append(string.Format("订单号{0},编码{1} 出票Office{2},编码状态为{3},不能自动出票!！", Order.OrderId, Pnr, pOffice, PnrStatus));
                        return;
                    }
                    //存在的票号
                    PTList = GetTicketNum(Pnr, Recvdata.ToUpper(), pnrFormat);
                    List<string> RRList = null;
                    List<string> XEList = null;
                    if (PTList.Count == 0)
                    {
                    Start:
                        {
                            //进行出票
                            HashObject hash = GetNumList(Recvdata);
                            XEList = hash["XEList"] as List<string>;
                            RRList = hash["RRList"] as List<string>;
                            List<string> StartXeList = hash["StartXeList"] as List<string>;
                            //XE项
                            if (StartXeList.Count > 0)
                            {
                                for (int i = 0; i < StartXeList.Count; i++)
                                {
                                    string XeStr = StartXeList[i];
                                    if (XeStr != "")
                                    {
                                        //发送指令
                                        SendIns = "RT" + Pnr + "|XE" + XeStr + "|@";
                                        PM.code = SendIns;
                                        PM.IsPn = false;
                                        //Recvdata = SendNewPID.SendCommand(PM);//MRT:JG61M2 IGNORED 
                                        Recvdata = WriteLogDB(PM, TicketLP);
                                        //指令日志
                                        Log(0, string.Format("\r\n【订单号{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", Order.OrderId, SendIns, Recvdata));

                                        //判断是否XE成功 
                                        if (pnrFormat.INFMarkIsOK(Recvdata, out Msg))
                                        {
                                            /*  3U8881  E TU30APR  CTUPEK HK1   0730 1005 
                                               JG61M2 -  航空公司使用自动出票时限, 请检查PNR 
                                                 *** 预订酒店指令HC, 详情  HC:HELP   ***  
                                            */
                                            //发送指令
                                            SendIns = "RT" + Pnr;
                                            PM.code = SendIns;
                                            PM.IsPn = true;
                                            //Recvdata = SendNewPID.SendCommand(PM);
                                            Recvdata = WriteLogDB(PM, TicketLP);
                                            //指令日志
                                            Log(0, string.Format("\r\n【订单号{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", Order.OrderId, SendIns, Recvdata));
                                            goto Start;
                                        }
                                    }
                                }
                            }
                        }



                        string patPrice = "pat:a";
                        if (!Order.IsChdFlag)
                        {
                            //成人
                            patPrice = "pat:a";
                        }
                        else
                        {
                            //儿童
                            patPrice = "pat:a*ch";
                        }
                        //发送指令
                        SendIns = "RT" + Pnr + "|" + patPrice;
                        PM.code = SendIns;
                        PM.IsPn = false;
                        //Recvdata = SendNewPID.SendCommand(PM);
                        Recvdata = WriteLogDB(PM, TicketLP);
                        //指令日志
                        Log(0, string.Format("\r\n【订单号{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", Order.OrderId, SendIns, Recvdata));
                        if (Recvdata.IndexOf("PAT") == -1)
                        {
                            //发送指令
                            Log(0, string.Format("订单号{0},编码{1} 出票Office{2},编码已取消，未能PAT出票价,出票失败！", Order.OrderId, Pnr, pOffice));
                            sbLog.Append(string.Format("订单号{0},编码{1} 出票Office{2},编码已取消，未能PAT出票价,出票失败！", Order.OrderId, Pnr, pOffice));
                            return;
                        }

                        //比较
                        string xuhao = "", Seat = "", Err = "";
                        decimal _xsFare = 0m;
                        bool IsExistParice = false;
                        PnrAnalysis.PatModel PAT = pnrFormat.GetPATInfo(Recvdata.Replace("\r", ""), out Err);
                        foreach (PatInfo pat in PAT.UninuePatList)
                        {
                            decimal.TryParse(pat.Fare, out _xsFare);
                            //存在此价格
                            if (_xsFare == pMode.PMFee)
                            {
                                IsExistParice = true;
                                xuhao = pat.SerialNum;
                                Seat = pat.SeatGroup;
                                break;
                            }
                        }
                        if (!IsExistParice)
                        {
                            Log(0, string.Format("订单号{0},编码{1} 出票Office{2},舱位价{3}与Pat价格{4}不一致，出票失败！", Order.OrderId, Pnr, pOffice, pMode.PMFee, _xsFare));
                            sbLog.Append(string.Format("订单号{0},编码{1} 出票Office{2},舱位价{3}与Pat价格{4}不一致，出票失败！", Order.OrderId, Pnr, pOffice, pMode.PMFee, _xsFare));
                            return;
                        }

                        //做价格进去                   
                        SendIns = "RT" + Pnr + "|" + patPrice + "|SFC:" + xuhao + "|@";
                        PM.code = SendIns;
                        PM.IsPn = false;
                        //Recvdata = SendNewPID.SendCommand(PM);
                        Recvdata = WriteLogDB(PM, TicketLP);
                        //指令日志
                        Log(0, string.Format("\r\n【订单号{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", Order.OrderId, SendIns, Recvdata));
                        if (Recvdata.Contains("超时") || Recvdata.Contains("NO PNR"))
                        {
                            //Recvdata = SendNewPID.SendCommand(PM);
                            Recvdata = WriteLogDB(PM, TicketLP);
                            //指令日志
                            Log(0, string.Format("\r\n【订单号{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", Order.OrderId, SendIns, Recvdata));
                        }

                        //做备注
                        decimal _Discount = 0m;
                        if (decimal.TryParse(Order.Discount, out _Discount) && Order.Space.IndexOf("Y") == -1 && Order.Space.IndexOf("C") == -1 && Order.Space.IndexOf("F") == -1 && _Discount < 100)
                        {
                            //做价格进去                   
                            SendIns = "RT" + Pnr + "|EI:不得签转|@";
                            PM.code = SendIns;
                            PM.IsPn = false;
                            //Recvdata = SendNewPID.SendCommand(PM);
                            Recvdata = WriteLogDB(PM, TicketLP);
                            //指令日志
                            Log(0, string.Format("\r\n【订单号{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", Order.OrderId, SendIns, Recvdata));
                            if (Recvdata != "" && Recvdata.Contains("超时"))
                            {
                                //Recvdata = SendNewPID.SendCommand(PM);
                                Recvdata = WriteLogDB(PM, TicketLP);
                                //指令日志
                                Log(0, string.Format("\r\n【订单号{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", Order.OrderId, SendIns, Recvdata));
                            }
                        }



                        foreach (string item in XEList)
                        {
                            if (item != "")
                            {
                                Xe += "XE" + item + "|";
                            }
                        }
                        foreach (string item in RRList)
                        {
                            if (item != "")
                            {
                                RR += item + "RR" + "|";
                            }
                        }
                        if (Xe.Trim(new char[] { '|' }) == "")
                        {
                            Xe = "";
                        }
                        else
                        {
                            Xe = "|" + Xe.Trim('|') + "|";
                        }
                        if (RR.Trim(new char[] { '|' }) == "")
                        {
                            RR = "";
                        }
                        else
                        {
                            RR = "|" + RR.Trim('|') + "|";
                        }
                        if (RR == "")
                        {
                            RR = "|";
                        }
                        if (XEList == null || XEList.Count == 0 || Xe == "")
                        {
                            Log(0, string.Format("订单号{0},编码{1} 出票Office{2},编码状态为{3},未能取出出票时限!！", Order.OrderId, Pnr, pOffice, PnrStatus));
                            sbLog.Append(string.Format("订单号{0},编码{1} 出票Office{2},编码状态为{3},未能取出出票时限!！", Order.OrderId, Pnr, pOffice, PnrStatus));
                            return;
                        }

                        //组合打票指令
                        string etdz = "rt" + Pnr + Xe + RR.TrimEnd('|') + "|ETDZ " + PrintCode;
                        etdz = etdz.Replace("||", "|");

                        //出票                
                        SendIns = etdz;
                        PM.code = SendIns;
                        PM.IsPn = false;
                        // Recvdata = SendNewPID.SendCommand(PM);
                        Recvdata = WriteLogDB(PM, TicketLP);
                        //指令日志
                        Log(0, string.Format("\r\n【订单号{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", Order.OrderId, SendIns, Recvdata));


                        if (Recvdata.Contains("超时") || Recvdata.Contains("NO PNR"))
                        {
                            //Recvdata = SendNewPID.SendCommand(PM);
                            Recvdata = WriteLogDB(PM, TicketLP);
                            //指令日志
                            Log(0, string.Format("\r\n【订单号{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", Order.OrderId, SendIns, Recvdata));
                        }
                        if (Recvdata.Contains("请输入证件信息"))
                        {
                            Log(0, string.Format("订单号{0},编码{1} 出票Office{2},编码状态为{3},编码中没有证件号,请输入证件信息,否则不能出票！", Order.OrderId, Pnr, pOffice, PnrStatus));
                            sbLog.Append(string.Format("订单号{0},编码{1} 出票Office{2},编码状态为{3},编码中没有证件号,请输入证件信息,否则不能出票！", Order.OrderId, Pnr, pOffice, PnrStatus));
                            return;
                        }
                        if (Recvdata.ToUpper().Contains("STOCK"))
                        {
                            Log(0, string.Format("订单号{0},编码{1} 出票Office{2},编码状态为{3}," + Recvdata + "没有票号了！", Order.OrderId, Pnr, pOffice, PnrStatus));
                            sbLog.Append(string.Format("订单号{0},编码{1} 出票Office{2},编码状态为{3}," + Recvdata + "没有票号了！", Order.OrderId, Pnr, pOffice, PnrStatus));
                            return;
                        }
                        //出票成功
                        if (Recvdata.Contains("CNY") && Recvdata.ToUpper().Contains(Pnr.ToUpper()))
                        {
                            /*CNY2730.00      HF9550 
                            876-3250823439         876-3250823441  */

                            SendIns = "RT" + Pnr;
                            PM.code = SendIns;
                            PM.IsPn = true;
                            //Recvdata = SendNewPID.SendCommand(PM);
                            Recvdata = WriteLogDB(PM, TicketLP);
                            //指令日志
                            Log(0, string.Format("\r\n【订单号{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", Order.OrderId, SendIns, Recvdata));
                            //存在的票号
                            PTList = GetTicketNum(Pnr, Recvdata.ToUpper(), pnrFormat);
                        }
                        else
                        {
                            //出票失败 重试                       
                            if (!Recvdata.ToUpper().Contains("**ELECTRONIC TICKET PNR**") &&
                             Recvdata.ToUpper().Contains("SSR TKNE") &&
                             Recvdata.ToUpper().Contains("/DPN") &&
                             Recvdata.ToUpper().Contains("RMK " + CarrayCode + "/"))
                            {
                                //"ETRY:"   重试指令                            
                                SendIns = "RT" + Pnr + "|ETRY:";
                                PM.code = SendIns;
                                PM.IsPn = false;
                                //Recvdata = SendNewPID.SendCommand(PM);
                                Recvdata = WriteLogDB(PM, TicketLP);
                                //指令日志
                                Log(0, string.Format("\r\n【订单号{0}】发送指令>{1}\r\n接收数据:\r\n{2}\r\n", Order.OrderId, SendIns, Recvdata));
                            }
                            else
                            {
                                Log(0, string.Format("订单号{0},编码{1} 出票Office{2},编码状态为{3}," + "出票失败,{4}", Order.OrderId, Pnr, pOffice, PnrStatus, Recvdata));
                                sbLog.Append(string.Format("订单号{0},编码{1} 出票Office{2},编码状态为{3}," + "出票失败,{4}", Order.OrderId, Pnr, pOffice, PnrStatus, Recvdata));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log(0, string.Format("订单号{0},出票异常：{1}", Order.OrderId, ex.Message + ex.Source + ex.StackTrace.ToString()));
                    sbLog.Append(string.Format("订单号{0},出票异常：{1}", Order.OrderId, ex.Message + ex.Source + ex.StackTrace.ToString()));
                }
                finally
                {
                    string TicketMsg = "";
                    Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                    //执行SQl语句
                    List<string> sqlList = new List<string>();
                    //修改订单数据列表
                    List<string> OrderList = new List<string>();
                    if (TicketLP != null)
                    {
                        //解锁
                        OrderList.Add(" LockCpyNo='' ");
                        OrderList.Add(" LockLoginName='' ");
                        OrderList.Add(" LockTime='1900-01-01' ");

                        OrderList.Add(" CPTime=getdate() ");
                        OrderList.Add(" CPName='管理员' ");
                        OrderList.Add(string.Format(" CPCpyNo='{0}' ", TicketLP.CpyNo));
                        OrderList.Add(" CPRemark='BSP自动出票' ");
                        OrderList.Add(string.Format(" CPCpyName='{0}' ", TicketLP.UninAllName));

                        //日志
                        OrderLog.id = Guid.NewGuid();
                        OrderLog.OrderId = Order.OrderId;
                        OrderLog.OperType = "出票";
                        OrderLog.OperTime = DateTime.Now;
                        OrderLog.OperLoginName = "管理员";
                        OrderLog.OperUserName = "管理员";
                        OrderLog.CpyNo = TicketLP.CpyNo;
                        OrderLog.CpyType = 1;
                        OrderLog.CpyName = TicketLP.UninAllName;
                    }

                    if (PTList != null && PTList.Count > 0 && PasList != null && PasList.Count > 0)
                    {
                        string PasName = "", TicketNumber = "";
                        for (int i = 0; i < PTList.Count; i++)
                        {
                            if (PTList[i].Split('|').Length == 2)
                            {
                                TicketNumber = PTList[i].Split('|')[0];
                                PasName = PTList[i].Split('|')[1];
                                Tb_Ticket_Passenger Passenger = PasList.Find(delegate(Tb_Ticket_Passenger _tempPassenger)
                                {
                                    return PTList[i].ToUpper().Trim().Contains(_tempPassenger.PassengerName.ToUpper().Trim());
                                });
                                if (Passenger != null)
                                {
                                    sqlList.Add(string.Format(" update Tb_Ticket_Passenger set TicketNumber='{0}',TicketStatus=2 where id='{1}' and PassengerName='{2}' ", TicketNumber, Passenger.id.ToString(), Passenger.PassengerName));
                                }
                            }
                        }
                        //修改订单数据
                        OrderList.Add(" TicketStatus=2 ");
                        OrderList.Add(" OrderStatusCode=4 ");
                        if (OrderList.Count > 0)
                        {
                            sqlList.Add(string.Format(" update Tb_Ticket_Order set {0} where id='{1}' ", string.Join(",", OrderList.ToArray()), Order.id.ToString()));
                        }

                        //出票成功
                        TicketMsg = "出票成功";
                        OrderLog.WatchType = 5;
                        //日志
                        OrderLog.OperContent = "订单号:" + Order.OrderId + " BSP自动出票成功," + string.Format(",", PasList.ToArray());
                        //修改数据库状态
                    }
                    else
                    {
                        //出票失败
                        TicketMsg = "出票失败";
                        //修改订单自动出票尝试次数
                        if (Order.AutoPrintTimes > 3)//尝试次数大于3改为手动出票
                        {
                            sqlList.Add(string.Format(" update Tb_Ticket_Order set AutoPrintFlag=0,AutoPrintTimes=CAST(AutoPrintTimes as int)+1  where id='{0}' ", Order.id.ToString()));
                        }
                        else
                        {
                            sqlList.Add(string.Format(" update Tb_Ticket_Order set AutoPrintTimes=CAST(AutoPrintTimes as int)+1  where id='{0}' ", Order.id.ToString()));
                        }
                        OrderLog.WatchType = 2;
                        //日志
                        OrderLog.OperContent = "BSP自动出票失败," + sbLog.ToString();
                    }
                    //日志
                    string tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlList.Add(tempSql);

                    string Msg = "";
                    //修改数据库
                    if (!Manage.ExecuteSqlTran(sqlList, out Msg))
                    {
                        Log(0, string.Format("订单号{0},修改数据库失败：{1}", Order.OrderId, Msg));
                    }
                    Log(0, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "订单号:" + Order.OrderId + "  " + TicketMsg + "=======================end=====================\r\n");
                }
            }
        }

        /// <summary>
        /// 指令记录到数据库
        /// </summary>
        /// <param name="PM"></param>
        /// <param name="TicketLP"></param>
        /// <returns></returns>
        public string WriteLogDB(ParamObject PM, ListParam TicketLP)
        {
            string Recvdata = "";
            Tb_SendInsData sendins = new Tb_SendInsData();
            try
            {
                sendins.SendTime = System.DateTime.Now;
                Recvdata = SendNewPID.SendCommand(PM);
                sendins.RecvTime = System.DateTime.Now;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                try
                {
                    //指令记入数据库               
                    sendins.SendIns = PM.code;
                    sendins.RecvData = Recvdata;
                    sendins.SendInsType = 18;
                    sendins.Office = PM.Office;
                    sendins.ServerIPAndPort = PM.ServerIP + ":" + PM.ServerPort;
                    sendins.UserAccount = "BSP";
                    sendins.CpyNo = TicketLP.CpyNo;
                    //插入数据库
                    Manage.ExecuteNonQuerySQLInfo(PbProject.Dal.Mapping.MappingHelper<Tb_SendInsData>.CreateInsertModelSql(sendins, new List<string>() { "id" }));
                }
                catch (Exception)
                {
                }
            }
            return Recvdata;
        }
    }
}
