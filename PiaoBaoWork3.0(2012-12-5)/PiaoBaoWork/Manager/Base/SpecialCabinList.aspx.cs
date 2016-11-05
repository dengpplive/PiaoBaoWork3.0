using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using DataBase.Data;
using PbProject.Model;

public partial class Manager_Base_SpecialCabinList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.btnAdd.PostBackUrl = string.Format("SpecialCabinEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            Curr = 1;
            Con = " 1=1 and CpyNo ='" + mCompany.UninCode + "'";
            AspNetPager1.PageSize = 20;
            ViewState["orderBy"] = " SpAddTime desc";
            BaseCabinListDataBind();
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
    private void BaseCabinListDataBind()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Tb_SpecialCabin> list = baseDataManage.CallMethod("Tb_SpecialCabin", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Tb_SpecialCabin>;
        TotalCount = outParams.GetValue<int>("1");

        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        repCabinList.DataSource = list;
        repCabinList.DataBind();
    }
    /// <summary>
    /// 修改删除操作
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void repCabinList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string msg = "";
        try
        {
            string id = e.CommandArgument.ToString();
            if (e.CommandName == "Del")
            {
                msg = (bool)baseDataManage.CallMethod("Tb_SpecialCabin", "DeleteById", null, new Object[] { id }) == true ? "删除成功" : "删除失败";
            }

            BaseCabinListDataBind();
        }
        catch (Exception)
        {
            msg = "操作异常";
            throw;
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        BaseCabinListDataBind();
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SelButton_Click(object sender, EventArgs e)
    {
        Curr = 1;
        AspNetPager1.CurrentPageIndex = Curr;
        Con = SelWhere();
        BaseCabinListDataBind();
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    private string SelWhere()
    {
        StringBuilder StrWhere = new StringBuilder(" 1=1 and CpyNo ='"+mCompany.UninCode+"' ");
        if (txtAirPortCode.Value.Trim() != "" && txtAirPortCode.Value.Trim() != "0")
        {
            StrWhere.Append(" and SpAirCode = '" + txtAirPortCode.Value.Trim().Replace("'", "") + "'");
        }
        if (txtCabin.Text.Trim() != "")
        {
            StrWhere.Append(" and SpCabin = '" + txtCabin.Text.Trim().Replace("'", "") + "'");
        }
       

        return StrWhere.ToString();
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Reset_Click(object sender, EventArgs e)
    {
        txtAirPortCode.Value = "0";
        txtCabin.Text = "";
        
    }
}