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

public partial class Bill_BillLocalAndOut : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Curr = 1;
            ListDataBind();
            lbtnDc3.Visible = false;
            cptimestart.Value = DateTime.Now.ToString("yyyy-MM-dd");
            cptimeend.Value = DateTime.Now.ToString("yyyy-MM-dd");

            ViewState["BillType"] = Request["BillType"] == null ? "" : Request["BillType"].ToString();
            if (ViewState["BillType"].ToString() == "local")
            {
                lblShow.InnerText = "本地报表";
            }
            else
            {
                lblShow.InnerText = "外采报表";
            }
        }
    }
    protected int Curr
    {
        get { return (int)ViewState["currpage"]; }
        set { ViewState["currpage"] = value; }
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
            #region 信息明细 绑定

            bool type = cbType.Checked == true ? true : false;
            string showdf = cbshowdf.Checked == true ? "1" : "0";
            Hparams.Add("TYPE", type);
            Hparams.Add("PAGE_COUNT", Curr);
            Hparams.Add("SHOWROW", AspNetPager1.PageSize);
            Hparams.Add("Export", false);
            Hparams.Add("ShowOutPay", showdf);
            if (mCompany.RoleType != 4 && mCompany.RoleType != 5)//分销报表无此参数
            {
                Hparams.Add("LoginCpyNo", mCompany.UninCode);
            }
            DataTable[] dsTicketDetail = base.baseDataManage.MulExecProc("pro_TicketDetail", Hparams);

            int countTicketDetail = 0;
            DataTable dt = new DataTable();
            DataTable dtNew = new DataTable();

            if (dsTicketDetail != null && dsTicketDetail.Length > 0)
            {
                countTicketDetail = dsTicketDetail[1].Rows.Count;

                dt = dsTicketDetail[1];

                string orderid = "";
                decimal dfCountPrice = 0;
                decimal cjPrice = 0;
                decimal tfqPrice = 0;
                decimal PayMoney = 0, RealPayMoney = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++)
                {

                    if (dt.Rows[i]["订单号"].ToString() == orderid)
                    {
                        dt.Rows[i]["代付金额"] = "0";
                        dt.Rows[i]["差价"] = "0";
                        if (!string.IsNullOrEmpty(dt.Rows[i]["机票状态"].ToString()) && dt.Rows[i]["机票状态"].ToString() == "改签")
                        {
                            dt.Rows[i]["退废改手续费"] = "0";
                            dt.Rows[i]["公司应收"] = "0";
                            dt.Rows[i]["公司实收"] = "0";
                        }

                    }
                    else
                    {
                        orderid = dt.Rows[i]["订单号"].ToString();
                    }

                    dfCountPrice += decimal.Parse(dt.Rows[i]["代付金额"].ToString());
                    cjPrice += decimal.Parse(dt.Rows[i]["差价"].ToString());
                    tfqPrice += decimal.Parse(dt.Rows[i]["退废改手续费"].ToString());
                    PayMoney += decimal.Parse(dt.Rows[i]["公司应收"].ToString());
                    RealPayMoney += decimal.Parse(dt.Rows[i]["公司实收"].ToString());
                }

                dt.Rows[dt.Rows.Count - 1]["代付金额"] = dfCountPrice;
                dt.Rows[dt.Rows.Count - 1]["差价"] = cjPrice;
                dt.Rows[dt.Rows.Count - 1]["退废改手续费"] = tfqPrice;
                dt.Rows[dt.Rows.Count - 1]["公司应收"] = PayMoney;
                dt.Rows[dt.Rows.Count - 1]["公司实收"] = RealPayMoney;

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
        ddlCarrier.Value = "0";
        ddlPayWay.SelectedIndex = 0;
        ddlTicketState.SelectedIndex = 0;
    }
    #endregion
    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    private string Query()
    {
        StringBuilder sb = new StringBuilder();

        if (mCompany.RoleType == 1)
        {
            sb.Append(" 1=1");
        }
        else if (mCompany.RoleType == 2)
        {
            sb.Append(" (_order.OwnerCpyNo like '" + mCompany.UninCode + "%' or _order.CPCpyNo like '" + mCompany.UninCode + "%')");
        }
        else
        {
            sb.Append(" (_order.OwnerCpyNo = '" + mCompany.UninCode + "' or _order.CPCpyNo = '" + mCompany.UninCode + "')");
        }
        //政策来源
        if (ViewState["BillType"].ToString() == "local")
        {
            sb.Append(" and (_order.PolicySource in (1,2) or (_order.PolicySource=9 and _order.CPCpyNo='" + mCompany.UninCode + "'))");
        }
        else
        {
            sb.Append(" and (_order.PolicySource in (3,4,5,6,7,8,10) or (_order.PolicySource=9 and _order.CPCpyNo <> '" + mCompany.UninCode + "'))");
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
        //是否做过退费
        if (showtf.Checked == false)
        {
            if (ddlTicketState.SelectedValue == "2")
            {
                sb.Append(" and _passenger.IsBack != 'true'");
            }
        }
        //支付方式
        if (ddlPayWay.SelectedValue != "")
        {
            sb.Append(" and _order.PayWay=" + ddlPayWay.SelectedValue);
        }
        //订单状态
        if (ddlTicketState.SelectedValue != "")
        {
            sb.Append(" and OrderStatusCode = " + ddlTicketState.SelectedValue);
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
            sb.Append(" and _order.CarryCode like '%" + ddlCarrier.Value + "%'");
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

        return sb.ToString();
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
    #region 导出
    protected void lbtnDc3_Click(object sender, EventArgs e)
    {
        #region
        DataBase.Data.HashObject Hpm = new DataBase.Data.HashObject();
        bool type = cbType.Checked == true ? true : false;
        string showdf = cbshowdf.Checked == true ? "1" : "0";
        Hpm.Add("CONDITION", Query());
        Hpm.Add("TYPE", type);
        Hpm.Add("PAGE_COUNT", 1);
        Hpm.Add("SHOWROW", 999999);
        Hpm.Add("Export", true);
        Hpm.Add("ShowOutPay", showdf);
        if (mCompany.RoleType != 4 && mCompany.RoleType != 5)//分销报表无此参数
        {
            Hpm.Add("LoginCpyNo", mCompany.UninCode);
        }
        DataTable[] dsTicketDetail = base.baseDataManage.MulExecProc("pro_TicketDetail", Hpm);

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
                string orderid = "";
                decimal dfCountPrice = 0;
                decimal cjPrice = 0;
                decimal tfqPrice = 0;
                decimal PayMoney = 0, RealPayMoney = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++)
                {

                    if (dt.Rows[i]["订单号"].ToString() == orderid)
                    {
                        dt.Rows[i]["代付金额"] = "0";
                        dt.Rows[i]["差价"] = "0";
                        if (!string.IsNullOrEmpty(dt.Rows[i]["机票状态"].ToString()) && dt.Rows[i]["机票状态"].ToString() == "改签")
                        {
                            dt.Rows[i]["退废改手续费"] = "0";
                            dt.Rows[i]["公司应收"] = "0";
                            dt.Rows[i]["公司实收"] = "0";
                        }

                    }
                    else
                    {
                        orderid = dt.Rows[i]["订单号"].ToString();
                    }

                    dfCountPrice += decimal.Parse(dt.Rows[i]["代付金额"].ToString());
                    cjPrice += decimal.Parse(dt.Rows[i]["差价"].ToString());
                    tfqPrice += decimal.Parse(dt.Rows[i]["退废改手续费"].ToString());
                    PayMoney += decimal.Parse(dt.Rows[i]["公司应收"].ToString());
                    RealPayMoney += decimal.Parse(dt.Rows[i]["公司实收"].ToString());
                }
                dt.Rows[dt.Rows.Count - 1]["代付金额"] = dfCountPrice;
                dt.Rows[dt.Rows.Count - 1]["差价"] = cjPrice;
                dt.Rows[dt.Rows.Count - 1]["退废改手续费"] = tfqPrice;
                dt.Rows[dt.Rows.Count - 1]["公司应收"] = PayMoney;
                dt.Rows[dt.Rows.Count - 1]["公司实收"] = RealPayMoney;

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
        Export("机票明细报表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
    }
    #endregion
    #region RowDataBound事件

    /// <summary>
    /// 报表详细
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// 
    protected void gvTicketDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.Cells[30].Text == "差价")
        {
            e.Row.Cells[30].Text = "利润(差价)";
        }
        if (e.Row.RowIndex != -1 && e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[5].Text != "订单号" && !string.IsNullOrEmpty(e.Row.Cells[5].Text) && e.Row.Cells[5].Text != "&nbsp;")
            {
                e.Row.Cells[5].Attributes.Add("onclick", "OnClickgetUrl('../Order/OrderDetail.aspx?orderid=" + e.Row.Cells[5].Text + "&Url=../Bill/BillOfCount.aspx&currentuserid=" + this.mUser.id.ToString() + "')");
                e.Row.Cells[5].Attributes.Add("style", "cursor:hand;color:blue");
            }

        }
    }
    protected void gvTicketDetailNew_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.Cells[30].Text == "差价")
        {
            e.Row.Cells[30].Text = "利润(差价)";
        }
        if (e.Row.Cells[5].Text != null && e.Row.Cells[5].Text != "")
        {
            e.Row.Cells[5].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
        }
        if (e.Row.Cells[6].Text != null && e.Row.Cells[6].Text != "")
        {
            e.Row.Cells[6].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
        }
        if (e.Row.Cells[7].Text != null && e.Row.Cells[7].Text != "")
        {
            e.Row.Cells[7].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
        }
        if (e.Row.Cells[33].Text != null && e.Row.Cells[33].Text != "")
        {
            e.Row.Cells[33].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
        }
        if (e.Row.Cells[36].Text != null && e.Row.Cells[36].Text != "")
        {
            e.Row.Cells[36].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
        }
    }
    #endregion

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

}