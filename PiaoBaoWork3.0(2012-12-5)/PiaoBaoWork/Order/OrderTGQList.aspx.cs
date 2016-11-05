using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using System.Xml;
using System.IO;
using System.Data;
using System.Collections;
using PbProject.Model;
using DataBase.Data;
using PbProject.Logic.Order;
using PbProject.WebCommon.Utility;

/// <summary>
/// 退改签处理  供应 页面
/// </summary>
public partial class Order_OrderTGQList : BasePage
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
                this.IsQuery.Value = "Query";
                //当前时间
                DateTime dt = DateTime.Now;
                //每月一号时间
                DateTime dt1 = new DateTime(dt.Year, dt.Month, 1);

                //txtFromDate1.Value = dt.AddDays(-7).ToString("yyyy-MM-dd");
                //txtFromDate2.Value = dt.ToString("yyyy-MM-dd");

                txtCreateTime1.Value = dt.AddDays(-7).ToString("yyyy-MM-dd");
                txtCreateTime2.Value = dt.ToString("yyyy-MM-dd");

                //if (mSupperConfig.a1.Contains("|13|"))
                string tempShow = true ? "none" : "";

                OrderSourceth.Style.Add("display", tempShow);
                OrderSourcetd.Style.Add("display", tempShow);

                if (Request.QueryString["type"] != null && Request.QueryString["type"] == "1")
                {
                    //单独退款
                    //tdSec1.Visible = false;
                    //tdSec2.Visible = false;
                    //tdSec3.Visible = false;
                    //tdSec4.Visible = false;
                    //tdSec6.Visible = false;
                    //tdSec7.Visible = false;
                    //tdSec8.Visible = false;
                    //tdSec10.Visible = false;

                    tdSec1.Style.Add("display", "none");
                    tdSec2.Style.Add("display", "none");
                    tdSec3.Style.Add("display", "none");
                    tdSec4.Style.Add("display", "none");
                    tdSec6.Style.Add("display", "none");
                    tdSec7.Style.Add("display", "none");
                    tdSec8.Style.Add("display", "none");
                    tdSec10.Style.Add("display", "none");

                    ddlStatus.Items.Add(new ListItem("审核通过,待退款", "5"));
                    ddlStatus.Items.Add(new ListItem("退款中的订单", "9"));

                    lblShow.Text = "待退款订单";

                    Hid_num.Value = "5";
                    
                }
                else
                {

                    ddlStatus.Items.Add(new ListItem("全部", "1"));
                    ddlStatus.Items.Add(new ListItem("审核中", "10"));
                    ddlStatus.Items.Add(new ListItem("退票等待审核", "2"));
                    ddlStatus.Items.Add(new ListItem("废票等待审核", "3"));
                    ddlStatus.Items.Add(new ListItem("改签订单", "4"));


                    if (KongZhiXiTong != null && KongZhiXiTong.Contains("|47|"))
                    {
                        // tdSec5.Visible = false; // 退废票 审核和退款分开
                        tdSec5.Style.Add("display", "none");
                    }
                    else
                    {
                        ddlStatus.Items.Add(new ListItem("审核通过,待退款", "5"));  //显示所有
                    }

                    ddlStatus.Items.Add(new ListItem("拒绝申请的订单", "6"));
                    ddlStatus.Items.Add(new ListItem("交易结束", "7"));
                    ddlStatus.Items.Add(new ListItem("异地退废改签订单", "8"));
                    ddlStatus.Items.Add(new ListItem("退款中的订单", "9"));

                    
                   
                }

                ViewState["orderBy"] = " CreateTime desc ";
                AspNetPager1.PageSize = 20;
                

                if (GetParam("IsQuery") == "Query")
                {
                    string status = GetParam("ddlStatus");
                    this.txtOrderId.Text = GetParam("txtOrderId");
                    this.txtPNR.Text = GetParam("txtPNR");
                    this.txtPassengerName.Text = GetParam("txtPassengerName");
                    this.hidFromCity.Value = GetParam("hidFromCity");
                    this.txtFromDate1.Value = GetParam("txtFromDate1");
                    this.txtFromDate2.Value = GetParam("txtFromDate2");
                    this.hidToCity.Value = GetParam("hidToCity");
                    this.txtCreateTime1.Value = GetParam("txtCreateTime1");
                    this.txtCreateTime2.Value = GetParam("txtCreateTime2");
                    this.txtFlightCode.Text = GetParam("txtFlightCode");
                    this.rbtlOrderS.SelectedValue = GetParam("rbtnlOrderS");
                    this.ddlStatus.SelectedValue = status;
                    this.Hid_num.Value = status;
                    Curr = int.Parse(GetParam("AspNetPager1_input"));
                    Con = SelWhere();
                    PageDataBind();
                    if (status != "1")
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "", "SetBtnClass();", true); 
                    }
                }
                //用于订单提醒查询
                showPrompt();
            }

        }
        catch (Exception ex)
        {

        }
    }
    private string GetParam(string key)
    {
        string result = string.Empty;
        if (Request.QueryString[key] != null)
            result = Request.QueryString[key].ToString();
        return result;
    }
    /// <summary>
    /// 用于 订单提醒查询
    /// </summary>
    public void showPrompt()
    {
        if (Request["prompt"] != null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "btnOk('" + Request["prompt"].ToString() + "');", true);
        }
    }

    public string ZFZ(string a8)
    {
        string result = string.Empty;
        if (!string.IsNullOrEmpty(a8))
        {
            if (a8 == "0")
                result = "<br /><span class=\"red\">非自愿</span>";
            else if (a8 == "1")
                result = "<br /><span class=\"red\">自愿</span>";
        }
        return result;
    }
    /// <summary>
    /// 绑定数据
    /// </summary>
    private void PageDataBind()
    {
        try
        {
            int TotalCount = 0;

            IHashObject outParams = new HashObject();
            //指定参数类型 第一个参数为out输出类型
            //key 为参数索引从1开始 value为引用类型 out ref
            outParams.Add("1", "out");
            PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
            List<Tb_Ticket_Order> list = baseDataManage.CallMethod("Tb_Ticket_Order", "GetBasePager1", outParams,
                new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Tb_Ticket_Order>;

            TotalCount = outParams.GetValue<int>("1");

            AspNetPager1.RecordCount = TotalCount;
            AspNetPager1.CurrentPageIndex = Curr;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;

            repList.DataSource = list;
            repList.DataBind();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('页面出错，请从新点击链接!');", true);
        }
    }

    protected string Con
    {
        get { return (string)ViewState["Con"]; }
        set { ViewState["Con"] = value; }
    }
    protected int Curr
    {
        get { return (int)ViewState["currpage"]; }
        set { ViewState["currpage"] = value; }
    }

    /// <summary>
    /// 翻页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        Con = SelWhere();
        PageDataBind();
    }

    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    private string SelWhere()
    {
        StringBuilder StrWhere = new StringBuilder(" PayStatus=1 ");
        StrWhere.Append(" and CPCpyNo='" + mUser.CpyNo + "' ");

        try
        {
            //订单号
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtOrderId.Text.Trim())))
                StrWhere.Append(" and OrderId='" + CommonManage.TrimSQL(txtOrderId.Text.Trim()) + "' ");
            //pnr
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtPNR.Text.Trim())))
                StrWhere.Append(" and PNR='" + CommonManage.TrimSQL(txtPNR.Text.Trim()) + "' ");
            //乘机人
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtPassengerName.Text.Trim())))
                StrWhere.Append(" and PassengerName like'%" + CommonManage.TrimSQL(txtPassengerName.Text.Trim()) + "%' ");

            //航班号
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtFlightCode.Text.Trim())))
                StrWhere.Append(" and FlightCode ='" + CommonManage.TrimSQL(txtFlightCode.Text.Trim()) + "' ");

            //乘机日期
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtFromDate1.Value.Trim())))
                StrWhere.Append(" and AirTime >'" + CommonManage.TrimSQL(txtFromDate1.Value.Trim()) + " 00:00:00'");
            //乘机日期
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtFromDate2.Value.Trim())))
                StrWhere.Append(" and AirTime <'" + CommonManage.TrimSQL(txtFromDate2.Value.Trim()) + " 23:59:59'");

            //创建日期
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtCreateTime1.Value.Trim())))
                StrWhere.Append(" and CreateTime >'" + CommonManage.TrimSQL(txtCreateTime1.Value.Trim()) + " 00:00:00'");
            //创建日期
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtCreateTime2.Value.Trim())))
                StrWhere.Append(" and CreateTime <'" + CommonManage.TrimSQL(txtCreateTime2.Value.Trim()) + " 23:59:59'");

            ////城市控件
            //if (txtFromCity.Value.Trim() != "" && txtFromCity.Value.Trim() != "中文/拼音" && txtFromCity.Value.Trim() != "中文/英文")
            //    StrWhere.Append(" and Travel like '" + CommonManage.TrimSQL(txtFromCity.Value.Trim()) + "%'");
            //if (txtToCity.Value.Trim() != "" && txtToCity.Value.Trim() != "中文/拼音" && txtToCity.Value.Trim() != "中文/英文")
            //    StrWhere.Append(" and Travel like '%" + CommonManage.TrimSQL(txtToCity.Value.Trim()) + "'");

            ////城市控件
            if (hidFromCity.Value.Trim() != "" && txtFromCity.Value.Trim() != "" && txtFromCity.Value.Trim() != "中文/拼音" && txtFromCity.Value.Trim() != "中文/英文")
                StrWhere.Append(" and TravelCode like '" + CommonManage.TrimSQL(hidFromCity.Value.Trim()) + "%'");
            if (hidToCity.Value.Trim() != "" && txtToCity.Value.Trim() != "" && txtToCity.Value.Trim() != "中文/拼音" && txtToCity.Value.Trim() != "中文/英文")
                StrWhere.Append(" and TravelCode like '%" + CommonManage.TrimSQL(hidToCity.Value.Trim()) + "'");
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtCpyName.Text.Trim())))
            {
                StrWhere.Append(" and OwnerCpyName like '%"+CommonManage.TrimSQL(txtCpyName.Text.Trim())+"%'");
            }
        }
        catch (Exception)
        {

        }

        #region 查询订单状态
        //<asp:ListItem Text="全部" Value="1"></asp:ListItem> 6,7,8,9,10,11,12,13,14,15,16,17,19,20,21,22
        //<asp:ListItem Text="进行中的退票" Value="2"></asp:ListItem> 7,  
        //<asp:ListItem Text="进行中的废票" Value="3"></asp:ListItem>8,
        //<asp:ListItem Text="改签订单" Value="4"></asp:ListItem>6,9,15
        //<asp:ListItem Text="审核通过" Value="5"></asp:ListItem>9,11,13
        //<asp:ListItem Text="拒绝申请的订单" Value="6"></asp:ListItem>10,12,14,18
        //<asp:ListItem Text="交易结束" Value="7"></asp:ListItem> 16,17,19
        //<asp:ListItem Text="异地待退废改签订单" Value="8"></asp:ListItem> PolicySource>1 and OrderStatusCode in(6,7,8,9,10,11,12,13,14,15,16,17,19,20,21,22
        //<asp:ListItem Text="退款中的订单" Value="9"></asp:ListItem> 20,21,22
        //<asp:ListItem Text="审核中" Value="10"></asp:ListItem> 20,21,22

        if (Hid_num.Value == "1") //全部
            StrWhere.Append(" and OrderStatusCode in(6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,29,30,31) ");
        else if (Hid_num.Value == "2")
            StrWhere.Append(" and OrderStatusCode=7 ");
        else if (Hid_num.Value == "3")
            StrWhere.Append(" and OrderStatusCode=8 ");
        else if (Hid_num.Value == "4")
            StrWhere.Append(" and OrderStatusCode in(6,9,15,23,24) ");
        else if (Hid_num.Value == "5")
            StrWhere.Append(" and OrderStatusCode in(11,13) ");
        else if (Hid_num.Value == "6")
            StrWhere.Append(" and OrderStatusCode in(10,12,14,18,23,24) ");
        else if (Hid_num.Value == "7")
            StrWhere.Append(" and OrderStatusCode in(16,17,19,26) ");
        else if (Hid_num.Value == "8")
            StrWhere.Append(" and PolicySource>1 and OrderStatusCode in(6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,29,30,31) ");
        else if (Hid_num.Value == "9")
            StrWhere.Append(" and OrderStatusCode in(20,21,22,23) ");
        else if (Hid_num.Value == "10")
            StrWhere.Append(" and OrderStatusCode in(29,30,31) "); //审核中


        #endregion

        return StrWhere.ToString();
    }

    /// <summary>
    /// repList_ItemCommand
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void repList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        try
        {
            string Id = e.CommandArgument.ToString();
            string Url = Server.UrlEncode(string.Format("OrderTGQList.aspx?{0}", queryparam.Value));
            if (e.CommandName == "Detail") //订单详情
            {
                Response.Redirect("OrderDetail.aspx?Id=" + Id + "&currentuserid=" + this.currentuserid.Value.ToString() + "&Url=" + Url);
            }
            else if (e.CommandName == "GQOk")//确定改签
            {
                #region 判断订单 是否锁定

                List<Tb_Ticket_Order> mOrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { " id= '" + Id + "'" }) as List<Tb_Ticket_Order>;
                Tb_Ticket_Order Order = mOrderList[0];

                if (!string.IsNullOrEmpty(Order.LockLoginName) && !string.IsNullOrEmpty(Order.LockCpyNo))
                {
                    if (Order.LockLoginName.Trim() == mUser.LoginName.Trim())
                    {
                        //本账号锁定
                    }
                    else
                    {
                        //非本账号锁定
                        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('订单已被锁定,不能处理!');", true);
                        return;
                    }
                }
                else
                {
                    //锁定订单
                    new Tb_Ticket_OrderBLL().LockOrder(true, Order.id.ToString(), mUser, mCompany);
                }

                #endregion

                Response.Redirect("OrderGQSuccess.aspx?Id=" + Id + "&currentuserid=" + this.currentuserid.Value.ToString() + "&Url=" + Url);
            }
            else if (e.CommandName == "OrderProces" || e.CommandName == "TPExamine") // //订单处理 或者 退款
            {
                #region 判断订单 是否锁定

                List<Tb_Ticket_Order> mOrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { " id= '" + Id + "'" }) as List<Tb_Ticket_Order>;
                Tb_Ticket_Order Order = mOrderList[0];


                if (!string.IsNullOrEmpty(Order.LockLoginName) && Order.LockLoginName != mUser.LoginName)
                {
                    //订单锁定 并且 非登录本账号锁定
                    ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('订单已被锁定,不能处理！')", true);
                }
                else
                {
                    new Tb_Ticket_OrderBLL().LockOrder(true, Order.id.ToString(), mUser, mCompany);
                    Response.Redirect("TGQProcess.aspx?Id=" + Id + "&currentuserid=" + this.currentuserid.Value.ToString() + "&Url=" + Url);
                }

                #endregion
            }
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 绑定事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void repList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        try
        {
            //来源
            string hid_PolicySource = (e.Item.FindControl("hid_PolicySource") as HiddenField).Value.ToString();

            //支付状态 0未付，1已付
            string hid_PayStatus = (e.Item.FindControl("hid_PayStatus") as HiddenField).Value.ToString();

            //支付方式
            string hid_PayWay = (e.Item.FindControl("hid_PayWay") as HiddenField).Value.ToString();
            //默认隐藏所有  操作按钮
            string hid_OrderStatusCode = (e.Item.FindControl("hid_OrderStatusCode") as HiddenField).Value.ToString();


            LinkButton lbtnOrderProces = e.Item.FindControl("lbtnOrderProces") as LinkButton;
            LinkButton lbtnGQOK = e.Item.FindControl("lbtnGQOK") as LinkButton;
            LinkButton lbtnTPExamine = e.Item.FindControl("lbtnTPExamine") as LinkButton;


            //审核已经通过，直接退款
            if ((hid_OrderStatusCode == "11" || hid_OrderStatusCode == "13"))
            {
                lbtnTPExamine.Visible = true;//退款
            }
            else if ((hid_OrderStatusCode == "6" || hid_OrderStatusCode == "7" || hid_OrderStatusCode == "8"
               || hid_OrderStatusCode == "29" || hid_OrderStatusCode == "30" || hid_OrderStatusCode == "31") && hid_PayStatus == "1")
            {
                lbtnOrderProces.Visible = true;
            }
            else if (hid_OrderStatusCode == "15")
            {
                lbtnGQOK.Visible = true;//改签确认
            }

            if (lbtnOrderProces.Visible == true)
                lbtnOrderProces.Text += "<br />";
            if (lbtnGQOK.Visible == true)
                lbtnGQOK.Text += "<br />";
            if (lbtnTPExamine.Visible == true)
                lbtnTPExamine.Text += "<br />";
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="A40"></param>
    /// <returns></returns>
    public string GetPTTF(string A40)
    {
        string reson = "";
        if (A40 == "1")//自动退废票已经申请
        {
            reson = "<span style=\"color:Red\">全自动</span>";
        }
        else if (A40 == "2")
        {
            reson = "<span style=\"color:Red\">全自动失败</span>";
        }
        else if (A40 == "3")
        {
            reson = "<span style=\"color:Red\">平台已退废</span>";
        }
        else if (A40 == "4")
        {
            reson = "<span style=\"color:Red\">平台拒绝退废</span>";
        }
        return reson;
    }

    /// <summary>
    /// 计算平台返回订单状态
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public string ReturnPTorderStumesg(string code, string policySource)
    {
        //3	517
        //4	百拓
        //5	8000翼
        //6	今日
        //7	票盟
        //8	51book
        if (policySource == "3")//517
        {
            #region 517
            switch (code)
            {
                case "1": code = "新订单等待支付"; break;
                case "2": code = "采购商取消交易，交易结束"; break;
                case "3": code = "已经出票，交易结束"; break;
                case "4": code = "取消出票，等待退款"; break;
                case "5": code = "改签订单，等待审核"; break;
                case "6": code = "改签审核通过，机票被挂起，等待支付"; break;
                case "7": code = "已经付款，等待解挂"; break;
                case "8": code = "已经解挂，交易结束"; break;
                case "9": code = "改签订单审核不通过，交易结束"; break;
                case "10": code = "退票订单，等待审核"; break;
                case "11": code = "已经退款，交易结束"; break;
                case "12": code = "退票订单审核不通过，交易结束"; break;
                case "13": code = "废票订单，等待审核"; break;
                case "14": code = "审核通过，等待退款"; break;
                case "15": code = "废票订单审核不通过，交易结束"; break;
                case "16": code = "已经付款，等待出票"; break;
                case "17": code = "代付中"; break;
                case "18": code = "退款订单，延迟处理"; break;
                case "19": code = "线下订单待确认"; break;
                case "20": code = "线下订单审核不通过，交易结束"; break;
                case "21": code = "暂不能出票，等待处理"; break;
                case "22": code = "退票订单，提交到航空公司审核"; break;
                default: code = "新订单等待支付"; break;
            }
            #endregion
        }
        else if (policySource == "4")//百拓
        {
            #region 百拓
            switch (code)
            {
                case "1": code = "预订成功,等待采购方支付"; break;
                case "2": code = "支付成功,等待出票方出票"; break;
                case "3": code = "出票成功,等待采购方确认"; break;
                case "4": code = "出票成功,交易结束"; break;
                case "5": code = "采购方申请退票，等待出票方处理"; break;
                case "6": code = "采购方申请废票，等待出票方处理"; break;
                case "7": code = "采购方申请退票,待出票方退款"; break;
                case "8": code = "采购方申请废票,待出票方退款"; break;
                case "9": code = "出票方完成退款,交易结束"; break;
                case "10": code = "采购方已取消订单,交易结束"; break;
                case "11": code = "出票方已取消订单,交易结束"; break;
                case "12": code = "已提交平台处理,请等待平台回复"; break;
                case "13": code = "采购方申请改期,待出票方处理"; break;
                case "14": code = "改期完成,交易结束"; break;
                case "15": code = "废票办理完成,待出票方退款"; break;
                case "16": code = "退票办理完成,待出票方退款"; break;
                case "17": code = "直接取消订单,待出票方退款"; break;
                default: code = "预订成功,等待采购方支付"; break;
            }
            #endregion
        }
        else if (policySource == "5")//8000yi
        {
            #region 8000yi
            #endregion
        }
        else if (policySource == "6")//今日
        {
            #region 今日
            switch (code)
            {
                case "0": code = "等待支付"; break;
                case "1": code = "支付成功"; break;
                case "2": code = "出票完成"; break;
                case "3": code = "申请废票"; break;
                case "4": code = "申请退票"; break;
                case "5": code = "退款中"; break;
                case "6": code = "取消订单"; break;
                case "7": code = "暂不能出票"; break;
                case "8": code = "暂不能废票"; break;
                case "9": code = "暂不能退票"; break;
                case "10": code = "航班延误"; break;
                case "11": code = "航班取消"; break;
                case "12": code = "订单转移"; break;
                case "13": code = "取消成功"; break;
                case "14": code = "退款成功"; break;
                default: code = "等待支付"; break;
            }
            #endregion
        }
        else if (policySource == "7")//票盟
        {
            #region 票盟
            switch (code)
            {
                case "1": code = "尚未支付"; break;
                case "10": code = "等待出票"; break;
                case "11": code = "订单出票处理中"; break;
                case "12": code = "无法出票"; break;
                case "13": code = "出票完成"; break;
                case "14": code = "更换PNR出票完成"; break;
                case "20": code = "申请退票"; break;
                case "21": code = "退票处理中"; break;
                case "22": code = "无法退票"; break;
                case "30": code = "申请废票"; break;
                case "31": code = "废票处理中"; break;
                case "32": code = "无法废票"; break;
                case "40": code = "申请升舱改期"; break;
                case "41": code = "升舱改期处理中"; break;
                case "42": code = "无法改期升舱"; break;
                case "43": code = "完成改期升舱"; break;
                case "90": code = "完成退款"; break;
                case "99": code = "交易取消已退款"; break;
                default: code = "尚未支付"; break;
            }
            #endregion
        }
        else if (policySource == "8")//51book
        {
            #region 51book
            #endregion
        }

        return code;
    }

    /// <summary>
    /// btnQuery_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Curr = 1;
        AspNetPager1.CurrentPageIndex = Curr;
        Con = SelWhere();
        PageDataBind();
    }

    /// <summary>
    /// btnClear_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtCreateTime1.Value = DateTime.Now.ToShortDateString();
        txtCreateTime2.Value = DateTime.Now.ToShortDateString();
        txtFlightCode.Text = "";
        txtFromCity.Value = "";
        txtFromDate1.Value = "";
        txtFromDate2.Value = "";
        txtOrderId.Text = "";
        txtPassengerName.Text = "";
        txtPNR.Text = "";
        txtToCity.Value = "";
        rbtlOrderS.SelectedValue = "0";
        ddlStatus.SelectedIndex = 0;
        //SelectAirCode1.Value = "0";
    }

    /// <summary>
    /// 页面数据信息绑定
    /// </summary>
    /// <param name="ParentId">父类编号</param>
    /// <param name="ChildId">子类编号</param>
    /// <returns>返回处理后的相对应信息</returns>
    public string DataSourceMessage(int ParentId, object objValue)
    {
        string Message = "";

        string ChildId = (objValue != null) ? objValue.ToString() : "";

        try
        {
            if (ParentId == 3)
            {
                Message = ChildId == "1" ? "<span class=\"green\">已付</span>" : "<span class=\"red\">未付</span>";
            }
            else if (ParentId == 23)
            {
                Message = ChildId.Replace("|", "<br />");
            }
            else
            {
                Message = GetDictionaryName(ParentId.ToString(), ChildId);
                Message = string.Join("<br />", Message.Split(new string[] { ",", "，", " " }, StringSplitOptions.RemoveEmptyEntries));
            }
        }
        catch
        {
            return "";
        }
        return Message;
    }

    /// <summary>
    /// 显示颜色
    /// </summary>
    /// <param name="stringValue"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public string DataSourceColor(object objValue, int type)
    {
        string rsColor = "White";
        string stringValue = (objValue != null) ? objValue.ToString() : "";

        try
        {
            if (type == 1)
            {
                if (stringValue.Contains("|"))
                {
                    int j = 0;
                    string[] airValue = stringValue.Split('|');
                    for (int i = 0; i < airValue.Length - 1; i++)
                    {
                        if (DateTime.Parse(airValue[i].ToString()).ToString("yyyy--MM--dd").Contains(DateTime.Now.ToString("yyyy--MM--dd")))
                            j++;
                        else
                            j = 0;
                    }
                    if (j > 0)
                        rsColor = "Red";
                    else
                        rsColor = "White"; //Black
                }
            }
            else if (type == 2)
            {

                //<asp:ListItem Text="全部" Value="1"></asp:ListItem> 6,7,8,9,10,11,12,13,14,15,16,17,19,20,21,22
                //<asp:ListItem Text="进行中的退票" Value="2"></asp:ListItem> 7,  
                //<asp:ListItem Text="进行中的废票" Value="3"></asp:ListItem>8,
                //<asp:ListItem Text="进行中的改签" Value="4"></asp:ListItem>6,15
                //<asp:ListItem Text="审核通过" Value="5"></asp:ListItem>9,11,13
                //<asp:ListItem Text="拒绝申请的订单" Value="6"></asp:ListItem>10,12,14,18
                //<asp:ListItem Text="交易结束" Value="7"></asp:ListItem> 16,17,19
                //<asp:ListItem Text="异地待退废改签订单" Value="8"></asp:ListItem> PolicySource>1 and OrderStatusCode in(6,7,8,9,10,11,12,13,14,15,16,17,19,20,21,22
                //<asp:ListItem Text="退款中的订单" Value="9"></asp:ListItem> 20,21,22

                rsColor = "Black";
                if (stringValue == "10" || stringValue == "12" || stringValue == "14" || stringValue == "18" || stringValue == "16" || stringValue == "17")
                    rsColor = "Black"; //Black
                else if (stringValue == "15")
                    rsColor = "red"; //
                else
                    rsColor = "#DC5C17"; //Black
            }
        }
        catch (Exception ex)
        {

        }
        return rsColor;
    }

    /// <summary>
    /// 平台或航空公司是否退款
    /// </summary>
    /// <param name="P12"></param>
    /// <returns></returns>
    public string GetPlatStatus(string P12)
    {
        string reFound = "未退款";
        if (P12 == "1")
        {
            reFound = "已退款";
        }
        return reFound;
    }


    /// <summary>
    /// 显示金额
    /// </summary>
    /// <param name="orderMoney"></param>
    /// <returns></returns>
    public string GetPrice(object orderMoney, object ticketStatus)
    {
        string Message = "";
        try
        {
            Message = orderMoney != null ? orderMoney.ToString() : "0.00";

            if (Message != "0.00" && ticketStatus != null && (ticketStatus.ToString() == "3" || ticketStatus.ToString() == "4"))
            {
                Message = "-" + Message;
            }
        }
        catch (Exception ex)
        {

        }
        return Message;
    }
}