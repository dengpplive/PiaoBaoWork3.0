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
/// 机型
/// </summary>
public partial class AircraftList : BasePage
{  
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnAdd.PostBackUrl = string.Format("AircraftEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString()); 
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
        List<Bd_Air_Aircraft> list = baseDataManage.CallMethod("Bd_Air_Aircraft", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Bd_Air_Aircraft>;
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
        Curr = 1;
        AspNetPager1.CurrentPageIndex = Curr;
        StringBuilder WhereStr = new StringBuilder();
        WhereStr.Append(" 1=1 ");
        if (txtJiXing.Text != "")
        {
            WhereStr.Append(" and Aircraft like'%" + txtJiXing.Text.Trim().Replace("'", "") + "%'");
        }
        if (txtN.Text != "")
        {
            WhereStr.Append(" and ABFeeN  =" + txtN.Text.Trim().Replace("'", "") + "");
        }
        if (txtW.Text != "")
        {
            WhereStr.Append(" and ABFeeW =" + txtW.Text.Trim().Replace("'", "") + "");
        }

        Con = WhereStr.ToString();
        PageDataBind();
    }
    protected void Reset_Click(object sender, EventArgs e)
    {
        txtJiXing.Text = "";
        txtN.Text = "";
        txtW.Text = "";
    }



}