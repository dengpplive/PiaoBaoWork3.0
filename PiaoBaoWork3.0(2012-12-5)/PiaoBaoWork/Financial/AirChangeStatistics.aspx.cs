using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.WebCommon.Utility;
using System.Data;

public partial class Financial_AirChangeStatistics : BasePage
{
    public DataBase.Data.HashObject Hparams = new DataBase.Data.HashObject();
    public DataTable[] dsSalesInfoCollect = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtTimeBegin.Value = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
            txtTimeEnd.Value = DateTime.Now.ToString("yyyy-MM-dd");
        }
       
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery1_Click(object sender, EventArgs e)
    {
        if (DateTime.Parse(txtTimeBegin.Value) < DateTime.Parse(txtTimeEnd.Value).AddMonths(-1))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('统计时间跨度不能超过一个月！');", true);
            return;
        }
        else
        {
            PageDataBind();
        }
    }
    /// <summary>
    /// 绑定数据
    /// </summary>
    protected void PageDataBind()
    {
        Hparams.Add("CpyNo", mUser.CpyNo.Trim());
        Hparams.Add("LoginName", CommonManage.TrimSQL(txtUserAccount.Text.Trim()));
        Hparams.Add("TimeStart", txtTimeBegin.Value);
        Hparams.Add("TimeEnd", txtTimeEnd.Value + " 23:59:59");
      
        dsSalesInfoCollect = base.baseDataManage.MulExecProc("pro_AirChangeStatistics", Hparams);
        gvinfo.DataSource=dsSalesInfoCollect[0];
        gvinfo.DataBind();
    }
    /// <summary>
    /// 导出
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOut_Click(object sender, EventArgs e)
    {
        Hparams.Add("CpyNo", mUser.CpyNo.Trim());
        Hparams.Add("LoginName", CommonManage.TrimSQL(txtUserAccount.Text.Trim()));
        Hparams.Add("TimeStart", txtTimeBegin.Value);
        Hparams.Add("TimeEnd", txtTimeEnd.Value + " 23:59:59");

        dsSalesInfoCollect = base.baseDataManage.MulExecProc("pro_AirChangeStatistics", Hparams);
        ExcelRender.RenderToExcel(dsSalesInfoCollect[0], Context, "航变统计报表" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".xls");
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear2_Click(object sender, EventArgs e)
    {
        txtTimeBegin.Value = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
        txtTimeEnd.Value = DateTime.Now.ToString("yyyy-MM-dd");
        txtUserAccount.Text = "";
    }
}