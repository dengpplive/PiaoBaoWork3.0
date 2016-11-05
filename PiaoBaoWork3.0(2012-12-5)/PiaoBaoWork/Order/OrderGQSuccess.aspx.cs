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

/// <summary>
/// 确认改签
/// </summary>
public partial class Order_OrderGQSuccess : BasePage
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
                if (Request.QueryString["id"] != null) 
                {
                    //订单号
                    OrderBind();
                }

                if (Request.QueryString["Url"] != null)
                {
                    string url =Request.QueryString["Url"].ToString();
                    if (!url.Contains("IsQuery"))
                    {
                        ViewState["Url"] = url + "?currentuserid=" + this.mUser.id.ToString();
                        Hid_GoUrl.Value = url + "?currentuserid=" + this.mUser.id.ToString();
                    }
                    else
                    {
                        url = Server.UrlDecode(url);
                    }
                    ViewState["Url"] = url;
                    Hid_GoUrl.Value = url;//返回  
                    spanReturn.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 绑定订单信息
    /// </summary>
    private void OrderBind()
    {
        try
        {
            string sqlWhere = " id='" + Request.QueryString["id"].ToString() + "' ";
           
            List<Tb_Ticket_Order> OrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Order>;
            Tb_Ticket_Order mOrder = OrderList != null && OrderList.Count > 0 ? OrderList[0] : null;

            if (mOrder != null)
            {
                sqlWhere = " OrderId='" + mOrder.OrderId + "' ";

                #region 订单信息

                //订单信息
                lblInPayNo.Text = mOrder.InPayNo;
                lblLockId.Text = mOrder.LockLoginName;
                lblLockTime.Text = mOrder.LockTime.ToString("yyyy-MM-dd HH:mm:ss");
                lblOrderId.Text = mOrder.OrderId;
                lblOrderSourceType.Text = GetDictionaryName("33",mOrder.OrderSourceType.ToString());
                lblOrderStatusCode.Text = GetDictionaryName("1",mOrder.OrderStatusCode.ToString());

                //lblPayMoney.Text = mOrder.PayMoney.ToString("F2");
                lblPayMoney.Text = mOrder.OrderMoney.ToString("F2");
                lblPayNo.Text = mOrder.PayNo;
                lblPayStatus.Text = (mOrder.PayStatus == 1) ? "已付" : "未付";
                lblPayWay.Text = GetDictionaryName("4", mOrder.PayWay.ToString());
                lblPNR.Text = mOrder.PNR;
                lblPolicyPoint.Text = mOrder.PolicyPoint + "/" + mOrder.ReturnPoint;
                lblPolicyPoint2.Text = mOrder.PolicyPoint.ToString();
                lblPolicyRemark.Text = mOrder.PolicyRemark;

                lblPolicySource.Text = GetDictionaryName("33", mOrder.PolicySource.ToString()); 

                // 显示 预订备注
                txtYDRemark.Text = mOrder.YDRemark;
                //退废改  申请理由
                txtTGQApplyReason.Text = mOrder.TGQApplyReason;
                // 拒绝理由
                txtTGQRefusalReason.Text = mOrder.TGQRefusalReason;

                if (mOrder.PolicySource > 2)
                {
                    trOutOrder.Visible = true;
                    //代付信息
                    lblOutOrderId.Text = mOrder.OutOrderId;
                    lblOutOrderPayFlag.Text = (mOrder.OutOrderPayFlag == true) ? "已代付" : "未代付";
                    lblOutOrderPayMoney.Text = mOrder.OutOrderPayMoney.ToString("F2");
                    lblOutOrderPayNo.Text = mOrder.OutOrderPayNo;
                }
                else
                {
                    trOutOrder.Visible = false;
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
                ViewState["Passenger"] = PassengerList;
            }

        }
        catch (Exception ex)
        {

        } 
    }


    /// <summary>
    /// 确定改签
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        bool result = false;
        try
        {
            Tb_Ticket_Order Order = ViewState["Order"] as Tb_Ticket_Order;
            List<Tb_Ticket_Passenger> PassengerList =ViewState["Passenger"] as List<Tb_Ticket_Passenger>;

            Order.OrderStatusCode = 19;//改签成功，交易结束
            result = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().OperOrderGQ(Order, PassengerList, mUser, mCompany);
        }
        catch(Exception ex)
        {

        }
        if (result)
            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialogOne('确认改签成功!','" + ViewState["Url"].ToString() + "');", true);
        else
            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('确认改签失败!');", true);
    }


    /// <summary>
    /// 拒绝改签
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUnOk_Click(object sender, EventArgs e)
    {
        bool result = false;
        try
        {
            Tb_Ticket_Order Order = ViewState["Order"] as Tb_Ticket_Order;
            List<Tb_Ticket_Passenger> PassengerList = ViewState["Passenger"] as List<Tb_Ticket_Passenger>;

            Order.OrderStatusCode = 23; //拒绝改签，退款中

            //流程： 1 修改状态 、2 退款
            //1.修改状态
            result = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().OperOrderGQ(Order, PassengerList, mUser, mCompany);

            string msg = "";
            
            //2.退款
            new PbProject.Logic.Pay.OperOnline().TitckOrderRefund(Order, mUser, mCompany, out msg);
        }
        catch
        {

        }
        if (result)
            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialogOne('拒绝改签,退款中!','" + ViewState["Url"].ToString() + "');", true);
        else
            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('拒绝改签,失败!');", true);
    }

}