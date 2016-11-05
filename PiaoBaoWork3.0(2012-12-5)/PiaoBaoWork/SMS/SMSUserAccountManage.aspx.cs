using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DataBase.Data;
using System.Text;

public partial class SMS_SMSUserAccountManage : BasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Curr = 1;
            Con = Query();
            AspNetPager1.PageSize = 20;
            Bind();
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
    /// 翻页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        try
        {
            Curr = e.NewPageIndex; //翻页
            Bind();  // 重新加载数据
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 数据绑定方法
    /// </summary>
    public void Bind()
    {
        int num = 0;
        DataTable dt = baseDataManage.GetBasePagerDataTable("V_Sms_UserManage", out num, AspNetPager1.PageSize, Curr, "*", Con, "SmsSendCount desc");
        AspNetPager1.RecordCount = num;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        RptShow.DataSource = dt;
        RptShow.DataBind();
    }
    private string Query()
    {
        StringBuilder strWhere = new StringBuilder();
        try
        {
            if (mCompany.RoleType == 2)
            {
                //运营商所有下级
                //strWhere.Append(" RoleType in (4,5) and len(CpyNo)=18 and SUBSTRING(CpyNo,1,12)='" + mCompany.UninCode + "' ");
                strWhere.Append(" RoleType in (4,5) and SUBSTRING(CpyNo,1,12)='" + mCompany.UninCode + "' ");
            }
            //else if (mCompany.RoleType == 4)
            //{
            //    //分销直属下级
            //    if (mCompany.UninCode.Length == 18)
            //    {
            //        strWhere.Append(" RoleType in (4,5) and len(CpyNo)=24 and SUBSTRING(CpyNo,1,18)='" + mCompany.UninCode + "' ");
            //    }
            //    else if (mCompany.UninCode.Length == 24)
            //    {
            //        //只有采购
            //        strWhere.Append(" RoleType = 5 and len(CpyNo)=30 and SUBSTRING(CpyNo,1,24)='" + mCompany.UninCode + "' ");
            //    }
            //}
            else if (mCompany.RoleType == 3 || mCompany.RoleType == 5)
            {
                strWhere.Append(" 1<>1");
            }
            else
            {
                //平台以下所有商家
                strWhere.Append(" RoleType<>1");
            }
            if (txtCpyName.Text.Trim()!="")
            {
                strWhere.Append(" and UninAllName like '%"+txtCpyName.Text.Trim()+"%'");
            }
            return strWhere.ToString();
        }
        catch (Exception)
        {
             return strWhere.ToString();
        }
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Con= Query();
        Bind();
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        this.txtCpyName.Text = "";
    }
   
}