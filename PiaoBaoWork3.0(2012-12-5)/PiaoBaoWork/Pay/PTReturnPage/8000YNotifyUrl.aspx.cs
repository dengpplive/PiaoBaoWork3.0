using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Data;
using PbProject.Logic;

public partial class Pay_PTReturnPage_8000YNotifyUrl : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        OnErrorNew("进入 Pay_8000YNotifyUrl_Load（）", true);
        try
        {
            if (Request.Form["type"] != null)
            {
                GetData();
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
        StringBuilder sb = new StringBuilder();
        string platform = Request.Form["platform"].ToString();
        string type = Request.Form["type"].ToString();
        string orderguid = Request.Form["orderguid"].ToString();
        string orderstate = Request.Form["orderstate"].ToString();
        string notifymsg = Request.Form["notifymsg"].ToString();
        string key = Request.Form["key"].ToString();
        string localKey = string.Empty;     

        if (type == "2")
            localKey = string.Format("$%^{0}{1}|8000YI$8000yi$", orderguid, notifymsg);
        else
            localKey = string.Format("$%^{0}{1}8000YI$8000yi$", orderguid, notifymsg);
        localKey = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(localKey, "MD5");
        if (!key.Equals(localKey))
            OnErrorNew("key错误", false);
        else
        {
            string[] notifyMsgSort = notifymsg.Split('^');
            string PassengerName = string.Empty, TicketNumber = string.Empty;
            if (type == "2")
            {
                PassengerName = notifyMsgSort[1];
                TicketNumber = notifyMsgSort[2];

                #region 更新数据库订单信息
                List<PbProject.Model.Tb_Ticket_Order> OrderUpdateList = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetListBySqlWhere(" OutOrderId='" + orderguid + "'");

                //Kevin 2013-05-28 判断当前订单状态
                if (OrderUpdateList[0].OrderStatusCode != 3)
                {
                    OnErrorNew("外部订单号："+orderguid+"对应订单状态不是等待出票状态，不做自动复核！", false);
                    return;
                }

                //Login(OrderUpdateList);
                //OrderUpdateList[0].OrderStatusCode = 4; //出票状态

                int tcount = 0;
                PbProject.Logic.Order.Tb_Ticket_PassengerBLL PassengerManager = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL();
                List<PbProject.Model.Tb_Ticket_Passenger> PassengerList = PassengerManager.GetPasListByOrderID(OrderUpdateList[0].OrderId);
                for (int j = 0; j < PassengerList.Count; j++)
                {
                    int number = -1;
                    for (int k = 0; k < PassengerName.Split('|').Length; k++)
                    {
                        if (PassengerName.Split('|')[k].ToString().Replace("CHD", "") == PassengerList[j].PassengerName.Replace("CHD", ""))
                        {
                            number = k;
                            break;
                        }
                    }
                    if (number != -1)
                    {
                        tcount++;
                        PassengerList[j].TicketNumber = TicketNumber.Split('|')[number];
                        PassengerList[j].TicketStatus = 2;
                    }
                }

                if (tcount == PassengerList.Count)
                {
                    OrderUpdateList[0].OrderStatusCode = 4; //出票状态
                }
                else
                {
                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
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
                OnErrorNew("记录：" + sb.ToString(), false);
                #endregion
            }
            else if (type == "3" || type == "4")
            {
                //PiaoBao.BLLLogic.Order.Tb_Ticket_OrderManager OrderMan = PiaoBao.BLLLogic.Factory_Air.CreateITb_Ticket_OrderManager();
                //PiaoBao.Models.Tb_Ticket_Order Order = OrderMan.SelectOrderByOutOrderId(orderguid)[0];
                //if (Request.Form["orderstate"].ToString().Contains("已经退票") || Request.Form["orderstate"].ToString().Contains("已经废票"))
                //{
                //    OnErrorNew("8000yi平台退废票成功" + Order.OrderId, false);
                //    Order.A40 = "3";
                //    #region 记录日志
                //    PiaoBao.Models.Log_Tb_AirOrder OrderLog = new PiaoBao.Models.Log_Tb_AirOrder();
                //    PiaoBao.BLLLogic.Order.Log_Tb_AirOrderManager OrderLogManager = PiaoBao.BLLLogic.Factory_Air.CreateILog_Tb_AirOrderManager();
                //    OrderLog.PNR = Order.PNR;
                //    OrderLog.OrderId = Order.OrderId;
                //    if (Order.OrderType == 3)
                //    {
                //        OrderLog.OperateType = 14;
                //    }
                //    else if (Order.OrderType == 4)
                //    {
                //        OrderLog.OperateType = 17;
                //    }
                //    OrderLog.OperateTime = DateTime.Now;
                //    OrderLog.Content = "于 " + DateTime.Now + " 8000yi平台供应已退票";
                //    OrderLog.OperateId = "adminys";
                //    OrderLog.OperateName = "管理员";
                //    OrderLog.OperateCorporationId = 1;
                //    OrderLog.A1 = 1;
                //    int Number = OrderLogManager.InsertLog_Tb_AirOrder(OrderLog);
                //    #endregion
                //    OrderMan.UpdateTb_Ticket_Order(Order);
                //}
                //else
                //{
                //    OnErrorNew("8000yi平台退废票失败" + Order.OrderId, false);
                //    Order.A40 = "4";
                //    #region 记录日志
                //    PiaoBao.Models.Log_Tb_AirOrder OrderLog = new PiaoBao.Models.Log_Tb_AirOrder();
                //    PiaoBao.BLLLogic.Order.Log_Tb_AirOrderManager OrderLogManager = PiaoBao.BLLLogic.Factory_Air.CreateILog_Tb_AirOrderManager();
                //    OrderLog.PNR = Order.PNR;
                //    OrderLog.OrderId = Order.OrderId;
                //    if (Order.OrderType == 3)
                //    {
                //        OrderLog.OperateType = 14;
                //    }
                //    else if (Order.OrderType == 4)
                //    {
                //        OrderLog.OperateType = 17;
                //    }
                //    OrderLog.OperateTime = DateTime.Now;
                //    OrderLog.Content = "于 " + DateTime.Now + " 8000yi平台供应已拒绝退废票，请联系平台手动处理 ";
                //    OrderLog.OperateId = "adminys";
                //    OrderLog.OperateName = "管理员";
                //    OrderLog.OperateCorporationId = 1;
                //    OrderLog.A1 = 1;
                //    int Number = OrderLogManager.InsertLog_Tb_AirOrder(OrderLog);
                //    #endregion
                //    OrderMan.UpdateTb_Ticket_Order(Order);
                //}
            }
            OnErrorNew("记录：" + sb.ToString(), false);
        }
    }

    /// <summary>
    /// 判断是否可以处理
    /// </summary>
    /// <returns></returns>
    //private int IsProcess(IList<PiaoBao.Models.Tb_Ticket_Passenger> PassengerList)
    //{
    //    int Num = 0;
    //    for (int i = 0; i < PassengerList.Count; i++)
    //    {
    //        string IsBack = PassengerList[i].IsBack.ToString();
    //        if (IsBack == "2")
    //        {
    //            Num = 0;
    //            break;
    //        }
    //        else
    //        {
    //            Num = Num + 1;
    //        }
    //    }
    //    return Num;
    //}
    /// <summary>
    /// 获取退改签手续费,乘机人Id,IsBack可以不可以再退格式(20.00,134,0)
    /// </summary>
    /// <returns></returns>
    //private string[] GetTGQFee(IList<PiaoBao.Models.Tb_Ticket_Passenger> PassengerList, string SXFees)
    //{
    //    string[] AllSXFee = new string[PassengerList.Count];
    //    decimal OneSXFees = Convert.ToDecimal(SXFees) / PassengerList.Count;
    //    //循环机票信息
    //    for (int i = 0; i < PassengerList.Count; i++)
    //    {
    //        string Id = PassengerList[i].Id.ToString();

    //        AllSXFee[i] = OneSXFees + "," + Id + "," + PassengerList[i].IsBack;
    //    }
    //    return AllSXFee;
    //}
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