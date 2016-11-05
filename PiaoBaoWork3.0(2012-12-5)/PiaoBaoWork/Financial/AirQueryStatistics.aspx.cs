using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PbProject.WebCommon.Utility;

public partial class Financial_AirQueryStatistics : BasePage
{
    public DataBase.Data.HashObject Hparams = new DataBase.Data.HashObject();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtTimeBegin.Value = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
            txtTimeEnd.Value = DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
    /// <summary>
    /// 绑定数据
    /// </summary>
    protected void PageDataBind()
    {
        Hparams.Add("CpyName", CommonManage.TrimSQL(txtCpyName.Text.Trim()));
        Hparams.Add("CpyNo", mUser.CpyNo.Trim());
        Hparams.Add("LoginName", CommonManage.TrimSQL(txtUserAccount.Text.Trim()));
        Hparams.Add("TimeStart", txtTimeBegin.Value);
        Hparams.Add("TimeEnd", txtTimeEnd.Value+" 23:59:59");
        DataTable[] dsSalesInfoCollect = null;
        dsSalesInfoCollect = base.baseDataManage.MulExecProc("pro_TrafficStatistics", Hparams);
        DataView dview = new DataView(dsSalesInfoCollect[0]);
        this.AspNetPager1.RecordCount = dview.Count;
        PagedDataSource pds = new PagedDataSource();
        pds.DataSource = dview;
        pds.AllowPaging = true;
        pds.CurrentPageIndex = AspNetPager1.CurrentPageIndex - 1;
        pds.PageSize = AspNetPager1.PageSize;
        gvinfo.DataSource = pds ;
        gvinfo.DataBind();
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
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear2_Click(object sender, EventArgs e)
    {
        txtTimeBegin.Value = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
        txtTimeEnd.Value = DateTime.Now.ToString("yyyy-MM-dd");
        txtUserAccount.Text = "";
        txtCpyName.Text = "";
    }
    /// <summary>
    /// 导出
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOut_Click(object sender, EventArgs e)
    {
        Hparams.Add("CpyName", CommonManage.TrimSQL(txtCpyName.Text.Trim()));
        Hparams.Add("CpyNo", mUser.CpyNo.Trim());
        Hparams.Add("LoginName", CommonManage.TrimSQL(txtUserAccount.Text.Trim()));
        Hparams.Add("TimeStart", txtTimeBegin.Value);
        Hparams.Add("TimeEnd", txtTimeEnd.Value);
        DataTable[] dsSalesInfoCollect = null;
        dsSalesInfoCollect = base.baseDataManage.MulExecProc("pro_TrafficStatistics", Hparams);
        ExcelRender.RenderToExcel(dsSalesInfoCollect[0], Context, "流量统计报表" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".xls");
    }
    protected void AspNetPager1_PageChanged(object sender, EventArgs e)
    {
        PageDataBind();
    }
}