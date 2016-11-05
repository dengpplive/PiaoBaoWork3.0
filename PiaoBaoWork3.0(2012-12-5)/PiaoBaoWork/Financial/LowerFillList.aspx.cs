using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;
using PbProject.Model;
using System.Text;
using System.Data;
using PbProject.WebCommon.Utility;

public partial class Financial_LowerFillList : BasePage
{

    protected List<Bd_Base_Parameters> listParameters = null;
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
    protected void Page_Load(object sender, EventArgs e)
    {
       
        if (!IsPostBack)
        {
            Curr = 1;
            Query();
            AspNetPager1.PageSize = 20;
            //listParameters = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "SetName='gongYingKongZhiFenXiao' and SetValue like '%|76|%'" }) as List<Bd_Base_Parameters>;
            //ViewState["listParameters"] = listParameters;
            if (mCompany.RoleType != 1 && mCompany.RoleType != 2)//分销采购权限(显示本身)
            {
                showtop.Visible = false;
                BindV_User_Company_List();
            }
        }
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    protected void Query()
    {
        StringBuilder strWhere = new StringBuilder();
        strWhere.Append("  1=1 ");
        if (txtLoginName.Text != "")
        {
            strWhere.Append(" and LoginName='" + CommonManage.TrimSQL(txtLoginName.Text.Trim()) + "'");
        }
        if (mCompany.RoleType != 1 && mCompany.RoleType != 2)
        {
            strWhere.Append(" and UninCode = '" + mCompany.UninCode + "'");
        }
        else if (mCompany.RoleType == 2)
        {
            strWhere.Append(" and UninCode like '" + mCompany.UninCode + "%'");
        }
        strWhere.Append(" and len(unincode)<>12 ");
        if (txtUninAllNAME.Text != "")
        {
            strWhere.Append(" and uninAllNAME like '%" + CommonManage.TrimSQL(txtUninAllNAME.Text.Trim()) + "%'");
        }

        Con = strWhere.ToString();
        Curr = 1;

    }
    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        BindV_User_Company_List();
    }
    /// <summary>
    /// 绑定
    /// </summary>
    protected void BindV_User_Company_List()
    {
        try
        {
            int num=0;
            DataTable dt =baseDataManage.GetBasePagerDataTable("V_User_Company_Listnew",out num,  AspNetPager1.PageSize ,Curr,"*",Con," len(UninCode)");
            AspNetPager1.RecordCount = num;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
            repPosList.DataSource = dt;
            repPosList.DataBind();
        }
        catch (Exception)
        {

            throw;
        }
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Query();
        BindV_User_Company_List();
    }
    protected void btnreset_Click(object sender, EventArgs e)
    {
        txtUninAllNAME.Text = "";
        txtLoginName.Text = "";
    }
    public string encodeName(string str)
    {
       return Server.UrlEncode(str);
    }
    /// <summary>
    /// 导出
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        DataTable dt = baseDataManage.ExecuteStrSQL("select uninAllNAME as 客户名称,LoginName as 客户账号,AccountMoney as 账户余额,MaxDebtMoney as 最大欠款额度 ,MaxDebtDays as 最大欠款天数 from V_User_Company_List where "+Con);
        //new ExcelData().SaveToExcel(dt, "客户账户报表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
        DataRow drnul = dt.NewRow();
        for (int i = 0; i < 50; i++)
        {
            dt.Rows.Add(drnul[1]);
        }
        GridView1.DataSource = dt;
        GridView1.DataBind();
        Response.Clear();
        DownloadExcelFlag = true;
        Export("客户账户报表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
    }
    /// <summary>
    /// 开关预存款
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void repPosList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string msg = "";
        try
        {
            if (e.CommandName.ToString() == "Clear")
            {
                msg = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().ExecuteNonQuerySQLInfo("update User_Company set AccountPwd='' where unincode='" + e.CommandArgument + "'") == true ? "操作成功" : "操作失败";
            }
            else
            {
                //listParameters = ViewState["listParameters"] as List<Bd_Base_Parameters>;
                listParameters = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + e.CommandArgument + "' and SetName='gongYingKongZhiFenXiao'" }) as List<Bd_Base_Parameters>;
                IHashObject parameter = new HashObject();
                parameter.Add("CpyNo", e.CommandArgument);
                parameter.Add("SetName", "gongYingKongZhiFenXiao");

