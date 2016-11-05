using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

public partial class Financial_OffLineList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            txtGoAlongTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            txtGoAlongTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    private string Query()
    {
        StringBuilder sb = new StringBuilder("1=1");
       
        if (mCompany.RoleType != 1 && mCompany.RoleType != 2)
        {
            sb.Append(" and _cpy.UninCode = '" + mCompany.UninCode + "'");
        }
        else if (mCompany.RoleType == 2)
        {
            sb.Append(" and _cpy.UninCode like '" + mCompany.UninCode + "%' and len(unincode)<>12");
        }
        if (txtUninAllNAME.Text.Trim() != "")
        {
            sb.Append(" and _cpy.UninAllName  like '%" + txtUninAllNAME.Text.Replace("'", "''") + "%'");
        }
        if (txtLoginName.Text.Trim() != "")
        {
            sb.Append(" and _user.LoginName  like '%" + txtLoginName.Text.Replace("'", "''") + "%'");
        }
        if (txtGoAlongTime1.Value.Trim() != "")
        {
            sb.Append(" and _order.CPTime >= convert(DateTime, '" + txtGoAlongTime1.Value + "')");
        }
        if (txtGoAlongTime2.Value.Trim() != "")
        {
            sb.Append(" and _order.CPTime <= convert(DateTime, '" + txtGoAlongTime2.Value + " 23:59:59')");
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
        //线下收银汇总
        DataTable[] dsSalesInfoCollect = base.baseDataManage.MulExecProc("pro_OffLineCollect", Hparams);


        gvOffLineInfo.DataSource = dsSalesInfoCollect[0];
        gvOffLineInfo.DataBind();

        DataTable dt = dsSalesInfoCollect[0];
        DataRow drnul = dt.NewRow();
        for (int i = 0; i < 50; i++)
        {
            dt.Rows.Add(drnul[1]);
        }
        gvOffLineInfoNew.DataSource = dt;
        gvOffLineInfoNew.DataBind();
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        PageDataBind();
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnreset_Click(object sender, EventArgs e)
    {
        this.txtUninAllNAME.Text = "";
        this.txtLoginName.Text = "";
        txtGoAlongTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
        txtGoAlongTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");
    }

    protected void gvOffLineInfo_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1 && e.Row.RowType == DataControlRowType.DataRow)
        {
            string url = "OffLineDetail.aspx?cpyname=" + e.Row.Cells[0].Text.Trim() + "&begintime=" + txtGoAlongTime1.Value + "&endtime=" + txtGoAlongTime2.Value + "&currentuserid=" + this.currentuserid.Value.ToString();
            if (e.Row.Cells[0].Text != "合计")
            {
                e.Row.Cells[0].Attributes.Add("onclick", "OnClickgetUrl('" + url + "')");
                e.Row.Cells[0].Attributes.Add("style", "cursor:hand;text-decoration:Underline;color:blue;");
            }
        }
    }

    #region 导出报表
    /// <summary>
    /// 导出报表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOut_Click(object sender, EventArgs e)
    {
        Response.Clear();
        DownloadExcelFlag = true;
        Export("线下收银汇总报表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
    }
    bool DownloadExcelFlag = false;
    /// <summary>
    /// RenderControl
    /// </summary>
    /// <param name="writer"></param>
    public override void RenderControl(HtmlTextWriter writer)
    {
        if (DownloadExcelFlag)
            this.gvOffLineInfoNew.RenderControl(writer);
        else
            base.RenderControl(writer);
    }
    /// <summary>
    /// VerifyRenderingInServerForm
    /// </summary>
    /// <param name="control"></param>
    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!DownloadExcelFlag)
            base.VerifyRenderingInServerForm(control);
    }
    public void Export(string sFileName)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.Charset = "utf-8";
        Response.AppendHeader("Content-Disposition", "attachment;filename=" + Server.UrlEncode(sFileName + ".xls"));
        Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
        Response.ContentType = "application/ms-excel";
        EnableViewState = false;
        System.IO.StringWriter oStringWriter = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter oHtmlTextWriter = new System.Web.UI.HtmlTextWriter(oStringWriter);
        gvOffLineInfoNew.RenderControl(oHtmlTextWriter);
        Response.Write(AddExcelHead());
        Response.Write(oStringWriter.ToString());
        Response.Write(AddExcelbottom());
        Response.End();
    }
    public static string AddExcelHead()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<html xmlns:x=\"urn:schemas-microsoft-com:office:excel\">");
        sb.Append(" <head>");
        sb.Append(" <!--[if gte mso 9]><xml>");
        sb.Append("<x:ExcelWorkbook>");
        sb.Append("<x:ExcelWorksheets>");
        sb.Append("<x:ExcelWorksheet>");
        sb.Append("<x:Name></x:Name>");
        sb.Append("<x:WorksheetOptions>");
        sb.Append("<x:Print>");
        sb.Append("<x:ValidPrinterInfo />");
        sb.Append(" </x:Print>");
        sb.Append("</x:WorksheetOptions>");
        sb.Append("</x:ExcelWorksheet>");
        sb.Append("</x:ExcelWorksheets>");
        sb.Append("</x:ExcelWorkbook>");
        sb.Append("</xml>");
        sb.Append("<![endif]-->");
        sb.Append(" </head>");
        sb.Append("<body>");
        return sb.ToString();
    }
    public static string AddExcelbottom()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("</body>");
        sb.Append("</html>");
        return sb.ToString();
    }
    #endregion
  
}