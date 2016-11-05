using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using System.Data;
using System.Text;
using PbProject.WebCommon.Utility;

public partial class Bill_BillOfFz : BasePage
{
    protected int Curr
    {
        get { return (int)ViewState["currpage"]; }
        set { ViewState["currpage"] = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
       
            if (!IsPostBack)
            {
                if (mCompany.RoleType == 4 || mCompany.RoleType == 5)//分销权限
                {
                    showtryy.Visible = false;
                    OrderSourceth.Visible = false;
                    OrderSourcetd.Visible = false;
                    cpuserth.Visible = false;
                    cpusertd.Visible = false;

                }
                ViewState["type"] = "";
                Curr = 1;
                ListDataBind();
                if (Request["cpyname"] != null && Request["begintime"] != null && Request["endtime"] != null)
                {
                    cptimestart.Value = Request["begintime"];
                    cptimeend.Value = Request["endtime"];
                    txtCustomer.Text = Request["cpyname"].ToString();
                    lbtnDc3.Visible = false;
                    AspNetPager1.CurrentPageIndex = Curr;
                    AspNetPager1.PageSize = int.Parse(selPageSize.Value);
                    PageDataBind();
                }
                else
                {
                    cptimestart.Value = DateTime.Now.ToString("yyyy-MM-dd");
                    cptimeend.Value = DateTime.Now.ToString("yyyy-MM-dd");
                }
        }
    }
    #region 数据绑定,加载
    /// <summary>
    /// 绑定数据列表
    /// </summary>
    private void ListDataBind()
    {
        ListItem item = new ListItem();
        item.Text = "";
        item.Value = "";
        ddlPayWay.Items.Add(new ListItem("--选择--", ""));
        ddlPayWay.Items.Add(new ListItem("支付宝", "1"));
        ddlPayWay.Items.Add(new ListItem("快钱", "2"));
        ddlPayWay.Items.Add(new ListItem("汇付", "3"));
        ddlPayWay.Items.Add(new ListItem("财付通", "4"));
        ddlPayWay.Items.Add(new ListItem("账户支付", "14"));
        ddlPayWay.Items.Add(new ListItem("收银", "15"));
        IList<Bd_Base_Dictionary> DictionaryList = GetDictionaryList("9");
        for (int i = 0; i < DictionaryList.Count; i++)
        {
            if (DictionaryList[i].ChildName == "预订")
            {
                DictionaryList.Remove(DictionaryList[i]);
            }
        }
        ddlTicketState.DataSource = DictionaryList;
        ddlTicketState.DataBind();


        ddlPolicySource.DataSource = GetDictionaryList("24");
        ddlPolicySource.DataValueField = "ChildID";
        ddlPolicySource.DataTextField = "ChildName";
        ddlPolicySource.DataBind();
        item.Text = "不限";
        item.Value = "";
        ddlPolicySource.Items.Insert(0, item);


    }

    /// <summary>
    /// 查询数据绑定
    /// </summary>
    private void PageDataBind()
    {
        DataBase.Data.HashObject Hparams = new DataBase.Data.HashObject();
        Hparams.Add("CONDITION", Query());
        try
        {
            #region 3.机票信息明细 绑定

            Hparams.Add("TYPE", true);
            Hparams.Add("PAGE_COUNT", Curr);
            Hparams.Add("SHOWROW", AspNetPager1.PageSize);
            Hparams.Add("Export", false);
            DataTable[] dsTicketDetail = base.baseDataManage.MulExecProc("pro_TicketFZ", Hparams);

            int countTicketDetail = 0;
            DataTable dt = new DataTable();
            DataTable dtNew = new DataTable();

            if (dsTicketDetail != null && dsTicketDetail.Length > 0)
            {
                countTicketDetail = dsTicketDetail[1].Rows.Count;

                dt = dsTicketDetail[1];

                if (countTicketDetail > 1)
                {
                    lbtnDc3.Visible = true;
                }
                lal3.Visible = true;
                dtNew = dt.Copy();
                AspNetPager1.RecordCount = Convert.ToInt32(dsTicketDetail[0].Rows[0]["总行数"].ToString());

                AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;

                gvTicketDetail.DataSource = dt;
            }
            else
            {
                gvTicketDetail.DataSource = null;
            }
            gvTicketDetail.DataBind();

            #endregion
        }
        catch (Exception ex3)
        {
            gvTicketDetail.DataSource = null;
            gvTicketDetail.DataBind();
        }
    }
    #endregion
    /// <summary>
    /// 导出报表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnDc3_Click(object sender, EventArgs e)
    {
        #region
        DataBase.Data.HashObject Hpm = new DataBase.Data.HashObject();
            Hpm.Add("CONDITION", Query());
            Hpm.Add("TYPE", true);
            Hpm.Add("PAGE_COUNT", 1);
            Hpm.Add("SHOWROW", 999999);
            Hpm.Add("Export",true);
            DataTable[] dsTicketDetail = base.baseDataManage.MulExecProc("pro_TicketFZ", Hpm);

            int countTicketDetail = 0;
            DataTable dt = new DataTable();
            DataTable dtNew = new DataTable();

            if (dsTicketDetail != null && dsTicketDetail.Length > 0)
            {
                countTicketDetail = dsTicketDetail[0].Rows.Count;

                dt = dsTicketDetail[0];

                if (countTicketDetail > 1)
                {
                    lbtnDc3.Visible = true;
                }
                lal3.Visible = true;

              
                DataRow drnul = dt.NewRow();
                for (int i = 0; i < 50; i++)
                {
                    dt.Rows.Add(drnul[1]);
                }
                gvTicketDetailNew.DataSource = dt;
            }
            else
            {
                gvTicketDetailNew.DataSource = null;
            }
            gvTicketDetailNew.DataBind();
        #endregion

        Response.Clear();
        DownloadExcelFlag = true;
        ViewState["type"] = "3";
        Export("分账报表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
    }

    /// <summary>
    /// 分页（详情表）
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        PageDataBind();
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    private string Query()
    {
        StringBuilder sb = new StringBuilder();
        if (mCompany.RoleType == 4)
        {
            if (mCompany.UninCode.Length==18)
            {
                sb.Append(" _tpay.CpyNo like '" + mCompany.UninCode + "%' and len(_tpay.CpyNo)=24");
            }
            else
            {
                sb.Append(" _tpay.CpyNo like '" + mCompany.UninCode + "%' and len(_tpay.CpyNo)=30");
            }
        }
        else
        {
            sb.Append(" _tpay.CpyNo = '" + mCompany.UninCode + "'");
        }
        //支付时间
        if (txtPayTime1.Value.Trim() != "")
        {
            sb.Append(" and _order.PayTime >= convert(DateTime, '" + txtPayTime1.Value + " 00:00:00')");
        }//支付时间
        if (txtPayTime2.Value.Trim() != "")
        {
            sb.Append(" and _order.PayTime <= convert(DateTime, '" + txtPayTime2.Value + " 23:59:59')");
        }
        //出退费票时间
        if (cptimestart.Value.Trim() != "" && cptimeend.Value.Trim() != "")
        {
            sb.Append(" and _order.CPTime >= convert(DateTime, '" + cptimestart.Value + " 00:00:00') and _order.CPTime <= convert(DateTime, '" + cptimeend.Value + " 23:59:59')");
        }
        //创建时间
        if (txtCreateTime1.Value.Trim() != "")
        {
            sb.Append(" and _order.CreateTime >= convert(DateTime, '" + txtCreateTime1.Value + " 00:00:00')");
        }
        //创建时间
        if (txtCreateTime2.Value.Trim() != "")
        {
            sb.Append(" and _order.CreateTime <= convert(DateTime, '" + txtCreateTime2.Value + " 23:59:59')");
        }
        //政策类型
        if (rbtlOrderS.SelectedValue.ToString() != "0")
        {
            if (rbtlOrderS.SelectedValue.ToString() == "1")
            {
                sb.Append(" and _order.PolicyType = 1");
            }
            else
            {
                sb.Append(" and _order.PolicyType = 2");
            }
        }
        //支付方式
        if (ddlPayWay.SelectedValue != "")
        {
            sb.Append(" and _order.PayWay=" + ddlPayWay.SelectedValue);
        }
        //机票状态
        if (ddlTicketState.SelectedValue != "")
        {
            sb.Append(" and _passenger.TicketStatus = " + ddlTicketState.SelectedValue);
        }
        else
        {
            sb.Append(" and _passenger.TicketStatus in(2,3,4,5,6) ");
        }
        //城市对
        if (hiStart.Value.Trim() != "" && txtStart.Text != "中文/拼音")
        {
            sb.Append(" and TravelCode like '" + hiStart.Value.Trim() + "%'");
        }
        if (hiTarget.Value.Trim() != "" && txtTarget.Text != "中文/拼音")
        {
            sb.Append(" and TravelCode like '%" + hiTarget.Value.Trim() + "'");
        }
        //航空公司
        if (ddlCarrier.Value != "")
        {
            sb.Append(" and _order.CarryCode like '%" + CommonManage.TrimSQL(ddlCarrier.Value.Trim()) + "%'");
        }
        //订单号
        if (txtOrderId.Text.Trim() != "")
        {
            sb.Append(" and _order.Orderid like '%" + CommonManage.TrimSQL(txtOrderId.Text.Trim()) + "%'");
        }
        //编码
        if (txtPNR.Text.Trim() != "")
        {
            sb.Append(" and PNR like '%" + CommonManage.TrimSQL(txtPNR.Text.Trim()) + "%'");
        }
        //操作人
        if (txtCPUser.Text.Trim() != "")
        {
            sb.Append(" and CPName like '%" + CommonManage.TrimSQL(txtCPUser.Text.Trim()) + "%'");
        }
        //客户名称
        if (txtCustomer.Text.Trim() != "")
        {
            sb.Append(" and _cpy.UninAllName like '%" + CommonManage.TrimSQL(txtCustomer.Text.Trim()) + "%'");
        }
        //客户帐号
        if (txtLoginName.Text.Trim() != "")
        {
            sb.Append(" and _user.LoginName like '%" + CommonManage.TrimSQL(txtLoginName.Text.Trim()) + "%'");
        }
        //政策来源
        if (ddlPolicySource.SelectedValue.ToString() != "")
        {
            sb.Append(" and PolicySource in (" + ddlPolicySource.SelectedValue.ToString() + ")");

        }
      
        return sb.ToString();
    }

    #region 导出报表

    /// <summary>
    /// 导出
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
        gvTicketDetailNew.RenderControl(oHtmlTextWriter);
        string strValue = AddExcelHead() + oStringWriter.ToString() + AddExcelbottom();

        Response.Write(strValue);
        Response.End();
    }
    bool DownloadExcelFlag = false;

    public override void RenderControl(HtmlTextWriter writer)
    {
        string type = ViewState["type"].ToString();
        if (DownloadExcelFlag)
        {
                this.gvTicketDetailNew.RenderControl(writer);
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
    #region 查询和重置事件
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        //判读查询时间
        if (txtPayTime1.Value != "" && txtPayTime2.Value != "")
        {
            DateTime dt1 = DateTime.Parse(txtPayTime1.Value);

            DateTime dt2 = DateTime.Parse(txtPayTime2.Value);

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
        }
        else if (txtCreateTime1.Value != "" && txtCreateTime2.Value != "")
        {
            DateTime dt1 = DateTime.Parse(txtCreateTime1.Value);

            DateTime dt2 = DateTime.Parse(txtCreateTime2.Value);

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
        }

        else
        {
            if (cptimestart.Value != "" && cptimeend.Value != "")
            {
                DateTime dt1 = DateTime.Parse(cptimestart.Value);

                DateTime dt2 = DateTime.Parse(cptimeend.Value);
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
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('出票时间必填!');", true);
                return;
            }
        }
        lbtnDc3.Visible = false;
        Curr = 1;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.PageSize = int.Parse(selPageSize.Value);
        PageDataBind();
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtCPUser.Text = "";
        txtCustomer.Text = "";
        txtOrderId.Text = "";
        cptimestart.Value = DateTime.Now.ToString("yyyy-MM-dd");
        cptimeend.Value = DateTime.Now.ToString("yyyy-MM-dd");
        txtPNR.Text = "";
        txtStart.Text = "";
        txtTarget.Text = "";
        hiTarget.Value = "";
        hiStart.Value = "";
        rbtlOrderS.SelectedValue = "0";
        txtTicketNum.Text = "";
        ddlCarrier.Value = "0";
        ddlPayWay.SelectedIndex = 0;
        ddlPolicySource.SelectedIndex = 0;
        ddlTicketState.SelectedIndex = 0;
    }
    #endregion
    protected void gvTicketDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1 && e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[0].Text != "订单号" && !string.IsNullOrEmpty(e.Row.Cells[0].Text.ToString()))
            {
                e.Row.Cells[0].Attributes.Add("onclick", "OnClickgetUrl('../Order/OrderDetail.aspx?orderid=" + e.Row.Cells[0].Text + "&Url=../Bill/BillOfFz.aspx&currentuserid=" + this.mUser.id.ToString() + "')");
                e.Row.Cells[0].Attributes.Add("style", "cursor:hand;color:blue");
            }
        }
    }
    protected void gvTicketDetailNew_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
        {
            if (e.Row.Cells[0].Text != null && e.Row.Cells[0].Text != "")
            {
                e.Row.Cells[0].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            }
            if (e.Row.Cells[1].Text != null && e.Row.Cells[1].Text != "")
            {
                e.Row.Cells[1].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            }
        }
        else
        {
            if (e.Row.Cells[4].Text != null && e.Row.Cells[4].Text != "")
            {
                e.Row.Cells[4].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            }
            if (e.Row.Cells[5].Text != null && e.Row.Cells[5].Text != "")
            {
                //e.Row.Cells[4].Text = e.Row.Cells[4].Text.Trim().Replace("-", "");
                e.Row.Cells[5].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            }
        }
    }
}