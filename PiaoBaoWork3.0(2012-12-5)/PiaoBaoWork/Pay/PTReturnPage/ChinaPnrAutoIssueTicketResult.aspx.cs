using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.ConsoleServerProc.Utils;
using System.IO;
using System.Text;
using System.Data;
using System.Collections.Specialized;
using PbProject.Logic;


public partial class Pay_PTReturnPage_ChinaPnrAutoIssueTicketResult : System.Web.UI.Page
{
    private string m_Version = "";
    private string m_ReqType = "";
    private string m_GUID = "";
    private string m_DrawResult = "";
    private string m_DrawResultDes = "";
    private string m_OrderAmt = "";
    private string m_AgencyPct = "";
    private string m_AgentFee = "";
    private string m_ChkValue = "";
    ChinaPnrClient client = null;
    private string m_Key = "";
    private string PnrNo = "";

    protected override void InitializeCulture()
    {
        base.InitializeCulture();
        Request.ContentEncoding = Encoding.GetEncoding("gb2312");//设置编码方式
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        LogInfo("进入汇付天下自动出票回调...", false);
        m_Key = System.Configuration.ConfigurationManager.AppSettings["HuifuAutoKey"];
        client = new ChinaPnrClient(System.Configuration.ConfigurationManager.AppSettings["HuifuAutoIP"]);
        if (string.IsNullOrEmpty(m_Key))
        {
            string msg = PnrNo + "_FAIL未配置Key";
            LogInfo("未配置Key", false);
            client.Send(AddHeadLen(msg));
            return;
        }
        GetParams();

        PbProject.Model.Tb_Ticket_Order orderList = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetTicketOrderByOrderId(m_GUID);
        if (orderList == null)
        {
            string msg = PnrNo + "_FAIL未找到相应订单";
            LogInfo("未找到相应订单", false);
            client.Send(AddHeadLen(msg));
            return;
        }
        PnrNo = orderList.PNR;
        if (!CheckParams())
        {
            if (!string.IsNullOrEmpty(m_DrawResult) && m_DrawResult.Trim() != "000000")
                RollBackTicketOrderState(orderList, "出票失败,原因:回调参数不完整");
            string msg = PnrNo + "_FAIL参数不完整";
            LogInfo(string.Format("参数不完整，订单号：{0}，PNR：{1}", orderList.OrderId, PnrNo), false);
            client.Send(AddHeadLen(msg));
            return;
        }
        if (!CheckSign())
        {
            if (m_DrawResult.Trim() != "000000")
                RollBackTicketOrderState(orderList, "出票失败,原因:回调签名验证失败");
            string msg = PnrNo + "_FAIL签名验证失败";
            LogInfo(string.Format("签名验证失败，订单号：{0}，PNR：{1}", orderList.OrderId, PnrNo), false);
            client.Send(AddHeadLen(msg));
            return;
        }
        if (orderList.OrderStatusCode == 4)
        {
            string msg = PnrNo + "_FAIL已出票";
            LogInfo(string.Format("已经出票，无需重复出票，订单号：{0}，PNR：{1}", orderList.OrderId, PnrNo), false);
            client.Send(AddHeadLen(msg));
            return;
        }
        //Login(orderList);
        //更新票号
        UpdateTicketOrder(orderList);

        
    }


    public void Login(PbProject.Model.Tb_Ticket_Order order)
    {
        LogInfo("开始Login", false);
        string sql = "select LoginName,LoginPassWord from User_Employees where CpyNo='" + order.OwnerCpyNo.Substring(0, 12) + "' and IsAdmin=0";
        PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sq = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
        LogInfo("开始查询", false);
        DataTable tb = sq.ExecuteStrSQL(sql);
        LogInfo("结束查询", false);
        string msg = "";
        PbProject.Logic.Login LoginManage = new PbProject.Logic.Login();
        DataTable[] tableArr = null;
        bool IsSuc = LoginManage.GetByName(tb.Rows[0][0].ToString(), tb.Rows[0][1].ToString(), false, Page.Request.UserHostAddress, out tableArr, out msg, 1);
        LogInfo("结束Login", false);
    }