                parameter.Add("SetDescription", "供应控制分销");
                parameter.Add("StartDate", "2012-12-05 00:00:00.000");
                parameter.Add("EndDate", "2112-12-05 00:00:00.000");
                parameter.Add("Remark", "供应控制分销");
                string types = e.CommandName.ToString();

                if (types == "开启预存款")
                {
                    parameter.Add("SetValue", "|76|");
                    if (listParameters == null || listParameters.Count <= 0)
                    {
                        msg = (bool)baseDataManage.CallMethod("Bd_Base_Parameters", "Insert", null, new Object[] { parameter }) == true ? "操作成功" : "操作失败";
                    }
                    else
                    {
                        msg = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().ExecuteNonQuerySQLInfo("update Bd_Base_Parameters set SetValue=SetValue+'|76|' where CpyNo='" + e.CommandArgument + "' and SetName='gongYingKongZhiFenXiao'") == true ? "操作成功" : "操作失败";
                    }

                }
                else if (types == "关闭预存款")
                {
                    parameter.Add("SetValue", "");
                    if (listParameters == null || listParameters.Count <= 0)
                    {
                        msg = (bool)baseDataManage.CallMethod("Bd_Base_Parameters", "Insert", null, new Object[] { parameter }) == true ? "操作成功" : "操作失败";
                    }
                    else
                    {
                        msg = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().ExecuteNonQuerySQLInfo("update Bd_Base_Parameters set SetValue='' where CpyNo='" + e.CommandArgument + "' and SetName='gongYingKongZhiFenXiao'") == true ? "操作成功" : "操作失败";

                    }
                }
             
            }
            Query();
            BindV_User_Company_List();
        }
        catch (Exception)
        {
            msg = "操作异常";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
   /// <summary>
   /// 获取预存款是否开启
   /// </summary>
   /// <param name="cpyno"></param>
   /// <returns></returns>
    public string returntext(string setvalue)
    {
        //listParameters = ViewState["listParameters"] as List<Bd_Base_Parameters>;
        //string showtext = "开启预存款";
        //string[] cpynos = new string[listParameters.Count];
        //if (listParameters != null && listParameters.Count > 0)
        //{
        //    for (int i = 0; i < listParameters.Count; i++)
        //    {
        //        cpynos.SetValue(listParameters[i].CpyNo, i);
        //    }
        //}
        //for (int i = 0; i < cpynos.Length; i++)
        //{
        //    if (cpyno == cpynos[i])
        //    {
        //        showtext = "关闭预存款";
        //        break;
        //    }
        //}
        //return showtext;
        string showtext = "开启预存款";
        if (setvalue.Contains("|76|"))
        {
            showtext = "关闭预存款";
        }
        return showtext;
    }
    /// <summary>
    /// repPosList_ItemDataBound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void repPosList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        try
        {

            if (e.Item.ItemIndex != -1)
            {
                Button btn1 = e.Item.FindControl("Button1") as Button;
                
                if (mCompany.RoleType == 2)
                {
                    if (BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao.Contains("|76|"))
                    {
                        btn1.Visible = true;
                        e.Item.FindControl("divclear").Visible = true;
                    }
                    e.Item.FindControl("divmake").Visible = true;
                }
                else if (mCompany.RoleType ==4 || mCompany.RoleType ==5)
                {
                    e.Item.FindControl("divshowpwd").Visible = true;
                }
            }
        }
        catch (Exception)
        {

        }
    }


    #region 导出报表
    bool DownloadExcelFlag = false;
    /// <summary>
    /// RenderControl
    /// </summary>
    /// <param name="writer"></param>
    public override void RenderControl(HtmlTextWriter writer)
    {
        if (DownloadExcelFlag)
            this.GridView1.RenderControl(writer);
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
        GridView1.RenderControl(oHtmlTextWriter);
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