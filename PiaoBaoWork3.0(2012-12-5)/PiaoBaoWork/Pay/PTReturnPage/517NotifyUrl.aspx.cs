using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using PbProject.Logic;
using System.Data;


public partial class Pay_PTReturnPage_517NotifyUrl : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        OnErrorNew("进入 Pay_517NotifyUrl_Load（）", true);
        try
        {
            if (Request.Form["NotifyType"] != null)
            {
                GetData();
                Response.Write("SUCCESS");
            }
        }
        catch (Exception ex)
        {
            OnErrorNew(ex.Message.ToString(), false);
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
    private void GetData()
    {
        string ticketnoinfo = "";
        for (int i = 0; i < Request.Form.Count; i++)
        {
            ticketnoinfo = ticketnoinfo + Request.Form.Keys[i].ToString() + ":" + Request.Form[i].ToString() + "|";
        }
        OnErrorNew("ticketnoinfo内容：" + ticketnoinfo, false);
        string NotifyType = Request.Form["NotifyType"].ToString();//通知类型（出票通知；待出票通知）
        string OrderId = Request.Form["OrderId"].ToString();//订单Id
        string DrawABillFlag = Request.Form["DrawABillFlag"].ToString(); //出票状态(0表示出票，1表示取消出票)
        string DrawABillRemark = Request.Form["DrawABillRemark"].ToString(); //取消出票理由
        string TicketNos = Request.Form["TicketNos"].ToString(); //票号信息（结算码|票号|证件类型|证件号|乘机人姓名）
        string Sign = Request.Form["Sign"].ToString(); //Sign
        string Pnr = Request.Form["Pnr"].ToString(); //Pnr
        string NewPnr = "";
        if (Request.Form["NewPnr"] != null)
        {
            NewPnr = Request.Form["NewPnr"].ToString(); //NewPnr
        }

        //Kevin 2013-05-28 Edit
        //判断当前订单状态，如果不是 已经支付等待出票，则退出
        List<PbProject.Model.Tb_Ticket_Order> OrderUpdateList2 = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetListBySqlWhere(" OutOrderId='" + OrderId + "'");
        //Login(OrderUpdateList);
        if (OrderUpdateList2[0].OrderStatusCode != 3) //等待出票状态
        {
            OnErrorNew("外部订单号：" + OrderId+"对应订单状态不为等待出票状态，不做自动复核！", false);
            return;
        }


        PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("\r\n\r\n\r\n\r\n\r\n");
        sb.AppendFormat("517出票通知日志记录" + "\r\n");
        sb.AppendFormat("访问IP" + "：" + Page.Request.UserHostAddress + "\r\n");
        sb.AppendFormat("通知类型:" + NotifyType.ToString() + "\r\n");
        sb.AppendFormat("517订单编号:" + OrderId.ToString() + "\r\n");
        if (DrawABillFlag == "0")
        {
            sb.AppendFormat("出票状态:已经出票\r\n");
            string JSMA = "";
            string TicketNumber = "";
            string PidType = "";
            string Pid = "";
            string PassengerName = "";
            for (int i = 0; i < TicketNos.Split(',').Length; i++)
            {
                JSMA = JSMA + "," + TicketNos.Split(',')[i].Split('|')[0].ToString();
                JSMA = JSMA.TrimStart(',').TrimEnd(',');
                TicketNumber = TicketNumber + "," + TicketNos.Split(',')[i].Split('|')[0].ToString() + "-" + TicketNos.Split(',')[i].Split('|')[1].ToString();
                TicketNumber = TicketNumber.TrimStart(',').TrimEnd(',');
                PidType = PidType + "," + TicketNos.Split(',')[i].Split('|')[2].ToString();
                PidType = PidType.TrimStart(',').TrimEnd(',');
                Pid = Pid + "," + TicketNos.Split(',')[i].Split('|')[3].ToString();
                Pid = Pid.TrimStart(',').TrimEnd(',');
                PassengerName = PassengerName + "," + TicketNos.Split(',')[i].Split('|')[4].ToString();
                PassengerName = PassengerName.TrimStart(',').TrimEnd(',');
            }
            if (JSMA != "")
            {
                sb.AppendFormat("结算码:" + JSMA + "\r\n");
                sb.AppendFormat("票号:" + TicketNumber + "\r\n");
                sb.AppendFormat("证件类型:" + PidType + "\r\n");
                sb.AppendFormat("证件号:" + Pid + "\r\n");
                sb.AppendFormat("乘机人姓名:" + PassengerName + "\r\n");
            }
            else
            {
                sb.AppendFormat("票号信息:" + TicketNos.ToString() + "\r\n");
            }
            sb.AppendFormat("效验码:" + Sign + "\r\n");
            #region 更新数据库订单信息

            List<PbProject.Model.Tb_Ticket_Order> OrderUpdateList = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetListBySqlWhere(" OutOrderId='" + OrderId + "'");
            //Login(OrderUpdateList);
            
            if (NewPnr != null && NewPnr != "")
            {
                OrderUpdateList[0].ChangePNR = NewPnr;
            }

            int tcount = 0;
            PbProject.Logic.Order.Tb_Ticket_PassengerBLL PassengerManager = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL();
            List<PbProject.Model.Tb_Ticket_Passenger> PassengerList = PassengerManager.GetPasListByOrderID(OrderUpdateList[0].OrderId);
            for (int j = 0; j < PassengerList.Count; j++)
            {
                int number = -1;
                for (int k = 0; k < PassengerName.Split(',').Length; k++)
                {
                    if (PassengerName.Split(',')[k].ToString().Replace("CHD", "") == PassengerList[j].PassengerName.Replace("CHD", ""))
                    {
                        number = k;
                        break;
                    }
                }
                if (number != -1)
                {
                    PassengerList[j].TicketNumber = TicketNumber.Split(',')[number];
                    PassengerList[j].TicketStatus = 2;
                    tcount++;
                }
            }

            //如果乘机人和票号数量一致，则更改订单状态
            if (tcount == PassengerList.Count)
            {
                OrderUpdateList[0].OrderStatusCode = 4; //出票状态
            }
            else
            {
                #region 记录操作日志
                //添加操作订单的内容
                PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = OrderUpdateList[0].OrderId;
                OrderLog.OperType = "修改";
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperContent = "自动回填票号失败:乘机人与票号不符，需要手动操作!";
                OrderLog.WatchType = 2;
                string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                #endregion
            }

            List<PbProject.Model.User_Company> mCompany = new PbProject.Logic.ControlBase.BaseDataManage().
                CallMethod("User_Company", "GetList", null, new Object[] { "UninCode='" + OrderUpdateList[0].CPCpyNo + "'" }) as List<PbProject.Model.User_Company>;

            List<PbProject.Model.User_Employees> mUser = new PbProject.Logic.ControlBase.BaseDataManage().
     CallMethod("User_Employees", "GetList", null, new Object[] { " IsAdmin=0 and CpyNo='" + OrderUpdateList[0].CPCpyNo + "'" }) as List<PbProject.Model.User_Employees>;
            bool reuslt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().OperOrderCP(OrderUpdateList[0], PassengerList, mUser[0], mCompany[0],"");
            if (reuslt)
            {
                sb.AppendFormat("更新数据库，订单号：" + OrderUpdateList[0].OrderId + "信息，更新成功！\r\n");
                #region  票宝开放服务接口异步通知出票

                if (OrderUpdateList[0].OrderSourceType == 5)
                {
                    PbProject.Logic.PTInterface.PbInterfaceNotify pbInterfaceCmd = new PbProject.Logic.PTInterface.PbInterfaceNotify();
                    if (pbInterfaceCmd != null)
                    {
                        bool pbNotifyResult = pbInterfaceCmd.NotifyTicketNo(OrderUpdateList[0]);
                    }
                }
                #endregion
            }
            else
            {
                sb.AppendFormat("更新数据库，订单号：" + OrderUpdateList[0].OrderId + "信息，更新失败！\r\n");
            }
        }
        else
        {
            sb.AppendFormat("出票状态:取消出票\r\n");
            sb.AppendFormat("取消出票理由:" + DrawABillRemark.ToString() + "\r\n");
        }
            #endregion
        OnErrorNew("记录：" + sb.ToString(), false);
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