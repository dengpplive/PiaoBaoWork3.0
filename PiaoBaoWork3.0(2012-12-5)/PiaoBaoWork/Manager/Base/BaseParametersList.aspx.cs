using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Dal.ControlBase;
using PbProject.Model;
using DataBase.Data;
using DataBase.Unique;
using System.Text;
using PbProject.Logic.ControlBase;
/// <summary>
/// 参数表
/// </summary>
public partial class Manager_Base_BaseParametersList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            btnAdd.PostBackUrl = string.Format("BaseParametersEdit.aspx?currentuserid={0}", this.currentuserid.Value);
            Curr = 1;
            Con = " 1=1";
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
    private void PageDataBind()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Bd_Base_Parameters> list = baseDataManage.CallMethod("Bd_Base_Parameters", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Bd_Base_Parameters>;
        TotalCount = outParams.GetValue<int>("1");
        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        Repeater.DataSource = list;
        Repeater.DataBind();
    }
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        PageDataBind();
    }
    //查询
    protected void SelButton_Click(object sender, EventArgs e)
    {
        StringBuilder sbWere = new StringBuilder(" 1=1 ");
        //公司编号
        if (txtCompanyNo.Text.Trim() != "")
        {
            sbWere.Append(string.Format(" and CpyNo like '%{0}%' ", txtCompanyNo.Text.Trim().Replace("'", "")));
        }
        //参数名称
        if (txtParamName.Text.Trim() != "")
        {
            sbWere.Append(string.Format(" and SetName like '%{0}%' ", txtParamName.Text.Trim().Replace("'", "")));
        }
        //参数值
        if (txtParamValue.Text.Trim() != "")
        {
            sbWere.Append(string.Format(" and SetValue like '%{0}%' ", txtParamValue.Text.Trim().Replace("'", "")));
        }
        //参数描述
        if (txtDescription.Text.Trim() != "")
        {
            sbWere.Append(string.Format(" and SetDescription like '%{0}%' ", txtDescription.Text.Trim().Replace("'", "")));
        }
        ////起止日期
        //if (txtStartDate.Value.Trim() != "" && txtEndDate.Value.Trim() != "")
        //{
        //    sbWere.Append(string.Format(" and  StartDate<EndDate and StartDate >'{0}' and  StartDate<'{1}' and   EndDate >'{0}' and EndDate<'{1}'", txtStartDate.Value.Trim().Replace("'", ""), txtEndDate.Value.Trim().Replace("'", "")));
        //}
        Curr = 1;
        AspNetPager1.CurrentPageIndex = Curr;
        Con = sbWere.ToString();
        PageDataBind();
    }
    /// <summary>
    /// 显示部分字符
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    public string DisplayChars(object objVal, int len, string replaceSchar)
    {
        string Str = objVal == null ? "" : objVal.ToString();
        if (!string.IsNullOrEmpty(Str))
        {
            if (Str.Length > len)
            {
                Str = Str.Substring(0, len) + " " + replaceSchar;
            }
        }
        else
        {
            Str = "";
        }
        return Str;
    }
    //清空数据
    protected void Reset_Click(object sender, EventArgs e)
    {
        txtCompanyNo.Text = "";
        txtParamName.Text = "";
        txtParamValue.Text = "";
        txtDescription.Text = "";
        //txtStartDate.Value = "";
        //txtEndDate.Value = "";
    }
    protected void btnSelectCompany_Click(object sender, EventArgs e)
    {
        Response.Redirect("../../User/ComPanyList.aspx?currentuserid=" + this.currentuserid.Value.ToString());
    }
}