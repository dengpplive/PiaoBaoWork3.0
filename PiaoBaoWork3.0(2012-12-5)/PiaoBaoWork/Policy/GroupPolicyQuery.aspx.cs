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

public partial class Policy_GroupPolicyQuery : BasePage
{

    ////发布页面类型 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊舱位政策
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            this.currentuserid.Value = this.mUser.id.ToString();
        try
        {
            //if (this.SessionIsNull)
            //{
            //    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面也失效,请重新登录！','0');", true);
            //    return;
            //}
            //初始化数据
            if (!InitPage())
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面初始化异常');", true);
                return;
            }
            if (mCompany == null || mCompany.RoleType > 3)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('您没有权限访问该页面','-1');", true);
                return;
            }
            if (!IsPostBack)
            {
                BindCity();
                BindClass("");
                Hid_LoginName.Value = mUser.LoginName;//供应商登录账号
                Hid_UserName.Value = mUser.UserName;//供应商名字
                Hid_CpyNo.Value = mCompany.UninCode;//公司编号
                Hid_CpyName.Value = mCompany.UninAllName;//公司名称
                //分页大小
                AspNetPager1.PageSize = int.Parse(hid_PageSize.Value);
                OrderBy = " id ";
                Curr = 1;
                Con = " 1=1";
            }
        }
        catch (Exception)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面初始化异常!');", true);
        }
    }

    private List<User_Company> Comlist = null;
    public List<User_Company> GetUserCompany()
    {
        try
        {
            string sqlWhere = " RoleType=2  ";//and AccountState=1
            Comlist = this.baseDataManage.CallMethod("User_Company", "GetList", null, new object[] { sqlWhere }) as List<User_Company>;
        }
        catch (Exception)
        {
        }
        return Comlist;
    }
    /// <summary>
    /// 初始化页面
    /// </summary>
    public bool InitPage()
    {
        bool IsInitSuc = false;
        if (Request["PageType"] != null && Request["PageType"].ToString() != "")
        {
            PageType = Request["PageType"].ToString();
            showText(PageType);
            IsInitSuc = true;
        }
        return IsInitSuc;
    }
    #region 页面显示
    /// <summary>
    /// 显示页面标题文字
    /// </summary>
    /// <param name="PageType"></param>
    public void showText(string PageType)
    {
        //发布页面类型 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊舱位政策
        if (PageType == "4")
        {
            tleName.Text = "散冲团政策查看";
            lblShow.Text = "散冲团政策查看";
        }
    }
    /// <summary>
    /// 绑定数据字面显示
    /// </summary>
    /// <param name="type"></param>
    /// <param name="strData"></param>
    /// <param name="strZK"></param>
    /// <param name="strPrice"></param>
    /// <returns></returns>
    public string ShowItem(int type, params object[] paObj)// string strData, string strZK, string strPrice)
    {
        string reData = "";
        //起飞抵达时间
        if (type == 1)
        {
            if (paObj != null && paObj.Length == 1)
            {
                string strData = paObj[0] != null ? paObj[0].ToString() : "";
                if (!string.IsNullOrEmpty(strData) && strData.Split('-').Length == 2)
                {
                    reData = strData.Split('-')[0].Substring(0, 5) + "-" + strData.Split('-')[1].Substring(0, 5);
                }
            }
        }
        else if (type == 2)//政策类型
        {
            if (paObj != null && paObj.Length == 1)
            {
                string strData = paObj[0] != null ? paObj[0].ToString() : "";
                if (strData == "1")
                {
                    reData = "B2B";
                }
                else if (strData == "2")
                {
                    reData = "BSP";
                }
                else if (strData == "3")
                {
                    reData = "B2B/BSP";
                }
            }
        }
        else if (type == 3)//优惠方式
        {
            if (paObj != null && paObj.Length == 3)
            {
                string strData = paObj[0] != null ? paObj[0].ToString() : "";
                string strPrice = paObj[2] != null ? paObj[2].ToString() : "0";
                string strZK = paObj[1] != null ? paObj[1].ToString() : "0";
                if (strData == "1" || strData.ToString().ToLower() == "true")
                {
                    reData = strPrice;
                }
                else
                {
                    reData = strZK;
                }
            }
        }
        else if (type == 4)//运营商
        {
            if (paObj != null && paObj.Length == 1)
            {
                string CoyNo = paObj[0] != null ? paObj[0].ToString() : "";
                if (Comlist == null)
                {
                    Comlist = GetUserCompany();
                }
                if (Comlist != null)
                {
                    User_Company uc = Comlist.Find((c) => (c.UninCode == CoyNo && CoyNo != ""));
                    if (uc != null)
                    {
                        reData = uc.UninAllName;
                    }
                }
            }
        }
        else if (type == 5)//修改
        {
            if (paObj != null && paObj.Length == 2)
            {
                string Id = paObj[0] != null ? paObj[0].ToString() : "";
                string CpyNo = paObj[1] != null ? paObj[1].ToString() : "";
                if (mCompany != null && !string.IsNullOrEmpty(CpyNo) && !string.IsNullOrEmpty(Id) && mCompany.UninCode == CpyNo)
                {
                    StringBuilder sbCon = new StringBuilder();
                    sbCon.Append("<div id='divContainer_" + Id + "'>");
                    sbCon.Append(" <a id=\"a_" + Id + "\" href=\"#\" onclick=\"return showUpdate('" + Id + "')\">");
                    sbCon.Append("   修改</a></div>");
                    sbCon.Append(" <div id=\"divUpdateCon_" + Id + "\" class=\"hide\">");
                    sbCon.Append("  <span id=\"span_update_" + Id + "\"><a id='a_update_" + Id + "' href=\"#\"");
                    sbCon.Append("     onclick=\"return ajaxUpdate('" + Id + "','1')\">更新</a></span> <span id=\"span_cancel_" + Id + "\">");
                    sbCon.Append("       <a id='a_cancel_" + Id + "' href=\"#\" onclick=\"return hideUpdate('" + Id + "')\">");
                    sbCon.Append("         取消</a></span><br /></div>");

                    reData = sbCon.ToString();
                }
            }
        }
        return reData;
    }
    /// <summary>
    /// 显示部分字符
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="Len"></param>
    /// <returns></returns>
    public string SubChar(object Str, int Len, string replaceSchar)
    {
        string reStr = "";
        if (Str == null) return reStr;
        reStr = Str.ToString();
        if (!string.IsNullOrEmpty(reStr))
        {
            if (reStr.Length > Len)
            {
                reStr = reStr.Substring(0, Len) + " " + replaceSchar;
            }
        }
        else
        {
            reStr = "";
        }
        return reStr;
    }
    #endregion
    #region 属性

    /// <summary>
    /// 检查Session是否丢失
    /// </summary>
    //public bool SessionIsNull
    //{
    //    get
    //    {
    //        bool isSuc = false;
    //        if (Session[new SessionContent().USERLOGIN] == null)
    //        {
    //            isSuc = true;
    //        }
    //        return isSuc;
    //    }
    //}
    /// <summary>
    /// 查询条件
    /// </summary>
    protected string Con
    {
        get { return (string)ViewState["Con"]; }
        set { ViewState["Con"] = value; }
    }
    /// <summary>
    /// 当前分页第几页
    /// </summary>
    protected int Curr
    {
        get { return (int)ViewState["currpage"]; }
        set { ViewState["currpage"] = value; }
    }
    /// <summary>
    /// 排序字段和升降序
    /// </summary>
    public string OrderBy
    {
        get { return (string)ViewState["orderBy"]; }
        set { ViewState["orderBy"] = value; }
    }
    /// <summary>
    /// 页面类型  1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊舱位政策
    /// </summary>
    public string PageType
    {
        get { return (string)ViewState["PageType"]; }
        set { ViewState["PageType"] = value; Hid_PageType.Value = value; }
    }
    #endregion

    private void PageDataBind()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Tb_Ticket_UGroupPolicy> list = baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, OrderBy }) as List<Tb_Ticket_UGroupPolicy>;
        TotalCount = outParams.GetValue<int>("1");
        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        Repeater.DataSource = list;
        Repeater.DataBind();
    }

    //绑定舱位
    public void BindClass(string SelClass)
    {
        string strChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        List<char> strList = new List<char>();
        strList.AddRange(strChar.ToCharArray());
        ddlClass.DataSource = strList;
        ddlClass.DataBind();
    }
    /// <summary>
    /// 绑定城市
    /// </summary>
    public void BindCity()
    {
        string sqlWhere = " IsDomestic=1 ";
        List<Bd_Air_AirPort> defaultList = this.baseDataManage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { sqlWhere }) as List<Bd_Air_AirPort>;
        //出发城市
        ddlFromCity.DataSource = defaultList;
        ddlFromCity.DataFiledText = "CityCodeWord-CityName";
        ddlFromCity.DataFiledValue = "CityCodeWord";
        ddlFromCity.DataBind();
        //到达城市
        ddlToCity.DataSource = defaultList;
        ddlToCity.DataFiledText = "CityCodeWord-CityName";
        ddlToCity.DataFiledValue = "CityCodeWord";
        ddlToCity.DataBind();
    }

    public string Query(string IsExpires)
    {
        StringBuilder sbWhere = new StringBuilder(" 1=1 ");
        // sbWhere.Append(string.Format(" CpyNo='{0}' ", mCompany.UninCode));

        //承运人
        if (ddlAirCode.Value != "" && ddlAirCode.Value != "0")
        {
            sbWhere.AppendFormat(" and AirCode = '{0}' ", ddlAirCode.Value);
        }
        //出发城市
        if (ddlFromCity.Value != "" && ddlFromCity.Value != "0")
        {
            sbWhere.AppendFormat(" and FromCityCode = '{0}' ", ddlFromCity.Value);
        }
        //到达城市
        if (ddlToCity.Value != "" && ddlToCity.Value != "0")
        {
            sbWhere.AppendFormat(" and ToCityCode = '{0}' ", ddlToCity.Value);
        }
        //舱位
        if (ddlClass.Value != "" && ddlClass.Value != "0")
        {
            sbWhere.AppendFormat(" and Class = '{0}' ", ddlClass.Value);
        }
        //政策类型
        if (ddlPolicyType.SelectedValue != "" && ddlPolicyType.SelectedValue != "-1")
        {
            sbWhere.AppendFormat(" and PolicyType = {0} ", ddlPolicyType.SelectedValue);
        }
        //优惠方式
        if (ddlPriceType.SelectedValue != "" && ddlPriceType.SelectedValue != "-1")
        {
            sbWhere.AppendFormat(" and PriceType = {0} ", ddlPriceType.SelectedValue);
        }
        //有效日期
        if (txtFlightStartDate.Value != "" && txtFlightEndDate.Value != "")
        {
            sbWhere.Append(" and FlightStartDate  >='" + txtFlightStartDate.Value.Trim() + ":00' and FlightEndDate <= '" + txtFlightEndDate.Value.Trim() + ":59' ");
        }
        //有效日期
        if (txtTicketStartDate.Value != "" && txtTicketEndDate.Value != "")
        {
            sbWhere.Append(" and PrintStartDate  >='" + txtTicketStartDate.Value.Trim() + " 00:00:00' and PrintEndDate <= '" + txtTicketEndDate.Value.Trim() + " 23:59:59' ");
        }
        return sbWhere.ToString();
    }
    #region 事件
    /// <summary>
    /// 分页事件
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        PageDataBind();
    }
    //列表事件
    protected void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
    }
    //初始化事件
    protected void Repeater_PreRender(object sender, EventArgs e)
    {

    }
    //查询政策
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Con = Query("");
        Curr = 1;
        PageDataBind();
    }
    //重置数据
    protected void btnClear_Click(object sender, EventArgs e)
    {
        ddlAirCode.Value = "";
        ddlClass.Value = "";
        ddlFromCity.Value = "";
        ddlToCity.Value = "";
        ddlPriceType.SelectedIndex = -1;
        ddlPolicyType.SelectedIndex = -1;
        txtFlightStartDate.Value = "";
        txtFlightEndDate.Value = "";
        txtTicketStartDate.Value = "";
        txtTicketEndDate.Value = "";
    }

    #endregion

    #region 选项卡
    protected void btn1_Click(object sender, EventArgs e)
    {
        Response.Redirect("PolicyQuery.aspx?PageType=1&currentuserid=" + this.currentuserid.Value.ToString());
    }
    protected void btn2_Click(object sender, EventArgs e)
    {
        Response.Redirect("PolicyQuery.aspx?PageType=2&currentuserid=" + this.currentuserid.Value.ToString());
    }
    protected void btn3_Click(object sender, EventArgs e)
    {
        Response.Redirect("PolicyQuery.aspx?PageType=3&currentuserid=" + this.currentuserid.Value.ToString());
    }
    protected void btn4_Click(object sender, EventArgs e)
    {
        Response.Redirect("GroupPolicyQuery.aspx?PageType=4&currentuserid=" + this.currentuserid.Value.ToString());
    }
    #endregion
}