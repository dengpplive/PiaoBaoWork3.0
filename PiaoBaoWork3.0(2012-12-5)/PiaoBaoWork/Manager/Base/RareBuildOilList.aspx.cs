using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;
using PbProject.Model;
using System.Text;
public partial class Manager_Base_RareBuildOilList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Curr = 1;
            Con = " 1=1";
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

    private void BindList()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Tb_Air_BuildOilInfo> list = baseDataManage.CallMethod("Tb_Air_BuildOilInfo", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, " id " }) as List<Tb_Air_BuildOilInfo>;
        TotalCount = outParams.GetValue<int>("1");

        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        repCabinList.DataSource = list;
        repCabinList.DataBind();
    }
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        BindList();
    }

    public string Query()
    {
        StringBuilder sqlWhere = new StringBuilder(" 1=1 ");
        //出发城市
        if (!string.IsNullOrEmpty(txtFromCityName.Value.Trim()) && txtFromCityName.Value.Trim() != "中文/英文")
        {
            sqlWhere.AppendFormat(" and FromCityCode='{0}' ", txtFromCityName.Value.Trim());
        }
        //到达城市
        if (!string.IsNullOrEmpty(txtToCityName.Value.Trim()) && txtToCityName.Value.Trim() != "中文/英文")
        {
            sqlWhere.AppendFormat(" and ToCityCode='{0}' ", txtToCityName.Value.Trim());
        }
        //乘客类型
        if (ddlPasType.SelectedValue != "")
        {
            sqlWhere.AppendFormat(" and PersonType={0} ", ddlPasType.SelectedValue.Trim());
        }
        //基建费
        if (txtTAX.Text.Trim() != "")
        {
            sqlWhere.AppendFormat(" and BuildPrice={0} ", txtTAX.Text.Trim());
        }
        //燃油费
        if (txtRQFare.Text.Trim() != "")
        {
            sqlWhere.AppendFormat(" and OilPrice={0} ", txtRQFare.Text.Trim());
        }
        return sqlWhere.ToString();
    }

    /// <summary>
    /// 显示
    /// </summary>
    /// <returns></returns>
    public string ShoText(int type, params object[] objParams)
    {
        string result = "";
        if (type == 1)
        {
            if (objParams != null && objParams.Length == 1 && objParams[0].ToString() != "")
            {
                result = objParams[0].ToString() == "1" ? "成人" : "儿童";
            }
        }
        return result;
    }
    protected void SelButton_Click(object sender, EventArgs e)
    {
        Con = Query();
        Curr = 1;
        BindList();
    }
    //添加
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Format("EditRareBuildOil.aspx?currentuserid={0}", Request["currentuserid"].ToString()));
    }
    protected void repCabinList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Del")
        {
            //删除
            bool DeleteSuc = (bool)baseDataManage.CallMethod("Tb_Air_BuildOilInfo", "DeleteById", null, new object[] { e.CommandArgument.ToString() });
            if (DeleteSuc)
            {
                BindList();
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('删除成功！')", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('删除失败！')", true);
            }
        }
    }
}