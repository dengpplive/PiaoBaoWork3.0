using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using PbProject.Model;

public partial class Policy_StrategyGroupDetailList : BasePage
{
    public List<Bd_Base_Dictionary> list = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnadd.PostBackUrl = string.Format("StrategyGroupDetailEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            btgroup.PostBackUrl = string.Format("StrategyGroupList.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            list = baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new Object[] { "ParentID=24" }) as List<Bd_Base_Dictionary>;
            Curr = 1;
            Query();
            AspNetPager1.PageSize = 50;
            PageDataBind();
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
    /// 绑定数据
    /// </summary>
    private void PageDataBind()
    {
        try
        {
            DataTable dt = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().GetViewInfoByStrWhere("V_StrategyGroup", Con + " order by OperTime desc");
            AspNetPager1.RecordCount = dt.Rows.Count;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
            repPosList.DataSource = dt;
            repPosList.DataBind();
        }
        catch (Exception)
        {

            throw;
        }

    }
    /// <summary>
    /// 查询条件
    /// </summary>
    protected void Query()
    {
        StringBuilder strWhere = new StringBuilder();
        strWhere.Append(mCompany.RoleType == 1 ? "1=1" : "CpyNo='" + mCompany.UninCode + "'");
        if (txtGroupName.Text.Trim() != "")
        {
            strWhere.Append(" and GroupName like '%" + txtGroupName.Text.Trim() + "%'");
        }
        if (ddlbasetype.SelectedValue!="")
        {
            strWhere.Append(" and BaseType = " + ddlbasetype.SelectedValue);
        }
        if (txtCarryCode.Text.Trim() != "")
        {
            strWhere.Append(" and CarryCode like '%" + txtCarryCode.Text.Trim() + "%'");
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
        PageDataBind();
    }
    protected void repPosList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string msg = "";
        try
        {
            string id = e.CommandArgument.ToString();
            if (e.CommandName == "Del")
            {
                msg = (bool)baseDataManage.CallMethod("Tb_Ticket_TakeOffDetail", "DeleteById", null, new Object[] { id }) == true ? "删除成功" : "删除失败";
            }

            PageDataBind();
        }
        catch (Exception)
        {
            msg = "操作异常";
            throw;
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
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
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnreset_Click(object sender, EventArgs e)
    {
        txtGroupName.Text = "";
        txtCarryCode.Text = "";
        ddlbasetype.SelectedValue = "";
    }
    /// <summary>
    /// 返回接口类型
    /// </summary>
    /// <param name="childid"></param>
    /// <returns></returns>
    public string Returnjktype(string childid)
    {
        string name = "";
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].ChildID.ToString() == childid)
            {
                name = list[i].ChildName.ToString();
            }
        }
        return name;
    }
}