    /// <summary>
    /// 获取表单参数
    /// </summary>
    private void GetParams()
    {
        Encoding encoding = Encoding.GetEncoding("gb2312");
        Stream resStream = Request.InputStream;
        byte[] filecontent = new byte[resStream.Length];
        resStream.Read(filecontent, 0, filecontent.Length);
        string postquery = encoding.GetString(filecontent);
        NameValueCollection result = GetFormParams(postquery);

        if (result["Vesion"] != null)
            m_Version = result["Vesion"];
        if (result["ReqType"] != null)
            m_ReqType = result["ReqType"];
        if (result["GUID"] != null)
            m_GUID = result["GUID"];
        if (result["DrawResult"] != null)
            m_DrawResult = result["DrawResult"];
        if (result["DrawResultDes"] != null)
            m_DrawResultDes = result["DrawResultDes"];
        if (result["OrderAmt"] != null)
            m_OrderAmt = result["OrderAmt"];
        if (result["AgencyPct"] != null)
            m_AgencyPct = result["AgencyPct"];
        if (result["AgentFee"] != null)
            m_AgentFee = result["AgentFee"];
        if (result["ChkValue"] != null)
            m_ChkValue = result["ChkValue"];
    }

    /// <summary>
    /// 检查参数完整性
    /// </summary>
    /// <returns></returns>
    private bool CheckParams()
    {
        if (string.IsNullOrEmpty(m_Version)
            || string.IsNullOrEmpty(m_GUID)
            || string.IsNullOrEmpty(m_DrawResult)
            || string.IsNullOrEmpty(m_ChkValue))
        {
            return false;
        }
        StringBuilder sb = new StringBuilder();
        if (!string.IsNullOrEmpty(m_Version))
        {
            sb.Append(string.Format(" 参数m_Version:{0}", m_Version));
        }
        if (!string.IsNullOrEmpty(m_ReqType))
        {
            sb.Append(string.Format(" 参数m_ReqType:{0}", m_ReqType));
        }
        if (!string.IsNullOrEmpty(m_GUID))
        {
            sb.Append(string.Format(" 参数m_GUID:{0}", m_GUID));
        }
        if (!string.IsNullOrEmpty(m_DrawResult))
        {
            sb.Append(string.Format(" 参数m_DrawResult:{0}", m_DrawResult));
        }
        if (!string.IsNullOrEmpty(m_DrawResultDes))
        {
            sb.Append(string.Format(" 参数m_DrawResultDes:{0}", m_DrawResultDes));
        }
        if (!string.IsNullOrEmpty(m_OrderAmt))
        {
            sb.Append(string.Format(" 参数m_OrderAmt:{0}", m_OrderAmt));
        }
        if (!string.IsNullOrEmpty(m_AgencyPct))
        {
            sb.Append(string.Format(" 参数m_AgencyPct:{0}", m_AgencyPct));
        }
        if (!string.IsNullOrEmpty(m_AgentFee))
        {
            sb.Append(string.Format(" 参数m_AgentFee:{0}", m_AgentFee));
        }
        if (!string.IsNullOrEmpty(m_ChkValue))
        {
            sb.Append(string.Format(" 参数m_ChkValue:{0}", m_ChkValue));
        }
        LogInfo(sb.ToString(), false);
        return true;
    }

