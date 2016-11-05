using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using PbProject.Logic;
using PbProject.Logic.ControlBase;

public partial class Pay_PTReturnPage_AutoPayByAlipayNotifyUrl : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //string result = "<?xml version=\"1.0\" encoding=\"gb2312\" ?><orderinfo><pnr>HF5N1R</pnr><code>1</code><message /><orderno></orderno><orderstatus>1</orderstatus><paystatus>1</paystatus><pnrsrcid>0101912051714341204</pnrsrcid ><payprice>543.6</ payprice><tradeno>2012051773929436</tradeno><tickets><ticket><passenger>张平</passenger><tktno>876-2037972475</tktno></ticket></tickets></orderinfo>";
        //System.Threading.Thread.Sleep(50000);
        OnErrorNew("进入 Pay_AutoPayByAlipayNotifyUrl_Load（）", true);

        try
        {
            if (Request.Form["ticketnoinfo"] != null && Request.Form["ticketnoinfo"].ToString() != "")
            {
                string ticketnoinfo = Request.Form["ticketnoinfo"].ToString();
                ticketnoinfo = HttpUtility.UrlDecode(ticketnoinfo, Encoding.GetEncoding("gb2312"));

                OnErrorNew("ticketnoinfo内容：" + ticketnoinfo, false);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ticketnoinfo);
                DataSet ds = new DataSet();
                StringReader rea = new StringReader(doc.InnerXml);

                XmlTextReader xmlReader = new XmlTextReader(rea);

                ds.ReadXml(xmlReader);
                PbProject.Model.Tb_Ticket_Order Order = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetTicketOrderByOrderId(ds.Tables[0].Rows[0]["pnrsrcid"].ToString());
                //Login(Order);
                if (ticketnoinfo.IndexOf("支付成功后，取票号失败，请您手工操作") > -1)
                {
                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "于 " + DateTime.Now + " 航空公司已出票，自动取票号失败，请您手工出票！";
                    OrderLog.WatchType = 2;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    #endregion
                    OnErrorNew("支付成功后，取票号失败，请您手工操作！", false);
                    return;
                }
                if (Order.OrderStatusCode == 4)
                {
                    OnErrorNew("该票号已经出票", false);
                    return;
                }

                int tcount = 0;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    PbProject.Logic.Order.Tb_Ticket_PassengerBLL psb = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL();
                    List<PbProject.Model.Tb_Ticket_Passenger> psmList = psb.GetPasListByOrderID(ds.Tables[0].Rows[0]["pnrsrcid"].ToString());
                    OnErrorNew("开始修改票号", false);
                    for (int i = 0; i < psmList.Count; i++)
                    {
                        for (int j = 0; j < ds.Tables[2].Rows.Count; j++)
                        {
                            if (ds.Tables[2].Rows[j]["tktno"].ToString() != "")
                            {
                                if (psmList[i].PassengerName.ToUpper() == ds.Tables[2].Rows[j]["passenger"].ToString().ToUpper() || psmList[i].PassengerName.ToUpper() == HttpUtility.UrlDecode(ds.Tables[2].Rows[j]["passenger"].ToString().ToUpper(), Encoding.Default) || psmList[i].PassengerName.ToUpper() == HttpUtility.UrlDecode(ds.Tables[2].Rows[j]["passenger"].ToString().ToUpper()))
                                {
                                    psmList[i].TicketStatus = 2;
                                    psmList[i].TicketNumber = ds.Tables[2].Rows[j]["tktno"].ToString().Trim();
                                    tcount++;
                                }
                            }
                        }
                    }

                    if (tcount != psmList.Count)
                    {
                        #region 记录操作日志
                        //添加操作订单的内容
                        PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
                        PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                        OrderLog.id = Guid.NewGuid();
                        OrderLog.OrderId = Order.OrderId;
                        OrderLog.OperType = "修改";
                        OrderLog.OperTime = DateTime.Now;
                        OrderLog.OperContent = "自动回填票号失败:乘机人与票号不符，需要手动操作!";
                        OrderLog.WatchType = 2;
                        string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                        sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                        #endregion
                    }

                    decimal payprice = 0;

                    try
                    {
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0
                            && !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["payprice"].ToString())
                            && !ds.Tables[0].Rows[0]["payprice"].ToString().Contains("-"))
                        {
                            payprice = payprice = Convert.ToDecimal(ds.Tables[0].Rows[0]["payprice"].ToString());
                        }
                    }
                    catch (Exception)
                    {

                    }

                    if (payprice != 0)
                    {
                        Order.AirPoint = 1 - (payprice - Order.ABFee - Order.FuelFee) / Order.PMFee;
                        Order.AirPoint = Math.Round(Order.AirPoint, 3);
                    }
                    //是否更改订单状态
                    //默认更改状态
                    bool IsUpdateOrderStatus = true;
                    try
                    {
                        foreach (PbProject.Model.Tb_Ticket_Passenger item in psmList)
                        {
                            //婴儿没有回帖票号 不修改状态
                            if (item.PassengerType == 3 && string.IsNullOrEmpty(item.TicketNumber.Trim()))
                            {
                                IsUpdateOrderStatus = false;
                                break;
                            }
                        }
                        if ((IsUpdateOrderStatus) && (tcount == psmList.Count))
                        {
                            Order.OrderStatusCode = 4;
                        }
                    }
                    catch (Exception)
                    {
                    }
                    List<PbProject.Model.User_Company> mCompany = new PbProject.Logic.ControlBase.BaseDataManage().
                CallMethod("User_Company", "GetList", null, new Object[] { "UninCode='" + Order.CPCpyNo + "'" }) as List<PbProject.Model.User_Company>;

                    List<PbProject.Model.User_Employees> mUser = new PbProject.Logic.ControlBase.BaseDataManage().
      CallMethod("User_Employees", "GetList", null, new Object[] { " IsAdmin=0 and CpyNo='" + Order.CPCpyNo + "'" }) as List<PbProject.Model.User_Employees>;

                    bool reuslt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().OperOrderCP(Order, psmList, mUser[0], mCompany[0], (IsUpdateOrderStatus ? "B2B自动出票" : "B2B自动出票【婴儿票请手动处理】"));
                    if (reuslt)
                    {
                        //零时改回状态
                        BaseDataManage manage = new BaseDataManage();
                        manage.ExecuteNonQuerySQLInfo("update Tb_Ticket_Order set OrderStatusCode=" + Order.OrderStatusCode + " where OrderId='" + Order.OrderId + "'");

                        #region  票宝开放服务接口异步通知出票

                        if (Order.OrderSourceType == 5)
                        {
                            PbProject.Logic.PTInterface.PbInterfaceNotify pbInterfaceCmd = new PbProject.Logic.PTInterface.PbInterfaceNotify();
                            if (pbInterfaceCmd != null)
                            {
                                bool pbNotifyResult = pbInterfaceCmd.NotifyTicketNo(Order);
                            }
                        }
                        #endregion
                        OnErrorNew(string.Format("出票成功.订单号:{0},PNR:{1}", Order.OrderId, Order.PNR), false);
                    }
                    else
                    {
                        OnErrorNew(string.Format("出票失败.订单号:{0},PNR:{1}", Order.OrderId, Order.PNR), false);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            #region 记录数据日志
            try
            {
                OnErrorNew("报错：" + ex.Message, false);
            }
            catch { }
            #endregion
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