using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using PbProject.Dal.ControlBase;
using PbProject.Model;
using DataBase.Data;
using DataBase.Unique;
using PbProject.Logic.ControlBase;
using PbProject.Logic.Order;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using PbProject.Logic.PID;
using PbProject.WebCommon.Utility;
using System.Data;
using System.Xml;
using System.IO;

/// <summary>
/// 待出票订单列表
/// </summary>
public partial class Order_OrderProcessList : BasePage
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
                OrderStatusCodeBind();
                bindPolicySource();
                bindDictionary();
                Curr = 1;

                AspNetPager1.PageSize = 20;
                ViewState["orderBy"] = " PayTime ";

                //txtFromDate1.Value = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                //txtFromDate2.Value = DateTime.Now.ToString("yyyy-MM-dd");

                //txtCreateTime1.Value = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");

                txtCreateTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                txtCreateTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");

                //Con = SelWhere();
                //用于订单提醒
                showPrompt();

                if (mCompany.RoleType == 2)
                {
                    orderstatus4.Visible = true;
                }
                else if (mCompany.RoleType == 3)
                {
                    orderstatus4.Visible = false; ;
                }
            }
        }
        catch (Exception) { }
    }
    public string ShowAdult(object AssociationOrder, object IsChdFlag)
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
    /// 用于 订单提醒查询
    /// </summary>
    public void showPrompt()
    {
        if (Request["prompt"] != null && Request["prompt"].ToString() == "1")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "RequestQuery('" + Request["prompt"].ToString() + "');", true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected string Con
    {
        get { return (string)ViewState["Con"]; }
        set { ViewState["Con"] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    protected int Curr
    {
        get { return (int)ViewState["currpage"]; }
        set { ViewState["currpage"] = value; }
    }

    /// <summary>
    /// 订单绑定
    /// </summary>
    private void OrderListDataBind()
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
                new object[] { TotalCount, AspNetPager1.PageSize, Curr, "DATEDIFF(MINUTE, PayTime, getdate()) OrderLeayTime,*", Con, ViewState["orderBy"].ToString() }) as List<Tb_Ticket_Order>;

            TotalCount = outParams.GetValue<int>("1");

            AspNetPager1.RecordCount = TotalCount;
            AspNetPager1.CurrentPageIndex = Curr;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;

            repOrderList.DataSource = list;
            repOrderList.DataBind();
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    private string SelWhere()
    {

        StringBuilder StrWhere = new StringBuilder(" 1=1 ");
        StrWhere.Append(" and OrderStatusCode=3 "); //订单状态
        //StrWhere.Append(" and CPCpyNo='" + mUser.CpyNo + "' "); //本地

        if (mCompany.RoleType == 2)
        {
            StrWhere.Append(" and (CPCpyNo='" + mUser.CpyNo + "' or left(OwnerCpyNo,12)= '" + mUser.CpyNo + "' )"); //可查询共享
        }
        else if (mCompany.RoleType == 3)
        {
            StrWhere.Append(" and CPCpyNo='" + mUser.CpyNo + "' ");
        }
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
            //客户名称
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtCorporationName.Text.Trim())))
                StrWhere.Append(" and CreateCpyName like'%" + CommonManage.TrimSQL(txtCorporationName.Text.Trim()) + "%' ");
            //航班号
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(txtFlightCode.Text.Trim())))
                StrWhere.Append(" and FlightCode ='" + CommonManage.TrimSQL(txtFlightCode.Text.Trim()) + "' ");
            //航空公司
            if (!string.IsNullOrEmpty(CommonManage.TrimSQL(SelectAirCode1.Value.Trim())))
                StrWhere.Append(" and CarryCode ='" + CommonManage.TrimSQL(SelectAirCode1.Value.Trim()) + "' ");

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
        }
        catch (Exception)
        {

        }

        #region 查询条件

        ViewState["selCount"] = StrWhere.ToString();

        if (ViewState["selType"] != null && !string.IsNullOrEmpty(ViewState["selType"].ToString()))
        {
            //1 本地B2B, 2 本地BSP,3 517,4 百拓,5 8000翼,6 今日,7 票盟,8 51book ,9 共享,10易行
            string selType = ViewState["selType"].ToString();
            if (selType == "0")
                selType = "";
            else if (selType == "1")
                selType = " and (PolicySource=1 or (PolicySource=9 and PolicyType=1 and CPCpyNo='" + mUser.CpyNo + "'))";
            else if (selType == "2")
                selType = " and (PolicySource=2 or (PolicySource=9 and PolicyType=2 and CPCpyNo='" + mUser.CpyNo + "'))";
            else if (selType == "3")
                selType = " and PolicySource in (3,4,5,6,7,8,10) ";
            else if (selType == "4")
                selType = " and (PolicySource=9 and CPCpyNo!='" + mUser.CpyNo + "')";

            StrWhere.Append(selType);
        }


        #endregion

        return StrWhere.ToString();
    }

    #region 绑定事件

    /// <summary>
    /// 订单状态
    /// </summary>
    private void OrderStatusCodeBind()
    {
        try
        {
            //List<Bd_Base_Dictionary> bDictionaryList = GetDictionaryList("1");

            //if (bDictionaryList != null && bDictionaryList.Count > 0)
            //{
            //    ddlStatus.DataSource = bDictionaryList;
            //    ddlStatus.DataTextField = "ChildName";
            //    ddlStatus.DataValueField = "ChildID";
            //    ddlStatus.DataBind();
            //}

            //ddlStatus.Items.Insert(0, new ListItem("全部状态", "0"));
            //ddlStatus.Items[0].Selected = true;


            ddlStatus.Items.Insert(0, new ListItem("已经支付,等待出票 ", "3"));
            ddlStatus.Items[0].Selected = true;
            ddlStatus.Enabled = false;
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// bindDictionary
    /// </summary>
    public void bindDictionary()
    {
        try
        {
            List<Bd_Base_Dictionary> mDictionary = GetDictionaryList("29");

            string strValue = "<option value='' check='true'>--请选择理由--</option>";
            for (int i = 0; i < mDictionary.Count; i++)
            {
                if (mDictionary[i].ParentName == "取消出票理由")
                {
                    strValue += "<option value='" + mDictionary[i].ChildName + "' check='true'>" + mDictionary[i].ChildName + "</option>";
                }
            }
            if (strValue == "<option value='' check='true'>--请选择理由--</option>")
            {
                strValue = "<option value='' check='true'>--请选择理由--</option><option value='网络故障！' check='true'>网络故障！</option>";
            }
            hid_SelValues.Value = strValue;
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 翻页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        try
        {
            Curr = e.NewPageIndex;
            OrderListDataBind();
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// repCabinList_ItemCommand
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void repOrderList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        Tb_Ticket_Order Order = null;
        Tb_Ticket_OrderBLL OrderBLL = new Tb_Ticket_OrderBLL();
        try
        {
            if (e.CommandArgument == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('无此订单!');", true);
                return;
            }
            string Id = e.CommandArgument.ToString().Replace("\'", "");  //订单 id 
            //获取相应订单
            List<Tb_Ticket_Order> mOrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { " id= '" + Id + "'" }) as List<Tb_Ticket_Order>;
            if (mOrderList != null && mOrderList.Count > 0)
            {
                Order = mOrderList[0];
            }
            //锁定订单。。。。           
            //if (OrderBLL.LockOrder(true, Order.id.ToString(), mUser, mCompany))
            //{

            bool reuslt = false;
            string errtitle = "";

            if (e.CommandName == "Detail") //订单详情
            {
                Response.Redirect("OrderDetail.aspx?Id=" + Id + "&Url=OrderList.aspx&currentuserid=" + this.currentuserid.Value.ToString());
            }
            else if (e.CommandName == "OrderOp") // 订单处理
            {
                #region OrderOp 订单处理

                //List<Tb_Ticket_Order> mOrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { " id= '" + Id + "'" }) as List<Tb_Ticket_Order>;
                //Tb_Ticket_Order Order = mOrderList[0];

                ViewState["Order"] = Order;

                if (Order.OrderStatusCode == 3)
                {
                    #region 判断订单 是否锁定

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
                        bool lockOrder = new Tb_Ticket_OrderBLL().LockOrder(true, Order.id.ToString(), mUser, mCompany);

                        if (lockOrder == true)
                        {
                            Label lblLockLoginName = e.Item.FindControl("lblLockLoginName") as Label;
                            lblLockLoginName.Text = mUser.LoginName;
                        }

                    }
                    #endregion

                    if (true) // 订单处理
                    {
                        #region OrderOp 订单处理 (显示处理界面)

                        #region 绑定显示选择中 订单 的乘机人

                        List<Tb_Ticket_Passenger> PassengerList = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new Object[] { " OrderId= '" + Order.OrderId + "'" }) as List<Tb_Ticket_Passenger>;

                        ViewState["PassengerList"] = PassengerList;

                        HtmlGenericControl htmlg = new HtmlGenericControl();
                        for (int i = 0; i < repOrderList.Items.Count; i++)
                        {
                            htmlg = (repOrderList.Items[i].FindControl("divRepeater")) as HtmlGenericControl;
                            htmlg.Attributes.CssStyle.Clear();
                            htmlg.Attributes.CssStyle.Add("display", "none");
                        }
                        htmlg = (e.Item.FindControl("divRepeater")) as HtmlGenericControl;
                        htmlg.Attributes.CssStyle.Clear();
                        htmlg.Attributes.CssStyle.Add("display", "block");

                        Repeater RepPassenger = (e.Item.FindControl("RepPassenger") as Repeater);
                        RepPassenger.DataSource = PassengerList;
                        RepPassenger.DataBind();

                        //处理证件类型
                        List<Bd_Base_Dictionary> DictionaryList = GetDictionaryList("7");
                        Tb_Ticket_Passenger Passenger = new Tb_Ticket_Passenger();
                        for (int i = 0; i < RepPassenger.Items.Count; i++)
                        {
                            TextBox txtCid = (TextBox)RepPassenger.Items[i].FindControl("txtCid");
                            DropDownList ddlCType = (DropDownList)RepPassenger.Items[i].FindControl("ddlCType");
                            ddlCType.DataSource = DictionaryList;
                            ddlCType.DataTextField = "ChildName";
                            ddlCType.DataValueField = "ChildId";
                            ddlCType.DataBind();
                            ddlCType.SelectedValue = PassengerList[i].CType.ToString();
                            //证件号为空 可以修改
                            if (txtCid.Text.Trim() == "")
                            {
                                txtCid.Enabled = true;
                            }
                            else
                            {
                                txtCid.Enabled = false;
                            }
                        }
                        #endregion

                        #region 支付方式

                        DropDownList ddlPayWay = (e.Item.FindControl("ddlPayWay")) as DropDownList; //支付方式
                        ddlPayWay.Items.Clear();
                        DictionaryList = GetDictionaryList("4");

                        ddlPayWay.DataSource = DictionaryList;
                        ddlPayWay.DataTextField = "ChildName";
                        ddlPayWay.DataValueField = "ChildID";
                        ddlPayWay.DataBind();

                        ddlPayWay.SelectedValue = Order.PayWay.ToString();

                        #endregion

                        #region 政策来源

                        DropDownList ddlPolicySource = (e.Item.FindControl("ddlPolicySource")) as DropDownList; //政策来源
                        ddlPolicySource.Items.Clear();
                        DictionaryList = GetDictionaryList("24");

                        ddlPolicySource.DataSource = DictionaryList;
                        ddlPolicySource.DataTextField = "ChildName";
                        ddlPolicySource.DataValueField = "ChildID";
                        ddlPolicySource.DataBind();

                        ddlPolicySource.SelectedValue = Order.PolicySource.ToString();


                        Label lblPolicySource = (e.Item.FindControl("lblPolicySource")) as Label; //政策来源
                        if (Order.PolicySource == 9)
                        {
                            ddlPolicySource.Visible = false;
                            lblPolicySource.Visible = true;
                            lblPolicySource.Text = GetStrValue("24", Order.PolicySource.ToString(), Order.CPCpyNo, Order.PolicyType.ToString());
                        }
                        else
                        {
                            ddlPolicySource.Visible = true;
                            lblPolicySource.Visible = false;
                        }

                        #endregion

                        //代付返点
                        TextBox txtAirlinePReturn = (e.Item.FindControl("txtAirlinePReturn")) as TextBox;
                        txtAirlinePReturn.Text = Order.A7.ToString();


                        //代付返点
                        TextBox txtOutOrderId = (e.Item.FindControl("txtOutOrderId")) as TextBox;
                        txtOutOrderId.Text = Order.OutOrderId.ToString();

                        //代付金额
                        TextBox txtCgFeeBB = (e.Item.FindControl("txtCgFeeBB")) as TextBox;
                        txtCgFeeBB.Text = Order.OutOrderPayMoney.ToString();

                        // PNR 和 大编
                        TextBox txtPnr = (e.Item.FindControl("txtPnr")) as TextBox;
                        TextBox txtBigPNR = (e.Item.FindControl("txtBigPNR")) as TextBox;
                        if (Order.PNR == "")
                        {
                            //C站订单 pnr 为空
                            txtPnr.Text = "";
                            txtPnr.Enabled = true;
                        }
                        else
                        {
                            txtPnr.Text = Order.PNR;
                            txtBigPNR.Text = Order.BigCode;
                            txtPnr.Enabled = false;
                        }

                        Label lblPnrNew = (e.Item.FindControl("lblPnrNew")) as Label;
                        TextBox txtPnrNew = (e.Item.FindControl("txtPnrNew")) as TextBox;

                        //换编码
                        if (Order.AllowChangePNRFlag == true)
                        {
                            //可以换编码
                            txtPnrNew.Text = Order.ChangePNR;
                            txtPnrNew.Visible = true;
                        }
                        else
                        {
                            //不能换编码
                            lblPnrNew.Text = "不可换编码";

                            txtPnrNew.Visible = false;
                        }

                        //出票备注
                        if (!string.IsNullOrEmpty(Order.CPRemark))
                        {
                            TextBox txtRemak = (e.Item.FindControl("txtRemak")) as TextBox;
                            txtRemak.Text = Order.CPRemark; //出票备注
                        }

                        //出票人
                        TextBox txtCpNameBB = (e.Item.FindControl("txtCpNameBB")) as TextBox;
                        txtCpNameBB.Text = mUser.UserName;

                        #region 获取乘机人信息


                        string strValueS = "";

                        foreach (Tb_Ticket_Passenger item in PassengerList)
                        {
                            // 成人
                            strValueS += item.PassengerType + "^" + item.PMFee + "^" + item.ABFee + "^" + item.FuelFee + "|";
                        }

                        strValueS = strValueS.TrimEnd('|');

                        HiddenField hidValPriceSVal = (e.Item.FindControl("hidValPriceSVal")) as HiddenField;
                        hidValPriceSVal.Value = strValueS;

                        #endregion

                        #region 出票方式
                        //Label lblPol = (e.Item.FindControl("lblPol")) as Label;
                        //DropDownList ddlPol = (e.Item.FindControl("ddlPol")) as DropDownList;
                        //if (Order.PolicySource > 1)
                        //{
                        //    lblPol.Visible = false;
                        //    ddlPol.Visible = false;
                        //}
                        //else
                        //{
                        //    lblPol.Visible = false;
                        //    ddlPol.Visible = false;
                        //    //ddlPol.SelectedValue = Order.PolicySource.ToString();
                        //}
                        #endregion

                        #endregion
                    }
                    else
                    {
                        //跳转页面处理
                        Response.Redirect("OrderProcess.aspx?Id=" + Id + "&Type=1&Url=OrderProcessList.aspx&currentuserid=" + this.currentuserid.Value.ToString()); //跳转页面处理
                    }
                }
                else
                {
                    // 
                    //绑定页面
                    OrderListDataBind();
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('该订单已经处理中!');", true);
                }
                #endregion
            }
            else if (e.CommandName == "Close")
            {
                #region Close 关闭并解锁
                HtmlGenericControl htmlg = new HtmlGenericControl();

                htmlg = (e.Item.FindControl("divRepeater")) as HtmlGenericControl;
                htmlg.Attributes.CssStyle.Clear();
                htmlg.Attributes.CssStyle.Add("display", "none");
                //解锁
                new Tb_Ticket_OrderBLL().LockOrder(false, Id, mUser, mCompany);

                OrderListDataBind();
                #endregion
            }
            else if (e.CommandName == "Save")
            {
                #region Save 保存并解锁

                TextBox txtRemak = (e.Item.FindControl("txtRemak")) as TextBox;

                if (Order.AllowChangePNRFlag == true)
                {
                    //可以换编码
                    TextBox txtPnrNew = (e.Item.FindControl("txtPnrNew")) as TextBox;
                    Order.ChangePNR = txtPnrNew.Text;
                }

                //政策返点
                TextBox txtAirlinePReturn = (e.Item.FindControl("txtAirlinePReturn")) as TextBox;
                decimal _PolicyPoint = Order.A7;
                if (decimal.TryParse(txtAirlinePReturn.Text.Trim(), out _PolicyPoint))
                {
                    Order.A7 = _PolicyPoint;
                }

                //代付金额
                TextBox txtCgFeeBB = (e.Item.FindControl("txtCgFeeBB")) as TextBox;
                decimal _daiFuMoney = Order.OutOrderPayMoney;
                if (decimal.TryParse(txtCgFeeBB.Text.Trim(), out _daiFuMoney))
                {
                    Order.OutOrderPayMoney = _daiFuMoney;
                }

                if (txtRemak != null && !string.IsNullOrEmpty(txtRemak.Text))
                {
                    if (txtRemak.Text.Length > 200)
                        errtitle = "备注不能太长，不能超过200个字符!";
                    else
                        Order.CPRemark = txtRemak.Text; //出票备注
                }

                //外部订单号
                TextBox txtOutOrderId = (e.Item.FindControl("txtOutOrderId")) as TextBox;
                Order.OutOrderId = txtOutOrderId.Text.Trim();

                if (errtitle == "")
                {
                    //政策来源
                    Order.PolicySource = int.Parse(((e.Item.FindControl("ddlPolicySource")) as DropDownList).SelectedValue);

                    #region 出票修改信息日志

                    string contentLog = "出票修改信息:";

                    Tb_Ticket_Order OldOrder = ViewState["Order"] as Tb_Ticket_Order;

                    if (OldOrder != null && Order != null)
                    {
                        //原换编码
                        if (!string.IsNullOrEmpty(OldOrder.ChangePNR) && OldOrder.ChangePNR != Order.ChangePNR)
                        {
                            contentLog += "原换编码:" + OldOrder.ChangePNR + "新换编码:" + Order.ChangePNR;
                        }
                        //代付返点
                        if (OldOrder.A7 != Order.A7)
                        {
                            contentLog += "原代付返点:" + OldOrder.A7 + "新代付返点:" + Order.A7;
                        }
                        //代付金额
                        if (OldOrder.OutOrderPayMoney != Order.OutOrderPayMoney)
                        {
                            contentLog += "原代付金额:" + OldOrder.OutOrderPayMoney + "新代付金额:" + Order.OutOrderPayMoney;
                        }
                        //政策来源
                        if (OldOrder.PolicySource != Order.PolicySource)
                        {
                            contentLog += "原政策来源:" + GetDictionaryName("24", OldOrder.PolicySource.ToString()) + "新政策来源:" + GetDictionaryName("24", OldOrder.PolicySource.ToString());
                        }

                        //外部订单号
                        if (!string.IsNullOrEmpty(OldOrder.OutOrderId) && OldOrder.OutOrderId != Order.OutOrderId)
                        {
                            contentLog += "原外部订单号:" + OldOrder.OutOrderId + "新外部订单号:" + Order.OutOrderId;
                        }

                        //出票备注
                        if (!string.IsNullOrEmpty(OldOrder.CPRemark) && OldOrder.CPRemark != Order.CPRemark)
                        {
                            contentLog += "原出票备注:" + OldOrder.CPRemark + "新出票备注:" + Order.CPRemark;
                        }
                    }

                    #endregion

                    reuslt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().OperOrderCPSave(Order, mUser, mCompany, contentLog);

                    if (reuslt)
                    {
                        OrderListDataBind(); //成功 刷新页面
                        errtitle = "保存成功并解锁！";
                    }
                    else
                    {
                        errtitle = "出票失败,订单已被锁定!";
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + errtitle + "');", true);

                #endregion
            }
            else if (e.CommandName == "CP")
            {
                #region CP 出票处理

                //Response.Redirect("OrderProcess.aspx?Id=" + Id + "&Type=1&Url=OrderProcessList.aspx"); //跳转页面处理
                //List<Tb_Ticket_Order> mOrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { " id= '" + Id + "'" }) as List<Tb_Ticket_Order>;
                //Tb_Ticket_Order Order = mOrderList[0];

                List<Tb_Ticket_Passenger> passengerList = ViewState["PassengerList"] as List<Tb_Ticket_Passenger>;

                //出票处理 
                Repeater RepPassenger = (e.Item.FindControl("RepPassenger") as Repeater);

                string tempTickeTnum = "";
                string tempCid = "";
                string tempPassengerID = "";
                string tempTickeTnumS = "";

                #region 验证票号是否正确

                for (int i = 0; i < RepPassenger.Items.Count; i++)
                {
                    //证件号验证
                    tempCid = ((TextBox)RepPassenger.Items[i].FindControl("txtCid")).Text.Trim();
                    tempPassengerID = ((HiddenField)RepPassenger.Items[i].FindControl("Hid_PassengerID")).Value.Trim();

                    if (string.IsNullOrEmpty(tempCid))
                    {
                        errtitle = "证件号不能为空！";
                        break;
                    }
                    tempTickeTnum = ((TextBox)RepPassenger.Items[i].FindControl("txtTicketNumber")).Text.Trim().Replace("-", "");

                    #region 判断票号重复

                    if (tempTickeTnumS.Contains("|" + tempTickeTnum + "|"))
                    {
                        errtitle = "票号不能重复！";
                        break;
                    }
                    else
                    {
                        tempTickeTnumS += "|" + tempTickeTnum + "|";
                    }

                    #endregion

                    if (tempTickeTnum.Length < 13)
                    {
                        errtitle = "票号位数不正确";
                        break;
                    }
                    else
                    {
                        if (!Regex.IsMatch(tempTickeTnum, @"^\d+$"))
                        {
                            errtitle = "票号错误,订单已被锁定!";
                            break;
                        }
                        else
                        {
                            for (int j = 0; j < passengerList.Count; j++)
                            {
                                if (passengerList[i].id.ToString() == tempPassengerID)
                                {
                                    passengerList[i].TicketNumber = tempTickeTnum;
                                    passengerList[i].Cid = tempCid;
                                    break;
                                }
                            }
                        }
                    }
                }

                #endregion

                TextBox txtRemak = (e.Item.FindControl("txtRemak")) as TextBox;

                if (Order.AllowChangePNRFlag == true)
                {
                    //可以换编码
                    TextBox txtPnrNew = (e.Item.FindControl("txtPnrNew")) as TextBox;
                    Order.ChangePNR = txtPnrNew.Text;
                }

                //政策返点
                TextBox txtAirlinePReturn = (e.Item.FindControl("txtAirlinePReturn")) as TextBox;
                decimal _PolicyPoint = Order.A7;
                if (decimal.TryParse(txtAirlinePReturn.Text.Trim(), out _PolicyPoint))
                {
                    Order.A7 = _PolicyPoint;
                }

                //外部订单号
                TextBox txtOutOrderId = (e.Item.FindControl("txtOutOrderId")) as TextBox;
                Order.OutOrderId = txtOutOrderId.Text.Trim();

                //代付金额
                TextBox txtCgFeeBB = (e.Item.FindControl("txtCgFeeBB")) as TextBox;
                decimal _daiFuMoney = Order.OutOrderPayMoney;
                if (decimal.TryParse(txtCgFeeBB.Text.Trim(), out _daiFuMoney))
                {
                    Order.OutOrderPayMoney = _daiFuMoney;
                }

                if (txtRemak != null && !string.IsNullOrEmpty(txtRemak.Text))
                {
                    if (txtRemak.Text.Length > 200)
                        errtitle = "备注不能太长，不能超过200个字符!";
                    else
                        Order.CPRemark = txtRemak.Text; //出票备注
                }

                // C站订单处理  PNR 未空  
                if (string.IsNullOrEmpty(Order.PNR))
                    Order.PNR = ((e.Item.FindControl("txtPnr")) as TextBox).Text;

                if (errtitle == "")
                {
                    Order.OrderStatusCode = 4; //出票状态

                    //政策来源
                    Order.PolicySource = int.Parse(((e.Item.FindControl("ddlPolicySource")) as DropDownList).SelectedValue);

                    #region 出票修改信息日志

                    string contentLog = "";

                    Tb_Ticket_Order OldOrder = ViewState["Order"] as Tb_Ticket_Order;

                    if (OldOrder != null && Order != null)
                    {
                        //原换编码
                        if (!string.IsNullOrEmpty(OldOrder.ChangePNR) && OldOrder.ChangePNR != Order.ChangePNR)
                        {
                            contentLog += "原换编码:" + OldOrder.ChangePNR + "新换编码:" + Order.ChangePNR;
                        }
                        //代付返点
                        if (OldOrder.A7 != Order.A7)
                        {
                            contentLog += "原代付返点:" + OldOrder.A7 + "新代付返点:" + Order.A7;
                        }
                        //代付金额
                        if (OldOrder.OutOrderPayMoney != Order.OutOrderPayMoney)
                        {
                            contentLog += "原代付金额:" + OldOrder.OutOrderPayMoney + "新代付金额:" + Order.OutOrderPayMoney;
                        }
                        //政策来源
                        if (OldOrder.PolicySource != Order.PolicySource)
                        {
                            contentLog += "原政策来源:" + GetDictionaryName("24", OldOrder.PolicySource.ToString()) + "新政策来源:" + GetDictionaryName("24", OldOrder.PolicySource.ToString());
                        }
                        //出票备注
                        if (!string.IsNullOrEmpty(OldOrder.CPRemark) && OldOrder.CPRemark != Order.CPRemark)
                        {
                            contentLog += "原出票备注:" + OldOrder.CPRemark + "新出票备注:" + Order.CPRemark;
                        }
                    }

                    if (!string.IsNullOrEmpty(contentLog))
                    {
                        contentLog = "出票修改信息:" + contentLog;
                    }

                    #endregion

                    reuslt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().OperOrderCP(Order, passengerList, mUser, mCompany, contentLog);

                    if (reuslt)
                    {
                        errtitle = "出票成功!";

                        #region 出票成功自动发送短信

                        try
                        {

                        }
                        catch (Exception ex)
                        {

                        }
                        #endregion

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

                        OrderListDataBind(); //出票成功 刷新页面
                    }
                    else
                    {
                        errtitle = "出票失败,订单已被锁定!";
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + errtitle + "');", true);

                #endregion
            }
            else if (e.CommandName == "FH")
            {
                #region 自动复合

                TextBox txtRemak = (e.Item.FindControl("txtRemak")) as TextBox;

                if (Order.AllowChangePNRFlag == true)
                {
                    //可以换编码
                    TextBox txtPnrNew = (e.Item.FindControl("txtPnrNew")) as TextBox;
                    Order.ChangePNR = txtPnrNew.Text;
                }

                //政策返点
                TextBox txtAirlinePReturn = (e.Item.FindControl("txtAirlinePReturn")) as TextBox;
                decimal _PolicyPoint = Order.A7;
                if (decimal.TryParse(txtAirlinePReturn.Text.Trim(), out _PolicyPoint))
                {
                    Order.A7 = _PolicyPoint;
                }

                //外部订单号
                TextBox txtOutOrderId = (e.Item.FindControl("txtOutOrderId")) as TextBox;
                Order.OutOrderId = txtOutOrderId.Text.Trim();

                //代付金额
                TextBox txtCgFeeBB = (e.Item.FindControl("txtCgFeeBB")) as TextBox;
                decimal _daiFuMoney = Order.OutOrderPayMoney;
                if (decimal.TryParse(txtCgFeeBB.Text.Trim(), out _daiFuMoney))
                {
                    Order.OutOrderPayMoney = _daiFuMoney;
                }

                if (txtRemak != null && !string.IsNullOrEmpty(txtRemak.Text))
                {
                    if (txtRemak.Text.Length > 200)
                        errtitle = "备注不能太长，不能超过200个字符!";
                    else
                        Order.CPRemark = txtRemak.Text; //出票备注
                }

                //政策来源
                Order.PolicySource = int.Parse(((e.Item.FindControl("ddlPolicySource")) as DropDownList).SelectedValue);

                string ErrMsg = "";

                bool IsSuc = false;

                if (Order != null)
                {
                    if (Order.PolicySource > 2)
                    {
                        IsSuc = OrderFH(Order, out ErrMsg);
                    }
                    else
                    {
                        IsSuc = AutoFHTicket(Order, out ErrMsg);
                    }
                    if (IsSuc)
                    {

                        #region 出票修改信息日志

                        string contentLog = "";

                        Tb_Ticket_Order OldOrder = ViewState["Order"] as Tb_Ticket_Order;

                        if (OldOrder != null && Order != null)
                        {
                            //原换编码
                            if (!string.IsNullOrEmpty(OldOrder.ChangePNR) && OldOrder.ChangePNR != Order.ChangePNR)
                            {
                                contentLog += "原换编码:" + OldOrder.ChangePNR + "新换编码:" + Order.ChangePNR;
                            }
                            //代付返点
                            if (OldOrder.A7 != Order.A7)
                            {
                                contentLog += "原代付返点:" + OldOrder.A7 + "新代付返点:" + Order.A7;
                            }
                            //代付金额
                            if (OldOrder.OutOrderPayMoney != Order.OutOrderPayMoney)
                            {
                                contentLog += "原代付金额:" + OldOrder.OutOrderPayMoney + "新代付金额:" + Order.OutOrderPayMoney;
                            }
                            //政策来源
                            if (OldOrder.PolicySource != Order.PolicySource)
                            {
                                contentLog += "原政策来源:" + GetDictionaryName("24", OldOrder.PolicySource.ToString()) + "新政策来源:" + GetDictionaryName("24", OldOrder.PolicySource.ToString());
                            }
                            //出票备注
                            if (!string.IsNullOrEmpty(OldOrder.CPRemark) && OldOrder.CPRemark != Order.CPRemark)
                            {
                                contentLog += "原出票备注:" + OldOrder.CPRemark + "新出票备注:" + Order.CPRemark;
                            }
                        }

                        if (!string.IsNullOrEmpty(contentLog))
                        {
                            contentLog = "自动复合修改信息:" + contentLog;
                            new PbProject.Logic.Order.Tb_Ticket_OrderBLL().CreateOrderLog(mUser, mCompany, Order.OrderId, mCompany.RoleType, contentLog);
                        }

                        #endregion

                        //绑定页面
                        OrderListDataBind();
                        ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + ErrMsg + "');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('自动复合失败,原因如下:<br />" + ErrMsg + "');", true);
                        new PbProject.Logic.Order.Tb_Ticket_OrderBLL().CreateOrderLog(mUser, mCompany, Order.OrderId, mCompany.RoleType, ErrMsg);

                    }


                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('自动复合失败,原因如下:<br />订单不存在');", true);
                }
                #endregion
            }
            else if (e.CommandName == "DetailByJK")//状态查询
            {
                #region DetailByJK 状态查询
                InterFaceStu(Order);
                OrderListDataBind();
                #endregion
            }
            else if (e.CommandName == "Pay")//代付
            {
                #region Pay
                List<PbProject.Model.Bd_Base_Parameters> mBP = new PbProject.Logic.ControlBase.BaseDataManage().
               CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + Order.OwnerCpyNo.Substring(0, 12) + "'" }) as List<PbProject.Model.Bd_Base_Parameters>;

                bool IsOk = false;
                if (Order.PolicySource == 3)//517政策
                {
                    IsOk = PayBy517(Order, mBP);
                }
                if (Order.PolicySource == 4)
                {
                    IsOk = BaiTuoPay(Order, mBP);//百拓
                }
                if (Order.PolicySource == 7)
                {
                    IsOk = PMPay(Order, mBP);//票盟
                }
                if (Order.PolicySource == 8)
                {
                    IsOk = bookPay(Order, mBP);//51book
                }
                if (Order.PolicySource == 5)
                {
                    IsOk = PayFor8000Y(Order, mBP);
                }
                if (Order.PolicySource == 6)
                {
                    IsOk = PayForToday(Order, mBP);
                }
                if (Order.PolicySource == 10)
                {
                    IsOk = PayByYeeXing(Order, mBP);
                }
                OrderListDataBind();
                if (IsOk == true)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('订单代付已申请!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('订单代付申请失败!');", true);
                }
                #endregion
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('无此操作!');", true);
            }
            //}
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('操作错误!');", true);
        }
        finally
        {
            //解锁订单
            //OrderBLL.LockOrder(false, Order.id.ToString(), mUser, mCompany);
        }
    }

    /// <summary>
    /// 自动复合(本地订单复合)
    /// </summary>
    /// <param name="Order">订单实体</param>
    /// <param name="ErrMsg">出错信息</param>
    /// <returns></returns>
    public bool AutoFHTicket(Tb_Ticket_Order Order, out string ErrMsg)
    {
        //出错信息
        ErrMsg = "";
        //自动复合成功失败标识
        bool IsSuc = false;
        try
        {
            if (Order != null)
            {
                #region 本地自动复合
                ConfigParam config = null;
                if (mCompany.RoleType == 1)
                {
                    List<Bd_Base_Parameters> baseParamList = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + Order.CreateCpyNo + "'" }) as List<Bd_Base_Parameters>;
                    config = Bd_Base_ParametersBLL.GetConfigParam(baseParamList);
                }
                else
                {
                    config = this.configparam;
                }
                string SqlWhere = string.Format(" OrderId ='{0}' ", Order.OrderId);
                List<Tb_Ticket_Passenger> pasList = this.baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new object[] { SqlWhere }) as List<Tb_Ticket_Passenger>;
                //扩展参数
                ParamEx pe = new ParamEx();
                pe.UsePIDChannel = this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|48|") ? 2 : 0;
                //发送指令的账号和公司编号 配置信息
                SendInsManage SendManage = new SendInsManage(mUser.LoginName, mCompany.UninCode, pe, config);
                if (!string.IsNullOrEmpty(Order.PNR))
                {
                    #region 回帖票号到乘机人
                    //获取编码信息
                    PnrAnalysis.PnrModel PnrModel = SendManage.GetPnr(Order.PNR, Order.Office, out ErrMsg);
                    if (PnrModel == null)
                    {
                        ErrMsg = "自动复核失败：" + ErrMsg;
                        return IsSuc;
                    }
                    List<string> sqlList = new List<string>();
                    foreach (PnrAnalysis.Model.TicketNumInfo TN in PnrModel._TicketNumList)
                    {
                        Tb_Ticket_Passenger pas = pasList.Find(delegate(Tb_Ticket_Passenger Pas)
                        {
                            return Pas.PassengerName.Trim().ToUpper() == TN.PasName.Trim().ToUpper();
                        });
                        if (pas != null)
                        {
                            //对应乘机人填充票号
                            sqlList.Add(string.Format(" update  Tb_Ticket_Passenger set TicketStatus=2,TicketNumber='{0}' where id='{1}' ", TN.TicketNum, pas.id.ToString()));
                        }
                    }
                    #endregion

                    int count = sqlList != null && sqlList.Count > 0 ? sqlList.Count : 0;

                    if (count > 0)
                    {
                        #region 1.修改订单

                        StringBuilder updateOrder = new StringBuilder();
                        updateOrder.Append(" update Tb_Ticket_Order set ");
                        updateOrder.Append(" LockCpyNo='',");//锁定帐号所属公司编号
                        updateOrder.Append(" LockLoginName='',");//锁定帐户
                        updateOrder.Append(" LockTime='1900-1-1',");//锁定时间
                        updateOrder.Append(" TicketStatus=2,");//机票状态
                        if (Order.AllowChangePNRFlag == true)
                        {
                            updateOrder.Append(" ChangePNR='" + Order.ChangePNR + "',");//可以换编码
                        }
                        updateOrder.Append(" OutOrderPayMoney=" + Order.OutOrderPayMoney + ",");//代付金额
                        updateOrder.Append(" A7=" + Order.A7 + ",");//代付返点

                        updateOrder.Append(" PolicySource=" + Order.PolicySource + ",");//政策来源
                        //updateOrder.Append(" OutOrderId='" + Order.OutOrderId + "',");//外部订单号
                        updateOrder.Append(" CPRemark='" + Order.CPRemark + "',");//出票备注

                        updateOrder.Append(" CPTime='" + DateTime.Now + "',");//出票时间
                        updateOrder.Append(" CPName='" + mUser.UserName + "',");//出票人姓名
                        updateOrder.Append(" CPCpyNo='" + mUser.CpyNo + "',");//出票公司编号
                        updateOrder.Append(" CPCpyName='" + mCompany.UninAllName + "',");//出票公司名称

                        updateOrder.Append(" OrderStatusCode=4");  //操作类型即要修改的订单状态
                        updateOrder.Append(" where OrderId='" + Order.OrderId + "'");

                        sqlList.Add(updateOrder.ToString());

                        #endregion

                        #region 3.添加订单日志

                        Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                        OrderLog.id = Guid.NewGuid();
                        OrderLog.OrderId = Order.OrderId;
                        OrderLog.OperType = "出票";
                        OrderLog.OperTime = DateTime.Now;
                        OrderLog.OperLoginName = mUser.LoginName;
                        OrderLog.OperUserName = mUser.UserName;
                        OrderLog.CpyNo = mCompany.UninCode;
                        OrderLog.CpyType = mCompany.RoleType;
                        OrderLog.CpyName = mCompany.UninAllName;
                        OrderLog.OperContent = "自动复合";
                        OrderLog.WatchType = 5;
                        string tempSql = PbProject.Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                        sqlList.Add(tempSql);
                        #endregion

                        //修改数据库
                        IsSuc = this.baseDataManage.ExecuteSqlTran(sqlList, out ErrMsg);

                        if (IsSuc)
                        {
                            ErrMsg = "自动复合成功！";
                        }
                        else
                        {
                            ErrMsg = "自动复合失败！";
                        }

                        if (count != pasList.Count)
                        {
                            ErrMsg += "有 " + (pasList.Count - count).ToString() + " 个乘客未找到票号信息，请手动处理！";
                            IsSuc = false;
                        }
                    }
                    else
                    {
                        ErrMsg = "编码中未能解析出票号";
                    }
                }
                #endregion
            }
            else
            {
                ErrMsg = "订单不存在!";
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message;
        }
        finally
        {

        }
        return IsSuc;
    }

    /// <summary>
    /// repOrderList_ItemDataBound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void repOrderList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        try
        {
            #region 第一层显示按钮

            LinkButton lbtnOrderOp = e.Item.FindControl("lbtnOrderOp") as LinkButton;

            #region 共享政策隐藏
            string hid_CPCpyNo = (e.Item.FindControl("hid_CPCpyNo") as HiddenField).Value.ToString();
            if (hid_CPCpyNo != mUser.CpyNo)
                lbtnOrderOp.Visible = false;
            #endregion

            if (lbtnOrderOp.Visible == true)
                lbtnOrderOp.Text += "<br/>";


            LinkButton lbtnDetail = e.Item.FindControl("lbtnDetail") as LinkButton;
            if (lbtnDetail.Visible == true)
                lbtnDetail.Text += "<br/>";

            #endregion 订单处理

            #region 订单处理 更多按钮

            //Button btnFH = e.Item.FindControl("btnFH") as Button;//自动复合
            //Button btnCP = e.Item.FindControl("btnCP") as Button; //确定出票
            //Button btnClose = e.Item.FindControl("btnClose") as Button; //关闭并解锁

            string hid_PayFlag = (e.Item.FindControl("hid_PayFlag") as HiddenField).Value.ToString();
            string hid_PolicySource = (e.Item.FindControl("hid_PolicySource") as HiddenField).Value.ToString();

            if (hid_PayFlag != "1" && Convert.ToInt32(hid_PolicySource) > 2)//代付状态未付款
            {
                (e.Item.FindControl("lbtnPay") as Button).Visible = true; // 代付
            }

            Button btnJJCP = e.Item.FindControl("btnJJCP") as Button; //拒绝出票
            btnJJCP.OnClientClick = "showdialogs('" + btnJJCP.CommandArgument + "');return false;";

            //接口的票才显示 代付和查询状态按钮
            if (hid_PolicySource != "1" && hid_PolicySource != "2" && hid_PolicySource != "9")
            {
                (e.Item.FindControl("LinkDetailByJK") as Button).Visible = true;
                (e.Item.FindControl("lbtnPay") as Button).Visible = true;

            }
            else
            {
                (e.Item.FindControl("LinkDetailByJK") as Button).Visible = false;
                (e.Item.FindControl("lbtnPay") as Button).Visible = true;
            }

            #endregion
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="OrderId"></param>
    /// <param name="OwnerCpyName"></param>
    /// <param name="CPCpyNo"></param>
    /// <param name="PolicySource"></param>
    /// <returns></returns>
    public string GetUserName(string OrderId, string OwnerCpyName, string CPCpyNo, string PolicySource)
    {
        string vale = "";
        try
        {
            if (PolicySource == "9")
            {
                if (CPCpyNo != mUser.CpyNo)
                {
                    vale = "<a href='#' onclick=\"return GetUserInfo('" + OrderId + "')\">" + OwnerCpyName + "</a>";
                }
                else
                {
                    vale = "异地采购商";
                }
            }
            else
            {
                vale = "<a href='#' onclick=\"return GetUserInfo('" + OrderId + "')\">" + OwnerCpyName + "</a>";
            }
        }
        catch (Exception ex)
        {

        }

        return vale;
    }

    #endregion

    #region 按钮事件

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        BtnSel(0);
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtOrderId.Text = "";
        txtPNR.Text = "";
        txtFromCity.Value = "";
        txtToCity.Value = "";
        txtPassengerName.Text = "";
        txtCorporationName.Text = "";
        txtFlightCode.Text = "";

        txtFromDate1.Value = "";
        txtFromDate2.Value = "";

        txtCreateTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
        txtCreateTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");
    }


    #region 按类型查询

    protected void btn0_Click(object sender, EventArgs e)
    {
        BtnSel(0);
    }
    protected void btn1_Click(object sender, EventArgs e)
    {
        BtnSel(1);
    }
    protected void btn2_Click(object sender, EventArgs e)
    {
        BtnSel(2);
    }
    protected void btn3_Click(object sender, EventArgs e)
    {
        BtnSel(3);
    }
    protected void btn4_Click(object sender, EventArgs e)
    {
        BtnSel(4);
    }

    /// <summary>
    /// 按类型查询
    /// </summary>
    /// <param name="type"></param>
    public void BtnSel(int type)
    {
        try
        {
            ViewState["selType"] = type.ToString();

            Curr = 1;
            AspNetPager1.CurrentPageIndex = Curr;
            Con = SelWhere();
            OrderListDataBind();

            GetOrderCount(type.ToString());
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 获取订单信息
    /// </summary>
    public void GetOrderCount(string type)
    {
        try
        {
            string sec1 = "background: url(../img/all_pic.gif);background-position: -262px -36px;text-align: center;cursor: hand;color: #000;";
            string sec2 = "background: url(../img/all_pic.gif);background-position: -79px -36px;text-align: center;cursor: hand;color: #fff;";

            orderstatus0.Style.Value = sec1;
            orderstatus1.Style.Value = sec1;
            orderstatus2.Style.Value = sec1;
            orderstatus3.Style.Value = sec1;
            orderstatus4.Style.Value = sec1;

            lab0.Text = "0";
            lab1.Text = "0";
            lab2.Text = "0";
            lab3.Text = "0";
            lab4.Text = "0";


            //所有订单
            if (type == "0")
                orderstatus0.Style.Value = sec2;
            else if (type == "1")
                orderstatus1.Style.Value = sec2;
            else if (type == "2")
                orderstatus2.Style.Value = sec2;
            else if (type == "3")
                orderstatus3.Style.Value = sec2;
            else if (type == "4")
                orderstatus4.Style.Value = sec2;

            if (ViewState["selCount"] != null && !string.IsNullOrEmpty(ViewState["selCount"].ToString()))
            {
                string sql = "select PolicySource,PolicyType,CPCpyNo,COUNT(1) as n from(select PolicySource,PolicyType,CPCpyNo from Tb_Ticket_Order where " + ViewState["selCount"].ToString() + ")as a group by PolicySource,PolicyType,CPCpyNo";
                DataTable dt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetOrderBySql(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int forCount = dt.Rows.Count;

                    string tempPolicySource = "";
                    string tempCpyNo = "";
                    string tempPolicyType = "";

                    int tempCount = 0;

                    int allCount = 0;
                    int b2bCount = 0;
                    int bspCount = 0;
                    int ptCount = 0;
                    int gxCount = 0;

                    //1 本地B2B, 2 本地BSP,3 517,4 百拓,5 8000翼,6 今日,7 票盟,8 51book ,9 共享,10易行

                    for (int i = 0; i < forCount; i++)
                    {
                        tempPolicySource = dt.Rows[i]["PolicySource"].ToString();
                        tempPolicyType = dt.Rows[i]["PolicyType"].ToString();
                        tempCpyNo = dt.Rows[i]["CPCpyNo"].ToString();
                        tempCount = int.Parse(dt.Rows[i]["n"].ToString());

                        if (tempPolicySource != "0")
                        {
                            allCount += tempCount;

                            if (tempPolicySource == "1")
                            {
                                b2bCount += tempCount;
                            }
                            else if (tempPolicySource == "2")
                            {
                                bspCount += tempCount;
                            }
                            else if (tempPolicySource == "9")
                            {
                                if (mUser.CpyNo == tempCpyNo)
                                {
                                    //本地出
                                    if (tempPolicyType == "1")
                                    {
                                        b2bCount += tempCount;
                                    }
                                    else if (tempPolicyType == "2")
                                    {
                                        bspCount += tempCount;
                                    }
                                }
                                else
                                {
                                    //共享
                                    gxCount += tempCount;
                                }
                            }
                            else
                            {
                                ptCount += tempCount;
                            }
                        }
                    }

                    lab0.Text = allCount.ToString();
                    lab1.Text = b2bCount.ToString();
                    lab2.Text = bspCount.ToString();
                    lab3.Text = ptCount.ToString();
                    lab4.Text = gxCount.ToString();
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    #endregion


    #endregion

    #region 数据处理

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ParentId">类型</param>
    /// <param name="ChildId"></param>
    /// <returns></returns>
    public string GetStrValue(string ParentId, string ChildId)
    {
        string msg = "";
        try
        {
            msg = GetDictionaryName(ParentId, ChildId);
            msg = string.Join("<br />", msg.Split(new string[] { ",", "，", " " }, StringSplitOptions.RemoveEmptyEntries));
            if (ChildId == "3")
                msg = "<span style='color:Red;'>" + msg + " </span>";
        }
        catch (Exception ex)
        {

        }

        return msg;
    }


    //<%# GetStrValue("24", Eval("CPCpyNo").ToString(), Eval("PolicySource").ToString(), Eval("PolicyType").ToString())%>

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ParentId">类型</param>
    /// <param name="PolicySource"></param>
    /// <returns></returns>
    public string GetStrValue(string ParentId, string PolicySource, string CPCpyNo, string PolicyType)
    {
        string msg = "";
        try
        {
            if (PolicySource == "9")
            {
                if (CPCpyNo == mUser.CpyNo)
                {
                    if (PolicyType == "1")
                    {
                        msg = "B2B";
                    }
                    else if (PolicyType == "1")
                    {
                        msg = "BSP";
                    }
                }
                else
                {
                    msg = GetDictionaryName(ParentId, PolicySource);
                }
            }
            else
            {
                msg = GetDictionaryName(ParentId, PolicySource);
            }

        }
        catch (Exception ex)
        {

        }

        return msg;
    }

    #endregion

    /// <summary>
    /// 拒绝出票
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnRefuse_Click(object sender, EventArgs e)
    {
        bool reuslt = false;
        string errtitle = "";

        try
        {
            #region   拒绝出票处理
            Tb_Ticket_Order Order = ViewState["Order"] as Tb_Ticket_Order;
            int OrderRows = repOrderList.Items.Count;
            for (int i = 0; i < OrderRows; i++)
            {
                TextBox txtRemark = repOrderList.Items[i].FindControl("txtRemak") as TextBox;
                HiddenField hid_Id = repOrderList.Items[i].FindControl("Hid_Id") as HiddenField;
                if (hid_Id != null && txtRemark != null && hid_Id.Value == Order.id.ToString())
                {
                    //出票拒绝理由
                    Order.CPRemark = txtRemark.Text.Trim();
                    break;
                }
            }
            //拒绝理由
            Order.TGQRefusalReason = hid_pek.Value;

            Order.OrderStatusCode = 20; //取消出票,退款中
            List<Tb_Ticket_Passenger> passengerList = ViewState["PassengerList"] as List<Tb_Ticket_Passenger>;
            reuslt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().OperOrderJJCP(Order, passengerList, mUser, mCompany);

            if (reuslt)
            {
                Order.CPTime = DateTime.Now;//出票时间
                Order.CPName = mUser.UserName;//出票人姓名
                Order.CPCpyNo = mUser.CpyNo;//出票公司编号
                Order.CPCpyName = mCompany.UninAllName;//出票公司名称

                // 拒绝出票退款处理
                reuslt = new PbProject.Logic.Pay.OperOnline().TitckOrderRefund(Order, mUser, mCompany, out errtitle);
                errtitle = reuslt ? "拒绝出票成功,退款中..." : "处理失败:" + errtitle;
            }
            else
            {
                errtitle = "拒绝出票失败,订单已被锁定!";
            }

            #endregion
        }
        catch (Exception)
        {
            errtitle = "拒绝失败,订单已被锁定!";
        }

        OrderListDataBind();

        ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + errtitle + "');", true);
    }


    /// <summary>
    /// 页面数据显示处理
    /// </summary>
    /// <param name="type">标识</param>
    /// <param name="Data">多列数据</param>
    /// <returns></returns>
    public string ShowText(int type, params Object[] Data)
    {
        string result = "";
        if (type == 0)
        {
            //自动出票标识 数据1:AutoPrintFlag  数据2：PolicyType 数据3:政策id
            if (Data != null && Data.Length == 3 && Data[0].ToString() != "" && Data[1].ToString() != "" && Data[2].ToString() != "")
            {
                string AutoPrintFlag = Data[0].ToString();
                string PolicyType = Data[1].ToString();
                string PolicyId = Data[2].ToString();//
                if (AutoPrintFlag == "2")//全自动
                {
                    if (PolicyType == "1")//1 本地B2B, 2 本地BSP
                    {
                        result = "<br /><span class=\"red\">B2B全自动" + (PolicyId.Contains("b2bpolicy") ? "(air)" : "") + "</span>";
                    }
                    else if (PolicyType == "2")
                    {
                        result = "<br /><span class=\"red\">BSP全自动" + (PolicyId.Contains("b2bpolicy") ? "(air)" : "") + "</span>";
                    }
                }
            }
        }
        else if (type == 1)
        {
            if (Data != null && Data.Length > 0 && Data[0].ToString() != "")
            {
                result = Data[0].ToString().Replace("/", "<br />");
            }
        }
        else if (type == 2)//代付状态
        {
            result = "未付";
            if (Data != null && Data.Length > 0 && Data[0].ToString() != "")
            {
                if (Data[0].ToString().Trim().ToUpper() == "TRUE")// yyy 2013_04_09 修改(数据库是bit字段0,1.但是model层是bool,此处应当判断TRUE,FALSE)
                {
                    result = "<span class=\"green\">已付</span>";
                }
                else
                {
                    result = "<span class=\"red\">未付</span>";
                }
            }
        }
        else if (type == 3)//订单时长
        {
            result = "0";
            if (Data != null && Data.Length > 0 && Data[0].ToString() != "")
            {
                int minute = 0;
                if (int.TryParse(Data[0].ToString(), out minute))
                {
                    if (minute < 60)
                    {
                        result = "<font class=\"red\">" + minute + "分钟</font>";
                    }
                    else
                    {
                        result = "<font class=\"red\">" + ((int)(minute / 60)) + "小时</font><font class=\"red\">" + ((int)(minute % 60)) + "分钟</font>";
                    }
                }
            }
        }
        else if (type == 4)//当日航班显示红色
        {
            //tdbg
            if (Data != null && Data.Length > 0 && Data[0].ToString() != "")
            {
                DateTime dt = DateTime.Parse("1901-01-01");
                if (DateTime.TryParse(Data[0].ToString(), out dt))
                {
                    //乘机日期为当天
                    if (dt.ToString("yyyy-MM-dd") == System.DateTime.Now.ToString("yyyy-MM-dd"))
                    {
                        result = "class=\"tdbg\"";
                    }
                }
            }
        }
        else if (type == 5) //支付时间、创建
        {
            if (Data != null && Data.Length > 0 && Data[0].ToString() != "")
            {
                DateTime dt = DateTime.Parse("1901-01-01");

                if (DateTime.TryParse(Data[0].ToString(), out dt))
                {
                    //支付时间
                    result = dt.ToString("yyyy-MM-dd") + "<br/>" + dt.ToString("HH:mm:ss");
                }
            }
        }
        else if (type == 6) //起飞时间
        {
            if (Data != null && Data.Length > 0 && Data[0].ToString() != "")
            {
                DateTime dt = DateTime.Parse("1901-01-01");

                if (DateTime.TryParse(Data[0].ToString(), out dt))
                {
                    //支付时间
                    result = dt.ToString("yyyy-MM-dd") + "<br/>" + dt.ToString("HH:mm:ss");
                }
            }
        }
        else if (type == 7) //儿童出成人票标记
        {
            if (Data != null && Data.Length == 4)
            {
                string OrderId = Data[0].ToString();
                string Identity = Data[1].ToString();
                string IsCHDETAdultTK = Data[2].ToString();
                string OutOrderId = Data[3].ToString();
                //IsChdFlag （0=成人订单，1=儿童订单）
                if (Identity.ToLower() == "true" || Identity.ToLower() == "1")
                {
                    result = string.Format("{0}<br /><font class='red'>{1}<br/>{2}</font>", OrderId, OutOrderId, IsCHDETAdultTK == "1" ? "儿童(成人价)" : "儿童");
                }
                else
                {
                    result = string.Format("{0}<br /><font class='red'>{1}</font>", OrderId, OutOrderId);
                }
            }
        }
        else if (type == 8)//9政策来源
        {
            if (Data != null && Data.Length == 3)
            {
                string PolicySource = Data[0] != null ? Data[0].ToString() : "";
                string CPCpyNo = Data[1] != null ? Data[1].ToString() : "";
                string PolicyType = Data[2] != null ? Data[2].ToString() : "";
                if (PolicySource == "9")
                {
                    if (CPCpyNo == mUser.CpyNo)
                    {
                        if (PolicyType == "1")
                        {
                            result = "本地B2B";
                        }
                        else if (PolicyType == "2")
                        {
                            result = "本地BSP";
                        }
                    }
                    else
                    {
                        if (PolicySource != "")
                        {
                            result = GetDictionaryName("24", PolicySource);
                        }
                    }
                }
                else
                {
                    result = GetDictionaryName("24", PolicySource);
                }
            }
        }
        else if (type == 9)//查看授权
        {
            result = "";
            if (Data != null && Data.Length == 3)
            {
                //：1 本地B2B, 2 本地BSP,3 517,4 百拓,5 8000翼,6 今日,7 票盟,8 51book ,9 共享,10易行
                string PolicySource = Data[0] != null ? Data[0].ToString() : "";
                string PrintOffice = Data[1] != null ? Data[1].ToString() : "";
                string OrderSourceType = Data[2] != null ? Data[2].ToString() : "";
                //不是PNR内容导入 不是本地B2B和BSP PrintOffice不为空mCompany.RoleType < 3 &&
                if (PolicySource != "1" && PolicySource != "2" && PrintOffice != "" && !"379".Contains(OrderSourceType))
                {
                    result = "<a href=\"#\" onclick=\"return GetAuth('" + PrintOffice + "');\">查看授权</a>";
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 自动复合（接口订单复合）
    /// </summary>
    /// <param name="id">id </param>
    /// <param name="msg">消息</param>
    /// <returns></returns>
    public bool OrderFH(Tb_Ticket_Order Order, out string msg)
    {
        msg = "";
        bool result = false;

        try
        {

            List<string> sqlList = new List<string>();

            #region 自动复活票号

            Tb_Ticket_OrderBLL OrderManager = new Tb_Ticket_OrderBLL();
            Tb_Ticket_PassengerBLL PassengerManager = new Tb_Ticket_PassengerBLL();

            if (Order == null)
            {
                return false;
            }

            IList<PbProject.Model.Tb_Ticket_Passenger> PassengerList = new PbProject.Logic.Order.Tb_Ticket_PassengerBLL().GetPasListByOrderID(Order.OrderId);

            if (Order.PolicySource == 3)
            {
                #region 517订单

                try
                {
                    PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);
                    string Accout517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[0];
                    string Password517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[1];
                    string Ag517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[2];
                    w_517WebService._517WebService ServiceBy517 = new w_517WebService._517WebService();
                    DataSet ds = ServiceBy517.GetOrderInfo(Accout517, Password517, Ag517, Order.OutOrderId, Order.PNR.ToString());

                    if (ds != null && ds.Tables.Count == 8 && ds.Tables[5].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[5].Rows.Count; i++)
                        {
                            if (ds.Tables[5].Rows[i]["TicketNo"].ToString() != "" && PassengerList != null && PassengerList.Count > 0)
                            {
                                for (int j = 0; j < PassengerList.Count; j++)
                                {
                                    if (PassengerList[j].PassengerName == ds.Tables[5].Rows[i]["Name"].ToString())
                                    {
                                        string[] TicketNum = new string[2];
                                        if (ds.Tables[5].Rows[i]["TicketNo"].ToString().IndexOf("-") >= 0)
                                        {
                                            TicketNum = ds.Tables[5].Rows[i]["TicketNo"].ToString().Split('-');
                                        }
                                        else
                                        {
                                            TicketNum[0] = ds.Tables[5].Rows[i]["TicketNo"].ToString().Substring(0, 3);
                                            TicketNum[1] = ds.Tables[5].Rows[i]["TicketNo"].ToString().Substring(3, ds.Tables[5].Rows[i]["TicketNo"].ToString().Length - 3);
                                        }

                                        if (TicketNum.Length == 2 && TicketNum[1].ToString() != "")
                                        {
                                            PassengerList[j].TicketNumber = ds.Tables[5].Rows[i]["TicketNo"].ToString().Trim(); //更改票号
                                            sqlList.Add(string.Format(" update  Tb_Ticket_Passenger set TicketStatus=2,TicketNumber='{0}' where id='{1}' ", PassengerList[j].TicketNumber, PassengerList[j].id.ToString()));
                                        }
                                    }
                                }
                            }
                        }
                        if (ds.Tables[5].Rows.Count > 0 && ds.Tables[0].Rows[0]["RR"] != null && ds.Tables[0].Rows[0]["RR"].ToString() != "")//是否换编码出票
                        {
                            Order.ChangePNR = ds.Tables[0].Rows[0]["RR"].ToString();
                        }
                    }
                    else
                    {
                        msg = "提取票号失败!";
                    }
                }
                catch (Exception e)
                {
                    msg = "提取票号失败!";
                }
                #endregion
            }
            else if (Order.PolicySource == 4)
            {
                #region 百拓订单

                try
                {
                    PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);
                    //百拓订单
                    PbProject.Logic.PTInterface.PTBybaituo BaiTuoInterface = new PbProject.Logic.PTInterface.PTBybaituo(Order, BS);
                    //百拓接口调用
                    w_BTWebService.BaiTuoWeb BaiTuoService = new w_BTWebService.BaiTuoWeb();
                    XmlElement xe = BaiTuoInterface.BaiTuoCpSend(Order.OutOrderId);
                    XmlNode xml = BaiTuoService.getOrderInfoXml(xe);
                    DataSet ds = new DataSet();
                    ds = BaiTuoInterface.BaiTuoCpReceive(xml);
                    //根据返回的信息进行判断

                    if (ds != null && ds.Tables.Count == 2 && ds.Tables[1].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        {
                            if (ds.Tables[1].Rows[i]["TICKETINFO_Text"].ToString() != "" && PassengerList != null && PassengerList.Count > 0)
                            {
                                for (int j = 0; j < PassengerList.Count; j++)
                                {
                                    if (PassengerList[j].PassengerName == ds.Tables[1].Rows[i]["personName"].ToString())
                                    {
                                        string[] TicketNum = new string[2];
                                        if (ds.Tables[1].Rows[i]["TICKETINFO_Text"].ToString().IndexOf("-") >= 0)
                                        {
                                            TicketNum = ds.Tables[1].Rows[i]["TICKETINFO_Text"].ToString().Split('-');
                                        }
                                        else
                                        {
                                            TicketNum[0] = ds.Tables[1].Rows[i]["TICKETINFO_Text"].ToString().Substring(0, 3);
                                            TicketNum[1] = ds.Tables[1].Rows[i]["TICKETINFO_Text"].ToString().Substring(3, ds.Tables[1].Rows[i]["TICKETINFO_Text"].ToString().Length - 3);
                                        }

                                        if (TicketNum.Length == 2)
                                        {
                                            if (TicketNum[1].ToString() != "")
                                                PassengerList[j].TicketNumber = TicketNum[0].ToString().Trim() + "-" + TicketNum[1].ToString().Trim();
                                            else
                                                PassengerList[j].TicketNumber = TicketNum[0].ToString().Trim();

                                            sqlList.Add(string.Format(" update  Tb_Ticket_Passenger set TicketStatus=2,TicketNumber='{0}' where id='{1}' ", PassengerList[j].TicketNumber, PassengerList[j].id.ToString()));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        msg = "提取票号失败!";
                    }
                }
                catch (Exception ex)
                {
                    msg = "提取票号失败!";
                }

                #endregion
            }
            else if (Order.PolicySource == 5)
            {
                #region 8000Y订单
                try
                {
                    //msg = "八千翼不支持自动复合票号。";
                    PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);
                    string Accout8000yi = BS.JieKouZhangHao.Split('|')[5].Split('^')[0];
                    string Password8000yi = BS.JieKouZhangHao.Split('|')[5].Split('^')[1];
                    //string Ag8000yi = BS.JieKouZhangHao.Split('|')[5].Split('^')[2];
                    w_8000YService.W8000YService WSvc8000Y = new w_8000YService.W8000YService();
                    DataSet ds = WSvc8000Y.ReturnTicekNo(Accout8000yi, Password8000yi, Order.OutOrderId);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (ds.Tables[0].Rows[i]["PsgerTicketNo"].ToString() != "" && PassengerList != null && PassengerList.Count > 0)
                            {
                                for (int j = 0; j < PassengerList.Count; j++)
                                {
                                    if (PassengerList[j].PassengerName == ds.Tables[0].Rows[i]["PsgerName"].ToString())
                                    {
                                        string[] TicketNum = new string[2];
                                        if (ds.Tables[0].Rows[i]["PsgerTicketNo"].ToString().IndexOf("-") >= 0)
                                        {
                                            TicketNum = ds.Tables[0].Rows[i]["PsgerTicketNo"].ToString().Split('-');
                                        }
                                        else
                                        {
                                            TicketNum[0] = ds.Tables[0].Rows[i]["PsgerTicketNo"].ToString().Substring(0, 3);
                                            TicketNum[1] = ds.Tables[0].Rows[i]["PsgerTicketNo"].ToString().Substring(3, ds.Tables[0].Rows[i]["PsgerTicketNo"].ToString().Length - 3);
                                        }

                                        if (TicketNum.Length == 2 && TicketNum[1].ToString() != "")
                                        {
                                            PassengerList[j].TicketNumber = ds.Tables[0].Rows[i]["PsgerTicketNo"].ToString().Trim(); //更改票号
                                            sqlList.Add(string.Format(" update  Tb_Ticket_Passenger set TicketStatus=2,TicketNumber='{0}' where id='{1}' ", PassengerList[j].TicketNumber, PassengerList[j].id.ToString()));
                                        }
                                    }
                                }
                            }
                        }
                        //if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["RR"] != null && ds.Tables[0].Rows[0]["RR"].ToString() != "")//是否换编码出票
                        //{
                        //    Order.ChangePNR = ds.Tables[0].Rows[0]["RR"].ToString();
                        //}
                    }
                    else
                    {
                        msg = "提取票号失败!";
                    }
                }
                catch (Exception e)
                {
                    msg = "提取票号失败!";
                }
                #endregion
            }
            else if (Order.PolicySource == 6)
            {
                #region 今日订单
                try
                {
                    PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);

                    string todayAccout2 = BS.JieKouZhangHao.Split('|')[4].Split('^')[1];
                    w_TodayService.WTodayService WSvcToday = new w_TodayService.WTodayService();
                    DataSet ds = WSvcToday.GetOrderInfo(todayAccout2, Order.OutOrderId);

                    if (ds != null)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (ds.Tables[0].Rows[i]["TicketNo"].ToString() != "" && PassengerList != null && PassengerList.Count > 0)
                            {
                                for (int j = 0; j < PassengerList.Count; j++)
                                {
                                    if (PassengerList[j].PassengerName == ds.Tables[0].Rows[i]["PName"].ToString())
                                    {
                                        string[] TicketNum = new string[2];
                                        if (ds.Tables[0].Rows[i]["TicketNo"].ToString().IndexOf("-") >= 0)
                                        {
                                            TicketNum = ds.Tables[0].Rows[i]["TicketNo"].ToString().Split('-');
                                        }
                                        else
                                        {
                                            TicketNum[0] = ds.Tables[0].Rows[i]["TicketNo"].ToString().Substring(0, 3);
                                            TicketNum[1] = ds.Tables[0].Rows[i]["TicketNo"].ToString().Substring(3, ds.Tables[0].Rows[i]["TicketNo"].ToString().Length - 3);
                                        }

                                        if (TicketNum.Length == 2 && TicketNum[1].ToString() != "")
                                        {
                                            PassengerList[j].TicketNumber = ds.Tables[0].Rows[i]["TicketNo"].ToString().Trim(); //更改票号
                                            sqlList.Add(string.Format(" update  Tb_Ticket_Passenger set TicketStatus=2,TicketNumber='{0}' where id='{1}' ", PassengerList[j].TicketNumber, PassengerList[j].id.ToString()));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        msg = "提取票号失败!";
                    }
                }
                catch (Exception e)
                {
                    msg = "提取票号失败!";
                }
                #endregion
            }
            else if (Order.PolicySource == 7)
            {
                #region 票盟订单
                try
                {
                    PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();

                    BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);
                    string pmAccout = BS.JieKouZhangHao.Split('|')[3].Split('^')[0];

                    string pmPassword = BS.JieKouZhangHao.Split('|')[3].Split('^')[1];
                    string pmAg = BS.JieKouZhangHao.Split('|')[3].Split('^')[2];
                    PMService.PMService PMservice = new PMService.PMService();
                    DataSet ds = PMservice.PMOrderQuery(Order.OutOrderId, pmAccout, pmAg);

                    if (ds != null)
                    {
                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        {
                            if (ds.Tables[1].Rows[i]["ticketno"].ToString() != "" && PassengerList != null && PassengerList.Count > 0)
                            {
                                for (int j = 0; j < PassengerList.Count; j++)
                                {
                                    if (PassengerList[j].PassengerName == ds.Tables[1].Rows[i]["passname"].ToString())
                                    {
                                        string[] TicketNum = new string[2];
                                        if (ds.Tables[1].Rows[i]["ticketno"].ToString().IndexOf("-") >= 0)
                                        {
                                            TicketNum = ds.Tables[1].Rows[i]["ticketno"].ToString().Split('-');
                                        }
                                        else
                                        {
                                            TicketNum[0] = ds.Tables[1].Rows[i]["ticketno"].ToString().Substring(0, 3);
                                            TicketNum[1] = ds.Tables[1].Rows[i]["ticketno"].ToString().Substring(3, ds.Tables[1].Rows[i]["ticketno"].ToString().Length - 3);
                                        }

                                        if (TicketNum.Length == 2 && TicketNum[1].ToString() != "")
                                        {
                                            PassengerList[j].TicketNumber = ds.Tables[1].Rows[i]["ticketno"].ToString().Trim();//更改票号
                                            sqlList.Add(string.Format(" update  Tb_Ticket_Passenger set TicketStatus=2,TicketNumber='{0}' where id='{1}' ", PassengerList[j].TicketNumber, PassengerList[j].id.ToString()));
                                        }
                                    }
                                }
                            }
                        }
                        //if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["oldPnrNo"] != null && ds.Tables[0].Rows[0]["oldPnrNo"].ToString() != "")//是否换编码出票
                        //{
                        //    Order.A28 = ds.Tables[0].Rows[0]["pnrNo"].ToString();
                        //}
                    }
                    else
                    {
                        msg = "提取票号失败!";
                    }
                }
                catch (Exception e)
                {
                    msg = "提取票号失败!";
                }
                #endregion
            }
            else if (Order.PolicySource == 8)
            {
                #region 51book订单

                try
                {
                    PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);

                    string Accout51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[0];

                    string Ag51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[2];

                    w_51bookService._51bookService bookService = new w_51bookService._51bookService();
                    DataSet ds = bookService.bookgetPolicyOrderByOrderNo(Accout51book, Order.OutOrderId, "1", Ag51book);

                    if (ds != null)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (ds.Tables[0].Rows[i]["ticketNo"].ToString() != "" && PassengerList != null && PassengerList.Count > 0)
                            {
                                for (int j = 0; j < PassengerList.Count; j++)
                                {
                                    if (PassengerList[j].PassengerName == ds.Tables[0].Rows[i]["Name"].ToString())
                                    {
                                        string[] TicketNum = new string[2];
                                        if (ds.Tables[0].Rows[i]["ticketNo"].ToString().IndexOf("-") >= 0)
                                        {
                                            TicketNum = ds.Tables[0].Rows[i]["ticketNo"].ToString().Split('-');
                                        }
                                        else
                                        {
                                            TicketNum[0] = ds.Tables[0].Rows[i]["ticketNo"].ToString().Substring(0, 3);
                                            TicketNum[1] = ds.Tables[0].Rows[i]["ticketNo"].ToString().Substring(3, ds.Tables[0].Rows[i]["ticketNo"].ToString().Length - 3);
                                        }

                                        if (TicketNum.Length == 2 && TicketNum[1].ToString() != "")
                                        {
                                            PassengerList[j].TicketNumber = ds.Tables[0].Rows[i]["ticketNo"].ToString().Trim();//更改票号

                                            sqlList.Add(string.Format(" update  Tb_Ticket_Passenger set TicketStatus=2,TicketNumber='{0}' where id='{1}' ", PassengerList[j].TicketNumber, PassengerList[j].id.ToString()));
                                        }
                                    }
                                }
                            }
                        }
                        //if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["oldPnrNo"] != null && ds.Tables[0].Rows[0]["oldPnrNo"].ToString() != "")//是否换编码出票
                        //{
                        //    Order.A28 = ds.Tables[0].Rows[0]["pnrNo"].ToString();
                        //}
                    }
                    else
                    {
                        msg = "提取票号失败!";
                    }
                }
                catch (Exception e)
                {
                    msg = "提取票号失败!";
                }
                #endregion
            }
            else if (Order.PolicySource == 10)
            {
                #region 易行
                try
                {
                    PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);

                    w_YeeXingService.YeeXingSerivce YeeXingService = new w_YeeXingService.YeeXingSerivce();

                    string yeeXingAccout = BS.JieKouZhangHao.Split('|')[6].Split('^')[0];

                    string yeeXingAccout2 = BS.JieKouZhangHao.Split('|')[6].Split('^')[1];

                    DataSet ds = YeeXingService.OrderQueryContract(yeeXingAccout, yeeXingAccout2, Order.OutOrderId, Order.OrderId);

                    if (ds != null)
                    {
                        for (int i = 0; i < ds.Tables["ticketInfo"].Rows.Count; i++)
                        {
                            if (ds.Tables["ticketInfo"].Rows[i]["airId"].ToString() != "" && PassengerList != null && PassengerList.Count > 0)
                            {
                                for (int j = 0; j < PassengerList.Count; j++)
                                {
                                    if (PassengerList[j].PassengerName == ds.Tables["ticketInfo"].Rows[i]["passengerName"].ToString())
                                    {
                                        string[] TicketNum = new string[2];
                                        if (ds.Tables["ticketInfo"].Rows[i]["airId"].ToString().IndexOf("-") >= 0)
                                        {
                                            TicketNum = ds.Tables["ticketInfo"].Rows[i]["airId"].ToString().Split('-');
                                        }
                                        else
                                        {
                                            TicketNum[0] = ds.Tables["ticketInfo"].Rows[i]["airId"].ToString().Substring(0, 3);
                                            TicketNum[1] = ds.Tables["ticketInfo"].Rows[i]["airId"].ToString().Substring(3, ds.Tables["ticketInfo"].Rows[i]["airId"].ToString().Length - 3);
                                        }

                                        if (TicketNum.Length == 2 && TicketNum[1].ToString() != "")
                                        {
                                            PassengerList[j].TicketNumber = ds.Tables["ticketInfo"].Rows[i]["airId"].ToString().Trim();//更改票号
                                            sqlList.Add(string.Format(" update  Tb_Ticket_Passenger set TicketStatus=2,TicketNumber='{0}' where id='{1}' ", PassengerList[j].TicketNumber, PassengerList[j].id.ToString()));
                                        }
                                    }
                                }
                            }
                        }
                        //if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["oldPnrNo"] != null && ds.Tables[0].Rows[0]["oldPnrNo"].ToString() != "")//是否换编码出票
                        //{
                        //    Order.A28 = ds.Tables[0].Rows[0]["pnrNo"].ToString();
                        //}
                    }
                    else
                    {
                        msg = "提取票号失败!";
                    }
                }
                catch (Exception e)
                {
                    msg = "提取票号失败!";
                }
                #endregion
            }

            #endregion

            //记录复合到的机票数量
            int tkcount = sqlList.Count;

            if (tkcount > 0)
            {
                #region 1.修改订单

                StringBuilder updateOrder = new StringBuilder();
                updateOrder.Append(" update Tb_Ticket_Order set ");
                updateOrder.Append(" LockCpyNo='',");//锁定帐号所属公司编号
                updateOrder.Append(" LockLoginName='',");//锁定帐户
                updateOrder.Append(" LockTime='1900-1-1',");//锁定时间
                updateOrder.Append(" TicketStatus=2,");//机票状态

                if (Order.AllowChangePNRFlag == true)
                {
                    updateOrder.Append(" ChangePNR='" + Order.ChangePNR + "',");//可以换编码
                }
                updateOrder.Append(" OutOrderPayMoney=" + Order.OutOrderPayMoney + ",");//代付金额
                updateOrder.Append(" A7=" + Order.A7 + ",");//代付返点

                updateOrder.Append(" PolicySource=" + Order.PolicySource + ",");//政策来源
                updateOrder.Append(" CPRemark='" + Order.CPRemark + "',");//出票备注

                updateOrder.Append(" CPTime='" + DateTime.Now.ToString() + "',");//时间 处理时间
                updateOrder.Append(" CPLoginName='" + mUser.LoginName + "',");//出票人登录帐号
                updateOrder.Append(" CPName='" + mUser.UserName + "',");//出票人姓名
                updateOrder.Append(" CPCpyNo='" + mCompany.UninCode + "',");//出票人公司编号
                updateOrder.Append(" CPCpyName='" + mCompany.UninAllName + "'");//出票公司名称


                //如果复核到的人员和票号数量不符，则不更改订单状态
                if (tkcount == PassengerList.Count)
                {
                    updateOrder.Append(" ,OrderStatusCode=4");  //操作类型即要修改的订单状态
                }

                updateOrder.Append(" where OrderId='" + Order.OrderId + "'");

                sqlList.Add(updateOrder.ToString());

                #endregion

                #region 2.添加订单日志

                Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();

                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = Order.OrderId;
                OrderLog.OperType = "出票";
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperLoginName = mUser.LoginName;
                OrderLog.OperUserName = mUser.UserName;
                OrderLog.CpyNo = mCompany.UninCode;
                OrderLog.CpyType = mCompany.RoleType;
                OrderLog.CpyName = mCompany.UninAllName;
                OrderLog.OperContent = "于 " + DateTime.Now + " 复合票号";
                OrderLog.WatchType = 5;
                string tempSql = PbProject.Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                sqlList.Add(tempSql);
                #endregion

                //修改数据库
                result = baseDataManage.ExecuteSqlTran(sqlList, out msg);

                if (result == true)
                {
                    msg = "复合票号成功！";
                }
                else
                {
                    msg = "复合票号失败！";//复合失败
                }

                if (tkcount != PassengerList.Count)
                {
                    msg += "有 " + (PassengerList.Count - tkcount).ToString() + " 个乘客未找到票号信息，请手动处理！";
                    result = false;
                }
            }
        }
        catch (Exception xp)
        {
            result = false;
        }

        msg = string.IsNullOrEmpty(msg) ? "复合票号失败！" : msg;

        return result;
    }


    /// <summary>
    /// 外部订单状态查询
    /// </summary>
    /// <param name="Order"></param>
    private void InterFaceStu(Tb_Ticket_Order Order)
    {
        try
        {
            string msg = "";
            if (Order.PolicySource == 3)
            {
                #region

                PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);
                string Accout517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[0];
                string Password517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[1];
                string Ag517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[2];
                w_517WebService._517WebService ServiceBy517 = new w_517WebService._517WebService();
                DataSet ds = new DataSet();
                ds = ServiceBy517.GetOrderInfo(Accout517, Password517, Ag517, Order.OutOrderId, Order.PNR);
                msg = ds.Tables[0].Rows[0]["OrderStatus"].ToString();
                if (ds.Tables.Count > 1)
                {
                    if (ds.Tables[0].Rows[0]["OrderStatus"].ToString().IndexOf("暂不能出票，等待处理") >= 0
                        || ds.Tables[0].Rows[0]["OrderStatus"].ToString().IndexOf("已经出票，交易结束") >= 0
                        || ds.Tables[0].Rows[0]["OrderStatus"].ToString().IndexOf("已经支付，等待出票") >= 0
                        || ds.Tables[0].Rows[0]["OrderStatus"].ToString().IndexOf("已经退款，交易结束") >= 0
                        || ds.Tables[0].Rows[0]["OrderStatus"].ToString().IndexOf("已经废票，交易结束") >= 0)
                    {
                        baseDataManage.ExecuteNonQuerySQLInfo("update Tb_Ticket_Order set OutOrderPayFlag=1 where OrderId='" + Order.OrderId + "'");
                    }
                }
                #endregion
            }
            else if (Order.PolicySource == 4)
            {
                #region
                PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);
                //百拓订单
                PbProject.Logic.PTInterface.PTBybaituo OrderBaiTuoInterface = new PbProject.Logic.PTInterface.PTBybaituo(Order, BS);
                w_BTWebService.BaiTuoWeb BaiTuoWebService = new w_BTWebService.BaiTuoWeb();
                XmlElement xmlElement = OrderBaiTuoInterface.BaiTuoCpSend(Order.OutOrderId);

                XmlNode xml = BaiTuoWebService.getOrderInfoXml(xmlElement);

                DataSet ds = new DataSet();
                StringReader rea = new StringReader("<ORDER_INFO_RS>" + xml.InnerXml + "</ORDER_INFO_RS>");

                XmlTextReader xmlReader = new XmlTextReader(rea);

                ds.ReadXml(xmlReader);
                if (ds.Tables.Count > 1)
                {
                    if (ds.Tables[0].Rows[0]["Status"].ToString() == "2")
                    {
                        msg = "平台订单已经支付，正在等待该平台出票!";
                    }
                    if (ds.Tables[0].Rows[0]["Status"].ToString() == "1")
                    {
                        msg = "该平台订单已经创建，如已经进行代付，请等待平台扣款出票后复合票号，如长时间未显示复合票号按钮，请登录平台检查是否扣款成功！";
                    }
                    if (ds.Tables[0].Rows[0]["Status"].ToString() == "4" || ds.Tables[0].Rows[0]["Status"].ToString() == "3")
                    {
                        baseDataManage.ExecuteNonQuerySQLInfo("update Tb_Ticket_Order set OutOrderPayFlag=1 where OrderId='" + Order.OrderId + "'");
                        msg = "平台出票方出票成功！";
                    }
                    if (ds.Tables[0].Rows[0]["Status"].ToString() == "11")
                    {
                        msg = "该平台出票方已取消出票！";
                    }
                }
                #endregion
            }
            else if (Order.PolicySource == 5)
            {
                #region
                msg = "8000yi不支持状态查询";
                //#region
                //PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);
                //string s8000yiAccout = BS.JieKouZhangHao.Split('|')[5].Split('^')[0];

                //string s8000yiPassword = BS.JieKouZhangHao.Split('|')[5].Split('^')[1];
                //w_8000YService.W8000YService WSvc8000Y = new w_8000YService.W8000YService();
                //DataSet dsReson = WSvc8000Y.OrderPayOutTicketAndPly(s8000yiAccout, s8000yiPassword, Order.CreateTime.ToString().Replace("/", "-"));

                //if (dsReson != null)
                //{
                //    msg = dsReson.Tables[0].Rows[0]["A9"].ToString();
                //    if (dsReson.Tables[0].Rows[0]["A10"].ToString() == "3" || dsReson.Tables[0].Rows[0]["A10"].ToString() == "8")
                //    {
                //        Order.OutOrderPayFlag = true;
                //        bool rs = (bool)baseDataManage.CallMethod("Tb_Ticket_Order", "Update", null, new object[] { Order });
                //    }
                //}
                #endregion
            }
            else if (Order.PolicySource == 6)
            {
                #region
                PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);

                string todayAccout2 = BS.JieKouZhangHao.Split('|')[4].Split('^')[1];
                w_TodayService.WTodayService WSvcToday = new w_TodayService.WTodayService();
                DataSet dsReson = WSvcToday.GetOrderInfo(todayAccout2, Order.OutOrderId);

                if (dsReson != null)
                {
                    msg = ReturnPTorderStumesg(dsReson.Tables[0].Rows[0]["Status"].ToString(), "6");
                    if (dsReson.Tables[0].Rows[0]["Status"].ToString() == "2" || dsReson.Tables[0].Rows[0]["Status"].ToString() == "14")
                    {
                        baseDataManage.ExecuteNonQuerySQLInfo("update Tb_Ticket_Order set OutOrderPayFlag=1 where OrderId='" + Order.OrderId + "'");
                    }
                }
                #endregion
            }
            else if (Order.PolicySource == 7)
            {
                #region
                PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();

                BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);
                string pmAccout = BS.JieKouZhangHao.Split('|')[3].Split('^')[0];

                string pmPassword = BS.JieKouZhangHao.Split('|')[3].Split('^')[1];
                string pmAg = BS.JieKouZhangHao.Split('|')[3].Split('^')[2];
                PMService.PMService Pmservice = new PMService.PMService();
                DataSet dsReson = Pmservice.PMOrderQuery(Order.OutOrderId, pmAccout, pmPassword);

                if (dsReson != null && !dsReson.Tables[0].Columns.Contains("resp_Text"))
                {
                    msg = ReturnPTorderStumesg(dsReson.Tables[0].Rows[0]["status"].ToString(), "7");
                    if (dsReson.Tables[0].Rows[0]["status"].ToString() == "11"
                        || dsReson.Tables[0].Rows[0]["status"].ToString() == "13"
                        || dsReson.Tables[0].Rows[0]["status"].ToString() == "14"
                        || dsReson.Tables[0].Rows[0]["status"].ToString() == "12"
                        || dsReson.Tables[0].Rows[0]["status"].ToString() == "90"
                        || dsReson.Tables[0].Rows[0]["status"].ToString() == "99")
                    {
                        baseDataManage.ExecuteNonQuerySQLInfo("update Tb_Ticket_Order set OutOrderPayFlag=1 where OrderId='" + Order.OrderId + "'");
                    }
                }
                #endregion
            }
            else if (Order.PolicySource == 8)
            {
                #region
                PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);

                string Accout51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[0];

                string Ag51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[2];
                w_51bookService._51bookService boolService = new w_51bookService._51bookService();
                DataSet dsReson = boolService.bookGetPolicyOrderStatusByOrderNo(Accout51book, Order.OutOrderId, "1", Ag51book);
                if (dsReson != null)
                {
                    msg = dsReson.Tables[0].Rows[0]["orderStatus"].ToString();
                    if (dsReson.Tables[0].Rows[0]["orderNo"].ToString() == "6"
                        || dsReson.Tables[0].Rows[0]["orderNo"].ToString() == "12"
                        || dsReson.Tables[0].Rows[0]["orderNo"].ToString() == "26"
                        || dsReson.Tables[0].Rows[0]["orderNo"].ToString() == "13"
                        || dsReson.Tables[0].Rows[0]["orderNo"].ToString() == "38")
                    {
                        baseDataManage.ExecuteNonQuerySQLInfo("update Tb_Ticket_Order set OutOrderPayFlag=1 where OrderId='" + Order.OrderId + "'");
                    }
                }
                #endregion
            }
            else if (Order.PolicySource == 10)
            {
                #region
                PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(supBaseParametersList);

                string yeeXingAccout = BS.JieKouZhangHao.Split('|')[6].Split('^')[0];

                string yeeXingAccout2 = BS.JieKouZhangHao.Split('|')[6].Split('^')[1];
                w_YeeXingService.YeeXingSerivce YeeXingService = new w_YeeXingService.YeeXingSerivce();
                DataSet dsReson = YeeXingService.OrderQueryContract(yeeXingAccout, yeeXingAccout2, Order.OutOrderId, Order.OrderId);
                if (dsReson != null)
                {
                    msg = ReturnPTorderStumesg(dsReson.Tables[1].Rows[0]["orderState"].ToString(), "10");
                    if (dsReson.Tables[1].Rows[0]["orderState"].ToString() == "2"
                        || dsReson.Tables[1].Rows[0]["orderState"].ToString() == "4"
                        || dsReson.Tables[1].Rows[0]["orderState"].ToString() == "5"
                        || dsReson.Tables[1].Rows[0]["orderState"].ToString() == "11"
                        || dsReson.Tables[1].Rows[0]["orderState"].ToString() == "12")
                    {
                        baseDataManage.ExecuteNonQuerySQLInfo("update Tb_Ticket_Order set OutOrderPayFlag=1 where OrderId='" + Order.OrderId + "'");
                        //bool rs = (bool)baseDataManage.CallMethod("Tb_Ticket_Order", "Update", null, new object[] { Order });
                    }
                }
                #endregion
            }
            if (msg != "")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
            }

        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('查询状态失败!');", true);
        }
    }

    /// <summary>
    /// bindDictionary
    /// </summary>
    public void bindPolicySource()
    {
        try
        {
            string strValue = "";
            strValue += "<option value='" + PbProject.Model.definitionParam.PolicySourceParam.b517na + "' check='true'>517</option>";
            strValue += "<option value='" + PbProject.Model.definitionParam.PolicySourceParam.b51book + "' check='true'>51book</option>";
            strValue += "<option value='" + PbProject.Model.definitionParam.PolicySourceParam.b8000yi + "' check='true'>8000yi</option>";
            strValue += "<option value='" + PbProject.Model.definitionParam.PolicySourceParam.baiTuo + "' check='true'>百拓</option>";
            strValue += "<option value='" + PbProject.Model.definitionParam.PolicySourceParam.bPiaoMeng + "' check='true'>票盟</option>";
            strValue += "<option value='" + PbProject.Model.definitionParam.PolicySourceParam.bToday + "' check='true'>今日</option>";
            strValue += "<option value='" + PbProject.Model.definitionParam.PolicySourceParam.bYeeXing + "' check='true'>易行</option>";
            //for (int i = 0; i < mDictionary.Count; i++)
            //{
            //    strValue += "<option value='" + mDictionary[i].ChildID + "' check='true'>" + mDictionary[i].ChildName + "</option>";
            //}
            hidSelValuesByPol.Value = strValue;
        }
        catch (Exception ex)
        {
        }
    }

    /// <summary>
    /// 计算平台返回订单状态
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public string ReturnPTorderStumesg(string code, string policySource)
    {
        //订单来源：1 本地B2B, 2 本地BSP,3 517,4 百拓,5 8000翼,6 今日,7 票盟,8 51book ,9 共享,10易行
        if (policySource == "3")//百拓
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
        else if (policySource == "4")//517
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
        else if (policySource == "5")//8000yi
        {
            #region 8000yi
            #endregion
        }
        else if (policySource == "10")//易行
        {
            #region 易行

            switch (code)
            {
                case "1": code = "等待支付"; break;
                case "2": code = "支付成功"; break;
                case "3": code = "处理完成"; break;
                case "4": code = "客户删除"; break;
                case "5": code = "已出票,票到付款"; break;
                case "6": code = "自动出票失败"; break;
                case "9": code = "申请取消"; break;
                case "10": code = "升舱被拒绝"; break;
                case "11": code = "出票中"; break;
                case "12": code = "拒绝出票"; break;
                case "13": code = "客户删除"; break;
                default: code = "尚未支付"; break;
            }
            #endregion
        }
        return code;
    }

    #region 手动代付
    private bool PayBy517(PbProject.Model.Tb_Ticket_Order Order, List<PbProject.Model.Bd_Base_Parameters> mBP)
    {
        bool Is517na = true;
        DataSet dsReson = new DataSet();
        DataSet dsResonPay = new DataSet();
        string sql = " update Tb_Ticket_Order set ";
        try
        {

            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();


            BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);
            string Accout517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[0];
            string Password517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[1];
            string Ag517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[2];
            string PayAccout517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[3];
            string PayPassword517 = BS.JieKouZhangHao.Split('|')[0].Split('^')[4];


            w_517WebService._517WebService ServiceBy517 = new w_517WebService._517WebService();
            List<PbProject.Model.Tb_Ticket_SkyWay> skyList = new PbProject.Logic.Order.Tb_Ticket_SkyWayBLL().GetSkyWayListBySQLWhere("OrderId='" + Order.OrderId + "'");
            sql = " update Tb_Ticket_Order set ";
            if (BS.KongZhiXiTong.Contains("54"))//开启517接口预存款支付
            {
                dsResonPay = ServiceBy517.OrderPay(Accout517, Password517, Ag517, PayAccout517, PayPassword517, Order.OutOrderId, Order.OutOrderPayMoney, "", Order.PNR);
            }
            else
            {
                dsResonPay = ServiceBy517.OrderNoPwdPay(Accout517, Password517, Order.OutOrderId, Order.OutOrderPayMoney, Ag517);
            }
            if (dsResonPay != null)
            {
                string mes517 = "";
                for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                    {
                        mes517 = mes517 + dsResonPay.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                    }

                    mes517 = mes517 + "|";
                }
                if (mes517 == "False%%%/|")//代付失败，可能为余额不足
                {

                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "于 " + DateTime.Now + " 517手工代付失败：请检查自动代付支付宝余额";
                    OrderLog.WatchType = 1;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                    return false;
                }
                if (dsResonPay.Tables[0].TableName == "error")//代付失败，记录日志
                {
                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "于 " + DateTime.Now + " 517手工代付失败：" + dsResonPay.Tables[0].Rows[0][0].ToString() + "," + dsResonPay.Tables[0].Rows[0][1].ToString();
                    OrderLog.WatchType = 1;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                    return false;
                }
                if (dsResonPay.Tables[0].Rows[0]["PaySuccess"].ToString() == "True")
                {
                    sql += " OutOrderPayFlag=1,PayStatus=1";
                    sql += " where OrderId='" + Order.OrderId + "'";
                    sqlbase.ExecuteNonQuerySQLInfo(sql);

                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "517手工代付成功!";
                    OrderLog.WatchType = 1;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                }
            }
        }
        catch (Exception ex)
        {
            Is517na = false;
        }
        return Is517na;
    }

    private bool BaiTuoPay(PbProject.Model.Tb_Ticket_Order Order, List<PbProject.Model.Bd_Base_Parameters> mBP)
    {
        bool IsOk = true;
        try
        {
            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();

            BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);
            PbProject.Logic.PTInterface.PTBybaituo OrderbaiTuoManager = new PbProject.Logic.PTInterface.PTBybaituo(Order, BS);
            string sql = " update Tb_Ticket_Order set ";
            string Message = "";
            try
            {
                string SendURL = OrderbaiTuoManager.BaiTuoPaySend(Order.OrderId, "1");
                w_BTWebService.BaiTuoWeb BaiTuoService = new w_BTWebService.BaiTuoWeb();
                if (SendURL != "")
                {
                    Message = HttpUtility.UrlDecode(BaiTuoService.GetUrlData(SendURL));
                }
            }
            catch (Exception ex)
            {
                IsOk = false;
                Message = "";
            }
            if (Message != "")
            {
                if (Message.Substring(Message.IndexOf("<PaymentResult>") + "<PaymentResult>".Length, 1) == "T" || Message.Substring(Message.IndexOf("<PaymentResult>") + "<PaymentResult>".Length, 1) == "1")
                {
                    sql += " OutOrderPayFlag=1,PayStatus=1";
                    sql += " where OrderId='" + Order.OrderId + "'";
                    sqlbase.ExecuteNonQuerySQLInfo(sql);

                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "百拓手工代付成功!";
                    OrderLog.WatchType = 2;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                }
                else
                {
                    IsOk = false;

                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "百拓手工代付失败：" + Message;
                    OrderLog.WatchType = 2;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                }
            }
            else
            {
                IsOk = false;
            }
        }
        catch (Exception ex)
        {
            IsOk = false;
        }
        return IsOk;
    }

    private bool PayForToday(PbProject.Model.Tb_Ticket_Order Order, List<PbProject.Model.Bd_Base_Parameters> mBP)
    {
        bool IsToday = true;
        try
        {
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);

            string todayAccout = BS.JieKouZhangHao.Split('|')[4].Split('^')[0];

            string todayAccout2 = BS.JieKouZhangHao.Split('|')[4].Split('^')[1];
            string sql = " update Tb_Ticket_Order set ";

            w_TodayService.WTodayService WSvcToday = new w_TodayService.WTodayService();
            DataSet dsResonPay = WSvcToday.AutoPayOrder(todayAccout2, Order.OutOrderId);
            string mesTodayPay = "";
            for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                {
                    mesTodayPay = mesTodayPay + dsResonPay.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                }

                mesTodayPay = mesTodayPay + "|";
            }
            if (dsResonPay != null && dsResonPay.Tables.Count > 0)
            {
                if (dsResonPay.Tables[0].Rows.Count > 0 && dsResonPay.Tables[0].Rows[0]["Result"].ToString() == "T")
                {
                    //  支付成功
                    sql += " OutOrderPayFlag=1,PayStatus=1";
                    sql += " where OrderId='" + Order.OrderId + "'";
                    sqlbase.ExecuteNonQuerySQLInfo(sql);

                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "今日手工代付成功!";
                    OrderLog.WatchType = 2;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                }
                else
                {
                    //  支付失败
                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "于 " + DateTime.Now + " 今日手工代付失败：请检查自动代付支付宝余额";
                    OrderLog.WatchType = 2;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                    IsToday = false;
                }
            }
        }
        catch (Exception ex)
        {
            IsToday = false;
        }
        return IsToday;
    }
    private bool PayFor8000Y(PbProject.Model.Tb_Ticket_Order Order, List<PbProject.Model.Bd_Base_Parameters> mBP)
    {
        bool Is8000Y = true;
        string sql = " update Tb_Ticket_Order set ";

        PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
        PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
        BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);
        //落地运营商和供应商公司参数信息

        string Accout8000yi = BS.JieKouZhangHao.Split('|')[5].Split('^')[0];

        string Password8000yi = BS.JieKouZhangHao.Split('|')[5].Split('^')[1];
        string Alipaycode8000yi = BS.JieKouZhangHao.Split('|')[5].Split('^')[2];
        try
        {
            w_8000YService.W8000YService WSvc8000Y = new w_8000YService.W8000YService();
            DataSet dsResonPay = WSvc8000Y.AutomatismPay(Accout8000yi, Password8000yi, Order.OutOrderId, Alipaycode8000yi);
            string mes8000YPay = "";
            for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                {
                    mes8000YPay = mes8000YPay + dsResonPay.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                }

                mes8000YPay = mes8000YPay + "|";
            }
            if (dsResonPay != null && dsResonPay.Tables.Count > 0)
            {
                try
                {
                    //  支付失败
                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "于 " + DateTime.Now + " 8000Y手工代付失败：请检查自动代付支付宝余额";
                    OrderLog.WatchType = 2;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                    Is8000Y = false;
                }
                catch
                {
                    //  支付成功
                    sql += " OutOrderPayFlag=1,PayStatus=1";
                    sql += " where OrderId='" + Order.OrderId + "'";
                    sqlbase.ExecuteNonQuerySQLInfo(sql);

                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "8000Y手工代付成功!";
                    OrderLog.WatchType = 2;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                }
            }
        }
        catch (Exception ex)
        {
            Is8000Y = false;
        }
        return Is8000Y = false;
    }

    private bool bookPay(PbProject.Model.Tb_Ticket_Order Order, List<PbProject.Model.Bd_Base_Parameters> mBP)
    {
        bool Isbook = true;

        string sql = " update Tb_Ticket_Order set ";

        PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
        PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
        BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);

        string Accout51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[0];

        string Password51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[1];

        string Ag51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[2];

        string Url51book = BS.JieKouZhangHao.Split('|')[1].Split('^')[3];
        try
        {
            w_51bookService._51bookService bookService = new w_51bookService._51bookService();
            DataSet dsResonPay = bookService.bookPayPolicyOrderByPNR(Accout51book, Order.OutOrderId, Accout51book, Password51book, Ag51book);
            if (dsResonPay != null)
            {
                string mes51bookPay = "";
                for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                    {
                        mes51bookPay = mes51bookPay + dsResonPay.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                    }

                    mes51bookPay = mes51bookPay + "|";
                }
                if (dsResonPay.Tables[0].Columns.Contains("ErorrMessage"))//代付失败，记录日志
                {
                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "于 " + DateTime.Now + " 51book手工代付失败：" + dsResonPay.Tables[0].Rows[0]["ErorrMessage"].ToString();
                    OrderLog.WatchType = 2;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                    Isbook = false;
                }
                if (dsResonPay.Tables[0].Rows[0]["orderStatus"].ToString() == "2")
                {
                    sql += " OutOrderPayFlag=1,PayStatus=1";
                    sql += " where OrderId='" + Order.OrderId + "'";
                    sqlbase.ExecuteNonQuerySQLInfo(sql);

                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "51book手工代付成功!";
                    OrderLog.WatchType = 2;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                }
            }
        }
        catch (Exception ex)
        {
            Isbook = false;
        }
        return Isbook;
    }
    private bool PMPay(PbProject.Model.Tb_Ticket_Order Order, List<PbProject.Model.Bd_Base_Parameters> mBP)
    {
        bool IsPm = true;
        string sql = " update Tb_Ticket_Order set ";
        try
        {
            PMService.PMService PMService = new PMService.PMService();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();

            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);
            string pmAccout = BS.JieKouZhangHao.Split('|')[3].Split('^')[0];

            string pmPassword = BS.JieKouZhangHao.Split('|')[3].Split('^')[1];
            string pmAg = BS.JieKouZhangHao.Split('|')[3].Split('^')[2];

            DataSet dsResonPay = PMService.PMPay(Order.OutOrderId, pmAccout, pmAg);
            if (dsResonPay != null)
            {
                string mesPMPay = "";
                for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                    {
                        mesPMPay = mesPMPay + dsResonPay.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                    }

                    mesPMPay = mesPMPay + "|";
                }
                if (dsResonPay.Tables[0].Rows[0]["statuscode"].ToString() == "0")
                {
                    sql += " OutOrderPayFlag=1,PayStatus=1";
                    sql += " where OrderId='" + Order.OrderId + "'";
                    sqlbase.ExecuteNonQuerySQLInfo(sql);

                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "票盟手工代付成功!";
                    OrderLog.WatchType = 2;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                }
                else
                {
                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "于 " + DateTime.Now + " 票盟手工代付失败：" + dsResonPay.Tables[0].Rows[0]["statuscode"].ToString() + ":" + dsResonPay.Tables[0].Rows[0]["resp_Text"].ToString();
                    OrderLog.WatchType = 2;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                    IsPm = false;
                }

            }
        }
        catch (Exception ex)
        {
            IsPm = false;
        }
        return IsPm;
    }

    private bool PayByYeeXing(PbProject.Model.Tb_Ticket_Order Order, List<PbProject.Model.Bd_Base_Parameters> mBP)
    {
        bool IsYeeXingna = true;
        DataSet dsReson = new DataSet();
        DataSet dsResonPay = new DataSet();
        string sql = " update Tb_Ticket_Order set ";
        try
        {

            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);

            string yeeXingAccout = BS.JieKouZhangHao.Split('|')[6].Split('^')[0];
            string yeeXingAccout2 = BS.JieKouZhangHao.Split('|')[6].Split('^')[1];

            w_YeeXingService.YeeXingSerivce ServiceByYeeXing = new w_YeeXingService.YeeXingSerivce();

            dsResonPay = ServiceByYeeXing.PayOutContract(yeeXingAccout, yeeXingAccout2, Order.OutOrderId, Order.OutOrderPayMoney.ToString(), "1", "", "");
            if (dsResonPay != null)
            {
                string mesYeeXing = "";
                for (int i = 0; i < dsResonPay.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < dsResonPay.Tables[0].Columns.Count; j++)
                    {
                        mesYeeXing = mesYeeXing + dsResonPay.Tables[0].Rows[i][j].ToString() + "%%%/";//DataTable转化成String类型
                    }

                    mesYeeXing = mesYeeXing + "|";
                }
                if (dsResonPay.Tables[0].Rows[0]["is_success"].ToString() == "F")
                {
                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();

                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "于 " + DateTime.Now + " 易行手工代付失败：" + dsResonPay.Tables[0].Rows[0][0].ToString() + "," + dsResonPay.Tables[0].Rows[0][1].ToString();
                    OrderLog.WatchType = 2;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                    IsYeeXingna = false;
                }
                if (dsResonPay.Tables[0].Rows[0]["is_success"].ToString() == "T")
                {
                    sql += " OutOrderPayFlag=1,PayStatus=1";
                    sql += " where OrderId='" + Order.OrderId + "'";
                    sqlbase.ExecuteNonQuerySQLInfo(sql);

                    #region 记录操作日志
                    //添加操作订单的内容
                    PbProject.Model.Log_Tb_AirOrder OrderLog = new PbProject.Model.Log_Tb_AirOrder();


                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = Order.OrderId;
                    OrderLog.OperType = "修改";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperContent = "易行手工代付成功!";
                    OrderLog.WatchType = 2;
                    string tempSql = PbProject.Dal.Mapping.MappingHelper<PbProject.Model.Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlbase.ExecuteNonQuerySQLInfo(tempSql);
                    #endregion
                }

            }
        }
        catch (Exception ex)
        {
            IsYeeXingna = false;
        }
        return IsYeeXingna;
    }
    #endregion


}