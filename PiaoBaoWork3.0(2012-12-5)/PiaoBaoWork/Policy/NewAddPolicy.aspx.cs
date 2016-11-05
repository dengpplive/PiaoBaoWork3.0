using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;
using System.Collections;
using System.Text;
using PbProject.WebCommon.Utility.Encoding;
using PbProject.WebCommon.Utility;
public partial class Policy_NewAddPolicy : BasePage
{
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

    protected override void OnInit(System.EventArgs e)
    {
        BindCityList();
        GetAirCode();
        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            currentuserid.Value = this.mUser.id.ToString();
            LoadSetParam();
        }
        Hid_LoginName.Value = mUser.LoginName;//供应商登录账号
        Hid_CpyNo.Value = mCompany.UninCode;//公司编号
        Hid_CpyName.Value = mUser.UserName;//供应商名字

        //是否开启高返
        if (KongZhiXiTong != null && KongZhiXiTong.Contains("|80|"))
        {
            Hid_IsOpenGF.Value = "1";
        }
        else
        {
            Hid_IsOpenGF.Value = "0";
        }
    }
    public void showText(string PageType, string IsEdit)
    {
        //发布页面类型 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊仓位政策
        if (PageType == "1")
        {
            lblShow.Text = "普通政策发布";
            //1编辑 0添加
            if (IsEdit == "1")
            {
                lblShow.Text = "普通政策编辑";
            }

        }
        else if (PageType == "2")
        {
            lblShow.Text = "特价政策发布";
            //1编辑 0添加
            if (IsEdit == "1")
            {
                lblShow.Text = "特价政策编辑";
            }
        }
        else if (PageType == "3")
        {
            lblShow.Text = "默认政策发布";
            //1编辑 0添加
            if (IsEdit == "1")
            {
                lblShow.Text = "默认政策编辑";
            }
        }
        else if (PageType == "5")
        {

        }
    }
    /// <summary>
    /// 加载页面类型
    /// </summary>
    public void LoadSetParam()
    {
        //政策种类 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊仓位政策
        if (Request["PageType"] != null && Request["PageType"].ToString() != "")
        {
            //政策种类 1.普通，2.特价
            string strPolicyKind = Request["PageType"].ToString();
            if (strPolicyKind == "1" || strPolicyKind == "2")
            {
                if (GetVal("id", "") != "")
                {
                    //按钮
                    addAndNext.Value = "保存";
                    //来源---------------
                    string id = GetVal("id", "");
                    Hid_id.Value = id;//列表中的数据id
                    string currPage = GetVal("currPage", "1");
                    Hid_currPage.Value = currPage;//来自列表第几页
                    Hid_where.Value = Request["where"].ToString();
                    //来源---------------
                    Tb_Ticket_Policy PTPolicy = this.baseDataManage.CallMethod("Tb_Ticket_Policy", "GetById", null, new object[] { id }) as Tb_Ticket_Policy;
                    if (PTPolicy != null)
                    {
                        Hid_IsEdit.Value = "1";//编辑
                        string formatData = JsonHelper.ObjToJson<Tb_Ticket_Policy>(PTPolicy);
                        Hid_EditData.Value = escape(formatData);
                    }
                }
                else
                {
                    //日期
                    SetDefaultDate();
                }
            }
            Hid_PolicyKind.Value = Request["PageType"].ToString();
            Hid_PageType.Value = Request["PageType"].ToString();
        }
        else
        {
            //日期
            SetDefaultDate();
        }
        //页面类型
        PageType = Hid_PageType.Value;
        showText(PageType, Hid_IsEdit.Value);
    }
    /// <summary>
    /// 获取城市列表
    /// </summary>
    /// <param name="IsDomestic">是否国内 1.是，2.否</param>
    /// <returns></returns>
    public List<Bd_Air_AirPort> GetCity(string IsDomestic)
    {
        List<Bd_Air_AirPort> list = this.baseDataManage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { "IsDomestic=" + IsDomestic }) as List<Bd_Air_AirPort>;
        if (list != null && list.Count > 0)
        {
            string cityData = JsonHelper.ObjToJson<List<Bd_Air_AirPort>>(list);
            Hid_InnerCityData.Value = escape(cityData);
        }
        return list;
    }
    //航空公司信息
    public void GetAirCode()
    {
        List<Bd_Air_Carrier> list = this.baseDataManage.CallMethod("Bd_Air_Carrier", "GetList", null, new object[] { "" }) as List<Bd_Air_Carrier>;
        StringBuilder sbValue = new StringBuilder();
        foreach (Bd_Air_Carrier item in list)
        {
            sbValue.AppendFormat("{0}^{1}^{2}^{3}|", item.AirName.Trim(), item.Code.ToUpper().Trim(), item.ShortName.Trim(), item.SettleCode.Trim());
        }
        Hid_AirCodeCache.Value = escape(sbValue.ToString());
    }
    /// <summary>
    /// 设置发布页面默认日期
    /// </summary>
    public void SetDefaultDate()
    {
        DateTime dt = DateTime.Now;
        DateTime dt1 = new DateTime(dt.Year, dt.Month, 1);

        //设置日期默认值
        txtTicketStartDate.Value = dt.ToString("yyyy-MM-dd");
        txtTicketEndDate.Value = dt1.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
        txtTicketStartDate_0.Value = dt.ToString("yyyy-MM-dd");
        txtTicketEndDate_0.Value = dt1.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

        txtFlightStartDate.Value = dt.ToString("yyyy-MM-dd");
        txtFlightEndDate.Value = dt1.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
        txtFlightStartDate_0.Value = dt.ToString("yyyy-MM-dd");
        txtFlightEndDate_0.Value = dt1.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
    }
    //绑定城市控件数据
    public void BindCityList()
    {
        List<Bd_Air_AirPort> cityList = GetCity("1");
        //出发城市
        From_RightBox.Items.Clear();
        //中转城市
        Middle_RightBox.Items.Clear();
        //到达城市       
        To_RightBox.Items.Clear();
        //排序 
        SortedList sortLst = new SortedList();
        foreach (Bd_Air_AirPort city in cityList)
        {
            ListItem item = new ListItem();
            item.Text = city.CityCodeWord + "_" + city.CityName;
            item.Value = city.CityCodeWord;
            if (!sortLst.ContainsKey(city.CityCodeWord))
            {
                sortLst.Add(city.CityCodeWord, item);
            }
        }
        ListItem[] newItem = new ListItem[sortLst.Values.Count];
        sortLst.Values.CopyTo(newItem, 0);
        Middle_RightBox.Items.AddRange(newItem);
        From_RightBox.Items.AddRange(newItem);
        To_RightBox.Items.AddRange(newItem);
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
    /// 页面类型  1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊仓位政策
    /// </summary>
    public string PageType
    {
        get { return (string)ViewState["PageType"]; }
        set { ViewState["PageType"] = value; Hid_PageType.Value = value; }
    }
}