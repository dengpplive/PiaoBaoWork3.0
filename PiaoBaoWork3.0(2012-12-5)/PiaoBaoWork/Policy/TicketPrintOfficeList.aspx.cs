using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using DataBase.Data;
using PbProject.Model;

public partial class Manager_Base_TicketPrintOfficeList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnAdd.PostBackUrl = string.Format("TicketPrintOfficeEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString()); 
            Curr = 1;
            Query();
            AspNetPager1.PageSize = 20;
            ViewState["orderBy"] = " OperTime desc ";
            BindInfo();
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
    /// 绑定补点信息
    /// </summary>
    protected void BindInfo()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Tb_Ticket_PrintOffice> list = baseDataManage.CallMethod("Tb_Ticket_PrintOffice", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Tb_Ticket_PrintOffice>;
        TotalCount = outParams.GetValue<int>("1");

        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        repList.DataSource = list;
        repList.DataBind();
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    protected void Query()
    {
        StringBuilder strWhere = new StringBuilder();
        strWhere.Append(" CpyNo='"+mCompany.UninCode+"'");
        if (txtAirPortCode.Text.Trim() != "" && txtAirPortCode.Text.Trim() != "0")
        {
            strWhere.Append(" and AirCode like '%" + txtAirPortCode.Value + "%'");
        }
        Con = strWhere.ToString();
        Curr = 1;
    }
    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        BindInfo();
    }
   /// <summary>
   /// 查询
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
    protected void SelButton_Click(object sender, EventArgs e)
    {
        Query();
        BindInfo();
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Reset_Click(object sender, EventArgs e)
    {
        txtAirPortCode.Text = "";
    }
    protected void repList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {

    }
}