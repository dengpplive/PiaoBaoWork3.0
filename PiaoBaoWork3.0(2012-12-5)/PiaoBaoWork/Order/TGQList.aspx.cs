using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using DataBase.Data;
using PbProject.Model;
using PbProject.WebCommon.Utility;

/// <summary>
/// 退改签申请  分销、采购 页面
/// </summary>
public partial class Order_TGQList : BasePage
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
                ViewState["orderBy"] = " CreateTime desc ";

                //txtFromDate1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                //txtFromDate2.Value = DateTime.Now.ToString("yyyy-MM-dd");

                txtCreateTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                txtCreateTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");

                Curr = 1;
                AspNetPager1.PageSize = 20;
                Con = SelWhere();
            }
        }
        catch (Exception)
        {
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
    /// 绑定
    /// </summary>
    private void PageDataBind()
    {
        try
        {
            int TotalCount = 0;

            IHashObject outParams = new HashObject();
            string sqlWhere = "OrderId in(select OrderId from Tb_Ticket_Passenger where IsBack=0 and OrderId in(select OrderId from Tb_Ticket_Order where " + Con + "))";

            //指定参数类型 第一个参数为out输出类型
            //key 为参数索引从1开始 value为引用类型 out ref
            outParams.Add("1", "out");
            PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
            List<Tb_Ticket_Order> list = baseDataManage.CallMethod("Tb_Ticket_Order", "GetBasePager1", outParams,
                new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", sqlWhere, ViewState["orderBy"].ToString() }) as List<Tb_Ticket_Order>;

            TotalCount = outParams.GetValue<int>("1");

            AspNetPager1.RecordCount = TotalCount;
            AspNetPager1.CurrentPageIndex = Curr;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;

            repList.DataSource = list;
            repList.DataBind();
        }
        catch (Exception ex)
        {
            repList.DataSource = null;
            repList.DataBind();
        }
    }

    /// <summary>
    /// 翻页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        PageDataBind();
    }

    /// <summary>
    /// 返回行程
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public string ReturnTravel(string str)
    {
        string[] value = str.Split('/');
        if (value.Length > 1)
        {
            return value[0] + "<br/>" + value[1];
        }
        return str;
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
                Message = ChildId == "1" ? "已付" : "未付";
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
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    private string SelWhere()
    {
        StringBuilder StrWhere = new StringBuilder(" PayStatus=1 ");

        StrWhere.Append(" and OwnerCpyNo='" + mUser.CpyNo + "' ");
        try
        {

            #region 查询订单状态
            //<asp:ListItem Text="已出票交易结束" Value="1"></asp:ListItem> 4,
            //<asp:ListItem Text="进行中的退票" Value="2"></asp:ListItem> 7,  
            //<asp:ListItem Text="进行中的废票" Value="3"></asp:ListItem>8,
            //<asp:ListItem Text="进行中的改签" Value="4"></asp:ListItem>6,15
            //<asp:ListItem Text="审核通过" Value="5"></asp:ListItem>9,11,13
            //<asp:ListItem Text="拒绝申请的订单" Value="6"></asp:ListItem>10,12,14,18
            //<asp:ListItem Text="交易结束" Value="7"></asp:ListItem> 16,17,19

            if (Hid_num.Value == "1")
                StrWhere.Append(" and OrderStatusCode=4 ");
            else if (Hid_num.Value == "2")
                StrWhere.Append(" and OrderStatusCode in(7,11,29) ");
            else if (Hid_num.Value == "3")
                StrWhere.Append(" and OrderStatusCode in(8,13,30) ");
            else if (Hid_num.Value == "4")
                StrWhere.Append(" and OrderStatusCode in(6,9,15,31) ");
            else if (Hid_num.Value == "5")
                StrWhere.Append(" and OrderStatusCode in(9,11,13) ");
            else if (Hid_num.Value == "6")
                StrWhere.Append(" and OrderStatusCode in(10,12,14,18) ");
            else if (Hid_num.Value == "7")
                StrWhere.Append(" and OrderStatusCode in(16,17,19) ");

            #endregion

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
            if (!string.IsNullOrEmpty(txtFromDate2.Value.Trim()))
                StrWhere.Append(" and AirTime <'" + txtFromDate2.Value.Trim() + " 23:59:59'");

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
            PbProject.Logic.Order.Tb_Ticket_OrderBLL orderBll = new PbProject.Logic.Order.Tb_Ticket_OrderBLL();

            string Id = e.CommandArgument.ToString();
            Tb_Ticket_Order Order = orderBll.GetTicketOrderById(Id);

            if (e.CommandName == "Detail")    //订单详情
            {
                ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "location.href='OrderDetail.aspx?Id=" + Id + "&Url=TGQList.aspx&currentuserid=" + this.currentuserid.Value.ToString()+"';", true);
                //Response.Redirect("OrderDetail.aspx?Id=" + Id + "&Url=TGQList.aspx&currentuserid=" + this.currentuserid.Value.ToString());
            }
            else if (e.CommandName == "Process") //退改签处理
            {
                //if (!string.IsNullOrEmpty(Order.LockLoginName) && Order.LockLoginName != mUser.LoginName)
                //{
                //    //订单锁定 并且 非登录本账号锁定
                //    ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('订单已锁,不能退改签申请！')", true);
                //}
                //else
                //{
                //    orderBll.LockOrder(true, Id, mUser, mCompany);
                    Response.Redirect("TGQApplication.aspx?Id=" + Id + "&Url=TGQList.aspx&currentuserid=" + this.currentuserid.Value.ToString());
                //}
            }
            else if (e.CommandName == "Pay") //改签支付
            {
                Response.Redirect("../Buy/Payment.aspx?Id=" + Id + "&Url=OperationingOrderList.aspx&currentuserid=" + this.currentuserid.Value.ToString());
            }
            else if (e.CommandName == "PayNo") //拒绝补差
            {
                string msg = "";

                Order.OrderStatusCode = 18;
                //orderBll.OperOrderGQ(Order, mUser, mCompany);
                string sqlWhere = " OrderId='" + Order.OrderId + "' ";
                List<Tb_Ticket_Passenger> PassengerList = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Passenger>;
                bool reuslt = orderBll.OperOrderGQ(Order, PassengerList, mUser, mCompany);
                if (reuslt)
                {
                    PageDataBind();
                    msg = "拒绝补差成功";
                }
                else
                {
                    msg = "拒绝补差失败";
                }

                ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "')", true);
            }
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Curr = 1;
        Con = SelWhere();
        AspNetPager1.CurrentPageIndex = Curr;
        PageDataBind();
    }
    /// <summary>
    /// 清空条件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtFromDate2.Value = DateTime.Now.ToString("yyyy-MM-dd");
        txtCreateTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
        txtCreateTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");

        txtFlightCode.Text = "";
        txtFromCity.Value = "";
        txtFromDate1.Value = "";
        txtFromDate2.Value = "";
        txtOrderId.Text = "";
        txtPassengerName.Text = "";
        txtPNR.Text = "";
        txtToCity.Value = "";
        //ddlStatus.SelectedValue = "1";
    }

    /// <summary>
    /// 页面数据信息绑定
    /// </summary>
    /// <param name="ParentId">父类编号</param>
    /// <param name="ChildId">子类编号</param>
    /// <returns>返回处理后的相对应信息</returns>
    public string DataSourceMessage(int ParentId, string ChildId)
    {
        string Message = "";
        try
        {
            //if (ParentId == 1)
            //{

            //}
            //else if (ParentId == 2)
            //{

            //}
            //else
            //{
            Message = GetDictionaryName(ParentId.ToString(), ChildId);
            //}

            Message = string.Join("<br />", Message.Split(new string[] { ",", "，", " " }, StringSplitOptions.RemoveEmptyEntries));
        }
        catch (Exception)
        {

        }
        return Message;
    }

    /// <summary>
    /// 航班号信息判断
    /// </summary>
    /// <param name="CarryCode">承运人代码</param>
    /// <param name="FlightCode">航班号代码</param>
    /// <returns>返回处理后的航班号信息</returns>
    public string DataSourceFlightCode(string CarryCode, string FlightCode)
    {
        string[] Carry = CarryCode.Split('/');
        string Message = "";
        try
        {
            if (Carry.Length > 1)
            {
                if (Carry[1].ToString() != "")
                {
                    for (int i = 0; i < Carry.Length; i++)
                    {
                        if (i < Carry.Length - 1)
                        {
                            Message = Message + Carry[i] + FlightCode.Split('/')[i].ToString() + "<br/>";
                        }
                        else
                        {
                            Message = Message + Carry[i] + FlightCode.Split('/')[i].ToString();
                        }
                    }
                }
                else
                {
                    Message = CarryCode + FlightCode;
                }
            }
            else
            {
                Message = CarryCode + FlightCode;
            }
        }
        catch (Exception)
        {
            Message = CarryCode + FlightCode;
        }
        return Message;
    }

    /// <summary>
    /// repList_ItemDataBound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void repList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        try
        {
            LinkButton lbtnProcess = e.Item.FindControl("lbtnProcess") as LinkButton;
            LinkButton lbtnPay = e.Item.FindControl("lbtnPay") as LinkButton;
            LinkButton lbtnPayNo = e.Item.FindControl("lbtnPayNo") as LinkButton;
            //LinkButton lbtnDetail = e.Item.FindControl("lbtnDetail") as LinkButton;

            string orderStatusCode = (e.Item.FindControl("Hid_OrderStatusCode") as HiddenField).Value; // 状态值

            // 支付状态查询
            if (orderStatusCode == "4")
                lbtnProcess.Visible = true;

            //退款处理 ： 注意 只能是 取消出票 退款中的
            if (orderStatusCode == "9")
            {
                lbtnPay.Visible = true;
                lbtnPayNo.Visible = true;
            }

            #region 显示、隐藏

            if (lbtnProcess.Visible == true)
                lbtnProcess.Text += "<br/>";

            if (lbtnPay.Visible == true)
                lbtnPay.Text += "<br/>";

            if (lbtnPayNo.Visible == true)
                lbtnPayNo.Text += "<br/>";

            ////订单详情
            //LinkButton lbtnDetail = e.Item.FindControl("lbtnDetail") as LinkButton;
            //if (lbtnDetail.Visible == true)
            //    lbtnDetail.Text += "<br/>";

            #endregion
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 判断PNR是否显示
    /// </summary>
    /// <param name="PNR">PNR</param>
    /// <param name="OrderSource">订单来源</param>
    /// <param name="PayStatus">支付状态</param>
    /// <returns>返回处理后的信息</returns>
    public string PNRShow(string PNR, string OrderSource, string PayStatus, string str)
    {
        string Message = "";
        try
        {
            if (Message != "" && str != null && str != "")
            {
                Message += "<br/><font style='color:red;'>" + str + "</font><br/>";
            }
            if (Message != "")
            {
                //Message += "<a href=javascript:copyValue('" + PNR + "') >&nbsp;复制</a>";
                Message += "<input id='btcopy' type='button' value='复制' onclick=copyValue('" + PNR + "') />";
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

                if (stringValue == "9")
                    //rsColor = "Red"; //Black
                    rsColor = "#DC5C17";
                else
                    rsColor = "Black"; //Black
            }
        }
        catch (Exception ex)
        {

        }
        return rsColor;
    }
}