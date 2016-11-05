using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Data.OleDb;
using DataBase.Data;
using System.Text;
using System.Collections;

public partial class Policy_OperationPolicy : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// 导入政策
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btn_Click(object sender, EventArgs e)
    {

        string msg = "请选择Excel文件";
        //string savename = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(this.FileUpload.FileName).ToLower();
        //string UpFilePath = Server.MapPath("../upload/" + savename);
        try
        {

            if (FileUpload.FileName != "")
            {
                string fex = Path.GetExtension(FileUpload.FileName).ToLower();
                if (fex != ".xls" && fex != ".xlsx")
                {
                    msg = "文件必须是excel";
                }
                else
                {
                    IHashObject parameter = new HashObject();
                     byte[] fileBytes = FileUpload.FileBytes;
                     if (!ExcelRender.HasData(new MemoryStream(fileBytes)))
                     {
                         msg = "导入数据为空";
                     }
                     else
                     {
                         DataTable dt = ExcelRender.RenderFromExcel(new MemoryStream(fileBytes));
                         int num = 0;
                         ArrayList list = new ArrayList();
                         for (int i = 0; i < dt.Rows.Count; i++)
                         {

                             if (dt.Rows[i]["公司编号"].ToString().Length == 0)
                             {
                                 list.Add("第" + (i + 1) + "行,公司编号不能为空</br>");
                             }
                             if (dt.Rows[i]["政策种类(1普通，2特价)"].ToString() != "0" && dt.Rows[i]["政策种类(1普通，2特价)"].ToString() != "1" && dt.Rows[i]["政策种类(1普通，2特价)"].ToString() != "2")
                             {
                                 list.Add("第" + (i + 1) + "行,政策种类请输入1或2</br>");
                             }
                             if (dt.Rows[i]["票价生成方式(1正常价格，2动态特价，3固定特价)"].ToString() != "1" && dt.Rows[i]["票价生成方式(1正常价格，2动态特价，3固定特价)"].ToString() != "2" && dt.Rows[i]["票价生成方式(1正常价格，2动态特价，3固定特价)"].ToString() != "3")
                             {
                                 list.Add("第" + (i + 1) + "行,票价生成方式请输入1或2,3</br>");
                             }
                             if (dt.Rows[i]["发布类型(1出港，2入港,3全国)"].ToString() != "1" && dt.Rows[i]["发布类型(1出港，2入港,3全国)"].ToString() != "2" && dt.Rows[i]["发布类型(1出港，2入港,3全国)"].ToString() != "3")
                             {
                                 list.Add("第" + (i + 1) + "行,发布类型请输入1或2,3</br>");
                             }
                             if (dt.Rows[i]["航空公司返点"].ToString().Length > 0)
                             {
                                 if (int.TryParse(dt.Rows[i]["航空公司返点"].ToString(), out num))
                                 {
                                     if (Convert.ToInt32(dt.Rows[i]["航空公司返点"].ToString()) < 0 || Convert.ToInt32(dt.Rows[i]["航空公司返点"].ToString()) >= 100)
                                     {
                                         list.Add("第" + (i + 1) + "行,航空公司返点请输入1到100之间的数字</br>");
                                     }
                                 }
                             }
                             else
                             {
                                 list.Add("第" + i + 1 + "行,航空公司返点请输入1到100之间的数字</br>");
                             }

                             if (list.Count < 1)
                             {
                                 parameter.Add(i.ToString(), "insert into Tb_Ticket_Policy(CpyNo,CpyName,PolicyKind,GenerationType,ReleaseType," +
                                                                                    "CarryCode,TravelType,PolicyType,TeamFlag,Office," +
                                                                                    "StartCityNameCode,StartCityNameSame,MiddleCityNameCode,MiddleCityNameSame,TargetCityNameCode," +
                                                                                    "TargetCityNameSame,ApplianceFlightType,ApplianceFlight,UnApplianceFlight,ScheduleConstraints," +
                                                                                    "ShippingSpace,SpacePrice,AdvanceDay,DownPoint,LaterPoint," +
                                                                                    "SharePoint,AirReBate,FlightStartDate,FlightEndDate,PrintStartDate," +
                                                                                    "PrintEndDate,AuditDate,AuditType,AuditLoginName,AuditName," +
                                                                                    "CreateDate,CreateLoginName,CreateName,Remark,IsApplyToShareFlight," +
                                                                                    "ShareAirCode,IsLowerOpen,HighPolicyFlag,AutoPrintFlag,A2) " +
                                "values('" + dt.Rows[i]["公司编号"].ToString() + "','" + dt.Rows[i]["供应商名字"].ToString() + "'," + dt.Rows[i]["政策种类(1普通，2特价)"].ToString() + "," + dt.Rows[i]["票价生成方式(1正常价格，2动态特价，3固定特价)"].ToString() + "," + dt.Rows[i]["发布类型(1出港，2入港，3全国)"].ToString() + "," +
                                "'" + dt.Rows[i]["承运人"].ToString() + "','" + dt.Rows[i]["行程类型(1单程，2往返/单程，3往返，4中转联程)"].ToString() + "','" + dt.Rows[i]["政策类型(1=B2B，2=BSP，3=B2B/BSP)"].ToString() + "','" + dt.Rows[i]["团队标志(0普通，1团队)"].ToString() + "','" + dt.Rows[i]["出票Office号"].ToString() + "'," +
                                "'" + dt.Rows[i]["出发城市三字码"].ToString() + "','" + dt.Rows[i]["出发城市同城机场共享政策(1是，2否)"].ToString() + "','" + dt.Rows[i]["中转城市三字码"].ToString() + "','" + dt.Rows[i]["中转城市同城机场共享政策(1是，2否)"].ToString() + "','" + dt.Rows[i]["到达城市三字码"].ToString() + "'," +
                                "'" + dt.Rows[i]["到达城市同城机场共享政策(1是，2否)"].ToString() + "','" + dt.Rows[i]["适用航班号类型(1全部2适用3不适用)"].ToString() + "','" + dt.Rows[i]["适用航班"].ToString() + "','" + dt.Rows[i]["不适用航班"].ToString() + "','" + dt.Rows[i]["班期限制"].ToString() + "'," +
                                "'" + dt.Rows[i]["舱位"].ToString() + "','" + dt.Rows[i]["舱位价格"].ToString() + "','" + dt.Rows[i]["提前天数"].ToString() + "','" + dt.Rows[i]["下级分销返点"].ToString() + "','" + dt.Rows[i]["下级分销后返"].ToString() + "'," +
                                "'" + dt.Rows[i]["共享政策返点"].ToString() + "','" + dt.Rows[i]["航空公司返点"].ToString() + "','" + dt.Rows[i]["乘机生效日期"].ToString() + "','" + dt.Rows[i]["乘机失效日期"].ToString() + "','" + dt.Rows[i]["出票生效日期"].ToString() + "'," +
                                "'" + dt.Rows[i]["出票失效日期"].ToString() + "','" + dt.Rows[i]["审核时间"].ToString() + "','" + dt.Rows[i]["审核状态(1已审，2未审)"].ToString() + "','" + dt.Rows[i]["审核人帐户"].ToString() + "','" + dt.Rows[i]["审核人姓名"].ToString() + "'," +
                                "'" + DateTime.Now.ToString() + "','" + mUser.LoginName + "','" + mUser.UserName + "','" + dt.Rows[i]["备注"].ToString() + "','" + dt.Rows[i]["是否适用于共享航班(1适用，0不适用)"].ToString() + "'," +
                                "'" + dt.Rows[i]["适用共享航空公司二字码(如:CA/CZ/ZH/HU)"].ToString() + "'," + dt.Rows[i]["是否往返低开(0不低开，1低开)"].ToString() + "," + dt.Rows[i]["是否高返政策(1是，其它不是)"].ToString() + "," + dt.Rows[i]["自动出票方式(手动(0或者null空)，半自动1，自动2)"].ToString() + ",1)");
                             }
                         }
                         if (list.Count >= 1)
                         {
                             msg = "";
                             for (int j = 0; j < list.Count; j++)
                             {
                                 msg += list[j].ToString();
                             }
                         }
                         else
                         {
                             msg = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().ImportPolicy(parameter) == true ? "导入成功(" + parameter.Count + " 条数据)" : "导入失败";
                         }
                     }
                }
            }

        }
        catch (Exception ex)
        {
            msg = ex.ToString();
            throw ex;
        }
        //File.Delete(UpFilePath);
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
    /// <summary>
    /// 导出政策
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btndc_Click(object sender, EventArgs e)
    {
        //IHashObject parameter = new HashObject();
        //GridView1.DataSource = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().GetPolicyExcelByStrWhere("公司编号='" + mCompany.UninCode + "'");
        //GridView1.DataBind();
        //Response.Clear();
        //DownloadExcelFlag = true;
        //Export("政策表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
        DataTable dt = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().GetPolicyExcelByStrWhere("公司编号='" + mCompany.UninCode + "'");
        ExcelRender.RenderToExcel(dt, Context, "政策表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")+".xls");
      

    }

    #region 导出方法
    /// <summary>
    /// 导出报表
    /// </summary>
    /// <param name="sFileName">文件名称</param>
    /// <param name="type"></param>
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
        GridView1.RenderControl(oHtmlTextWriter);

        string strValue = AddExcelHead() + oStringWriter.ToString() + AddExcelbottom();
        Response.Write(strValue);
        Response.End();
    }
    bool DownloadExcelFlag = false;

    public override void RenderControl(HtmlTextWriter writer)
    {
        if (DownloadExcelFlag)
        {
            GridView1.RenderControl(writer);
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
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowIndex != -1 && e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        e.Row.Cells[i].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
                    }
                }
            }
        }
        catch (Exception ex)
        {

        }
    }
}