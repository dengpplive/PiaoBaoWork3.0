using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using PbProject.Model;

public partial class SMS_SMSRateSetManage : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnadd.PostBackUrl = string.Format("SMSRateSetEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            Query();
            BindRatesSetInfo();
        }
    }
    protected string Con
    {
        get { return (string)ViewState["Con"]; }
        set { ViewState["Con"] = value; }
    }
    protected void BindRatesSetInfo()
    {
        try
        {
            List<Tb_Sms_RateSet> list = baseDataManage.CallMethod("Tb_Sms_RateSet", "GetList", null, new Object[] { Con + " order by RatesCount asc" }) as List<Tb_Sms_RateSet>;
            repList.DataSource = list;
            repList.DataBind();
        }
        catch (Exception ex)
        {

        }
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    protected void Query()
    {
        StringBuilder strWhere = new StringBuilder();
        strWhere.Append("CpyNo='"+mCompany.UninCode+"'");
        if (ddlState.SelectedValue != "")
        {
            strWhere.Append(" and RatesState = " + ddlState.SelectedValue);
        }

        Con = strWhere.ToString();
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Query();
        BindRatesSetInfo();
    }

    protected void repList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string msg = (bool)baseDataManage.CallMethod("Tb_Sms_RateSet", "DeleteById", null, new Object[] { e.CommandArgument }) == true ? "删除成功" : "删除失败";
        Query();
        BindRatesSetInfo();
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
}