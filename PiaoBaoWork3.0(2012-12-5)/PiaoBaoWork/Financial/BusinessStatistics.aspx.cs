using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using PbProject.Model;
public partial class Person_BusinessStatistics : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            hidcpyno.Value = mUser.CpyNo;
            hiduserid.Value = mUser.id.ToString();
            txtCPTime3.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            txtCPTime4.Value = DateTime.Now.ToString("yyyy-MM-dd");
            if (mUser.IsAdmin!=0)
            {
                thYG.Visible = false;
                tdYG.Visible = false;
            }
            else
            {
                thYG.Visible = true;
                tdYG.Visible = true;
            }
            //PageDataBind();
        }
    }

    /// <summary>
    /// btnQuery1_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery1_Click(object sender, EventArgs e)
    {
        if (txtCPTime3.Value != "" && txtCPTime4.Value != "" && DateTime.Parse(txtCPTime3.Value) < DateTime.Parse(txtCPTime4.Value).AddMonths(-1))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('统计时间跨度不能超过一个月！');", true);
            return;
        }
        else
        {
            PageDataBind("");
        }
    }
    /// <summary>
    /// 客户统计(第一个条件)
    /// </summary>
    /// <returns></returns>
    private string strQuery()
    {
        StringBuilder sb = new StringBuilder();
        if (rblcp.SelectedValue == "1")
        {
            if (txtCPTime3.Value.Trim().Length > 0)
            {
                sb.Append(" and CPTime >= convert(DateTime, '" + txtCPTime3.Value.Trim() + "')");
            }
            if (txtCPTime4.Value.Trim().Length > 0)
            {
                sb.Append(" and CPTime <= convert(DateTime, '" + txtCPTime4.Value + " 23:59:59')");
            }
        }
        else
        {
            sb.Append(" and len(_cpy.UninCode)=" + (mUser.CpyNo.Length + 6) + " and _cpy.UninCode like '" + mUser.CpyNo + "%'");
        }
        return sb.ToString();
    }
      /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    private string Query()
    {
        StringBuilder sb = new StringBuilder(" len(a.UninCode)=" + (mUser.CpyNo.Length + 6) + " and e.CpyNo like '" + mUser.CpyNo.ToString() + "%'");
        if (mUser.IsAdmin==0)
        {
            if (txtYG.Text.Trim().Length>0)
            {
                sb.AppendFormat(" and e.UserName like '%{0}%'", txtYG.Text.Trim());
            }
        }
        else
        {
            sb.AppendFormat(" and e.UserName = '{0}'",mUser.UserName);
        }
        if (txtTo.Text.Trim().Length>0)
        {
            sb.AppendFormat(" and UninAllName like '%{0}%'", txtTo.Text.Trim());
        }
        if (txtAccount.Text.Trim().Length > 0)
        {
            sb.AppendFormat(" and a.LoginName like '%{0}%'", txtAccount.Text.Trim());
        }
       
        return sb.ToString();
    }
   
    /// <summary>
    ///绑定信息
    /// </summary>
    private void PageDataBind(string showtype)
    {
        DataBase.Data.HashObject Hparams = new DataBase.Data.HashObject();
        Hparams.Add("strWhere", strQuery());
        Hparams.Add("sqlWhere", Query());
        Hparams.Add("orderbyWhere", "order by " + ddlSort.SelectedValue + " desc");
        Hparams.Add("showType", int.Parse(rblcp.SelectedValue));
        DataTable[] dsSalesInfoCollect = null;
        dsSalesInfoCollect = base.baseDataManage.MulExecProc("pro_BusinessStatistics", Hparams);
        gvBusinessInfo.DataSource = dsSalesInfoCollect[0];
        gvBusinessInfo.DataBind();

        if (!string.IsNullOrEmpty(showtype))
        {
            DataTable dt = dsSalesInfoCollect[0];
            DataRow drnul = dt.NewRow();
            for (int i = 0; i < 50; i++)
            {
                dt.Rows.Add(drnul[1]);
            }
            gvBusinessInfoNew.DataSource = dt;
            gvBusinessInfoNew.DataBind();
        }
        if (gvBusinessInfo.Rows.Count > 2 && rblcp.SelectedValue=="1")
        {
            gvBusinessInfo.Rows[0].Attributes.Add("Style", "color:Red");
            if (!string.IsNullOrEmpty(showtype))
            {
                gvBusinessInfoNew.Rows[0].Attributes.Add("Style", "color:Red");
            }
        }
        else
        {
            gvBusinessInfo.Rows[gvBusinessInfo.Rows.Count - 1].Attributes.Add("Style", "color:Red");
            if (!string.IsNullOrEmpty(showtype))
            {
                gvBusinessInfoNew.Rows[gvBusinessInfoNew.Rows.Count - 1].Attributes.Add("Style", "color:Red");
            }
        }
    }
    /// <summary>
    /// btnClear2_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear2_Click(object sender, EventArgs e)
    {
        txtCPTime3.Value = DateTime.Now.AddMonths(-1).ToShortDateString();
        txtCPTime4.Value = DateTime.Now.ToShortDateString();
        txtTo.Text = "";
    }


    bool DownloadExcelFlag = false;
    /// <summary>
    /// btnOut_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOut_Click(object sender, EventArgs e)
    {
        PageDataBind("excel");
        Response.Clear();
        DownloadExcelFlag = true;
        Export("业务员统计_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
    }


    /// <summary>
    /// 通过 DataSet 导出 Excel
    /// </summary>
    /// <param name="FileName">文件名：默认“未命名.xls”</param>

    #region 导出Excel
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

        gvBusinessInfoNew.RenderControl(oHtmlTextWriter);
        string strValue = AddExcelHead() + oStringWriter.ToString() + AddExcelbottom();
        Response.Write(strValue);
        Response.End();
    }
    public override void RenderControl(HtmlTextWriter writer)
    {
        if (DownloadExcelFlag)
        {
            this.gvBusinessInfoNew.RenderControl(writer);
        }
        else
        {
            base.RenderControl(writer);
        }
    }
    public override void VerifyRenderingInServerForm(Control control)
    {
        if (!DownloadExcelFlag)
            base.VerifyRenderingInServerForm(control);
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
    protected void gvBusinessInfo_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.Cells[1].Text != "客户" && e.Row.Cells[1].Text.Trim() != "&nbsp;" && !string.IsNullOrEmpty(e.Row.Cells[1].Text.Trim()))
        {
            e.Row.Cells[1].Attributes.Add("onclick", "mouseOver(null,'" + e.Row.Cells[1].Text + "',window.event)");
            e.Row.Cells[1].Attributes.Add("style", "cursor:hand;text-decoration:Underline;color:blue;");
        }
    }
}