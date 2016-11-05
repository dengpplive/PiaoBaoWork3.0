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
using System.Drawing;
using PbProject.WebCommon.Utility.Encoding;
using PbProject.WebCommon.Utility;
public partial class Policy_PolicyList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            BindEmployee();
            if (!IsPostBack)
            {
                this.currentuserid.Value = this.mUser.id.ToString();
                //if (this.SessionIsNull)
                //{
                //    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面已经失效,请重新登录！','0');", true);
                //    return;
                //}
                //初始化数据
                if (!InitPage())
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面初始化异常');", true);
                    return;
                }
                Hid_LoginName.Value = mUser.LoginName;//供应商登录账号
                Hid_CpyNo.Value = mCompany.UninCode;//公司编号
                Hid_CpyName.Value = mUser.UserName;//供应商名字
                Hid_RoleType.Value = mCompany.RoleType.ToString();//用户角色
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

    private List<Bd_Air_AirPort> list = null;

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
            if (list == null)
            {
                list = GetCity("1");
            }
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
        if (PageType == "1")
        {
            tleName.Text = "普通政策管理";
            lblShow.Text = "普通政策管理";
        }
        else if (PageType == "2")
        {
            tleName.Text = "特价政策管理";
            lblShow.Text = "特价政策管理";
        }
        else if (PageType == "3")
        {
            tleName.Text = "默认政策管理";
            lblShow.Text = "默认政策管理";
        }
        else if (PageType == "5")
        {
            tleName.Text = "团政策管理";
            lblShow.Text = "团政策管理";
        }
    }
    //是否显示中转城市 true 显示 false隐藏
    public bool IsShowMiddle
    {
        get
        {
            return Hid_IsShowMiddle.Value == "0" ? false : true;
        }
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
    /// <summary>
    /// 显示页面文字
    /// </summary>
    /// <param name="type"></param>
    /// <param name="objValue"></param>
    /// <returns></returns>
    public string ShowItem(int type, Object objValue, params object[] paObj)
    {
        string reStrData = "";
        if (type == 0)
        {
            //处理多参数
            if (paObj.Length == 3)
            {
                //适用航班号
                if (objValue.ToString() == "1")
                {
                    if (paObj[0].ToString() == "1")
                    {
                        reStrData = "所有航班号";
                    }
                    else
                    {
                        reStrData = paObj[1].ToString();
                    }
                }
                //排除航班号
                else if (objValue.ToString() == "2")
                {
                    if (paObj[0].ToString() == "1")
                    {
                        reStrData = "";
                    }
                    else
                    {
                        reStrData = paObj[2].ToString(); ;
                    }
                }
            }
        }
        //发布类型
        else if (type == 1)
        {
            reStrData = "出巷";
            if (objValue != null)
            {
                if (objValue.ToString() == "2")
                {
                    reStrData = "入巷";
                }
                else if (objValue.ToString() == "3")
                {
                    reStrData = "全国";
                }
            }
        }
        //行程类型
        else if (type == 2)
        {
            reStrData = "单程";
            if (objValue != null)
            {
                if (objValue.ToString() == "2")
                {
                    reStrData = "单程/往返";
                }
                else if (objValue.ToString() == "3")
                {
                    reStrData = "往返";
                }
                else if (objValue.ToString() == "4")
                {
                    reStrData = "联程";
                }
            }
        } //政策类型
        else if (type == 3)
        {
            reStrData = "B2B";
            if (objValue != null)
            {
                if (objValue.ToString() == "2")
                {
                    reStrData = "BSP";
                }
                else if (objValue.ToString() == "3")
                {
                    reStrData = "B2B/BSP";
                }
            }
        } //审核状态
        else if (type == 4)
        {
            reStrData = "<font style='color:red;'>未审核</font>";
            if (objValue != null)
            {
                if (objValue.ToString() == "1")
                {
                    reStrData = "<font style='color:green;'>已审核</font>";
                }
                else if (objValue.ToString() == "2")
                {
                    reStrData = "<font style='color:red;'>未审核</font>";
                }
            }
        }
        else if (type == 5)
        {
            //是否团队标识
            reStrData = "<font style='color:red;'>普通</font>";
            if (objValue != null)
            {
                if (objValue.ToString() == "1")
                {
                    reStrData = "<font style='color:green;'>团队</font>";
                }
                else if (objValue.ToString() == "0")
                {
                    reStrData = "<font style='color:red;'>普通</font>";
                }
            }
        }
        else if (type == 6)
        {
            //出票类型
            reStrData = "<font style='color:red;'>手动</font>";
            if (objValue != null)
            {
                if (objValue.ToString() == "1")
                {
                    reStrData = "<font style='color:green;'>半自动</font>";
                }
                else if (objValue.ToString() == "2")
                {
                    reStrData = "<font style='color:red;'>自动</font>";
                }
                else
                {
                    reStrData = "<font style='color:red;'>手动</font>";
                }
            }
        }
        else if (type == 7)//票价生成方式
        {
            reStrData = "正常价格";
            if (objValue != null)
            {
                if (objValue.ToString() == "2")
                {
                    reStrData = "动态特价";
                }
                else if (objValue.ToString() == "3")
                {
                    reStrData = "固定特价";
                }
            }
        }
        else if (type == 8)//政策挂起解挂状态
        {
            reStrData = "<font style='color:green;'>未挂</font>";
            if (objValue != null)
            {
                if (objValue.ToString() == "1")
                {
                    reStrData = "<font style='color:red;'>已挂</font>";
                }
            }
        }
        else if (type == 9)//是否高返
        {
            reStrData = "<font style='color:red;'></font>";
            if (objValue != null)
            {
                if (objValue.ToString() == "1" || objValue.ToString().ToLower() == "true")
                {
                    reStrData = "<font style='color:red;'>高返</font>/";
                }
            }
        }
        return reStrData;
    }
    /// <summary>
    /// 跳到编辑页面参数部分
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetUrl(string id, string CpyNo, string CarryCode)
    {
        //带有该查询列表的查询条件
        string url = string.Format("NewAddPolicy.aspx?PageType={0}&id={1}&where={2}&currentuserid={3}", this.PageType, escape(id), HttpUtility.UrlEncode(Con, Encoding.Default), this.currentuserid.Value.ToString());
        if (this.PageType == "3")
        {
            url = string.Format("AddDefaultPolicy.aspx?PageType={0}&id={1}&where={2}&cpyNo={3}&aircode={4}&currentuserid={5}", this.PageType, escape(id), HttpUtility.UrlEncode(Con, Encoding.Default), CpyNo, CarryCode, this.currentuserid.Value.ToString());
        }
        return url;
    }
    #endregion


    public List<Bd_Air_AirPort> GetCity(string IsDomestic)
    {
        return this.baseDataManage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { "IsDomestic=" + IsDomestic }) as List<Bd_Air_AirPort>;
    }
    /// <summary>
    /// 获取城市信息
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public Bd_Air_AirPort GetCityInfo(string data)
    {
        if (list == null)
        {
            list = GetCity("1");
        }
        Bd_Air_AirPort cityinfo = list.Find(delegate(Bd_Air_AirPort item)
        {
            return (!string.IsNullOrEmpty(data) && (item.CityCodeWord.ToUpper() == data.ToUpper() || item.CityName.ToUpper() == data.ToUpper()));
        });

        return cityinfo;
    }
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
    /// <summary>
    /// 获取控制系统权限 
    /// </summary>
    public string KongZhiXiTong
    {
        get
        {
            string result = "";
            if (mCompany != null && mCompany.RoleType > 1)
            {
                result = BaseParams.getParams(supBaseParametersList).KongZhiXiTong;
            }
            return result;
        }
    }
    /// <summary>
    /// 供应控制分销开关 
    /// </summary>
    public string GongYingKongZhiFenXiao
    {
        get
        {
            string result = "";
            if (mCompany != null && mCompany.RoleType > 1)
            {
                result = BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao;
            }
            return result;
        }
    }
    /// <summary>
    /// 高返权限是否开启 true开启 false关闭
    /// </summary>
    public bool IsOpenGF
    {
        get
        {
            bool m_IsOpenGF = false;
            //是否开启高返
            if (GongYingKongZhiFenXiao != null && GongYingKongZhiFenXiao.Contains("|80|"))
            {
                m_IsOpenGF = true;
                Hid_IsOpenGF.Value = "1";
            }
            else
            {
                Hid_IsOpenGF.Value = "0";
                m_IsOpenGF = false;
            }
            return m_IsOpenGF;
        }
    }
    #endregion

    private void PageDataBind()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Tb_Ticket_Policy> list = baseDataManage.CallMethod("Tb_Ticket_Policy", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, OrderBy }) as List<Tb_Ticket_Policy>;
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



    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    public string Query(string IsExpires)
    {
        StringBuilder sbWhere = new StringBuilder();
        sbWhere.Append(string.Format(" CpyNo='{0}' ", mCompany.UninCode));
        if (PageType == "1")
        {
            //普通政策
            sbWhere.Append(" and PolicyKind=1 ");
            if (!rdAuditState.SelectedValue.Equals("0"))
            {
                sbWhere.AppendFormat(" and AuditType={0} ", rdAuditState.SelectedValue);
            }
        }
        else if (PageType == "2")
        {
            //特价政策
            sbWhere.Append(" and PolicyKind=2 ");
            if (!rdAuditState.SelectedValue.Equals("0"))
            {
                sbWhere.AppendFormat(" and AuditType={0} ", rdAuditState.SelectedValue);
            }
        }
        if (PageType == "3")
        {
            //默认政策
            if (ddlPasType.SelectedValue != "" && ddlPasType.SelectedValue != "0")
            {
                sbWhere.AppendFormat(" and A1 ={0} ", ddlPasType.SelectedValue);
            }
            else
            {
                sbWhere.Append(" and A1 in(1,2) ");
            }
        }
        else
        {
            //不是默认政策
            sbWhere.Append(" and A1 =0 ");
        }
        //发布者
        if (SelPublic.Value != "0" && SelPublic.Value != "")
        {
            sbWhere.Append(string.Format(" and CreateLoginName='{0}' ", SelPublic.Text.Trim()));
        }
        //行程类型
        if (rblTravelType.SelectedValue.Trim() != "0")
        {
            sbWhere.Append(string.Format(" and TravelType={0} ", rblTravelType.SelectedValue.Trim()));
        }
        //出票方式
        if (ddlETDZType.SelectedValue != "")
        {
            sbWhere.Append(string.Format(" and AutoPrintFlag={0} ", ddlETDZType.SelectedValue.Trim()));
        }
        if (PageType != "3")//不是默认政策
        {
            //出发城市
            if (txtFromCityName.Value.Trim() != "" && txtFromCityName.Value.Trim() != "中文/英文")
            {
                Bd_Air_AirPort airinfo = GetCityInfo(txtFromCityName.Value.Trim());
                if (airinfo != null)
                {
                    sbWhere.Append(string.Format(" and  StartCityNameCode like '%{0}%'", airinfo.CityCodeWord));
                }
                else
                {
                    if (FromCityCode.Value.Trim() != "")
                    {
                        sbWhere.Append(string.Format(" and  StartCityNameCode like '%{0}%'", FromCityCode.Value.Trim()));
                    }
                }
            }
            if (rblTravelType.SelectedValue.Trim() == "4")
            {
                //中转城市
                if (txtMiddleCityName.Value.Trim() != "" && txtMiddleCityName.Value.Trim() != "中文/英文")
                {
                    Bd_Air_AirPort airinfo = GetCityInfo(txtMiddleCityName.Value.Trim());
                    if (airinfo != null)
                    {
                        sbWhere.Append(string.Format(" and  MiddleCityNameCode like '%{0}%'", airinfo.CityCodeWord));
                    }
                    else
                    {
                        if (MiddleCityCode.Value.Trim() != "")
                        {
                            sbWhere.Append(string.Format(" and  MiddleCityNameCode like '%{0}%'", MiddleCityCode.Value.Trim()));
                        }
                    }
                }
            }
            //到达城市
            if (txtToCityName.Value.Trim() != "" && txtToCityName.Value.Trim() != "中文/英文")
            {
                Bd_Air_AirPort airinfo = GetCityInfo(txtToCityName.Value.Trim());
                if (airinfo != null)
                {
                    sbWhere.Append(string.Format(" and  TargetCityNameCode like '%{0}%'", airinfo.CityCodeWord));
                }
                else
                {
                    if (ToCityCode.Value.Trim() != "")
                    {
                        sbWhere.Append(string.Format(" and  TargetCityNameCode like '%{0}%'", ToCityCode.Value.Trim()));
                    }
                }
            }
        }
        //航空公司
        if (ddlCarry.Value != "" && ddlCarry.Value != "0")
        {
            sbWhere.Append(string.Format(" and  CarryCode like '%{0}%'", ddlCarry.Value));
        }
        if (string.IsNullOrEmpty(IsExpires))
        {
            //乘机日期
            if (!string.IsNullOrEmpty(txtFlightStartDate.Value.Trim()) && !string.IsNullOrEmpty(txtFlightEndDate.Value.Trim()))
            {
                sbWhere.Append(" and FlightStartDate  >='" + txtFlightStartDate.Value.Trim() + " 00:00:00' and FlightEndDate <= '" + txtFlightEndDate.Value.Trim() + " 23:59:59' ");
            }
            //出票日期
            if (!string.IsNullOrEmpty(txtTicketStartDate.Value.Trim()) && !string.IsNullOrEmpty(txtTicketEndDate.Value.Trim()))
            {
                sbWhere.Append(" and PrintStartDate  >='" + txtTicketStartDate.Value.Trim() + " 00:00:00' and PrintEndDate <= '" + txtTicketEndDate.Value.Trim() + " 23:59:59' ");
            }
        }
        else
        {
            //过期政策查看
            sbWhere.Append(" and  (FlightEndDate <= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' and PrintEndDate <= '" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "') ");
        }
        //审核
        return sbWhere.ToString();
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
    #endregion

    //添加政策
    protected void btnInsert_Click(object sender, EventArgs e)
    {
        string url = string.Format("NewAddPolicy.aspx?PageType={0}&currentuserid={1}", PageType, this.currentuserid.Value.ToString());
        if (this.PageType == "3")
        {
            url = string.Format("AddDefaultPolicy.aspx?PageType={0}&cpyNo={1}&aircode=ALL&currentuserid={2}", PageType, mCompany.UninCode, this.currentuserid.Value.ToString());
        }
        else if (this.PageType == "4")
        {
            url = string.Format("AddGroupPolicy.aspx?PageType={0}&currentuserid={1}", PageType,this.currentuserid.Value.ToString(), mCompany.UninCode);
        }
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
        SelPublic.Value = "";
        rblTravelType.SelectedIndex = -1;
        txtFromCityName.Value = "";
        txtMiddleCityName.Value = "";
        txtToCityName.Value = "";
        ddlCarry.Value = "";
        txtFlightStartDate.Value = "";
        txtFlightEndDate.Value = "";
        txtTicketStartDate.Value = "";
        txtTicketEndDate.Value = "";
        FromCityCode.Value = "";
        MiddleCityCode.Value = "";
        ToCityCode.Value = "";

        rdAuditState.SelectedIndex = -1;
        ddlPolicyType.SelectedIndex = -1;
        ddlPasType.SelectedIndex = -1;
        ddlETDZType.SelectedIndex = -1;
    }
    //过期政策查看
    protected void btnExpiresQuery_Click(object sender, EventArgs e)
    {
        Con = Query("1");
        Curr = 1;
        PageDataBind();
    }

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