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
public partial class Sys_CabinDiscountList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.btnAdd.PostBackUrl = string.Format("CabinDiscountEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString());
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

    private void repCabinListDataBind()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Bd_Air_CabinDiscount> list = baseDataManage.CallMethod("Bd_Air_CabinDiscount", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Bd_Air_CabinDiscount>;
        TotalCount = outParams.GetValue<int>("1");

        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        repCabinList.DataSource = list;
        repCabinList.DataBind();
    }
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        repCabinListDataBind();
    }
    //查询
    protected void SelButton_Click(object sender, EventArgs e)
    {
        Curr = 1;
        AspNetPager1.CurrentPageIndex = Curr;
        Con = SelWhere();
        repCabinListDataBind();
    }
    private string SelWhere()
    {
        StringBuilder StrWhere = new StringBuilder(" 1=1 ");

        if (FromCityCode.Value.Trim() != "" && FromCityCode.Value.Trim() != "中文/英文")
        {
            StrWhere.Append(" and FromCityCode = '" + FromCityCode.Value.Trim().Replace("'", "") + "' ");
        }
        if (ToCityCode.Value.Trim() != "" && ToCityCode.Value.Trim() != "中文/英文")
        {
            StrWhere.Append(" and ToCityCode = '" + ToCityCode.Value.Trim().Replace("'", "") + "' ");
        }
        if (txtAirPortCode.Value.Trim() != "" && txtAirPortCode.Value.Trim() != "0")
        {
            StrWhere.Append(" and AirCode = '" + txtAirPortCode.Value.Trim().Replace("'", "") + "' ");
        }
        if (txtCabin.Value.Trim() != "")
        {
            StrWhere.Append(" and Cabin = '" + txtCabin.Value.Trim().Replace("'", "") + "'");
        }
        if (txtDiscountRate.Value.Trim() != "")
        {
            StrWhere.Append(" and CabinPrice = " + txtDiscountRate.Value.Trim().Replace("'", "") + " ");
        }
        if (ddlIsGN.SelectedValue != "wx")
        {
            StrWhere.Append(" and IsGN = " + int.Parse(ddlIsGN.SelectedValue) + " ");
        }
        return StrWhere.ToString();
    }
    protected void Reset_Click(object sender, EventArgs e)
    {
        txtCabin.Value = "";
        txtDiscountRate.Value = "";
        txtAirPortCode.Value = "";
        FromCityCode.Value = "";
        ToCityCode.Value = "";
        txtAirPortCode.Value = "0";
        ddlIsGN.SelectedIndex = 0;
    }
    /// <summary>
    /// 国家类型转换
    /// </summary>
    /// <param name="Types"></param>
    /// <returns></returns>
    public string ReturnType(string Types)
    {
        string t = "";
        if (Types == "0")
        {
            t = "国内";
        }
        if (Types == "1")
        {
            t = "国际";
        }
        return t;
    }

    protected void repCabinList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Del")
        {
            //删除
            bool DeleteSuc = (bool)baseDataManage.CallMethod("Bd_Air_CabinDiscount", "DeleteById", null, new object[] { e.CommandArgument.ToString() });
            if (DeleteSuc)
            {
                repCabinListDataBind();
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('删除成功！')", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('删除失败！')", true);
            }
        }
    }
}