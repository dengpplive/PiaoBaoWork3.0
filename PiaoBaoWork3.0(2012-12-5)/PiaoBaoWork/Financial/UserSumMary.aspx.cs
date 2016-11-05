using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using PbProject.WebCommon.Utility;

public partial class Financial_UserSumMary : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value=this.mUser.id.ToString();
            txtGoAlongTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            txtGoAlongTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");
            string paytype = Request["paytype"] == null ? "" : Request["paytype"].ToString();
            switch (paytype)
            {
                case "zx":
                    this.spantitle.InnerText = "在线交易汇总";
                    break;
                case "zh":
                    this.spantitle.InnerText = "账户余额汇总";
                    break;
                case "qkxz":
                    this.spantitle.InnerText = "欠款销账汇总";
                    break;
            }
            ViewState["paytype"] = paytype;
            if (mCompany.RoleType != 1 && mCompany.RoleType != 2)
            {
                PageDataBind();
            }
        }
    }
     /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    private string Query()
    {
        StringBuilder sb = new StringBuilder("");
        switch (ViewState["paytype"].ToString())
        {
            case "zh":
                sb.Append("(_logmd.PayType in (13,14) or (_logmd.PayType not in (13,14) and (_logmd.Remark like '%充值%' or _logmd.OperReason like '%充值%'))) and _logmd.Remark not like '%欠款明细记录%' and _logmd.OperReason not like '%欠款明细记录%'");
                break;
            case "zx":
                sb.Append("_logmd.PayType not in (13,14,15,20,21)");
                break;
            case "qkxz":
                sb.Append("PayType = 21");
                break;
        }
        if (mCompany.RoleType != 1 && mCompany.RoleType != 2)
        {
            sb.Append(" and _cpy.UninCode = '" + mCompany.UninCode + "'");
        }
        else if (mCompany.RoleType == 2)
        {
            sb.Append(" and _cpy.UninCode like '" + mCompany.UninCode + "%' and len(unincode)<>12");
        }
        if (txtTo.Text.Trim() != "")
        {
            sb.Append(" and _cpy.UninAllName like '%" + CommonManage.TrimSQL(txtTo.Text.Trim()) + "%'");
        }
        if (txtUserAccount.Text.Trim() != "")
        {
            sb.Append(" and _user.LoginName like '%" + CommonManage.TrimSQL(txtUserAccount.Text.Trim()) + "%'");
        }
        if (txtGoAlongTime1.Value.Trim() != "")
        {
            sb.Append(" and _logmd.OperTime >= convert(DateTime, '" + txtGoAlongTime1.Value + "')");
        }
        if (txtGoAlongTime2.Value.Trim() != "")
        {
            sb.Append(" and _logmd.OperTime <= convert(DateTime, '" + txtGoAlongTime2.Value + " 23:59:59')");
        }
        return sb.ToString();
    }
    /// <summary>
    /// 绑定信息
    /// </summary>
    private void PageDataBind()
    {
        DataBase.Data.HashObject Hparams = new DataBase.Data.HashObject();
        Hparams.Add("sqlwhere", Query());
        //（账户，在线，销账）汇总
        DataTable[] dsSalesInfoCollect = null;
        if (ViewState["paytype"].ToString()=="qkxz")
        {
            dsSalesInfoCollect = base.baseDataManage.MulExecProc("pro_CpyXZAccountCollect", Hparams);
        }
        else
        {
            dsSalesInfoCollect = base.baseDataManage.MulExecProc("pro_CpyAccountCollect", Hparams);
        }
        gvUserSumMaryInfo.DataSource = dsSalesInfoCollect[0];
        gvUserSumMaryInfo.DataBind();

        //DataTable dt = dsSalesInfoCollect[0];
        //DataRow drnul = dt.NewRow();
        //for (int i = 0; i < 50; i++)
        //{
        //    dt.Rows.Add(drnul[1]);
        //}
        //gvUserSumMaryInfoNew.DataSource = dt;
        //gvUserSumMaryInfoNew.DataBind();
        

    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery1_Click(object sender, EventArgs e)
    {
        PageDataBind();
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear2_Click(object sender, EventArgs e)
    {
        this.txtUserAccount.Text = "";
        txtTo.Text = "";
        txtGoAlongTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
        txtGoAlongTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");
    }
    #region 导出报表
    /// <summary>
    /// 导出报表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOut_Click(object sender, EventArgs e)
    {
        //Response.Clear();
        //DownloadExcelFlag = true;
        //Export(this.spantitle.InnerText+"报表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
        DataBase.Data.HashObject Hparams = new DataBase.Data.HashObject();
        Hparams.Add("sqlwhere", Query());
        //（账户，在线，销账）汇总
        DataTable[] dsSalesInfoCollect = null;
        if (ViewState["paytype"].ToString() == "qkxz")
        {
            dsSalesInfoCollect = base.baseDataManage.MulExecProc("pro_CpyXZAccountCollect", Hparams);
        }
        else
        {
            dsSalesInfoCollect = base.baseDataManage.MulExecProc("pro_CpyAccountCollect", Hparams);
        }
        ExcelRender.RenderToExcel(dsSalesInfoCollect[0], Context, this.spantitle.InnerText + "报表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")+".xls");
    }
    #endregion
    /// <summary>
    /// gvUserSumMaryInfo_RowDataBound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvUserSumMaryInfo_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1 && e.Row.RowType == DataControlRowType.DataRow)
        {
            string url = "";
            switch (ViewState["paytype"].ToString())
            {
                case "zx":
                    url = "PaymentRecord.aspx?cpyname=" + Server.UrlEncode(e.Row.Cells[0].Text.Trim()) + "&paytype=zx&currentuserid=" + this.currentuserid.Value.ToString();
                    break;
                case "zh":
                    url = "PaymentRecord.aspx?cpyname=" + Server.UrlEncode(e.Row.Cells[0].Text.Trim()) + "&paytype=zh&currentuserid=" + this.currentuserid.Value.ToString();
                    break;
                case "qkxz":
                    url = "PaymentRecord.aspx?cpyname=" + Server.UrlEncode(e.Row.Cells[0].Text.Trim()) + "&paytype=qkxz&currentuserid=" + this.currentuserid.Value.ToString();
                    break;
            }
            if (e.Row.Cells[0].Text != "合计" && e.Row.Cells[0].Text != "")
            {
                e.Row.Cells[0].Attributes.Add("onclick", "OnClickgetUrl('" + url + "')");
                e.Row.Cells[0].Attributes.Add("style", "cursor:hand;text-decoration:Underline;color:blue;");
            }
            
        }
    }
    /// <summary>
    /// 查看明细
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btndetails_Click(object sender, EventArgs e)
    {
        string type = ViewState["paytype"].ToString();
        Response.Redirect("PaymentRecord.aspx?paytype=" + type + "&currentuserid=" + this.currentuserid.Value.ToString());
    }
}