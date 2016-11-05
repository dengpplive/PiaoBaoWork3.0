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
public partial class Sys_FaresList : BasePage
{
    BaseDataManage Manage = new BaseDataManage();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnAdd.PostBackUrl = string.Format("FaresEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString()); 
            Curr = 1;
            Con = " 1=1";
            AspNetPager1.PageSize = 20;
            ViewState["orderBy"] = " id ";
        }
    }
    private void repFaresListDataBind()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Bd_Air_Fares> list = Manage.CallMethod("Bd_Air_Fares", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Bd_Air_Fares>;
        TotalCount = outParams.GetValue<int>("1");

        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        repFaresList.DataSource = list;
        repFaresList.DataBind();
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

    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        repFaresListDataBind();
    }
    protected void SelButton_Click(object sender, EventArgs e)
    {
        Curr = 1;
        AspNetPager1.CurrentPageIndex = Curr;
        Con = SelWhere();
        repFaresListDataBind();
    }
    protected void Reset_Click(object sender, EventArgs e)
    {
        ddlType.SelectedIndex = 0;
        txtFromCode.Value = "";
        txtToCode.Value = "";
        txtCarryCode.Value = "0";
    }
    public string ReturnType(string t)
    {
        string tp = "";
        if (t == "1")
        {
            tp = "国内";
        }
        if (t == "2")
        {
            tp = "国际";
        }
        return tp;
    }

    private string SelWhere()
    {
        StringBuilder StrWhere = new StringBuilder(" 1=1 ");

        if (txtFromCode.Value.Trim() != "中文/英文" && FromCityCode.Value != "" && FromCityCode.Value != "中文/英文" && (txtToCode.Value.Trim() != "中文/英文" && ToCityCode.Value != "" && ToCityCode.Value != "中文/英文"))
        {
            StrWhere.Append(" and ((FromCityCode like '%" + FromCityCode.Value.Replace("'", "") + "%' and  ToCityCode like '%" + ToCityCode.Value.Replace("'", "") + "%' ) or (FromCityCode like '%" + ToCityCode.Value.Replace("'", "") + "%' and  ToCityCode like '%" + ToCityCode.Value.Replace("'", "") + "%' )) ");
        }
        else
        {
            if (txtFromCode.Value.Trim() != "中文/英文" && FromCityCode.Value != "" && FromCityCode.Value != "中文/英文")
            {
                StrWhere.Append(" and FromCityCode like '%" + FromCityCode.Value.Replace("'", "") + "%' ");
            }
            if (txtToCode.Value.Trim() != "中文/英文" && ToCityCode.Value != "" && ToCityCode.Value != "中文/英文")
            {
                StrWhere.Append(" and ToCityCode like '%" + ToCityCode.Value.Replace("'", "") + "%' ");
            }
        }
        if (txtCarryCode.Value.Trim() != "" && txtCarryCode.Value != "0")
        {
            StrWhere.Append(" and CarryCode = '" + txtCarryCode.Value.Trim().Replace("'", "") + "' ");
        }
        if (ddlType.SelectedValue != "wx")
        {
            StrWhere.Append(" and IsDomestic = " + int.Parse(ddlType.SelectedValue) + " ");
        }
        return StrWhere.ToString();
    }
    protected void repFaresList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Del")
        {
            bool DeleteSuc = (bool)Manage.CallMethod("Bd_Air_Fares", "DeleteById", null, new object[] { e.CommandArgument.ToString() });
            if (DeleteSuc)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('删除成功');", true);
                Con = SelWhere();
                repFaresListDataBind();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('删除失败');", true);
            }
        }
    }
}