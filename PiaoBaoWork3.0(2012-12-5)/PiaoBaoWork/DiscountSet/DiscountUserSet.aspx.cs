using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using PbProject.Model;
using DataBase.Data;

public partial class DiscountSet_DiscountUserSet : BasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GroupList = baseDataManage.CallMethod("Tb_Ticket_StrategyGroup", "GetList", null, new object[] { "CpyNo='" + mCompany.UninCode + "'" }) as List<Tb_Ticket_StrategyGroup>;
            Curr = 1;
            Con = Query();
            AspNetPager1.PageSize = 20;
            BindUser();
            BindGroup();
        }
    }
    protected List<Tb_Ticket_StrategyGroup> GroupList
    {
        get { return (List<Tb_Ticket_StrategyGroup>)ViewState["GroupList"]; }
        set { ViewState["GroupList"] = value; }
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
    /// 绑定组
    /// </summary>
    protected void BindGroup()
    {
        this.ddlGroup.DataSource = GroupList;
        ddlGroup.DataTextField = "GroupName";
        ddlGroup.DataValueField = "id";
        ddlGroup.DataBind();
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Curr = 1;
        Con = Query();
        BindUser();
    }
    /// <summary>
    /// 绑定组用户
    /// </summary>
    public void BindUser()
    {
        try
        {
            int num = 0;
            DataTable dt = baseDataManage.GetBasePagerDataTable("V_AccountInfo", out num, AspNetPager1.PageSize, Curr, "*", Con, "CreateTime desc");
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
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    private string Query()
    {
        StringBuilder strWhere = new StringBuilder("IsAdmin=0");
        try
        {
            if (mCompany.RoleType == 2)
            {
                //运营商直属下级
                strWhere.Append(" and RoleType in (4,5) and len(UninCode)=18 and SUBSTRING(UninCode,1,12)='" + mCompany.UninCode + "' ");
            }
            else if (mCompany.RoleType == 4)
            {
                //分销直属下级
                if (mCompany.UninCode.Length == 18)
                {
                    strWhere.Append(" and RoleType in (4,5) and len(UninCode)=24 and SUBSTRING(UninCode,1,18)='" + mCompany.UninCode + "' ");
                }
                else if (mCompany.UninCode.Length == 24)
                {
                    strWhere.Append(" and RoleType =5 and len(UninCode)=30 and SUBSTRING(UninCode,1,24)='" + mCompany.UninCode + "' ");
                }
            }
            else
            {
                strWhere.Append(" and 1<>1");
            }
            strWhere.Append(ddlGroup.SelectedValue != "-1" ? ddlGroup.SelectedValue == "" ? " and (GroupId is null or GroupId='')" : " and GroupId='" + ddlGroup.SelectedValue + "'" : "");
            strWhere.Append(txtLoginName.Text.Trim() != "" ? " and LoginName like '%" + txtLoginName.Text.Trim() + "%'" : "");
            return strWhere.ToString();
        }
        catch (Exception)
        {
            return strWhere.ToString();
        }
    }
    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Con = Query();
        Curr = e.NewPageIndex;
        BindUser();
    }
    /// <summary>
    /// 将选择的用户加入分组
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddGroupUser_Click(object sender, EventArgs e)
    {
        string msg = "";
        try
        {
            int count = this.repList.Items.Count;
            string ids = "";
            for (var i = 0; i < count; i++)
            {
                if (((System.Web.UI.HtmlControls.HtmlInputCheckBox)this.repList.Items[i].FindControl("cbItem")).Checked)
                {
                    string id = ((System.Web.UI.HtmlControls.HtmlInputCheckBox)this.repList.Items[i].FindControl("cbItem")).Value;
                    if (id.Length != 0)
                    {
                        ids += "'" + id + "',";
                    }
                }
            }
            ids = ids.TrimEnd(',');
            if (ids.Length>0)
            {
                string sql = "";
                string groupid = ddlGroup.SelectedValue.ToString();
                if (groupid!="" && groupid!="-1")
                {
                     sql = "Update User_Company set GroupId='" + ddlGroup.SelectedValue + "' where id in (" + ids + ")";
                }
                else
                {
                    sql = "Update User_Company set GroupId=NULL where id in (" + ids + ")";
                }
                msg = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().ExecuteNonQuerySQLInfo(sql) == true ? "操作成功" : "操作失败";
                BindUser();
            }
            else
            {
                msg = "请选择要划分的用户";
            }
            
        }
        catch (Exception)
        {
            msg = "操作异常";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
    public string ReturnName(string id)
    {
        string name = "";
        for (int i = 0; i < GroupList.Count; i++)
        {
            if (GroupList[i].id.ToString() == id)
            {
                name = GroupList[i].GroupName;
            }
        }
        return name;
    }
}