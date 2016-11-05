using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using PbProject.Model;
using System.Linq;
using PbProject.WebCommon.Utility.Encoding;
using DataBase.Data;
using PbProject.Logic.ControlBase;
using PnrAnalysis.Model;
using PbProject.Logic.PID;
using PbProject.WebCommon.Utility;

/// <summary>
/// 订单详情
/// </summary>
public partial class Order_OrderDetail : BasePage
{
    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                this.currentuserid.Value = this.mUser.id.ToString();
                if ((Request.QueryString["id"] != null && Request.QueryString["Url"] != null) || (Request.QueryString["orderid"] != null && Request.QueryString["Url"] != null))
                {
                    //订单号
                    OrderBind(0);
                    string url = Request.QueryString["Url"].ToString();
                    if (!url.Contains("IsQuery"))
                        Hid_GoUrl.Value = url + "?currentuserid=" + Request.QueryString["currentuserid"].ToString();
                    else
                    {
                        url = Server.UrlDecode(url);
                        Hid_GoUrl.Value = url;
                    }
                    //获取可用行程单号
                    GetValidTrip();
                }
            }
        }
        catch (Exception ex)
        {

        }
        if (mCompany.RoleType < 3)
        {
            span_A9.Visible = true;
        }
        else
        {
            span_A9.Visible = false;
        }

        //行程单操作
        OPTrip();

        //修改证件号
        UpdateSsr();
    }
    private string ShowAdult(object AssociationOrder, object IsChdFlag)
    {
        string ass = AssociationOrder.ToString();
        if (IsChdFlag != null && !string.IsNullOrEmpty(ass))
        {
            bool temp = Convert.ToBoolean(IsChdFlag);
            //儿童订单
            if (temp)
                return string.Format("<br/>{0}(成人)", ass);
        }
        return string.Empty;
    }
    /// <summary>
    /// 绑定订单信息
    /// </summary>
    private void OrderBind(int Source)
    {
        try
        {
            string sqlWhere = "";
            Tb_Ticket_Order mOrder = null;
            if (Request.QueryString["id"] != null)
            {
                sqlWhere = Request.QueryString["id"].ToString().Replace("'", "");
                mOrder = baseDataManage.CallMethod("Tb_Ticket_Order", "GetById", null, new Object[] { sqlWhere }) as Tb_Ticket_Order;
            }
            if (Request.QueryString["orderid"] != null)
            {
                sqlWhere = Request.QueryString["orderid"].ToString().Replace("'", "");
                mOrder = (baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { "OrderId ='" + sqlWhere + "'" }) as List<Tb_Ticket_Order>)[0];
            }

            if (mOrder != null)
            {
                //用到的隐藏信息
                SetHidInfo(mOrder);


                sqlWhere = " OrderId='" + mOrder.OrderId + "' ";

                #region 订单信息
                ViewState["orderDetail"] = mOrder;

                //订单信息
                lblInPayNo.Text = mOrder.InPayNo;
                lblLockId.Text = mOrder.LockLoginName;
                lblLockTime.Text = mOrder.LockTime.ToString("yyyy-MM-dd") == "1900-01-01" ? "" : mOrder.LockTime.ToString("yyyy-MM-dd HH:mm:ss");
                lblOrderId.Text = mOrder.OrderId;
                lblOrderId.Text += ShowAdult(mOrder.AssociationOrder, mOrder.IsChdFlag);
                //lblOrderSourceType.Text = mOrder.OrderSourceType.ToString();
                //订单来源
                lblOrderSourceType.Text = GetDictionaryName("33", mOrder.OrderSourceType.ToString());
                //客规
                labKeGui.Text = string.IsNullOrEmpty(mOrder.KeGui) ? "暂无客规!!!" : mOrder.KeGui;
                labFGQ.Text = mOrder.PolicyCancelTime;

                //lblOrderStatusCode.Text = mOrder.OrderStatusCode.ToString();
                lblOrderStatusCode.Text = GetDictionaryName("1", mOrder.OrderStatusCode.ToString());
                lblOrderStatusCode.ForeColor = mOrder.OrderStatusCode == 3 ? System.Drawing.Color.Red : System.Drawing.Color.Black;

                //lblPayWay.Text = mOrder.PayWay.ToString();
                lblPayWay.Text = GetDictionaryName("4", mOrder.PayWay.ToString());

                string strPayMoney = "";
                strPayMoney = (mCompany.RoleType == 2 || mCompany.RoleType == 3) ? mOrder.OrderMoney.ToString("F2") : mOrder.PayMoney.ToString("F2");
                if (mOrder.TicketStatus.ToString() == "3" || mOrder.TicketStatus.ToString() == "4")
                {
                    strPayMoney = "-" + strPayMoney;
                }
                lblPayMoney.Text = strPayMoney;


                lblPayNo.Text = mOrder.PayNo;
                lblPayStatus.Text = (mOrder.PayStatus == 1) ? "已付" : "未付";

                //小编码
                if (!string.IsNullOrEmpty(mOrder.PNR))
                {
                    string strFN = "小编码:<a href=\"#\" title='点击提取编码信息' style=\"color:blue;\"    onclick=\"GetPNR('" + mOrder.PNR + "');return false;\"  >" + mOrder.PNR + "</a>";
                    /*
                    //内容的直接取数据库
                    if (mOrder.OrderSourceType != 1 && mOrder.OrderSourceType != 2 && mOrder.OrderSourceType != 5 && mOrder.OrderSourceType != 6 && mOrder.OrderSourceType != 8 && mOrder.OrderSourceType != 10)
                    {
                        strFN = "小编码:<a href=\"#\" title='点击提取编码信息' style=\"color:blue;\"    onclick=\"RecvData('',1);return false;\"  >" + mOrder.PNR + "</a>";
                    }
                    */
                    lblPNR.Text = (strFN + "&nbsp;<a href=\"#\" title='复制换编码' style=\"color:blue;\"  onclick=\"copyToClipboard('" + mOrder.PNR + "');return false;\" >复制</a>");

                }

                if (Source == 0)
                {
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
                }
                string showPolicyPoint = "";

                if (mCompany.RoleType == 1)
                {
                    spanZC.Visible = true;
                    lblPolicySource.Visible = true;
                    span_CPRemark.Visible = true;
                    tr_PolicySource.Visible = true;
                    trDiscountDetail.Visible = true;
                    td_PayCheck0.Visible = true;
                    td_PayCheck1.Visible = true;
                    lblHandlingRate.Text = mOrder.HandlingRate.ToString();
                    lblDiscountDetail.Text = mOrder.DiscountDetail;
                    //OldPolicyPoint,PolicyPoint,ReturnPoint,ReturnMoney,
                    showPolicyPoint += "原政策:" + mOrder.OldPolicyPoint.ToString("F1") + " 现返:" + mOrder.OldReturnMoney;
                    showPolicyPoint += "<br />出票政策:" + mOrder.PolicyPoint.ToString("F1") + " 现返:" + mOrder.ReturnMoney + " 最终政策:" + mOrder.ReturnPoint.ToString("F1");
                }
                else if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
                {
                    if (mOrder.ReturnMoney != 0)
                    {
                        showPolicyPoint += "原政策:" + mOrder.OldPolicyPoint.ToString("F1") + " 现返:" + mOrder.OldReturnMoney;
                        //showPolicyPoint += "<br />出票政策:" + mOrder.PolicyPoint.ToString("F1") + " 现返:" + mOrder.ReturnMoney;
                        showPolicyPoint += "<br />出票政策:" + mOrder.PolicyPoint.ToString("F1") + " 现返:" + mOrder.ReturnMoney + " 最终政策:" + mOrder.ReturnPoint.ToString("F1");
                    }
                    else
                    {
                        showPolicyPoint += "原政策:" + mOrder.OldPolicyPoint.ToString("F1");
                        //showPolicyPoint += "<br />出票政策:" + mOrder.PolicyPoint.ToString("F1");
                        showPolicyPoint += "<br />出票政策:" + mOrder.PolicyPoint.ToString("F1") + " 最终政策:" + mOrder.ReturnPoint.ToString("F1");
                    }
                    td_PayCheck0.Visible = true;
                    td_PayCheck1.Visible = true;
                    //政策来源
                    spanZC.Visible = true;
                    lblPolicySource.Visible = true;

                    span_CPRemark.Visible = true;

                    tr_PolicySource.Visible = true;

                }
                else if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
                {
                    td_PayCheck0.Visible = false;
                    td_PayCheck1.Visible = false;
                    if (mOrder.ReturnMoney != 0)
                    {
                        showPolicyPoint = "政策:" + mOrder.ReturnPoint.ToString("F1") + " 现返:" + mOrder.ReturnMoney;
                    }
                    else
                    {
                        showPolicyPoint = "政策:" + mOrder.ReturnPoint.ToString("F1");
                    }
                }
                lblPolicyPoint.Text = showPolicyPoint;

                lblPolicyPoint2.Text = mOrder.A7.ToString();
                lblPolicyRemark.Text = mOrder.PolicyRemark;


                ddlA9.SelectedValue = (!string.IsNullOrEmpty(mOrder.A9) && mOrder.A9 == "1") ? "1" : "0";//机票检查

                #region 政策来源

                if (mOrder.PolicySource == 9)
                {
                    if (mOrder.CPCpyNo == mUser.CpyNo)
                    {
                        if (mOrder.PolicyType == 1)
                        {
                            lblPolicySource.Text = "B2B";
                        }
                        else if (mOrder.PolicyType == 2)
                        {
                            lblPolicySource.Text = "BSP";
                        }

                        tr_PolicySource.Visible = false;
                    }
                    else
                    {
                        lblPolicySource.Text = GetDictionaryName("24", mOrder.PolicySource.ToString()); ;
                    }
                }
                else
                {
                    lblPolicySource.Text = GetDictionaryName("24", mOrder.PolicySource.ToString()); ;
                }
                #endregion


                //绑定政策来源
                BindPolicySource(mOrder.PolicySource.ToString());

                //lblPolicySource.Text = mOrder.PolicySource.ToString();

                if (mOrder.PolicySource > 1)
                {
                    if (mCompany.RoleType != 4 && mCompany.RoleType != 5)
                    {
                        trOutOrder.Visible = true;
                        //代付信息
                        lblOutOrderId.Text = mOrder.OutOrderId;
                        lblOutOrderPayFlag.Text = (mOrder.OutOrderPayFlag == true) ? "已代付" : "未代付";
                        lblOutOrderPayMoney.Text = mOrder.OutOrderPayMoney.ToString("F2");
                        lblOutOrderPayNo.Text = mOrder.OutOrderPayNo;
                    }
                }
                else
                {
                    trOutOrder.Visible = false;
                }
                // mOrder.TGQApplyReason  退改签申请理由
                // mOrder.TGQRefusalReason  退改签拒绝理由
                // mOrder.YDRemark （订票备注）预订时备注信息
                // mOrder.CPRemark （出票备注）出票时备注信息

                // 显示 预订备注
                trYDRemark.Visible = true;
                txtYDRemark.Text = mOrder.YDRemark;
                if (mOrder.TicketStatus == 1)
                {

                }
                else if (mOrder.TicketStatus == 2)
                {
                    // 显示 预订备注  出票备注
                    trCPRemark.Visible = true;
                    txtCPRemark.Text = mOrder.CPRemark;
                }
                else if (mOrder.TicketStatus == 3 || mOrder.TicketStatus == 4 || mOrder.TicketStatus == 5)
                {
                    //退废改  申请理由
                    trTGQApplyReason.Visible = true;
                    txtTGQApplyReason.Text = mOrder.TGQApplyReason;

                    // 拒绝理由
                    trTGQRefusalReason.Visible = true;
                    txtTGQRefusalReason.Text = mOrder.TGQRefusalReason;

                }
                else if (mOrder.TicketStatus == 6)
                {
                    if (mOrder.OrderStatusCode == 5 || mOrder.OrderStatusCode == 20)
                    {
                        // 拒绝理由
                        trTGQRefusalReason.Visible = true;
                        txtTGQRefusalReason.Text = mOrder.TGQRefusalReason;
                    }
                }

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

                //现在
                List<Tb_Ticket_SkyWay> SkyWayList = baseDataManage.CallMethod("Tb_Ticket_SkyWay", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_SkyWay>;

                if (SkyWayList != null && SkyWayList.Count > 0)
                {
                    RepSkyWay.DataSource = SkyWayList;
                    RepSkyWay.DataBind();
                    //查看航段信息
                    Hid_SkyWay.Value = escape(JsonHelper.ObjToJson<List<Tb_Ticket_SkyWay>>(SkyWayList));
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

                //#region 排序
                //IEnumerable<Log_Tb_AirOrder> querys = null;
                //querys = from aa in AirOrderList orderby aa.OperTime select aa;
                //AirOrderList = querys.ToList<Log_Tb_AirOrder>();
                //#endregion

                if (AirOrderList != null && AirOrderList.Count > 0)
                {
                    RepOrderLog.DataSource = AirOrderList;
                    RepOrderLog.DataBind();
                }

                #endregion

                #region 分账记录

                List<Tb_Order_PayDetail> PayDetailList = null;

                if (((mOrder.PayWay == 1 || mOrder.PayWay == 5) && mOrder.OrderStatusCode != 1 && mOrder.OrderStatusCode != 2) || mCompany.RoleType == 1)
                {
                    PayDetailList = baseDataManage.CallMethod("Tb_Order_PayDetail", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Order_PayDetail>;

                    foreach (Tb_Order_PayDetail item in PayDetailList)
                    {
                        if (!string.IsNullOrEmpty(item.PayAccount) && item.PayType == "付款" && !item.PayAccount.Contains("kehuzijinbu"))
                        {
                            lblZFZH.Text = item.PayAccount;
                            break;
                        }
                    }
                }

                if (mCompany.RoleType == 1)
                {
                    ptShow.Visible = true;

                    #region 排序
                    IEnumerable<Tb_Order_PayDetail> queryss = null;
                    queryss = from aa in PayDetailList orderby aa.CpyNo select aa;
                    PayDetailList = queryss.ToList<Tb_Order_PayDetail>();
                    #endregion

                    if (PayDetailList != null && PayDetailList.Count > 0)
                    {
                        RepPayDetail.DataSource = PayDetailList;
                        RepPayDetail.DataBind();
                    }
                }
                #endregion
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 绑定政策来源
    /// </summary>
    private void BindPolicySource(string PolicySource)
    {
        List<Bd_Base_Dictionary> dicList = GetDictionaryList("24");
        ddlPolicySource.DataSource = dicList;
        ddlPolicySource.DataTextField = "ChildName";
        ddlPolicySource.DataValueField = "ChildID";
        ddlPolicySource.DataBind();
        ddlPolicySource.SelectedValue = PolicySource.ToString();
    }

    /// <summary>
    /// 显示修改票号
    /// </summary>
    /// <param name="PasId"></param>
    /// <param name="TicketNumber"></param>
    /// <param name="TravelNumber"></param>
    /// <returns></returns>
    public string ShowTK(object PasId, object PasName, object TicketNumber, object TravelNumber)
    {
        string result = "";

        //票号不为空
        if (TicketNumber.ToString() != "")
        {
            result = "<a href=\"#\" title='点击提取票号信息' style=\"color:blue;\" onclick=\"GetDetr('" + TicketNumber.ToString() + "');return false;\">" + TicketNumber.ToString() + "</a>&nbsp;<a href=\"#\" title='复制编码' style=\"color:blue;\" onclick=\"copyToClipboard('" + TicketNumber.ToString() + "')\" >复制</a>";
        }

        if (mCompany != null && ((mCompany.RoleType != 5 && mCompany.RoleType != 4)))
        {
            Tb_Ticket_Order Order = ViewState["orderDetail"] as Tb_Ticket_Order;


            //存在行程单号不容许修改票号
            if ((TravelNumber != null && TravelNumber.ToString() != "") || TicketNumber.ToString() == "")
            {
                if (Order != null && Order.OrderSourceType == 11)//入库记账也可以修改票号
                {
                    result += "<br /><a href=\"#\" style=\"color:blue;\" onclick=\"ShowTicketNumber('" + OrderId + "','" + PasId.ToString() + "','" + PasName.ToString() + "','" + TicketNumber.ToString() + "','" + TravelNumber.ToString() + "');return false;\">修改</a>";
                }
            }
            else
            {
                result += "<br /><a href=\"#\" style=\"color:blue;\" onclick=\"ShowTicketNumber('" + OrderId + "','" + PasId.ToString() + "','" + PasName.ToString() + "','" + TicketNumber.ToString() + "','" + TravelNumber.ToString() + "');return false;\">修改</a>";
            }
        }
        return result;
    }

    #region 修改证件号

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Remark"></param>
    /// <returns></returns>
    public string ShowHide(object Remark)
    {
        string style = "display:none;";
        if (Remark != null && Remark != DBNull.Value && Remark.ToString().Trim() != "")
        {
            style = "display:block;";
        }
        return style;
    }

    /// <summary>
    /// 是否显示修改证件号
    /// </summary>
    /// <param name="SsrCard"></param>
    /// <param name="PayStatus"></param>
    /// <param name="PasId"></param>
    /// <param name="carray"></param>
    /// <returns></returns>
    public string IsShowSsrUpdate(object Remark, string PasName, string PasType, object SsrCard, string PasId)
    {
        string strRemark = escape((Remark == null ? "" : Remark.ToString()));
        PasName = PasName.Replace("\'", "");
        StringBuilder sbStr = new StringBuilder(" ");
        //证件号
        string SsrCardId = (SsrCard != DBNull.Value && SsrCard != null) ? SsrCard.ToString() : "";
        Tb_Ticket_Order Order = ViewState["orderDetail"] as Tb_Ticket_Order;
        int PayStatus = 1;
        string Carry = "";
        string OrderId = "";

        if (Order != null)
        {
            PayStatus = Order.PayStatus;
            Carry = Order.CarryCode.Split('/')[0];
            OrderId = Order.OrderId;
            //没有支付 且是白屏预订 可以修改证件号
            if (Order.OrderSourceType == 1 && (mCompany != null && (mCompany.RoleType != 5 && mCompany.RoleType != 4)))//婴儿证件号改不了 屏蔽掉//PayStatus != 2 && && PasType != "3"
            {
                string IsDate = "0";
                if (Regex.IsMatch(SsrCardId.Trim(), @"^\d{4}\-\d{2}\-\d{2}$", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase))
                {
                    IsDate = "1";
                }
                //3U只能修改证件号后4位
                if (!string.IsNullOrEmpty(Carry) && Carry.ToUpper() == "3U")
                {
                    sbStr.Append(SsrCard + "<br /><a href=\"#\" onclick=\"return ShowUpdateSsr('" + IsDate + "','" + strRemark + "','" + escape(PasName) + "','" + PasType + "','" + SsrCardId + "','" + OrderId + "','" + PasId + "',1);\">修改</a><br /><span style='" + ShowHide(strRemark) + "'><a href=\"#\" onclick=\"showdialog(unescape('" + strRemark + "'));return false; \">查看备注</a></span>");
                }
                else
                {
                    sbStr.Append(SsrCard + "<br /><a href=\"#\" onclick=\"return ShowUpdateSsr('" + IsDate + "','" + strRemark + "','" + escape(PasName) + "','" + PasType + "','" + SsrCardId + "','" + OrderId + "','" + PasId + "',0);\">修改</a><br /><span style='" + ShowHide(strRemark) + "'><a href=\"#\" onclick=\"showdialog(unescape('" + strRemark + "'));return false; \">查看备注</a></span>");
                }
            }
            else
            {
                sbStr.Append(SsrCard);
            }
        }
        return sbStr.ToString();
    }

    /// <summary>
    /// 修改证件号
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void UpdateSsr()
    {
        if (Request["SsrOpType"] != null)
        {
            string SsrOpType = GetVal("SsrOpType", "");
            string content = "";
            string msg = "";
            PbProject.Logic.Order.Tb_Ticket_OrderBLL OrderManage = new PbProject.Logic.Order.Tb_Ticket_OrderBLL();
            try
            {
                //修改证件号
                if (SsrOpType == "ssr")
                {
                    //是否同步PNR中的证件号 1是 0否
                    string IsBlockPnr = GetVal("IsBlockPnr", "");
                    //乘机人证件号
                    string PassngerSsr = GetVal("PassngerSsr", "");
                    //原来证件号
                    string oldCid = GetVal("oldCid", "");
                    //备注
                    string pRemark = GetVal("pRemark", "");
                    //乘机人id##姓名
                    string strPassengerId = GetVal("passengerId", "");
                    string[] arrList = strPassengerId.ToString().Replace("\'", "").Split(new string[] { "##" }, StringSplitOptions.None);
                    if (arrList.Length >= 2)
                    {
                        //乘机人id
                        string passengerId = arrList[0];
                        //乘机人姓名
                        string PassengerName = arrList[1];
                        //订单ID
                        string OrderId = GetVal("OrderId", "");
                        //查询订单
                        Tb_Ticket_Order mOrder = OrderManage.GetTicketOrderByOrderId(OrderId);
                        if (mOrder != null)
                        {
                            //构造需要修改的乘机人信息
                            Tb_Ticket_Passenger Passenger = new Tb_Ticket_Passenger();
                            Passenger.id = Guid.Parse(passengerId);
                            //证件号
                            Passenger.Cid = PassngerSsr;
                            //修改备注
                            Passenger.Remark = pRemark.Replace("'", "");
                            if (IsBlockPnr == "1")
                            {
                                content += " 同步编码";
                            }
                            //日志
                            content += "修改(" + PassengerName + ")证件号,原证件号:" + oldCid + ",<br />修改后证件号:" + PassngerSsr;

                            #region 指令返回消息
                            if (IsBlockPnr == "1")
                            {
                                // 指令返回消息                        
                                string pnr = mOrder.PNR;
                                string Office = mOrder.Office;
                                string PrintOffice = mOrder.PrintOffice;
                                string CarrayCode = mOrder.CarryCode.Split('/')[0];
                                if (!string.IsNullOrEmpty(pnr) && pnr.Trim().Length == 6 && !string.IsNullOrEmpty(CarrayCode) && CarrayCode.Trim().Length == 2)
                                {
                                    //----------修改证件号发送指令---------------------------------
                                    PbProject.Model.ConfigParam config = null;
                                    if (mCompany != null && mCompany.RoleType == 1)
                                    {
                                        string GYCpyNo = mOrder.CPCpyNo;
                                        if (mOrder.CPCpyNo.Length >= 12)
                                        {
                                            GYCpyNo = GYCpyNo.Substring(0, 12);
                                        }
                                        List<Bd_Base_Parameters> baseParamList = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + GYCpyNo + "'" }) as List<Bd_Base_Parameters>;
                                        config = Bd_Base_ParametersBLL.GetConfigParam(baseParamList);
                                    }
                                    if (this.configparam != null)
                                    {
                                        config = this.configparam;
                                    }
                                    //扩展参数
                                    ParamEx pe = new ParamEx();
                                    pe.UsePIDChannel = this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|48|") ? 2 : 0;
                                    SendInsManage SendManage = new SendInsManage(mUser.LoginName, mCompany.UninCode, pe, config);
                                    //修改证件号
                                    bool IsSuc = SendManage.UpdateSsr(pnr, CarrayCode, PassngerSsr, Office, PrintOffice, PassengerName.Trim(), out msg);
                                    if (!IsSuc)
                                    {
                                        msg = "同步编码【" + pnr + "】中证件号失败，请手动处理编码！";
                                    }
                                    //-------------------------------------------
                                }
                                else
                                {
                                    msg = "同步编码【" + pnr + "】中证件号失败，编码不能为空！";
                                }
                            }
                            #endregion

                            content += msg;
                            //修改证件号
                            bool reuslt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().UpdateOrderDetailInfo(0, mOrder, Passenger, mUser, mCompany, content);

                            if (reuslt)
                                msg = "修改证件号成功!<br />" + msg;
                            else
                                msg = "修改证件号失败!<br />" + msg;
                        }
                        else
                        {
                            msg = "修改失败!";
                        }
                    }
                }
                //修改票号
                else if (SsrOpType == "UPTK")
                {
                    string strOrderId = GetVal("OrderId", "");
                    string strPasID = GetVal("PasID", "");
                    string strTicketNumber = GetVal("TicketNumber", "");
                    string strOldNumber = GetVal("OldNumber", "");
                    string strTravelNumber = GetVal("TravelNumber", "");
                    string PassengerName = GetVal("PassengerName", "");
                    //查询订单
                    Tb_Ticket_Order mOrder = OrderManage.GetTicketOrderByOrderId(strOrderId);
                    if (mOrder != null)
                    {
                        string sqlWhere = string.Format(" OrderId='{0}' and TicketNumber='{1}'", strOrderId, strTicketNumber);
                        bool IsExist = (bool)this.baseDataManage.CallMethod("Tb_Ticket_Passenger", "IsExist", null, new object[] { sqlWhere });
                        if (IsExist)
                        {
                            msg = "修改票号失败，同一订单票号不能重复！";
                        }
                        else
                        {
                            Tb_Ticket_Passenger Passenger = new Tb_Ticket_Passenger();
                            Passenger.id = Guid.Parse(strPasID);
                            //票号
                            Passenger.TicketNumber = strTicketNumber;
                            //日志
                            content = "修改(" + PassengerName + ")票号,原票号:" + strOldNumber + ",修改后票号:" + strTicketNumber;
                            //修改票号
                            bool reuslt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().UpdateOrderDetailInfo(1, mOrder, Passenger, mUser, mCompany, content);
                            if (reuslt)
                                msg = "修改票号成功!";
                            else
                                msg = "修改票号失败，原因如下:<br />" + msg;
                        }
                    }
                    else
                    {
                        msg = "修改失败!";
                    }
                }
            }
            catch (Exception ex)
            {
                msg = "修改异常,错误信息:<br />" + ex.Message;
            }
            finally
            {
                OutPut(msg);
            }
        }
    }

    /// <summary>
    /// 儿童名字处理
    /// </summary>
    /// <param name="pasName"></param>
    /// <returns></returns>
    public string ReplaceCHD(object pasName)
    {
        string PasName = "";
        if (pasName != null && pasName.ToString() != "")
        {
            PasName = pasName.ToString().Trim();
            if (PnrAnalysis.PinYingMange.IsChina(PasName))
            {
                if (PasName.EndsWith("CHD"))
                {
                    PasName = PasName.Substring(0, PasName.LastIndexOf("CHD"));
                }
            }
            else
            {
                if (PasName.EndsWith(" CHD"))
                {
                    PasName = PasName.Substring(0, PasName.LastIndexOf(" CHD"));
                }
            }
        }
        return PasName;
    }

    /// <summary>
    /// 证件号修改显示
    /// </summary>
    /// <returns></returns>
    public bool showSsr(int type, object PassengerType)
    {
        bool result = false;
        if (type == 0)
        {
            //非婴儿
            if (PassengerType != null)
            {
                if (PassengerType.ToString() != "3" && CarryCode == "3U")
                {
                    result = true;
                }
            }
        }
        return result;
    }

    #endregion

    #region 创建行程单

    private int m_OrderStatus = 0;
    private string m_CarryCode = string.Empty;

    /// <summary>
    /// 订单状态
    /// </summary>
    public int OrderStatus
    {
        get
        {
            return string.IsNullOrEmpty(Hid_OrderStatusCode.Value) ? 0 : int.Parse(Hid_OrderStatusCode.Value);
        }
        set
        {
            m_OrderStatus = value;
            Hid_OrderStatusCode.Value = value.ToString();
        }
    }
    /// <summary>
    /// 订单号
    /// </summary>
    public string OrderId
    {
        get
        {
            return Hid_OrderId.Value;
        }
        set
        {
            Hid_OrderId.Value = value;
        }
    }
    /// <summary>
    /// 航空公司二字码
    /// </summary>
    public string CarryCode
    {
        get
        {
            return m_CarryCode;
        }
        set
        {
            m_CarryCode = value;
        }
    }
    /// <summary>
    /// 获取请求值
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="DefaultVal"></param>
    /// <returns></returns>
    public string GetVal(string Name, string DefaultVal)
    {
        if (Request[Name] != null)
        {
            DefaultVal = HttpUtility.UrlDecode(Request[Name].ToString(), Encoding.Default);
        }
        return DefaultVal;
    }

    /// <summary>
    /// 响应客户端结果数据
    /// </summary>
    /// <param name="result"></param>
    public void OutPut(string result)
    {
        Context.Response.ContentType = "text/plain";
        Context.Response.Clear();
        Context.Response.Write(result);
        Context.Response.Flush();
        Context.Response.End();
    }

    /// <summary>
    /// 行程单操作
    /// </summary>
    public void OPTrip()
    {
        if (Request["optype"] != null)
        {
            string optype = Request["optype"].ToString();
            string result = "0##失败";
            try
            {
                string type = GetVal("type", "");
                string pid = GetVal("pid", "");
                string TPId = GetVal("TPId", "");
                string pName = GetVal("pName", "");
                string TKnum = GetVal("TKnum", "");
                string TPNum = GetVal("TPNum", "");
                string Office = GetVal("Office", "");
                string CpyNo = GetVal("CpyNo", "");
                string GYCpyNo = GetVal("GYCpyNo", "");
                string RoleType = GetVal("RoleType", "");
                string OrderId = GetVal("OrderId", "");//订单号
                SendInsManage SendManage = null;
                PbProject.Model.ConfigParam config = null;
                if (RoleType == "1" && optype != "SAVENUM")
                {
                    List<Bd_Base_Parameters> baseParamList = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + GYCpyNo + "'" }) as List<Bd_Base_Parameters>;
                    config = Bd_Base_ParametersBLL.GetConfigParam(baseParamList);
                }
                if (optype == "SAVENUM")
                {
                    IHashObject hash = new HashObject();
                    string StartAndEnd = GetVal("StartAndEnd", "");
                    CpyNo = GetVal("CpyNo", "");
                    string ParamId = GetVal("ParamId", "");
                    hash.Add("id", ParamId);
                    hash.Add("CpyNo", CpyNo);
                    hash.Add("SetValue", StartAndEnd);
                    hash.Add("SetName", "SaveTrip");
                    string sqlWhere = string.Format(" CpyNo='{0}' and SetName='SaveTrip' ", CpyNo);
                    bool IsSuc = (bool)this.baseDataManage.CallMethod("Bd_Base_Parameters", "IsExist", null, new object[] { sqlWhere });
                    if (!IsSuc)
                    {
                        Bd_Base_Parameters bd_base_parameters = new Bd_Base_Parameters();
                        bd_base_parameters.CpyNo = CpyNo;
                        bd_base_parameters.SetName = "SaveTrip";
                        bd_base_parameters.SetValue = StartAndEnd;
                        bd_base_parameters.StartDate = DateTime.Now;
                        bd_base_parameters.EndDate = DateTime.Now.AddYears(20);
                        bd_base_parameters.SetDescription = "行程单打印号段设置";
                        bd_base_parameters.Remark = "行程单打印号段设置";
                        //不存在添加
                        IsSuc = (bool)this.baseDataManage.CallMethod("Bd_Base_Parameters", "Insert", null, new object[] { bd_base_parameters });
                        this.baseParametersList.Add(bd_base_parameters);
                    }
                    else
                    {
                        //存在修改                       
                        IsSuc = (bool)this.baseDataManage.CallMethod("Bd_Base_Parameters", "Update", null, new object[] { hash });
                    }
                    if (IsSuc)
                    {
                        for (int i = 0; i < this.baseParametersList.Count; i++)
                        {
                            if (this.baseParametersList[i].SetName == "SaveTrip")
                            {
                                this.baseParametersList[i].SetValue = StartAndEnd;
                                break;
                            }
                        }
                        result = "1##保存成功";
                    }
                    else
                    {
                        result = "0##保存失败";
                    }
                }
                else if (optype == "create")
                {
                    if (this.configparam != null)
                    {
                        config = this.configparam;
                    }
                    //扩展参数
                    ParamEx pe = new ParamEx();
                    pe.UsePIDChannel = this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|48|") ? 2 : 0;
                    SendManage = new SendInsManage(mUser.LoginName, mCompany.UninCode, pe, config);
                    //创建
                    TripParam TP = new TripParam();
                    TP.TicketNumber = TKnum.Trim();
                    TP.TripNumber = TPNum.Trim();
                    TP.Office = Office;
                    TP.InsType = 0;
                    bool IsOk = SendManage.SendTrip(TP);
                    if (IsOk)
                    {
                        bool IsUpdate = true;
                        if (TP.Msg.Contains("##"))
                        {
                            string[] strArr = TP.Msg.Split(new string[] { "##" }, StringSplitOptions.None);
                            result = "1##" + strArr[0];
                            if (strArr[0].Contains("已创建行程单") && strArr.Length >= 2)
                            {
                                string[] strTKTP = strArr[1].Split('|');
                                if (!(strTKTP.Length == 2 && strTKTP[0] == TP.TicketNumber && strTKTP[1] == TP.TripNumber))
                                {
                                    //检查行程单号是否为系统行程单号
                                    string sqlWhere = string.Format(" UseCpyNo='{0}' and TripNum='{1}' ", CpyNo, strTKTP[1]);
                                    bool IsExist = (bool)this.baseDataManage.CallMethod("Tb_TripDistribution", "IsExist", null, new object[] { sqlWhere });
                                    if (!IsExist)
                                    {
                                        result = "1##该票号（" + TP.TicketNumber + "）已关联非系统行程单号(" + strTKTP[1] + "),请申请该系统行程单！";
                                        IsUpdate = false;
                                    }
                                }
                                else
                                {
                                    result = "1##该票号（" + TP.TicketNumber + "）已关联系统行程单号(" + strTKTP[1] + "),创建行程单号成功！";
                                }
                            }
                        }
                        else
                        {
                            result = "1##创建行程单号成功！";
                        }
                        //修改行程单状态和乘机人数据
                        if (IsUpdate)
                        {
                            string errMsg = "";
                            List<string> lstSQL = new List<string>();
                            lstSQL.Add(string.Format(" update Tb_Ticket_Passenger set TicketNumber='{0}',TravelNumber='{1}',TravelStatus=1 where id='{2}' ", TP.TicketNumber, TP.TripNumber, pid));
                            lstSQL.Add(string.Format(" update  Tb_TripDistribution set TripStatus=9,PrintTime=GETDATE() ,TicketNum='{0}' where id='{1}' and TripNum='{2}' ", TP.TicketNumber, TPId, TP.TripNumber));
                            if (!this.baseDataManage.ExecuteSqlTran(lstSQL, out errMsg))
                            {
                                List<string> strList = new List<string>();
                                strList.Add(TPId);
                                this.baseDataManage.CallMethod("Tb_TripDistribution", "UpdateByIds", null, new object[] { " A1=A1+1 ", strList });
                            }
                            #region //同步2.94
                            string[] strParam = new string[]
                            {
                                "OpType=1",
                                "TripNum="+TPNum,
                                "TicketNumber="+TP.TicketNumber,                          
                                "Office="+TP.Office,
                                "OrderId="+OrderId,
                                "LoginName="+mUser.LoginName,
                                "CompanyName="+mCompany.UninAllName,
                                "PassengerName="+pName,
                                "CpyNo="+mCompany.UninCode
                            };
                            TonBuParam(strParam);
                            #endregion
                        }
                    }
                    else
                    {
                        result = "0##创建行程单失败，原因如下:<br />" + TP.Msg;
                    }
                }
                else if (optype == "void")
                {
                    //作废
                    if (this.configparam != null)
                    {
                        config = this.configparam;
                    }
                    #region 获取Office
                    string sqlWhere = string.Format(" TripNum='{0}' and UseCpyNo='{1}'", TPNum, CpyNo);
                    string strFileds = " CreateOffice,TripNum ";
                    List<Tb_TripDistribution> tbList = this.baseDataManage.CallMethod("Tb_TripDistribution", "GetList", null, new object[] { strFileds, sqlWhere }) as List<Tb_TripDistribution>;
                    if (tbList != null && tbList.Count > 0)
                    {
                        Office = tbList[0].CreateOffice;
                    }
                    #endregion

                    //扩展参数
                    ParamEx pe = new ParamEx();
                    pe.UsePIDChannel = this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|48|") ? 2 : 0;
                    SendManage = new SendInsManage(mUser.LoginName, mCompany.UninCode, pe, config);
                    TripParam TP = new TripParam();
                    TP.TicketNumber = TKnum.Trim();
                    TP.TripNumber = TPNum.Trim();
                    TP.Office = Office;
                    TP.InsType = 1;
                    bool IsOk = SendManage.SendTrip(TP);
                    if (IsOk)
                    {
                        result = "1##作废行程单成功！";
                        string errMsg = "";
                        List<string> lstSQL = new List<string>();
                        lstSQL.Add(string.Format(" update Tb_Ticket_Passenger set TravelStatus=2 where id='{0}' ", pid));
                        lstSQL.Add(string.Format(" update  Tb_TripDistribution set TripStatus=6,InvalidTime=GETDATE() where {0} ", string.Format(" TripNum='{0}' and UseCpyNo='{1}'", TPNum, CpyNo)));
                        if (!this.baseDataManage.ExecuteSqlTran(lstSQL, out errMsg))
                        {
                            result = "1##作废行程单成功，状态修改失败！";
                        }
                        #region //同步2.94
                        string[] strParam = new string[]
                        {
                            "OpType=2",
                            "TripNum="+TPNum,
                            "TicketNumber="+TP.TicketNumber,                          
                            "Office="+TP.Office,
                            "OrderId="+OrderId,
                            "LoginName="+mUser.LoginName,
                            "CompanyName="+mCompany.UninAllName,
                            "PassengerName="+pName,   
                            "CpyNo="+mCompany.UninCode                         
                        };
                        TonBuParam(strParam);
                        #endregion
                    }
                    else
                    {
                        if (TP.Msg.Contains("##"))
                        {
                            result = "0##" + TP.Msg.Split(new string[] { "##" }, StringSplitOptions.None)[0];
                        }
                        else
                        {
                            if (TP.Msg.Trim() != "")
                            {
                                result = "0##" + TP.Msg;
                            }
                            else
                            {
                                result = "0##作废行程单失败！";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "0##请求异常!";
            }
            finally
            {
                OutPut(result);
            }
        }
    }
    List<Bd_Base_Dictionary> db_dicList = null;

    /// <summary>
    /// 绑定乘客列表显示隐藏数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public string ShowText(int type, params object[] obj)
    {
        string result = "";

        try
        {
            if (type == 0)
            {
                result = "hide";
                //是否有创建行程单的权限
                if (KongZhiXiTong != null && KongZhiXiTong.Contains("|14|"))
                {
                    //是否显示行程单信息
                    if (OrderStatus == 4)
                    {
                        result = "show";
                    }
                }
            }
            else if (type == 1)
            {
                //获取操作类型 日志
                if (obj != null && obj.Length > 0 && obj[0] != DBNull.Value && obj[0].ToString() != "")
                {
                    result = obj[0].ToString();

                    //int ChildId = int.Parse(obj.ToString());
                    //if (db_dicList == null)
                    //{
                    //    db_dicList = new Bd_Base_DictionaryBLL().GetListByParentID(5);
                    //}
                    //List<Bd_Base_Dictionary> ldc = (from Bd_Base_Dictionary dic in db_dicList
                    //                                where dic.ChildID == ChildId
                    //                                select dic).ToList<Bd_Base_Dictionary>();
                    //if (ldc.Count > 0)
                    //{
                    //    result = ldc[0].ChildName;
                    //}
                    //else
                    //{
                    //    result = obj.ToString();
                    //}
                }
            }
            else if (type == 2)//打印票据
            {
                result = "hide";
                if (obj != null && obj.Length == 1)
                {
                    string OrderStatusCode = OrderStatus.ToString();
                    string OwnerCpyNo = Hid_CpyNo.Value.ToString();
                    string ShowCpyNo = obj[0].ToString();//显示的公司编码 无权限控制盒订单号判断
                    if (OwnerCpyNo.Length >= 12)
                    {
                        OwnerCpyNo = OwnerCpyNo.Substring(0, 12);
                    }
                    if (ShowCpyNo.Contains("|" + OwnerCpyNo + "|"))//重庆奥奇
                    {
                        result = "show";
                    }
                    else
                    {   //订单状态
                        if (OrderStatusCode == "4" || OrderStatusCode == "7" || OrderStatusCode == "8")
                        {
                            //权限控制
                            result = (GongYingKongZhiFenXiao != null && GongYingKongZhiFenXiao.Contains("|1|")) ? "show" : "hide";
                        }
                    }
                }
            }
            else if (type == 3)//行程单号
            {
                result = "<font class=\"red\">未创建</font>";
                if (obj != null && obj.Length == 2)
                {
                    string TravelNumber = obj[0] != null ? obj[0].ToString() : "";
                    string TravelStatus = obj[1].ToString();
                    if (TravelStatus == "1")
                    {
                        result = TravelNumber + "<br /><font class=\"green\">已创建</font>";
                    }
                    else if (TravelStatus == "2")
                    {
                        result = TravelNumber + "<br /><font class=\"green\">已作废</font>";
                    }
                    else
                    {
                        result = "<font class=\"red\">未创建</font>";
                    }
                }
            }
            else if (type == 4)//时间 
            {
                //起飞日期
                if (obj != null && obj.Length > 0)
                {
                    //result = objData.ToString().Trim().Replace("/", "<br />");
                    result = DateTime.Parse(obj[0].ToString()).ToString("yyyy-MM-dd HH:mm");
                }
            }
        }
        catch (Exception)
        {
        }
        return result;
    }
    /// <summary>
    /// 同步老版本行程单
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public void TonBuParam(string[] postParam)
    {
        try
        {
            HttpSend http = new HttpSend();
            string domain = "";
            string[] strIPAddress = Hid_IPAddress.Value.Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in strIPAddress)
            {
                string[] strData = item.Split('|');
                if (strData.Length == 2 && mCompany.UninCode.Trim().Length >= 12 && strData[0].Trim() == mCompany.UninCode.Trim().Substring(0, 12))
                {
                    domain = strData[1];
                    break;
                }
            }
            StringBuilder sbTongBuLog = new StringBuilder();
            if (domain != "" && postParam != null)
            {
                domain = "http://" + domain + "/Pay/TripNotify.aspx";
                sbTongBuLog.Append("请求URL:" + domain + "\r\n请求数据：" + string.Join("\r\n", postParam));
                for (int i = 0; i < postParam.Length; i++)
                {
                    if (postParam[i].Split('=').Length == 2)
                    {
                        postParam[i] = postParam[i].Split('=')[0] + "=" + HttpUtility.UrlEncode(postParam[i].Split('=')[1]);
                    }
                }
                string equResult = http.SendRequest(domain, "POST", Encoding.Default, 60, postParam);
                sbTongBuLog.Append("接收数据：" + equResult);
                //记录日志
                PnrAnalysis.LogText.LogWrite(sbTongBuLog.ToString(), "SendOld");
            }
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// 显示和隐藏 创建作废行程单按钮
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="objPassengerName"></param>
    /// <param name="objTicketNumber"></param>
    /// <param name="TripNumber"></param>
    /// <returns></returns>
    public string ShowTrip(string Id, object objPassengerName, object objTicketNumber, object TripNumber, object TravelStatus)
    {
        string result = "";
        string strPassengerName = objPassengerName == null ? "" : objPassengerName.ToString();
        string strTicketNumber = objTicketNumber == null ? "" : objTicketNumber.ToString();
        string strTravelStatus = TravelStatus == null ? "0" : TravelStatus.ToString();
        if (TripNumber != null && TripNumber != DBNull.Value && TripNumber.ToString() != "" && strTravelStatus == "1")
        {
            //已创建 显示作废按钮
            result = "<a href='#' onclick='return ShowHTML({type:1,pid:\"" + Id + "\",pName:\"" + strPassengerName + "\", TKnum:\"" + strTicketNumber.Replace("'", "-") + "\",TPNum:\"" + TripNumber.ToString() + "\"});'>作废行程单</a>";
        }
        else
        {
            //已作废 显示创建按钮
            result = "<a href='#' onclick='return ShowHTML({type:0,pid:\"" + Id + "\",pName:\"" + strPassengerName + "\", TKnum:\"" + strTicketNumber.Replace("'", "-") + "\",TPNum:\"" + TripNumber.ToString() + "\"});'>创建行程单</a>";
        }
        return result;
    }

    /// <summary>
    /// 隐藏信息
    /// </summary>
    /// <param name="mOrder"></param>
    public void SetHidInfo(Tb_Ticket_Order mOrder)
    {
        //订单号
        Hid_OrderId.Value = mOrder.OrderId;
        //设置订单状态
        OrderStatus = mOrder.OrderStatusCode;
        //订单号
        OrderId = mOrder.OrderId;
        //航空公司二字码
        CarryCode = !string.IsNullOrEmpty(mOrder.CarryCode) ? mOrder.CarryCode.ToUpper().Split('/')[0] : "";
        //用户角色
        Hid_RoleType.Value = mCompany.RoleType.ToString();
        //创建订单的公司编号(领用行程单公司编号)
        Hid_CpyNo.Value = mOrder.OwnerCpyNo;
        //生成订单时的Office       
        Hid_Office.Value = mOrder.Office;
        //供应或者落地运营商公司编号
        Hid_GYCpyNo.Value = mOrder.CreateCpyNo;
        //管理员时
        if (mCompany.RoleType == 1)
        {
            //创建订单的账号 姓名 公司编号 公司名称(即分销或者采购)用于行程单申请
            Hid_ApplyApram.Value = mOrder.CreateLoginName + "@@" + mOrder.CreateUserName + "@@" + mOrder.OwnerCpyNo + "@@" + mOrder.OwnerCpyName;
        }
    }

    /// <summary>
    /// 获取可利用的行程单号
    /// </summary>
    public void GetValidTrip()
    {
        ////2已分配,未使用 11空白回收,已分配 
        if (mCompany.RoleType > 3)
        {
            string sqlWhere = string.Format(" UseCpyNo='{0}' and TripStatus in(2,11) order by A1,TripNum ", mCompany.UninCode);
            string Fileds = " id,TripNum,CreateOffice ";
            List<Tb_TripDistribution> triplist = this.baseDataManage.CallMethod("Tb_TripDistribution", "GetList", null, new object[] { Fileds, sqlWhere }) as List<Tb_TripDistribution>;
            string startnum = "", endnum = "";
            //获取设置的行程单号段
            if (this.baseParametersList != null && this.baseParametersList.Count > 0)
            {
                List<Bd_Base_Parameters> SaveTripList = (from Bd_Base_Parameters p in this.baseParametersList
                                                         where p.SetName == "SaveTrip"
                                                         select p).ToList<Bd_Base_Parameters>();
                if (SaveTripList.Count > 0)
                {
                    Hid_SetStartEndNum.Value = SaveTripList[0].SetValue;
                    Hid_ParamId.Value = SaveTripList[0].id.ToString();
                    string[] strArr = SaveTripList[0].SetValue.Split('-');
                    if (strArr != null && strArr.Length == 2)
                    {
                        startnum = strArr[0];
                        endnum = strArr[1];
                    }
                }
            }
            if (triplist.Count > 0)
            {
                //是否有可用行程单
                Hid_IsValid.Value = "1";
                List<Tb_TripDistribution> tempList = (from Tb_TripDistribution t in triplist
                                                      orderby t.TripNum
                                                      select t).ToList<Tb_TripDistribution>();
                //行程单可用号段
                Hid_StartEndNum.Value = tempList[0].TripNum + "-" + tempList[tempList.Count - 1].TripNum;
                if (startnum != "" && endnum != "")
                {
                    triplist = (from Tb_TripDistribution t in triplist
                                where long.Parse(t.TripNum) >= long.Parse(startnum) && long.Parse(t.TripNum) <= long.Parse(endnum)
                                orderby t.TripNum
                                select t).ToList<Tb_TripDistribution>();
                }
                if (triplist != null && triplist.Count > 0)
                {
                    //行程单数据
                    Hid_TripData.Value = JsonHelper.ObjToJson<List<Tb_TripDistribution>>(triplist);
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// 修改出票备注
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateCPRemark_Click(object sender, EventArgs e)
    {
        PbProject.Logic.Order.Tb_Ticket_OrderBLL OrderManage = new PbProject.Logic.Order.Tb_Ticket_OrderBLL();
        Tb_Ticket_Order mOrder = OrderManage.GetTicketOrderByOrderId(OrderId);
        if (mOrder != null)
        {
            //修改之前的出票备注
            string OldCPRemark = mOrder.CPRemark.Trim();
            string NewRemark = txtCPRemark.Text.Trim();
            string content = "", msg = "";
            if (OldCPRemark == NewRemark)
            {
                msg = "修改出票备注不能和原来相同！";
            }
            else
            {
                mOrder.CPRemark = NewRemark;
                //日志
                content = " 修改出票备注 ,原出票备注:" + OldCPRemark + ",修改后出票备注:" + NewRemark;
                //修改票号
                bool reuslt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().UpdateOrderDetailInfo(2, mOrder, null, mUser, mCompany, content);
                if (reuslt)
                {
                    msg = "修改出票备注成功";
                    //订单号
                    OrderBind(1);
                }
                else
                {
                    msg = "修改出票备注失败";
                }
            }
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('" + msg + "')", true);
        }
    }

    /// <summary>
    /// 修改政策来源
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdatePolicySource_Click(object sender, EventArgs e)
    {
        PbProject.Logic.Order.Tb_Ticket_OrderBLL OrderManage = new PbProject.Logic.Order.Tb_Ticket_OrderBLL();
        Tb_Ticket_Order mOrder = OrderManage.GetTicketOrderByOrderId(OrderId);
        if (mOrder != null)
        {
            //修改政策来源
            string OldPolicyText = GetDictionaryName("24", mOrder.PolicySource.ToString());
            string PolicyVal = ddlPolicySource.SelectedValue.Trim();
            string PolicyText = GetDictionaryName("24", PolicyVal);

            string content = "", msg = "";
            if (mOrder.PolicySource.ToString().Trim() == PolicyVal.Trim())
            {
                msg = "政策来源相同不能修改！";
            }
            else
            {
                mOrder.PolicySource = int.Parse(PolicyVal);
                //日志
                content = "修改政策来源 ,原政策来源:" + OldPolicyText + ",修改后政策来源:" + PolicyText;
                //修改票号
                bool reuslt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().UpdateOrderDetailInfo(3, mOrder, null, mUser, mCompany, content);
                if (reuslt)
                {
                    msg = "修改政策来源成功";
                    //订单号
                    OrderBind(1);
                }
                else
                {
                    msg = "修改政策来源失败";
                }
            }

            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('" + msg + "')", true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RepPassenger_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
        {
            e.Item.FindControl("spanSmsSend").Visible = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="A10"></param>
    /// <returns></returns>
    public string GetTel(object A10)
    {
        string msg = "";
        try
        {
            Tb_Ticket_Order mOrder = ViewState["orderDetail"] as Tb_Ticket_Order;

            //<%#Eval("A10")%><br />
            //<span runat="server" id="spanSmsSend" visible='<%#Eval("A10")==""?false:true %>'><a href="#" onclick='OpenWinSendSms(<%#Eval("A10")%>);'>发送短信</a></span>

            if (A10 != null && !string.IsNullOrEmpty(A10.ToString()))
            {
                if (mOrder.PolicySource == 9)
                {
                    if (mOrder.CPCpyNo == mUser.CpyNo)
                    {
                        msg = "";
                    }
                    else
                    {
                        msg = A10 + "<br/><a href='#' onclick='OpenWinSendSms(" + A10 + ");'>发送短信</a>";//显示
                    }
                }
                else
                {
                    msg = A10 + "<br/><a href='#' onclick='OpenWinSendSms(" + A10 + ");'>发送短信</a>";//显示
                }
            }
            else
            {
                msg = "";
            }
        }
        catch (Exception ex)
        {

        }
        return msg;
    }
    //修改  (支付/退废)检查
    protected void btnUpStatus_Click(object sender, EventArgs e)
    {
        try
        {
            //0要检查 1不检查
            string A9 = ddlA9.SelectedValue;
            PbProject.Logic.Order.Tb_Ticket_OrderBLL OrderBLL = new PbProject.Logic.Order.Tb_Ticket_OrderBLL();
            Tb_Ticket_Order m_order = (ViewState["orderDetail"] as Tb_Ticket_Order).Clone() as Tb_Ticket_Order;
            bool IsSuc = OrderBLL.UpdatePayRefounedChecked(m_order, mUser, mCompany, A9);
            string msg = "";
            if (IsSuc)
            {
                //订单号
                OrderBind(1);
                msg = "修改成功";
            }
            else
            {
                msg = "修改失败";
            }
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('" + msg + "')", true);
        }
        catch (Exception ex)
        {
            PnrAnalysis.LogText.LogWrite("(支付/退废)检查：" + ex.Message, "btnUpStatus");
        }
    }

    /// <summary>
    /// 是否开启行程单打印数据可以修改的权限
    /// </summary>
    public string IsOpenTravelPrintUpdate
    {
        get
        {
            return this.GongYingKongZhiFenXiao != null && this.GongYingKongZhiFenXiao.Contains("|45|") ? "PrintAfter.aspx" : "TravelPrint.aspx";
        }
    }
}