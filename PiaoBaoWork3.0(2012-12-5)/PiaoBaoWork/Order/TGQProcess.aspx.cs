using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using PbProject.Model;

/// <summary>
/// 退改签审核处理
/// </summary>
public partial class Order_TGQProcess : BasePage
{
    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            try
            {
                if (Request.QueryString["Id"] != null && Request.QueryString["Url"] != null)
                {
                    //btnCancel.PostBackUrl = Request.QueryString["Url"].ToString();//返回
                    string url = Request.QueryString["Url"].ToString();
                    if (!url.Contains("IsQuery"))
                    {
                        ViewState["Url"] = Request.QueryString["Url"].ToString() + "?currentuserid=" + this.mUser.id.ToString();
                        ViewState["Id"] = Request.QueryString["Id"].ToString();
                    }
                    else
                    {
                        url = Server.UrlDecode(url);
                        ViewState["Url"] = url;
                        ViewState["Id"] = Request.QueryString["Id"].ToString();
                    }
                    PageDataBind();
                }
                else
                {
                    Response.Redirect(Request.RawUrl);
                }
            }
            catch
            {
                Response.Redirect(Request.RawUrl);
            }
        }
    }

    /// <summary>
    /// 页面信息绑定
    /// </summary>
    private void PageDataBind()
    {
        try
        {
            //btnTK.Enabled = false;
            string sqlWhere = " id='" + ViewState["Id"].ToString() + "' ";

            List<Tb_Ticket_Order> OrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Order>;
            Tb_Ticket_Order mOrder = OrderList != null && OrderList.Count > 0 ? OrderList[0] : null;

            if (mOrder != null)
            {
                sqlWhere = " OrderId='" + mOrder.OrderId + "' ";
               // if(mOrder.OrderStatusCode)

                /*
                20	取消出票，退款中
                21	退票成功，退款中
                22	废票成功，退款中
                23	拒绝改签，退款中
                */
                if ("|20|21|22|23|".Contains("|" + mOrder.OrderStatusCode + "|"))
                {
                    btnTK.Enabled = false;
                    Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialogOne('该订单正在退款中。。。','" + ViewState["Url"].ToString() + "');", true);
                    return;
                }




                #region 订单信息

                //订单信息
                lblInPayNo.Text = mOrder.InPayNo;
                lblLockId.Text = mOrder.LockLoginName;
                lblLockTime.Text = mOrder.LockTime.ToString("yyyy-MM-dd HH:mm:ss");
                lblOrderId.Text = mOrder.OrderId;

                //lblOrderSourceType.Text = mOrder.OrderSourceType.ToString();
                //lblOrderStatusCode.Text = mOrder.OrderStatusCode.ToString();
                lblOrderSourceType.Text = GetDictionaryName("33", mOrder.OrderSourceType.ToString());
                lblOrderStatusCode.Text = GetDictionaryName("1", mOrder.OrderStatusCode.ToString());
                //lblPayMoney.Text = mOrder.PayMoney.ToString("F2");
                //订单状态数据
                Hid_OrderStatus.Value = mOrder.OrderStatusCode.ToString();

                string strPayMoney = mOrder.OrderMoney.ToString("F2");
                if (mOrder.ToString() == "3" || mOrder.ToString() == "4")
                {
                    strPayMoney = "-" + strPayMoney;
                }
                lblPayMoney.Text = strPayMoney;

                lblPayNo.Text = mOrder.PayNo;
                lblPayStatus.Text = (mOrder.PayStatus == 1) ? "已付" : "未付";
                lblPayWay.Text = GetDictionaryName("4", mOrder.PayWay.ToString());
                lblPNR.Text = mOrder.PNR;


                // 换编码
                if (!string.IsNullOrEmpty(mOrder.ChangePNR))
                    lblShowPNR.Text += "换编码:<span style='color:red;'>" + mOrder.ChangePNR + "</span>";

                if (!string.IsNullOrEmpty(mOrder.BigCode))
                    //大编码
                    lblShowPNR.Text += "大编码:<span style='color:red;'>" + mOrder.BigCode + "</span>";

                if (lblShowPNR.Text != "")
                {
                    lblShowPNR.Visible = true;
                    lblShowPNR.Text = "<br/>" + lblShowPNR.Text;
                }

                string showPolicyPoint = "";
                if (mOrder.ReturnMoney != 0)
                {
                    showPolicyPoint += "原政策:" + mOrder.OldPolicyPoint.ToString("F1") + " 现返:" + mOrder.OldReturnMoney;
                    showPolicyPoint += "<br />出票政策:" + mOrder.PolicyPoint.ToString("F1") + " 现返:" + mOrder.ReturnMoney;
                }
                else
                {
                    showPolicyPoint += "原政策:" + mOrder.OldPolicyPoint.ToString("F1");
                    showPolicyPoint += "<br />出票政策:" + mOrder.PolicyPoint.ToString("F1");
                }

                lblPolicyPoint.Text = showPolicyPoint;


                lblPolicyRemark.Text = mOrder.PolicyRemark;
                lblPolicySource.Text = GetDictionaryName("24", mOrder.PolicySource.ToString());


                // mOrder.TGQApplyReason  退改签申请理由
                // mOrder.TGQRefusalReason  退改签拒绝理由
                // mOrder.YDRemark （订票备注）预订时备注信息
                // mOrder.CPRemark （出票备注）出票时备注信息

                // 显示 预订备注
                txtYDRemark.Text = mOrder.YDRemark;
                //退废改  申请理由
                txtTGQApplyReason.Text = mOrder.TGQApplyReason;
                // 拒绝理由
                txtTGQRefusalReason.Text = mOrder.TGQRefusalReason;


                if (mOrder.A4.ToString("yyyy-MM-dd") == "1900-01-01")
                {
                    txtA4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    txtA4.Text = mOrder.A4.ToString("yyyy-MM-dd HH:mm:ss");
                }

                txtA6.Text = mOrder.A6.ToString();

                #endregion

                #region 乘机人信息

                List<Tb_Ticket_Passenger> PassengerList = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Passenger>;

                if (PassengerList != null && PassengerList.Count > 0)
                {
                    RepPassenger.DataSource = PassengerList;
                    RepPassenger.DataBind();
                }

                #endregion

                #region 行程信息

                List<Tb_Ticket_SkyWay> SkyWayList = baseDataManage.CallMethod("Tb_Ticket_SkyWay", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_SkyWay>;

                if (SkyWayList != null && SkyWayList.Count > 0)
                {
                    RepSkyWay.DataSource = SkyWayList;
                    RepSkyWay.DataBind();
                }

                //改签的，显示原航程信息
                if (mOrder.TicketStatus == 5)
                {
                    trRepSkyWayOld.Visible = true;
                    string tempSqlWhere = "OrderId='" + mOrder.OldOrderId + "'";
                    List<Tb_Ticket_SkyWay> SkyWayListOld = baseDataManage.CallMethod("Tb_Ticket_SkyWay", "GetList", null, new Object[] { tempSqlWhere }) as List<Tb_Ticket_SkyWay>;

                    if (SkyWayListOld != null && SkyWayListOld.Count > 0)
                    {
                        RepSkyWayOld.DataSource = SkyWayListOld;
                        RepSkyWayOld.DataBind();
                    }
                }
                else
                {
                    trRepSkyWayOld.Visible = false;
                }

                #endregion

                #region 日志信息

                string sqlAirOrderWhere = " OrderId='" + mOrder.OrderId + "'";
                if (mCompany.RoleType == 1)
                    sqlAirOrderWhere += " and WatchType in(0,1,2,3,4,5)";
                else if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
                    sqlAirOrderWhere += " and WatchType in(2,3,4,5)";
                else if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
                    sqlAirOrderWhere += " and WatchType in(4,5)";
                sqlAirOrderWhere += " order by OperTime ";

                List<Log_Tb_AirOrder> AirOrderList = baseDataManage.CallMethod("Log_Tb_AirOrder", "GetList", null, new Object[] { sqlAirOrderWhere }) as List<Log_Tb_AirOrder>;


                if (AirOrderList != null && AirOrderList.Count > 0)
                {
                    RepOrderLog.DataSource = AirOrderList;
                    RepOrderLog.DataBind();
                }

                #endregion

                ViewState["Order"] = mOrder;
                ViewState["PassengerList"] = PassengerList;
                ViewState["SkyWayList"] = SkyWayList;

                //7	申请退票，等待审核
                //8	申请废票，等待审核
                //9	改签审核成功,等待补差
                //10	审核失败，拒绝改签
                //11	审核成功，等待退票
                //12	审核失败，拒绝退票
                //13	审核成功，等待废票
                //14	审核失败，拒绝废票
                //15	补差成功，等待确认
                //16	退票成功，交易结束
                //17	废票成功，交易结束
                //18	拒绝补差，改签失败
                //19	改签成功，交易结束

                span2.Visible = true; // 2.拒绝审核

                // span1 1. 通过审核不退款
                // span2 2.拒绝审核
                // span3 3.审核通过并退款
                // span4 4.退款

                hid_TicketStatus.Value = mOrder.TicketStatus.ToString();

                if (mOrder.TicketStatus == 3) //3.退票
                {
                    trTF.Style["display"] = "block";

                    #region 退票手续费

                    if (mOrder.OrderStatusCode == 7)
                    {
                        //7	申请退票，等待审核
                        lblShow.Text = "退票审核";
                        span1.Visible = true; // 1. 通过审核不退款

                        td4.Visible = true;
                        td2.Visible = true;
                        td3.Visible = true;

                        span0.Visible = true;
                    }
                    else if (mOrder.OrderStatusCode == 29)
                    {
                        //7	申请退票，等待审核
                        lblShow.Text = "退票审核";
                        span1.Visible = true; // 1. 通过审核不退款

                        td4.Visible = true;
                        td2.Visible = true;
                        td3.Visible = true;
                    }
                    else if (mOrder.OrderStatusCode == 11)
                    {
                        //11 审核成功，等待退票
                        lblShow.Text = "退款处理";
                        btnNoOk.Text = "拒绝退票";
                        td4.Visible = true;
                        td2.Visible = true;
                        td3.Visible = true;

                        span4.Visible = true;
                    }
                    #endregion
                }
                else if (mOrder.TicketStatus == 4) //废票
                {
                    trTF.Style["display"] = "block";

                    #region 废票
                    if (mOrder.OrderStatusCode == 8)
                    {
                        //8	申请废票，等待审核
                        lblShow.Text = "废票审核";
                        span1.Visible = true; //1. 通过审核不退款

                        td4.Visible = true;
                        td2.Visible = true;
                        td3.Visible = true;

                        span0.Visible = true;
                    }
                    else if (mOrder.OrderStatusCode == 30)
                    {
                        //8	申请废票，等待审核
                        lblShow.Text = "废票审核";
                        span1.Visible = true; //1. 通过审核不退款

                        td4.Visible = true;
                        td2.Visible = true;
                        td3.Visible = true;
                    }
                    else if (mOrder.OrderStatusCode == 13)
                    {
                        //13	审核成功，等待废票
                        lblShow.Text = "退款处理";
                        btnNoOk.Text = "拒绝废票";

                        td4.Visible = true;
                        td2.Visible = true;
                        td3.Visible = true;

                        span4.Visible = true;
                    }

                    #endregion
                }
                else if (mOrder.TicketStatus == 5) //5.改签
                {
                    trGQ.Style["display"] = "block";
                    // 隐藏


                    if (mOrder.OrderStatusCode == 31)
                    {
                        span1.Visible = true;
                        btnOktoSH.Text = "通过审核";

                        labHCName.Text = "新行程信息";
                        lblShow.Text = "改签审核";
                        btnNoOk.Text = "拒绝改签";

                        span0.Visible = true;
                    }
                    else
                    {
                        span1.Visible = true;
                        btnOktoSH.Text = "通过审核";

                        labHCName.Text = "新行程信息";
                        lblShow.Text = "改签审核";
                        btnNoOk.Text = "拒绝改签";
                    }
                }

                if (KongZhiXiTong != null && KongZhiXiTong.Contains("|47|"))
                {
                    //分开
                    span3.Visible = false;
                }
                else
                {
                    if (mOrder.OrderStatusCode == 7 || mOrder.OrderStatusCode == 8)
                    {
                        span3.Visible = true;//不分开
                    }
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 页面信息判断并返回对应值
    /// </summary>
    /// <param name="ParentId">父类编号</param>
    /// <param name="ChildId">子类编号</param>
    /// <returns>返回判断后所对应的值</returns>
    public string DataSourceMessage(int ParentId, string ChildId)
    {
        string Message = "";
        try
        {
            Message = GetDictionaryName(ParentId.ToString(), ChildId);
            Message = string.Join("<br />", Message.Split(new string[] { ",", "，", " " }, StringSplitOptions.RemoveEmptyEntries));
        }
        catch
        {

        }
        return Message;
    }

    /// <summary>
    /// 显示客规
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public string DataSourceMessageKeGui(string str)
    {
        if (str == null || str == "")
        {
            return "暂无客规!!!";
        }
        //【退票规定】 收取30％退票费  【变更规定】 每次改期收取票面价20％的改期费  
        return str;
    }

    #region 按钮事件

    /// <summary>
    /// 审核中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSH_Click(object sender, EventArgs e)
    {
        //ProcessNew(0);

        Process(0);
    }

    /// <summary>
    /// 1.审核通过，不退款
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOktoSH_Click(object sender, EventArgs e)
    {
        Process(1);
    }

    /// <summary>
    /// 2.拒绝审核
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnNoOk_Click(object sender, EventArgs e)
    {
        Process(2);
    }

    /// <summary>
    /// 3.审核通过并退款
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        Process(3);
    }

    /// <summary>
    /// 4.退款
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnTK_Click(object sender, EventArgs e)
    {
        Process(4);
    }

    #endregion

    /// <summary>
    /// 处理退废改
    /// </summary>
    /// <param name="OperationType">0.审核中 1 审核通过不退款 、2 拒绝处理、3 审核通过并退款 、4 只退款 </param>
    /// <param name="msg"></param>
    /// <returns></returns>
    private void Process(int OperationType)
    {
        bool result = false;
        string msg = "";

        try
        {
            string sqlWhere = " id='" + ViewState["Id"].ToString() + "' ";
            List<Tb_Ticket_Order> OrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Order>;
            Tb_Ticket_Order mOrder = OrderList != null && OrderList.Count > 0 ? OrderList[0] : null;
            if (mOrder != null)
            {
                sqlWhere = " OrderId='" + mOrder.OrderId + "' ";
                /*
                20	取消出票，退款中
                21	退票成功，退款中
                22	废票成功，退款中
                23	拒绝改签，退款中
                */
                if ("|20|21|22|23|".Contains("|" + mOrder.OrderStatusCode + "|"))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialogOne('该订单正在退款中。。。','" + ViewState["Url"].ToString() + "');", true);
                    return;
                }

                //Tb_Ticket_Order mOrder = ViewState["Order"] as Tb_Ticket_Order;
                List<Tb_Ticket_Passenger> PassengerList = ViewState["PassengerList"] as List<Tb_Ticket_Passenger>;
                PbProject.Logic.Order.Tb_Ticket_OrderBLL orderBll = new PbProject.Logic.Order.Tb_Ticket_OrderBLL();

                #region 处理订单状态

                int OrderType = mOrder.TicketStatus; //3.退 4.废 5.改

                if (OperationType == 0)
                {
                    if (mOrder.OrderStatusCode == 6 || mOrder.OrderStatusCode == 7 || mOrder.OrderStatusCode == 8)
                    {
                        //修改订单状态为  审核中
                        if (OrderType == 3)
                            mOrder.OrderStatusCode = 29;
                        else if (OrderType == 4)
                            mOrder.OrderStatusCode = 30;
                        else if (OrderType == 5)
                            mOrder.OrderStatusCode = 31;
                    }
                }
                else  if (OperationType == 1) //审核通过不退款
                {
                    //修改订单状态为  审核通过
                    if (OrderType == 3)
                        mOrder.OrderStatusCode = 11;
                    else if (OrderType == 4)
                        mOrder.OrderStatusCode = 13;
                    else if (OrderType == 5)
                    {
                        decimal gqPrice = decimal.Parse(txtGQPrice.Text.Trim());

                        if (gqPrice == 0)
                        {
                            // 直接确认改签
                            mOrder.OrderStatusCode = 19;
                            mOrder.PayMoney = 0;
                            mOrder.OrderMoney = 0;
                        }
                        else
                        {
                            // 改签等待补差
                            mOrder.OrderStatusCode = 9;
                            mOrder.PayMoney = gqPrice;
                            mOrder.OrderMoney = gqPrice;
                            mOrder.TGQHandlingFee = gqPrice;
                        }
                    }
                }
                else if (OperationType == 2)// 拒绝处理
                {
                    if (OrderType == 3)
                        mOrder.OrderStatusCode = 12;
                    else if (OrderType == 4)
                        mOrder.OrderStatusCode = 14;
                    else if (OrderType == 5)
                        mOrder.OrderStatusCode = 10;

                    mOrder.TGQRefusalReason = txtTGQRefusalReason.Text; //拒绝理由
                }
                else if (OperationType == 3 || OperationType == 4) // 审核通过并退款    // 只退款
                {
                    //修改订单状态为 退款中， 退款中 只能平台处理

                    if (OrderType == 3)
                        mOrder.OrderStatusCode = 21;
                    else if (OrderType == 4)
                        mOrder.OrderStatusCode = 22;
                    //else if (OrderType == 5)
                    //    mOrder.OrderStatusCode = 10;
                }

                #endregion

                #region 线下收银 处理

                //线下收银 处理
                if (mOrder.PayWay == 15 && (mOrder.OrderStatusCode == 11 || mOrder.OrderStatusCode == 13 || mOrder.OrderStatusCode == 21 || mOrder.OrderStatusCode == 22))
                {
                    if (mOrder.TicketStatus == 3)
                        mOrder.OrderStatusCode = 16;
                    else if (mOrder.TicketStatus == 4)
                        mOrder.OrderStatusCode = 17;
                }

                #endregion

                mOrder.A4 = DateTime.Parse(txtA4.Text.Trim());

                mOrder.A6 = decimal.Parse(txtA6.Text.Trim());

                //计算每个乘机人的手续费
                List<Tb_Ticket_Passenger> pasList = GetPassengerList(PassengerList, mOrder);

                string content = "";
                #region 记录手续费日志

                // 11 13　16　17 21 22 29 30

                if (mOrder.OrderStatusCode == 11 || mOrder.OrderStatusCode == 13 ||
                    mOrder.OrderStatusCode == 16 || mOrder.OrderStatusCode == 17 ||
                     mOrder.OrderStatusCode == 21 || mOrder.OrderStatusCode == 22 ||
                    mOrder.OrderStatusCode == 29 || mOrder.OrderStatusCode == 30)
                {
                    string temp = "";
                    bool re = false;

                    foreach (Tb_Ticket_Passenger item in pasList)
                    {
                        if (item.TGQHandlingFee > 0)
                            re = true;
                        temp += item.TGQHandlingFee + "/";
                    }
                    temp = temp.TrimEnd('/');

                    if (re)
                    {
                        //有手续费
                        //记录日志

                        content = "手续费:" + temp;
                    }
                }

                #endregion

                //退、废、改 处理
                result = orderBll.OperOrderTFG(mOrder, pasList, mUser, mCompany, content);

                string errtitle = "";

                if (result == true)
                {
                    if (OperationType == 3 || OperationType == 4) //退款
                    {
                        result = new PbProject.Logic.Pay.OperOnline().TitckOrderRefund(mOrder, mUser, mCompany, out errtitle);
                    }
                }
                else
                {
                    msg = "处理失败";
                }

                #region

                if (OperationType == 0)
                    msg = result ? "操作成功审核中!" : "操作失败!";
                else if (OperationType == 1)
                    msg = result ? "审核通过成功!" : "审核通过失败!";
                else if (OperationType == 2)
                    msg = result ? "拒绝审核成功!" : "拒绝审核失败!";
                else if (OperationType == 3 || OperationType == 4)
                {
                    msg = mOrder.TicketStatus == 5 ? "处理成功！" : "处理成功！系统退款中...";
                }

                #endregion

            }
        }
        catch (Exception ex)
        {
            msg = "处理出错";
        }

        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialogOne('" + msg + "','OrderTGQList.aspx?currentuserid=" + this.currentuserid.Value.ToString() + "');", true);
    }

    ///// <summary>
    ///// 审核中
    ///// </summary>
    ///// <param name="OperationType">0 审核中 </param>
    ///// <param name="msg"></param>
    ///// <returns></returns>
    //private void ProcessNew(int OperationType)
    //{
    //    bool result = false;
    //    string msg = "";

    //    try
    //    {
    //        Tb_Ticket_Order mOrder = ViewState["Order"] as Tb_Ticket_Order;
    //        List<Tb_Ticket_Passenger> PassengerList = ViewState["PassengerList"] as List<Tb_Ticket_Passenger>;
    //        PbProject.Logic.Order.Tb_Ticket_OrderBLL orderBll = new PbProject.Logic.Order.Tb_Ticket_OrderBLL();

    //        #region 处理订单状态

    //        int OrderType = mOrder.TicketStatus; //3.退 4.废 5.改

    //        if (OperationType == 0 && (mOrder.OrderStatusCode == 6 || mOrder.OrderStatusCode == 7 || mOrder.OrderStatusCode == 8)) //审核中
    //        {
    //            //修改订单状态为  审核中
    //            if (OrderType == 3)
    //                mOrder.OrderStatusCode = 29;
    //            else if (OrderType == 4)
    //                mOrder.OrderStatusCode = 30;
    //            else if (OrderType == 5)
    //                mOrder.OrderStatusCode = 31;

    //            string content = "";
    //            #region 记录手续费日志

    //            // 11 13　16　17 21 22 29 30

    //            if (mOrder.OrderStatusCode == 29 || mOrder.OrderStatusCode == 30)
    //            {
    //                string temp = "";
    //                bool re = false;

    //                mOrder.A4 = DateTime.Parse(txtA4.Text.Trim());

    //                mOrder.A6 = decimal.Parse(txtA6.Text.Trim());

    //                //计算每个乘机人的手续费
    //                List<Tb_Ticket_Passenger> pasList = GetPassengerList(PassengerList, mOrder);

    //                foreach (Tb_Ticket_Passenger item in pasList)
    //                {
    //                    if (item.TGQHandlingFee > 0)
    //                        re = true;
    //                    temp += item.TGQHandlingFee + "/";
    //                }
    //                temp = temp.TrimEnd('/');

    //                if (re)
    //                {
    //                    //有手续费
    //                    //记录日志

    //                    content = "手续费:" + temp;
    //                }
    //            }

    //            #endregion

    //            result = orderBll.OperOrderTFGSHZ(mOrder, mUser, mCompany, content);
    //        }

    //        #endregion

    //        msg = result == true ? "操作成功！" : "操作失败";

    //    }
    //    catch (Exception ex)
    //    {
    //        msg = "处理出错";
    //    }

    //    Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialogOne('" + msg + "','OrderTGQList.aspx?currentuserid=" + this.currentuserid.Value.ToString() + "');", true);
    //}

    /// <summary>
    /// 获取乘机人信息
    /// </summary>
    /// <returns></returns>
    public List<Tb_Ticket_Passenger> GetPassengerList(List<Tb_Ticket_Passenger> pasList, Tb_Ticket_Order Order)
    {
        if (Order != null && pasList != null && pasList.Count > 0)
        {
            //decimal TotalTGQHandlingFee = Order.TGQHandlingFee;

            decimal TotalTGQHandlingFee = 0;

            int Count = RepPassenger.Items.Count;
            for (int i = 0; i < Count; i++)
            {
                System.Web.UI.HtmlControls.HtmlInputText input = RepPassenger.Items[i].FindControl("txtTGQHandlingFee") as System.Web.UI.HtmlControls.HtmlInputText;
                System.Web.UI.HtmlControls.HtmlInputHidden Hid_Id = RepPassenger.Items[i].FindControl("Hid_Id") as System.Web.UI.HtmlControls.HtmlInputHidden;
                if (Hid_Id != null)
                {
                    string id = Hid_Id.Value;
                    Tb_Ticket_Passenger PasModel = pasList.Find(delegate(Tb_Ticket_Passenger _pas)
                     {
                         return _pas.id.ToString() == id;
                     });
                    if (PasModel != null)
                    {
                        if (input != null)
                        {
                            decimal TGQHandlingFee = 0;
                            if (decimal.TryParse(input.Value, out TGQHandlingFee))
                            {
                                PasModel.TGQHandlingFee = TGQHandlingFee;
                                TotalTGQHandlingFee += TGQHandlingFee;
                            }
                        }
                    }
                }
            }
            Order.TGQHandlingFee = TotalTGQHandlingFee;
        }
        return pasList;
    }

    /// <summary>
    /// 返回 解锁
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        try
        {
            Tb_Ticket_Order mOrder = ViewState["Order"] as Tb_Ticket_Order;

            new PbProject.Logic.Order.Tb_Ticket_OrderBLL().LockOrder(false, mOrder.id.ToString(), mUser, mCompany);

            if (ViewState["Url"] != null)
            {
                Response.Redirect(ViewState["Url"].ToString());
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 页面显示数据展示
    /// </summary>
    /// <param name="type"></param>
    /// <param name="objParam"></param>
    /// <returns></returns>
    public string ShowText(int type, params object[] objParam)
    {
        string result = "";
        if (type == 0)//输入手续费添加默认值
        {
            if (objParam != null && objParam.Length == 1)
            {
                string OrderStatus = Hid_OrderStatus.Value;//8.申请废票，等待审核
                string strTGQHandlingFee = (objParam[0] != null && objParam[0].ToString() != "") ? objParam[0].ToString() : "0";
                decimal m_TGQHandlingFee = 0;
                decimal.TryParse(strTGQHandlingFee, out m_TGQHandlingFee);
                if (OrderStatus == "8" && m_TGQHandlingFee == 0)
                {
                    m_TGQHandlingFee = 10;
                }
                result = m_TGQHandlingFee.ToString();
            }
        }
        return result;
    }
}