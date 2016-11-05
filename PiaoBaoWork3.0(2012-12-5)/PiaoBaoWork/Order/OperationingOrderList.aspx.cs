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
using PbProject.Logic.PID;

/// <summary>
/// 未付款订单
/// </summary>
/// 
public partial class Order_OperationingOrderList : BasePage
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
                Curr = 1;
                AspNetPager1.PageSize = 20;
                ViewState["orderBy"] = " CreateTime desc ";

                //txtFromDate1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                //txtFromDate2.Value = DateTime.Now.ToString("yyyy-MM-dd");

                txtCreateTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                txtCreateTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");
                Hid_IsOpenTFSplitPnr.Value = KongZhiXiTong.Contains("|32|") ? "1" : "0";

                Con = SelWhere();
                OrderListDataBind();
            }
        }
        catch (Exception) { }
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
                new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Tb_Ticket_Order>;

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
        StringBuilder StrWhere = new StringBuilder();
        StrWhere.Append(" OwnerCpyNo='" + mUser.CpyNo + "' ");

        //StrWhere.Append(" and A1=1 "); // 只查确认的订单

        StrWhere.Append(" and (OrderStatusCode=1 or OrderStatusCode=9) "); //订单状态
        try
        {
            //订单号
            if (!string.IsNullOrEmpty(txtOrderId.Text.Trim()))
                StrWhere.Append(" and OrderId='" + txtOrderId.Text.Trim() + "' ");
            //pnr
            if (!string.IsNullOrEmpty(txtPNR.Text.Trim()))
                StrWhere.Append(" and PNR='" + txtPNR.Text.Trim() + "' ");
            //乘机人
            if (!string.IsNullOrEmpty(txtPassengerName.Text.Trim()))
                StrWhere.Append(" and PassengerName like'%" + txtPassengerName.Text.Trim() + "%' ");
            //航班号
            if (!string.IsNullOrEmpty(txtFlightCode.Text.Trim()))
                StrWhere.Append(" and FlightCode ='" + txtFlightCode.Text.Trim() + "' ");
            //航空公司
            if (!string.IsNullOrEmpty(SelectAirCode1.Value.Trim()))
                StrWhere.Append(" and CarryCode ='" + SelectAirCode1.Value.Trim() + "' ");

            ////乘机日期
            if (!string.IsNullOrEmpty(txtFromDate1.Value.Trim()))
                StrWhere.Append(" and AirTime >'" + txtFromDate1.Value.Trim() + " 00:00:00'");
            ////乘机日期
            if (!string.IsNullOrEmpty(txtFromDate2.Value.Trim()))
                StrWhere.Append(" and AirTime <'" + txtFromDate2.Value.Trim() + " 23:59:59'");

            //创建日期
            if (!string.IsNullOrEmpty(txtCreateTime1.Value.Trim()))
                StrWhere.Append(" and CreateTime >'" + txtCreateTime1.Value.Trim() + " 00:00:00'");
            //创建日期
            if (!string.IsNullOrEmpty(txtCreateTime2.Value.Trim()))
                StrWhere.Append(" and CreateTime <'" + txtCreateTime2.Value.Trim() + " 23:59:59'");

            ////城市控件
            //if (txtFromCity.Value.Trim() != "" && txtFromCity.Value.Trim() != "中文/拼音" && txtFromCity.Value.Trim() != "中文/英文")
            //    StrWhere.Append(" and Travel like '" + txtFromCity.Value.Trim() + "%'");
            //if (txtToCity.Value.Trim() != "" && txtToCity.Value.Trim() != "中文/拼音" && txtToCity.Value.Trim() != "中文/英文")
            //    StrWhere.Append(" and Travel like '%" + txtToCity.Value.Trim() + "'");


            ////城市控件
            if (hidFromCity.Value.Trim() != "" && txtFromCity.Value.Trim() != "" && txtFromCity.Value.Trim() != "中文/拼音" && txtFromCity.Value.Trim() != "中文/英文")
                StrWhere.Append(" and TravelCode like '" + hidFromCity.Value.Trim() + "%'");
            if (hidToCity.Value.Trim() != "" && txtToCity.Value.Trim() != "" && txtToCity.Value.Trim() != "中文/拼音" && txtToCity.Value.Trim() != "中文/英文")
                StrWhere.Append(" and TravelCode like '%" + hidToCity.Value.Trim() + "'");
        }
        catch (Exception)
        {

        }
        return StrWhere.ToString();

    }

    #region 绑定事件

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
        try
        {
            string Id = e.CommandArgument.ToString();
            string temp = "";

            if (e.CommandName == "NewPolicy") //重新确认订单
            {
                temp = (e.Item.FindControl("Hid_IsChdFlag") as HiddenField).Value;
                if (temp.ToUpper() == "TRUE" || temp.ToUpper() == "1")
                    Response.Redirect("../Buy/Confirmation.aspx?ChildOrderId=" + Id + "&currentuserid=" + this.currentuserid.Value.ToString() + "");
                else
                    Response.Redirect("../Buy/Confirmation.aspx?AdultOrderId=" + Id + "&currentuserid=" + this.currentuserid.Value.ToString() + "");
            }
            else if (e.CommandName == "Detail") //订单详情
            {
                Response.Redirect("OrderDetail.aspx?Id=" + Id + "&Url=" + "OperationingOrderList.aspx" + "&currentuserid=" + this.currentuserid.Value.ToString());
            }
            else if (e.CommandName == "Pay")//支付
            {
                Response.Redirect("../Buy/PayMent.aspx?Id=" + Id + "&Url=OperationingOrderList.aspx" + "&currentuserid=" + this.currentuserid.Value.ToString());
            }
            else if (e.CommandName == "CancelOrder")//取消订单
            {
                temp = (e.Item.FindControl("Hid_PolicySource") as HiddenField).Value;

                #region 判断改编码是否已经支付过

                string PNR = (e.Item.FindControl("Hid_PNR") as HiddenField).Value;


                List<PbProject.Model.Tb_Ticket_Order> OrderList = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().GetListBySqlWhere("PNR='" + PNR + "' and (OrderStatusCode=4 or OrderStatusCode=3)");

                if (OrderList != null && OrderList.Count > 0)
                {
                    temp = "0"; // 该编码已经支付成功 或 出票了。 取消此订单时， 只能取消订单，不能消编码
                }

                #endregion

                ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "quXiaoOrder('" + Id + "','" + temp + "');", true);
            }
        }
        catch (Exception)
        {

        }
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
            HiddenField Hid_A1 = e.Item.FindControl("Hid_A1") as HiddenField;

            LinkButton lbtnNewPolicy = e.Item.FindControl("lbtnNewPolicy") as LinkButton;
            LinkButton lbtnPay = e.Item.FindControl("lbtnPay") as LinkButton;
            LinkButton lbtnDetail = e.Item.FindControl("lbtnDetail") as LinkButton;
            LinkButton lbtnCancelOrder = e.Item.FindControl("lbtnCancelOrder") as LinkButton;


            //暂时不能使用重新获取政策。问题在:确定订单时会重新生成账单。不能重复生成账单，冲突了
            if (Hid_A1.Value == "1")
            {
                // 已经确认的订单 

                lbtnNewPolicy.Visible = false;
                lbtnPay.Visible = true;
            }
            else
            {
                // 未确认的订单
                lbtnNewPolicy.Visible = true;
                lbtnPay.Visible = false;
            }

            if (lbtnNewPolicy.Visible == true)
                lbtnNewPolicy.Text += "<br/>";

            if (lbtnPay.Visible == true)
                lbtnPay.Text += "<br/>";

            if (lbtnDetail.Visible == true)
                lbtnDetail.Text += "<br/>";

            if (lbtnCancelOrder.Visible == true)
                lbtnCancelOrder.Text += "<br/>";

        }
        catch (Exception)
        {

        }
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
        try
        {
            Curr = 1;
            AspNetPager1.CurrentPageIndex = Curr;
            Con = SelWhere();
            OrderListDataBind();
        }
        catch (Exception)
        {
        }
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
        txtFlightCode.Text = "";

        txtFromDate1.Value = "";
        txtFromDate2.Value = "";

        txtCreateTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
        txtCreateTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");
    }


    #endregion


    /// <summary>
    /// 取消订单
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancelOrder_Click(object sender, EventArgs e)
    {
        string showMsg = "";


        try
        {
            //扩展参数
            ParamEx pe = new ParamEx();
            pe.UsePIDChannel = this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|48|") ? 2 : 0;
            //发送指令管理类
            SendInsManage SendIns = new SendInsManage(mUser.LoginName, mCompany.UninCode, pe, configparam);

            string id = Hid_id.Value; //订单id
            Tb_Ticket_Order Order = new Tb_Ticket_OrderBLL().GetTicketOrderById(id);

            if (Order != null)  ////判断订单状态
            {
                bool restult = true;
                //白屏预订 新订单等待支付 并且开启退废票（分离、取消）编码的权限 才取消编码
                if (Order.OrderSourceType == 1 && Order.OrderStatusCode == 1 && KongZhiXiTong.Contains("|32|"))
                {

                    #region 1.白屏预订2.新订单等待支付3.开启退废票（分离、取消）编码的权限  判断是否需要取消编码

                    if (Hid_isCancelPnr.Value == "1") // 取消编码
                    {
                        Hid_isCancelPnr.Value = "0";

                        if (Order.PNR != "")
                        {
                            string ErrMsg = "";
                            PnrAnalysis.PnrModel pnrModel = SendIns.GetPnr(Order.PNR, out ErrMsg);
                            //编码状态不是已经出票的PNR
                            if (pnrModel != null && !pnrModel.PnrStatus.Contains("RR"))
                            {
                                //调用取消编码的方法
                                restult = SendIns.CancelPnr(Order.PNR, Order.Office);
                            }
                        }
                        showMsg = restult == true ? "取消编码成功！" : showMsg;
                    }
                    else
                    {
                        showMsg = "取消订单,用户需要保留编码!";
                    }

                    #endregion
                }
                else
                {
                    showMsg = "取消订单";
                }



                //取消订单
                restult = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().CancelOrder(Order, mUser, mCompany, showMsg);

                if (restult)
                {
                    OrderListDataBind();

                    showMsg = "取消订单成功！";
                }
                else
                    showMsg = "取消订单失败！";

            }
            else
            {
                showMsg = "该订单不能取消！！！";
            }
        }
        catch (Exception ex)
        {
            showMsg = "操作失败";
        }

        ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + showMsg + "');", true);
    }

    /// <summary>
    /// 显示绑定数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="Data"></param>
    /// <returns></returns>
    public string ShowText(int type, params object[] Data)
    {
        string result = "";
        if (type == 1)//订单标识 儿童还是成人
        {
            //乘机人
            if (Data != null && Data.Length == 2)
            {
                string Identity = Data[0].ToString();
                string IsCHDETAdultTK = Data[1].ToString();
                //IsChdFlag （0=成人订单，1=儿童订单）
                if (Identity.ToLower() == "true" || Identity.ToLower() == "1")
                {
                    result = string.Format("<br /><font class='red'>{0}</font>", IsCHDETAdultTK == "1" ? "儿童(成人价)" : "儿童");
                }
            }
        }
        else if (type == 2)
        {
            if (Data != null && Data.Length == 2)
            {
                string ParentId = Data[0].ToString();
                string ChildId = Data[1].ToString();
                result = GetDictionaryName(ParentId, ChildId);
                result = string.Join("<br />", result.Split(new string[] { ",", "，", " " }, StringSplitOptions.RemoveEmptyEntries));
            }
        }
        else if (type == 3)
        {
            if (Data != null && Data.Length == 1)
            {
                result = string.Join("<br />", Data[0].ToString().Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries));
            }
        }
        return result;
    }
}