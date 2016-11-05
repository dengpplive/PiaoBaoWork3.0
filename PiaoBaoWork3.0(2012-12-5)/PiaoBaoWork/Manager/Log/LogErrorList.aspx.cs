using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;
using PbProject.Model;
using System.Text;

public partial class Logs_LogErrorList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Curr = 1;
            Query();
            AspNetPager1.PageSize = 20;
            ViewState["orderBy"] = " ErorrTime desc ";
            BindErrorinfo();
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
    protected void BindErrorinfo()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Log_Error> list = baseDataManage.CallMethod("Log_Error", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Log_Error>;
        TotalCount = outParams.GetValue<int>("1");

        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        repErrorList.DataSource = list;
        repErrorList.DataBind();
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    protected void Query()
    {
        try
        {
            Curr = 1;
            StringBuilder WhereStr = new StringBuilder(" 1=1");
            WhereStr.Append(txtCpyNo.Text.Length > 0 ? " and CpyNo like '%" + txtCpyNo.Text + "%'" : "");
            WhereStr.Append(txtDevName.Text.Length > 0 ? " and DevName like '%" + txtDevName.Text + "%'" : "");
            WhereStr.Append(txtStartTime.Text != "" ? " and ErorrTime >= convert(DateTime, '" + txtStartTime.Text + " 00:00:00')" : "");
            WhereStr.Append(txtEndTime.Text != "" ? " and ErorrTime <= convert(DateTime, '" + txtEndTime.Text + " 23:59:59')" : "");
            Con = WhereStr.ToString();
        }
        catch (Exception)
        {

            throw;
        }
    }
   
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SelButton_Click(object sender, EventArgs e)
    {
        Query();
        BindErrorinfo();
    }
    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        BindErrorinfo();
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Reset_Click(object sender, EventArgs e)
    {
        this.txtCpyNo.Text = "";
        this.txtDevName.Text = "";
        this.txtStartTime.Text = "";
        this.txtEndTime.Text = "";
    }
   
}