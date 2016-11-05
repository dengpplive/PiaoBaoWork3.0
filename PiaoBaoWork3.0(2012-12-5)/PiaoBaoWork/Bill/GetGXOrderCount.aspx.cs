using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Bill_GetGXOrderCount : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                cptimestart.Value = DateTime.Now.ToString("yyyy-MM-dd");
                cptimeend.Value = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        BindCount();
    }
    /// <summary>
    /// 
    /// </summary>
    public void BindCount()
    {
        try
        {
            string UninAllName = txtCustomer.Text.Trim();
            string CPTimeState = DateTime.Parse(cptimestart.Value).ToString("yyyy-MM-dd") + " 00:00:00";
            string CPTimeEnd = DateTime.Parse(cptimeend.Value).ToString("yyyy-MM-dd") + " 23:59:59";

            PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
            DataBase.Data.HashObject queryParamter = new DataBase.Data.HashObject();
            queryParamter.Add("UninAllName", UninAllName);
            queryParamter.Add("CPTimeState", CPTimeState);
            queryParamter.Add("CPTimeEnd", CPTimeEnd);

            System.Data.DataTable dt = baseDataManage.EexcProc("Proc_GetGXOrderCount", queryParamter);
            gvTicketDetail.DataSource = dt;
            gvTicketDetail.DataBind();

        }
        catch (Exception)
        {

        }
    }
}