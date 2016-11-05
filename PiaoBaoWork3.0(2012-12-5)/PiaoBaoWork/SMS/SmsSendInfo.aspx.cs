using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;
using PbProject.Model;
using System.Text;

public partial class SMS_SmsSendInfo : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["flag"] != null && Request.QueryString["flag"].ToString() != "")
        {
            ViewState["flag"] = Request.QueryString["flag"].ToString();//0返回公司账户管理、1客户账户管理
        }
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            if (Request.QueryString["cpyno"] != null && Request.QueryString["cpyno"].ToString() != "")
            {
                ViewState["cpyno"] = Request.QueryString["cpyno"].ToString();
            }
            Curr = 1;
            Query();
            AspNetPager1.PageSize = 20;
            ViewState["orderBy"] = " SmsCreateDate desc ";
            Bindrplist();
        }
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    protected string Con
    {
        get { return (string)ViewState["Con"]; }
        set { ViewState["Con"] = value; }
    }
    /// <summary>
    /// 分页起始页从 0开始
    /// </summary>
    protected int Curr
    {
        get { return (int)ViewState["currpage"]; }
        set { ViewState["currpage"] = value; }
    }

    protected void Bindrplist()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Tb_Sms_SendInfo> list = baseDataManage.CallMethod("Tb_Sms_SendInfo", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Tb_Sms_SendInfo>;
        TotalCount = outParams.GetValue<int>("1");

        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        RptShow.DataSource = list;
        RptShow.DataBind();
    }
    protected void Query()
    {
        StringBuilder strWhere = new StringBuilder("");
        strWhere.Append(ViewState["cpyno"] != null ? "CpyNo='" + ViewState["cpyno"].ToString() + "'" : "1<>1");
        strWhere.Append(txtDateS.Value != "" ? " and SmsCreateDate >= convert(DateTime, '" + txtDateS.Value + " 00:00:00')" : "");
        //时间结束
        strWhere.Append(txtDateE.Value != "" ? " and SmsCreateDate <= convert(DateTime, '" + txtDateE.Value + " 23:59:59')" : "");
        Con = strWhere.ToString();
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Query();
        Bindrplist();
    }
    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        Bindrplist();
    }
    /// <summary>
    /// 返回
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        Response.Redirect(ViewState["flag"].ToString() == "0" ? "SMSCompanyAccoutManage.aspx?currentuserid=" + this.currentuserid.Value.ToString() : "SMSUserAccountManage.aspx?currentuserid=" + this.currentuserid.Value.ToString());
    }
}