using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.IO;
using PbProject.Logic;

public partial class Pay_PTReturnPage_TodayNotifyUrl : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        OnErrorNew("进入 Pay_TodayNotifyUrl_Load（）", true);
        try
        {
            if (Request.Form["ticket_no"] != null)
            {
                GetData();
            }
        }
        catch (Exception ex)
        {
            OnErrorNew(ex.Message.ToString(), false);
        }
    }
    private string GetValue(string Key)
    {
        string value = "";
        foreach (string key in Request.Form.Keys)
        {
            if (key.Trim() == Key)
            {
                value = Request.Form[key].ToString().Trim();
                break;
            }
        }
        return value;
    }

    private void GetData()
    {
        string ticketnoinfo = "";
        for (int i = 0; i < Request.Form.Count; i++)
        {
            ticketnoinfo = ticketnoinfo + Request.Form.Keys[i].ToString() + ":" + Request.Form[i].ToString() + "|";
        }
        ticketnoinfo = HttpUtility.UrlDecode(ticketnoinfo, Encoding.GetEncoding("gb2312"));
        OnErrorNew("ticketnoinfo内容：" + ticketnoinfo, false);
        if (GetValue("ticket_no").ToString().Trim() != "")
        {
            OnErrorNew("进入 Request.Form['status'].ToString().Trim() != ''", false);
            if (GetValue("status").ToString().Trim() == "T" && GetValue("service").ToString().Trim() == "order_issue_notify")//出票完成
            {
                bool reuslt = false;
                OnErrorNew("进入出票 Request.Form['status'].ToString().Trim() == 'T'", false);
                StringBuilder sb = new StringBuilder();
                PbProject.Model.Tb_Ticket_Order OrderUpdateList = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetTicketOrderByOrderId(GetValue("order_no").ToString());
                //Login(OrderUpdateList);
                if (OrderUpdateList.OrderStatusCode == 3)
                {

                    int tcount = 0;
                    //OrderUpdateList.OrderStatusCode = 4; //出票状态

                    PbProject.Logic.Order.Tb_Ticket_PassengerBLL PassengerManager = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL();
                    List<PbProject.Model.Tb_Ticket_Passenger> PassengerList = PassengerManager.GetPasListByOrderID(OrderUpdateList.OrderId);
                    for (int j = 0; j < PassengerList.Count; j++)
                    {
                        string sss = HttpUtility.UrlDecode(GetValue("passenger").ToString(), Encoding.GetEncoding("gb2312"));
                        string[] Name = sss.Split('^');
                        for (int i = 0; i < Name.Length; i++)
                        {
                            if (PassengerList[j].PassengerName.Trim().Replace("CHD", "").Trim().Replace(" ", "") == Name[i].Replace("CHD", "").Trim().Replace(" ", ""))
                            {
                                PassengerList[j].TicketNumber = GetValue("ticket_no").ToString().Split('^')[i].ToString();
                                tcount++;
                            }
                        }
                    }

                    if (tcount == PassengerList.Count)
                    {
                        OrderUpdateList.OrderStatusCode = 4; //出票状态
                    }
                    else
                    {
                        #region 记录操作日志
                        //添加操作订单的内容
                        PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
                        PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                        OrderLog.id = Guid.NewGuid();
                        OrderLog.OrderId = OrderUpdateList.OrderId;
                        OrderLog.OperType = "修改";
                        OrderLog.OperTime = DateTime.Now;
                        OrderLog.OperContent = "自动回填票号失败:乘机人与票号不符，需要手动操作!";
                        OrderLog.WatchType = 2;
                        string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                        sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                        #endregion
                    }

                    List<PbProject.Model.User_Company> mCompany = new PbProject.Logic.ControlBase.BaseDataManage().
               CallMethod("User_Company", "GetList", null, new Object[] { "UninCode='" + OrderUpdateList.CPCpyNo + "'" }) as List<PbProject.Model.User_Company>;

                    List<PbProject.Model.User_Employees> mUser = new PbProject.Logic.ControlBase.BaseDataManage().
      CallMethod("User_Employees", "GetList", null, new Object[] { " IsAdmin=0 and CpyNo='" + OrderUpdateList.CPCpyNo + "'" }) as List<PbProject.Model.User_Employees>;
                    reuslt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().OperOrderCP(OrderUpdateList, PassengerList, mUser[0], mCompany[0],"");
                    if (reuslt)
                    {
                        sb.AppendFormat("更新数据库，订单号：" + OrderUpdateList.OrderId + "信息，更新成功！\r\n");
                        #region  票宝开放服务接口异步通知出票

                        if (OrderUpdateList.OrderSourceType == 5)
                        {
                            PbProject.Logic.PTInterface.PbInterfaceNotify pbInterfaceCmd = new PbProject.Logic.PTInterface.PbInterfaceNotify();
                            if (pbInterfaceCmd != null)
                            {
                                bool pbNotifyResult = pbInterfaceCmd.NotifyTicketNo(OrderUpdateList);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        sb.AppendFormat("更新数据库，订单号：" + OrderUpdateList.OrderId + "信息，更新失败！\r\n");
                    }
                    OnErrorNew("今日记录：" + sb.ToString(), false);
                }
            }
            else if (GetValue("status").ToString().Trim() == "1" && GetValue("service").ToString().Trim() == "order_refund_notify")
            {
                PbProject.Model.Tb_Ticket_Order Order = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetTicketOrderByOrderId(GetValue("order_no").ToString());
                OnErrorNew("今日退废票成功" + Order.OrderId, false);
                //Order.A40 = "3";
                //#region 记录日志
                //PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();
                //PbProject.Logic.Log.Log_ErrorBLL OrderLogManager = PiaoBao.BLLLogic.Factory_Air.CreateILog_Tb_AirOrderManager();
                //OrderLog.PNR = Order.PNR;
                //OrderLog.OrderId = Order.OrderId;
                //if (Order.OrderType == 3)
                //{
                //    OrderLog.OperateType = 14;
                //}
                //else if (Order.OrderType == 4)
                //{
                //    OrderLog.OperateType = 17;
                //}
                //OrderLog.OperateTime = DateTime.Now;
                //OrderLog.Content = "于 " + DateTime.Now + " 今日平台供应已退票";
                //OrderLog.OperateId = "adminys";
                //OrderLog.OperateName = "管理员";
                //OrderLog.OperateCorporationId = 1;
                //OrderLog.A1 = 1;
                //int Number = OrderLogManager.InsertLog_Tb_AirOrder(OrderLog);
                //#endregion
                //OrderMan.UpdateTb_Ticket_Order(Order);
            }
            else if (GetValue("status").ToString().Trim() == "2" && GetValue("service").ToString().Trim() == "order_refund_notify")
            {
                //PiaoBao.BLLLogic.Order.Tb_Ticket_OrderManager OrderMan = PiaoBao.BLLLogic.Factory_Air.CreateITb_Ticket_OrderManager();
                //PiaoBao.Models.Tb_Ticket_Order Order = OrderMan.SelectOrderByOutOrderId(GetValue("order_no").ToString())[0];
                //OnErrorNew("今日退废票失败" + Order.OrderId, false);
                //Order.A40 = "4";
                //#region 记录日志
                //PiaoBao.Models.Log_Tb_AirOrder OrderLog = new PiaoBao.Models.Log_Tb_AirOrder();
                //PiaoBao.BLLLogic.Order.Log_Tb_AirOrderManager OrderLogManager = PiaoBao.BLLLogic.Factory_Air.CreateILog_Tb_AirOrderManager();
                //OrderLog.PNR = Order.PNR;
                //OrderLog.OrderId = Order.OrderId;
                //if (Order.OrderType == 3)
                //{
                //    OrderLog.OperateType = 14;
                //}
                //else if (Order.OrderType == 4)
                //{
                //    OrderLog.OperateType = 17;
                //}
                //OrderLog.OperateTime = DateTime.Now;
                //OrderLog.Content = "于 " + DateTime.Now + " 今日平台供应已拒绝退废票，请联系平台手动处理 拒绝原因：" + GetValue("remark").ToString();
                //OrderLog.OperateId = "adminys";
                //OrderLog.OperateName = "管理员";
                //OrderLog.OperateCorporationId = 1;
                //OrderLog.A1 = 1;
                //int Number = OrderLogManager.InsertLog_Tb_AirOrder(OrderLog);
                //#endregion
                //OrderMan.UpdateTb_Ticket_Order(Order);
            }
        }
    }

    public void Login(PbProject.Model.Tb_Ticket_Order order)
    {
        OnErrorNew("开始Login", false);
        string sql = "select LoginName,LoginPassWord from User_Employees where CpyNo='" + order.OwnerCpyNo.Substring(0, 12) + "' and IsAdmin=0";
        PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sq = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
        OnErrorNew("开始查询", false);
        DataTable tb = sq.ExecuteStrSQL(sql);
        OnErrorNew("结束查询", false);
        string msg = "";
        PbProject.Logic.Login LoginManage = new PbProject.Logic.Login();
        DataTable[] tableArr = null;
        bool IsSuc = LoginManage.GetByName(tb.Rows[0][0].ToString(), tb.Rows[0][1].ToString(), false, Page.Request.UserHostAddress, out tableArr, out msg, 1);
        OnErrorNew("结束Login", false);
    }
    /// <summary>
    /// 记录文本日志
    /// </summary>
    /// <param name="content">记录内容</param>
    /// <param name="IsPostBack">是否记录 Request 参数</param>
    private void OnErrorNew(string errContent, bool IsRecordRequest)
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