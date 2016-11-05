using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

public partial class Financial_OffLineDetail : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["cpyname"] = Request["cpyname"] == null ? "" : Request["cpyname"].ToString();
            if (Request["begintime"] != null && Request["endtime"] != null)
            {
                this.txtGoAlongTime1.Value = Request["begintime"];
                this.txtGoAlongTime2.Value = Request["endtime"];
            }
            Curr = 1;
            Query();
            AspNetPager1.PageSize = 20;
           
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
    /// 查询条件
    /// </summary>
    protected void Query()
    {
       
        StringBuilder strWhere = new StringBuilder("1=1");
       
        if (ViewState["cpyname"].ToString().Length != 0)
        {
            strWhere.Append(" and UninAllName = '" + ViewState["cpyname"].ToString().Trim() + "'");
        }
        if (txtGoAlongTime1.Value != "")
        {
            strWhere.Append(" and CPTime >= '" + txtGoAlongTime1.Value + "'");
        }
        if (txtGoAlongTime2.Value != "")
        {
            strWhere.Append(" and CPTime <= '" + txtGoAlongTime2.Value + "'");
        }

        Con = strWhere.ToString();
        Curr = 1;

    }
    /// <summary>
    /// 绑定数据
    /// </summary>
    private void PageDataBind()
    {
        try
        {
            int num = 0;
            DataTable dt = baseDataManage.GetBasePagerDataTable("V_OffLineDetail", out num, AspNetPager1.PageSize, Curr, "*", Con, "CPTime desc");
            AspNetPager1.RecordCount = num;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
            repList.DataSource = dt;
            repList.DataBind();
          
        }
        catch (Exception)
        {

            throw;
        }

    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Query();
        PageDataBind();
    }
    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        PageDataBind();
    }
    /// <summary>
    /// 导出报表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        DataTable dt = baseDataManage.ExecuteStrSQL("select _order.OrderId 订单号,_order.PNR PNR,_order.PayMoney 交易金额,_order.CPTime 出票时间,_order.PayTime 支付时间, case when _order.PayStatus=1 then '已付' else '未付' end 支付状态,_cpy.UninAllName 客户名称 " +  
        "from Tb_Ticket_Order _order inner join User_Company _cpy on _order.OwnerCpyNo=_cpy.UninCode where "+Con);
        new ExcelData().SaveToExcel(dt, "线下收银明细表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnreset_Click(object sender, EventArgs e)
    {
        txtGoAlongTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
        txtGoAlongTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");
    }
}