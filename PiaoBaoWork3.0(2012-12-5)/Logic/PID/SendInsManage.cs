using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PnrAnalysis;
using PnrAnalysis.Model;
using PbProject.Model;
using System.Text.RegularExpressions;
using PbProject.Logic.ControlBase;
namespace PbProject.Logic.PID
{
    /// <summary>
    /// 扩展参数
    /// </summary>
    public class ParamEx
    {
        public int UsePIDChannel = 2;
        /// <summary>
        /// PID
        /// </summary>
        public string PID = string.Empty;
        /// <summary>
        /// KeyNo 
        /// </summary>
        public string KeyNo = string.Empty;

    }
    /// <summary>
    /// 发送指令管理
    /// </summary>
    public class SendInsManage
    {
        class SendInsParam
        {
            public string IP = string.Empty;
            public int Port = 0;
            public string Office = string.Empty;
            public string LoginAccount = string.Empty;
            public string CpyNo = string.Empty;
        }
        //数据库操作类
        private BaseDataManage BaseMange = new BaseDataManage();
        //PNR格式化类
        private FormatPNR format = new PnrAnalysis.FormatPNR();
        private SendInsParam m_SendInsParam;
        public ConfigParam m_ConfigParam;
        private ParamEx m_ParamEx;
        public SendInsManage(string LoginName, string CpyNo, ParamEx PE, ConfigParam _configParam)
        {
            m_SendInsParam = new SendInsParam();
            m_SendInsParam.CpyNo = CpyNo;
            m_SendInsParam.LoginAccount = LoginName;
            m_SendInsParam.IP = _configParam.WhiteScreenIP;
            m_SendInsParam.Port = int.Parse(_configParam.WhiteScreenPort);
            m_ConfigParam = _configParam;
            m_ParamEx = PE;
            string _cpyNo = CpyNo;
            if (_cpyNo.Length >= 12)
            {
                _cpyNo = _cpyNo.Substring(0, 12);
            }
            if (_cpyNo == "")//公司编号
            {
                m_ParamEx.PID = _configParam.Pid;
                m_ParamEx.KeyNo = _configParam.KeyNo;
            }
        }
        /// <summary>
        /// 取消编码
        /// </summary>
        /// <param name="Pnr">需要取消的PNR</param>
        /// <param name="Office">提指令需要使用的Office</param>
        /// <returns></returns>
        public bool CancelPnr(string Pnr, string Office)
        {
            string cmd = string.Format("RT{0}|XEPNR@{0}", Pnr.ToUpper());
            string recvData = Send(cmd, ref Office, 1);
            if (IsReSend(recvData))
            {
                recvData = Send(cmd, ref Office, 1);
            }
            bool IsSuc = (recvData.Contains("CANCELLED"));
            return IsSuc;
        }


