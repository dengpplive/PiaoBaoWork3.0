using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PnrAnalysis;
using PBPid.WebManage;
using System.Text.RegularExpressions;
using PbProject.Logic.User;
using PbProject.Model;
using PbProject.Dal.ControlBase;
using DataBase.Data;
using PbProject.Logic.ControlBase;
using PbProject.WebCommon.Utility;
public partial class AJAX_GetInsInfo : BasePage
{
    /// <summary>
    /// 获取指令内容
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        string SendIns = Request["SendIns"].ToString();
        string Office = Request["Office"].ToString();
        string CpyCode = Request["CpyCode"].ToString();
        string Other = Request["Other"].ToString();
        string strData = GetData(SendIns, Office, CpyCode, Other);
        OutputData(strData);
    }

    public string TrimSQL(string strSQL)
    {
        if (!string.IsNullOrEmpty(strSQL))
        {
            strSQL = strSQL.Replace("\'", "");
        }
        return strSQL;
    }
    /// <summary>
    /// 参数名获取数据
    /// </summary>
    /// <param name="ParamName"></param>
    /// <param name="db_param"></param>
    /// <returns></returns>
    public string GetValue(string ParamName, List<Bd_Base_Parameters> db_param)
    {
        string Val = "";
        IHashObject param = new HashObject();
        param.Add("SetName", ParamName);
        List<Bd_Base_Parameters> db_ImportMark = baseDataManage.CallMethod("Bd_Base_Parameters", "GetListByAndParam", null, new object[] { param, db_param }) as List<Bd_Base_Parameters>;
        if (db_ImportMark != null && db_ImportMark.Count > 0)
        {
            Val = db_ImportMark[0].SetValue;
        }
        return Val;
    }


    /// <summary>
    /// 获取指令数据
    /// </summary>
    /// <param name="SendIns"></param>
    /// <param name="Office"></param>
    /// <param name="cpyNo">公司编号</param>
    /// <returns></returns>
    public string GetData(string SendIns, string Office, string cpyNo, string Other)
    {
        string recvData = string.Empty;
        try
        {
            ConfigParam CP = null;
            if (!string.IsNullOrEmpty(Other))
            {
                string[] strArr = Other.Split(new string[] { "@@@@" }, StringSplitOptions.None);
                string strHeiPingCanShu = strHeiPingCanShu = strArr[0];
                string strDaPeiZhiCanShu = strArr[1];
                CP = GetConfigParam(strHeiPingCanShu, strHeiPingCanShu);
            }
            if (string.IsNullOrEmpty(SendIns))
            {
                recvData = "发送指令为空";
                return recvData;
            }
            if (CP == null)
            {
                recvData = "参数错误";
                return recvData;
            }
            IHashObject param = new HashObject();
            string sqlWhere = string.Format("UninCode='{0}' and  RoleType in(2,3) ", cpyNo);
            User_Company m_Company = null;
            List<User_Company> CompanyList = baseDataManage.CallMethod("User_Company", "GetList", null, new object[] { sqlWhere }) as List<User_Company>;
            if (CompanyList != null && CompanyList.Count > 0)
            {
                //该公司实体
                m_Company = CompanyList[0];
                //该公司参数表信息               
                List<Bd_Base_Parameters> db_param = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new object[] { "CpyNo='" + cpyNo + "'" }) as List<Bd_Base_Parameters>;

                string Mark = BaseParams.getParams(db_param).KongZhiXiTong;

                string bigIP = "127.0.0.1", bigPort = "391", BigOffice = "";
                string IP = "127.0.0.1", Port = "391";


                Tb_SendInsData sendins = new Tb_SendInsData();
                sendins.SendInsType = 11;  //标识为控台系统发送的指令       
                sendins.UserAccount = mUser != null ? mUser.LoginName : "控台管理员";
                sendins.CpyNo = mUser != null ? mUser.CpyNo : "控台管理员";

                //查找白屏预订Pid的IP地址
                IP = CP.WhiteScreenIP;
                //查找白屏预订Pid的端口
                Port = CP.WhiteScreenPort;
                //查找大配置IP           
                bigIP = CP.BigCfgIP;
                //查找大配置Port             
                bigPort = CP.BigCfgPort;
                //查找大配置Office              
                BigOffice = CP.BigCfgOffice;

                //使用的IP 端口 Office
                string ServerIP = "";
                int ServerPort = 0;
                //是否开启大配置
                bool IsUseBigConfig = Mark.Contains("|39|");
                //是有使用新的PID
                bool IsUseNewPid = Mark.Contains("|48|");

                if (IsUseBigConfig)
                {
                    //大配置
                    int _Port = 451;
                    int.TryParse(bigPort, out _Port);
                    ServerIP = bigIP;
                    ServerPort = _Port;
                    //大配置Office
                    Office = BigOffice;
                }
                else
                {
                    int.TryParse(Port, out ServerPort);
                    ServerIP = IP;
                }
                string[] OfficeNum = null;
                string tempOffice = CP.Office;//GetValue("office", db_param);
                if (Office == "")
                {
                    //空台设置的Office
                    OfficeNum = tempOffice.Split(new string[] { "|", " ", "/", ",", "，", "\\", "#", "^" }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    //空台设置的Office
                    OfficeNum = Office.Split(new string[] { "|", " ", "/", ",", "，", "\\", "#", "^" }, StringSplitOptions.RemoveEmptyEntries);
                }
                ///使用新的PID
                if (IsUseNewPid)
                {
                    //DataModel.A2 = "新的PID";
                    // WebManage.ServerIp = ServerIP;
                    // WebManage.ServerPort = ServerPort;
                    ParamObject Pm = new ParamObject();
                    Pm.ServerIP = ServerIP;
                    Pm.ServerPort = ServerPort;


                    bool IsPn = false;//是否PN
                    string patternPnr = @"\s*(?<=rt|\(eas\)rt|rtx/|\(eas\)rtx/)(?=\w{6})\s*";
                    Match mch = Regex.Match(SendIns, patternPnr, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    if (mch.Success)
                    {
                        IsPn = true;
                    }
                    SendIns = SendIns.ToLower().StartsWith("ig|") ? SendIns.Trim().Substring(2) : SendIns;
                    if (Office != "" && Office.IndexOf("|") == -1)
                    {
                        //发送指令数据
                        SendIns = SendIns.Replace("\n", "").Replace("\r", "^");
                        //去掉ig
                        SendIns = SendIns.ToLower().StartsWith("ig|") ? SendIns.Trim().Substring(3).ToLower() : SendIns.ToLower();
                        sendins.SendIns = SendIns;
                        sendins.Office = Office;
                        sendins.ServerIPAndPort = ServerIP + ":" + ServerPort;
                        sendins.SendTime = System.DateTime.Now;

                        Pm.code = SendIns;
                        Pm.IsPn = IsPn;
                        Pm.Office = Office;
                        recvData = SendNewPID.SendCommand(Pm);

                        //recvData = WebManage.SendCommand(SendIns, Office, IsPn, false, ServerIP, ServerPort);

                        sendins.RecvTime = System.DateTime.Now;
                        if (recvData == null)
                        {
                            recvData = "";
                        }
                        recvData = recvData.Replace("^", "\r");
                        sendins.RecvData = recvData;
                        //添加日志
                        AddLog(sendins);
                    }
                    if (recvData.Contains("授权") || Office == "")
                    {
                        foreach (string _Office in OfficeNum)
                        {
                            if (_Office.ToLower() != Office.ToLower())
                            {
                                Office= _Office.ToLower();
                                //发送指令数据
                                SendIns = SendIns.Replace("\n", "").Replace("\r", "^");
                                //去掉ig
                                SendIns = SendIns.ToLower().StartsWith("ig|") ? SendIns.Trim().Substring(3).ToLower() : SendIns.ToLower();
                                sendins.SendIns = SendIns;
                                sendins.Office = Office;
                                sendins.ServerIPAndPort = ServerIP + ":" + ServerPort;
                                sendins.SendTime = System.DateTime.Now;
                                
                                Pm.code=SendIns;
                                Pm.IsPn = IsPn;
                                Pm.Office = Office;
                                recvData = SendNewPID.SendCommand(Pm);

                                //recvData = WebManage.SendCommand(SendIns, _Office, IsPn, false, ServerIP, ServerPort);

                                sendins.RecvTime = System.DateTime.Now;
                                if (recvData == null)
                                {
                                    recvData = "";
                                }
                                recvData = recvData.Replace("^", "\r");
                                sendins.RecvData = recvData;
                                //添加日志
                                AddLog(sendins);
                            }
                            if (!recvData.Contains("授权"))
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Office = (Office == "" ? "" : "&" + Office.TrimEnd('$').Trim() + "$") + "#1";
                    ECParam ecParam = new ECParam();
                    ecParam.ECIP = ServerIP;
                    ecParam.ECPort = ServerPort.ToString();
                    //ecParam.PID = supModel.PId;
                    //ecParam.KeyNo = supModel.KeyNo;

                    ecParam.UserName = mUser == null ? "控台管理员" : mUser.UserName;
                    SendEC sendec = new SendEC(ecParam);
                    if (Office != "")
                    {
                        //发送指令数据
                        //logPnr.SSContent = "[EC:" + ServerIP + ":" + ServerPort + "|" + Office + "]" + SendIns + Office;
                        sendins.Office = Office;
                        sendins.ServerIPAndPort = ServerIP + ":" + ServerPort;
                        sendins.SendTime = System.DateTime.Now;
                        sendins.SendIns = SendIns + Office;
                        recvData = sendec.SendData(SendIns + Office, out recvData);
                        sendins.RecvData = recvData;
                        sendins.RecvTime = System.DateTime.Now;
                        // logPnr.ResultContent = recvData;
                        //添加日志
                        AddLog(sendins);
                    }
                    if (recvData.Contains("授权") || Office == "" || Office == "#1")
                    {
                        tempOffice = "";
                        foreach (string _Office in OfficeNum)
                        {
                            if (_Office.ToLower() != Office.ToLower())
                            {
                                tempOffice = (_Office == "" ? "" : "&" + _Office.TrimEnd('$').Trim() + "$") + "#1";
                                //logPnr.SSContent = "[EC:" + ServerIP + ":" + ServerPort + "|" + Office + "]" + SendIns + Office;
                                //发送指令数据  
                                sendins.SendIns = SendIns + Office;
                                sendins.Office = _Office;
                                sendins.ServerIPAndPort = ServerIP + ":" + ServerPort;
                                sendins.SendTime = System.DateTime.Now;
                                recvData = sendec.SendData(SendIns + tempOffice, out recvData);
                                sendins.RecvData = recvData;
                                sendins.RecvTime = System.DateTime.Now;
                                //  logPnr.ResultContent = recvData;
                                //添加日志
                                AddLog(sendins);
                            }
                            if (!recvData.Contains("授权"))
                            {
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                recvData = "该供应商不存在";
                return recvData;
            }
        }
        catch (Exception ex)
        {
            recvData = System.DateTime.Now + ":" + ex.Message + "|" + ex.StackTrace.ToString();
        }
        return escape(recvData);
    }
    /// <summary>
    /// 记录指令
    /// </summary>
    /// <param name="tb_sendinsdata"></param>
    /// <returns></returns>
    public bool AddLog(Tb_SendInsData tb_sendinsdata)
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
                Insert = this.baseDataManage.ExecuteSqlTran(sqlList, out errMsg);
            }
        }
        return Insert;
    }
    /// <summary>
    /// 输出数据
    /// </summary>
    /// <param name="data"></param>
    public void OutputData(string data)
    {
        try
        {
            if (string.IsNullOrEmpty(data))
            {
                data = escape("错误");
            }
            Response.Clear();
            Response.Write(data);
            Response.Flush();
            Response.End();
        }
        catch (Exception)
        {
        }
    }
}