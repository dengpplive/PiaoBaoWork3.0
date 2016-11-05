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
public partial class Sys_CityList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.btnAdd.PostBackUrl = string.Format("CityEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            Curr = 1;
            Con = " 1=1";
            AspNetPager1.PageSize = 20;
            ViewState["orderBy"] = " id ";
        }
    }

    private void repCityListDataBind()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Bd_Air_AirPort> list = baseDataManage.CallMethod("Bd_Air_AirPort", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Bd_Air_AirPort>;
        TotalCount = outParams.GetValue<int>("1");
        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        repCityList.DataSource = list;
        repCityList.DataBind();
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
        repCityListDataBind();
    }

    /// <summary>
    /// 国家类型转换
    /// </summary>
    /// <param name="Types"></param>
    /// <returns></returns>
    public string ReturnType(string Types)
    {
        string t = "";
        if (Types == "1")
        {
            t = "国内";
        }
        else
        {
            t = "国际";
        }
        return t;
    }
    protected void SelButton_Click(object sender, EventArgs e)
    {
        Curr = 1;
        AspNetPager1.CurrentPageIndex = Curr;
        Con = SelWhere();
        repCityListDataBind();
    }
    protected void Reset_Click(object sender, EventArgs e)
    {
        txtCity.Text = "";
        txtSpell.Text = "";
        txtCode.Text = "";
        txtCountries.Text = "";
        ddlType.SelectedIndex = 0;
    }
    private string SelWhere()
    {
        StringBuilder StrWhere = new StringBuilder(" 1=1 ");
        if (txtCity.Text.Trim() != "")
        {
            StrWhere.Append(" and CityName like '%" + txtCity.Text.Trim().Replace("'", "") + "%'");
        }
        if (txtSpell.Text.Trim() != "")
        {
            StrWhere.Append(" and CityQuanPin like '%" + txtSpell.Text.Trim().Replace("'", "") + "%' ");
        }
        if (txtCode.Text.Trim() != "")
        {
            StrWhere.Append(" and CityCodeWord like '%" + txtCode.Text.Trim().Replace("'", "") + "%' ");
        }
        if (txtCountries.Text.Trim() != "")
        {
            StrWhere.Append(" and Continents = '" + txtCountries.Text.Trim().Replace("'", "") + "' ");
        }
        if (ddlType.SelectedValue != "wx")
        {
            StrWhere.Append(" and IsDomestic = " + int.Parse(ddlType.SelectedValue) + " ");
        }
        return StrWhere.ToString();
    }
    protected void repCityList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Del")
        {
            bool DeleteSuc = (bool)baseDataManage.CallMethod("Bd_Air_AirPort", "DeleteById", null, new object[] { e.CommandArgument.ToString() });
            if (DeleteSuc)
            {

            }
            Con = SelWhere();
            repCityListDataBind();
        }
    }
}