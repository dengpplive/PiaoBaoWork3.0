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

public partial class Financial_PaymentRecord : BasePage
{
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
    public string paytype = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtGoAlongTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            txtGoAlongTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");
            ViewState["paytype"] = Request["paytype"] == null ? "" : Request["paytype"].ToString();
            ViewState["cpyname"] = Request["cpyname"] == null ? "" : Server.UrlDecode(Request["cpyname"].ToString());
            ViewState["payUnincode"] = Request["unincode"] == null ? "" : Request["unincode"].ToString();
            paytype = ViewState["paytype"].ToString();
            if (ViewState["cpyname"].ToString().Length != 0)
            {
                this.txtUninAllNAME.Text = ViewState["cpyname"].ToString();
                this.txtUninAllNAME.Enabled = false;
            }
            Curr = 1;
            Query();
            AspNetPager1.PageSize = 20;
            switch (ViewState["paytype"].ToString())
            {
                case "zh":
                    this.spantitle.InnerText = "账户余额流水账";
                    break;
                case "zx":
                    this.spantitle.InnerText = "在线交易流水账";
                    break;
                case "qkxz":
                    this.spantitle.InnerText = "欠款销账记录";
                    break;
                case "pos":
                    this.spantitle.InnerText = "pos机流水账";
                    this.ddlpaytype.Visible = false;
                    this.txtUninAllNAME.Enabled = true;
                    div1.Visible = false;
                    div2.Visible = false;
                    OperReasonth.Visible = false;
                    break;
            }
        }
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    protected void Query()
    {
        StringBuilder strWhere = new StringBuilder("1=1");
        switch (ViewState["paytype"].ToString())
        {
            case "zh":
                strWhere.Append(" and (PayType in (13,14) or (PayType not in (13,14) and (Remark like '%充值%' or OperReason like '%充值%'))) and Remark not like '%欠款明细记录%' and OperReason not like '%欠款明细记录%'");
                break;
            case "zx":
                strWhere.Append(" and PayType not in (13,14,15,20,21)");
                break;
            case "qkxz":
                strWhere.Append(" and PayType = 21");
                break;
            case "pos":
                strWhere.Append(" and PayType in (9,10,11,12,13)");
                break;
            case "all":
                strWhere.Append(" and Remark not like '%欠款明细记录%' and OperReason not like '%欠款明细记录%'");
                break;
        }
        if (ddlpaytype.SelectedValue!="-1")
        {
            strWhere.Append(" and childname = '" + ddlpaytype.SelectedItem.Text + "'");
        }
        if (mCompany.RoleType != 1 && mCompany.RoleType != 2)
        {
            strWhere.Append(" and (PayCpyNo = '" + mCompany.UninCode + "' or RecCpyNo= '" + mCompany.UninCode + "')");
        }
        else if (mCompany.RoleType == 2)
        {
            strWhere.Append(" and ((PayCpyNo like '" + mCompany.UninCode + "%' and len(PayCpyNo)<>12) or (RecCpyNo like '" + mCompany.UninCode + "%' and len(RecCpyNo)<>12))");
        }    
        if (txtGoAlongTime1.Value != "")
        {
            strWhere.Append(" and OperTime >= convert(DateTime, '" + txtGoAlongTime1.Value + "')");
        }
        if (txtGoAlongTime2.Value != "")
        { 
            strWhere.Append(" and OperTime <= convert(DateTime, '" + txtGoAlongTime2.Value + " 23:59:59')");
        }
        if (txtUninAllNAME.Text!="")
        {
            if (ViewState["cpyname"].ToString().Length != 0)
            {
                strWhere.Append(" and (PayCpyName = '" + CommonManage.TrimSQL(txtUninAllNAME.Text.Trim()) + "' or RecCpyName = '" + CommonManage.TrimSQL(txtUninAllNAME.Text.Trim()) + "')");
            }
            else
            {
                strWhere.Append(" and (PayCpyName like '%" + CommonManage.TrimSQL(txtUninAllNAME.Text.Trim()) + "%' or RecCpyName like '%" + CommonManage.TrimSQL(txtUninAllNAME.Text.Trim()) + "%')");
            }
            
        }
       
        Con = strWhere.ToString();
        Curr = 1;

    }
    /// <summary>
    /// 绑定
    /// </summary>
    protected void BindV_User_Company_List()
    {
        try
        {
            int num = 0;
            DataTable dt = baseDataManage.GetBasePagerDataTable("V_PaymentRecord", out num, AspNetPager1.PageSize, Curr, "*", Con, "OperTime desc");
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
    public string jieduanStr(string str){
        string rs = "";
        if (str.Length > 20)
        {
            decimal count = str.Length / 20m;
            decimal rsrowcount = Math.Ceiling(count);
            for (int i = 0; i < rsrowcount; i++)
            {
                if (i != rsrowcount-1)
                {
                    rs += str.Substring(i * 20, 20) + "<br>";
                }
                else
                {
                    rs += str.Substring(i * 20, (str.Length - (i * 20)));
                }
                
            }
        }
        else
        {
            rs = str;
        }
        return rs;
    }
    public string changeType(string str)
    {
        //支付方式：1支付宝、2快钱、3汇付、4财付通、5支付宝网银、6快钱网银 、
        //7汇付网银、8财付通网银、9支付宝pos、10快钱pos、11汇付pos、12财付通pos、13易宝pos、14账户支付、15收银
        string rs = "";
        switch (str)
        {
            case "1":
                rs = "支付宝";
                break;
            case "2":
                rs = "快钱";
                break;
            case "3":
                rs = "汇付";
                break;
            case "4":
                rs = "财付通";
                break;
            case "5":
                rs = "支付宝网银";
                break;
            case "6":
                rs = "快钱网银";
                break;
            case "7":
                rs = "汇付网银";
                break;
            case "8":
                rs = "财付通网银";
                break;
            case "9":
                rs = "支付宝pos";
                break;
            case "10":
                rs = "快钱pos";
                break;
            case "11":
                rs = "汇付pos";
                break;
            case "12":
                rs = "财付通pos";
                break;
            case "13":
                rs = "易宝pos";
                break;
            case "14":
                rs = "账户支付";
                break;
            case "15":
                rs = "收银";
                break;
            default:
                break;
        }
        return rs;
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Query();
        BindV_User_Company_List();
    }
    protected void btnreset_Click(object sender, EventArgs e)
    {
        //txtUninAllNAME.Text = "";
        txtGoAlongTime1.Value = "";
        txtGoAlongTime2.Value = "";
        txtGoAlongTime1.Value = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
        txtGoAlongTime2.Value = DateTime.Now.ToString("yyyy-MM-dd");
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        string cpyno = mCompany.UninCode.Length > 12 ? mCompany.UninCode.Substring(0, 12) : mCompany.UninCode;
        DataBase.Data.HashObject Hparams = new DataBase.Data.HashObject();
        Hparams.Add("sqlWhere",Con);
        Hparams.Add("cpyno", cpyno);
        string pro = "pro_PayMentRecord";
        if (ViewState["paytype"].ToString()=="pos")
        {
            pro = "pro_PosPayMentRecord";
        }
        DataTable[] dtlist = base.baseDataManage.MulExecProc(pro, Hparams);
        DataTable dt = dtlist[0]; 
        ExcelRender.RenderToExcel(dt, Context, this.spantitle.InnerText + "报表_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")+".xls");
    }

    /// <summary>
    /// 获取客户名
    /// </summary>
    /// <param name="cpyid"></param>
    /// <returns></returns>
    public String GetCom(string paycpyno, string collcpyno,string payname, string collname)
    {
        string strname = "";
        if (paycpyno != "0" && mCompany.UninCode.ToString() == paycpyno)
        {
            if (mCompany.RoleType==2)
            {
                strname = collname;
            }
            else
            {
                strname = payname;
            }
        }
        else if (collcpyno != "0" && mCompany.UninCode.ToString() == collcpyno)
        {
            if (mCompany.RoleType == 2)
            {
                strname = payname;
            }
            else
            {
                strname = collname;
            }
        }
        return strname;

    }
    /// <summary>
    /// 获取支付方式
    /// </summary>
    /// <param name="childid"></param>
    /// <returns></returns>
    public String GetPayType(string childid)
    {
        return GetDictionaryName("4", childid);
    }
    protected void repPosList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (ViewState["paytype"].ToString()=="pos")
        {
            e.Item.FindControl("div3").Visible = false;
            e.Item.FindControl("div4").Visible = false;
            e.Item.FindControl("OperReasontd").Visible = false;
        }
       
    }
}