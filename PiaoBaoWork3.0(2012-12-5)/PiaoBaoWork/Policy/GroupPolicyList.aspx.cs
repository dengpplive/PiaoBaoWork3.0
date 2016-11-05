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
public partial class Policy_GroupPolicyList : BasePage
{
    ////发布页面类型 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊仓位政策
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
            BindEmployee();
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
                if (Request["edit"] != null && Request["edit"].ToString() != "")
                {
                    //编辑返回
                    //查询条件
                    Con = GetVal("where", " 1=1 ");
                    //操作的分页索引
                    Curr = int.Parse(GetVal("pid", "1"));
                    //返回的当前列表id
                    Hid_CurrId.Value = GetVal("cid", "0");
                    if (Con != " 1=1 ")
                    {
                        //绑定
                        PageDataBind();
                    }
                    scriptText.Text = "<script language='javascript' type='text/javascript'>SetCurrSelStyle('" + Hid_CurrId.Value + "');</script>";
                }
                else
                {
                    Curr = 1;
                    Con = " 1=1";
                }
            }
        }
        catch (Exception)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面初始化异常!');", true);
        }
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
        //发布页面类型 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊仓位政策
        if (PageType == "4")
        {
            tleName.Text = "散冲团政策列表";
            lblShow.Text = "散冲团政策列表";
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
    public string ShowItem(int type, string strData, string strZK, string strPrice)
    {
        string reData = "";
        //起飞抵达时间
        if (type == 1)
        {
            if (!string.IsNullOrEmpty(strData) && strData.Split('-').Length == 2)
            {
                reData = strData.Split('-')[0].Substring(0, 5) + "-" + strData.Split('-')[1].Substring(0, 5);
            }
        }
        else if (type == 2)//政策类型
        {
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
        else if (type == 3)//优惠方式
        {
            if (strData == "1" || strData.ToString().ToLower() == "true")
            {
                reData = strPrice;
            }
            else
            {
                reData = strZK;
            }
        }
        return reData;
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
    /// 页面类型  1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊仓位政策
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
    /// <summary>
    /// 绑定公司所有人员账号
    /// </summary>
    public void BindEmployee()
    {
        string sqlWhere = string.Format(" CpyNo='{0}' ", mCompany.UninCode);
        List<User_Employees> empList = baseDataManage.CallMethod("User_Employees", "GetList", null, new object[] { sqlWhere }) as List<User_Employees>;
        SelPublic.DataSource = empList;
        SelPublic.DataFiledText = "LoginName";
        SelPublic.DataFiledValue = "id";
        SelPublic.DataBind();
    }
    //绑定仓位
    public void BindClass(string SelClass)
    {
        string strChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        List<char> strList = new List<char>();
        strList.AddRange(strChar.ToCharArray());
        ddlClass.DataSource = strList;
        ddlClass.DataBind();
    }
    /// <summary>
    /// 获取请求值
    /// </summary>
    /// <param name="Name">请求key名称</param>
    /// <param name="DefaultVal">数据默认值</param>
    /// <returns></returns>
    public string GetVal(string Name, string DefaultVal)
    {
        if (Request[Name] != null && Request[Name].ToString() != "")
        {
            DefaultVal = HttpUtility.UrlDecode(Request[Name].ToString(), Encoding.Default);
        }
        return DefaultVal;
    }
    /// <summary>
    /// 跳到编辑页面参数部分
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetUrl(string id, string CpyNo)
    {
        //带有该查询列表的查询条件
        string url = string.Format("AddGroupPolicy.aspx?PageType={0}&id={1}&where={2}&currentuserid={3}", this.PageType, escape(id), HttpUtility.UrlEncode(Con, Encoding.Default), this.currentuserid.Value.ToString());
        return url;
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
        StringBuilder sbWhere = new StringBuilder();
        sbWhere.Append(string.Format(" CpyNo='{0}' ", mCompany.UninCode));
        //发布者
        if (SelPublic.Value != "" && SelPublic.Value != "0")
        {
            sbWhere.AppendFormat(" and OperLoginName = '{0}' ", SelPublic.Value);
        }
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
        //仓位
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
        if (string.IsNullOrEmpty(IsExpires))
        {
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
        }
        else
        {
            //过期政策查看
            sbWhere.Append(" and  (FlightEndDate <= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' and PrintEndDate <= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "') ");
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
    //添加政策
    protected void btnInsert_Click(object sender, EventArgs e)
    {
        string url = string.Format("AddGroupPolicy.aspx?PageType={0}&currentuserid={1}", PageType,this.currentuserid.Value.ToString(), mCompany.UninCode);
        Response.Redirect(url);
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
        SelPublic.Value = "";
        txtFlightStartDate.Value = "";
        txtFlightEndDate.Value = "";
        txtTicketStartDate.Value = "";
        txtTicketEndDate.Value = "";
    }
    //过期政策查看
    protected void btnExpiresQuery_Click(object sender, EventArgs e)
    {
        Con = Query("1");
        Curr = 1;
        PageDataBind();
    }
    #endregion

    #region 选项卡
    protected void btn1_Click(object sender, EventArgs e)
    {
        Response.Redirect("PolicyList.aspx?PageType=1&currentuserid=" + this.currentuserid.Value.ToString());
    }
    protected void btn2_Click(object sender, EventArgs e)
    {
        Response.Redirect("PolicyList.aspx?PageType=2&currentuserid=" + this.currentuserid.Value.ToString());
    }
    protected void btn3_Click(object sender, EventArgs e)
    {
        Response.Redirect("PolicyList.aspx?PageType=3&currentuserid=" + this.currentuserid.Value.ToString());
    }
    protected void btn4_Click(object sender, EventArgs e)
    {
        Response.Redirect("GroupPolicyList.aspx?PageType=4&currentuserid=" + this.currentuserid.Value.ToString());
    }
    #endregion
}