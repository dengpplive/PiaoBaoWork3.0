using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DataBase.Data;
using PbProject.Model;
using System.Text;

public partial class Sms_Transaction : BasePage
{
    public string id = null;
  
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["flag"] != null && Request.QueryString["flag"].ToString() != "")
        {
            ViewState["flag"] = Request.QueryString["flag"].ToString();//0返回公司账户管理、1客户账户管理
        }
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            List<Bd_Base_Dictionary> dictionarylist = baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new Object[] { "ParentID=4" }) as List<Bd_Base_Dictionary>;
            ViewState["dictionary"] = dictionarylist;
            if (Request.QueryString["cpyno"] != null && Request.QueryString["cpyno"].ToString() != "")
            {
                ViewState["cpyno"] = Request.QueryString["cpyno"].ToString();
            }
            Curr = 1;
            Query();
            AspNetPager1.PageSize = 20;
            BindInfo();
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

    /// <summary>
    /// 绑定数据方法
    /// </summary>
    public void BindInfo()
    {
        
        int num = 0;
        DataTable dt = baseDataManage.GetBasePagerDataTable("V_Sms_Transaction", out num, AspNetPager1.PageSize, Curr, "*", Con, "RechargeDate desc");
        AspNetPager1.RecordCount = num;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        RptShow.DataSource = dt;
        RptShow.DataBind();
    }

    /// <summary>
    /// 加载查询条件
    /// </summary>
    protected void Query()
    {
        StringBuilder strWhere = new StringBuilder("1=1");
        if (ViewState["cpyno"]!=null)
        {
            strWhere.Append(" and CpyNo='"+ViewState["cpyno"].ToString()+"'");
        }
        strWhere.Append(txtDateS.Value != "" ? " and RechargeDate >= convert(DateTime, '" + txtDateS.Value + " 00:00:00')" : "");
        //时间结束
        strWhere.Append(txtDateE.Value != "" ? " and RechargeDate <= convert(DateTime, '" + txtDateE.Value + " 23:59:59')" : "");
        Con = strWhere.ToString();
    }
   
    /// <summary>
    /// 查询按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Query();
        BindInfo();
    }

    /// <summary>
    /// 翻页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex; //翻页
        BindInfo();  // 重新加载数据
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
    public string getpaytypename(string childid)
    {
        string name = "";
        List<Bd_Base_Dictionary> list = ViewState["dictionary"] as List<Bd_Base_Dictionary>;
        foreach (var item in list)
        {
            if (item.ChildID.ToString() == childid)
            {
                name = item.ChildName.ToString();
            }
        }
        return name;
    }
   


}