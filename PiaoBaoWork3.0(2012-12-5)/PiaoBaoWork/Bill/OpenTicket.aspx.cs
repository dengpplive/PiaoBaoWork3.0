using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Data;
using System.Data.OleDb;
using DataBase.Data;
using System.Text.RegularExpressions;
/// <summary>
/// open票扫描
/// </summary>
public partial class Bill_OpenTicket : BasePage
{
    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Init();
        }
    }

    public void Init()
    {
        ddlSel.Visible = false;
        currentuserid.Value = mUser.id.ToString();
        Hid_Office.Value = this.configparam.Office.Split('^')[0];
        Hid_CpyNo.Value = mCompany.UninCode;



        //当前时间
        DateTime dt = DateTime.Now;
        //乘机日期
        txtCPEndTime.Value = dt.ToString("yyyy-MM-dd");
        txtCPStartTime.Value = dt.AddDays(-7).ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Excel装换为Datable
    /// </summary>
    /// <param name="strExcelFileName"></param>
    /// <param name="strSheetName"></param>
    /// <returns></returns>
    public System.Data.DataTable ExcelToDataTable(out bool IsSuc)
    {
        IsSuc = false;
        DataTable table = new DataTable();
        string filepath = Server.HtmlEncode(fileUpload.PostedFile.FileName);
        string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);
        string masthrod = DateTime.Now.ToString("yyyyMMddHHmmss");
        string serverpath = Server.MapPath("~/upload/" + masthrod) + filename;
        fileUpload.PostedFile.SaveAs(serverpath);
        if (File.Exists(serverpath))
        {
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + serverpath + ";" + "Extended Properties='Excel 8.0;HDR=yes;IMEX=1';";
            string strExcelSql = "select * from [Sheet1$]";

            System.Data.DataSet ds = new DataSet();
            OleDbConnection conn = new OleDbConnection(strConn);
            try
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter(strExcelSql, strConn);
                adapter.Fill(ds, "mytable");
                table = ds.Tables["mytable"];
            }
            catch (System.Data.OleDb.OleDbException e)
            {
                IsSuc = true;
                PnrAnalysis.LogText.LogWrite(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "", "OpenTicket");
            }
            finally
            {
                conn.Close();
                try
                {
                    if (File.Exists(serverpath))
                    {
                        File.Delete(serverpath);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        return table;
    }


    /// <summary>
    /// 读取Excel数据在界面上显示
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReadExcel_Click(object sender, EventArgs e)
    {
        try
        {
            //政策批量修改数据获取
            bool IsSuc = false;
            DataTable table = ExcelToDataTable(out IsSuc);
            if (IsSuc)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showMsgDg('该Excel不是使用的Excel模板');", true);
                return;
            }
            if (table != null && table.Rows.Count > 0)
            {
                Repeater.DataSource = table;
                Repeater.DataBind();
                ddlSel.Visible = true;
                span_1.Visible = true;
            }
            else
            {
                ddlSel.Visible = false;
                span_1.Visible = false;
                Hid_TicketNumber.Value = "";
            }
            span_2.Attributes["class"] = "btn btn-ok-s hide";
        }
        catch (Exception ex)
        {
            PnrAnalysis.LogText.LogWrite("open票导入:" + ex.Message, "openErr");
        }
    }
    /// <summary>
    /// 读取系统票号在界面上显示
    /// </summary>  
    protected void btnReadSysTicketNumber_Click(object sender, EventArgs e)
    {
        try
        {
            HashObject queryParamter = new HashObject();
            queryParamter.Add("CpyNo", mCompany.UninCode);
            queryParamter.Add("startTime", "");
            queryParamter.Add("endTime", "");
            if (txtCPStartTime.Value.Trim() != "" && txtCPEndTime.Value.Trim() != "")
            {
                queryParamter["startTime"] = txtCPStartTime.Value.Trim() + " 00:00:00";
                queryParamter["endTime"] = txtCPEndTime.Value.Trim() + " 23:59:59";
            }
            DataTable table = this.baseDataManage.EexcProc("GetOpenTicketNumber1", queryParamter);
            Repeater.DataSource = table;
            Repeater.DataBind();
            if (table != null && table.Rows.Count > 0)
            {
                ddlSel.Visible = true;
                span_1.Visible = true;
            }
            else
            {
                ddlSel.Visible = false;
                span_1.Visible = false;
                Hid_TicketNumber.Value = "";
            }
            span_2.Attributes["class"] = "btn btn-ok-s hide";
        }
        catch (Exception ex)
        {
            PnrAnalysis.LogText.LogWrite("open票查询:" + ex.Message, "openErr");
        }
    }

    public void SetTicketNumber(DataTable table)
    {
        Hid_TicketNumber.Value = "";
        List<string> lstTicketNumber = new List<string>();
        foreach (DataRow dr in table.Rows)
        {
            if (dr["票号"] != DBNull.Value && dr["票号"].ToString() != "")
            {
                lstTicketNumber.Add(dr["票号"].ToString());
            }
        }
        if (lstTicketNumber.Count > 0)
        {
            Hid_TicketNumber.Value = string.Join("|", lstTicketNumber.ToArray());
        }
    }
    /// <summary>
    /// 下载指定Excel格式
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnDownloadExcel_Click(object sender, EventArgs e)
    {
        string filePath = Server.MapPath(@"~/upload/Open票导入报表格式.xls");

        if (System.IO.File.Exists(filePath))
        {
            FileInfo file = new FileInfo(filePath);
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(file.Name));
            Response.AddHeader("Content-length", file.Length.ToString());
            Response.ContentType = "appliction/octet-stream";
            Response.WriteFile(file.FullName);
            Response.End();
        }
    }
    /// <summary>
    /// 导出数据到指定路径 下载Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnImportData_Click(object sender, EventArgs e)
    {
        Export("Open票_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
    }
    public void Export(string sFileName)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.Charset = "gb2312";
        Response.AppendHeader("Content-Disposition", "attachment;filename=" + sFileName + ".xls");
        Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
        Response.ContentType = "application/ms-excel";
        string strData = HttpUtility.UrlDecode(Hid_ImportData.Value, Encoding.UTF8);
        string regExp = @"<td\s*format=[""]?0[""]?\s*>\s*(?<TK>\d{3,4}(\-?|\s+)\d{10})\s*</td>";
        MatchEvaluator me = new MatchEvaluator(
        delegate(Match mch)
        {
            string result = "";
            if (mch.Success)
            {
                result += "<td>'" + mch.Groups["TK"].Value + "</td>";
            }
            return result;
        });
        strData = Regex.Replace(strData, regExp, me, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        Response.Write(AddExcelHead());
        Response.Write(strData);
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
}