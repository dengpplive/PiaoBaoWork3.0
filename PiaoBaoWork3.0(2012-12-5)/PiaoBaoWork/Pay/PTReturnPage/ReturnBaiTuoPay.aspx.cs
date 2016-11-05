using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Logic;
using System.Xml;
using System.Data;
using System.IO;

public partial class Pay_PTReturnPage_ReturnBaiTuoPay : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        OnErrorNew("进入 ReturnBaiTuoPay.aspx_Load（）", false);

        string tmpstr = "";
        string[] sl = Request.QueryString.AllKeys;
        for (int i = 0; i < sl.Length; i++)
        {
            tmpstr += sl[i] + "=" + Request.QueryString[sl[i]].ToString() + "&";
        }

        OnErrorNew(tmpstr, false);


        PbProject.Model.Tb_Ticket_Order Order = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetTicketOrderByOrderId(Request.QueryString["portorderid"].ToString());
        PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
        List<PbProject.Model.Bd_Base_Parameters> mBP = new PbProject.Logic.ControlBase.BaseDataManage().
                 CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + Order.OwnerCpyNo.Substring(0, 12) + "'" }) as List<PbProject.Model.Bd_Base_Parameters>;
        BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);
        //Login(Order);
        PbProject.Logic.PTInterface.PTBybaituo OrderBaiTuoInterface = new PbProject.Logic.PTInterface.PTBybaituo(Order, BS);
        w_BTWebService.BaiTuoWeb BaiTuoWebService = new w_BTWebService.BaiTuoWeb();
        //BaiTuoWebService.BaiTuoWeb BaiTuoWebService = new BaiTuoWebService.BaiTuoWeb();
        //System.Threading.Thread.Sleep(50000);        
        
        if (Request.QueryString["forderformid"] != null && Request.QueryString["messageType"].ToString() == "2")
        {
            #region 出票
            #region 参数接收
            string forderformid = Request.QueryString["forderformid"].ToString();
            string produceType = Request.QueryString["produceType"].ToString();
            string messageType = Request.QueryString["messageType"].ToString();
            OnErrorNew("ticketnoinfo内容：" + "messageType:" + messageType + "/forderformid:" + forderformid + "/produceType:" + produceType, false);
            #endregion
            XmlElement xmlElement = OrderBaiTuoInterface.BaiTuoCpSend(forderformid);

            XmlNode xml = BaiTuoWebService.getOrderInfoXml(xmlElement);

            OnErrorNew("取订单XML内容：" + xml.InnerXml, false);
            DataSet ds = new DataSet();
            StringReader rea = new StringReader("<ORDER_INFO_RS>" + xml.InnerXml + "</ORDER_INFO_RS>");
            #region 测试数据
            //string sss = "";
            //sss += "<ORDERINFO OrderID=\"f1020\" IssuedDate=\"2008-12-24 10:07:31\" Status=\"4\" Flag=\"1\" Shouldpaid=\"8180.00\" Money=\"8000.00\">";
            //sss += "<TICKETINFO PNR=\"RE566\" personName=\"wangchun2\" DepartCity=\"PEK\" ArrivalCity=\"SHA\" InsuranceNumber=\"D000028127\" Price=\"450\" AgentPrice=\"440\" InsurancePolicyNO=\"\" EhomeBillno=\"\" >999-789456120</TICKETINFO>";
            //sss += "<TICKETINFO PNR=\"RE567\" personName=\"wangchun3\" DepartCity=\"PEK\" ArrivalCity=\"SHA\" InsuranceNumber=\"\" Price=\"450\" AgentPrice=\"440\" InsurancePolicyNO=\"\" EhomeBillno=\"\">999-789456121</TICKETINFO>";
            //sss += "<TICKETINFO PNR=\"RE568\" personName=\"wangchun4\" DepartCity=\"PEK\" ArrivalCity=\"SHA\" InsuranceNumber=\"D000028127\" Price=\"450\" AgentPrice=\"440\" InsurancePolicyNO=\"\" EhomeBillno=\"\">999-789456122</TICKETINFO>";
            //sss += "</ORDERINFO>";
            //sss += "<Error Code=\"611001\">出错原因</Error>";
            //StringReader rea = new StringReader("<ORDER_INFO_RS>" + sss + "</ORDER_INFO_RS>");
            #endregion
            XmlTextReader xmlReader = new XmlTextReader(rea);

            ds.ReadXml(xmlReader);
            if (ds.Tables.Count > 1)
            {
                OnErrorNew("订单状态：" + ds.Tables[0].Rows[0]["Status"].ToString(), false);
                if (ds.Tables[0].Rows[0]["Status"].ToString() == "4")
                {

                    if (Order != null)
                    {
                        if (Order.OrderStatusCode == 4)
                        {
                            OnErrorNew("该票号已经出票", false);
                            return;
                        }
                        if (Order.OrderStatusCode == 3)
                        {
                            int tcount = 0;
                            PbProject.Logic.Order.Tb_Ticket_PassengerBLL PassengerManager = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL();
                            List<PbProject.Model.Tb_Ticket_Passenger> PassengerList = PassengerManager.GetPasListByOrderID(Order.OrderId);
                            OnErrorNew("开始修改订单状态", false);
                            for (int i = 0; i < PassengerList.Count; i++)
                            {
                                for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
                                {
                                    if (PassengerList[i].PassengerName.Replace("CHD", "") == ds.Tables[1].Rows[j]["personName"].ToString().Replace("CHD", ""))
                                    {
                                        OnErrorNew(PassengerList[i].PassengerName + "：" + ds.Tables[1].Rows[j]["TICKETINFO_TEXT"].ToString(), false);
                                        PassengerList[i].TicketNumber = ds.Tables[1].Rows[j]["TICKETINFO_TEXT"].ToString();
                                        PassengerList[i].TicketStatus = 2;
                                        tcount++;
                                    }
                                }
                            }

                            if (tcount == PassengerList.Count)
                            {
                                Order.OrderStatusCode = 4; //出票状态
                            }
                            else
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

                            
                            bool reuslt = false;
                            List<PbProject.Model.User_Company> mCompany = new PbProject.Logic.ControlBase.BaseDataManage().
                 CallMethod("User_Company", "GetList", null, new Object[] { "UninCode='" + Order.CPCpyNo + "'" }) as List<PbProject.Model.User_Company>;

                            List<PbProject.Model.User_Employees> mUser = new PbProject.Logic.ControlBase.BaseDataManage().
      CallMethod("User_Employees", "GetList", null, new Object[] { " IsAdmin=0 and CpyNo='" + Order.CPCpyNo + "'" }) as List<PbProject.Model.User_Employees>;
                            reuslt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().OperOrderCP(Order, PassengerList, mUser[0], mCompany[0],"");
                            if (reuslt)
                            {
                                OnErrorNew("修改订单完成", false);
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
                            }
                            else
                            {
                                OnErrorNew("修改订单失败", false);
                            }
                        }
                    }
                }
            }
            #endregion
        }
        else if (Request.QueryString["orderID"] != null && Request.QueryString["messageType"].ToString() == "12")//拒绝退/废票的消息
        {
            //PiaoBao.BLLLogic.Order.Tb_Ticket_OrderManager OrderMan = PiaoBao.BLLLogic.Factory_Air.CreateITb_Ticket_OrderManager();
            //Tb_Ticket_Order Order = OrderMan.SelectOrderByOutOrderId(Request.QueryString["orderID"].ToString())[0];
            //OnErrorNew("百拓退废票失败" + Order.OrderId, false);
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
            //OrderLog.Content = "于 " + DateTime.Now + " 百拓平台供应已拒绝退废票，请联系平台手动处理 拒绝原因：" + Request.QueryString["memo"].ToString();
            //OrderLog.OperateId = "adminys";
            //OrderLog.OperateName = "管理员";
            //OrderLog.OperateCorporationId = 1;
            //OrderLog.A1 = 1;
            //int Number = OrderLogManager.InsertLog_Tb_AirOrder(OrderLog);
            //#endregion
            //OrderMan.UpdateTb_Ticket_Order(Order);
        }
        else if (Request.QueryString["orderID"] != null && (Request.QueryString["messageType"].ToString() == "13" || Request.QueryString["messageType"].ToString() == "14"))//退废票办理完成，等待供应商退款的消息
        {
            //PiaoBao.BLLLogic.Order.Tb_Ticket_OrderManager OrderMan = PiaoBao.BLLLogic.Factory_Air.CreateITb_Ticket_OrderManager();
            //Tb_Ticket_Order Order = OrderMan.SelectOrderByOutOrderId(Request.QueryString["orderID"].ToString())[0];
            //OnErrorNew("百拓退废票成功" + Order.OrderId, false);
            //Order.A40 = "3";
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
            //OrderLog.Content = "于 " + DateTime.Now + " 百拓平台供应已退票";
            //OrderLog.OperateId = "adminys";
            //OrderLog.OperateName = "管理员";
            //OrderLog.OperateCorporationId = 1;
            //OrderLog.A1 = 1;
            //int Number = OrderLogManager.InsertLog_Tb_AirOrder(OrderLog);
            //#endregion
            //OrderMan.UpdateTb_Ticket_Order(Order);
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