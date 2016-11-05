using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;
using PbProject.Model;
using System.Text;

public partial class Financial_CpyBankAccountList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnadd.PostBackUrl = string.Format("CpyBankAccountEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            Curr = 1;
            Query();
            AspNetPager1.PageSize = 20;
            ViewState["orderBy"] = " id ";
        }
    }
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
    /// <summary>
    /// 绑定pos机信息
    /// </summary>
    protected void BindCpyBankAccountInfo()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Tb_Company_BankAccount> list = baseDataManage.CallMethod("Tb_Company_BankAccount", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Tb_Company_BankAccount>;
        TotalCount = outParams.GetValue<int>("1");

        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        repPosList.DataSource = list;
        repPosList.DataBind();
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    protected void Query()
    {
        StringBuilder strWhere = new StringBuilder();
        strWhere.Append(mCompany.RoleType == 1 ? " 1=1 " : " CpyNo='" + mCompany.UninCode + "'");

        if (txtAccount.Text != "")
        {
            strWhere.Append(" and Account like '%" + txtAccount.Text + "%'");
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
        BindCpyBankAccountInfo();
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Query();
        BindCpyBankAccountInfo();
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnreset_Click(object sender, EventArgs e)
    {
        txtAccount.Text = "";
    }
    /// <summary>
    /// 返回账号类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    protected string ReturnBankType(string Banktype)
    {
        switch (Banktype)
        {
            case "1":
                Banktype = "银行账户";
                break;
            case "2":
                Banktype = "支付宝";
                break;
            case "3":
                Banktype = "快钱";
                break;
            case "4":
                Banktype = "汇付天下";
                break;
            case "5":
                Banktype = "财付通";
                break;
        }
        return Banktype;
    }
    /// <summary>
    /// 返回试用类型
    /// </summary>
    /// <param name="UseType"></param>
    /// <returns></returns>
    protected string ReturnUseType(string UseType)
    {
        switch (UseType)
        {
            case "1":
                UseType = "分账账户";
                break;
            case "2":
                UseType = "支付收款";
                break;
            case "3":
                UseType = "充值收款";
                break;
            case "4":
                UseType = "代付账号";
                break;
            case "5":
                UseType = "扣点分账账号";
                break;
        }
        return UseType;
    }
}