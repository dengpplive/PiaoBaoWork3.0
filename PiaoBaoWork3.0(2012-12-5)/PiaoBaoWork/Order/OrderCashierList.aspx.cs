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
using PbProject.WebCommon.Utility;
using PbProject.Logic.Order;

/// <summary>
/// 待收银订单
/// </summary>
public partial class Order_OrderCashierList : BasePage
{

    private List<Bd_Base_Dictionary> dicList = new List<Bd_Base_Dictionary>();

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

                //当前时间
                DateTime dt = DateTime.Now;
                //每月一号时间
                //DateTime dt1 = new DateTime(dt.Year, dt.Month, 1);

                //乘机日期
                //txtFromDate1.Value = dt.AddDays(-7).ToString("yyyy-MM-dd");
                //txtFromDate2.Value = dt.ToString("yyyy-MM-dd");

                //创建日期
                txtCreateTime1.Value = dt.AddDays(-7).ToString("yyyy-MM-dd");
                txtCreateTime2.Value = dt.ToString("yyyy-MM-dd");

                Con = SelWhere();

                //初始化数据
                GetDictionary();
                //订单提醒 查询订单
                showPrompt();

            }
        }
        catch (Exception) { }
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
    /// 初始化字典表
    /// </summary>
    public void GetDictionary()
    {
        try
        {
            if (dicList.Count == 0)
            {
                dicList = new Bd_Base_DictionaryBLL().GetList();
            }

            spanUnLock.Visible = false;

        }
        catch (Exception)
        {

        }
    }
    /// <summary>
    /// 获取字典表名称
    /// </summary>
    /// <param name="parentId"></param>
    /// <param name="childId"></param>
    /// <returns></returns>
    public string GetDicName(int parentId, string childId)
    {
        string result = "";
        GetDictionary();
        var query = from Bd_Base_Dictionary d in dicList
                    where d.ParentID == parentId && d.ChildID == int.Parse(childId)
                    select d;
        if (query.Count<Bd_Base_Dictionary>() > 0)
        {
            List<Bd_Base_Dictionary> list = query.ToList<Bd_Base_Dictionary>();
            result = list[0].ChildName;
        }
        return result;
    }

    //页面显示数据
    public string ShowText(int opType, object objData)
    {
        string strReData = "";
        if (opType == 0)
        {
            //创建时间
            if (objData != null)
            {
                strReData = objData.ToString().Trim().Replace(" ", "<br />");
            }
        }
        else if (opType == 1)
        {
            //显示乘机人
            if (objData != null)
            {
                strReData = objData.ToString().Replace("|", "<br />");
            }
        }
        else if (opType == 2)
        {
            //起飞日期
            if (objData != null)
            {
                strReData = objData.ToString().Trim().Replace("/", "<br />");
            }
        }
        else if (opType == 3)
        {
            //行程
            if (objData != null)
            {
                if (objData.ToString().Trim() == "1")
                {
                    strReData = "单程";
                }
                else if (objData.ToString().Trim() == "2")
                {
                    strReData = "往返";
                }
                else if (objData.ToString().Trim() == "3")
                {
                    strReData = "联程";
                }
                else if (objData.ToString().Trim() == "4")
                {
                    strReData = "多程";
                }
            }
        }
        else if (opType == 4)
        {
            //航程
            if (objData != null)
            {
                strReData = objData.ToString().Trim().Replace("/", "<br />");
            }
        }
        else if (opType == 5)
        {
            //支付状态

            if (objData != null)
            {
                if (objData.ToString() == "0")
                    strReData = "<span style='color:Red'>未付</span>";
                else if (objData.ToString() == "1")
                    strReData = "<span style='color:Green'>已付</span>";
            }
        }

        return strReData;
    }

    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    private string SelWhere()
    {
        StringBuilder StrWhere = new StringBuilder(" OrderStatusCode= 4 and  PayStatus=0 ");
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
            if (!string.IsNullOrEmpty(txtPassengerName.Text.Trim()))
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

            //订单收银
            if (e.CommandName == "Cashier")
            {
                string msg = "";

                List<Tb_Ticket_Order> mOrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { " id= '" + Id + "'" }) as List<Tb_Ticket_Order>;
                Tb_Ticket_Order Order = null; //订单

                if (mOrderList != null && mOrderList.Count > 0)
                {
                    Order = mOrderList[0];

                    if (Order.PayStatus == 0)
                    {
                        bool reuslt = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().OperOrderCashier(Order, mUser, mCompany);

                        if (reuslt)
                        {
                            msg = "订单收银成功！";
                            OrderListDataBind();
                        }
                        else
                        {
                            msg = "订单收银失败";
                        }
                    }

                    ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
                }
            }
            else if (e.CommandName == "Detail")
            {
                //订单详情
                Response.Redirect("OrderDetail.aspx?Id=" + Id + "&Url=OrderList.aspx&currentuserid=" + this.currentuserid.Value.ToString());
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
            LinkButton lbtnCashier = e.Item.FindControl("lbtnCashier") as LinkButton;

            if (lbtnCashier.Visible == true)
                lbtnCashier.Text += "<br/>";

            ////订单详情
            //LinkButton lbtnDetail = e.Item.FindControl("lbtnDetail") as LinkButton;
            //if (lbtnDetail.Visible == true)
            //    lbtnDetail.Text += "<br/>";
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
        txtCorporationName.Text = "";
        txtFlightCode.Text = "";
        txtFromDate1.Value = "";
        txtFromDate2.Value = "";

        //当前时间
        DateTime dt = DateTime.Now;
        //每月一号时间
        DateTime dt1 = new DateTime(dt.Year, dt.Month, 1);

        //创建日期
        txtCreateTime1.Value = dt1.AddMonths(-1).ToString("yyyy-MM-dd");
        txtCreateTime2.Value = dt.ToString("yyyy-MM-dd");

    }

    /// <summary>
    /// 获取选择的订单 格式: id
    /// </summary>
    /// <returns></returns>
    public List<string> GetSelId()
    {
        List<string> Idslist = new List<string>();
        string[] SelOrderIdArr = Hid_SelOrderId.Value.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
        if (SelOrderIdArr.Length > 0)
        {
            Idslist.AddRange(SelOrderIdArr);
        }
        return Idslist;
    }

    /// <summary>
    /// 订单收银
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUnLock_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg = "";
            Tb_Ticket_OrderBLL OrderManage = new Tb_Ticket_OrderBLL();
            List<string> ids = GetSelId();

            bool reuslt = OrderManage.OperOrderCashiers(ids, mUser, mCompany);

            if (reuslt)
            {
                ErrMsg = "收银成功!";
                OrderListDataBind();
            }
            else
            {
                ErrMsg = "订单收银失败！";
            }
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('" + ErrMsg + "')", true);
        }
        catch (Exception)
        {

            throw;
        }
    }

    #endregion

    #region 数据处理

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="strValue"></param>
    /// <returns></returns>
    public string GetStrValue(int ParentId, string ChildId)
    {
        string Message = "";
        try
        {
            Message = GetDicName(ParentId, ChildId);
            Message = string.Join("<br />", Message.Split(new string[] { ",", "，", " " }, StringSplitOptions.RemoveEmptyEntries));

            if (ParentId == 1 && ChildId == "3")
                Message = "<span style='color:Red;'>" + Message + " </span>";
        }
        catch (Exception ex)
        {

        }
        return Message;
    }

    #endregion
}