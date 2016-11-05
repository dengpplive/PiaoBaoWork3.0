using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;
using System.Text;

public partial class Policy_StrategyGroupList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnadd.PostBackUrl = string.Format("StrategyGroupEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            Curr = 1;
            Query();
            AspNetPager1.PageSize = 20;
            ViewState["orderBy"] = " OperTime desc ";
            BindStrategyGroupInfo();
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
    protected void BindStrategyGroupInfo()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Tb_Ticket_StrategyGroup> list = baseDataManage.CallMethod("Tb_Ticket_StrategyGroup", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Tb_Ticket_StrategyGroup>;
        TotalCount = outParams.GetValue<int>("1");

        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        repPosList.DataSource = list;
        repPosList.DataBind();
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    protected void Query()
    {
        StringBuilder strWhere = new StringBuilder();
        strWhere.Append(mCompany.RoleType == 1 ? "1=1" : "CpyNo='" + mCompany.UninCode + "'");
        if (txtGroupName.Text != "")
        {
            strWhere.Append(" and GroupName like '%" + txtGroupName.Text + "%'");
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
        BindStrategyGroupInfo();
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Query();
        BindStrategyGroupInfo();
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnreset_Click(object sender, EventArgs e)
    {
        txtGroupName.Text = "";
    }
    protected void repPosList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string msg = "";
        try
        {
            string id = e.CommandArgument.ToString();
            if (e.CommandName == "Del")
            {
                bool rs1 = (bool)baseDataManage.CallMethod("Tb_Ticket_StrategyGroup", "DeleteById", null, new Object[] { id });
                bool rs2 = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().ExecuteNonQuerySQLInfo("delete from Tb_Ticket_TakeOffDetail where GroupId ='"+id+"'");
                msg = rs1 == true & rs1 == true ? "删除成功" : "删除失败";
            }

            BindStrategyGroupInfo();
        }
        catch (Exception)
        {
            msg = "操作异常";
            throw;
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
}