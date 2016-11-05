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
/// <summary>
/// 航空公司
/// </summary>
public partial class Sys_AirlineCompanyList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnAdd.PostBackUrl = string.Format("AirlineCompanyEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            Curr = 1;
            Con = " 1=1";
            AspNetPager1.PageSize = 20;
            ViewState["orderBy"] = " id ";
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
    private void PageDataBind()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Bd_Air_Carrier> list = baseDataManage.CallMethod("Bd_Air_Carrier", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Bd_Air_Carrier>;
        TotalCount = outParams.GetValue<int>("1");
        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        Repeater.DataSource = list;
        Repeater.DataBind();
    }
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        PageDataBind();
    }

    protected void SelButton_Click(object sender, EventArgs e)
    {
        StringBuilder WhereStr = new StringBuilder();
        WhereStr.Append(" 1=1 ");
        if (txtCarrier.Text != "")
        {
            WhereStr.Append(" and AirName like'%" + txtCarrier.Text.Trim().Replace("'", "") + "%'");
        }
        if (txtCode.Text != "")
        {
            WhereStr.Append(" and Code  like '%" + txtCode.Text.Trim().Replace("'", "") + "%'");
        }
        if (ddlGNGJ.SelectedIndex != 0)
        {
            WhereStr.Append(" and Type =" + ddlGNGJ.SelectedValue + "");
        }
        if (txtShort.Text != "")
        {
            WhereStr.Append(" and ShortName like'%" + txtShort.Text.Trim().Replace("'", "") + "%'");
        }
        if (ddlXiaoShou.SelectedValue != "-1")
        {
            if (int.Parse(ddlXiaoShou.SelectedValue) != 0)
            {
                //是否销售
                WhereStr.Append(" and A1=" + ddlXiaoShou.SelectedValue + " ");
            }
            else
            {
                //是否销售
                WhereStr.Append(" and A1=0 or A1 is null ");
            }
        }
        Curr = 1;
        AspNetPager1.CurrentPageIndex = Curr;
        Con = WhereStr.ToString();
        PageDataBind();
    }
    protected void Reset_Click(object sender, EventArgs e)
    {
        txtCarrier.Text = "";
        txtCode.Text = "";
        txtShort.Text = "";
        ddlGNGJ.SelectedIndex = 0;
        ddlXiaoShou.SelectedIndex = 0;
    }



    protected void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string id = e.CommandArgument.ToString();
        if (e.CommandName == "Del")
        {
            //删除
            bool DeleteSuc = (bool)baseDataManage.CallMethod("Bd_Air_Carrier", "DeleteById", null, new object[] { id });
            if (DeleteSuc)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('删除成功！')", true);
                PageDataBind();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('删除失败！')", true);
            }
        }
    }
    public bool GetXS(object objXS)
    {
        bool xs = true;
        if (objXS != null)
        {
            xs = int.Parse(objXS.ToString()) == 0 ? false : true;
        }
        return xs;
    }
}