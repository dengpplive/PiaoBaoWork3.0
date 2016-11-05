using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;
using PbProject.Model;
using System.Text;

public partial class Policy_PolicyShareTakeoffList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnadd.PostBackUrl = string.Format("PolicyShareTakeoffEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            list = baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new Object[] { "ParentID=24" }) as List<Bd_Base_Dictionary>;
            Bindjk();
            Curr = 1;
            Query();
            AspNetPager1.PageSize = 20;
            ViewState["orderBy"] = " OperTime desc ";
            BindPolicyShareTakeoffInfo();
        }
    }
    protected List<Bd_Base_Dictionary> list
    {
        get { return (List<Bd_Base_Dictionary>)ViewState["list"]; }
        set { ViewState["list"] = value; }
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
    /// 绑定接口
    /// </summary>
    protected void Bindjk()
    {
        List<Bd_Base_Dictionary> list = baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new Object[] { "ParentID=24" }) as List<Bd_Base_Dictionary>;
        this.ddljk.DataSource = list;
        ddljk.DataTextField = "ChildName";
        ddljk.DataValueField = "ChildID";
        this.ddljk.DataBind();
    }
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
    /// <summary>
    /// 绑定补点信息
    /// </summary>
    protected void BindPolicyShareTakeoffInfo()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Tb_ShareInterface_TakeOff> list = baseDataManage.CallMethod("Tb_ShareInterface_TakeOff", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Tb_ShareInterface_TakeOff>;
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
        strWhere.Append(mCompany.RoleType == 1 ? "1=1" : "CpyNo='"+mCompany.UninCode+"'");
        if (txtCopName.Text != "")
        {
            strWhere.Append(" and CpyName like '%" + txtCopName.Text + "%'");
        }
        if (ddljk.SelectedValue.ToString()!="0")
        {
            strWhere.Append(" and PolicySource = " + ddljk.SelectedValue + "");
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
        BindPolicyShareTakeoffInfo();
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Query();
        BindPolicyShareTakeoffInfo();
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnreset_Click(object sender, EventArgs e)
    {
        txtCopName.Text = "";
        ddljk.SelectedValue = "0";
    }
}