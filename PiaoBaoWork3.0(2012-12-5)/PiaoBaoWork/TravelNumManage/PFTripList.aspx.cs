using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;
using PbProject.Model;
using System.Text;
using PbProject.Logic;
using System.Data;
public partial class TravelNumManage_PFTripList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Curr = 1;
            Con = " 1=1 ";
            OrderBy = " TripNum ";
            AspNetPager1.PageSize = int.Parse(Hid_PageSize.Value);
            Hid_RoleType.Value = mCompany.RoleType.ToString();
            //绑定供应或落地运营商
            BindGY();
        }
    }
    /// <summary>
    /// 绑定供应或落地运营
    /// </summary>
    public void BindGY()
    {
        DataTable table = this.baseDataManage.GetGYEmpolyees();
        ddlGY.Items.Clear();
        ddlGY.Items.Add(new ListItem("---供应或落地运营商---", ""));
        if (table != null && table.Rows.Count > 0)
        {
            foreach (DataRow dr in table.Rows)
            {
                //if (dr["RoleType"] != DBNull.Value && dr["RoleType"].ToString() == "2")
                //{
                ListItem li = new ListItem(dr["LoginName"].ToString() + "-" + dr["UninAllName"].ToString(), dr["UninCode"].ToString());
                ddlGY.Items.Add(li);
                //}
            }
        }
    }
    #region 属性
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
    /// 用于排序
    /// </summary>
    protected string OrderBy
    {
        get { return (string)ViewState["OrderBy"]; }
        set { ViewState["OrderBy"] = value; }
    }
    #endregion
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        PageDataBind();
    }
    /// <summary>
    /// 绑定列表
    /// </summary>
    private void PageDataBind()
    {
        try
        {
            int TotalCount = 0;
            IHashObject outParams = new HashObject();
            //指定参数类型 第一个参数为out输出类型
            //key 为参数索引从1开始 value为引用类型 out ref
            outParams.Add("1", "out");
            List<Tb_TripDistribution> list = baseDataManage.CallMethod("Tb_TripDistribution", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, OrderBy }) as List<Tb_TripDistribution>;
            TotalCount = outParams.GetValue<int>("1");
            AspNetPager1.RecordCount = TotalCount;
            AspNetPager1.CurrentPageIndex = Curr;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
            repList.DataSource = list;
            repList.DataBind();
        }
        catch (Exception ex)
        {
            DataBase.LogCommon.Log.Error("ApplyTravel.aspx:PageDataBind()", ex);
        }
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    public string Query()
    {
        StringBuilder sbLog = new StringBuilder(" 1 =1");
        //行程单号段
        if (txtStartNum.Text.Trim() != "" && txtEndNum.Text.Trim() != "")
        {
            sbLog.AppendFormat(" and left(TripNum ,6)=left('{0}',6) and cast(right(TripNum,4) as int)>=cast(right('{0}',4) as int) and cast(right(TripNum,4) as int)<=cast(right('{1}',4) as int) ", txtStartNum.Text.Trim(), txtEndNum.Text.Trim());
        }
        //领用公司名称
        if (!string.IsNullOrEmpty(txtUseCpyName.Text.Trim()))
        {
            sbLog.AppendFormat(" and  UseCpyName='{0}' ", txtUseCpyName.Text.Trim());
        }
        //领用公司编号
        if (!string.IsNullOrEmpty(txtUseCpyNo.Text.Trim()))
        {
            sbLog.AppendFormat(" and  UseCpyNo='{0}' ", txtUseCpyNo.Text.Trim());
        }
        //票号
        if (!string.IsNullOrEmpty(txtTicketNumber.Text.Trim()))
        {
            sbLog.AppendFormat(" and  TicketNum='{0}' ", txtTicketNumber.Text.Trim());
        }
        //创建Office
        if (!string.IsNullOrEmpty(txtOffice.Text.Trim()))
        {
            sbLog.AppendFormat(" and  CreateOffice='{0}' ", txtOffice.Text.Trim());
        }
        //落地运营商
        if (ddlGY.SelectedValue.Trim() != "")
        {
            sbLog.AppendFormat(" and  OwnerCpyNo='{0}' ", ddlGY.SelectedValue.Trim());
        }
        //行程单状态
        if (ddlTripStatus.SelectedValue.Trim() != "-1")
        {
            sbLog.AppendFormat(" and  TripStatus='{0}' ", ddlTripStatus.SelectedValue.Trim());
        }
        //入库日期
        if (txtSDate.Value.Trim() != "" && txtEDate.Value.Trim() != "")
        {
            sbLog.AppendFormat(" and  (AddTime between '{0} 00:00:00' and '{1} 23:59:59')", txtSDate.Value.Trim(), txtEDate.Value.Trim());
        }
        return sbLog.ToString();
    }
    //查询
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Curr = 1;
        Con = Query();
        PageDataBind();
    }
    /// <summary>
    /// 显示文本
    /// </summary>
    /// <param name="type"></param>
    /// <param name="objs"></param>
    /// <returns></returns>
    public string ShowText(int type, params object[] objs)
    {
        string result = "";
        if (type == 0)//行程单状态
        {
            if (objs != null && objs.Length > 0)
            {
                string TripStatus = objs[0].ToString();
                result = GetDictionaryName("34", TripStatus);
            }
        }
        else if (type == 1)
        {
            if (objs != null && objs.Length > 0)
            {
                result = objs[0].ToString();
                DateTime dt = DateTime.Parse("1900-01-01");
                DateTime.TryParse(objs[0].ToString(), out dt);
                if (dt.ToString("yyyy-MM-dd") == "1900-01-01" || dt.ToString("yyyy-MM-dd") == "1901-01-01" || dt.ToString("yyyy-MM-dd") == "1900-1-1" || dt.ToString("yyyy-MM-dd") == "1901-1-1")
                {
                    result = "";
                }
            }
        }
        else if (type == 2)//行程单状态
        {
            StringBuilder sbAppend = new StringBuilder();
            if (objs != null && objs.Length == 2)
            {
                string strId = objs[0].ToString();
                string strTripStatus = objs[1].ToString();
                sbAppend.Append("<select id=\"selTripStatus_" + strId + "\">");
                sbAppend.AppendFormat("<option value=\"1\" " + (strTripStatus == "1" ? "Selected=true" : "") + ">已入库,未分配</option>");
                sbAppend.AppendFormat("<option value=\"2\" " + (strTripStatus == "2" ? "Selected=true" : "") + ">已分配,未使用</option>");
                sbAppend.AppendFormat("<option value=\"6\" " + (strTripStatus == "6" ? "Selected=true" : "") + ">作废行程单,等待审核</option>");
                sbAppend.AppendFormat("<option value=\"7\" " + (strTripStatus == "7" ? "Selected=true" : "") + ">拒绝作废行程单,审核失败</option>");
                sbAppend.AppendFormat("<option value=\"8\" " + (strTripStatus == "8" ? "Selected=true" : "") + ">空白回收,未分配</option>");
                sbAppend.AppendFormat("<option value=\"9\" " + (strTripStatus == "9" ? "Selected=true" : "") + ">已创建行程单</option>");
                sbAppend.AppendFormat("<option value=\"10\" " + (strTripStatus == "10" ? "Selected=true" : "") + ">已作废行程单,审核成功</option>");
                sbAppend.AppendFormat("<option value=\"11\" " + (strTripStatus == "11" ? "Selected=true" : "") + ">空白回收,已分配</option>");
                sbAppend.Append("</select>");
                result = sbAppend.ToString();
            }
        }
        return result;
    }
}