    /// <summary>
    /// 检查签名是否正确
    /// </summary>
    /// <returns></returns>
    private bool CheckSign()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(m_Key).Append(m_Version).Append(m_ReqType).Append(m_GUID).Append(m_DrawResult).Append(m_DrawResultDes)
            .Append(m_OrderAmt).Append(m_AgencyPct);
        string sign = StringUtils.GetMd5(sb.ToString());
        if (sign == m_ChkValue.ToUpper())
            return true;
        else
            return false;
    }

    /// <summary>
    /// 为参数加上4位标识字符长度的数字，不足用0补齐
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private string AddHeadLen(string str)
    {
        string headLen = str.Length.ToString();
        if (4 - headLen.Length > 0)
        {
            for (int i = 0; i < 4 - headLen.Length; i++)
            {
                headLen = "0" + headLen;
            }
        }
        return headLen + str.ToString();
    }

    /// <summary>
    /// 解析查询字符串
    /// </summary>
    /// <param name="postQuery"></param>
    /// <returns></returns>
    private NameValueCollection GetFormParams(string postQuery)
    {
        NameValueCollection result = new NameValueCollection();
        string[] nameValueList = postQuery.Split('&');
        foreach (string item in nameValueList)
        {
            if (item.Contains('='))
            {
                string[] nameValue = item.Split('=');
                result.Add(nameValue[0], nameValue[1]);
            }
        }
        return result;
    }

    /// <summary>
    /// 更新出票相关信息
    /// </summary>
    /// <param name="order"></param>
    private void UpdateTicketOrder(PbProject.Model.Tb_Ticket_Order order)
    {
        string resultDes = HttpUtility.UrlDecode(m_DrawResultDes, Encoding.GetEncoding("gb2312"));
        if (m_DrawResult.Trim() == "000000")//出票成功
        {
            string[] passengerInfoList = resultDes.Split(';');
            PbProject.Logic.Order.Tb_Ticket_PassengerBLL psb = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL();
            List<PbProject.Model.Tb_Ticket_Passenger> psmList = psb.GetPasListByOrderID(order.OrderId);

            psmList = ModifyPassenger(passengerInfoList, psb, psmList);

            order = ModifyOrder(order);

            List<PbProject.Model.User_Company> mCompany = new PbProject.Logic.ControlBase.BaseDataManage().
                CallMethod("User_Company", "GetList", null, new Object[] { "UninCode='" + order.CPCpyNo + "'" }) as List<PbProject.Model.User_Company>;

            List<PbProject.Model.User_Employees> mUser = new PbProject.Logic.ControlBase.BaseDataManage().
      CallMethod("User_Employees", "GetList", null, new Object[] { " IsAdmin=0 and CpyNo='" + order.CPCpyNo + "'" }) as List<PbProject.Model.User_Employees>;
            bool reuslt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().OperOrderCP(order, psmList, mUser[0], mCompany[0],"");
            if (reuslt)
            {
                #region  票宝开放服务接口异步通知出票

                if (order.OrderSourceType == 5)
                {
                    PbProject.Logic.PTInterface.PbInterfaceNotify pbInterfaceCmd = new PbProject.Logic.PTInterface.PbInterfaceNotify();
                    if (pbInterfaceCmd != null)
                    {
                        bool pbNotifyResult = pbInterfaceCmd.NotifyTicketNo(order);
                    }
                }
                #endregion
                string msg = PnrNo + "_SUCC";
                client.Send(AddHeadLen(msg));
                LogInfo(string.Format("出票成功.订单号:{0},PNR:{1}", order.OrderId, PnrNo), false);
            }
            else
            {
                LogInfo(string.Format("出票失败.订单号:{0},PNR:{1}", order.OrderId, PnrNo), false);
            }
            
        }
        else
        {
            LogInfo(resultDes + "，订单号：" + order.OrderId + "，PNR：" + PnrNo, false);
            RollBackTicketOrderState(order, string.Format("出票失败,原因:{0}", resultDes));
        }
        client.Send(AddHeadLen(PnrNo + "_SUCC"));//返回成功
    }

    /// <summary>
    /// 记录失败日志到数据库
    /// </summary>
    /// <param name="order"></param>
    /// <param name="content"></param>
    private void FailedLogToDb(PbProject.Model.Tb_Ticket_Order order, string content)
    {

        #region 记录操作日志
        //添加操作订单的内容
        PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

        OrderLog.id = Guid.NewGuid();
        OrderLog.OrderId = order.OrderId;
        OrderLog.OperType = "修改";
        OrderLog.OperTime = DateTime.Now;
        OrderLog.OperContent = "于 " + DateTime.Now + "," + content;
        OrderLog.WatchType = 2;
        string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
        #endregion
    }

    /// <summary>
    /// 修改订单
    /// </summary>
    /// <param name="order"></param>
    private PbProject.Model.Tb_Ticket_Order ModifyOrder(PbProject.Model.Tb_Ticket_Order order)
    {
        LogInfo("开始修改订单", false);
        order.AirPoint = 1 - (Convert.ToDecimal(m_OrderAmt) - order.ABFee - order.FuelFee) / order.PMFee;
        order.AirPoint = Math.Round(order.AirPoint, 3);
        order.OrderStatusCode = 4;
        return order;
    }

    /// <summary>
    /// 修改票号
    /// </summary>
    /// <param name="passengerInfoList"></param>
    /// <param name="psb"></param>
    /// <param name="psmList"></param>
    private List<PbProject.Model.Tb_Ticket_Passenger> ModifyPassenger(string[] passengerInfoList, PbProject.Logic.Order.Tb_Ticket_PassengerBLL psb, List<PbProject.Model.Tb_Ticket_Passenger> psmList)
    {
        //LogInfo("开始修改票号", false);
        for (int i = 0; i < psmList.Count; i++)
        {
            foreach (string passengerInfo in passengerInfoList)
            {
                if (psmList[i].PassengerName == passengerInfo.Split('+')[1])
                {
                    psmList[i].TicketStatus = 2;
                    psmList[i].TicketNumber = passengerInfo.Split('+')[0];
                    //psb.UpdateTb_Ticket_Passenger(psmList[i]);
                    //LogInfo("修改乘客：【" + psmList[i].PassengerName + "】票号完成", false);
                }
            }
        }
        return psmList;
        //LogInfo("修改票号完成", false);
    }

    /// <summary>
    /// 还原订单状态，改为手动出票
    /// </summary>
    /// <param name="order"></param>
    private void RollBackTicketOrderState(PbProject.Model.Tb_Ticket_Order order, string msg)
    {
        try
        {
            string SQL = " update Tb_Ticket_Order set A1=2,AutoPrintFlag=0 where OrderId='" + order.OrderId + "'";
            FailedLogToDb(order, msg);
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base ss = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            ss.ExecuteNonQuerySQLInfo(SQL);
            //LogInfo(string.Format("{0},已改为手动方式,订单号:{1}", msg, order.OrderId), false);
        }
        catch (Exception e)
        {
            LogInfo("出票失败,改为手动方式失败.原因:" + e.Message, false);
        }
    }

    /// <summary>
    /// 记录文本日志
    /// </summary>
    /// <param name="content">记录内容</param>
    /// <param name="IsPostBack">是否记录 Request 参数</param>
    private void LogInfo(string errContent, bool IsRecordRequest)
    {
        try
        {
            PbProject.WebCommon.Log.Log.RecordLog(Page.ToString(), errContent, IsRecordRequest, null);
            #region 记录文本日志
            /*
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("记录时间：" + DateTime.Now.ToString() + "\r\n");
            sb.AppendFormat("  IP ：" + Page.Request.UserHostAddress + "\r\n");
            sb.AppendFormat("  Content : " + errContent + "\r\n");

            if (IsRecordRequest)
            {
                #region 记录 Request 参数
                try
                {
                    sb.AppendFormat("  Request.HttpMethod:" + HttpContext.Current.Request.HttpMethod + "\r\n");

                    if (HttpContext.Current.Request != null)
                    {
                        if (HttpContext.Current.Request.HttpMethod == "POST")
                        {
                            #region POST 提交
                            if (HttpContext.Current.Request.Form.Count != 0)
                            {
                                for (int i = 0; i < HttpContext.Current.Request.Form.Count; i++)
                                {
                                    sb.AppendFormat(HttpContext.Current.Request.Form.Keys[i].ToString() + " = " + HttpContext.Current.Request.Form[i].ToString() + "\r\n");
                                }
                            }
                            else
                            {
                                sb.AppendFormat(" HttpContext.Current.Request.Form.Count = 0 \r\n");
                            }

                            #endregion
                        }
                        else if (HttpContext.Current.Request.HttpMethod == "GET")
                        {
                            #region GET 提交

                            if (HttpContext.Current.Request.QueryString.Count != 0)
                            {
                                for (int i = 0; i < HttpContext.Current.Request.QueryString.Count; i++)
                                {
                                    sb.AppendFormat(HttpContext.Current.Request.QueryString.Keys[i].ToString() + " = " + HttpContext.Current.Request.QueryString[i].ToString() + "\r\n");
                                }
                            }
                            else
                            {
                                sb.AppendFormat(" HttpContext.Current.QueryString.Form.Count = 0 \r\n");
                            }

                            #endregion
                        }
                        else
                        {
                            #region 不是 GET 和 POST

                            sb.AppendFormat("  不是 GET 和 POST, Request.HttpMethod:" + HttpContext.Current.Request.HttpMethod + "\r\n");

                            System.Collections.Specialized.NameValueCollection nv = Request.Params;
                            foreach (string key in nv.Keys)
                            {
                                sb.AppendFormat("{0}={1} \r\n", key, nv[key]);
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        sb.AppendFormat("  HttpContext.Current.Request=null \r\n");
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendFormat("  catch: " + ex + "\r\n");
                }

                #endregion
            }

            sb.AppendFormat("----------------------------------------------------------------------------------------------------\r\n");
            sb.AppendFormat("----------------------------------------------------------------------------------------------------");

            string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Log\\" + Page + "\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            StreamWriter fs = new StreamWriter(dir + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true, System.Text.Encoding.Default);
            fs.WriteLine(sb.ToString());
            fs.Close();
            */
            #endregion
        }
        catch (Exception)
        {

        }
    }
}