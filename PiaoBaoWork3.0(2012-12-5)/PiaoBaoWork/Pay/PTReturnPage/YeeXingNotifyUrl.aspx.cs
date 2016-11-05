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

public partial class Pay_PTReturnPage_YeeXingNotifyUrl : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        OnErrorNew("进入 Pay_PTReturnPage_YeeXingNotifyUrl（）", true);

        //OnErrorNew("提交方式："+HttpContext.Current.Request.HttpMethod, false); 

        try
        {
            if (HttpContext.Current.Request.Form.Get("airId")!=null)//Request.QueryString["AirId"] != null)
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
        foreach (string key in Request.QueryString.AllKeys)
        {
            if (key.Trim() == Key)
            {
                value = Request.QueryString[key].ToString().Trim();
                break;
            }
        }
        return value;
    }

    private void GetData()
    {
        string tmpstr = "";
        string[] sl = HttpContext.Current.Request.Form.AllKeys;//Request.QueryString.AllKeys;
        for (int i = 0; i < sl.Length; i++)
        {
            tmpstr += sl[i] + "=" + HttpContext.Current.Request.Form.Get(i) + "&";//HttpContext.Current.Request.Form.Keys[i] + "&";
        }

        OnErrorNew(HttpUtility.UrlDecode(tmpstr), false);

        //string ticketnoinfo = "";
        //for (int i = 0; i < Request.Form.Count; i++)
        //{
        //    ticketnoinfo = ticketnoinfo + Request.Form.Keys[i].ToString() + ":" + Request.Form[i].ToString() + "|";
        //}
        //ticketnoinfo = HttpUtility.UrlDecode(ticketnoinfo, Encoding.GetEncoding("gb2312"));
        //OnErrorNew("ticketnoinfo内容：" + ticketnoinfo, false);

        string NewPnr = "";

        string airId = HttpContext.Current.Request.Form.Get("airId");//Request.QueryString["airId"].ToString().Trim();
	    OnErrorNew("判断是否有票号信息，airId="+airId, false);

        if (HttpContext.Current.Request.Form.Get("airId").Trim() != "")
        {
            OnErrorNew("进入 Request.QueryString['status'].ToString().Trim() != ''", false);
            if (HttpContext.Current.Request.Form.Get("Type").Trim() == "1")//出票完成
            {
                bool reuslt = false;
                OnErrorNew("进入出票 Request.QueryString['status'].ToString().Trim() == 'T'", false);
                StringBuilder sb = new StringBuilder();
                List<PbProject.Model.Tb_Ticket_Order> OrderUpdateList = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetListBySqlWhere(" OutOrderId='" + HttpContext.Current.Request.Form.Get("orderid").Trim() + "'");

                if (OrderUpdateList.Count == 0)
                {
                    OnErrorNew("根据外部订单号未查找到对应订单，外部订单号：" + HttpContext.Current.Request.Form.Get("orderid").Trim(), false);
                }

                if (OrderUpdateList[0].OrderStatusCode != 3)
                {
                    OnErrorNew("订单状态不是待出票状态...", false);
                }

                //Login(OrderUpdateList);
                if (OrderUpdateList[0].OrderStatusCode == 3)
                {
                    //OrderUpdateList.OrderStatusCode = 4; //出票状态
                    if (HttpContext.Current.Request.Form.Get("NewPnr") != null && HttpContext.Current.Request.Form.Get("NewPnr") != "")
                    {
                        OrderUpdateList[0].ChangePNR = HttpContext.Current.Request.Form.Get("NewPnr");
                    }

                    int tcount = 0;
                    PbProject.Logic.Order.Tb_Ticket_PassengerBLL PassengerManager = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL();
                    List<PbProject.Model.Tb_Ticket_Passenger> PassengerList = PassengerManager.GetPasListByOrderID(OrderUpdateList[0].OrderId);
                    for (int j = 0; j < PassengerList.Count; j++)
                    {
                        string sss = HttpUtility.UrlDecode(HttpContext.Current.Request.Form.Get("passengerName"));

                        OnErrorNew("乘机人列表：" + sss, false);

                        string[] Name = sss.Split('^');
                        for (int i = 0; i < Name.Length; i++)
                        {
                            if (PassengerList[j].PassengerName.Trim().Replace("CHD", "").Trim().Replace(" ", "") == Name[i].Replace("CHD", "").Trim().Replace(" ", ""))
                            {
                                PassengerList[j].TicketNumber = HttpContext.Current.Request.Form.Get("airId").Split('^')[i].ToString();
                                tcount++;
                            }
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
                    reuslt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().OperOrderCP(OrderUpdateList[0], PassengerList, mUser[0], mCompany[0],"");
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
                    OnErrorNew("易行记录：" + sb.ToString(), false);
                }
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