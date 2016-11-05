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

public partial class Bill_BillOfCount : BasePage
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

                string qx = BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao;
                if (!qx.Contains("|9|"))
                {
                    thpolicy.Visible = false;
                    tdpolicy.Visible = false;
                }
                showtryy.Visible = false;
                cpuserth.Visible = false;
                cpusertd.Visible = false;
                cbshowdf.Visible = false;
                divTicketInfo.Visible = false;
                divTicketSell.Visible = false;
                moreSearchOrderA.Visible = false;
                ViewState["pro1"] = "pro_TicketInfoCollectFX";
                ViewState["pro2"] = "pro_TicketSalesCollectFX";
                ViewState["pro3"] = "pro_TicketDetailFX";
                ViewState["qx"] = qx;
            }
            else
            {
                ViewState["pro1"] = "pro_TicketInfoCollect";
                ViewState["pro2"] = "pro_TicketSalesCollect";
                ViewState["pro3"] = "pro_TicketDetail";

            }
            ViewState["type"] = "";
            Curr = 1;
            ListDataBind();
            if (Request["cpyname"]!=null && Request["begintime"]!=null && Request["endtime"]!=null)
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
            this.hidcolspancount.Value = mCompany.RoleType == 4 ? "17" : "25";
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
        if (mCompany.RoleType != 4 && mCompany.RoleType != 5)//分销不用显示汇总和统计
        {
            try
            {

                #region 1.机票信息汇总

                //1.机票信息汇总
                DataTable[] dsTicketInfoCollect = base.baseDataManage.MulExecProc(ViewState["pro1"].ToString(), Hparams);

                int countTicketDetail = 0;

                if (dsTicketInfoCollect != null && dsTicketInfoCollect.Length > 0)
                {
                    countTicketDetail = dsTicketInfoCollect[0].Rows.Count;
                }
                if (countTicketDetail > 1)
                {
                    lbtnDc1.Visible = true;
                }
                lal1.Visible = true;

                gvTicketInfoCollect.DataSource = dsTicketInfoCollect[0];
                gvTicketInfoCollect.DataBind();

                DataTable dt = dsTicketInfoCollect[0];
                DataRow drnul = dt.NewRow();
                for (int i = 0; i < 50; i++)
                {
                    dt.Rows.Add(drnul[1]);
                }
                gvTicketInfoCollectNew.DataSource = dt;
                gvTicketInfoCollectNew.DataBind();
                #endregion
            }
            catch (Exception ex1)
            {
                gvTicketInfoCollect.DataSource = null;
                gvTicketInfoCollect.DataBind();
            }

            try
            {
                #region 2.机票销售统计

                //2.机票销售统计
                //string sql2 = "exec dbo.pro_TicketSalesCollect '" + Query() + "'";


                DataTable[] dsTicketSellCount = base.baseDataManage.MulExecProc(ViewState["pro2"].ToString(), Hparams);

                int countTicketSellCount = 0;

                if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
                {
                    dsTicketSellCount[0].Columns.Remove("出票地");
                }
                if (dsTicketSellCount != null && dsTicketSellCount.Length > 0)
                {
                    countTicketSellCount = dsTicketSellCount[0].Rows.Count;

                    if (dsTicketSellCount.Length > 1)
                    {
                        ViewState["dt"] = dsTicketSellCount[1];

                        #region 用于下载报表

                        DataTable dtNewS = dsTicketSellCount[0].Clone();

                        DataView dvb2b = dsTicketSellCount[1].DefaultView;
                        dvb2b.RowFilter = "出票地='本地B2B'";
                        DataTable dtb2b = dvb2b.ToTable();
                        dtb2b.Columns.Remove("出票地");
                        dtb2b.Columns[0].ColumnName = "出票地";

                        DataView dvbsp = dsTicketSellCount[1].DefaultView;
                        dvbsp.RowFilter = "出票地='本地BSP'";
                        DataTable dtbsp = dvbsp.ToTable();
                        dtbsp.Columns.Remove("出票地");
                        dtbsp.Columns[0].ColumnName = "出票地";

                        DataView dv517 = dsTicketSellCount[1].DefaultView;
                        dv517.RowFilter = "出票地='517'";
                        DataTable dt517 = dv517.ToTable();
                        dt517.Columns.Remove("出票地");
                        dt517.Columns[0].ColumnName = "出票地";

                        DataView dv51book = dsTicketSellCount[1].DefaultView;
                        dv51book.RowFilter = "出票地='51book'";
                        DataTable dt51book = dv51book.ToTable();
                        dt51book.Columns.Remove("出票地");
                        dt51book.Columns[0].ColumnName = "出票地";


                        DataView dvbt = dsTicketSellCount[1].DefaultView;
                        dvbt.RowFilter = "出票地='百拓'";
                        DataTable dtbt = dvbt.ToTable();
                        dtbt.Columns.Remove("出票地");
                        dtbt.Columns[0].ColumnName = "出票地";

                        DataView dvpm = dsTicketSellCount[1].DefaultView;
                        dvpm.RowFilter = "出票地='票盟'";
                        DataTable dtpm = dvpm.ToTable();
                        dtpm.Columns.Remove("出票地");
                        dtpm.Columns[0].ColumnName = "出票地";

                        DataView dvjr = dsTicketSellCount[1].DefaultView;
                        dvjr.RowFilter = "出票地='今日'";
                        DataTable dtjr = dvjr.ToTable();
                        dtjr.Columns.Remove("出票地");
                        dtjr.Columns[0].ColumnName = "出票地";

                        DataView dvgx = dsTicketSellCount[1].DefaultView;
                        dvgx.RowFilter = "出票地='共享'";
                        DataTable dtgx = dvgx.ToTable();
                        dtgx.Columns.Remove("出票地");
                        dtgx.Columns[0].ColumnName = "出票地";

                        DataView dv8qy = dsTicketSellCount[1].DefaultView;
                        dv8qy.RowFilter = "出票地='8000翼'";
                        DataTable dt8qy = dv8qy.ToTable();
                        dt8qy.Columns.Remove("出票地");
                        dt8qy.Columns[0].ColumnName = "出票地";

                        DataView dv4k = dsTicketSellCount[1].DefaultView;
                        dv4k.RowFilter = "出票地='4K商旅'";
                        DataTable dt4k = dv4k.ToTable();
                        dt4k.Columns.Remove("出票地");
                        dt4k.Columns[0].ColumnName = "出票地";

                        string type = "";
                        for (int i = 0; i < countTicketSellCount; i++)
                        {
                            type = dsTicketSellCount[0].Rows[i][0].ToString();

                            dtNewS.ImportRow(dsTicketSellCount[0].Rows[i]);

                            if (type == "本地B2B")
                            {
                                for (int j = 0; j < dtb2b.Rows.Count; j++)
                                {
                                    dtNewS.ImportRow(dtb2b.Rows[j]);
                                }
                            }
                            else if (type == "本地BSP")
                            {
                                for (int j = 0; j < dtbsp.Rows.Count; j++)
                                {
                                    dtNewS.ImportRow(dtbsp.Rows[j]);
                                }
                            }
                            else if (type == "517")
                            {
                                for (int j = 0; j < dt517.Rows.Count; j++)
                                {
                                    dtNewS.ImportRow(dt517.Rows[j]);
                                }
                            }
                            else if (type == "51book")
                            {
                                for (int j = 0; j < dt51book.Rows.Count; j++)
                                {
                                    dtNewS.ImportRow(dt51book.Rows[j]);
                                }
                            }
                            else if (type == "百拓")
                            {
                                for (int j = 0; j < dtbt.Rows.Count; j++)
                                {
                                    dtNewS.ImportRow(dtbt.Rows[j]);
                                }
                            }
                            else if (type == "票盟")
                            {
                                for (int j = 0; j < dtpm.Rows.Count; j++)
                                {
                                    dtNewS.ImportRow(dtpm.Rows[j]);
                                }
                            }
                            else if (type == "今日")
                            {
                                for (int j = 0; j < dtjr.Rows.Count; j++)
                                {
                                    dtNewS.ImportRow(dtjr.Rows[j]);
                                }
                            }
                            else if (type == "共享")
                            {
                                for (int j = 0; j < dtgx.Rows.Count; j++)
                                {
                                    dtNewS.ImportRow(dtgx.Rows[j]);
                                }
                            }
                            else if (type == "8000翼")
                            {
                                for (int j = 0; j < dt8qy.Rows.Count; j++)
                                {
                                    dtNewS.ImportRow(dt8qy.Rows[j]);
                                }
                            }
                            else if (type == "4K商旅")
                            {
                                for (int j = 0; j < dt4k.Rows.Count; j++)
                                {
                                    dtNewS.ImportRow(dt4k.Rows[j]);
                                }
                            }
                        }
                        DataRow drnul = dtNewS.NewRow();
                        for (int i = 0; i < 50; i++)
                        {
                            dtNewS.Rows.Add(drnul[1]);
                        }
                        gvTicketSellCountNew.DataSource = dtNewS;
                        gvTicketSellCountNew.DataBind();
                        for (int i = 0; i < dtNewS.Rows.Count; i++)
                        {
                            string titles = dtNewS.Rows[i][0].ToString();
                            if (titles == "本地B2B" || titles == "本地BSP" || titles == "517" || titles == "51book" || titles == "百拓" || titles == "票盟" || titles == "今日" || titles == "共享" || titles == "8000翼" || titles == "4K商旅")
                            {
                                gvTicketSellCountNew.Rows[i].Attributes.Add("Style", "color:Red");
                            }
                        }
                        gvTicketSellCountNew.Rows[gvTicketSellCountNew.Rows.Count - 1].Attributes.Add("Style", "color:Red");
                        #endregion
                    }
                }
                if (countTicketSellCount > 1)
                {
                    lbtnDc2.Visible = true;
                }

                lal2.Visible = true;
                gvTicketSellCount.DataSource = dsTicketSellCount[0];
                gvTicketSellCount.DataBind();


                //设置 行 的颜色 （最后一行 合计显示红色）
                if (gvTicketSellCount.Rows.Count > 1)
                {
                    gvTicketSellCount.Rows[gvTicketSellCount.Rows.Count - 1].Attributes.Add("class", "tds");
                }

                #endregion
            }
            catch (Exception ex2)
            {
                gvTicketSellCount.DataSource = null;
                gvTicketSellCount.DataBind();
            }
        }
        try
        {
            #region 3.机票信息明细 绑定

            bool type = cbType.Checked == true ? true : false;
            string showdf = cbshowdf.Checked == true ? "1" : "0";
            Hparams.Add("TYPE", type);
            Hparams.Add("PAGE_COUNT", Curr);
            Hparams.Add("SHOWROW", AspNetPager1.PageSize);
            Hparams.Add("Export",false);
            Hparams.Add("ShowOutPay", showdf);
            if (mCompany.RoleType != 4 && mCompany.RoleType != 5)//分销报表无此参数
            {
                Hparams.Add("LoginCpyNo", mCompany.UninCode);
            }
            DataTable[] dsTicketDetail = base.baseDataManage.MulExecProc(ViewState["pro3"].ToString(), Hparams);
            int countTicketDetail = 0;
            DataTable dt = new DataTable();
            DataTable dtNew = new DataTable();

            if (dsTicketDetail != null && dsTicketDetail.Length > 0)
            {
                countTicketDetail = dsTicketDetail[1].Rows.Count;

                dt = dsTicketDetail[1];
                if (mCompany.RoleType != 4 && mCompany.RoleType != 5)//运营
                {
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
                else
                {
                    string qx = ViewState["qx"].ToString();
                    if (!qx.Contains("|9|"))
                    {
                        if (dt.Columns[2].ColumnName == "政策来源")
                        {
                            dt.Columns.Remove("政策来源");
                        }
                    }
                }
                if (countTicketDetail > 1)
                {
                    lbtnDc3.Visible = true;
                }
                lal3.Visible = true;
                //根据勾选显示

                bool result = false;
                for (int i = 0; i < cblist.Items.Count; i++)
                {
                    if (cblist.Items[i].Selected == true)
                    {
                        result = true;
                        break;
                    }
                }
                if (result == true)
                {
                    for (int i = 0; i < cblist.Items.Count; i++)
                    {
                        if (cblist.Items[i].Selected == true)
                        {

                        }
                        else
                        {
                            dt.Columns.Remove(cblist.Items[i].Value); //隐藏
                        }
                    }
                }


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

    #region RowDataBound事件
    /// <summary>
    /// 信息汇总
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvTicketInfoCollect_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowIndex != -1 && e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[9].Text != null && e.Row.Cells[9].Text != "")
                {
                    decimal strValue = decimal.Parse(e.Row.Cells[9].Text.Trim().ToString());
                    e.Row.Cells[9].Text = Math.Round(strValue + 0.0000001M, 2).ToString();
                }
            }
        }
        catch (Exception ex)
        {
        }
    }
    /// <summary>
    /// 销售统计
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvTicketSellCount_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (mCompany.RoleType!=4 && mCompany.RoleType!=5)
            {
                if (e.Row.RowIndex != -1 && e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (!string.IsNullOrEmpty(e.Row.Cells[0].Text) && !e.Row.Cells[0].Text.Contains("合计"))
                    {
                        string str = StrValue(e.Row.Cells[0].Text);
                        string id = "a" + e.Row.RowIndex;
                        e.Row.Cells[0].Text = "<a id='" + id + "' onclick=\"OpenOrclose(escape('" + str + "')," + e.Row.RowIndex + ",event)\">田</a>&nbsp;" + e.Row.Cells[0].Text;//
                    }
                }
            }
            
        }
        catch (Exception ex)
        {
        }
    }
    /// <summary>
    /// 机票详情
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvTicketDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
       
        if (e.Row.RowIndex!=-1&&e.Row.RowType == DataControlRowType.DataRow)
        {

            if (mCompany.RoleType == 4 || mCompany.RoleType == 5)//分销权限
            {
                if (e.Row.Cells[0].Text != "订单号" && !e.Row.Cells[0].Text.Contains("合计") && !string.IsNullOrEmpty(e.Row.Cells[0].Text.ToString()))
                {
                    e.Row.Cells[0].Attributes.Add("onclick", "OnClickgetUrl('../Order/OrderDetail.aspx?orderid=" + e.Row.Cells[0].Text + "&Url=../Bill/BillOfCount.aspx&currentuserid=" + this.mUser.id.ToString() + "')");
                    e.Row.Cells[0].Attributes.Add("style", "cursor:hand;color:blue");
                }
            }
            else
            {
                 int cellcount = e.Row.Cells.Count;
                 if (cellcount == 45)
                 {
                     if (e.Row.Cells[32].Text == "差价")
                     {
                         e.Row.Cells[32].Text = "利润(差价)";
                     }
                     if (e.Row.Cells[5].Text != "订单号" && !string.IsNullOrEmpty(e.Row.Cells[5].Text) && e.Row.Cells[5].Text != "&nbsp;")
                     {
                         e.Row.Cells[5].Attributes.Add("onclick", "OnClickgetUrl('../Order/OrderDetail.aspx?orderid=" + e.Row.Cells[5].Text + "&Url=../Bill/BillOfCount.aspx&currentuserid=" + this.mUser.id.ToString() + "')");
                         e.Row.Cells[5].Attributes.Add("style", "cursor:hand;color:blue");
                     }
                 }
            }
            
        }
        
    }
    /// <summary>
    /// 报表详细
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //protected void gvTicketDetailNew_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
    //    {
    //        if (e.Row.Cells[0].Text != null && e.Row.Cells[0].Text != "")
    //        {
    //            e.Row.Cells[0].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
    //        }
    //        if (e.Row.Cells[1].Text != null && e.Row.Cells[1].Text != "")
    //        {
    //            e.Row.Cells[1].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
    //        }
           
    //    }
    //    else
    //    {
    //        if (e.Row.Cells[30].Text == "差价")
    //        {
    //            e.Row.Cells[30].Text = "利润(差价)";
    //        }
    //        if (e.Row.Cells[5].Text != null && e.Row.Cells[5].Text != "")
    //        {
    //            e.Row.Cells[5].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
    //        }
    //        if (e.Row.Cells[6].Text != null && e.Row.Cells[6].Text != "")
    //        {
    //            e.Row.Cells[6].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
    //        }
    //        if (e.Row.Cells[7].Text != null && e.Row.Cells[7].Text != "")
    //        {
    //            e.Row.Cells[7].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
    //        }
    //        if (e.Row.Cells[33].Text != null && e.Row.Cells[33].Text != "")
    //        {
    //            e.Row.Cells[33].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
    //        }
    //        if (e.Row.Cells[36].Text != null && e.Row.Cells[36].Text != "")
    //        {
    //            e.Row.Cells[36].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
    //        }
    //    }
    //}
    #endregion

    #region 查询和重置事件
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        //判读查询时间
        string msg = "";
        DateTime dt1;
        DateTime dt2;
        if (txtPayTime1.Value != "" && txtPayTime2.Value != "")
        {
             dt1 = DateTime.Parse(txtPayTime1.Value);
             dt2 = DateTime.Parse(txtPayTime2.Value);
        }
        else if (txtCreateTime1.Value != "" && txtCreateTime2.Value != "")
        {
             dt1 = DateTime.Parse(txtCreateTime1.Value);
             dt2 = DateTime.Parse(txtCreateTime2.Value);
        }
        else
        {
            if (cptimestart.Value != "" && cptimeend.Value != "")
            {
                 dt1 = DateTime.Parse(cptimestart.Value);
                 dt2 = DateTime.Parse(cptimeend.Value);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('出票时间必填!');", true);
                return;
            }
        }
        //if (DateTime.Now >= DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 09:00:00")) && DateTime.Now <= DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 18:00:00")))
        //{
        //    if (dt2 >= dt1 && dt2.AddDays(-7) >= dt1)
        //    {
        //        msg = "请查询1周以内的数据!";
        //    }
        //    else if (dt1 > dt2)
        //    {
        //        msg = "请选择正确的查询时间";
        //    }
        //}
        //else
        //{
            if (dt2 >= dt1 && dt2.AddMonths(-1) >= dt1)
            {
                msg = "请查询1个月以内的数据!";
            }
            else if (dt1 > dt2)
            {
                msg = "请选择正确的查询时间";
            }
        //}
        if (!string.IsNullOrEmpty(msg))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('"+msg+"');", true);
        }
        else
        {
            lbtnDc3.Visible = false;
            Curr = 1;
            AspNetPager1.CurrentPageIndex = Curr;
            AspNetPager1.PageSize = int.Parse(selPageSize.Value);

            PageDataBind();
        }
       
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
        ddlCarrier.Value = "";
        ddlPayWay.SelectedIndex = 0;
        ddlPolicySource.SelectedIndex = 0;
        ddlTicketState.SelectedIndex = 0;
        for (int i = 0; i < cblist.Items.Count; i++)
        {
            cblist.Items[i].Selected = false;
        }
    }
    #endregion

    #region 导出Excl按钮事件
    protected void lbtnDc1_Click(object sender, EventArgs e)
    {
        Response.Clear();
        DownloadExcelFlag = true;
        ViewState["type"] = "1";
        Export("机票信息汇总报表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
    }
    protected void lbtnDc2_Click(object sender, EventArgs e)
    {
        Response.Clear();
        DownloadExcelFlag = true;
        ViewState["type"] = "2";
        Export("机票销售统计报表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
    }
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
            Hpm.Add("Export",true);
            Hpm.Add("ShowOutPay", showdf);
            if (mCompany.RoleType != 4 && mCompany.RoleType != 5)//分销报表无此参数
            {
                Hpm.Add("LoginCpyNo", mCompany.UninCode);
            }
            DataTable[] dsTicketDetail = base.baseDataManage.MulExecProc(ViewState["pro3"].ToString(), Hpm);

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
                    if (mCompany.RoleType != 4 && mCompany.RoleType != 5)//运营
                    {
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
                      
                        //根据勾选显示

                        bool result = false;
                        for (int i = 0; i < cblist.Items.Count; i++)
                        {
                            if (cblist.Items[i].Selected == true)
                            {
                                result = true;
                                break;
                            }
                        }
                        if (result == true)
                        {
                            for (int i = 0; i < cblist.Items.Count; i++)
                            {
                                if (cblist.Items[i].Selected == true)
                                {

                                }
                                else
                                {
                                    dt.Columns.Remove(cblist.Items[i].Value); //隐藏
                                }
                            }
                        }
                        else
                        {
                            if (dt.Columns[32].ColumnName == "差价")
                            {
                                dt.Columns[32].ColumnName = "利润(差价)";
                            }
                        }
                    }
                    else
                    {
                        string qx  = ViewState["qx"].ToString();
                        if (!qx.Contains("|9|"))
                        {
                            if (dt.Columns[2].ColumnName == "政策来源")
                            {
                                dt.Columns.Remove("政策来源");
                            }
                        }
                    }
                }
                lal3.Visible = true;
            }
           
        #endregion
        //Response.Clear();
        //DownloadExcelFlag = true;
        //ViewState["type"] = "3";
        //Export("机票明细报表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
        ExcelRender.RenderToExcel(dt, Context, "机票明细报表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".xls");
    }
    #endregion

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
     
        if (mCompany.RoleType == 1)
        {
            sb.Append(" 1=1");
        }
        else if (mCompany.RoleType == 2)
        {
            sb.Append(" (_order.OwnerCpyNo like '" + mCompany.UninCode + "%' or _order.CPCpyNo like '"+mCompany.UninCode+"%')");
        }
        else
        {
            sb.Append(" (_order.OwnerCpyNo = '" + mCompany.UninCode + "' or _order.CPCpyNo = '" + mCompany.UninCode + "')");
        }
        //是否做过退费
        if (showtf.Checked == false)
        {
            if (ddlTicketState.SelectedValue == "2")
            {
                sb.Append(" and _passenger.IsBack != 'true'");
            }
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
            if (ddlPolicySource.SelectedValue == "-1")
            {
                sb.Append(" and PolicySource in (1,2)");
            }
            else if (ddlPolicySource.SelectedValue == "-2")
            {
                sb.Append(" and PolicySource not in (1,2)");
            }
            else
            {
                sb.Append(" and PolicySource in (" + ddlPolicySource.SelectedValue.ToString() + ")");
            }
            

        }
        
        return sb.ToString();
    }

    /// <summary>
    /// 字符串
    /// </summary>
    /// <returns></returns>
    protected string StrValue(string val)
    {
        string msg = "";
        bool rs = false;
        try
        {
            if (ViewState["dt"] != null && val != null && val != "")
            {
                DataTable dt = (DataTable)ViewState["dt"];
                msg += "<table width=100% class=table2>";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString().ToUpper() == val.ToUpper())
                    {
                        msg += "<tr>";
                        for (int j = 1; j < dt.Columns.Count; j++)
                        {
                            msg += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                            rs = true;
                        }
                        msg += "</tr>";
                    }
                }
                msg += "</table>";
            }
            else
            {
            }
        }
        catch (Exception ex)
        {
        }
        if (rs == false)
        {
            msg = "";
        }
        return msg;
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

        string type = ViewState["type"].ToString();
        if (type == "1")
        {
            gvTicketInfoCollectNew.RenderControl(oHtmlTextWriter);
        }
        else if (type == "2")
        {
            gvTicketSellCountNew.RenderControl(oHtmlTextWriter);
        }
        else if (type == "3")
        {
            gvTicketDetailNew.RenderControl(oHtmlTextWriter);
        }
        else
        {

        }
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
            if (type == "1")
            {
                this.gvTicketInfoCollectNew.RenderControl(writer);
            }
            else if (type == "2")
            {
                this.gvTicketSellCountNew.RenderControl(writer);
            }
            else if (type == "3")
            {
                this.gvTicketDetailNew.RenderControl(writer);
            }
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