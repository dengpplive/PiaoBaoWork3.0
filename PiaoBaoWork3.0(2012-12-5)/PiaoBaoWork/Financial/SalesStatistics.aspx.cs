using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using PbProject.Model;
using PbProject.WebCommon.Utility;

/// <summary>
/// 客户销售统计
/// </summary>
public partial class Financial_SalesStatistics : BasePage
{
   
    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
           
            txtCPTimeBegin.Value = DateTime.Now.ToString("yyyy-MM-dd");
            txtCPTimeEnd.Value = DateTime.Now.ToString("yyyy-MM-dd");
            
        }
        ListBind();
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery1_Click(object sender, EventArgs e)
    {
        DateTime dt1 = DateTime.Parse(txtCPTimeBegin.Value);
        DateTime dt2 = DateTime.Parse(txtCPTimeEnd.Value);
        if (dt2 >= dt1 && dt2.AddMonths(-1) > dt1)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('请查询1个月以内的数据!');", true);
            return;
        }
        else if (dt1 > dt2)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('请选择正确的查询时间!');", true);
            return;
        }
        PageDataBind("");
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear2_Click(object sender, EventArgs e)
    {
        txtCPTimeBegin.Value = DateTime.Now.ToString("yyyy-MM-dd");
        txtCPTimeEnd.Value = DateTime.Now.ToString("yyyy-MM-dd");
        this.txtStartCity.Text = "";
        this.ddlCarrier.Value = "";
        this.ddlpaytype.SelectedIndex = 0;
        //this.ddlloginname.SelectedIndex = 0;
        //this.ddlCustomer.SelectedIndex = 0;
        this.txtUserAccount.Text = "";
        txtTo.Text = ""; 
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    private string Query()
    {
        StringBuilder sb = new StringBuilder("OrderStatusCode in (4,16,17)");
        if (mCompany.RoleType != 1 && mCompany.RoleType != 2)
        {
            sb.Append(" and UninCode = '" + mCompany.UninCode + "'");
        }
        else if (mCompany.RoleType ==2)
        {
            sb.Append(" and UninCode like '"+mCompany.UninCode+"%'");
        }
      
        if (txtCPTimeBegin.Value.Trim() != "")
        {
            sb.Append(" and _order.CPTime >= convert(DateTime, '" + txtCPTimeBegin.Value + "')");
        }
        if (txtCPTimeEnd.Value.Trim() != "")
        {
            sb.Append(" and _order.CPTime <= convert(DateTime, '" + txtCPTimeEnd.Value + " 23:59:59')");
        }
        if (txtTo.Text.Trim() != "")
        {
            sb.Append(" and _cpy.UninAllName  like'%" + CommonManage.TrimSQL(txtTo.Text.Trim()) + "%'");
        }
        if (txtUserAccount.Text.Trim() != "")
        {
            sb.Append(" and _user.LoginName  like'%" + CommonManage.TrimSQL(txtUserAccount.Text.Trim()) + "%'");
        }
        if (ddlCarrier.Value != "0" && ddlCarrier.Value!="")
        {
            sb.Append(" and _order.CarryCode = '" + ddlCarrier.Value + "'");
        }
        if (ddlpaytype.SelectedValue !="")
        {
            sb.Append(" and _order.PayWay = '" + ddlpaytype.SelectedValue + "'");
        }
        if (txtStartCity.Text !="")
        {
            sb.Append(" and _order.Travel like '" + CommonManage.TrimSQL(txtStartCity.Text.Trim()) + "%'");
        }
        
        return sb.ToString();
    }
    /// <summary>
    /// 绑定信息
    /// </summary>
    private void PageDataBind(string showtype)
    {
        DataBase.Data.HashObject Hparams = new DataBase.Data.HashObject();
        Hparams.Add("sqlWhere", Query());
        //1.机票信息汇总
        DataTable[] dsSalesInfoCollect = base.baseDataManage.MulExecProc("pro_UserSalesCollect", Hparams);
        gvSalesInfo.DataSource = dsSalesInfoCollect[0];
        gvSalesInfo.DataBind();

        if (!string.IsNullOrEmpty(showtype))
        {
            DataTable dt = dsSalesInfoCollect[0];
            DataRow drnul = dt.NewRow();
            for (int i = 0; i < 50; i++)
            {
                dt.Rows.Add(drnul[1]);
            }
            gvSalesInfoNew.DataSource = dt;
            gvSalesInfoNew.DataBind();
        }
        if (gvSalesInfo.Rows.Count>2)
        {
            gvSalesInfo.Rows[0].Attributes.Add("Style", "color:Red");
            if (!string.IsNullOrEmpty(showtype))
            {
                gvSalesInfoNew.Rows[0].Attributes.Add("Style", "color:Red");
            }
        }
        else
        {
            gvSalesInfo.Rows[gvSalesInfo.Rows.Count - 1].Attributes.Add("Style", "color:Red");
            if (!string.IsNullOrEmpty(showtype))
            {
                gvSalesInfoNew.Rows[gvSalesInfoNew.Rows.Count - 1].Attributes.Add("Style", "color:Red");
            }
        }
        
    }

    bool DownloadExcelFlag = false;
    /// <summary>
    /// RenderControl
    /// </summary>
    /// <param name="writer"></param>
    public override void RenderControl(HtmlTextWriter writer)
    {
        if (DownloadExcelFlag)
            this.gvSalesInfoNew.RenderControl(writer);
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
    /// <summary>
    /// Export
    /// </summary>
    /// <param name="sFileName"></param>
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
        gvSalesInfoNew.RenderControl(oHtmlTextWriter);
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
    /// <summary>
    /// 导出报表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOut_Click(object sender, EventArgs e)
    {
        PageDataBind("excel");
        Response.Clear();
        DownloadExcelFlag = true;
        Export("客户销售统计报表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
    }
   
    /// <summary>
    /// 绑定客户名称，账号
    /// </summary>
    private void ListBind()
    {
        try
        {
           
            string comsqlwhere = "1=1";
            string empsqlwhere = "1=1";
            if (mCompany.RoleType != 1 && mCompany.RoleType != 2)
            {
                comsqlwhere = " UninCode = '" + mCompany.UninCode + "'";
                empsqlwhere = " CpyNo='" + mCompany.UninCode + "'";
            }
            else if (mCompany.RoleType == 2)
            {
                comsqlwhere = " UninCode like '" + mCompany.UninCode + "%'";
                empsqlwhere = " CpyNo like '" + mCompany.UninCode + "%'";
            }
            //IList<User_Company> comlist = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { "UninAllName,UninCode",comsqlwhere }) as List<User_Company>;
            //IList<User_Employees> emplist = baseDataManage.CallMethod("User_Employees", "GetList", null, new Object[] { "CpyNo,LoginName", "IsAdmin=0 and " + empsqlwhere }) as List<User_Employees>;
            //ddlCustomer.DataSource = comlist;
            //ddlCustomer.DataValueField = "UninCode";
            //ddlCustomer.DataTextField = "UninAllName";
            //ddlCustomer.DataBind();
            //ListItem item = new ListItem();
            //item.Text = "";
            //item.Value = "";
            //ddlCustomer.Items.Insert(0, item);


            //ddlloginname.DataSource = emplist;
            //ddlloginname.DataValueField = "CpyNo";
            //ddlloginname.DataTextField = "LoginName";
            //ddlloginname.DataBind();
            //ddlloginname.Items.Insert(0, item);

            ddlpaytype.Items.Add(new ListItem("--选择--", ""));
            ddlpaytype.Items.Add(new ListItem("支付宝", "1"));
            ddlpaytype.Items.Add(new ListItem("快钱", "2"));
            ddlpaytype.Items.Add(new ListItem("汇付", "3"));
            ddlpaytype.Items.Add(new ListItem("财付通", "4"));
            ddlpaytype.Items.Add(new ListItem("账户支付", "14"));
            ddlpaytype.Items.Add(new ListItem("收银", "15"));
          
        
            //hidCustomervalue(comlist);
            //hidLoginNamevalue(emplist);
        }
        catch (Exception)
        {
            
            throw;
        }
       
    }
    private void hidCustomervalue(IList<User_Company> comlist)
    {
        string Customer = "";
        foreach (User_Company com in comlist)
        {
            Customer += com.UninCode + "^" + com.UninAllName + "|";
        }
        if (Customer != "")
        {
            Customer = Customer.Substring(0, Customer.Length - 1);
        }
        hidCustomer.Value = Customer;
    }
    private void hidLoginNamevalue(IList<User_Employees> emplist)
    {
        string Customer = "";
        foreach (User_Employees com in emplist)
        {
            Customer += com.CpyNo + "^" + com.LoginName + "|";
        }
        if (Customer != "")
        {
            Customer = Customer.Substring(0, Customer.Length - 1);
        }
        hidLoginName.Value = Customer;
    }
    protected void gvSalesInfo_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1 && e.Row.RowType == DataControlRowType.DataRow)
        {
            string url = "../Bill/BillOfCount.aspx?cpyname=" + e.Row.Cells[0].Text.Trim() + "&begintime=" + txtCPTimeBegin.Value + "&endtime=" + txtCPTimeEnd.Value + "&currentuserid="+mUser.id;
            if (e.Row.Cells[0].Text != "合计" && e.Row.Cells[0].Text != "")
            {
                e.Row.Cells[0].Attributes.Add("onclick", "OnClickgetUrl('" + url + "')");
                e.Row.Cells[0].Attributes.Add("style", "cursor:hand;text-decoration:Underline;color:blue;");
            }
        }
    }
}