        /// <summary>
        /// 分离编码
        /// </summary>
        /// <param name="pnr">原编码</param>
        /// <param name="Office">Office</param>
        /// <param name="PasName">PNR中需要分离的乘客姓名</param>        
        /// <param name="newPnr">分离出来的新编码</param>
        /// <param name="IsCancelNewPnr">新编码是否取消</param>
        /// <param name="ErrMsg">内部错误</param>
        /// <returns></returns>
        public bool SplitPnr(string pnr, string Office, List<string> PasName, ref string newPnr, ref bool IsCancelNewPnr, out string ErrMsg)
        {
            bool IsSuc = false;
            ErrMsg = "";
            try
            {
                if (PasName != null && PasName.Count > 0)
                {
                    PnrModel pnrModel = GetPnr(pnr, Office, out ErrMsg);
                    if (pnrModel._PassengerList.Count == 0)
                    {
                        ErrMsg = "编码【" + pnr + "】中未能解析出乘客姓名！";
                    }
                    else
                    {
                        #region  //获取分离编码中乘客的序号集合
                        List<string> pNo = new List<string>();
                        List<string> NoSplitPName = new List<string>();
                        string pName = "";
                        for (int i = 0; i < PasName.Count; i++)
                        {
                            pName = PasName[i].ToUpper().Trim();
                            //处理儿童姓名问题
                            if (PinYingMange.IsChina(pName))
                            {
                                if (pName.EndsWith("CHD"))
                                {
                                    pName = pName.Substring(0, pName.LastIndexOf("CHD"));
                                }
                            }
                            else
                            {
                                if (pName.EndsWith(" CHD"))
                                {
                                    pName = pName.Substring(0, pName.LastIndexOf(" CHD"));
                                }
                            }
                            //比较乘客姓名
                            PassengerInfo _PassengerInfo = pnrModel._PassengerList.Find(delegate(PassengerInfo _p1)
                              {
                                  return _p1.PassengerName.ToUpper().Trim() == pName;
                              });
                            if (_PassengerInfo != null)
                            {
                                pNo.Add(_PassengerInfo.SerialNum);
                            }
                            else
                            {
                                NoSplitPName.Add(PasName[i]);
                            }
                        }
                        if (ErrMsg == "")
                        {
                            string cmd = "", recvData = "";
                            if (pnrModel._PassengerList.Count > 1)//人数需要大于1人才可分离
                            {
                                //如果是出票了的 分离编码需要删掉TL项 在分离
                                //if (pnrModel.PnrStatus.Contains("RR") && pnrModel._Other.TKTL != null)
                                //{
                                //    //删除出票时限
                                //    cmd = string.Format("rt{0}|xe{1}|@", pnr, pnrModel._Other.TKTL.SerialNum);
                                //    recvData = Send(cmd, ref Office, 17);
                                //    //cmd = string.Format("rt{0}|TKT/072-5555555514/p3|@", pnr, pnrModel.);                                  
                                //}
                                if (pNo.Count > 0)
                                {
                                    //分离乘客
                                    cmd = string.Format("rt{0}|sp{1}|\\KI", pnr, string.Join("/", pNo.ToArray()));
                                    recvData = Send(cmd, ref Office, 1);
                                    if (IsReSend(recvData))
                                    {
                                        recvData = Send(cmd, ref Office, 1);
                                    }
                                    ErrMsg = "";
                                    newPnr = format.GetSplitPnr(pnr, recvData, out ErrMsg);
                                }
                                else
                                {
                                    if (NoSplitPName.Count > 0)
                                    {
                                        ErrMsg = "编码【" + pnr + "】中不存在以下申请的乘客姓名【" + string.Join(",", NoSplitPName.ToArray()) + "】<br />";
                                    }
                                }
                            }
                            else
                            {
                                //取消编码
                                IsSuc = true; //CancelPnr(pnr, Office);
                                if (NoSplitPName.Count > 0)
                                {
                                    ErrMsg = "编码【" + pnr + "】中不存在以下申请的乘客姓名【" + string.Join(",", NoSplitPName.ToArray()) + "】,<br />";
                                }
                            }
                            if (ErrMsg == "" && newPnr.Trim().Length == 6)
                            {
                                #region 新编码是否取消

                                //默认取消新编码 true取消 false 不取消
                                List<string> newPasList = new List<string>();
                                bool m_IsCancelNewPnr = true;
                                PnrModel NewPnr = GetPnr(newPnr, Office, out ErrMsg);
                                int pCount = 0;
                                //新编码中是否含有不需要分离的乘客
                                foreach (PassengerInfo PnrPasName in NewPnr._PassengerList)
                                {
                                    if (!PasName.Contains(PnrPasName.PassengerName))
                                    {
                                        m_IsCancelNewPnr = false;
                                        newPasList.Add(PnrPasName.PassengerName);
                                    }
                                    else
                                    {
                                        pCount++;
                                    }
                                }
                                if (NoSplitPName.Count > 0)
                                {
                                    ErrMsg = "编码【" + pnr + "】中不存在以下申请的乘客姓名【" + string.Join(",", NoSplitPName.ToArray()) + "】,<br />";
                                }
                                //如果新编码中所有所有人是在所选人分离的中 就取消新编码
                                if (m_IsCancelNewPnr && pCount == NewPnr._PassengerList.Count)
                                {
                                    //取消新编码
                                    IsCancelNewPnr = CancelPnr(newPnr, Office);
                                }
                                else
                                {
                                    if (newPasList.Count > 0)
                                    {
                                        ErrMsg += "分离的新编码【" + newPnr + "】中的乘客与提交申请的乘客【" + string.Join("|", newPasList.ToArray()) + "】不一致，请手动确认！";
                                    }
                                }
                                //分离成功
                                IsSuc = true;
                                #endregion
                            }
                            else
                            {
                                if (recvData != "")
                                {
                                    if (recvData.Contains("服务器繁忙"))
                                    {
                                        ErrMsg = "服务器繁忙,请稍后再试！";
                                    }
                                    else if (recvData.Contains("CANNOT CANCL OR SPLT PASSENGER IF SEGMENTS ARE NO STATUS"))
                                    {
                                        ErrMsg = "该编码【" + pnr + "】系统不能自动分离！";
                                    }
                                    else if (recvData.Contains("LEASE WAIT - TRANSACTION IN PROGRESS"))
                                    {
                                        ErrMsg = "分离编码【" + pnr + "】出现问题,返回数据【" + recvData.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("\v", "").Replace("\f", "") + "】,请手动确认编码是否分离！";
                                    }
                                    else
                                    {
                                        ErrMsg = format.RemoveHideChar(recvData).Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("\v", "").Replace("\f", "");
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
            return IsSuc;
        }

        /// <summary>
        /// 获取Pnr信息
        /// </summary>
        /// <param name="Pnr">编码</param>
        /// <param name="Office">提指令需要使用的Office</param>      
        /// <param name="ErrMsg">内部错误信息</param>
        /// <returns></returns>
        public PnrModel GetPnr(string Pnr, string Office, out string ErrMsg)
        {
            PnrModel model = null;
            ErrMsg = "";
            string cmd = string.Format("RT{0}", Pnr.ToUpper());
            if (m_ParamEx.UsePIDChannel == 0)
            {
                //旧版本EC
                cmd = string.Format("(eas)RT{0}", Pnr.ToUpper());
            }
            string recvData = Send(cmd, ref Office, 2);
            if (IsReSend(recvData))
            {
                recvData = Send(cmd, ref Office, 2);
            }
            if (!IsReSend(recvData))
            {
                if (!recvData.Contains("授权"))
                {
                    model = format.GetPNRInfo(Pnr, recvData, false, out ErrMsg);
                    model._CurrUseOffice = Office;
                    //是否为团编码
                    if (model._PnrType == "2")
                    {
                        cmd = string.Format("RT{0}|RTN", Pnr.ToUpper());
                        if (m_ParamEx.UsePIDChannel == 0)
                        {
                            //旧版本EC
                            cmd = string.Format("(eas)RT{0}|RTN", Pnr.ToUpper());
                        }
                        recvData = Send(cmd, ref Office, 2);
                        if (IsReSend(recvData))
                        {
                            recvData = Send(cmd, ref Office, 2);
                        }
                        if (!IsReSend(recvData))
                        {
                            model = format.GetPNRInfo(Pnr, recvData, false, out ErrMsg);
                            model._CurrUseOffice = Office;
                        }
                        else
                        {
                            ErrMsg = recvData;
                        }
                    }
                }
                else
                {
                    ErrMsg = "授权#" + Office;
                }
            }
            else
            {
                ErrMsg = recvData;
            }
            return model;
        }
        /// <summary>
        /// 获取Pnr信息 没有具体Office 会循环供应商Office提取
        /// </summary>
        /// <param name="Pnr">编码</param>
        /// <param name="Office">提指令需要使用的Office</param>
        /// <param name="ErrMsg">内部错误信息</param>
        /// <returns></returns>
        public PnrModel GetPnr(string Pnr, out string ErrMsg)
        {
            PnrModel model = null;
            ErrMsg = "";
            string cmd = string.Format("RT{0}", Pnr.ToUpper());
            if (m_ParamEx.UsePIDChannel == 0)
            {
                //旧版本EC
                cmd = string.Format("(eas)RT{0}", Pnr.ToUpper());
            }
            PnrImportParam param = new PnrImportParam();
            string Office = "";
            string recvData = Send(cmd, ref Office, 5, param);
            if (IsReSend(recvData))
            {
                recvData = Send(cmd, ref Office, 5);
            }
            if (!IsReSend(recvData))
            {
                if (!recvData.Contains("授权"))
                {
                    model = format.GetPNRInfo(Pnr, recvData, false, out ErrMsg);
                    model._CurrUseOffice = Office;
                    //是否为团编码
                    if (model._PnrType == "2")
                    {
                        cmd = string.Format("RT{0}|RTN", Pnr.ToUpper());
                        if (m_ParamEx.UsePIDChannel == 0)
                        {
                            //旧版本EC
                            cmd = string.Format("(eas)RT{0}|RTN", Pnr.ToUpper());
                        }
                        recvData = Send(cmd, ref Office, 2);
                        if (IsReSend(recvData))
                        {
                            recvData = Send(cmd, ref Office, 2);
                        }
                        model = format.GetPNRInfo(Pnr, recvData, false, out ErrMsg);
                        model._CurrUseOffice = Office;
                    }
                }
                else
                {
                    ErrMsg = "授权#" + string.Join("|", param.authOfficeList.ToArray());
                }
            }
            else
            {
                ErrMsg = recvData;
            }
            return model;
        }
        /// <summary>
        /// 大编码转换为小编码获取RT实体
        /// </summary>
        /// <param name="BigPnr"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public PnrModel GetBigPnr(string BigPnr, out string ErrMsg)
        {
            PnrModel model = null;
            ErrMsg = "";
            string cmd = string.Format("RRT:V/{0}/0000/.", BigPnr.ToUpper());
            PnrImportParam param = new PnrImportParam();
            string Office = "";
            string recvData = Send(cmd, ref Office, 5, param);
            if (IsReSend(recvData))
            {
                recvData = Send(cmd, ref Office, 5);
            }
            if (!IsReSend(recvData))
            {
                string Pnr = format.GetRRTPnr(recvData);
                if (!string.IsNullOrEmpty(Pnr) && Pnr.Length == 6)
                {
                    cmd = string.Format("RT{0}", Pnr.ToUpper());
                    if (m_ParamEx.UsePIDChannel == 0)
                    {
                        //旧版本EC
                        cmd = string.Format("(eas)RT{0}", Pnr.ToUpper());
                    }
                    recvData = Send(cmd, ref Office, 2, param);
                    if (IsReSend(recvData))
                    {
                        recvData = Send(cmd, ref Office, 2);
                    }
                    if (!recvData.Contains("授权"))
                    {
                        model = format.GetPNRInfo(Pnr, recvData, false, out ErrMsg);
                        model._CurrUseOffice = Office;
                        //是否为团编码
                        if (model._PnrType == "2")
                        {
                            cmd = string.Format("RT{0}|RTN", Pnr.ToUpper());
                            if (m_ParamEx.UsePIDChannel == 0)
                            {
                                //旧版本EC
                                cmd = string.Format("(eas)RT{0}|RTN", Pnr.ToUpper());
                            }
                            recvData = Send(cmd, ref Office, 2);
                            if (IsReSend(recvData))
                            {
                                recvData = Send(cmd, ref Office, 2);
                            }
                            model = format.GetPNRInfo(Pnr, recvData, false, out ErrMsg);
                            model._CurrUseOffice = Office;
                        }
                    }
                    else
                    {
                        ErrMsg = "授权#" + string.Join("|", param.authOfficeList.ToArray());
                    }
                }
                else
                {
                    ErrMsg = "未能取到编码:" + recvData;
                }
            }
            else
            {
                ErrMsg = recvData;
            }
            return model;
        }


        /// <summary>
        /// 获取编码PAT信息
        /// </summary>
        /// <param name="Pnr">编码</param>
        /// <param name="Office">提指令需要使用的Office</param>
        /// <param name="PasType">1成人PAT信息 2儿童PAT信息 3婴儿PAT信息</param>
        /// <param name="ErrMsg">内部错误信息</param>
        /// <returns></returns>
        public PatModel GetPat(string Pnr, string Office, int PasType, out string ErrMsg)
        {
            PatModel patmodel = null;
            string cmd = string.Format("RT{0}|PAT:A", Pnr.ToUpper());
            if (PasType == 1)
            {
                cmd = string.Format("RT{0}|PAT:A", Pnr.ToUpper());
            }
            else if (PasType == 2)
            {
                cmd = string.Format("RT{0}|PAT:A*CH", Pnr.ToUpper());
            }
            else if (PasType == 3)
            {
                cmd = string.Format("RT{0}|PAT:A*IN", Pnr.ToUpper());
            }
            string recvData = Send(cmd, ref Office, 3);
            if (!recvData.Contains("CAN NOT USE *CH FOR NON CHD PASSENGER"))
            {
                if (IsReSend(recvData) || recvData.IndexOf("PAT:A") == -1)
                {
                    recvData = Send(cmd, ref Office, 3);
                }
            }
            if (!recvData.Contains("没有符合条件的运价") && !recvData.Contains("CAN NOT USE *CH FOR NON CHD PASSENGER"))
            {
                patmodel = format.GetPATInfo(recvData, out ErrMsg);
            }
            else
            {
                ErrMsg = recvData;
            }
            return patmodel;
        }

        /// <summary>
        /// 对pnr授权authOffice
        /// </summary>
        /// <param name="pnr">编码</param>
        /// <param name="authOffice">需要授权的Office</param>
        /// <param name="Office">提指令需要使用的Office</param>
        /// <param name="ErrMsg">错误信息</param>
        /// <returns></returns>
        public bool AuthToOffice(string pnr, string authOffice, string Office, out string ErrMsg)
        {
            ErrMsg = "";
            string cmd = string.Format("RT{0}|RMK TJ AUTH {1}|@", pnr, authOffice);
            string recvData = Send(cmd, ref Office, 4);
            if (IsReSend(recvData))
            {
                recvData = Send(cmd, ref Office, 4);
            }
            //判断授权是否成功
            return format.INFMarkIsOK(recvData, out ErrMsg); ;
        }
        /// <summary>
        /// 票号挂起解挂
        /// </summary>
        /// <param name="OpType">1.挂起 2.解挂</param>
        /// <param name="TicketNumber">票号</param>
        /// <param name="Office">Office</param>
        /// <param name="ErrMsg">错误信息</param>
        /// <returns></returns>
        public bool TicketNumberLock(int OpType, string TicketNumber, string Office, out string ErrMsg)
        {
            ErrMsg = "";
            bool IsSuc = false;
            string cmd = string.Format("TSS:TN/{0}/{1}", TicketNumber, (OpType == 1 ? "S" : "B"));
            string recvData = Send(cmd, ref Office, 7);
            if (IsReSend(recvData))
            {
                recvData = Send(cmd, ref Office, 7);
            }
            //if (OpType == 1)//挂起
            //{
            if (recvData.Trim().ToUpper().Contains("ACCEPTED"))//|| recvData.Trim().ToUpper().Contains("COUPON STATUS CODE INVALID"))
            {
                IsSuc = true;
            }
            else
            {
                ErrMsg = recvData;
            }
            //}
            //else if (OpType == 2)//解挂
            //{
            //    if (recvData.Trim().ToUpper().Contains("ACCEPTED") || recvData.Trim().ToUpper().Contains("COUPON STATUS CODE INVALID"))
            //    {
            //        IsSuc = true;
            //    }
            //}
            return IsSuc;
        }

        /// <summary>
        /// 记录指令
        /// </summary>
        /// <param name="tb_sendinsdata"></param>
        /// <returns></returns>
        public bool LogIns(Tb_SendInsData tb_sendinsdata)
        {
            bool Insert = false;
            if (tb_sendinsdata != null)
            {
                List<string> sqlList = new List<string>();
                List<string> Removelist = new List<string>();
                Removelist.Add("id");
                sqlList.Add(PbProject.Dal.Mapping.MappingHelper<Tb_SendInsData>.CreateInsertModelSql(tb_sendinsdata, Removelist));
                if (sqlList.Count > 0)
                {
                    string errMsg = "";
                    Insert = this.BaseMange.ExecuteSqlTran(sqlList, out errMsg);
                }
            }
            return Insert;
        }

        /// <summary>
        /// 记录批量指令
        /// </summary>
        /// <param name="tb_sendinsdata"></param>
        /// <returns></returns>
        private bool LogInsList(List<Tb_SendInsData> insList)
        {
            bool Insert = false;
            List<string> sqlList = new List<string>();
            foreach (Tb_SendInsData tb_sendinsdata in insList)
            {
                List<string> Removelist = new List<string>();
                Removelist.Add("id");
                sqlList.Add(PbProject.Dal.Mapping.MappingHelper<Tb_SendInsData>.CreateInsertModelSql(tb_sendinsdata, Removelist));
            }
            if (sqlList.Count > 0)
            {
                string errMsg = "";
                Insert = this.BaseMange.ExecuteSqlTran(sqlList, out errMsg);
            }
            return Insert;
        }
        /// <summary>
        /// 是否需要重新发送指令
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool IsReSend(string data)
        {
            bool resend = false;
            if (data == null)
            {
                data = "";
            }
            data = data.ToLower();
            if (data == "" || data.Contains("地址无效") || data.Contains("超时") || data.Contains("服务器忙") || data.Contains("wsacancelblockingcall") || data.Contains("无法连接") || data.Contains("由于") || data.Contains("强迫关闭") || data.Contains("无法从传输连接中读取数据"))
            {
                resend = true;
            }
            return resend;
        }
        /// <summary>
        /// 行程单操作 如创建或者作废
        /// </summary>
        /// <param name="opType">操作类型:0创建行程单 1作废行程单 </param>
        /// <param name="TicketNumber">票号</param>
        /// <param name="TripNum">行程单号</param>
        /// <param name="PrintOffice">行程单终端号（Office）</param>
        /// <returns></returns>
        public bool SendTrip(TripParam Param)
        {
            bool IsSuc = false;
            Param.Msg = "";
            //跟中流程
            StringBuilder sbTrip = new StringBuilder();
            //指令对象 记录指令
            List<Tb_SendInsData> objList = new List<Tb_SendInsData>();
            sbTrip.AppendFormat("操作类型:{0} 票号:{1} 行程单号:{2} 终端号:{3}", Param.InsType, Param.TicketNumber, Param.TripNumber, Param.Office);
            try
            {
                #region //验证数据
                //验证票号有效性
                string pattern = @"\d{3,4}(\-?|\s+)\d{10}";
                if (!Regex.Match(Param.TicketNumber, pattern, RegexOptions.IgnoreCase).Success)
                {
                    Param.Msg = string.Format("客票票号{0}格式错误！", Param.TicketNumber);
                }
                //验证行程单格式有效性
                pattern = @"(?:\d{10})";
                if (!Regex.IsMatch(Param.TripNumber, pattern, RegexOptions.IgnoreCase))
                {
                    Param.Msg = string.Format("行程单号{0}格式不正确！", Param.TripNumber);
                }
                //验证Office号
                pattern = @"[a-zA-Z]{3}\d{3}";
                if (!Regex.IsMatch(Param.Office, pattern, RegexOptions.IgnoreCase))
                {
                    Param.Msg = string.Format("行程单终端号({0})错误！", Param.Office.Trim());
                }
                Param.TicketNumber = Param.TicketNumber.Replace(" ", "").Replace("-", "").Trim();
                Param.TripNumber = Param.TripNumber.Trim();
                Param.Office = Param.Office.Trim();
                #endregion


                if (Param.Msg == "")
                {
                    PnrAnalysis.FormatPNR format = new FormatPNR();
                    ParamObject PM = new ParamObject();
                    PM.ServerIP = m_SendInsParam.IP;
                    PM.ServerPort = m_SendInsParam.Port;
                    PM.Office = Param.Office;
                    string sendIns = "";
                    string recvData = "";
                    if (Param.InsType == 0)
                    {
                        #region //创建
                        //是否成功
                        bool IsCreateSuc = false;
                        sendIns = string.Format("DETR:TN/{0}", Param.TicketNumber);
                        if (Param.InsIsEndOffice)
                        {
                            sendIns = string.Format("DETR:TN/{0}&{1}", Param.TicketNumber, Param.Office);
                        }
                        recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                        if (IsReSend(recvData))
                        {
                            recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                        }
                        //NOT EXIST   TICKET NUMBER
                        if (!IsReSend(recvData) && !recvData.ToUpper().Contains("NOT EXIST") && !recvData.ToUpper().Contains("TICKET NUMBER"))// && (recvData.ToUpper().Contains("NO RECORD") || recvData.ToUpper().Contains("DETR:TN") || recvData.ToUpper().Contains("DETR TN")))
                        {
                            //检查该票号是否创建行程单
                            if (recvData.ToUpper().Contains("RECEIPT PRINTED"))
                            {
                                //已创建处理
                                sendIns = string.Format("DETR:TN/{0},F", Param.TicketNumber);
                                if (Param.InsIsEndOffice)
                                {
                                    sendIns = string.Format("DETR:TN/{0},F&{1}", Param.TicketNumber, Param.Office);
                                }
                                recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                                if (IsReSend(recvData))
                                {
                                    recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                                }
                                Param.DetrList = format.GetDetrF(recvData);
                                if (Param.DetrList.Count > 0)
                                {
                                    DetrInfo detrInfo = Param.DetrList[0];
                                    if (detrInfo.CreateSerialNumber.Trim() != Param.TripNumber.Trim())
                                    {
                                        Param.Msg = string.Format("该票号{0}已创建行程单!##{0}|{1}", Param.TicketNumber, detrInfo.CreateSerialNumber.Trim());
                                    }
                                }
                                IsCreateSuc = true;
                            }
                            else
                            {
                                //需要创建
                                #region 方法一
                                if (Param.UseIns == 0 || Param.UseIns == 2)
                                {
                                    sendIns = string.Format("PRINV:{0},{{ITTN={1}}}", Param.TicketNumber, Param.TripNumber);
                                    if (Param.InsIsEndOffice)
                                    {
                                        sendIns = string.Format("PRINV:{0},{{ITTN={1}}}&{2}", Param.TicketNumber, Param.TripNumber, Param.Office);
                                    }
                                    recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                                    if (recvData.Contains("该行程单号已被"))
                                    {
                                        string GetPattern = @"ERROR:(?<TravelNumber>\d{10})该行程单号已被(?<TicketNum>\d{3,4}(\-?|\s+)\d{10})使用!";
                                        Match m_mch = Regex.Match(recvData, GetPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                        if (m_mch.Success)
                                        {
                                            string innner_TravelNumber = m_mch.Groups["TravelNumber"].Value.Trim();
                                            string innner_TicketNum = m_mch.Groups["TicketNum"].Value.Trim();
                                            if (innner_TravelNumber == Param.TripNumber.Trim() && innner_TicketNum == Param.TicketNumber.Trim())
                                            {
                                                IsCreateSuc = true;
                                            }
                                            else
                                            {
                                                Param.Msg = recvData;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string sucPatern = @"(?<=\<Flag\>\s*)(?<Flag>\w)(?=\s*\<\/Flag\>)";
                                        Match mch = Regex.Match(recvData, sucPatern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                        string TripFlag = "";
                                        if (mch.Success)
                                        {
                                            TripFlag = mch.Groups["Flag"].Value.Trim().ToUpper();
                                        }
                                        if (recvData.Contains("SUCCESS") || recvData.Contains("成功") || TripFlag == "S")
                                        {
                                            IsCreateSuc = true;
                                        }
                                        else
                                        {
                                            Param.Msg = recvData;
                                        }
                                    }
                                }
                                #endregion

                                #region 方法二
                                /*
                                if (Param.UseIns == 1 || Param.UseIns == 2)
                                {
                                    if (!IsCreateSuc)
                                    {
                                        sendIns = string.Format("PASS{0}/{1}", Param.TicketNumber, Param.TripNumber);
                                        if (Param.InsIsEndOffice)
                                        {
                                            sendIns = string.Format("PASS{0}/{1}&{2}", Param.TicketNumber, Param.TripNumber, Param.Office);
                                        }
                                        recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                                        sendIns = string.Format("DETR:TN/{0},F", Param.TicketNumber);
                                        if (Param.InsIsEndOffice)
                                        {
                                            sendIns = string.Format("DETR:TN/{0},F&{1}", Param.TicketNumber, Param.Office);
                                        }
                                        recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                                        if (IsReSend(recvData) || !recvData.ToUpper().Contains("DETR:TN"))
                                        {
                                            recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                                        }
                                        Param.DetrList = format.GetDetrF(recvData);
                                        if (Param.DetrList.Count > 0)
                                        {
                                            DetrInfo detrInfo = Param.DetrList[0];
                                            if (detrInfo.CreateSerialNumber.Trim() != "" && detrInfo.CreateSerialNumber.Trim() == Param.TripNumber && detrInfo.TicketNum.Trim() == Param.TicketNumber.Trim())
                                            {
                                                IsCreateSuc = true;
                                                Param.Msg = string.Format("该票号{0}创建行程单成功!##{0}|{1}", Param.TicketNumber, detrInfo.CreateSerialNumber.Trim());
                                            }
                                        }
                                    }
                                }
                                */
                                #endregion
                            }
                            IsSuc = IsCreateSuc;
                        }
                        else
                        {
                            Param.Msg = recvData;
                        }
                        #endregion
                    }
                    else if (Param.InsType == 1)
                    {
                        #region//作废
                        //判断票号是否已作废
                        bool IsVoid = false;
                        sendIns = string.Format("DETR:TN/{0},F", Param.TicketNumber);
                        if (Param.InsIsEndOffice)
                        {
                            sendIns = string.Format("DETR:TN/{0},F&{1}", Param.TicketNumber, Param.Office);
                        }
                        recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                        if (IsReSend(recvData) || (!recvData.ToUpper().Contains("DETR:TN") && !recvData.ToUpper().Contains("DETR TN")))
                        {
                            recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                        }
                        Param.DetrList = format.GetDetrF(recvData);
                        if (Param.DetrList.Count > 0)
                        {
                            if (Param.DetrList[0].VoidSerialNumber.Trim() != "")
                            {
                                if (Param.DetrList[0].VoidSerialNumber.Trim() == Param.TripNumber && Param.DetrList[0].TicketNum.Trim() == Param.TicketNumber.Trim())
                                {
                                    IsVoid = true;
                                }
                                else
                                {
                                    Param.Msg = string.Format("该票号{0}已作废行程单!##{0}|{1}", Param.TicketNumber, Param.DetrList[0].VoidSerialNumber.Trim());
                                }
                            }
                        }

                        //没有作废就作废
                        if (!IsVoid)
                        {
                            #region 方法一
                            if (Param.UseIns == 0 || Param.UseIns == 2)
                            {
                                sendIns = string.Format("VTINV:{0},{{ITTN={1}}}", Param.TicketNumber, Param.TripNumber);
                                if (Param.InsIsEndOffice)
                                {
                                    sendIns = string.Format("VTINV:{0},{{ITTN={1}}}&{2}", Param.TicketNumber, Param.TripNumber, Param.Office);
                                }
                                recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                                if (IsReSend(recvData) || (!recvData.ToUpper().Contains("DETR:TN") && !recvData.ToUpper().Contains("DETR TN")))
                                {
                                    recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                                }
                                if (recvData.Contains("SUCCESS"))
                                {
                                    IsVoid = true;
                                }
                                else
                                {
                                    if (!recvData.Contains("ERROR"))
                                    {
                                        sendIns = string.Format("DETR:TN/{0},F", Param.TicketNumber);
                                        if (Param.InsIsEndOffice)
                                        {
                                            sendIns = string.Format("DETR:TN/{0},F&{1}", Param.TicketNumber, Param.Office);
                                        }
                                        recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                                        Param.DetrList = format.GetDetrF(recvData);
                                        if (Param.DetrList.Count > 0)
                                        {
                                            if (Param.DetrList[0].VoidSerialNumber.Trim() != "")
                                            {
                                                if (Param.DetrList[0].VoidSerialNumber.Trim() == Param.TripNumber && Param.DetrList[0].TicketNum.Trim() == Param.TicketNumber.Trim())
                                                {
                                                    IsVoid = true;
                                                }
                                                else
                                                {
                                                    Param.Msg = string.Format("该票号{0}作废行程单失败!##{0}|{1}", Param.TicketNumber, Param.DetrList[0].VoidSerialNumber.Trim());
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Param.Msg = recvData;
                                        }
                                    }
                                    else
                                    {
                                        Param.Msg = recvData;
                                    }
                                }
                            }
                            #endregion

                            #region 方法二
                            /*
                            if (Param.UseIns == 1 || Param.UseIns == 2)
                            {
                                if (!IsVoid)
                                {
                                    sendIns = string.Format("VOID{0}/{1}", Param.TicketNumber, Param.TripNumber);
                                    if (Param.InsIsEndOffice)
                                    {
                                        sendIns = string.Format("VOID{0}/{1}&{2}", Param.TicketNumber, Param.TripNumber, Param.Office);
                                    }
                                    recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                                    sendIns = string.Format("DETR:TN/{0},F", Param.TicketNumber);
                                    if (Param.InsIsEndOffice)
                                    {
                                        sendIns = string.Format("DETR:TN/{0},F&{1}", Param.TicketNumber, Param.Office);
                                    }
                                    recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                                    if (IsReSend(recvData) || (!recvData.ToUpper().Contains("DETR:TN") && !recvData.ToUpper().Contains("DETR TN")))
                                    {
                                        recvData = SendOne(sendIns, Param.Office, 6, sbTrip, PM, objList);
                                    }
                                    Param.DetrList = format.GetDetrF(recvData);
                                    if (Param.DetrList.Count > 0)
                                    {
                                        if (Param.DetrList[0].VoidSerialNumber.Trim() != "")
                                        {
                                            if (Param.DetrList[0].VoidSerialNumber.Trim() == Param.TripNumber && Param.DetrList[0].TicketNum.Trim() == Param.TicketNumber.Trim())
                                            {
                                                IsVoid = true;
                                            }
                                            else
                                            {
                                                Param.Msg = string.Format("该票号{0}作废行程单失败!!##{0}|{1}", Param.TicketNumber, Param.DetrList[0].VoidSerialNumber.Trim());
                                            }
                                        }
                                    }
                                }
                            }
                            */
                            #endregion
                        }

                        IsSuc = IsVoid;
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                DataBase.LogCommon.Log.Error("行程单操作：" + sbTrip.ToString(), ex);
            }
            finally
            {
                //记录指令
                #region 指令数据添加到数据库
                if (objList.Count > 0)
                {
                    LogInsList(objList);
                }
                #endregion
            }
            return IsSuc;
        }

        /// <summary>
        /// 修改乘客证件号
        /// </summary>
        /// <param name="pnr">编码</param>
        /// <param name="pnr">航空公司二字码</param>
        /// <param name="Office">该乘客修改的证件号</param>
        /// <param name="Office">Office</param>
        /// <param name="Office">出票Office</param>
        /// <param name="PassengerName">乘客姓名</param>
        /// <param name="ErrMsg">内部错误信息</param>
        /// <returns></returns>
        public bool UpdateSsr(string pnr, string CarryCode, string NewCid, string Office, string PrintOffice, string PassengerName, out string ErrMsg)
        {
            bool IsSuc = false;
            ErrMsg = "";
            //指令日志
            StringBuilder sbInsLog = new StringBuilder();
            sbInsLog.AppendFormat("==修改证件号【时间:{0}】\r\n入口参数:pnr=>{1}\tPassengerName=>{2}\tCarryCode=>{3}\tNewCid=>{4}\tOffice=>{5}\tPrintOffice=>{6}====\r\n",
             System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), pnr, PassengerName, CarryCode, NewCid, Office, PrintOffice
             );
            try
            {
                PnrModel pnrModel = GetPnr(pnr, Office, out ErrMsg);
                if (pnrModel != null)
                {
                    if (!pnrModel.PnrStatus.Contains("XX") && ErrMsg == "")
                    {
                        PnrAnalysis.Model.PassengerInfo pas = pnrModel._PassengerList.Find(delegate(PnrAnalysis.Model.PassengerInfo _P)
                        {
                            if (_P.PassengerName.Trim().ToUpper() == PassengerName.Trim().ToUpper() || _P.YinToINFTName.Trim().ToUpper() == PassengerName.Trim().ToUpper())
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        });
                        if (pas != null)
                        {
                            string RTData = pnrModel._OldPnrContent;
                            string SendIns = "", RecvData = "", Msg = "";
                            StringBuilder sbLog = new StringBuilder();
                            if (pas.PassengerType != "3")
                            {
                                if (RTData.ToUpper().Contains("SSR FOID"))//修改
                                {
                                    //成人或者儿童                               
                                    SendIns = "RT" + pnr + "|XE" + pas.SsrCardIDSerialNum + "|@";
                                    sbInsLog.AppendFormat("时间{0}发送指令:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SendIns);
                                    RecvData = Send(SendIns, ref Office, 8);
                                    sbInsLog.AppendFormat("时间{0}接收数据:\r\n{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SendIns);

                                    SendIns = "RT" + pnr + "|SSR FOID " + CarryCode + " HK/NI" + NewCid + "/P" + pas.SerialNum + "|@";
                                    sbInsLog.AppendFormat("时间{0}发送指令:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SendIns);
                                    RecvData = Send(SendIns, ref Office, 8);
                                    sbInsLog.AppendFormat("时间{0}接收数据:\r\n{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SendIns);
                                }
                                else//添加
                                {
                                    SendIns = "RT" + pnr + "|SSR FOID " + CarryCode + " HK/NI" + NewCid + "/P" + pas.SerialNum + "|@";
                                    sbInsLog.AppendFormat("时间{0}发送指令:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SendIns);
                                    RecvData = Send(SendIns, ref Office, 8);
                                    sbInsLog.AppendFormat("时间{0}接收数据:\r\n{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SendIns);
                                }
                                if (format.INFMarkIsOK(RecvData, out Msg))
                                {
                                    IsSuc = true;
                                }
                            }
                            else
                            {
                                string Pattern = @"^(19|20)\d{2}\-\d{2}\-\d{2}$";
                                if (Regex.IsMatch(NewCid, Pattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase))
                                {
                                    if (RTData.ToUpper().Contains("SSR INFT"))//修改婴儿证件号
                                    {
                                        //婴儿
                                        //rtHS6PQ2|xe8|SSR INFT 3U NN1 Zhang/Ming 08sep12/p1/s2|@
                                        SendIns = string.Format("RT{0}|XE{1}|SSR INFT {2} NN1 {3} {4}/p{5}/s{6}|@", pnr, pas.YinToINFTNum, CarryCode, pas.YinToINFTName, FormatPNR.DateToStr(NewCid, DataFormat.dayMonthYear), pas.YinToAdltNum, pas.YinToLegNum);
                                        sbInsLog.AppendFormat("时间{0}发送指令:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SendIns);
                                        RecvData = Send(SendIns, ref Office, 8);
                                        sbInsLog.AppendFormat("时间{0}接收数据:\r\n{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SendIns);
                                    }
                                    else //添加婴儿证件号
                                    {
                                        PnrAnalysis.Model.LegInfo Leg = pnrModel._LegList[0];
                                        string pinyin = "";
                                        if (PinYingMange.IsChina(PassengerName.Trim()))
                                        {
                                            pinyin = PinYingMange.GetSpellByChinese(PassengerName.Trim().Substring(0, 1)) + "/" + PinYingMange.GetSpellByChinese(PinYingMange.RepacePinyinChar(PassengerName.Trim().Substring(1)));
                                        }
                                        else
                                        {
                                            pinyin = PassengerName.Trim();
                                        }
                                        //rtHS6PQ2|XN:IN/张明INF(sep12)/p1  
                                        //SSR INFT 3U NN1 CTUCAN 8737 E 31OCT Zhang/Ming 02sep12/p1/s2
                                        //@&CTU303
                                        //RTJDQPVZ|XN:IN/王宇INF(jan08)/p1^SSR INFT 3U NN1 CTUPEK 3U8881 Y 31jan Wang/Yu 02jan08/P1/S2^@
                                        StringBuilder sbSendIns = new StringBuilder();
                                        sbSendIns.AppendFormat("RT{0}|XN:IN/{1}INF({2})/p{3}\r", pnr, PassengerName.Trim(), FormatPNR.DateToStr(NewCid, DataFormat.monthYear), pas.YinToAdltNum);
                                        sbSendIns.AppendFormat("SSR INFT {0} NN1 {1} {2} {3} {4} {5} {6}/P{7}/S{8}\r", CarryCode, Leg.FromCode + Leg.ToCode, Leg.AirCode.Replace("*", "") + Leg.FlightNum, Leg.Seat, FormatPNR.DateToStr(Leg.FlyDate1, DataFormat.dayMonth), pinyin, FormatPNR.DateToStr(NewCid, DataFormat.dayMonthYear), pas.YinToAdltNum, Leg.SerialNum);
                                        sbSendIns.Append("@");

                                        SendIns = sbSendIns.ToString();
                                        sbInsLog.AppendFormat("时间{0}发送指令:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SendIns);
                                        RecvData = Send(SendIns, ref Office, 8);
                                        sbInsLog.AppendFormat("时间{0}接收数据:\r\n{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SendIns);
                                    }
                                    if (format.INFMarkIsOK(RecvData, out Msg))
                                    {
                                        IsSuc = true;
                                    }
                                }
                                else
                                {
                                    ErrMsg = "婴儿证件号必须为年-月-日格式!";
                                }
                            }
                        }
                    }
                    else
                    {
                        if (ErrMsg == "")
                        {
                            ErrMsg = "编码中不存在该乘客姓名,解析失败！";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sbInsLog.Append("错误信息:" + ex.Message);
            }
            finally
            {
                sbInsLog.Append("=================================================================================================\r\n");
                //记录日志
                LogText.LogWrite(sbInsLog.ToString(), "UpdateSsr");
            }
            return IsSuc;
        }



        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="cmd">发送指令</param>
        /// <param name="Office">Office</param>
        /// <param name="insType">自定义指令类型 默认0  1取消编码 2获取PNR内容 3获取Pat内容 4对编码授予指定Office的权限 5.循环供应商Office提取指令 6.行程单操作指令,7票号挂起解挂,8修改证件号 9发送特价获取指令 10.备注指令OSI HU CKIN SSAC/S1 11.标识控台发送指令,12.获取特价指令 13.扫描程序发送的指令</param>
        /// <returns></returns>
        public string Send(string cmd, ref string Office, int insType, params object[] objArr)
        {
            StringBuilder sb = new StringBuilder();
            ParamObject PM = new ParamObject();
            PM.ServerIP = m_SendInsParam.IP;
            PM.ServerPort = m_SendInsParam.Port;
            PM.Office = Office;
            PM.code = cmd;
            if (insType == 2 || insType == 5)
            {
                PM.IsPn = true;
            }
            PnrImportParam Param = null;
            if (objArr != null && objArr.Length > 0)
            {
                Param = objArr[0] as PnrImportParam;
            }
            string recvData = "";
            string TempOffice = "";
            try
            {
            lab: if (insType == 5 || string.IsNullOrEmpty(Office))
                {
                    #region 循环Office
                    string[] strArrSupOffice = m_ConfigParam.Office.Split(new string[] { "|", " ", "/", ",", "，", "\\", "#", "^" }, StringSplitOptions.None);
                    for (int i = 0; i < strArrSupOffice.Length; i++)
                    {
                        if (TempOffice != "")
                        {
                            if (TempOffice == strArrSupOffice[i])
                            {
                                continue;
                            }
                        }
                        recvData = SendOne(cmd, strArrSupOffice[i], insType, sb, PM);
                        if (!recvData.Contains("授权"))
                        {
                            Office = strArrSupOffice[i];
                            break;
                        }
                        else
                        {
                            //需要授权的Office
                            if (Param != null)
                            {
                                Param.authOfficeList.Add(strArrSupOffice[i]);
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    recvData = SendOne(cmd, Office, insType, sb, PM);
                    //当前Office没有取到数据转到循环Offcie中
                    if (recvData.Contains("NO PNR") && !cmd.ToLower().Contains("|sp"))
                    {
                        TempOffice = Office;
                        Office = "";
                        goto lab;
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendFormat("\r\n错误信息:{0}", ex.Message);
                PnrAnalysis.LogText.LogWrite(sb.ToString(), "SendIns");
            }
            return recvData;
        }
        /// <summary>
        /// 发送一次指令内部使用
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="Office"></param>
        /// <param name="insType"></param>
        /// <param name="sb"></param>
        /// <param name="PM"></param>
        /// <param name="recvData"></param>
        /// <returns></returns>
        private string SendOne(string cmd, string Office, int insType, StringBuilder sb, ParamObject PM, params object[] obj)
        {
            //发送
            DateTime sendTime = System.DateTime.Now;

            sb.AppendFormat("======[登录账号:{0} 公司ID:{1} IP端口:{2}:{3} Office:{4}]=========>\r\n时间:{0} 发送指令>{1}\r\n", m_SendInsParam.LoginAccount, m_SendInsParam.CpyNo, m_SendInsParam.IP + ":" + m_SendInsParam.Port, Office, sendTime.ToString("yyyy-MM-dd HH:mm:ss"), cmd);
            string recvData = string.Empty;
            if (m_ParamEx.UsePIDChannel == 0)
            {
                //旧版本EC
                ECParam ecParam = new ECParam();
                ecParam.ECIP = PM.ServerIP;
                ecParam.ECPort = PM.ServerPort.ToString();
                ecParam.PID = m_ParamEx.PID;
                ecParam.KeyNo = m_ParamEx.KeyNo;
                ecParam.UserName = m_SendInsParam.LoginAccount;
                SendEC sendec = new SendEC(ecParam);
                string errMsg = "";
                if (!cmd.ToUpper().StartsWith("IG|"))
                {
                    cmd = "IG|" + cmd;
                }
                if (!cmd.Contains("&" + Office))
                {
                    cmd = cmd + "&" + Office + "$#1";
                }
                else
                {
                    cmd = cmd + "#1";
                }
                recvData = sendec.SendData(cmd, out errMsg);
            }
            else if (m_ParamEx.UsePIDChannel == 2)
            {
                //新版本PID
                PM.Office = Office;
                PM.code = cmd;
                recvData = SendNewPID.SendCommand(PM);
            }
            DateTime recvTime = System.DateTime.Now;
            Tb_SendInsData sendins = new Tb_SendInsData();
            sendins.SendIns = cmd;
            sendins.RecvData = recvData;
            sendins.SendInsType = insType;
            sendins.SendTime = sendTime;
            sendins.RecvTime = recvTime;
            sendins.Office = Office;
            sendins.ServerIPAndPort = PM.ServerIP + ":" + PM.ServerPort;
            sendins.UserAccount = m_SendInsParam.LoginAccount;
            sendins.CpyNo = m_SendInsParam.CpyNo;
            List<Tb_SendInsData> insObj = null;
            if (obj != null && obj.Length > 0)
            {
                insObj = obj[0] as List<Tb_SendInsData>;
                insObj.Add(sendins);
            }
            else
            {
                LogIns(sendins);
            }
            sb.AppendFormat("时间:{0}接收数据>\r\n{1}\r\n\r\n\r\n", recvTime.ToString("yyyy-MM-dd HH:mm:ss"), recvData);
            return recvData;
        }
    }
}
