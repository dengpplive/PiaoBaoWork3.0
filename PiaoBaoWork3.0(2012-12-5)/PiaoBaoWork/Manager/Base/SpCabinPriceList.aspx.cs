using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using DataBase.Data;
using PbProject.Model;
public partial class Manager_Base_SpCabinPriceList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Curr = 1;
            Con = " 1=1";
            currentuserid.Value = mUser.id.ToString();
        }
        HandRequest();
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
        List<Tb_SpecialCabin_PriceInfo> list = baseDataManage.CallMethod("Tb_SpecialCabin_PriceInfo", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, " UpdateTime desc " }) as List<Tb_SpecialCabin_PriceInfo>;
        TotalCount = outParams.GetValue<int>("1");

        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        repSpList.DataSource = list;
        repSpList.DataBind();
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
            sqlWhere.AppendFormat(" and FromCityCode='{0}' ", FromCityCode.Value.Trim());
        }
        //到达城市
        if (!string.IsNullOrEmpty(txtToCityName.Value.Trim()) && txtToCityName.Value.Trim() != "中文/英文")
        {
            sqlWhere.AppendFormat(" and ToCityCode='{0}' ", ToCityCode.Value.Trim());
        }
        //承运人 航空公司二字码
        if (txtCarryCode.Value != "")
        {
            sqlWhere.AppendFormat(" and SpAirCode='{0}' ", txtCarryCode.Value.Trim());
        }
        //舱位
        if (txtSpCabin.Text.Trim() != "")
        {
            sqlWhere.AppendFormat(" and SpCabin='{0}' ", txtSpCabin.Text.Trim());
        }
        //起飞时间
        if (txtStartFlightTime.Value.Trim() != "" && txtEndFlightTime.Value.Trim() != "")
        {
            sqlWhere.AppendFormat(" and  FlightTime between '{0} 00:00:00' and '{1} 23:59:59' ", txtStartFlightTime.Value.Trim(), txtEndFlightTime.Value.Trim());
        }
        //缓存时间
        if (txtStartUpdateTime.Value.Trim() != "" && txtEndUpdateTime.Value.Trim() != "")
        {
            sqlWhere.AppendFormat(" and  UpdateTime between '{0} 00:00:00' and '{1} 23:59:59' ", txtStartUpdateTime.Value.Trim(), txtEndUpdateTime.Value.Trim());
        }
        return sqlWhere.ToString();
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Con = Query();
        Curr = 1;
        BindList();
    }
    /// <summary>
    /// 命令
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void repSpList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string id = e.CommandArgument.ToString();
        if (e.CommandName == "Del")
        {
            bool IsDel = (bool)this.baseDataManage.CallMethod("Tb_SpecialCabin_PriceInfo", "DeleteById", null, new object[] { id });
            if (IsDel)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('删除成功！')", true);
                BindList();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('删除失败！')", true);
            }
        }
        else if (e.CommandName == "Update")
        {

        }
    }
    public void OutPut(string result)
    {
        Response.ContentType = "text/plain";
        Response.Clear();
        Response.Write(escape(result));
        Response.Flush();
        Response.End();
    }
    public void HandRequest()
    {
        if (Request["CommandName"] != null && Request["CommandName"].ToString() != "")
        {
            string CommandName = Request["CommandName"].ToString();
            string IdList = Request["IdList"].ToString();
            int a = 0;
            try
            {
                if (CommandName == "Del")
                {
                    string[] strArr = IdList.Split(',');
                    if (strArr.Length > 0)
                    {
                        List<string> lst = new List<string>();
                        lst.AddRange(strArr);
                        List<Tb_SpecialCabin_PriceInfo> lstModel = new List<Tb_SpecialCabin_PriceInfo>();
                        IHashObject OutParams = new HashObject();
                        OutParams.Add("2", "out");
                        bool IsDel = (bool)this.baseDataManage.CallMethod("Tb_SpecialCabin_PriceInfo", "DeleteByIds", OutParams, new object[] { lst, lstModel });
                        if (IsDel)
                        {
                            a = 1;
                        }
                    }
                }
                else if (CommandName == "Update")
                {
                    string strSpPrice = Request["SpPrice"] != null ? Request["SpPrice"].ToString() : "";
                    string strSpABFare = Request["SpABFare"] != null ? Request["SpABFare"].ToString() : "";
                    string strSpRQFare = Request["SpRQFare"] != null ? Request["SpRQFare"].ToString() : "";

                    IHashObject parameter = new HashObject();
                    parameter.Add("SpPrice", strSpPrice);
                    parameter.Add("SpABFare", strSpABFare);
                    parameter.Add("SpRQFare", strSpRQFare);
                    bool IsUpdate = (bool)this.baseDataManage.CallMethod("Tb_SpecialCabin_PriceInfo", "Update", null, new object[] { parameter });
                    if (IsUpdate)
                    {
                        a = 1;
                    }
                }
            }
            catch (Exception)
            {
            }
            OutPut(a.ToString());
        }
    }
}